using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<int> AddAsync(Invoice entity);
        Task UpdateAsync(Invoice entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
    }
}
