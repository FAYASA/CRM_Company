using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace seashore_CRM.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.IgnoreQueryFilters()
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.IgnoreQueryFilters()
                .Include(u => u.Role) 
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.IgnoreQueryFilters()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }

        public void Remove(User entity)
        {
            _context.Users.Remove(entity);
        }
    }
}