namespace InsuranceManagement.Web.Domain;

public class UserAccount : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public RoleType Role { get; set; }
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

public class Employee : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation collections
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<LeadAssignment> LeadAssignments { get; set; } = new List<LeadAssignment>();
}

public class Lead : BaseEntity, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    
    // New Reference FKs
    public int LeadStatusTypeId { get; set; }
    public LeadStatusType LeadStatusType { get; set; } = null!;
    
    public int LeadSourceTypeId { get; set; }
    public LeadSourceType LeadSourceType { get; set; } = null!;

    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
    public string Note { get; set; } = string.Empty;
    
    public int? AssignedEmployeeId { get; set; }
    public Employee? AssignedEmployee { get; set; }

    public DateTime? ScheduledVisitDate { get; set; }
    public int? ConvertedAccountId { get; set; }
    public int? ConvertedActivityId { get; set; }

    public ICollection<LeadAssignment> Assignments { get; set; } = new List<LeadAssignment>();
    public ICollection<LeadNote> Notes { get; set; } = new List<LeadNote>();

    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public class LeadNote : BaseEntity
{
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
}

public class Account : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string Status { get; set; } = "Active";
    public int? OwnerEmployeeId { get; set; }
    public Employee? OwnerEmployee { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Navigation collections
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

public class Activity : BaseEntity, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int? AccountId { get; set; }
    public Account? Account { get; set; }
    public int? LeadId { get; set; }
    public Lead? Lead { get; set; }

    public DateTime? PlannedAt { get; set; }
    public int? DurationMinutes { get; set; }

    public string ContactName { get; set; } = string.Empty;

    public int ContactStatusTypeId { get; set; }
    public ActivityContactStatusType? ContactStatusType { get; set; }

    public int? OutcomeStatusTypeId { get; set; }
    public ActivityOutcomeStatusType? OutcomeStatusType { get; set; }

    public string Summary { get; set; } = string.Empty;
    
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public class Sale : BaseEntity, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    public int? ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public int ProductTypeId { get; set; }
    public InsuranceProductType? InsuranceProductType { get; set; }

    public decimal? CollectionAmount { get; set; }
    public decimal? ApeAmount { get; set; }
    public decimal? LumpSumAmount { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; }
    public decimal? PremiumAmount { get; set; }
    public decimal? ProductionAmount { get; set; }
    public decimal? SaleAmount { get; set; }

    public int SaleCount { get; set; } = 1;
    public string Notes { get; set; } = string.Empty;
    
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public class Expense : BaseEntity, ISoftDeletable
{
    public string Code { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ExpenseTypeId { get; set; }
    public ExpenseReferenceType? ExpenseTypeEntity { get; set; }

    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public class ImportBatch : BaseEntity
{
    public string ModuleName { get; set; } = string.Empty; // e.g. "Lead", "Employee", "Account", etc.
    public string FileName { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; }
    public string ImportedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class RolePermission : BaseEntity
{
    public RoleType Role { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleKey { get; set; } = string.Empty; // e.g. "Leads", "Sales"
    public string AccessLevel { get; set; } = "Yasakli"; // e.g. "Tam Yetki", "Izleme"
    public bool IsAllowed { get; set; }
    public string Icon { get; set; } = "📁";
    public string Tooltip { get; set; } = string.Empty;
}

public class AuditLog
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string EntityCode { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}
