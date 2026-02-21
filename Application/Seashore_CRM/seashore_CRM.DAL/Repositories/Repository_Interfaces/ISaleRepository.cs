using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
