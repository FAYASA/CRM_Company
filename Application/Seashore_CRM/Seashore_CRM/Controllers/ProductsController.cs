using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.ApplicationLayer.DTOs;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Seashore_CRM.ViewModels.Product;
using System.Linq;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _uow;

        public ProductsController(IProductService productService, IUnitOfWork uow)
        {
            _productService = productService;
            _uow = uow;

        }

        public async Task<IActionResult> Index()
        {
            var products = _productService.GetAllAsync();
            var productVMs = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName ?? "(n/a)",
                ProductGroupId = p.ProductGroupId,
                ProductGroupName = p.ProductGroupName ?? "(n/a)",
                Cost = p.Cost,
                TaxPercentage = p.TaxPercentage,
                IsActive = p.IsActive
            }).ToList();
            return View(productVMs);
        }

        public async Task<IActionResult> Details(int id)
        {
            //var product = await _uow.Products.GetByIdAsync(id);
            var product = await _productService.GetByIdAsync(id);
            var productVM = new ProductViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                ProductGroupId = product.ProductGroupId,
                Cost = product.Cost,
                TaxPercentage = product.TaxPercentage,
                IsActive = product.IsActive
            };
            if (product == null) return NotFound();
            return View(productVM);
        }

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

            var dto = new ProductCreateDto
            {
                ProductName = model.ProductName,
                CategoryId = model.CategoryId,
                ProductGroupId = model.ProductGroupId,
                Cost = model.Cost,
                TaxPercentage = model.TaxPercentage
            };

            await _productService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));

        }

        // Auto Fill Category & Product Group
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false });
            }

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                categoryId = product.CategoryId,
                categoryName = product.CategoryName,
                productGroupId = product.ProductGroupId,
                productGroupName = product.ProductGroupName,
                cost = product.Cost,
                tax = product.TaxPercentage
            });
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreate(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed" });

            var dto = new ProductCreateDto
            {
                ProductName = model.ProductName,
                CategoryId = model.CategoryId,
                ProductGroupId = model.ProductGroupId,
                Cost = model.Cost,
                TaxPercentage = model.TaxPercentage
            };

            var newProductId = await _productService.CreateAsync(dto);

            // Fetch the created product details to return full info to caller
            var created = await _productService.GetByIdAsync(newProductId);

            if (created == null)
            {
                return Json(new { success = false, message = "Failed to load created product" });
            }

            return Json(new
            {
                success = true,
                id = newProductId,
                name = created.ProductName,
                categoryId = created.CategoryId,
                categoryName = created.CategoryName,
                productGroupId = created.ProductGroupId,
                productGroupName = created.ProductGroupName,
                cost = created.Cost,
                tax = created.TaxPercentage
            });
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