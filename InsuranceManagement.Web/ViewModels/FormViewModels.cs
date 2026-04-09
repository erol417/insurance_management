using System.ComponentModel.DataAnnotations;
using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.ViewModels;

public class LeadFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Source { get; set; } = "Call Center";
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public string Priority { get; set; } = "Medium";
    public string Note { get; set; } = string.Empty;
}

public class LeadInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Source { get; set; } = "Call Center";
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public string Priority { get; set; } = "Medium";
    public string Note { get; set; } = string.Empty;
    public int? AssignedEmployeeId { get; set; }
}

public class LeadsIndexViewModel
{
    public List<LeadInlineEditViewModel> Items { get; set; } = [];
    public LeadInlineEditViewModel NewLead { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class LeadGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class GridBulkSaveFormViewModel
{
    public string ActionName { get; set; } = string.Empty;
    public string FormId { get; set; } = string.Empty;
    public string PayloadInputId { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class AccountFormViewModel
{
    public int? Id { get; set; }
    public AccountType AccountType { get; set; } = AccountType.Corporate;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    public string? District { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public int? OwnerEmployeeId { get; set; }
    public string Status { get; set; } = "Active";
    public string Notes { get; set; } = string.Empty;
}

public class AccountInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.Corporate;
    public string DisplayName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public int? OwnerEmployeeId { get; set; }
    public string Status { get; set; } = "Active";
    public string Notes { get; set; } = string.Empty;
}

public class AccountsIndexViewModel
{
    public List<AccountInlineEditViewModel> Items { get; set; } = [];
    public AccountInlineEditViewModel NewAccount { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class AccountGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class ActivityFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public DateTime ActivityDate { get; set; } = DateTime.Today;

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? LeadId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public ContactStatus ContactStatus { get; set; } = ContactStatus.Contacted;
    public OutcomeStatus? OutcomeStatus { get; set; }

    [Required]
    public string Summary { get; set; } = string.Empty;
}

public class ActivityInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int? LeadId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public ContactStatus ContactStatus { get; set; } = ContactStatus.Contacted;
    public OutcomeStatus? OutcomeStatus { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class ActivitiesIndexViewModel
{
    public List<ActivityInlineEditViewModel> Items { get; set; } = [];
    public ActivityInlineEditViewModel NewActivity { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class ActivityGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class SaleFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? ActivityId { get; set; }
    public ProductType ProductType { get; set; } = ProductType.Bes;
    public decimal? CollectionAmount { get; set; }
    public decimal? ApeAmount { get; set; }
    public decimal? LumpSumAmount { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; }
    public decimal? PremiumAmount { get; set; }
    public decimal? ProductionAmount { get; set; }
    public decimal? SaleAmount { get; set; }
    public int SaleCount { get; set; } = 1;
    public string Notes { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        switch (ProductType)
        {
            case ProductType.Bes when CollectionAmount is null && MonthlyPaymentAmount is null:
                yield return new ValidationResult("BES satisinda tahsilat veya aylik odeme zorunludur.", [nameof(CollectionAmount), nameof(MonthlyPaymentAmount)]);
                break;
            case ProductType.Life when PremiumAmount is null:
                yield return new ValidationResult("Hayat satisinda prim zorunludur.", [nameof(PremiumAmount)]);
                break;
            case ProductType.Health when ProductionAmount is null && CollectionAmount is null:
                yield return new ValidationResult("Saglik satisinda uretim veya tahsilat zorunludur.", [nameof(ProductionAmount), nameof(CollectionAmount)]);
                break;
            case ProductType.Travel when SaleAmount is null && CollectionAmount is null:
                yield return new ValidationResult("Seyahat satisinda satis tutari veya tahsilat zorunludur.", [nameof(SaleAmount), nameof(CollectionAmount)]);
                break;
            case ProductType.Other when SaleAmount is null:
                yield return new ValidationResult("Diger urunlerde satis tutari zorunludur.", [nameof(SaleAmount)]);
                break;
        }
    }
}

public class SaleInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int? ActivityId { get; set; }
    public ProductType ProductType { get; set; } = ProductType.Bes;
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

public class SalesIndexViewModel
{
    public List<SaleInlineEditViewModel> Items { get; set; } = [];
    public SaleInlineEditViewModel NewSale { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class SaleGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class ExpenseFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    public int EmployeeId { get; set; }

    public ExpenseType ExpenseType { get; set; } = ExpenseType.Travel;

    [Range(1, double.MaxValue)]
    public decimal Amount { get; set; }

    public string Notes { get; set; } = string.Empty;
}

public class ExpenseInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public ExpenseType ExpenseType { get; set; } = ExpenseType.Travel;
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ExpensesIndexViewModel
{
    public List<ExpenseInlineEditViewModel> Items { get; set; } = [];
    public ExpenseInlineEditViewModel NewExpense { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class ExpenseGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
}

public class EmployeeFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Region { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;
}

public class ImportUploadViewModel
{
    [Required]
    public IFormFile? File { get; set; }

    public string Notes { get; set; } = string.Empty;
}

public class LeadImportPreviewRowViewModel
{
    public int RowNumber { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string AssignedEmployee { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public bool CanImport => Errors.Count == 0;
}

public class LeadImportPreviewViewModel
{
    public int BatchId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public List<LeadImportPreviewRowViewModel> Rows { get; set; } = [];
}
