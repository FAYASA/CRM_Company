using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.IgnoreQueryFilters()
            .Include(p => p.Category)
                // then iclude ProductGroups of the Category
                .ThenInclude(c => c.ProductGroups)
            .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
        }
        public void Update(Product entity)
        {
            _context.Products.Update(entity);
        }
        public void Remove(Product entity)
        {
            _context.Products.Remove(entity);
        }
        public IQueryable<Product> GetAllAsync()
        {
            return _context.Products.IgnoreQueryFilters()
            .Include(p => p.Category)
                // then iclude ProductGroups of the Category
                .ThenInclude(c => c.ProductGroups)
                .AsNoTracking();
        }
        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products.IgnoreQueryFilters()
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
