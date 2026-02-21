using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class SaleItemService : ISaleItemService
    {
        private readonly IUnitOfWork _uow;

        public SaleItemService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(SaleItem entity)
        {
            await _uow.SaleItems.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.SaleItems.GetByIdAsync(id);
            if (e == null) return;
            _uow.SaleItems.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<SaleItem>> GetAllAsync()
        {
            return await _uow.SaleItems.GetAllAsync();
        }

        public async Task<SaleItem?> GetByIdAsync(int id)
        {
            return await _uow.SaleItems.GetByIdAsync(id);
        }

        public async Task UpdateAsync(SaleItem entity)
        {
            _uow.SaleItems.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
