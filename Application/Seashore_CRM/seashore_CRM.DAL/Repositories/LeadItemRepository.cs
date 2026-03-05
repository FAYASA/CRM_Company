using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class LeadItemRepository : Repository<LeadItem>, ILeadItemRepository
    {
        public LeadItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
