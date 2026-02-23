using Microsoft.AspNetCore.Mvc;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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
        public async Task<IActionResult> Index(string q, string sortBy = "CompanyName", string sortOrder = "asc", int page = 1, int pageSize = 20)
        {
            var companies = string.IsNullOrWhiteSpace(q)
                ? await _service.GetAllAsync()
                : await _service.SearchAsync(q);

            // apply sorting
            companies = SortCompanies(companies, sortBy, sortOrder);

            // provide view with the same keys Index.cshtml expects
            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = companies.Count;
            ViewBag.TotalPages = 1; // adjust if you introduce real paging

            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View(companies);
        }

        private List<CompanyListDto> SortCompanies(List<CompanyListDto> list, string sortBy, string sortOrder)
        {
            if (list == null || list.Count == 0) return list;

            bool asc = string.Equals(sortOrder, "asc", System.StringComparison.OrdinalIgnoreCase);

            return sortBy switch
            {
                "City" => asc ? list.OrderBy(c => c.City).ToList() : list.OrderByDescending(c => c.City).ToList(),
                "Address" => asc ? list.OrderBy(c => c.Address).ToList() : list.OrderByDescending(c => c.Address).ToList(),
                "Status" => asc ? list.OrderBy(c => c.IsActive).ToList() : list.OrderByDescending(c => c.IsActive).ToList(),
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

            return View(company);
        }

        // ============================
        // CREATE
        // ============================
        public IActionResult Create()
        {
            return View(new CompanyCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyCreateDto dto)
        {
            if (await _service.IsCompanyNameTakenAsync(dto.CompanyName))
                ModelState.AddModelError(nameof(dto.CompanyName), "Company name already exists.");

            if (await _service.IsEmailTakenAsync(dto.Email))
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");

            if (await _service.IsCompanyPhoneTakenAsync(dto.Phone))
                ModelState.AddModelError(nameof(dto.Phone), "Phone Number already exists.");

            if (!ModelState.IsValid)
                return View(dto);

            await _service.CreateAsync(dto);

            TempData["Success"] = "Company created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // EDIT
        // ============================
        public async Task<IActionResult> Edit(int id)
        {
            var detail = await _service.GetByIdAsync(id);
            if (detail == null) return NotFound();

            var dto = new CompanyUpdateDto
            {
                Id = detail.Id,
                CompanyName = detail.CompanyName,
                Address = detail.Address,
                AddressPost = detail.AddressPost,
                Pin = detail.Pin,
                City = detail.City,
                //Country = detail.Country,
                Phone = detail.Phone,
                Email = detail.Email,
                IsActive = detail.IsActive,
                Website = detail.Website,
                Industry = detail.Industry
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (await _service.IsCompanyNameTakenAsync(dto.CompanyName, dto.Id))
                ModelState.AddModelError(nameof(dto.CompanyName), "Company name already exists.");

            if (await _service.IsEmailTakenAsync(dto.Email, dto.Id))
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");

            if (await _service.IsCompanyPhoneTakenAsync(dto.Phone, dto.Id))
                ModelState.AddModelError(nameof(dto.Phone), "Phone Number already exists.");

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.UpdateAsync(dto);

            if (!result)
                return NotFound();

            TempData["Success"] = "Company updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DELETE
        // ============================
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _service.GetByIdAsync(id);
            if (company == null) return NotFound();

            return View(company);
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
        // Remote validation endpoints (used by RemoteAttribute)
        // ============================
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email, int? id)
        {
            var taken = await _service.IsEmailTakenAsync(email, id);
            if (taken)
                return Json($"Email '{email}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyCompanyName(string companyName, int? id)
        {
            var taken = await _service.IsCompanyNameTakenAsync(companyName, id);
            if (taken)
                return Json($"Company name '{companyName}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyCompanyPhone(string companyPhone, int? id)
        {
            var taken = await _service.IsCompanyPhoneTakenAsync(companyPhone, id);
            if (taken)
                return Json($"Phone Number '{companyPhone}' is already in use.");
            return Json(true);
        }
    }
}