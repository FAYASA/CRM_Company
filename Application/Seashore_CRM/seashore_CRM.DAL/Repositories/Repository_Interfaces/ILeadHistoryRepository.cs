using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface ILeadHistoryRepository : IRepository<LeadHistory>
    {
        Task<IEnumerable<LeadHistory>> FindAsync(Expression<Func<LeadHistory, bool>> predicate);
    }
}
