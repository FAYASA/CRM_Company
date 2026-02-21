using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<int> AddAsync(Role entity);
        Task UpdateAsync(Role entity);
        Task DeleteAsync(int id);
    }
}
