using seashore_CRM.DAL.Data;
using seashore_CRM.DataLayer.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories
{
    public class ProductGroupRepository : IProductGroupRepository
    {
        private readonly AppDbContext _context;

        public ProductGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            return await _context.ProductGroups
                .Include(pg => pg.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.ProductGroups
                .Where(pg => pg.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<ProductGroup?> GetByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            return await _context.ProductGroups.FindAsync(id.Value);
        }

        public async Task<IEnumerable<ProductGroup>> FindAsync(System.Linq.Expressions.Expression<System.Func<ProductGroup, bool>> predicate)
        {
            return await _context.ProductGroups.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(ProductGroup entity)
        {
            await _context.ProductGroups.AddAsync(entity);
        }

        public void Update(ProductGroup entity)
        {
            _context.ProductGroups.Update(entity);
        }

        public void Remove(ProductGroup entity)
        {
            _context.ProductGroups.Remove(entity);
        }
    }
}