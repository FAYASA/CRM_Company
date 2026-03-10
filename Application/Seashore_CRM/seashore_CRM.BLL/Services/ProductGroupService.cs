using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IUnitOfWork _uow;

        public ProductGroupService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(ProductGroup entity)
        {
            await _uow.ProductGroups.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.ProductGroups.GetByIdAsync(id);
            if (e == null) return;
            e.IsActive = false; // soft delete
            _uow.ProductGroups.Update(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            var all = await _uow.ProductGroups.GetAllAsync();
            return all;
        }

        public async Task<ProductGroup?> GetByIdAsync(int id)
        {
            return await _uow.ProductGroups.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ProductGroup>> GetByCategoryIdAsync(int categoryId)
        {
            return await _uow.ProductGroups.GetByCategoryIdAsync(categoryId);
        }

        public async Task UpdateAsync(ProductGroup entity)
        {
            _uow.ProductGroups.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
