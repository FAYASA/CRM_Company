using System.Collections.Generic;
using System.Threading.Tasks;
using seashore_CRM.DomainModelLayer.Entities;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserLeadRightsService
    {
        Task<IEnumerable<UserLeadRights>> ListAsync(int? leadId = null, int? userId = null);
        Task<UserLeadRights?> GetAsync(int id);
        Task<int> SaveAsync(UserLeadRights entity);
        Task<bool> DeleteAsync(int id);
    }
}