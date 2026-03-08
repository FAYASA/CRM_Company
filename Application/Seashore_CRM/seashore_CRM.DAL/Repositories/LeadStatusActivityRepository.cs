using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace seashore_CRM.DAL.Repositories
{
    public class LeadStatusActivityRepository : Repository<LeadStatusActivity>, ILeadStatusActivityRepository
    {
        public LeadStatusActivityRepository(AppDbContext context) : base(context)
        {
        }
    }
}
