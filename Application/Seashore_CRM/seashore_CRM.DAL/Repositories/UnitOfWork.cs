using System.Threading.Tasks;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.DataLayer.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ICompanyRepository Companies { get; }
        public IContactRepository Contacts { get; }
        public IInvoiceRepository Invoices { get; }
        public IPaymentRepository Payments { get; }
        public ISaleRepository Sales { get; }
        public ISaleItemRepository SaleItems { get; }

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public ILeadRepository Leads { get; }
        public ILeadItemRepository LeadItems { get; }
        public ILeadStatusRepository LeadStatuses { get; }
        public ILeadSourceRepository LeadSources { get; }
        public ILeadStatusActivityRepository LeadStatusActivities { get; }
        public ILeadHistoryRepository LeadHistories { get; }

        public ICommentRepository Comments { get; }

        public IUserRepository Users { get; }
        public IOpportunityRepository Opportunities { get; }
        public IRoleRepository Roles { get; }

        public IProductGroupRepository ProductGroups { get; }
        public IUserLeadRightsRepository UserLeadRights { get; }
        public IUserActivityRepository UserActivities { get; }

        // Individual customers
        public IIndividualCustomerRepository IndividualCustomers { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Companies = new CompanyRepository(context);
            Contacts = new ContactRepository(context);
            Invoices = new InvoiceRepository(context);
            Payments = new PaymentRepository(context);
            Sales = new SaleRepository(context);
            SaleItems = new SaleItemRepository(context);

            Products = new ProductRepository(context);
            Categories = new CategoryRepository(context);
            ProductGroups = new ProductGroupRepository(context);

            Leads = new LeadRepository(context);
            LeadItems = new LeadItemRepository(context);
            LeadStatuses = new LeadStatusRepository(context);
            LeadSources = new LeadSourceRepository(context);
            LeadStatusActivities = new LeadStatusActivityRepository(context);
            LeadHistories = new LeadHistoryRepository(context);

            Comments = new CommentRepository(context);

            Users = new UserRepository(context);
            Opportunities = new OpportunityRepository(context);
            Roles = new RoleRepository(context);
            UserLeadRights = new UserLeadRightsRepository(context);

            IndividualCustomers = new IndividualCustomerRepository(context);

            // wire user activity
            UserActivities = new UserActivityRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            await transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            await transaction.RollbackAsync();
        }
    }
}
