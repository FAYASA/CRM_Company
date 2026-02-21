using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
        {
            return await _dbSet.Where(c => c.CompanyId == companyId).ToListAsync();
        }
    }
}
