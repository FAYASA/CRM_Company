using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;

namespace Seashore_CRM.Pages.Companies
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public CreateModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [BindProperty]
        public Company Company { get; set; } = new Company();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _uow.Companies.AddAsync(Company);
            await _uow.CommitAsync();

            return RedirectToPage("Index");
        }
    }
}