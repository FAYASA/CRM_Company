using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using seashore_CRM.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace seashore_CRM.DAL.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<LeadStatus> LeadStatuses => Set<LeadStatus>();
        public DbSet<LeadSource> LeadSources => Set<LeadSource>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<LeadItem> LeadItems => Set<LeadItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseEntity(modelBuilder);
            ConfigureIndexes(modelBuilder);
            ConfigureRelationships(modelBuilder);
        }

        // ===============================
        // BASE ENTITY CONFIGURATION
        // ===============================
        private void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                // Apply only to BaseEntity types
                if (typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    // Soft delete filter
                    modelBuilder.Entity(clrType)
                        .HasQueryFilter(GetIsDeletedRestriction(clrType));

                    // Default values
                    modelBuilder.Entity(clrType)
                        .Property(nameof(BaseEntity.CreatedDate))
                        .HasDefaultValueSql("GETUTCDATE()");

                    modelBuilder.Entity(clrType)
                        .Property(nameof(BaseEntity.IsDeleted))
                        .HasDefaultValue(false);

                    // Concurrency token
                    modelBuilder.Entity(clrType)
                        .Property(nameof(BaseEntity.RowVersion))
                        .IsRowVersion();
                }

                // Global decimal precision
                var decimalProperties = clrType
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) ||
                                p.PropertyType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    modelBuilder.Entity(clrType)
                        .Property(property.Name)
                        .HasPrecision(18, 2);
                }
            }
        }

        // ===============================
        // INDEXES
        // ===============================
        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique(); // Must be unique

            modelBuilder.Entity<Lead>()
                .HasIndex(l => l.AssignedUserId);

            modelBuilder.Entity<Lead>()
                .HasIndex(l => l.StatusId);

            modelBuilder.Entity<SaleItem>()
                .HasIndex(si => si.ProductId);

            modelBuilder.Entity<SaleItem>()
                .HasIndex(si => si.SaleId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.InvoiceId);

            modelBuilder.Entity<Opportunity>()
                .HasIndex(o => o.LeadId);

            modelBuilder.Entity<LeadItem>()
                .HasIndex(li => li.LeadId);

            modelBuilder.Entity<LeadItem>()
                .HasIndex(li => li.ProductId);
        }

        // ===============================
        // RELATIONSHIPS
        // ===============================
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Lead>()
                .HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Lead>()
                .HasOne(l => l.Contact)
                .WithMany()
                .HasForeignKey(l => l.ContactId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Lead>()
                .HasOne(l => l.AssignedUser)
                .WithMany()
                .HasForeignKey(l => l.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Opportunity>()
                .HasOne(o => o.Lead)
                .WithMany()
                .HasForeignKey(o => o.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Opportunity)
                .WithMany()
                .HasForeignKey(s => s.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Sale)
                .WithMany()
                .HasForeignKey(i => i.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany()
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Lead)
                .WithMany()
                .HasForeignKey(a => a.LeadId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Lead)
                .WithMany()
                .HasForeignKey(c => c.LeadId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // LeadItem relations
            modelBuilder.Entity<LeadItem>()
                .HasOne(li => li.Lead)
                .WithMany()
                .HasForeignKey(li => li.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeadItem>()
                .HasOne(li => li.Product)
                .WithMany()
                .HasForeignKey(li => li.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // ===============================
        // SOFT DELETE FILTER
        // ===============================
        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);
            return lambda;
        }
    }
}
