using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DomainModelLayer.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class UserLeadRightsRepository : Repository<UserLeadRights>, IUserLeadRightsRepository
    {
        public UserLeadRightsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
