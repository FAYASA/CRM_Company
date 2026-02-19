using Microsoft.AspNetCore.Mvc.RazorPages;
using seashore_CRM.BLL.Services;
using seashore_CRM.Models.Entities;
using seashore_CRM.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seashore_CRM.Pages.Companies
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        public IndexModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IEnumerable<Company> Companies { get; set; } = new List<Company>();

        public async Task OnGetAsync()
        {
            Companies = await _uow.Companies.GetAllAsync();
        }
    }
}