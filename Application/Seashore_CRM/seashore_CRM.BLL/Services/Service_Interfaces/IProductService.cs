using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<int> AddAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    }
}
