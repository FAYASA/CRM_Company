using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactListDto>> GetAllAsync();
        Task<ContactDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ContactCreateDto dto);
        Task UpdateAsync(ContactUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
