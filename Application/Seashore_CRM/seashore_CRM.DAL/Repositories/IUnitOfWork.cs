using System;
using System.Threading.Tasks;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<Company> Companies { get; }
        IRepository<Contact> Contacts { get; }
        IRepository<Lead> Leads { get; }
        IRepository<LeadStatus> LeadStatuses { get; }
        IRepository<LeadSource> LeadSources { get; }
        IRepository<Opportunity> Opportunities { get; }
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<Sale> Sales { get; }
        IRepository<SaleItem> SaleItems { get; }
        IRepository<Invoice> Invoices { get; }
        IRepository<Payment> Payments { get; }
        IRepository<Activity> Activities { get; }
        IRepository<Comment> Comments { get; }

        Task<int> CommitAsync();
    }
}
