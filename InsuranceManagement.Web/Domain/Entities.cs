namespace InsuranceManagement.Web.Domain;

public class UserAccount
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public RoleType Role { get; set; }
    public int? EmployeeId { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class Lead
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Source { get; set; } = "Call Center";
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public LeadStatus Status { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Note { get; set; } = string.Empty;
    public int? AssignedEmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledVisitDate { get; set; }
    public int? ConvertedAccountId { get; set; }
    public int? ConvertedActivityId { get; set; }
}

public class Account
{
    public int Id { get; set; }
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
    public string Notes { get; set; } = string.Empty;
}

public class Activity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int? LeadId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public ContactStatus ContactStatus { get; set; }
    public OutcomeStatus? OutcomeStatus { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class Sale
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int? ActivityId { get; set; }
    public ProductType ProductType { get; set; }
    public decimal? CollectionAmount { get; set; }
    public decimal? ApeAmount { get; set; }
    public decimal? LumpSumAmount { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; }
    public decimal? PremiumAmount { get; set; }
    public decimal? ProductionAmount { get; set; }
    public decimal? SaleAmount { get; set; }
    public int SaleCount { get; set; } = 1;
    public string Notes { get; set; } = string.Empty;
}

public class Expense
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public int EmployeeId { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ImportBatch
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; }
    public string ImportedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
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
