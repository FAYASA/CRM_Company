using Microsoft.AspNetCore.Mvc;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CategoriesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: CategoriesController
        public async Task<IActionResult> Index()
        {
            var list = await _uow.Categories.GetAllAsync();
            return View(list);
        }

        // GET: CategoriesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: CategoriesController/Create
        public IActionResult Create()
        {
            return View(new Category());
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            await _uow.Categories.AddAsync(category);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            if (!ModelState.IsValid) return View(category);
            _uow.Categories.Update(category);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CategoriesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: CategoriesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category != null)
            {
                _uow.Categories.Remove(category);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
