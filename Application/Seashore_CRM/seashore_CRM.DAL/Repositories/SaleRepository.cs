using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            // Sale entity uses `SaleDate` property
            return await _dbSet.Where(s => s.SaleDate >= from && s.SaleDate <= to).ToListAsync();
        }
    }
}
