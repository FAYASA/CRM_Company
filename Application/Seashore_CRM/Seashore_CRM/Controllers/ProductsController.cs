using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Seashore_CRM.ViewModels.Product;
using System.Linq;
using System.Threading.Tasks;

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
            var products = _uow.Products.GetAllAsync();

            var viewModel = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                ProductGroupId = p.ProductGroupId,
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            }).ToList();

            return View(viewModel);

        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        //public async Task<IActionResult> Create()
        //{
        //    var categories = await _uow.Categories.GetAllAsync();
        //    ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        //    return View(new Product());
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Product product)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var categories = await _uow.Categories.GetAllAsync();
        //        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", product.CategoryId);
        //        return View(product);
        //    }

        //    await _uow.Products.AddAsync(product);
        //    await _uow.CommitAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductViewModel();

            var categories = await _uow.Categories.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();

            model.ProductGroups = new List<SelectListItem>();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = new Product
            {
                ProductName = model.ProductName,
                CategoryId = model.CategoryId,
                ProductGroupId = model.ProductGroupId,
                Cost = model.Cost,
                TaxPercentage = model.TaxPercentage
            };

            await _uow.Products.AddAsync(product);

            await _uow.CommitAsync();

            return RedirectToAction("Index");
        }

        public async Task<JsonResult> GetGroups(int categoryId)
        {
            var groups = await _uow.ProductGroups.GetAllAsync();

            var result = groups
                .Where(g => g.CategoryId == categoryId)
                .Select(g => new
                {
                    id = g.Id,
                    name = g.GroupName
                })
                .ToList();

            return Json(result);
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