using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);

        Task<List<Company>> GetAllAsync();

        Task<List<Company>> SearchAsync(string? query);

        Task AddAsync(Company company);

        void Update(Company company);

        void SoftDelete(Company company);
    }
}
