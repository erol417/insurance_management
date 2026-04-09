using InsuranceManagement.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public override int SaveChanges()
    {
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        NormalizeDateTimes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
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
            entity.Property(x => x.Password).HasMaxLength(200).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(50);
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
            entity.Property(x => x.Source).HasMaxLength(100).IsRequired();
            entity.Property(x => x.City).HasMaxLength(150).IsRequired();
            entity.Property(x => x.District).HasMaxLength(150);
            entity.Property(x => x.ContactName).HasMaxLength(150);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Priority).HasMaxLength(50);
            entity.Property(x => x.Note).HasMaxLength(2000);
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
            entity.Property(x => x.ContactStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.OutcomeStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Summary).HasMaxLength(3000).IsRequired();
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sales");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ProductType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.CollectionAmount).HasPrecision(18, 2);
            entity.Property(x => x.ApeAmount).HasPrecision(18, 2);
            entity.Property(x => x.LumpSumAmount).HasPrecision(18, 2);
            entity.Property(x => x.MonthlyPaymentAmount).HasPrecision(18, 2);
            entity.Property(x => x.PremiumAmount).HasPrecision(18, 2);
            entity.Property(x => x.ProductionAmount).HasPrecision(18, 2);
            entity.Property(x => x.SaleAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ExpenseType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Notes).HasMaxLength(1000);
        });

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
