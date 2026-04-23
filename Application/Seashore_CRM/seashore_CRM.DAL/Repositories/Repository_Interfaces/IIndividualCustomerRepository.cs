using seashore_CRM.DomainModelLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IIndividualCustomerRepository
    {
        Task<IndividualCustomer?> GetByIdAsync(int id);
        IQueryable<IndividualCustomer> GetAllAsync();
        IQueryable<IndividualCustomer> GetAllExceptInactive();
        Task<IEnumerable<IndividualCustomer>> FindAsync(Expression<System.Func<IndividualCustomer, bool>> predicate);
        Task AddAsync(IndividualCustomer entity);
        void Update(IndividualCustomer entity);
        void Remove(IndividualCustomer entity);
    }
}