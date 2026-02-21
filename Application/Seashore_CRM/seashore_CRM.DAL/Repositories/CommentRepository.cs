using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetByEntityIdAsync(int entityId)
        {
            // Comment entity has LeadId and CustomerId; return by LeadId for example
            return await _dbSet.Where(c => c.LeadId == entityId).ToListAsync();
        }
    }
}
