using seashore_CRM.Models.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyListDto>> GetAllAsync();
        Task<List<CompanyListDto>> SearchAsync(string? query);
        Task<CompanyDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CompanyCreateDto dto);
        Task<bool> UpdateAsync(CompanyUpdateDto dto);
        Task DeleteAsync(int id);

        // Validation helpers
        Task<bool> IsEmailTakenAsync(string email, int? excludeId = null);
        Task<bool> IsCompanyNameTakenAsync(string companyName, int? excludeId = null);

        Task<bool> IsCompanyPhoneTakenAsync(string companyPhone, int? excludeId = null);
    }
}
