using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace seashore_CRM.DAL.Repositories
{
    public class IndividualCustomerRepository : IIndividualCustomerRepository
    {
        private readonly AppDbContext _context;

        public IndividualCustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IndividualCustomer?> GetByIdAsync(int id)
        {
            return await _context.Set<IndividualCustomer>().IgnoreQueryFilters().FirstOrDefaultAsync(ic => ic.Id == id);
        }

        public IQueryable<IndividualCustomer> GetAllExceptInactive()
        {
            return _context.Set<IndividualCustomer>().AsQueryable();
        }
        public IQueryable<IndividualCustomer> GetAllAsync()
        {
            return _context.Set<IndividualCustomer>().IgnoreQueryFilters().AsQueryable();
        }

        public async Task<IEnumerable<IndividualCustomer>> FindAsync(Expression<System.Func<IndividualCustomer, bool>> predicate)
        {
            return await Task.FromResult(_context.Set<IndividualCustomer>().IgnoreQueryFilters().Where(predicate).AsEnumerable());
        }

        public async Task AddAsync(IndividualCustomer entity)
        {
            await _context.Set<IndividualCustomer>().AddAsync(entity);
        }

        public void Update(IndividualCustomer entity)
        {
            _context.Set<IndividualCustomer>().Update(entity);
        }

        public void Remove(IndividualCustomer entity)
        {
            _context.Set<IndividualCustomer>().Remove(entity);
        }
    }
}