using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    // Align with the generic repository pattern used across the project
    public interface ILeadRepository : IRepository<Lead>
    {
        Task<IEnumerable<Lead>> GetByStatusIdAsync(int statusId);
    }
}
