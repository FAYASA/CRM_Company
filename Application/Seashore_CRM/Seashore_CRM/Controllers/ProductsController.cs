using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace Seashore_CRM.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _uow;

        public ProductsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _uow.Products.GetAllAsync();
            return View(products.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _uow.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _uow.Categories.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", product.CategoryId);
                return View(product);
            }

            await _uow.Products.AddAsync(product);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();
            var categories = await _uow.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var categories = await _uow.Categories.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", product.CategoryId);
                return View(product);
            }
            _uow.Products.Update(product);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product != null)
            {
                _uow.Products.Remove(product);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}