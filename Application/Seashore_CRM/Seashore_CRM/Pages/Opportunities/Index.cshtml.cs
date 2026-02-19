using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Seashore_CRM.Pages.Opportunities
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public IndexModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IEnumerable<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            var all = (await _uow.Opportunities.GetAllAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                all = all.Where(o => o.Stage.Contains(Search) || o.LeadId.ToString() == Search);
            }

            var count = all.Count();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            Opportunities = all.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}