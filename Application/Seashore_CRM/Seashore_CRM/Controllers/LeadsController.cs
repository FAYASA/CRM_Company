using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.Models.DTOs;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using seashore_CRM.Models.Entities;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.Services.Service_Interfaces;
using System.Text.Json;

namespace Seashore_CRM.Controllers
{
    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;
        private readonly IActivityService _activityService;

        // Mapping from lead status name to suggested activities
        private static readonly Dictionary<string, string[]> StatusActivities = new()
        {
            { "Discover", new[] { "Interested", "Not Interested", "Average" } },
            { "Quote Given", new[] { "Follow up", "Very Important", "Site visit", "Submit Sample" } },
            { "Follow Up", new[] { "Site Visited", "Tele calling", "Price high", "Need more clarification", "Waiting for approval", "Expecting PO" } },
            { "Forecast", new[] { "Quote confirmed", "Partially confirmed" } },
            { "PO Received", new[] { "Delivery completed", "Delivery pending", "Partially delivered" } },
            { "Invoicing", new[] { "Invoiced", "Note Invoiced", "Partially Invoiced" } },
            { "Order Closed", new[] { "Payment completed", "Payment pending", "Partially payment received" } },
            { "Order Lost", new[] { "Lost to Competition Reason Unknown", "Lost to Competition Reason known", "Client Dropped the plan Higher Price", "Client Dropped the plan Delivery Distance" } }
        };

        public LeadsController(ILeadService leadService, IUnitOfWork uow, IWebHostEnvironment env, IActivityService activityService)
        {
            _leadService = leadService;
            _uow = uow;
            _env = env;
            _activityService = activityService;
        }

        public async Task<IActionResult> Index()
        {
            var leads = await _leadService.GetAllLeadsAsync();
            return View(leads.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();

            // suggested activities from mapping
            if (!string.IsNullOrEmpty(lead.StatusName) && StatusActivities.TryGetValue(lead.StatusName, out var acts))
            {
                ViewBag.SuggestedActivities = acts.ToList();
            }
            else
            {
                ViewBag.SuggestedActivities = new List<string>();
            }

            // load recent activities for this lead
            var activities = (await _uow.Activities.FindAsync(a => a.LeadId == id)).OrderByDescending(a => a.ActivityDate).ToList();
            ViewBag.Activities = activities;

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(int leadId, string activityType)
        {
            if (string.IsNullOrWhiteSpace(activityType)) return BadRequest();

            var act = new Activity
            {
                LeadId = leadId,
                ActivityType = activityType,
                Description = null,
                ActivityDate = System.DateTime.UtcNow
            };

            await _activityService.AddAsync(act);
            return RedirectToAction(nameof(Details), new { id = leadId });
        }

        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View(new LeadDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadDto lead)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(lead);
                return View(lead);
            }

            // Handle file uploads
            var files = Request.Form.Files;
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);
            var savedFiles = new List<string>();
            foreach (var f in files)
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(f.FileName);
                var path = Path.Combine(uploadFolder, fileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await f.CopyToAsync(stream);
                }
                savedFiles.Add(fileName);
            }
            if (savedFiles.Any())
            {
                lead.AttachmentsJson = System.Text.Json.JsonSerializer.Serialize(savedFiles);
            }

            // If product items include free-entry products (no ProductId) create Product records
            if (lead.ProductItems != null)
            {
                foreach (var pi in lead.ProductItems.Where(x => !x.ProductId.HasValue && !string.IsNullOrEmpty(x.ProductName)))
                {
                    var prod = new Product
                    {
                        ProductName = pi.ProductName,
                        UnitPrice = pi.UnitPrice,
                        TaxPercentage = pi.TaxPercentage,
                        IsActive = true
                    };
                    await _uow.Products.AddAsync(prod);
                    await _uow.CommitAsync();
                    // set created product id back on dto
                    pi.ProductId = prod.Id;
                }
            }

            // Create Lead and get its Id
            var leadId = await _leadService.AddLeadAsync(lead);

            // Persist LeadItem rows for each product item
            if (lead.ProductItems != null && lead.ProductItems.Any())
            {
                foreach (var pi in lead.ProductItems.Where(x => x.ProductId.HasValue))
                {
                    var lineTotal = pi.Quantity * pi.UnitPrice * (1 + (pi.TaxPercentage / 100M));
                    var li = new LeadItem
                    {
                        LeadId = leadId,
                        ProductId = pi.ProductId.Value,
                        Quantity = pi.Quantity,
                        UnitPrice = pi.UnitPrice,
                        TaxPercentage = pi.TaxPercentage,
                        LineTotal = lineTotal
                    };
                    await _uow.LeadItems.AddAsync(li);
                }

                await _uow.CommitAsync();
            }

            // Persist any selected activities from create form
            var selectedActivities = Request.Form["SelectedActivities"].ToList();
            if (selectedActivities != null && selectedActivities.Any())
            {
                foreach (var at in selectedActivities)
                {
                    if (string.IsNullOrWhiteSpace(at)) continue;
                    var a = new Activity
                    {
                        LeadId = leadId,
                        ActivityType = at,
                        ActivityDate = System.DateTime.UtcNow
                    };
                    await _uow.Activities.AddAsync(a);
                }
                await _uow.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();
            await PopulateSelectListsAsync(lead);
            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeadDto lead)
        {
            if (id != lead.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(lead);
                return View(lead);
            }

            await _leadService.UpdateLeadAsync(lead);

            // Persist any selected activities from edit form
            var selectedActivities = Request.Form["SelectedActivities"].ToList();
            if (selectedActivities != null && selectedActivities.Any())
            {
                foreach (var at in selectedActivities)
                {
                    if (string.IsNullOrWhiteSpace(at)) continue;
                    var a = new Activity
                    {
                        LeadId = lead.Id,
                        ActivityType = at,
                        ActivityDate = System.DateTime.UtcNow
                    };
                    await _uow.Activities.AddAsync(a);
                }
                await _uow.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();
            return View(lead);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _leadService.DeleteLeadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Qualify(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();
            await PopulateSelectListsAsync(lead);
            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Qualify(LeadDto lead)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(lead);
                return View(lead);
            }

            await _leadService.QualifyLeadAsync(lead);
            return RedirectToAction("Details", new { id = lead.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convert(int id)
        {
            var oppId = await _leadService.ConvertToOpportunityAsync(id);
            if (oppId.HasValue) return RedirectToAction("Details", "Opportunities", new { id = oppId.Value });
            return RedirectToAction("Details", new { id });
        }

        private async Task PopulateSelectListsAsync(LeadDto? model = null)
        {
            var companies = await _uow.Companies.GetAllAsync();
            var contacts = await _uow.Contacts.GetAllAsync();
            var sources = await _uow.LeadSources.GetAllAsync();
            var statuses = await _uow.LeadStatuses.GetAllAsync();
            var users = await _uow.Users.GetAllAsync();
            var products = await _uow.Products.GetAllAsync();

            ViewBag.Companies = new SelectList(companies, "Id", "CompanyName", model?.CompanyId);
            ViewBag.Contacts = new SelectList(contacts, "Id", "FirstName", model?.ContactId);
            ViewBag.Sources = new SelectList(sources, "Id", "SourceName", model?.SourceId);
            ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName", model?.StatusId);
            ViewBag.Users = new SelectList(users, "Id", "FullName", model?.AssignedUserId);
            ViewBag.ProductList = products.Select(p => new SelectListItem(p.ProductName, p.Id.ToString())).ToList();

            // expose mapping to client as JSON (status name -> activities array)
            ViewBag.StatusActivitiesJson = JsonSerializer.Serialize(StatusActivities);
        }
    }
}