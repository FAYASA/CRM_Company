using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class OpportunityRepository : Repository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(AppDbContext context) : base(context)
        {
        }
    }
}
