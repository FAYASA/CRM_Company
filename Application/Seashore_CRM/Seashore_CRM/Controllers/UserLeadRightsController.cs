using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.Common.Constants;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using Seashore_CRM.ViewModels.Lead;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using seashore_CRM.BLL.Services.Service_Interfaces;

namespace Seashore_CRM.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class UserLeadRightsController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserLeadRightsService _rightsService;

        public UserLeadRightsController(IUnitOfWork uow, IUserLeadRightsService rightsService)
        {
            _uow = uow;
            _rightsService = rightsService;
        }

        // Master page that loads the SPA-like rights manager
        public async Task<IActionResult> Index()
        {
            var users = _uow.Users.GetAllAsync().ToList();
            var leads = (await _uow.Leads.GetAllAsync()).ToList();

            ViewBag.Users = new SelectList(users, "Id", "FullName");
            ViewBag.Leads = new SelectList(leads.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = $"#{l.Id} - {l.LeadType ?? string.Empty}" }), "Value", "Text");

            return View();
        }

        // JSON list endpoint used by client JS
        [HttpGet]
        public async Task<IActionResult> List(int? leadId, int? userId)
        {
            var list = (await _rightsService.ListAsync(leadId, userId)).ToList();

            var vm = list.Select(r => new UserLeadRightsViewModel
            {
                Id = r.Id,
                UserId = r.UserId ?? 0,
                LeadId = r.LeadId ?? 0,
                UserName = r.User != null ? (r.User.FullName ?? r.User.Email) : r.UserId.ToString(),
                LeadName = r.Lead != null ? ($"#{r.Lead.Id} - {r.Lead.LeadType}") : r.LeadId.ToString(),
                CanView = r.CanView,
                CanEdit = r.CanEdit
            }).ToList();

            return Json(vm);
        }

        // Get single rights entry
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var r = await _rightsService.GetAsync(id);
            if (r == null) return NotFound();

            var vm = new UserLeadRightsViewModel
            {
                Id = r.Id,
                UserId = r.UserId ?? 0,
                LeadId = r.LeadId ?? 0,
                UserName = r.User != null ? (r.User.FullName ?? r.User.Email) : string.Empty,
                LeadName = r.Lead != null ? ($"#{r.Lead.Id} - {r.Lead.LeadType}") : string.Empty,
                CanView = r.CanView,
                CanEdit = r.CanEdit
            };

            return Json(vm);
        }

        // Create or update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(UserLeadRightsViewModel model)
        {
            if (model == null) return BadRequest("Model required");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Update
            if (model.Id > 0)
            {
                var existing = await _rightsService.GetAsync(model.Id);
                if (existing == null) return NotFound();

                existing.UserId = model.UserId;
                existing.CanView = model.CanView;
                existing.CanEdit = model.CanEdit;
                existing.LeadId = model.LeadId;

                await _rightsService.SaveAsync(existing);
                return Json(new { success = true, id = existing.Id });
            }

            // Prevent duplicates by (UserId, LeadId)
            var dupList = (await _rightsService.ListAsync(model.LeadId, model.UserId)).ToList();
            var dup = dupList.FirstOrDefault();
            if (dup != null)
            {
                dup.CanView = model.CanView;
                dup.CanEdit = model.CanEdit;
                await _rightsService.SaveAsync(dup);
                return Json(new { success = true, id = dup.Id, updated = true });
            }

            var entity = new UserLeadRights
            {
                UserId = model.UserId,
                LeadId = model.LeadId,
                CanView = model.CanView,
                CanEdit = model.CanEdit
            };

            var id = await _rightsService.SaveAsync(entity);
            return Json(new { success = true, id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _rightsService.DeleteAsync(id);
            if (!ok) return NotFound();
            return Json(new { success = true });
        }
    }
}
