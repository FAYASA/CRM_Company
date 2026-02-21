using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;

        public PaymentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> AddAsync(Payment entity)
        {
            await _uow.Payments.AddAsync(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _uow.Payments.GetByIdAsync(id);
            if (e == null) return;
            _uow.Payments.Remove(e);
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _uow.Payments.GetAllAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _uow.Payments.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Payment entity)
        {
            _uow.Payments.Update(entity);
            await _uow.CommitAsync();
        }
    }
}
