using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<Activity?> GetByIdAsync(int id);
        Task<int> AddAsync(Activity entity);
        Task UpdateAsync(Activity entity);
        Task DeleteAsync(int id);
    }
}
