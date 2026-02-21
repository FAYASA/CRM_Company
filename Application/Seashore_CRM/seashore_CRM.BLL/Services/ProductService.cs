using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;

        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Product entity)
        {
            await _uow.Products.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Products.GetByIdAsync(id);
            if (e == null) return;
            _uow.Products.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _uow.Products.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _uow.Products.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _uow.Products.GetByCategoryIdAsync(categoryId);
        }

        public async Task UpdateAsync(Product entity)
        {
            _uow.Products.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
