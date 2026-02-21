using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            // Invoice is linked to Sale -> Sale.CustomerId; include Sale and filter
            return await _dbSet.Include(i => i.Sale)
                               .Where(i => i.Sale != null && i.Sale.CustomerId == customerId)
                               .ToListAsync();
        }
    }
}
