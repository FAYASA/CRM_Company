using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using Seashore_CRM.ViewModels.Company;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seashore_CRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        // ============================
        // INDEX
        // ============================
        public async Task<IActionResult> Index(string q, string sortBy = "CompanyName", string sortOrder = "asc")
        {
            var companiesDto = string.IsNullOrWhiteSpace(q)
                ? await _service.GetAllAsync()
                : await _service.SearchAsync(q);

            // Map DTOs to ListViewModel
            var companies = companiesDto.Select(c => new CompanyListViewModel
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                City = c.City,
                Phone = c.Phone,
                Email = c.Email,
                Industry = c.Industry,
                IsActive = c.IsActive,
                Address = c.Address,
                AddressPost = c.AddressPost,

            }).ToList();

            // Sorting
            companies = SortCompanies(companies, sortBy, sortOrder);

            ViewBag.Query = q;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View(companies);
        }

        private List<CompanyListViewModel> SortCompanies(List<CompanyListViewModel> list, string sortBy, string sortOrder)
        {
            bool asc = string.Equals(sortOrder, "asc", System.StringComparison.OrdinalIgnoreCase);
            return sortBy switch
            {
                "City" => asc ? list.OrderBy(c => c.City).ToList() : list.OrderByDescending(c => c.City).ToList(),
                "Phone" => asc ? list.OrderBy(c => c.Phone).ToList() : list.OrderByDescending(c => c.Phone).ToList(),
                "Industry" => asc ? list.OrderBy(c => c.Industry).ToList() : list.OrderByDescending(c => c.Industry).ToList(),
                _ => asc ? list.OrderBy(c => c.CompanyName).ToList() : list.OrderByDescending(c => c.CompanyName).ToList(),
            };
        }

        // ============================
        // DETAILS
        // ============================
        public async Task<IActionResult> Details(int id)
        {
            var company = await _service.GetByIdAsync(id);
            if (company == null) return NotFound();

            var vm = new CompanyDetailsViewModel
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                City = company.City,
                Phone = company.Phone,
                Email = company.Email,
                Website = company.Website,
                Industry = company.Industry,
                Address = company.Address,
                AddressPost = company.AddressPost,
                Pin = company.Pin,
                IsActive = company.IsActive
                
            };

            return View(vm);
        }

        // ============================
        // CREATE
        // ============================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CompanyCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyCreateViewModel vm)
        {
            if (await _service.IsCompanyNameTakenAsync(vm.CompanyName))
                ModelState.AddModelError(nameof(vm.CompanyName), "Company name already exists.");

            if (await _service.IsEmailTakenAsync(vm.Email))
                ModelState.AddModelError(nameof(vm.Email), "Email already exists.");

            if (await _service.IsCompanyPhoneTakenAsync(vm.Phone))
                ModelState.AddModelError(nameof(vm.Phone), "Phone number already exists.");

            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CompanyCreateDto
            {
                CompanyName = vm.CompanyName,
                Address = vm.Address,
                AddressPost = vm.AddressPost,
                Pin = vm.Pin,
                City = vm.City,
                Phone = vm.Phone,
                Email = vm.Email,
                Website = vm.Website,
                Industry = vm.Industry
            };

            await _service.CreateAsync(dto);

            TempData["Success"] = "Company created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // EDIT
        // ============================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CompanyUpdateViewModel
            {
                Id = dto.Id,
                CompanyName = dto.CompanyName,
                Address = dto.Address,
                AddressPost = dto.AddressPost,
                Pin = dto.Pin,
                City = dto.City,
                Phone = dto.Phone,
                Email = dto.Email,
                Website = dto.Website,
                Industry = dto.Industry,
                IsActive = dto.IsActive,
                RowVersion = dto.RowVersion
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyUpdateViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (await _service.IsCompanyNameTakenAsync(vm.CompanyName, vm.Id))
                ModelState.AddModelError(nameof(vm.CompanyName), "Company name already exists.");

            if (await _service.IsEmailTakenAsync(vm.Email, vm.Id))
                ModelState.AddModelError(nameof(vm.Email), "Email already exists.");

            if (await _service.IsCompanyPhoneTakenAsync(vm.Phone, vm.Id))
                ModelState.AddModelError(nameof(vm.Phone), "Phone number already exists.");

            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CompanyUpdateDto
            {
                Id = vm.Id,
                CompanyName = vm.CompanyName,
                Address = vm.Address,
                AddressPost = vm.AddressPost,
                Pin = vm.Pin,
                City = vm.City,
                Phone = vm.Phone,
                Email = vm.Email,
                Website = vm.Website,
                Industry = vm.Industry,
                IsActive = vm.IsActive,
                RowVersion = vm.RowVersion
            };

            var updated = await _service.UpdateAsync(dto);
            if (!updated) return NotFound();

            TempData["Success"] = "Company updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DELETE
        // ============================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CompanyListViewModel
            {
                Id = dto.Id,
                CompanyName = dto.CompanyName,
                City = dto.City,
                Phone = dto.Phone,
                Email = dto.Email,
                Industry = dto.Industry
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Company deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // Remote validation
        // ============================
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email, int? id)
        {
            var taken = await _service.IsEmailTakenAsync(email, id);
            if (taken) return Json($"Email '{email}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyCompanyName(string companyName, int? id)
        {
            var taken = await _service.IsCompanyNameTakenAsync(companyName, id);
            if (taken) return Json($"Company name '{companyName}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyCompanyPhone(string companyPhone, int? id)
        {
            var taken = await _service.IsCompanyPhoneTakenAsync(companyPhone, id);
            if (taken) return Json($"Phone '{companyPhone}' is already in use.");
            return Json(true);
        }
    }
}