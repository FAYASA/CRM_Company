using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    using seashore_CRM.Models.Entities;

    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate);
        Task AddAsync(Role entity);
        void Update(Role entity);
        void Remove(Role entity);
    }
}
