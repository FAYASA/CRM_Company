using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.BLL.DTOs;
using Seashore_CRM.ViewModels.Contact;

namespace Seashore_CRM.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ICompanyService _companyService;

        public ContactsController(
            IContactService contactService,
            ICompanyService companyService)
        {
            _contactService = contactService;
            _companyService = companyService;
        }

        // ===============================
        // INDEX
        // ===============================
        public async Task<IActionResult> Index(
            string? q,
            int? companyId,
            bool? isActive,
            int page = 1,
            int pageSize = 20)
        {
            var dtos =  _contactService.GetAllAsync();

            //  Search filter
            if (!string.IsNullOrWhiteSpace(q))
            {
                dtos = dtos.Where(c =>
                    EF.Functions.Like(c.ContactName, $"%{q}%") ||
                    EF.Functions.Like(c.Email, $"%{q}%") ||
                    EF.Functions.Like(c.Phone, $"%{q}%")
                );
            }

            //  Company filter
            if (companyId.HasValue)
            {
                dtos = dtos.Where(c => c.CompanyId == companyId.Value);
            }

            //  Status filter
            if (isActive.HasValue)
            {
                dtos = dtos.Where(c => c.IsActive == isActive);
            }

            //  Pagination
            var totalCount = dtos.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedDtos = dtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //  DTO → ViewModel mapping
            var viewModels = pagedDtos.Select(c => new ContactListViewModel
            {
                Id = c.Id,
                ContactName = c.ContactName,
                Email = c.Email,
                Phone = c.Phone,
                Mobile = c.Mobile,
                Designation = c.Designation,
                CompanyName = c.CompanyName,
                IsActive = c.IsActive,
                CompanyId = c.CompanyId
            }).ToList();

            // ViewBag values
            ViewBag.Query = q;
            ViewBag.CompanyId = companyId;
            ViewBag.IsActive = isActive;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            await LoadCompanies();

            return View(viewModels);
        }

        // ===============================
        // DETAILS
        // ===============================
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            var vm = new ContactDetailsViewModel
            {
                Id = contact.Id,
                CompanyName = contact.CompanyName,
                ContactName = contact.ContactName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Designation = contact.Designation,
                IsActive = contact.IsActive
            };

            return View(vm);
        }

        // ===============================
        // CREATE (GET)
        // ===============================
        public async Task<IActionResult> Create()
        {
            await LoadCompanies();
            return View();
        }

        // ===============================
        // CREATE (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactCreateViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                await LoadCompanies();
                return View(cvm);
            }

            var dto = new ContactCreateDto
            {
                ContactName = cvm.ContactName,
                Email = cvm.Email,
                Phone = cvm.Phone,
                Mobile = cvm.Mobile,
                Designation = cvm.Designation,
                CompanyId = cvm.CompanyId
            };

            await _contactService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDIT (GET)
        // ===============================
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            var dto = new ContactUpdateViewModel
                {
                ContactId = contact.Id,
                ContactName = contact.ContactName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Designation = contact.Designation,
                CompanyId = contact.CompanyId,
                IsActive = contact.IsActive,
                RowVersion = contact.RowVersion
            };

            await LoadCompanies();
            return View(dto);
        }

        // ===============================
        // EDIT (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactUpdateViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                await LoadCompanies();
                return View(cvm);
            }

            var dto = new ContactUpdateDto
            {
                Id = cvm.ContactId,
                ContactName = cvm.ContactName,
                Email = cvm.Email,
                Phone = cvm.Phone,
                Mobile = cvm.Mobile,
                Designation = cvm.Designation,
                CompanyId = cvm.CompanyId,
                IsActive = cvm.IsActive,
                RowVersion = cvm.RowVersion
            };

            await _contactService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // DELETE
        // ===============================
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            return View(contact);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contactService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // HELPER
        // ===============================
        private async Task LoadCompanies()
        {
            var companies = _companyService.GetAllAsync();
            ViewBag.Companies = new SelectList(
                companies,
                "Id",
                "CompanyName",
                ViewBag.CompanyId   // preserve selection
            );
        }
    }
}