using seashore_CRM.Models.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace seashore_CRM.DAL.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Apply migrations
            await context.Database.EnsureCreatedAsync();

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "SalesRep" },
                    new Role { RoleName = "Manager" }
                );
            }

            if (!context.LeadStatuses.Any())
            {
                context.LeadStatuses.AddRange(
                    new LeadStatus { StatusName = "Discover" },
                    new LeadStatus { StatusName = "Quote Given" },
                    new LeadStatus { StatusName = "Follow Up" },
                    new LeadStatus { StatusName = "Forecast" },
                    new LeadStatus { StatusName = "PO Received" },
                    new LeadStatus { StatusName = "Invoicing" },
                    new LeadStatus { StatusName = "Order Closed" },
                    new LeadStatus { StatusName = "Order Lost" }
                );
            }

            if (!context.LeadSources.Any())
            {
                context.LeadSources.AddRange(
                    new LeadSource { SourceName = "Agent" },
                    new LeadSource { SourceName = "Cold Call" },
                    new LeadSource { SourceName = "Existing" },
                    new LeadSource { SourceName = "Online" },
                    new LeadSource { SourceName = "Referrals" }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
