using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.BLL.Services;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Seashore_CRM.Pages.Leads
{
    public class CreateModel : PageModel
    {
        private readonly ILeadService _leadService;
        private readonly IUnitOfWork _uow;

        public CreateModel(ILeadService leadService, IUnitOfWork uow)
        {
            _leadService = leadService;
            _uow = uow;
        }

        [BindProperty]
        public LeadDto Lead { get; set; } = new LeadDto();

        public IEnumerable<SelectListItem> CompanyList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ContactList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> UserList { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            CompanyList = await GetCompanies();
            ContactList = await GetContacts();
            UserList = await GetUsers();
        }

        private async Task<IEnumerable<SelectListItem>> GetCompanies()
        {
            var list = await _uow.Companies.GetAllAsync();
            var items = new List<SelectListItem>();
            foreach (var c in list)
                items.Add(new SelectListItem(c.CompanyName, c.Id.ToString()));
            return items;
        }

        private async Task<IEnumerable<SelectListItem>> GetContacts()
        {
            var list = await _uow.Contacts.GetAllAsync();
            var items = new List<SelectListItem>();
            foreach (var c in list)
                items.Add(new SelectListItem(c.FirstName + " " + c.LastName, c.Id.ToString()));
            return items;
        }

        private async Task<IEnumerable<SelectListItem>> GetUsers()
        {
            var list = await _uow.Users.GetAllAsync();
            var items = new List<SelectListItem>();
            foreach (var u in list)
                items.Add(new SelectListItem(u.FullName, u.Id.ToString()));
            return items;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CompanyList = await GetCompanies();
                ContactList = await GetContacts();
                UserList = await GetUsers();
                return Page();
            }

            await _leadService.AddLeadAsync(Lead);
            return RedirectToPage("Index");
        }
    }
}