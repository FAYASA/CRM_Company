using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class LeadStatusActivityRepository : Repository<LeadStatusActivity>, ILeadStatusActivityRepository
    {
        public LeadStatusActivityRepository(AppDbContext context) : base(context)
        {
        }
    }
}
