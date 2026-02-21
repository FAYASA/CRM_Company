using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.BLL.Services.Service_Interfaces;

namespace seashore_CRM.BLL.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _uow;

        public SaleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Sale entity)
        {
            await _uow.Sales.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Sales.GetByIdAsync(id);
            if (e == null) return;
            _uow.Sales.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _uow.Sales.GetAllAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _uow.Sales.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _uow.Sales.GetByDateRangeAsync(from, to);
        }

        public async Task UpdateAsync(Sale entity)
        {
            _uow.Sales.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
