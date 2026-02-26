using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListDto>> GetAllAsync();
        Task<UserDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(UserUpdateDto dto);
        Task DeleteAsync(int id);
        Task ToggleStatusAsync(int id);

        // added for profile management
        Task<ProfileViewDto?> GetProfileAsync(int userId);
        Task UpdateProfileAsync(ProfileUpdateDto dto);
        Task ChangePasswordAsync(ChangePasswordDto dto);

        // Validation helpers
        Task<bool> IsEmailTakenAsync(string email, int? excludeId = null);
        Task<bool> IsFullNameTakenAsync(string fullName, int? excludeId = null);
        Task<bool> IsContactTakenAsync(string contact, int? excludeId = null);
    }
}
