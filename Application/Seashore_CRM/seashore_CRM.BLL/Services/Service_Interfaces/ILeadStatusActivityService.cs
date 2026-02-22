using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadStatusActivityService
    {
        Task<IEnumerable<LeadStatusActivity>> GetAllAsync();
        Task<LeadStatusActivity?> GetByIdAsync(int id);
        Task<int> AddAsync(LeadStatusActivity entity);
        Task UpdateAsync(LeadStatusActivity entity);
        Task DeleteAsync(int id);
    }
}
