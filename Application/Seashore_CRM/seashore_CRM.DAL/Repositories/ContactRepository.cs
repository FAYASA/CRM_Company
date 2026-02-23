using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Contact?> GetWithCompanyAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contact>> GetAllWithCompanyAsync()
    {
        return await _dbSet
            .Include(c => c.Company)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }
}