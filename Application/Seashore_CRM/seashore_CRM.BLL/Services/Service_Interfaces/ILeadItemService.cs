using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadItemService
    {
        Task<IEnumerable<LeadItem>> GetAllAsync();
        Task<LeadItem?> GetByIdAsync(int id);
        Task<int> AddAsync(LeadItem entity);
        Task UpdateAsync(LeadItem entity);
        Task DeleteAsync(int id);
    }
}
