using seashore_CRM.Application.DTOs;
using seashore_CRM.Application.Helper_;
using seashore_CRM.BLL.DTOs;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadService
    {
        // Task<(string result, int? leadId)> CreateLeadAsync(LeadDto dto);
        Task<ServiceResult<int>> CreateLeadAsync(LeadCreateDataDto vm);
        Task<ServiceResult<int>> UpdateLeadAsync(LeadEditDataDto dto);

        //Task UpdateLeadAsync(LeadDto dto);
        Task<IEnumerable<LeadDto>> GetAllLeadsAsync();
        Task<LeadDto?> GetLeadByIdAsync(int id);
        Task DeleteLeadAsync(int id);

        // Build data required by Lead Create/Edit pages as plain DTOs (no MVC types)
        Task<LeadCreateDataDto> BuildLeadCreateDataAsync(int? selectedCategoryId = null, int? selectedCompanyId = null, int? selectedStatusId = null);

        // Simple lookup helpers used by AJAX endpoints
        Task<IEnumerable<OptionDto>> GetContactsByCompanyAsync(int companyId);
        Task<IEnumerable<OptionDto>> GetProductGroupsByCategoryAsync(int categoryId);
        Task<IEnumerable<OptionDto>> GetActivitiesByStatusAsync(int statusId);

        // Build mapping from DB only (status name -> activities)
        Task<Dictionary<string, string[]>> GetStatusActivitiesAsync();

        // New helpers to avoid using UnitOfWork in controller
        Task<IEnumerable<seashore_CRM.Models.Entities.LeadStatusActivity>> GetActivitiesByLeadAsync(int leadId);
        Task<IEnumerable<seashore_CRM.Models.Entities.LeadHistory>> GetHistoryByLeadAsync(int leadId);
        Task<IEnumerable<seashore_CRM.Models.Entities.Comment>> GetCommentsByLeadAsync(int leadId);
        Task<Dictionary<string, string[]>> AddStatusActivityAsync(string statusName, string activityName);
        Task AddActivitiesToLeadAsync(int leadId, IEnumerable<string> activities);
        Task<IEnumerable<seashore_CRM.BLL.DTOs.UserLeadRightDto>> GetUserLeadRightsAsync(int leadId);
    }
}
