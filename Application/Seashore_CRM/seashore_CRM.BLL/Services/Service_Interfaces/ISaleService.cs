using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<int> AddAsync(Sale entity);
        Task UpdateAsync(Sale entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
