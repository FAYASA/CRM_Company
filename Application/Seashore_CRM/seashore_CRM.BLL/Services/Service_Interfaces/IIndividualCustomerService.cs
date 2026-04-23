using seashore_CRM.BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IIndividualCustomerService
    {
        IQueryable<IndividualCustomerListDto> GetAll();
        IQueryable<IndividualCustomerListDto> GetAllExceptInactive();
        Task<IndividualCustomerDetailDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(IndividualCustomerCreateDto dto);
        Task UpdateAsync(IndividualCustomerUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
