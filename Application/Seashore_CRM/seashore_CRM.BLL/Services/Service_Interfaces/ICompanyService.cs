using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int id);
        Task<int> AddAsync(Company entity);
        Task UpdateAsync(Company entity);
        Task DeleteAsync(int id);
    }
}
