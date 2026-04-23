using System.Threading.Tasks;
using Seashore_CRM.ViewModels.Lead;
using seashore_CRM.BLL.DTOs;

namespace Seashore_CRM.Services
{
    public interface ILeadViewModelService
    {
        Task<LeadCreateViewModel> BuildLeadCreateViewModelAsync(LeadDto? model = null);
    }
}
