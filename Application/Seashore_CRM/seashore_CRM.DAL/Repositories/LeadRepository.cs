using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    // Dedicated Lead repository implementation that does not rely on a shared generic repository.
    public class LeadRepository : ILeadRepository
    {
        private readonly AppDbContext _context;

        public LeadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Lead entity)
        {
            await _context.Leads.AddAsync(entity);
        }

        public async Task<IEnumerable<Lead>> FindAsync(Expression<System.Func<Lead, bool>> predicate)
        {
            return await Task.FromResult(_context.Leads.Where(predicate).AsEnumerable());
        }

        public async Task<IEnumerable<Lead>> GetAllAsync()
        {
            return await _context.Leads.ToListAsync();
        }

        // Eager-load navigations commonly used by views to avoid missing fields and N+1 queries
        public async Task<Lead?> GetByIdAsync(int? id)
        {
            if (!id.HasValue) return null;

            return await _context.Leads
                .IgnoreQueryFilters()
                .Include(l => l.Company)
                .Include(l => l.Contact)
                .Include(l => l.IndividualCustomer)
                .Include(l => l.AssignedUser)
                .Include(l => l.Status)
                .Include(l => l.LeadItems)
                    .ThenInclude(li => li.Product)
                        .ThenInclude(p => p.ProductGroup)
                .Include(l => l.LeadItems)
                    .ThenInclude(li => li.Product)
                        .ThenInclude(p => p.Category)
                .Include(l => l.LeadStatusActivities)
                .Include(l => l.UserLeadRights)
                .FirstOrDefaultAsync(l => l.Id == id.Value);
        }

        public void Remove(Lead entity)
        {
            _context.Leads.Remove(entity);
        }

        public void Update(Lead entity)
        {
            _context.Leads.Update(entity);
        }

        public async Task<IEnumerable<Lead>> GetByStatusIdAsync(int statusId)
        {
            return await _context.Leads.Where(l => l.StatusId == statusId).ToListAsync();
        }
    }
}
