using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IOpportunityService
    {
        Task<IEnumerable<Opportunity>> GetAllAsync();
        Task<Opportunity?> GetByIdAsync(int id);
        Task<int> AddAsync(Opportunity entity);
        Task UpdateAsync(Opportunity entity);
        Task DeleteAsync(int id);
    }
}
