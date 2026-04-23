using Microsoft.AspNetCore.Http; // for user logn 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using seashore_CRM.DomainModelLayer.Entities;
using seashore_CRM.Models.Entities;
using seashore_CRM.Models.Identity;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace seashore_CRM.DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // For accessing user info in audit fields

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<LeadStatus> LeadStatuses => Set<LeadStatus>();
        public DbSet<LeadStatusActivity> LeadStatusActivities => Set<LeadStatusActivity>();
        public DbSet<LeadSource> LeadSources => Set<LeadSource>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<LeadHistory> LeadHistories => Set<LeadHistory>();

        // log sets
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
        public DbSet<UserActivity> UserActivities => Set<UserActivity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseEntity(modelBuilder);
            ConfigureIndexes(modelBuilder);
            ConfigureRelationships(modelBuilder);

            // Configure UserActivity indexes
            modelBuilder.Entity<UserActivity>()
                .HasIndex(u => u.PerformedAt);

            modelBuilder.Entity<UserActivity>()
                .HasIndex(u => u.UserId);
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

                    // Default IsActive should be true for new rows at DB level
                    modelBuilder.Entity(clrType)
                        .Property(nameof(BaseEntity.IsActive))
                        .HasDefaultValue(true);

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

            // LeadItem indexes (previously referenced OpportunityItem)
            modelBuilder.Entity<LeadItem>()
                .HasIndex(li => li.LeadId);

            modelBuilder.Entity<LeadItem>()
                .HasIndex(li => li.ProductId);

            // Index for report-to relationship
            modelBuilder.Entity<User>()
                .HasIndex(u => u.ReportToUserId);

            // Indexes for LeadHistory
            modelBuilder.Entity<LeadHistory>()
                .HasIndex(h => h.LeadId);
            modelBuilder.Entity<LeadHistory>()
                .HasIndex(h => h.ChangedAt);
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

            // Self-referencing report-to relationship for User
            modelBuilder.Entity<User>()
                .HasOne(u => u.ReportToUser)
                .WithMany()
                .HasForeignKey(u => u.ReportToUserId)
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

            // LeadItem relations (previously configured for OpportunityItem)
            modelBuilder.Entity<LeadItem>()
                .HasOne(li => li.Lead)
                .WithMany(l => l.LeadItems)
                .HasForeignKey(li => li.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<LeadItem>()
                .HasOne(li => li.Product)
                .WithMany()
                .HasForeignKey(li => li.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // LeadStatus -> LeadStatusActivity
            modelBuilder.Entity<LeadStatusActivity>()
                .HasOne(a => a.LeadStatus)
                .WithMany(s => s.Activities)
                .HasForeignKey(a => a.LeadStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserLeadRights relations
            modelBuilder.Entity<UserLeadRights>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                // mark Restrict to avoid cascade paths and avoid issues with global query filters
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserLeadRights>()
                .HasOne(r => r.Lead)
                .WithMany(l => l.UserLeadRights)
                .HasForeignKey(r => r.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // IndividualCustomer relations
            modelBuilder.Entity<Lead>()
                .HasOne(l => l.IndividualCustomer)
                .WithMany()
                .HasForeignKey(l => l.IndividualCustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // ProductGroup -> Category
            modelBuilder.Entity<ProductGroup>()
                .HasOne(pg => pg.Category)
                .WithMany(c => c.ProductGroups)
                .HasForeignKey(pg => pg.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product -> Category and ProductGroup
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product -> ProductGroup
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductGroup)
                .WithMany()
                .HasForeignKey(p => p.ProductGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // LeadHistory relationships
            modelBuilder.Entity<LeadHistory>()
                .HasOne(h => h.Lead)
                .WithMany()
                .HasForeignKey(h => h.LeadId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LeadHistory>()
                .HasOne(h => h.ChangedBy)
                .WithMany()
                .HasForeignKey(h => h.ChangedById)
                .OnDelete(DeleteBehavior.SetNull);

            // RelatedLeadStatusActivity: avoid cascade to prevent multiple cascade paths
            modelBuilder.Entity<LeadHistory>()
                .HasOne(h => h.RelatedLeadStatusActivity)
                .WithMany()
                .HasForeignKey("RelatedLeadStatusActivityId")
                .OnDelete(DeleteBehavior.Restrict);
        }


        // Override SaveChangesAsync // this for automatically setting audit fields and implementing soft delete logic
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Capture audit entries before SaveChanges
            var auditEntries = new List<AuditLog>();

            var entries = ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

            var now = DateTime.UtcNow;
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? _httpContextAccessor?.HttpContext?.User?.Identity?.Name
                        ?? "System";
            var correlationId = _httpContextAccessor?.HttpContext?.TraceIdentifier;

            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    ChangedAt = now,
                    ChangedBy = userId,
                    CorrelationId = correlationId
                };

                // capture key
                var keyNames = entry.Metadata.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList();
                if (keyNames != null && keyNames.Any())
                {
                    var keyValues = keyNames.ToDictionary(k => k, k => entry.Property(k).CurrentValue);
                    audit.KeyValues = JsonSerializer.Serialize(keyValues);
                }

                if (entry.State == EntityState.Added)
                {
                    audit.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                }
                else if (entry.State == EntityState.Deleted)
                {
                    audit.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                }
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();

                    foreach (var prop in entry.Properties)
                    {
                        if (prop.IsTemporary) continue;
                        var propName = prop.Metadata.Name;
                        var original = entry.GetDatabaseValues()?.GetValue<object?>(propName);
                        var current = prop.CurrentValue;
                        if (!Equals(original, current))
                        {
                            oldValues[propName] = original;
                            newValues[propName] = current;
                        }
                    }

                    if (oldValues.Any()) audit.OldValues = JsonSerializer.Serialize(oldValues);
                    if (newValues.Any()) audit.NewValues = JsonSerializer.Serialize(newValues);
                }

                auditEntries.Add(audit);

                // Also apply standard base entity auditing/soft delete
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.IsActive = true; // Ensure new entities are active
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = now;
                        entry.Entity.UpdatedBy = userId;
                        break;
                    case EntityState.Deleted:
                        // Soft delete: mark as inactive instead of deleting
                        entry.State = EntityState.Modified;
                        entry.Entity.IsActive = false;
                        entry.Entity.UpdatedDate = now;
                        entry.Entity.UpdatedBy = userId;
                        break;
                }
            }

            // Save changes (this will persist data and generate keys for added entries)
            var result = await base.SaveChangesAsync(cancellationToken);

            // After successful save, persist audit logs (with any generated keys)
            if (auditEntries.Any())
            {
                foreach (var audit in auditEntries)
                {
                    // if key values were empty for added entries, try to refetch
                    if (string.IsNullOrEmpty(audit.KeyValues))
                    {
                        // best effort: skip
                    }

                    AuditLogs.Add(audit);
                }

                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        // ===============================
        // SOFT DELETE FILTER /// Global Query Filter
        // ===============================
        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsActive));
            var condition = Expression.Equal(property, Expression.Constant(true));
            var lambda = Expression.Lambda(condition, parameter);
            return lambda;
        }
    }
}
