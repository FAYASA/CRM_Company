using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using Seashore_CRM.ViewModels.ProductGroup;
using System.Linq;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public class ProGroupController : Controller
    {
        private readonly IUnitOfWork _uow;

        public ProGroupController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: ProGroupController
        public async Task<IActionResult> Index()
        {
            var groups = await _uow.ProductGroups.GetAllAsync();
            var vm = groups.Select(g => new ProGroupListViewModel
            {
                Id = g.Id,
                GroupName = g.GroupName,
                CategoryId = g.CategoryId,
                CategoryName = g.Category?.CategoryName ?? "(n/a)",
                IsActive = g.IsActive
            }).ToList();

            return View(vm);
        }

        // GET: ProGroupController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var g = await _uow.ProductGroups.GetByIdAsync(id);
            if (g == null) return NotFound();
            return View(g);
        }

        // GET: ProGroupController/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ProGroupCreateViewModel();
            var cats = await _uow.Categories.GetAllAsync();
            vm.Categories = cats.Select(c => new SelectListItem(c.CategoryName, c.Id.ToString()));
            vm.IsActive = true;
            return View(vm);
        }

        // POST: ProGroupController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProGroupCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cats = await _uow.Categories.GetAllAsync();
                model.Categories = cats.Select(c => new SelectListItem(c.CategoryName, c.Id.ToString()));
                return View(model);
            }

            var entity = new ProductGroup
            {
                GroupName = model.GroupName,
                CategoryId = model.CategoryId!.Value,
                IsActive = model.IsActive
            };

            await _uow.ProductGroups.AddAsync(entity);
            await _uow.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ProGroupController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var g = await _uow.ProductGroups.GetByIdAsync(id);
            if (g == null) return NotFound();

            var vm = new ProGroupUpdateViewModel
            {
                GroupName = g.GroupName,
                CategoryId = g.CategoryId,
                IsActive = g.IsActive
            };
            var cats = await _uow.Categories.GetAllAsync();
            vm.Categories = cats.Select(c => new SelectListItem(c.CategoryName, c.Id.ToString()));
            ViewBag.Id = id;
            return View(vm);
        }

        // POST: ProGroupController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProGroupUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cats = await _uow.Categories.GetAllAsync();
                model.Categories = cats.Select(c => new SelectListItem(c.CategoryName, c.Id.ToString()));
                return View(model);
            }

            var g = await _uow.ProductGroups.GetByIdAsync(id);
            if (g == null) return NotFound();

            g.GroupName = model.GroupName;
            g.CategoryId = model.CategoryId!.Value;
            g.IsActive = model.IsActive;

            _uow.ProductGroups.Update(g);
            await _uow.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ProGroupController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var g = await _uow.ProductGroups.GetByIdAsync(id);
            if (g == null) return NotFound();
            var vm = new ProGroupListViewModel
            {
                Id = g.Id,
                GroupName = g.GroupName,
                CategoryId = g.CategoryId,
                CategoryName = g.Category?.CategoryName,
                IsActive = g.IsActive
            };
            return View(vm);
        }

        // POST: ProGroupController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var g = await _uow.ProductGroups.GetByIdAsync(id);
            if (g == null) return NotFound();
            g.IsActive = false; // soft delete
            _uow.ProductGroups.Update(g);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> ByCategory(int categoryId)
        {
            var groups = await _uow.ProductGroups.GetByCategoryIdAsync(categoryId);
            var result = groups.Select(g => new { id = g.Id, name = g.GroupName }).ToList();
            return Json(result);
        }
    }
}
