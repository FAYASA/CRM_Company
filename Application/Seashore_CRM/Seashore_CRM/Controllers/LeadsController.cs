using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.ApplicationLayer.DTOs;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using seashore_CRM.Models.Entities;
using Seashore_CRM.ViewModels.Lead;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public partial class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IUnitOfWork _uow;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IActivityService _activityService;
        private readonly ILeadStatusActivityService _leadStatusActivityService;
        private readonly IProductService _productService;

        public LeadsController(ILeadService leadService, IUnitOfWork uow, AppDbContext db, IWebHostEnvironment env, IActivityService activityService,
            ILeadStatusActivityService leadStatusActivityService, IProductService productService)
        {
            _leadService = leadService;
            _uow = uow;
            _db = db;
            _env = env;
            _activityService = activityService;
            _leadStatusActivityService = leadStatusActivityService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? q, int? status, int? assigned, int? category, int page = 1, int pageSize = 20)
        {
            var allLeads = (await _leadService.GetAllLeadsAsync()).ToList();

            // Basic filtering in memory for now (consider pushing to repository for large datasets)
            if (!string.IsNullOrWhiteSpace(q))
            {
                allLeads = allLeads.Where(l => (l.ProductNames != null && l.ProductNames.Any() && l.ProductNames.Any(p => p.Contains(q, System.StringComparison.OrdinalIgnoreCase)))
                                            || (!string.IsNullOrWhiteSpace(l.AssignedUserName) && l.AssignedUserName.Contains(q, System.StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (status.HasValue)
            {
                allLeads = allLeads.Where(l => l.StatusId == status.Value).ToList();
            }

            if (assigned.HasValue)
            {
                allLeads = allLeads.Where(l => l.AssignedUserId == assigned.Value).ToList();
            }

            if (category.HasValue)
            {
                allLeads = allLeads.Where(l => l.CategoryId == category.Value).ToList();
            }

            var total = allLeads.Count;
            var totalPages = (int)System.Math.Ceiling(total / (double)pageSize);

            var items = allLeads.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var vm = new LeadListViewModel
            {
                Leads = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages,
                Query = q,
                SelectedStatusId = status,
                SelectedAssignedId = assigned,
                SelectedCategoryId = category,
                Statuses = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _uow.LeadStatuses.GetAllAsync(), "Id", "StatusName", status),
                Users = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_uow.Users.GetAllAsync().ToList(), "Id", "FullName", assigned),
                Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _uow.Categories.GetAllAsync(), "Id", "CategoryName", category)
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();

            // suggested activities from mapping (load from DB only)
            var mapping = await GetStatusActivitiesAsync();
            if (!string.IsNullOrEmpty(lead.StatusName) && mapping.TryGetValue(lead.StatusName, out var acts))
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

            // load comments
            var comments = (await _uow.Comments.FindAsync(c => c.LeadId == id)).OrderByDescending(c => c.CreatedDate).ToList();
            ViewBag.Comments = comments;

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

        // Allows adding a new activity name for a specific status (persist in DB)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStatusActivity(string statusName, string activityName)
        {
            if (string.IsNullOrWhiteSpace(statusName) || string.IsNullOrWhiteSpace(activityName))
                return BadRequest("statusName and activityName are required");

            // Find the status in DB
            var status = (await _uow.LeadStatuses.FindAsync(s => s.StatusName == statusName)).FirstOrDefault();
            if (status == null)
            {
                // create new status if not exists
                status = new LeadStatus { StatusName = statusName };
                await _uow.LeadStatuses.AddAsync(status);
                await _uow.CommitAsync();
            }

            // check if activity already exists for this status
            var existing = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == status.Id && a.ActivityName == activityName)).FirstOrDefault();
            if (existing == null)
            {
                var act = new LeadStatusActivity
                {
                    LeadStatusId = status.Id,
                    ActivityName = activityName
                };
                await _uow.LeadStatusActivities.AddAsync(act);
                await _uow.CommitAsync();
            }

            // Return current mapping (build from DB only)
            var mapping = new Dictionary<string, string[]>();
            var statuses = await _uow.LeadStatuses.GetAllAsync();
            foreach (var st in statuses)
            {
                var acts = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == st.Id)).Select(a => a.ActivityName).ToArray();
                if (acts.Length > 0)
                    mapping[st.StatusName] = acts;
            }

            return Json(new { success = true, mapping = mapping });
        }

        public async Task<IActionResult> Create()
        {
            var vm = await BuildLeadCreateViewModel();
            vm.Mode = "create";
            vm.SubmitButtonText = "Save Lead";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadCreateViewModel vm)
        {
            var lead = vm?.Lead ?? new LeadDto();

            // if rowversion sent as base64 hidden input convert back
            var rv = Request.Form["Lead.RowVersion"].FirstOrDefault();
            if (!string.IsNullOrEmpty(rv))
            {
                try { lead.RowVersion = System.Convert.FromBase64String(rv); } catch {  }
            }

            if (!ModelState.IsValid)
            {
                var newVm = await BuildLeadCreateViewModel(lead);
                newVm.Mode = "create";
                newVm.SubmitButtonText = "Save Lead";
                return View(newVm);
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

            // Parse and persist any UserLeadRights submitted in the form
            var rights = new List<UserLeadRightsViewModel>();
            if (Request.Form.Keys.Any(k => k.StartsWith("UserLeadRights[")))
            {
                try
                {
                    var groups = new Dictionary<int, Dictionary<string, string>>();
                    var rightsKeys = Request.Form.Keys.Where(k => k.StartsWith("UserLeadRights[")).ToList();
                    foreach (var key in rightsKeys)
                    {
                        var after = key.Substring("UserLeadRights[".Length);
                        var idxStr = after.Substring(0, after.IndexOf(']'));
                        if (!int.TryParse(idxStr, out var idx)) continue;
                        var prop = after.Substring(after.IndexOf(']') + 2);
                        var val = Request.Form[key].FirstOrDefault();
                        if (!groups.ContainsKey(idx)) groups[idx] = new Dictionary<string, string>();
                        groups[idx][prop] = val ?? string.Empty;
                    }

                    foreach (var g in groups.OrderBy(x => x.Key))
                    {
                        var dict = g.Value;
                        if (!dict.TryGetValue("UserId", out var userIdStr)) continue;
                        if (!int.TryParse(userIdStr, out var userId)) continue;
                        var canView = dict.TryGetValue("CanView", out var cv) && (cv == "True" || cv == "true" || cv == "on");
                        var canEdit = dict.TryGetValue("CanEdit", out var ce) && (ce == "True" || ce == "true" || ce == "on");
                        var id = 0;
                        if (dict.TryGetValue("Id", out var idStr) && int.TryParse(idStr, out var parsedId)) id = parsedId;

                        rights.Add(new UserLeadRightsViewModel
                        {
                            Id = id,
                            UserId = userId,
                            LeadId = 0, // will set after lead created
                            CanView = canView,
                            CanEdit = canEdit
                        });
                    }

                    // persist rights after lead created
                    foreach (var rvm in rights)
                    {
                        if (rvm.Id > 0)
                        {
                            var existing = (await _uow.UserLeadRights.GetByIdAsync(rvm.Id));
                            if (existing != null)
                            {
                                existing.UserId = rvm.UserId;
                                existing.CanView = rvm.CanView;
                                existing.CanEdit = rvm.CanEdit;
                                _uow.UserLeadRights.Update(existing);
                            }
                        }
                        else
                        {
                            var entity = new UserLeadRights
                            {
                                LeadId = leadId,
                                UserId = rvm.UserId,
                                CanView = rvm.CanView,
                                CanEdit = rvm.CanEdit
                            };
                            await _uow.UserLeadRights.AddAsync(entity);
                        }
                    }

                    await _uow.CommitAsync();
                }
                catch
                {
                    // swallow for now; if persistence fails we'll continue — consider logging and surface model errors
                }
            }

            // NOTE: Product items and selected activities are persisted inside _leadService.AddLeadAsync.
            // Removing duplicate persistence here to avoid creating duplicate LeadItem/Activity rows.

            // Persist comments submitted in the form
            var commentsText = Request.Form["Comments"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(commentsText))
            {
                var comment = new Comment
                {
                    LeadId = leadId,
                    CommentText = commentsText,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                };
                await _uow.Comments.AddAsync(comment);
                await _uow.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }






        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();
            var vm = await BuildLeadEditViewModel(lead);
            // Use Create view to render the shared form, but VM has Mode="edit"
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeadEditViewModel vm)
        {
            var lead = vm?.Lead ?? new LeadDto();

            // if rowversion sent as base64 hidden input convert back
            var rv = Request.Form["Lead.RowVersion"].FirstOrDefault();
            if (!string.IsNullOrEmpty(rv))
            {
                try { lead.RowVersion = System.Convert.FromBase64String(rv); } catch { /* ignore invalid */ }
            }

            if (id != lead.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var newVm = await BuildLeadEditViewModel(lead);
                return View("Create", newVm);
            }

            try
            {
                await _leadService.UpdateLeadAsync(lead);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var newVm = await BuildLeadEditViewModel(lead);
                return View("Create", newVm);
            }

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

            var oppId = await _leadService.QualifyLeadAsync(lead);

            // If this is an AJAX request (X-Requested-With header) return JSON for client-side handling
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isAjax)
            {
                return Json(new { success = true, opportunityId = oppId });
            }

            if (oppId.HasValue)
            {
                // Redirect to newly created opportunity details when conversion occurred
                return RedirectToAction("Details", "Opportunities", new { id = oppId.Value });
            }

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

        //[HttpGet]
        //public async Task<IActionResult> GetContactsByCompany(int companyId)
        //{
        //    var contacts = await _leadService.GetLeadByIdAsync(companyId);

        //    var result = contacts;

        //    return Json(result);
        //}

        // returns contacts for the provided company id
        [HttpGet]
        public async Task<IActionResult> ContactsByCompany(int companyId)
        {
            if (companyId <= 0) return Json(new List<object>());

            // Use repository to fetch contacts by company id
            var contacts = await _uow.Contacts.GetByCompanyIdAsync(companyId);

            var result = contacts.Select(c => new {
                id = c.Id,
                name = !string.IsNullOrWhiteSpace(c.ContactName) ? c.ContactName : (c.Email ?? c.Mobile ?? "(no name)")
            }).ToList();

            return Json(result);
        }

        // Return Product Groups for the selected Category
        [HttpGet]
        public async Task<JsonResult> ProductGroupsByCategory(int categoryId)
        {
            if (categoryId <= 0)
                return Json(new List<object>());

            var groups = await _uow.ProductGroups.GetByCategoryIdAsync(categoryId);

            var result = groups.Select(g => new
            {
                id = g.Id,
                name = g.GroupName
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ActivitiesByStatus(int statusId)
        {
            if (statusId <= 0)
                return Json(new List<object>());

            var activities = await _uow.LeadStatusActivities
                .FindAsync(a => a.LeadStatusId == statusId);

            var result = activities.Select(a => new
            {
                id = a.Id,
                name = a.ActivityName
            }).ToList();

            return Json(result);
        }

        private async Task PopulateSelectListsAsync(LeadDto? model = null)
        {
            var companiesQueryable = _uow.Companies.GetAllAsync(); 
            var companies = await companiesQueryable.ToListAsync();

            var contactsEn = await _uow.Contacts.GetByCompanyIdAsync(model?.CompanyId);
            var contacts = contactsEn.ToList();

            var SAvtivitiesEn = await _uow.LeadStatusActivities.GetByIdAsync(model?.StatusId);

            var SActivities = new List<LeadStatusActivity>();
            if (SAvtivitiesEn != null)
            {
                SActivities.Add(SAvtivitiesEn);
            }

            var IndcontactsQueryable = _uow.Contacts.GetAllIndAsync();
            var Indcontacts = await IndcontactsQueryable.ToListAsync();

            var sources = await _uow.LeadSources.GetAllAsync();
            var statuses = await _uow.LeadStatuses.GetAllAsync();

            var usersQueryable = _uow.Users.GetAllAsync(); 
            var users = await usersQueryable.ToListAsync();

            var products =  _uow.Products.GetAllAsync(); 
            var categories = await _uow.Categories.GetAllAsync();
            var pro_Groups = await _uow.ProductGroups.GetByCategoryIdAsync(model?.CategoryId);
            var pro_GroupsEN = pro_Groups.ToList();



            ViewBag.Companies = new SelectList(companies, "Id", "CompanyName", model?.CompanyId);
            ViewBag.Contacts = new SelectList(contacts, "Id", "ContactName", model?.ContactId);
            ViewBag.ContactForIndv = new SelectList(Indcontacts, "Id", "ContactName", model?.ContactId);   
            ViewBag.Sources = new SelectList(sources, "Id", "SourceName", model?.SourceId);
            ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName", model?.StatusId);
            ViewBag.StatusActivities = new SelectList(SActivities, "Id", "ActivityName", model?.ActivityId);
            ViewBag.Users = new SelectList(users, "Id", "FullName", model?.AssignedUserId);
            ViewBag.ProductList = products.Select(p => new SelectListItem(p.ProductName, p.Id.ToString())).ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewBag.Pro_Groups = new SelectList(pro_GroupsEN, "Id", "GroupName");

            // Expose product metadata to client as JSON for auto-fill (unit price, cost, tax, category)
            var prodMap = products.Select(p => new {
                id = p.Id,
                name = p.ProductName,
                cost = p.Cost,
                tax = p.TaxPercentage,
                categoryId = p.CategoryId
            }).ToDictionary(x => x.id.ToString(), x => x);
            ViewBag.ProductsJson = JsonSerializer.Serialize(prodMap);

            // Simple comment templates for quick selection in UI
            var commentTemplates = new List<string>
            {
                "Need more information",
                "Sent quote",
                "Followed up",
                "Client requested sample",
                "Waiting for approval"
            };
            ViewBag.CommentTemplates = new SelectList(commentTemplates);

            // expose mapping to client as JSON (status name -> activities array). Load from DB only
            var mapping = await GetStatusActivitiesAsync();
            ViewBag.StatusActivitiesJson = JsonSerializer.Serialize(mapping);
        }

        // Build mapping from DB only (do not fallback to file or defaults)
        private async Task<Dictionary<string, string[]>> GetStatusActivitiesAsync()
        {
            var mapping = new Dictionary<string, string[]>();
            var statuses = (await _uow.LeadStatuses.GetAllAsync()).ToList();
            if (statuses.Any())
            {
                foreach (var st in statuses)
                {
                    var acts = (await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == st.Id)).Select(a => a.ActivityName).ToArray();
                    if (acts.Length > 0) mapping[st.StatusName] = acts;
                }

                return mapping;
            }

            // If there are no statuses in DB return an empty mapping (client will show no suggestions)
            return mapping;
        }

        private async Task SaveStatusActivitiesAsync(Dictionary<string, string[]> mapping)
        {
            try
            {
                var dataDir = Path.Combine(_env.WebRootPath ?? "", "data");
                Directory.CreateDirectory(dataDir);
                var file = Path.Combine(dataDir, "status_activities.json");
                var json = JsonSerializer.Serialize(mapping);
                await System.IO.File.WriteAllTextAsync(file, json);
            }
            catch
            {
                // swallow for now
            }
        }

        private async Task<LeadCreateViewModel> BuildLeadCreateViewModel(LeadDto? model = null)
        {
            var vm = new LeadCreateViewModel();

            var companiesQueryable = _uow.Companies.GetAllAsync();
            var companies = await companiesQueryable.ToListAsync();

            var contactsEn = await _uow.Contacts.GetByCompanyIdAsync(model?.CompanyId);
            var contacts = contactsEn.ToList();

            var SAvtivitiesEn = await _uow.LeadStatusActivities.GetByIdAsync(model?.StatusId);

            var SActivities = new List<LeadStatusActivity>();
            if (SAvtivitiesEn != null)
            {
                SActivities.Add(SAvtivitiesEn);
            }

            var IndcontactsQueryable = _uow.Contacts.GetAllIndAsync();
            var Indcontacts = await IndcontactsQueryable.ToListAsync();

            var sources = await _uow.LeadSources.GetAllAsync();
            var statuses = await _uow.LeadStatuses.GetAllAsync();

            var usersQueryable = _uow.Users.GetAllAsync();
            var users = await usersQueryable.ToListAsync();

            var productsQueryable = _uow.Products.GetAllAsync();
            var products = await productsQueryable.ToListAsync(); 
            var categories = await _uow.Categories.GetAllAsync();
            var categoriesList = categories.ToList();
            var categoriesDict = categoriesList.ToDictionary(c => c.Id, c => c.CategoryName);
            var allProductGroups = await _uow.ProductGroups.GetAllAsync();
            var productGroupsList = allProductGroups.ToList();
            var groupsDict = productGroupsList.ToDictionary(g => g.Id, g => g.GroupName);
            var pro_Groups = await _uow.ProductGroups.GetByCategoryIdAsync(model?.CategoryId);
            var pro_GroupsEN = pro_Groups.ToList();


            vm.Companies = new SelectList(companies, "Id", "CompanyName", model?.CompanyId);
            vm.Contacts = new SelectList(contacts, "Id", "ContactName", model?.ContactId);
            vm.ContactForIndv = new SelectList(Indcontacts, "Id", "ContactName", model?.ContactId);
            vm.Sources = new SelectList(sources, "Id", "SourceName", model?.SourceId);
            vm.Statuses = new SelectList(statuses, "Id", "StatusName", model?.StatusId);

            // Populate status activities for the selected status (use FindAsync)
            IEnumerable<seashore_CRM.Models.Entities.LeadStatusActivity> statusActivitiesForSelected = Enumerable.Empty<seashore_CRM.Models.Entities.LeadStatusActivity>();
            if (model?.StatusId != null)
            {
                statusActivitiesForSelected = await _uow.LeadStatusActivities.FindAsync(a => a.LeadStatusId == model.StatusId.Value);
            }
            vm.StatusActivities = new SelectList(statusActivitiesForSelected, "Id", "ActivityName", model?.ActivityId);

            vm.Users = new SelectList(users, "Id", "FullName", model?.AssignedUserId);

            vm.ProductList = products.Select(p => new ProductOptionViewModel
             {
                 Text = p.ProductName,
                 Value = p.Id.ToString(),
                 Category = categoriesDict.TryGetValue(p.CategoryId, out var cname) ? cname : p.CategoryId.ToString(),
                 ProGroup = p.ProductGroupId.HasValue && groupsDict.TryGetValue(p.ProductGroupId.Value, out var gname) ? gname : null
             }).ToList();

            vm.Categories = new SelectList(categories, "Id", "CategoryName");
            vm.Pro_Groups = new SelectList(pro_GroupsEN, "Id", "GroupName");

            var productsAll = products;
            var prodMap = productsAll.Select(p => new {
                 id = p.Id,
                 name = p.ProductName,
                 cost = p.Cost,
                 tax = p.TaxPercentage,
                 categoryId = p.CategoryId,
                 categoryName = categoriesDict.TryGetValue(p.CategoryId, out var cn) ? cn : null,
                 productGroupName = p.ProductGroupId.HasValue && groupsDict.TryGetValue(p.ProductGroupId.Value, out var gn) ? gn : null
              }).ToDictionary(x => x.id.ToString(), x => x);
             vm.ProductsJson = JsonSerializer.Serialize(prodMap);

            // If editing an existing lead, load its LeadItems and map to ProductItems in VM so client JS can render rows
            if (model != null)
            {
                var leadItems = (await _uow.LeadItems.FindAsync(li => li.LeadId == model.Id)).ToList();
                if (leadItems.Any())
                {
                    vm.Lead.ProductItems = new List<seashore_CRM.BLL.DTOs.LeadProductDto>();
                    foreach (var li in leadItems)
                    {
                        var p = await _uow.Products.GetByIdAsync(li.ProductId);
                        var itemDto = new seashore_CRM.BLL.DTOs.LeadProductDto
                        {
                            ProductId = li.ProductId,
                            ProductName = p?.ProductName,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TaxPercentage = li.TaxPercentage,
                            CategoryId = p?.CategoryId,
                            ProductGroup = p != null && p.ProductGroupId.HasValue ? (groupsDict.TryGetValue(p.ProductGroupId.Value, out var gname) ? gname : null) : null,
                            SaleValue = li.UnitPrice * li.Quantity,
                            TaxValue = li.UnitPrice * li.Quantity * (li.TaxPercentage / 100M),
                            GrossTotal = li.LineTotal,
                            Cost = p?.Cost ?? 0,
                            GrossProfit = (li.LineTotal - (p?.Cost ?? 0) * li.Quantity)
                        };
                        vm.Lead.ProductItems.Add(itemDto);
                    }
                }

                // Also preload selected activities from existing Activity records for UI (if desired)
                var existingActs = (await _uow.Activities.FindAsync(a => a.LeadId == model.Id)).OrderByDescending(a => a.ActivityDate).ToList();
                if (existingActs.Any())
                {
                    vm.Lead.SelectedActivities = existingActs.Select(a => a.ActivityType).Distinct().ToList();
                }
            }

            var commentTemplates = new List<string>
            {
                "Need more information",
                "Sent quote",
                "Followed up",
                "Client requested sample",
                "Waiting for approval"
            };
            vm.CommentTemplates = new SelectList(commentTemplates);

            var mapping = await GetStatusActivitiesAsync();
            vm.StatusActivitiesJson = JsonSerializer.Serialize(mapping);

            if (model != null) vm.Lead = model;

            return vm;
        }

        private async Task<LeadEditViewModel> BuildLeadEditViewModel(LeadDto model)
        {
            var vm = new LeadEditViewModel();
            vm.Mode = "edit";
            vm.SubmitButtonText = "Update Lead";

            // reuse the same data population as create VM
            var createVm = await BuildLeadCreateViewModel(model);

            vm.Companies = createVm.Companies;
            vm.Contacts = createVm.Contacts;
            vm.ContactForIndv = createVm.ContactForIndv;
            vm.Sources = createVm.Sources;
            vm.Statuses = createVm.Statuses;
            vm.StatusActivities = createVm.StatusActivities;
            vm.Users = createVm.Users;
            vm.ProductList = createVm.ProductList;
            vm.Categories = createVm.Categories;
            vm.Pro_Groups = createVm.Pro_Groups;
            vm.ProductsJson = createVm.ProductsJson;
            vm.StatusActivitiesJson = createVm.StatusActivitiesJson;
            vm.CommentTemplates = createVm.CommentTemplates;
            vm.UserLeadRights = createVm.UserLeadRights;

            vm.Lead = model;

            return vm;
        }

        // ... other helper methods unchanged ...
    }
}