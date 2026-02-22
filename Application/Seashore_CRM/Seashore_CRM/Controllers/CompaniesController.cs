using Microsoft.AspNetCore.Mvc;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using System;
using System.Collections.Generic;

namespace Seashore_CRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CompaniesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Enhanced Index: search, filter, sort, pagination
        public async Task<IActionResult> Index(string q = null, string sort = "name", string dir = "asc", int page = 1, int pageSize = 20, string status = null)
        {
            var companies = (await _uow.Companies.GetAllAsync()).AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.Trim().ToLowerInvariant();
                companies = companies.Where(c => (c.CompanyName ?? "").ToLower().Contains(qq)
                                                || (c.City ?? "").ToLower().Contains(qq)
                                                || (c.Country ?? "").ToLower().Contains(qq)
                                                || (c.Phone ?? "").ToLower().Contains(qq)
                                                || (c.Email ?? "").ToLower().Contains(qq));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    companies = companies.Where(c => !c.IsDeleted);
                if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    companies = companies.Where(c => c.IsDeleted);
            }

            // Sorting
            var isAsc = dir?.ToLowerInvariant() != "desc";
            companies = sort?.ToLowerInvariant() switch
            {
                "location" => isAsc ? companies.OrderBy(c => c.City) : companies.OrderByDescending(c => c.City),
                "status" => isAsc ? companies.OrderBy(c => c.IsDeleted) : companies.OrderByDescending(c => c.IsDeleted),
                _ => isAsc ? companies.OrderBy(c => c.CompanyName) : companies.OrderByDescending(c => c.CompanyName),
            };

            // Paging
            var total = companies.Count();
            pageSize = Math.Max(5, Math.Min(100, pageSize));
            page = Math.Max(1, page);
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            var items = companies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass state to view
            ViewBag.TotalCount = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Query = q;
            ViewBag.Sort = sort;
            ViewBag.Dir = dir;
            ViewBag.Status = status;

            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        public IActionResult Create()
        {
            return View(new Company());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (!ModelState.IsValid) return View(company);
            await _uow.Companies.AddAsync(company);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id) return BadRequest();
            if (!ModelState.IsValid) return View(company);
            _uow.Companies.Update(company);
            await _uow.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company != null)
            {
                _uow.Companies.Remove(company);
                await _uow.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}