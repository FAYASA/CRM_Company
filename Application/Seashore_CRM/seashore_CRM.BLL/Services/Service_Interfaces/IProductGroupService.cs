using seashore_CRM.DomainModelLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroup>> GetAllAsync();
        Task<ProductGroup?> GetByIdAsync(int id);
        Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId);
        Task<int> AddAsync(ProductGroup entity);
        Task UpdateAsync(ProductGroup entity);
        Task DeleteAsync(int id);
    }
}
