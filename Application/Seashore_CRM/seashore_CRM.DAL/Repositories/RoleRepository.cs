using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace seashore_CRM.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get a Role by Id
        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.IgnoreQueryFilters().FirstAsync(r => r.Id == id);
        }

        // Get all Roles
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.IgnoreQueryFilters().
                ToListAsync();
        }

        // Find Roles using a predicate
        public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _context.Roles.IgnoreQueryFilters().
                Where(predicate).ToListAsync();
        }

        // Add a new Role
        public async Task AddAsync(Role entity)
        {
            await _context.Roles.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Update an existing Role
        public void Update(Role entity)
        {
            _context.Roles.Update(entity);
            _context.SaveChanges();
        }

        // Remove a Role
        public void Remove(Role entity)
        {
            _context.Roles.Remove(entity);
            _context.SaveChanges();
        }
    }
}