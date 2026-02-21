using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadStatusService
    {
        Task<IEnumerable<LeadStatus>> GetAllAsync();
        Task<LeadStatus?> GetByIdAsync(int id);
        Task<int> AddAsync(LeadStatus entity);
        Task UpdateAsync(LeadStatus entity);
        Task DeleteAsync(int id);
    }
}
