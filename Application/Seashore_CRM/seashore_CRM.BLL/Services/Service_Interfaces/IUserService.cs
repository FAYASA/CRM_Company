using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<int> AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);
    }
}
