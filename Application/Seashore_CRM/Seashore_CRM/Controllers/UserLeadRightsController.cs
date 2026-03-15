using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.Common.Constants;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using Seashore_CRM.ViewModels.Lead;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Seashore_CRM.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    public class UserLeadRightsController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly AppDbContext _db;

        public UserLeadRightsController(IUnitOfWork uow, AppDbContext db)
        {
            _uow = uow;
            _db = db;
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
            var q = _db.Set<UserLeadRights>().AsQueryable();
            if (leadId.HasValue) q = q.Where(r => r.LeadId == leadId.Value);
            if (userId.HasValue) q = q.Where(r => r.UserId == userId.Value);

            var list = await q
                .Include(r => r.User)
                .Include(r => r.Lead)
                .OrderBy(r => r.Id)
                .Select(r => new UserLeadRightsViewModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    LeadId = r.LeadId,
                    UserName = r.User != null ? (r.User.FullName ?? r.User.Email) : r.UserId.ToString(),
                    LeadName = r.Lead != null ? ($"#{r.Lead.Id} - {r.Lead.LeadType}") : r.LeadId.ToString(),
                    CanView = r.CanView,
                    CanEdit = r.CanEdit
                }).ToListAsync();

            return Json(list);
        }

        // Get single rights entry
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var r = await _db.Set<UserLeadRights>()
                .Include(x => x.User)
                .Include(x => x.Lead)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r == null) return NotFound();

            var vm = new UserLeadRightsViewModel
            {
                Id = r.Id,
                UserId = r.UserId,
                LeadId = r.LeadId,
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
                var existing = await _db.Set<UserLeadRights>().FindAsync(model.Id);
                if (existing == null) return NotFound();

                existing.UserId = model.UserId;
                existing.CanView = model.CanView;
                existing.CanEdit = model.CanEdit;
                existing.LeadId = model.LeadId;

                _db.Set<UserLeadRights>().Update(existing);
                await _db.SaveChangesAsync();
                return Json(new { success = true, id = existing.Id });
            }

            // Prevent duplicates by (UserId, LeadId)
            var dup = await _db.Set<UserLeadRights>().FirstOrDefaultAsync(x => x.UserId == model.UserId && x.LeadId == model.LeadId);

            if (dup != null)
            {
                dup.CanView = model.CanView;
                dup.CanEdit = model.CanEdit;
                _db.Set<UserLeadRights>().Update(dup);
                await _db.SaveChangesAsync();
                return Json(new { success = true, id = dup.Id, updated = true });
            }

            var entity = new UserLeadRights
            {
                UserId = model.UserId,
                LeadId = model.LeadId,
                CanView = model.CanView,
                CanEdit = model.CanEdit
            };
            await _db.Set<UserLeadRights>().AddAsync(entity);
            await _db.SaveChangesAsync();

            return Json(new { success = true, id = entity.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _db.Set<UserLeadRights>().FindAsync(id);
            if (e == null) return NotFound();
            _db.Set<UserLeadRights>().Remove(e);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
