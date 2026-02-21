using seashore_CRM.Models.Entities;
using seashore_CRM.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ILeadService
    {
        Task<IEnumerable<LeadDto>> GetAllLeadsAsync();
        Task<LeadDto?> GetLeadByIdAsync(int id);
        Task<int> AddLeadAsync(LeadDto leadDto);
        Task UpdateLeadAsync(LeadDto leadDto);
        Task DeleteLeadAsync(int id);
        Task QualifyLeadAsync(LeadDto dto);
        Task<int?> ConvertToOpportunityAsync(int leadId);
    }
}
