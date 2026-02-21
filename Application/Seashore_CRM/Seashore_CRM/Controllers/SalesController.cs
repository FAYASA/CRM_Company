using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.Entities;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Seashore_CRM.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly ICompanyService _companyService;
        private readonly IOpportunityService _opportunityService;
        private readonly IProductService _productService;
        private readonly ISaleItemService _saleItemService;
        private readonly IUnitOfWork _uow;

        public SalesController(
            ISaleService saleService,
            ICompanyService companyService,
            IOpportunityService opportunityService,
            IProductService productService,
            ISaleItemService saleItemService,
            IUnitOfWork uow)
        {
            _saleService = saleService;
            _companyService = companyService;
            _opportunityService = opportunityService;
            _productService = productService;
            _saleItemService = saleItemService;
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _saleService.GetAllAsync();
            return View(items.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null) return NotFound();
            return View(sale);
        }

        public async Task<IActionResult> Create(int? opportunityId)
        {
            var companies = await _companyService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            ViewBag.Companies = new SelectList(companies, "Id", "CompanyName");
            ViewBag.Products = products.Select(p => new SelectListItem(p.ProductName, p.Id.ToString())).ToList();

            // If opportunityId provided, prefill from opportunity -> lead -> lead items
            if (opportunityId.HasValue)
            {
                var opp = await _opportunityService.GetByIdAsync(opportunityId.Value);
                if (opp != null)
                {
                    var lead = await _uow.Leads.GetByIdAsync(opp.LeadId);
                    if (lead != null && lead.CompanyId.HasValue)
                    {
                        var sale = new Sale { CustomerId = lead.CompanyId.Value, OpportunityId = opp.Id, SaleDate = DateTime.UtcNow };

                        var leadItems = (await _uow.LeadItems.FindAsync(li => li.LeadId == lead.Id)).ToList();
                        ViewBag.PrefillItems = leadItems.Select(li => new SaleItem
                        {
                            ProductId = li.ProductId,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TaxPercentage = li.TaxPercentage,
                            LineTotal = li.LineTotal
                        }).ToList();

                        return View(sale);
                    }
                }
            }

            var model = new Sale();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale, List<SaleItem> items)
        {
            if (!ModelState.IsValid)
            {
                var companies = await _companyService.GetAllAsync();
                var products = await _productService.GetAllAsync();
                ViewBag.Companies = new SelectList(companies, "Id", "CompanyName", sale.CustomerId);
                ViewBag.Products = products.Select(p => new SelectListItem(p.ProductName, p.Id.ToString())).ToList();
                return View(sale);
            }

            await _saleService.AddAsync(sale);

            if (items != null && items.Any())
            {
                foreach (var it in items)
                {
                    it.SaleId = sale.Id;
                    await _saleItemService.AddAsync(it);
                }
            }

            return RedirectToAction("Details", new { id = sale.Id });
        }

        public async Task<IActionResult> CreateFromOpportunity(int opportunityId)
        {
            var opp = await _opportunityService.GetByIdAsync(opportunityId);
            if (opp == null) return NotFound();

            // Prefill logic: redirect to Create which will prefill from opportunity
            return RedirectToAction(nameof(Create), new { opportunityId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var s = await _saleService.GetByIdAsync(id);
            if (s == null) return NotFound();
            return View(s);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _saleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
