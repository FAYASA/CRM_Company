using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadSourceService
    {
        Task<IEnumerable<LeadSource>> GetAllAsync();
        Task<LeadSource?> GetByIdAsync(int id);
        Task<int> AddAsync(LeadSource entity);
        Task UpdateAsync(LeadSource entity);
        Task DeleteAsync(int id);
    }
}
