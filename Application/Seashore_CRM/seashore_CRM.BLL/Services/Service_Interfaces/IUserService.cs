using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);
        Task ToggleStatusAsync(int id);

        // Validation helpers
        Task<bool> IsEmailTakenAsync(string email, int? excludeId = null);
        Task<bool> IsFullNameTakenAsync(string fullName, int? excludeId = null);
        Task<bool> IsContactTakenAsync(string contact, int? excludeId = null);
    }
}
