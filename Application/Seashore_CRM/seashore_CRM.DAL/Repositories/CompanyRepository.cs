using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Company> GetAllAsync()
        {
            return _context.Companies.IgnoreQueryFilters().AsNoTracking();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Company> SearchAsync(string query)
        {
            IQueryable<Company> companies = _context.Companies.IgnoreQueryFilters();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();

                companies = companies.Where(c =>
                    c.CompanyName.ToLower().Contains(query) ||
                    (c.City != null && c.City.ToLower().Contains(query)) ||
                    (c.Email != null && c.Email.ToLower().Contains(query)));
            }

            return companies.AsNoTracking();
        }

        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
        }

        public void Update(Company company)
        {
            _context.Companies.Update(company);
        }

        public void SoftDelete(Company company)
        {
            company.IsActive = false;
            _context.Companies.Update(company);
        }
    }
}
