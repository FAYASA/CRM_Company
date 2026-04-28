using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.Application.DTOs;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.BLL.Services.Service_Interfaces;
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
        private readonly IWebHostEnvironment _env;
        private readonly ILeadStatusActivityService _leadStatusActivityService;
        private readonly IProductService _product_service; 
        private readonly IContactService _contactService;

        public LeadsController(ILeadService leadService, IWebHostEnvironment env, ILeadStatusActivityService leadStatusActivityService,
            IProductService product_service, IContactService contactService)
        {
            _leadService = leadService;
            _env = env;
            _leadStatusActivityService = leadStatusActivityService;
            _product_service = product_service;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index(string? q, int? status, int? assigned, int? category, int page = 1, int pageSize = 20)
        {
            var allLeads = (await _leadService.GetAllLeadsAsync()).ToList();

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

            var lookupData = await _leadService.BuildLeadCreateDataAsync();

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
                Statuses = new SelectList(lookupData.Statuses, "Id", "Name", status),
                Users = new SelectList(lookupData.Users, "Id", "Name", assigned),
                Categories = new SelectList(lookupData.Categories, "Id", "Name", category)
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound();

            var mapping = await GetStatusActivitiesAsync();
            if (!string.IsNullOrEmpty(lead.StatusName) && mapping.TryGetValue(lead.StatusName, out var acts))
            {
                ViewBag.SuggestedActivities = acts.ToList();
            }
            else
            {
                ViewBag.SuggestedActivities = new List<string>();
            }

            var activities = (await _leadService.GetActivitiesByLeadAsync(id)).ToList();
            ViewBag.Activities = activities;

            var comments = (await _leadService.GetCommentsByLeadAsync(id)).ToList();
            ViewBag.Comments = comments;

            // Lead history (status changes, created, etc.)
            try
            {
                var history = (await _leadService.GetHistoryByLeadAsync(id)).ToList();
                ViewBag.History = history;
            }
            catch
            {
                ViewBag.History = new List<seashore_CRM.Models.Entities.LeadHistory>();
            }

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(int leadId, string activityType)
        {
            if (string.IsNullOrWhiteSpace(activityType)) return BadRequest();

            var act = new LeadStatusActivity
            {
                LeadId = leadId,
                ActivityName = activityType,
                ActivityDate = DateTime.UtcNow
            };

            await _leadStatusActivityService.AddAsync(act);
            return RedirectToAction(nameof(Details), new { id = leadId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStatusActivity(string statusName, string activityName)
        {
            if (string.IsNullOrWhiteSpace(statusName) || string.IsNullOrWhiteSpace(activityName))
                return BadRequest("statusName and activityName are required");

            var mapping = await _leadService.AddStatusActivityAsync(statusName, activityName);
            return Json(new { success = true, mapping = mapping });
        }


        #region Lead Create
        ///////////////////////////////////////////////////////////////////////////////
        /// Lead create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Build empty form with all dropdown data
            var vm = await BuildLeadCreateViewModel();

            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return await RebuildView(vm, isEdit: false);

            var dto = new LeadCreateDataDto
            {
                Lead = vm.Lead,
                CommentsText = vm.CommentsText,
                Files = vm.Files
            };

            var result = await _leadService.CreateLeadAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                return await RebuildView(vm, isEdit: false);
            }

            return RedirectToAction(nameof(Index));
        }

        // if model state is invalid,
        // we need to rebuild the select lists and return the view so user can correct
        private async Task<IActionResult> RebuildView(LeadCreateViewModel vm, bool isEdit = false)
        {
            var data = await _leadService.BuildLeadCreateDataAsync(
                vm?.Lead?.CategoryId,
                vm?.Lead?.CompanyId,
                vm?.Lead?.StatusId);

            vm.Companies = new SelectList(data.Companies, "Id", "Name", vm?.Lead?.CompanyId);
            vm.Contacts = new SelectList(data.Contacts, "Id", "Name", vm?.Lead?.ContactId);
            vm.Sources = new SelectList(data.Sources, "Id", "Name", vm?.Lead?.SourceId);
            vm.Statuses = new SelectList(data.Statuses, "Id", "Name", vm?.Lead?.StatusId);
            vm.Users = new SelectList(data.Users, "Id", "Name", vm?.Lead?.AssignedUserId);

            vm.ProductList = data.ProductList.Select(p => new ProductOptionViewModel
            {
                Text = p.ProductName,
                Value = p.Id.ToString()
            }).ToList();

            return View("Create", vm);
        }


        ///////////////////////////////////////////////////////////////////////////////
        /// Lead create

        #endregion lead create



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(LeadCreateViewModel vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var data = await _leadService.BuildLeadCreateDataAsync(vm?.Lead?.CategoryId, vm?.Lead?.CompanyId, 
        //            vm?.Lead?.StatusId);

        //        await _leadService.CreateLeadAsync(vm, Request.Form.Files);



        //        var repop = new LeadCreateViewModel();
        //        repop.Companies = new SelectList(data.Companies, "Id", "Name", vm?.Lead?.CompanyId);
        //        repop.Contacts = new SelectList(data.Contacts, "Id", "Name", vm?.Lead?.ContactId);
        //        repop.ContactForIndv = new SelectList(data.ContactForIndv, "Id", "Name", vm?.Lead?.IndividualCustomerId ?? vm?.Lead?.ContactId);
        //        repop.Sources = new SelectList(data.Sources, "Id", "Name", vm?.Lead?.SourceId);
        //        repop.Statuses = new SelectList(data.Statuses, "Id", "Name", vm?.Lead?.StatusId);
        //        repop.Users = new SelectList(data.Users, "Id", "Name", vm?.Lead?.AssignedUserId);

        //        repop.ProductList = data.ProductList.Select(p => new ProductOptionViewModel
        //        { 
        //            Text = p.ProductName, 
        //            Value = p.Id.ToString(), 
        //            Category = p.CategoryName, 
        //            ProGroup = p.ProductGroupName 
        //        }).ToList();

        //        repop.Categories = new SelectList(data.Categories, "Id", "Name", vm?.Lead?.CategoryId);
        //        repop.Pro_Groups = new SelectList(data.ProductGroups, "Id", "Name");
        //        repop.ProductsJson = data.ProductsJson;
        //        repop.CommentTemplates = new SelectList(data.CommentTemplates);
        //        repop.StatusActivitiesJson = JsonSerializer.Serialize(data.StatusActivitiesMapping);
        //        repop.Lead = vm?.Lead;
        //        repop.Mode = "create";
        //        repop.SubmitButtonText = "Save Lead";
        //        return View(repop);
        //    }

        //    try
        //    {

        //        var dtoLead = new LeadDto
        //        {
        //            LeadType = vm.Lead.LeadType,
        //            CompanyId = vm.Lead.CompanyId,
        //            ContactId = vm.Lead.ContactId,
        //            IndividualCustomerId = vm.Lead.IndividualCustomerId,
        //            ActivityId = vm.Lead.ActivityId,
        //            SourceId = vm.Lead.SourceId,
        //            StatusId = vm.Lead.StatusId,
        //            ActivityType = vm.Lead.ActivityType,
        //            FollowUpDate = vm.Lead.FollowUpDate,
        //            FollowUpTime = vm.Lead.FollowUpTime,
        //            ExpectedClosureDate = vm.Lead.ExpectedClosureDate,
        //            Priority = vm.Lead.Priority,
        //            AssignedUserId = vm.Lead.AssignedUserId,
        //            IsQualified = vm.Lead.IsQualified,
        //            QualifiedOn = vm.Lead.QualifiedOn,
        //            QualifiedById = vm.Lead.QualifiedById,
        //            QualificationNotes = vm.Lead.QualificationNotes,
        //            IsConverted = vm.Lead.IsConverted,
        //            Budget = vm.Lead.Budget,
        //            DecisionDate = vm.Lead.DecisionDate,
        //            Probability = vm.Lead.Probability,
        //            ProductItems = vm.Lead.ProductItems,
        //            AttachmentsJson = vm.Lead.AttachmentsJson,
        //            UserLeadRights = vm.Lead.UserLeadRights,
        //            Comments = vm.Lead.Comments ?? new System.Collections.Generic.List<seashore_CRM.BLL.DTOs.CommentDto>()
        //        };

        //        // If user provided the comments textarea, convert into a single CommentDto and include
        //        var commentsText = Request.Form["Comments"].FirstOrDefault();
        //        if (!string.IsNullOrWhiteSpace(commentsText))
        //        {
        //            dtoLead.Comments.Add(new seashore_CRM.BLL.DTOs.CommentDto { LeadId = 0, UserId = 0, Content = commentsText, CreatedAt = DateTime.UtcNow });
        //        }

        //        // Capture uploaded attachments and store filenames in AttachmentsJson
        //        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
        //        {
        //            var files = Request.Form.Files; // IFormFileCollection
        //            var savedNames = new List<string>();
        //            var uploadPath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads", "leads");
        //            Directory.CreateDirectory(uploadPath);
        //            foreach (var f in files)
        //            {
        //                try
        //                {
        //                    var fileName = Path.GetFileName(f.FileName);
        //                    var savePath = Path.Combine(uploadPath, fileName);
        //                    using (var fs = new FileStream(savePath, FileMode.Create)) { await f.CopyToAsync(fs); }
        //                    savedNames.Add(Path.Combine("/uploads/leads", fileName));
        //                }
        //                catch { /* ignore single file failure */ }
        //            }

        //            // merge with existing attachments if any
        //            var existing = new List<string>();
        //            if (!string.IsNullOrWhiteSpace(dtoLead.AttachmentsJson))
        //            {
        //                try { existing = JsonSerializer.Deserialize<List<string>>(dtoLead.AttachmentsJson) ?? new List<string>(); } catch { }
        //            }
        //            existing.AddRange(savedNames);
        //            dtoLead.AttachmentsJson = JsonSerializer.Serialize(existing);
        //        }

        //        var (result, leadId) = await _leadService.CreateLeadAsync(dtoLead);

        //        if (result != "Success")
        //        {
        //            ModelState.AddModelError(string.Empty, result);
        //            var data2 = await _leadService.BuildLeadCreateDataAsync(dtoLead?.CategoryId, dtoLead?.CompanyId, dtoLead?.StatusId);
        //            var newVm = new LeadCreateViewModel();
        //            newVm.Companies = new SelectList(data2.Companies, "Id", "Name", dtoLead?.CompanyId);
        //            newVm.Contacts = new SelectList(data2.Contacts, "Id", "Name", dtoLead?.ContactId);
        //            newVm.ContactForIndv = new SelectList(data2.ContactForIndv, "Id", "Name", dtoLead?.IndividualCustomerId ?? dtoLead?.ContactId);
        //            newVm.Sources = new SelectList(data2.Sources, "Id", "Name", dtoLead?.SourceId);
        //            newVm.Statuses = new SelectList(data2.Statuses, "Id", "Name", dtoLead?.StatusId);
        //            newVm.Users = new SelectList(data2.Users, "Id", "Name", dtoLead?.AssignedUserId);
        //            newVm.ProductList = data2.ProductList.Select(p => new ProductOptionViewModel { Text = p.ProductName, Value = p.Id.ToString(), Category = p.CategoryName, ProGroup = p.ProductGroupName }).ToList();
        //            newVm.Categories = new SelectList(data2.Categories, "Id", "Name", dtoLead?.CategoryId);
        //            newVm.Pro_Groups = new SelectList(data2.ProductGroups, "Id", "Name");
        //            newVm.ProductsJson = data2.ProductsJson;
        //            newVm.CommentTemplates = new SelectList(data2.CommentTemplates);
        //            newVm.StatusActivitiesJson = JsonSerializer.Serialize(data2.StatusActivitiesMapping);
        //            newVm.Lead = dtoLead;
        //            newVm.Mode = "create";
        //            newVm.SubmitButtonText = "Save Lead";
        //            return View(newVm);
        //        }

        //    }
        //    catch (ValidationException vex)
        //    {
        //        foreach (var err in vex.Errors)
        //        {
        //            if (string.IsNullOrEmpty(err.PropertyName))
        //                ModelState.AddModelError(string.Empty, err.ErrorMessage);
        //            else
        //                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
        //        }

        //        var newVm = await BuildLeadCreateViewModel();
        //        newVm.Mode = "create";
        //        newVm.SubmitButtonText = "Save Lead";
        //        return View(newVm);
        //    }

        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
        //        var newVm = await BuildLeadCreateViewModel();
        //        newVm.Mode = "create";
        //        newVm.SubmitButtonText = "Save Lead";
        //        return View(newVm);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        #region Lead Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            // Get existing lead
            var lead = await _leadService.GetLeadByIdAsync(id);

            if (lead == null)
                return NotFound();

            // Build a LeadEditViewModel which the Edit view expects
            var vm = await BuildLeadEditViewModel(lead);

            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeadEditViewModel vm)
        {
            // LeadEditViewModel inherits LeadCreateViewModel so it can be passed to RebuildView
            if (!ModelState.IsValid)
                return await RebuildView(vm, isEdit: true);

            var dto = new LeadEditDataDto
            {
                Id = vm.Lead.Id,
               //Lead = vm.Lead,
                CommentsText = vm.CommentsText,
                Files = vm.Files
            };

            var result = await _leadService.UpdateLeadAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);
                return await RebuildView(vm, isEdit: true);
            }

            return RedirectToAction(nameof(Index));
        }


        #endregion Lead Edit

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var lead = await _leadService.GetLeadByIdAsync(id);
        //    if (lead == null) return NotFound();

        //    // Use the factory to ensure contacts (including inactive selected) and user-rights are populated correctly
        //    var vm = await BuildLeadEditViewModel(lead);
        //    return View("Edit", vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, LeadEditViewModel vm)
        //{
        //    var lead = vm?.Lead ?? new LeadDto();

        //    var rv = Request.Form["Lead.RowVersion"].FirstOrDefault();
        //    if (!string.IsNullOrEmpty(rv))
        //    {
        //        try { lead.RowVersion = Convert.FromBase64String(rv); } catch { }
        //    }

        //    if (id != lead.Id) return BadRequest();

        //    try
        //    {
        //        // Handle uploaded attachments during edit
        //        if (Request.Form.Files != null && Request.Form.Files.Count > 0)
        //        {
        //            var files = Request.Form.Files;
        //            var savedNames = new List<string>();
        //            var uploadPath = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads", "leads");
        //            Directory.CreateDirectory(uploadPath);
        //            foreach (var f in files)
        //            {
        //                try
        //                {
        //                    var fileName = Path.GetFileName(f.FileName);
        //                    var savePath = Path.Combine(uploadPath, fileName);
        //                    using (var fs = new FileStream(savePath, FileMode.Create)) { await f.CopyToAsync(fs); }
        //                    savedNames.Add(Path.Combine("/uploads/leads", fileName));
        //                }
        //                catch { }
        //            }

        //            var existing = new List<string>();
        //            if (!string.IsNullOrWhiteSpace(lead.AttachmentsJson))
        //            {
        //                try { existing = JsonSerializer.Deserialize<List<string>>(lead.AttachmentsJson) ?? new List<string>(); } catch { }
        //            }
        //            existing.AddRange(savedNames);
        //            lead.AttachmentsJson = JsonSerializer.Serialize(existing);
        //        }

        //        // Capture comments textarea on edit and append as a CommentDto so service persists it
        //        var commentsText = Request.Form["Comments"].FirstOrDefault();
        //        if (!string.IsNullOrWhiteSpace(commentsText))
        //        {
        //            lead.Comments = lead.Comments ?? new List<seashore_CRM.BLL.DTOs.CommentDto>();
        //            lead.Comments.Add(new seashore_CRM.BLL.DTOs.CommentDto { LeadId = lead.Id, UserId = 0, Content = commentsText, CreatedAt = DateTime.UtcNow });
        //        }

        //        await _leadService.UpdateLeadAsync(lead);
        //        // persisted by service
        //    }
        //    catch (ValidationException vex)
        //    {
        //        foreach (var err in vex.Errors)
        //        {
        //            if (string.IsNullOrEmpty(err.PropertyName))
        //                ModelState.AddModelError(string.Empty, err.ErrorMessage);
        //            else
        //                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
        //        }

        //        // Rebuild edit view model and return Edit view so user can correct values
        //        var editVm = await BuildLeadEditViewModel(lead);
        //        return View("Edit", editVm);
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        var editVm = await BuildLeadEditViewModel(lead);
        //        return View("Edit", editVm);
        //    }
        //    catch (Exception ex)
        //    {
        //        // General exception handling - surface message and repopulate edit form
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        var editVm = await BuildLeadEditViewModel(lead);
        //        return View("Edit", editVm);
        //    }

        //    var selectedActivities = Request.Form["SelectedActivities"].ToList();
        //    if (selectedActivities != null && selectedActivities.Any())
        //    {
        //        await _leadService.AddActivitiesToLeadAsync(lead.Id, selectedActivities);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Qualify(LeadEditDataDto lead)
        //{
        //    try
        //    {
        //        // Persist qualification updates using existing UpdateLeadAsync
        //        await _leadService.UpdateLeadAsync(lead);

        //        var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        //        if (isAjax)
        //        {
        //            return Json(new { success = true, leadId = lead.Id });
        //        }

        //        return RedirectToAction("Details", new { id = lead.Id });
        //    }
        //    catch (ValidationException vex)
        //    {
        //        foreach (var err in vex.Errors)
        //        {
        //            if (string.IsNullOrEmpty(err.PropertyName))
        //                ModelState.AddModelError(string.Empty, err.ErrorMessage);
        //            else
        //                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
        //        }

        //        await PopulateSelectListsAsync(lead);
        //        return View(lead);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Validation is handled in service; surface error and repopulate select lists
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        await PopulateSelectListsAsync(lead);
        //        return View(lead);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> ContactsByCompany(int companyId, int? selectedContactId = null)
        {
            if (companyId <= 0 && !selectedContactId.HasValue)
                return Json(new List<object>());

            // Get contacts (service returns IEnumerable/Queryable of ContactListDto)
            var contacts = await _contactService.GetAllActiveByCompanyIdAsync(companyId);

            // Use DTO list instead of tuple list to avoid tuple literals inside expressions
            var list = new List<seashore_CRM.BLL.DTOs.OptionDto>();
            if (contacts != null)
            {
                list.AddRange(contacts.Select(c => new seashore_CRM.BLL.DTOs.OptionDto { Id = c.Id, Name = c.ContactName }));
            }

            // If caller requested a specific contact that may be inactive or not part of the active list, include it so UI can pre-select
            if (selectedContactId.HasValue)
            {
                try
                {
                    var exists = list.FirstOrDefault(x => x.Id == selectedContactId.Value);
                    if (exists == null)
                    {
                        var cdet = await _contactService.GetByIdAsync(selectedContactId.Value);
                        if (cdet != null)
                        {
                            list.Add(new seashore_CRM.BLL.DTOs.OptionDto { Id = cdet.Id, Name = !string.IsNullOrWhiteSpace(cdet.ContactName) ? cdet.ContactName : (cdet.Email ?? cdet.Mobile ?? "(no name)") });
                        }
                    }
                }
                catch { }
            }

            var result = list.Select(c => new { Id = c.Id, Name = c.Name });

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> ProductGroupsByCategory(int categoryId)
        {
            if (categoryId <= 0) return Json(new List<object>());
            var groups = await _leadService.GetProductGroupsByCategoryAsync(categoryId);
            var result = groups.Select(g => new { id = g.Id, name = g.Name });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ActivitiesByStatus(int statusId)
        {
            if (statusId <= 0) return Json(new List<object>());
            var activities = await _leadService.GetActivitiesByStatusAsync(statusId);
            var result = activities.Select(a => new { id = a.Id, name = a.Name }).ToList();
            return Json(result);
        }

        private async Task PopulateSelectListsAsync(LeadDto? model = null)
        {
            var data = await _leadService.BuildLeadCreateDataAsync(model?.CategoryId, model?.CompanyId, model?.StatusId);

            ViewBag.Companies = new SelectList(data.Companies, "Id", "Name", model?.CompanyId);
            ViewBag.Contacts = new SelectList(data.Contacts, "Id", "Name", model?.ContactId);
            ViewBag.ContactForIndv = new SelectList(data.ContactForIndv, "Id", "Name", model?.IndividualCustomerId ?? model?.ContactId);
            ViewBag.Sources = new SelectList(data.Sources, "Id", "Name", model?.SourceId);
            ViewBag.Statuses = new SelectList(data.Statuses, "Id", "Name", model?.StatusId);
            ViewBag.StatusActivities = new SelectList(new List<object>());
            ViewBag.Users = new SelectList(data.Users, "Id", "Name", model?.AssignedUserId);
            ViewBag.ProductList = data.ProductList.Select(p => new SelectListItem(p.ProductName, p.Id.ToString())).ToList();
            ViewBag.Categories = new SelectList(data.Categories, "Id", "Name", model?.CategoryId);
            ViewBag.Pro_Groups = new SelectList(data.ProductGroups, "Id", "Name");
            ViewBag.ProductsJson = data.ProductsJson;
            ViewBag.CommentTemplates = new SelectList(data.CommentTemplates);
            ViewBag.StatusActivitiesJson = JsonSerializer.Serialize(data.StatusActivitiesMapping);
        }

        private async Task<LeadCreateViewModel> BuildLeadCreateViewModel(LeadDto? model = null)
        {
            var data = await _leadService.BuildLeadCreateDataAsync(model?.CategoryId, model?.CompanyId, model?.StatusId);
            var vm = new LeadCreateViewModel();
            vm.Companies = new SelectList(data.Companies, "Id", "Name", model?.CompanyId);
            vm.Contacts = new SelectList(data.Contacts, "Id", "Name", model?.ContactId);
            vm.ContactForIndv = new SelectList(data.ContactForIndv, "Id", "Name", model?.IndividualCustomerId ?? model?.ContactId);
            vm.Sources = new SelectList(data.Sources, "Id", "Name", model?.SourceId);
            vm.Statuses = new SelectList(data.Statuses, "Id", "Name", model?.StatusId);
            vm.StatusActivities = new SelectList(new List<object>());
            vm.Users = new SelectList(data.Users, "Id", "Name", model?.AssignedUserId);
            vm.ProductList = data.ProductList.Select(p => new ProductOptionViewModel { Text = p.ProductName, Value = p.Id.ToString(), 
                Category = p.CategoryName, ProGroup = p.ProductGroupName }).ToList();
            vm.Categories = new SelectList(data.Categories, "Id", "Name");
            vm.Pro_Groups = new SelectList(data.ProductGroups, "Id", "Name");
            vm.ProductsJson = data.ProductsJson;
            vm.CommentTemplates = new SelectList(data.CommentTemplates);
            vm.StatusActivitiesJson = JsonSerializer.Serialize(data.StatusActivitiesMapping);

            // ensure existing contact (corporate or individual) is present in Contacts list even if inactive
            if ((model?.ContactId != null && model.ContactId > 0) || (model?.IndividualCustomerId != null && model.IndividualCustomerId > 0))
            {
                var contactIdToFetch = model.ContactId ?? model.IndividualCustomerId.Value;
                var has = data.Contacts.Any(c => c.Id == contactIdToFetch);
                if (!has)
                {
                    try
                    {
                        var cdet = await _contactService.GetByIdAsync(contactIdToFetch);
                        if (cdet != null)
                        {
                            var list = data.Contacts.ToList();
                            list.Add(new seashore_CRM.BLL.DTOs.OptionDto { Id = cdet.Id, Name = !string.IsNullOrWhiteSpace(cdet.ContactName) ? cdet.ContactName : (cdet.Email ?? cdet.Mobile ?? "(no name)") });
                            vm.Contacts = new SelectList(list, "Id", "Name", contactIdToFetch);
                        }
                    }
                    catch { }
                }
            }

            // populate user lead rights when editing existing lead
            if (model != null && model.Id > 0)
            {
                try
                {
                    var rights = await _leadService.GetUserLeadRightsAsync(model.Id);
                    if (rights != null)
                    {
                        vm.UserLeadRights = rights.Select(r => new UserLeadRightsViewModel
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            LeadId = r.LeadId,
                            CanView = r.CanView,
                            CanEdit = r.CanEdit,
                            UserName = !string.IsNullOrWhiteSpace(r.UserName) ? r.UserName : (data.Users.FirstOrDefault(u => u.Id == r.UserId)?.Name ?? string.Empty)
                        }).ToList();
                    }
                }
                catch { }
            }

            if (model != null) vm.Lead = model;
            return vm;
        }

        // prepare model (helper)

        [HttpGet]
        private async Task<LeadCreateViewModel> PrepareLeadCreateViewModelAsync(LeadCreateViewModel vm = null)
        {
            var data = await _leadService.BuildLeadCreateDataAsync();
            var newVm = vm ?? new LeadCreateViewModel();

            newVm.Companies = new SelectList(data.Companies, "Id", "Name");
            newVm.Contacts = new SelectList(data.Contacts, "Id", "Name");
            newVm.ContactForIndv = new SelectList(data.ContactForIndv, "Id", "Name");
            newVm.Sources = new SelectList(data.Sources, "Id", "Name");
            newVm.Statuses = new SelectList(data.Statuses, "Id", "Name");
            newVm.Users = new SelectList(data.Users, "Id", "Name");
            newVm.ProductList = data.ProductList.Select(p => new ProductOptionViewModel { Text = p.ProductName, Value = p.Id.ToString(), Category = p.CategoryName, ProGroup = p.ProductGroupName }).ToList();
            newVm.Categories = new SelectList(data.Categories, "Id", "Name");
            newVm.Pro_Groups = new SelectList(data.ProductGroups, "Id", "Name");
            newVm.ProductsJson = data.ProductsJson;
            newVm.CommentTemplates = new SelectList(data.CommentTemplates);
            newVm.StatusActivitiesJson = JsonSerializer.Serialize(data.StatusActivitiesMapping);

            return newVm;
        }
        private async Task<LeadEditViewModel> BuildLeadEditViewModel(LeadDto model)
        {
            var vm = new LeadEditViewModel();

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

        private async Task<Dictionary<string, string[]>> GetStatusActivitiesAsync()
        {
            return await _leadService.GetStatusActivitiesAsync();
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
            catch { }
        }
    }
}