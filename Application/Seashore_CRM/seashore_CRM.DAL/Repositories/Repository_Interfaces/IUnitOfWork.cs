using System.Threading.Tasks;
using seashore_CRM.DataLayer.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IUnitOfWork
    {
        ICompanyRepository Companies { get; }
        IContactRepository Contacts { get; }
        IInvoiceRepository Invoices { get; }
        IPaymentRepository Payments { get; }
        ISaleRepository Sales { get; }
        ISaleItemRepository SaleItems { get; }

        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }

        ILeadRepository Leads { get; }
        ILeadItemRepository LeadItems { get; }
        ILeadStatusRepository LeadStatuses { get; }
        ILeadSourceRepository LeadSources { get; }
        ILeadStatusActivityRepository LeadStatusActivities { get; }
        ILeadHistoryRepository LeadHistories { get; }

        ICommentRepository Comments { get; }

        IUserRepository Users { get; }
        IOpportunityRepository Opportunities { get; }
        IRoleRepository Roles { get; }

        IProductGroupRepository ProductGroups { get; }
        IUserLeadRightsRepository UserLeadRights { get; }
        IUserActivityRepository UserActivities { get; }

        IIndividualCustomerRepository IndividualCustomers { get; }

        Task<int> CommitAsync();

        // Transaction helpers
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
    }
}
