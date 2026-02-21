using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _uow;

        public InvoiceService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Invoice entity)
        {
            await _uow.Invoices.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Invoices.GetByIdAsync(id);
            if (e == null) return;
            _uow.Invoices.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _uow.Invoices.GetAllAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _uow.Invoices.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await _uow.Invoices.GetByCustomerIdAsync(customerId);
        }

        public async Task UpdateAsync(Invoice entity)
        {
            _uow.Invoices.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
