using System.Collections.Generic;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;
using System.Linq.Expressions;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task AddAsync(User entity);
        void Update(User entity);
        void Remove(User entity);

        // Add user-specific methods
    }
}
