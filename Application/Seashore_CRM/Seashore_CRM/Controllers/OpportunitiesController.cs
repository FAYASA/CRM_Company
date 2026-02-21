using Microsoft.AspNetCore.Mvc;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using seashore_CRM.BLL.Services.Service_Interfaces;

namespace Seashore_CRM.Controllers
{
    public class OpportunitiesController : Controller
    {
        private readonly IOpportunityService _opportunityService;
        private readonly ILeadService _leadService;

        public OpportunitiesController(IOpportunityService opportunityService, ILeadService leadService)
        {
            _opportunityService = opportunityService;
            _leadService = leadService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _opportunityService.GetAllAsync();
            return View(items.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _opportunityService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            var leads = await _leadService.GetAllLeadsAsync();
            ViewBag.Leads = leads.Select(l => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(l.Id.ToString(), l.Id.ToString())).ToList();
            return View(new Opportunity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (!ModelState.IsValid) return View(opportunity);
            await _opportunityService.AddAsync(opportunity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _opportunityService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Opportunity opportunity)
        {
            if (id != opportunity.Id) return BadRequest();
            if (!ModelState.IsValid) return View(opportunity);
            await _opportunityService.UpdateAsync(opportunity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _opportunityService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _opportunityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
