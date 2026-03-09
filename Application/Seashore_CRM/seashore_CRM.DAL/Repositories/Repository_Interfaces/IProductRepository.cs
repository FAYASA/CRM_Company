using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product entity);
        void Update(Product entity);
        void Remove(Product entity);
        IQueryable<Product> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);

    }
}
