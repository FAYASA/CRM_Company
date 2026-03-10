using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Seashore_CRM.ViewModels.Category;
using System.Linq;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: CategoriesController
        public async Task<IActionResult> Index()
        {
            var list = await _categoryService.GetAllCategoriesAsync();
            var vm = list.Select(c => new CategoryListViewModel
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                IsActive = c.IsActive
            });
            return View(vm);
        }

        // GET: CategoriesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: CategoriesController/Create
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = new Category
            {
                CategoryName = model.CategoryName,
                IsActive = true
            };
            await _categoryService.AddCategoryAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            var vm = new CategoryUpdateViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                IsActive = category.IsActive,
                RowVersion = category.RowVersion
            };
            return View(vm);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryUpdateViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            // Concurrency check
            if (model.RowVersion != null && category.RowVersion != null && !category.RowVersion.SequenceEqual(model.RowVersion))
            {
                ModelState.AddModelError(string.Empty, "The category has been modified by another user.");
                return View(model);
            }

            category.CategoryName = model.CategoryName;
            category.IsActive = model.IsActive;
            await _categoryService.UpdateCategoryAsync(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: CategoriesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            var vm = new CategoryListViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                IsActive = category.IsActive
            };
            return View(vm);
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // implement soft delete via service
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // to inactivate category instead of deleting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inactivate(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            category.IsActive = false;
            await _categoryService.UpdateCategoryAsync(category);
            return RedirectToAction(nameof(Index));
        }
    }
}
