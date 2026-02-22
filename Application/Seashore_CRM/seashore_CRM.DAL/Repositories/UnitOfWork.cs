using System.Threading.Tasks;
using seashore_CRM.DAL.Data;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            Companies = new CompanyRepository(_context);
            Contacts = new ContactRepository(_context);
            Leads = new LeadRepository(_context);
            LeadStatuses = new LeadStatusRepository(_context);
            LeadStatusActivities = new LeadStatusActivityRepository(_context);
            LeadSources = new LeadSourceRepository(_context);
            Opportunities = new OpportunityRepository(_context);
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Sales = new SaleRepository(_context);
            SaleItems = new SaleItemRepository(_context);
            Invoices = new InvoiceRepository(_context);
            Payments = new PaymentRepository(_context);
            Activities = new ActivityRepository(_context);
            Comments = new CommentRepository(_context);
            LeadItems = new LeadItemRepository(_context);
        }

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public ICompanyRepository Companies { get; }
        public IContactRepository Contacts { get; }
        public ILeadRepository Leads { get; }
        public ILeadStatusRepository LeadStatuses { get; }
        public ILeadStatusActivityRepository LeadStatusActivities { get; }
        public ILeadSourceRepository LeadSources { get; }
        public IOpportunityRepository Opportunities { get; }
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public ISaleRepository Sales { get; }
        public ISaleItemRepository SaleItems { get; }
        public IInvoiceRepository Invoices { get; }
        public IPaymentRepository Payments { get; }
        public IActivityRepository Activities { get; }
        public ICommentRepository Comments { get; }
        public ILeadItemRepository LeadItems { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
