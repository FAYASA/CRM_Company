using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<IEnumerable<Contact>> FindAsync(Expression<Func<Contact, bool>> predicate);
        Task AddAsync(Contact entity);
        void Update(Contact entity);
        void Remove(Contact entity);

        Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId);
    }
}
