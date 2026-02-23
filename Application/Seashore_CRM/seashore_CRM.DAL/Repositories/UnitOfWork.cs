using System.Threading.Tasks;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
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

        public IActivityRepository Activities { get; }
        public ICommentRepository Comments { get; }

        public IUserRepository Users { get; }
        public IOpportunityRepository Opportunities { get; }
        public IRoleRepository Roles { get; }

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

            Leads = new LeadRepository(context);
            LeadItems = new LeadItemRepository(context);
            LeadStatuses = new LeadStatusRepository(context);
            LeadSources = new LeadSourceRepository(context);
            LeadStatusActivities = new LeadStatusActivityRepository(context);

            Activities = new ActivityRepository(context);
            Comments = new CommentRepository(context);

            Users = new UserRepository(context);
            Opportunities = new OpportunityRepository(context);
            Roles = new RoleRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
