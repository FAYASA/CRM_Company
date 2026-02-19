using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;

namespace Seashore_CRM.Pages.Companies
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public DetailsModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Company Company { get; set; } = new Company();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var company = await _uow.Companies.GetByIdAsync(id);
            if (company == null) return RedirectToPage("Index");

            Company = company;
            return Page();
        }
    }
}