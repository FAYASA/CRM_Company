using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IContactService
    {
        IQueryable<ContactListDto> GetAllAsync();
        IQueryable<ContactListDto> GetAllActiveAsync();
        Task<IQueryable<ContactListDto>> GetAllActiveByCompanyIdAsync(int companyId);
        //IQueryable<ContactListDto> SearchAsync(string? query);  
        //Task<IEnumerable<ContactListDto>> GetAllAsync();
        Task<ContactDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ContactCreateDto dto);
        Task UpdateAsync(ContactUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
