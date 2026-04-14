using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<LeadNote> LeadNotes { get; set; }
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Sprint 2 Reference Tables
    public DbSet<LeadStatusType> LeadStatusTypes => Set<LeadStatusType>();
    public DbSet<LeadSourceType> LeadSourceTypes => Set<LeadSourceType>();
    public DbSet<ActivityContactStatusType> ActivityContactStatusTypes => Set<ActivityContactStatusType>();
    public DbSet<ActivityOutcomeStatusType> ActivityOutcomeStatusTypes => Set<ActivityOutcomeStatusType>();
    public DbSet<InsuranceProductType> InsuranceProductTypes => Set<InsuranceProductType>();
    public DbSet<ExpenseReferenceType> ExpenseTypes => Set<ExpenseReferenceType>();
    public DbSet<LeadAssignment> LeadAssignments => Set<LeadAssignment>();

    public override int SaveChanges()
    {
        ApplyAuditFields();
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditFields();
        NormalizeDateTimes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        NormalizeDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        NormalizeDateTimes();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Region).HasMaxLength(150).IsRequired();
            entity.Property(x => x.City).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("leads");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(250).IsRequired();
            entity.Property(x => x.City).HasMaxLength(150).IsRequired();
            entity.Property(x => x.District).HasMaxLength(150);
            entity.Property(x => x.ContactName).HasMaxLength(150);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.Priority).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Note).HasMaxLength(2000);

            entity.HasOne(x => x.AssignedEmployee)
                .WithMany()
                .HasForeignKey(x => x.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.LeadStatusType)
                .WithMany()
                .HasForeignKey(x => x.LeadStatusTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.LeadSourceType)
                .WithMany()
                .HasForeignKey(x => x.LeadSourceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AccountType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.DisplayName).HasMaxLength(250).IsRequired();
            entity.Property(x => x.City).HasMaxLength(150).IsRequired();
            entity.Property(x => x.District).HasMaxLength(150);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.TaxNumber).HasMaxLength(50);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("activities");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ContactName).HasMaxLength(150);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ContactName).HasMaxLength(150);
            entity.Property(x => x.Summary).HasMaxLength(3000).IsRequired();

            entity.HasOne(x => x.Employee)
                .WithMany(e => e.Activities)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Account)
                .WithMany(a => a.Activities)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ContactStatusType)
                .WithMany()
                .HasForeignKey(x => x.ContactStatusTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.OutcomeStatusType)
                .WithMany()
                .HasForeignKey(x => x.OutcomeStatusTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sales");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.CollectionAmount).HasPrecision(18, 2);
            entity.Property(x => x.ApeAmount).HasPrecision(18, 2);
            entity.Property(x => x.LumpSumAmount).HasPrecision(18, 2);
            entity.Property(x => x.MonthlyPaymentAmount).HasPrecision(18, 2);
            entity.Property(x => x.PremiumAmount).HasPrecision(18, 2);
            entity.Property(x => x.ProductionAmount).HasPrecision(18, 2);
            entity.Property(x => x.SaleAmount).HasPrecision(18, 2);

            entity.HasOne(x => x.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Account)
                .WithMany(a => a.Sales)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Activity)
                .WithMany()
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.InsuranceProductType)
                .WithMany()
                .HasForeignKey(x => x.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Notes).HasMaxLength(1000);

            entity.HasOne(x => x.Employee)
                .WithMany(e => e.Expenses)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ExpenseTypeEntity)
                .WithMany()
                .HasForeignKey(x => x.ExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeadAssignment>(entity =>
        {
            entity.ToTable("lead_assignments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Priority).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.AssignmentNote).HasMaxLength(2000);

            entity.HasOne(x => x.Lead)
                .WithMany(l => l.Assignments)
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AssignedEmployee)
                .WithMany(e => e.LeadAssignments)
                .HasForeignKey(x => x.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AssignedByUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeadNote>(entity =>
        {
            entity.ToTable("lead_notes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Content).HasMaxLength(4000).IsRequired();

            entity.HasOne(x => x.Lead)
                .WithMany(l => l.Notes)
                .HasForeignKey(x => x.LeadId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Common config for reference tables
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseReferenceEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType, b =>
            {
                b.Property("Code").HasMaxLength(50).IsRequired();
                b.Property("Name").HasMaxLength(150).IsRequired();
            });
        }

        modelBuilder.Entity<LeadStatusType>().ToTable("lead_status_types");
        modelBuilder.Entity<LeadSourceType>().ToTable("lead_source_types");
        modelBuilder.Entity<ActivityContactStatusType>().ToTable("activity_contact_status_types");
        modelBuilder.Entity<ActivityOutcomeStatusType>().ToTable("activity_outcome_status_types");
        modelBuilder.Entity<InsuranceProductType>().ToTable("insurance_product_types");
        modelBuilder.Entity<ExpenseReferenceType>().ToTable("expense_types");

        modelBuilder.Entity<ImportBatch>(entity =>
        {
            entity.ToTable("import_batches");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).HasMaxLength(250).IsRequired();
            entity.Property(x => x.ImportedBy).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Module).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ActionType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EntityCode).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Detail).HasMaxLength(4000);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.ModuleName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ModuleKey).HasMaxLength(100).IsRequired();
            entity.Property(x => x.AccessLevel).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Icon).HasMaxLength(50);
            entity.Property(x => x.Tooltip).HasMaxLength(500);
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType, b =>
            {
                b.Property("CreatedBy").HasMaxLength(150);
                b.Property("UpdatedBy").HasMaxLength(150);
            });
        }
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType, b =>
            {
                b.Property("DeletedBy").HasMaxLength(150);
            });
        }

        modelBuilder.Entity<Lead>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Activity>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Sale>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Expense>().HasQueryFilter(x => x.DeletedAt == null);
    }

    private void ApplyAuditFields()
    {
        var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                {
                    entry.Entity.CreatedBy = userName;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userName;
            }
        }
    }

    private void NormalizeDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified))
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue is DateTime dateTime)
                {
                    property.CurrentValue = NormalizeDateTime(dateTime);
                }
                else if (property.Metadata.ClrType == typeof(DateTime?) && property.CurrentValue is DateTime nullableDateTime)
                {
                    property.CurrentValue = NormalizeDateTime(nullableDateTime);
                }
            }
        }
    }

    private static DateTime NormalizeDateTime(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
