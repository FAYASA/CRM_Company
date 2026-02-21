using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<int> AddAsync(Contact entity);
        Task UpdateAsync(Contact entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId);
    }
}
