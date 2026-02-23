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

        public async Task<List<Company>> GetAllAsync()
        {
            // to see all companies, including inactive ones and bypass any global query filters
            return await _context.Companies
                                 .IgnoreQueryFilters()
                                 .AsNoTracking()
                                 .ToListAsync();

        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            // to view all companies (including inactive) bypassing global filters
            return await _context.Companies
                                 .IgnoreQueryFilters()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Company>> SearchAsync(string? query)
        {
            // to see all companies, including deleted/inactive ones, bypass global filters
            IQueryable<Company> companies = _context.Companies.IgnoreQueryFilters();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();

                companies = companies.Where(c =>
                    c.CompanyName.ToLower().Contains(query) ||
                    c.City!.ToLower().Contains(query) ||
                    //c.Country!.ToLower().Contains(query) ||
                    c.Email!.ToLower().Contains(query));
            }

            return await companies.AsNoTracking().ToListAsync();
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
