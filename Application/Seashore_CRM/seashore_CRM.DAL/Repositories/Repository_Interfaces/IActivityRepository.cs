using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IActivityRepository
    {
        Task<Activity?> GetByIdAsync(int id);
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<IEnumerable<Activity>> FindAsync(Expression<Func<Activity, bool>> predicate);
        Task AddAsync(Activity entity);
        void Update(Activity entity);
        void Remove(Activity entity);
    }
}
