using seashore_CRM.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserAppService
    {
        Task<List<UserListDto>> GetAllAsync();
        Task<UserDetailDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(UserCreateDto dto);
        Task<bool> UpdateAsync(UserUpdateDto dto);
        Task DeleteAsync(string id);
    }
}