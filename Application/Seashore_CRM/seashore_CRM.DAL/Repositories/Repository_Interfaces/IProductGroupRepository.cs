using seashore_CRM.DomainModelLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.DataLayer.Repositories.Repository_Interfaces
{
    public interface IProductGroupRepository
    {
        Task<IEnumerable<ProductGroup>> GetAllAsync();

        // Optional: Add by category directly (cleaner)
        Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId);
    }
}
