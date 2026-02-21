using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<int> AddAsync(Payment entity);
        Task UpdateAsync(Payment entity);
        Task DeleteAsync(int id);
    }
}
