using System.Linq.Expressions;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(int id);

        Task<Contact?> GetWithCompanyAsync(int id);   // include Company

        Task<IEnumerable<Contact>> GetAllAsync();

        Task<IEnumerable<Contact>> GetAllWithCompanyAsync();  // include Company

        Task<IEnumerable<Contact>> FindAsync(Expression<Func<Contact, bool>> predicate);

        Task<IEnumerable<Contact>> GetByCompanyIdAsync(int companyId);

        Task AddAsync(Contact entity);

        void Update(Contact entity);

        void Remove(Contact entity);
    }
}