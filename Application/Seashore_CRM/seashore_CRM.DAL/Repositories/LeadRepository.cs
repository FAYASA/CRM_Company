using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class LeadRepository : Repository<Lead>, ILeadRepository
    {
        public LeadRepository(AppDbContext context) : base(context)
        {
        }

        // Add Lead-specific methods here. Example:
        public async Task<IEnumerable<Lead>> GetByStatusIdAsync(int statusId)
        {
            // Lead entity uses `StatusId` property
            return await _dbSet.Where(l => l.StatusId == statusId).ToListAsync();
        }
    }
}
