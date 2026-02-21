using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
    }
}
