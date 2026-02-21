using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ISaleItemService
    {
        Task<IEnumerable<SaleItem>> GetAllAsync();
        Task<SaleItem?> GetByIdAsync(int id);
        Task<int> AddAsync(SaleItem entity);
        Task UpdateAsync(SaleItem entity);
        Task DeleteAsync(int id);
    }
}
