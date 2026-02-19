using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;

namespace Seashore_CRM.Pages.Companies
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public DeleteModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [BindProperty]
        public Company Company { get; set; } = new Company();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return RedirectToPage("Index");

            Company = company;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var company = await _uow.Companies.GetByIdAsync(Company.Id);
            if (company != null)
            {
                _uow.Companies.Remove(company);
                await _uow.CommitAsync();
            }

            return RedirectToPage("Index");
        }
    }
}