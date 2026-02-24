using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.Models.DTOs;

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
        public async Task<IActionResult> Index()
        
        {
            var contacts = await _contactService.GetAllAsync();
            return View(contacts);
        }

        // ===============================
        // DETAILS
        // ===============================
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // ===============================
        // CREATE (GET)
        // ===============================
        public async Task<IActionResult> Create()
        {
            await LoadCompanies();
            return View(new ContactCreateDto());
        }

        // ===============================
        // CREATE (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCompanies();
                return View(dto);
            }

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

            var dto = new ContactUpdateDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                ContactName = contact.ContactName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Designation = contact.Designation,
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
        public async Task<IActionResult> Edit(ContactUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCompanies();
                return View(dto);
            }

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
            var companies = await _companyService.GetAllAsync();
            ViewBag.Companies = new SelectList(companies, "Id", "CompanyName");
        }
    }
}