using System.Threading.Tasks;
using seashore_CRM.DAL.Data;
using seashore_CRM.Models.Entities;

namespace seashore_CRM.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Roles = new Repository<Role>(_context);
            Companies = new Repository<Company>(_context);
            Contacts = new Repository<Contact>(_context);
            Leads = new Repository<Lead>(_context);
            LeadStatuses = new Repository<LeadStatus>(_context);
            LeadSources = new Repository<LeadSource>(_context);
            Opportunities = new Repository<Opportunity>(_context);
            Categories = new Repository<Category>(_context);
            Products = new Repository<Product>(_context);
            Sales = new Repository<Sale>(_context);
            SaleItems = new Repository<SaleItem>(_context);
            Invoices = new Repository<Invoice>(_context);
            Payments = new Repository<Payment>(_context);
            Activities = new Repository<Activity>(_context);
            Comments = new Repository<Comment>(_context);
        }

        public IRepository<User> Users { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<Company> Companies { get; }
        public IRepository<Contact> Contacts { get; }
        public IRepository<Lead> Leads { get; }
        public IRepository<LeadStatus> LeadStatuses { get; }
        public IRepository<LeadSource> LeadSources { get; }
        public IRepository<Opportunity> Opportunities { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Product> Products { get; }
        public IRepository<Sale> Sales { get; }
        public IRepository<SaleItem> SaleItems { get; }
        public IRepository<Invoice> Invoices { get; }
        public IRepository<Payment> Payments { get; }
        public IRepository<Activity> Activities { get; }
        public IRepository<Comment> Comments { get; }

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
