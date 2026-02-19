using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Threading.Tasks;

namespace Seashore_CRM.Pages.Opportunities
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public DeleteModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [BindProperty]
        public Opportunity Opportunity { get; set; } = new Opportunity();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var opp = await _uow.Opportunities.GetByIdAsync(id);
            if (opp == null) return RedirectToPage("Index");
            Opportunity = opp;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var opp = await _uow.Opportunities.GetByIdAsync(Opportunity.Id);
            if (opp != null)
            {
                _uow.Opportunities.Remove(opp);
                await _uow.CommitAsync();
            }
            return RedirectToPage("Index");
        }
    }
}