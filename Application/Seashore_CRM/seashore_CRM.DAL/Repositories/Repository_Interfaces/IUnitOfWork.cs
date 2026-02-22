using System;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories.Repository_Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ICompanyRepository Companies { get; }
        IContactRepository Contacts { get; }
        ILeadRepository Leads { get; }
        ILeadStatusRepository LeadStatuses { get; }
        ILeadStatusActivityRepository LeadStatusActivities { get; }
        ILeadSourceRepository LeadSources { get; }
        IOpportunityRepository Opportunities { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        ISaleRepository Sales { get; }
        ISaleItemRepository SaleItems { get; }
        IInvoiceRepository Invoices { get; }
        IPaymentRepository Payments { get; }
        IActivityRepository Activities { get; }
        ICommentRepository Comments { get; }
        ILeadItemRepository LeadItems { get; }

        Task<int> CommitAsync();
    }
}
