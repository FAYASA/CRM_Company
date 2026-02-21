using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface ILeadRepository
    {
        Task<Lead?> GetByIdAsync(int id);
        Task<IEnumerable<Lead>> GetAllAsync();
        Task<IEnumerable<Lead>> FindAsync(Expression<Func<Lead, bool>> predicate);
        Task AddAsync(Lead entity);
        void Update(Lead entity);
        void Remove(Lead entity);

        // Define Lead-specific methods here, for example:
        // Task<IEnumerable<Lead>> GetByStatusAsync(int statusId);
    }
}
