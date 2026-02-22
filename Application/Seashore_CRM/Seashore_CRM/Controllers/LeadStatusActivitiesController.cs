using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Seashore_CRM.Controllers
{
    public class LeadStatusActivitiesController : Controller
    {
        private readonly IUnitOfWork _uow;

        public LeadStatusActivitiesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: LeadStatusActivities
        public async Task<IActionResult> Index()
        {
            var items = (await _uow.LeadStatusActivities.GetAllAsync()).OrderBy(a => a.Id).ToList();
            // ensure LeadStatus navigation is populated
            var statuses = (await _uow.LeadStatuses.GetAllAsync()).ToDictionary(s => s.Id, s => s.StatusName);
            ViewBag.Statuses = statuses;
            return View(items);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var statuses = await _uow.LeadStatuses.GetAllAsync();
            ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName");
            return View(new LeadStatusActivity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadStatusActivity model)
        {
            if (!ModelState.IsValid)
            {
                var statuses = await _uow.LeadStatuses.GetAllAsync();
                ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName", model.LeadStatusId);
                return View(model);
            }

            await _uow.LeadStatusActivities.AddAsync(model);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (item == null) return NotFound();
            var statuses = await _uow.LeadStatuses.GetAllAsync();
            ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName", item.LeadStatusId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeadStatusActivity model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var statuses = await _uow.LeadStatuses.GetAllAsync();
                ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName", model.LeadStatusId);
                return View(model);
            }

            var existing = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.ActivityName = model.ActivityName;
            existing.LeadStatusId = model.LeadStatusId;

            _uow.LeadStatusActivities.Update(existing);
            await _uow.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (item == null) return NotFound();
            var status = await _uow.LeadStatuses.GetByIdAsync(item.LeadStatusId);
            ViewBag.StatusName = status?.StatusName;
            return View(item);
        }

        // GET: Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (item == null) return NotFound();
            var status = await _uow.LeadStatuses.GetByIdAsync(item.LeadStatusId);
            ViewBag.StatusName = status?.StatusName;
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _uow.LeadStatusActivities.GetByIdAsync(id);
            if (item != null)
            {
                _uow.LeadStatusActivities.Remove(item);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
