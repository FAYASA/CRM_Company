using seashore_CRM.DomainModelLayer.Entities;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DataLayer.Repositories.Repository_Interfaces
{
    public interface IProductGroupRepository : IRepository<ProductGroup>
    {
        // Optional: Add by category directly (cleaner)
        Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId);
    }
}
