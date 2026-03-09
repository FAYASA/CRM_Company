using seashore_CRM.DAL.Data;
using seashore_CRM.DataLayer.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;

namespace seashore_CRM.DAL.Repositories
{
    internal class ProductGroupRepository : IProductGroupRepository
    {
        public ProductGroupRepository(AppDbContext context)
        {
            Context = context;
        }

        public AppDbContext Context { get; }

        public Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}