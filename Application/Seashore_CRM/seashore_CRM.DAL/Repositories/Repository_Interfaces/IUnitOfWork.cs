using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

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

        IActivityRepository Activities { get; }
        ICommentRepository Comments { get; }

        IUserRepository Users { get; }
        IOpportunityRepository Opportunities { get; }
        IRoleRepository Roles { get; }

        Task<int> CommitAsync();
    }
}
