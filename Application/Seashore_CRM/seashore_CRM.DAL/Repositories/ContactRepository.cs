using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Linq.Expressions;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    public async Task AddAsync(Contact entity)
    {
        await _context.Contacts.AddAsync(entity);
    }

    public void Update(Contact entity)
    {
        _context.Contacts.Update(entity);
    }

    public void Remove(Contact entity)
    {
        _context.Contacts.Remove(entity);
    }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task<IEnumerable<Contact>> FindAsync(Expression<Func<Contact, bool>> predicate)
    {
        return await _context.Contacts.Where(predicate).ToListAsync();
    }

    public async Task<Contact?> GetWithCompanyAsync(int id)
    {
        return await _context.Contacts
            .Include(c => c.Company)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contact>> GetAllWithCompanyAsync()
    {
        return await _context.Contacts
            .Include(c => c.Company)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Contacts
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }



    //public async Task<Contact?> GetWithCompanyAsync(int id)
    //{
    //    return await _dbSet
    //        .Include(c => c.Company)
    //        .FirstOrDefaultAsync(c => c.Id == id);
    //}

    //public async Task<IEnumerable<Contact>> GetAllWithCompanyAsync()
    //{
    //    return await _dbSet
    //        .Include(c => c.Company)
    //        .ToListAsync();
    //}

    //public async Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId)
    //{
    //    return await _dbSet
    //        .Where(c => c.CompanyId == companyId)
    //        .ToListAsync();
    //}
}