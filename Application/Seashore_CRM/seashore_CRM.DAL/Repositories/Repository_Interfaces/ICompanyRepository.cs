using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<IEnumerable<Company>> FindAsync(Expression<Func<Company, bool>> predicate);
        Task AddAsync(Company entity);
        void Update(Company entity);
        void Remove(Company entity);

        // Add company-specific methods here
    }
}
