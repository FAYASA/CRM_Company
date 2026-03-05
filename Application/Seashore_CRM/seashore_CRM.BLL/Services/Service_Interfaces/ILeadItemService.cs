using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadItemService
    {
        Task<IEnumerable<OpportunityItem>> GetAllAsync();
        Task<OpportunityItem?> GetByIdAsync(int id);
        Task<int> AddAsync(OpportunityItem entity);
        Task UpdateAsync(OpportunityItem entity);
        Task DeleteAsync(int id);
    }
}
