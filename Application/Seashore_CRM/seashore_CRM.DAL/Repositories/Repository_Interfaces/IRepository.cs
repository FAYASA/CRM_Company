using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    // Generic repository interface preserved for backward compatibility.
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
