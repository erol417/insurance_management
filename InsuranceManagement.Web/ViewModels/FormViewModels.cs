using System.ComponentModel.DataAnnotations;
using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.ViewModels;

public class LeadFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Musteri Adi / Unvani")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sehir")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Ilce")]
    public string District { get; set; } = string.Empty;
    [Display(Name = "Yetkili Kisi")]
    public string ContactName { get; set; } = string.Empty;
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = string.Empty;
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Lead Kaynagi")]
    public int LeadSourceTypeId { get; set; } = 1; // Default: Call Center
    
    [Required]
    [Display(Name = "Lead Durumu")]
    public int LeadStatusTypeId { get; set; } = 1; // Default: New
    
    [Display(Name = "Oncelik")]
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
    [Display(Name = "Not")]
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
    
    // New FK IDs
    public int LeadSourceTypeId { get; set; }
    public int LeadStatusTypeId { get; set; }
    
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
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

    [Display(Name = "Musteri Tipi")]
    public AccountType AccountType { get; set; } = AccountType.Corporate;

    [Required]
    [Display(Name = "Musteri Adi / Unvani")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sehir")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Ilce")]
    public string? District { get; set; }
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }
    [Display(Name = "E-posta")]
    public string? Email { get; set; }
    [Display(Name = "Vergi Numarasi")]
    public string? TaxNumber { get; set; }
    [Display(Name = "Sorumlu Personel")]
    public int? OwnerEmployeeId { get; set; }
    [Display(Name = "Durum")]
    public string Status { get; set; } = "Aktif";
    [Display(Name = "Notlar")]
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
    public string Status { get; set; } = "Aktif";
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
    [Display(Name = "Aktivite Tarihi")]
    public DateTime ActivityDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Personel")]
    public int EmployeeId { get; set; }

    [Display(Name = "Musteri")]
    public int? AccountId { get; set; }

    [Display(Name = "Bagli Lead")]
    public int? LeadId { get; set; }
    [Display(Name = "Gorusulen Kisi")]
    public string ContactName { get; set; } = string.Empty;

    [Display(Name = "Temas Durumu")]
    public int ContactStatusTypeId { get; set; } = 1;
    [Display(Name = "Gorusme Sonucu")]
    public int? OutcomeStatusTypeId { get; set; }

    [Required]
    [Display(Name = "Gorusme Ozeti")]
    public string Summary { get; set; } = string.Empty;
}

public class ActivityInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public int? AccountId { get; set; }
    public int? LeadId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    
    // New FK IDs
    public int ContactStatusTypeId { get; set; }
    public int? OutcomeStatusTypeId { get; set; }
    
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

public class SaleFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Satis Tarihi")]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Personel")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Musteri")]
    public int AccountId { get; set; }

    [Display(Name = "Bagli Aktivite")]
    public int? ActivityId { get; set; }
    
    [Required]
    [Display(Name = "Urun Tipi")]
    public int ProductTypeId { get; set; } = 1; // Default: BES
    
    [Display(Name = "Tahsilat Tutari")]
    public decimal? CollectionAmount { get; set; }
    [Display(Name = "APE Tutari")]
    public decimal? ApeAmount { get; set; }
    [Display(Name = "Toplu Para")]
    public decimal? LumpSumAmount { get; set; }
    [Display(Name = "Aylik Odeme")]
    public decimal? MonthlyPaymentAmount { get; set; }
    [Display(Name = "Prim Tutari")]
    public decimal? PremiumAmount { get; set; }
    [Display(Name = "Uretim Tutari")]
    public decimal? ProductionAmount { get; set; }
    [Display(Name = "Satis Tutari")]
    public decimal? SaleAmount { get; set; }
    [Display(Name = "Satis Adedi")]
    public int SaleCount { get; set; } = 1;
    [Display(Name = "Notlar")]
    public string Notes { get; set; } = string.Empty;
}

public class SaleInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
    public int? ActivityId { get; set; }
    
    // New FK ID
    public int ProductTypeId { get; set; }
    
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
    [Display(Name = "Masraf Tarihi")]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Personel")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Masraf Turu")]
    public int ExpenseTypeId { get; set; } = 1; // Default: Travel
    
    [Range(1, double.MaxValue)]
    [Display(Name = "Tutar")]
    public decimal Amount { get; set; }

    [Display(Name = "Notlar")]
    public string Notes { get; set; } = string.Empty;
}

public class ExpenseInlineEditViewModel
{
    public int? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    
    // New FK ID
    public int ExpenseTypeId { get; set; }
    
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
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bolge")]
    public string Region { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sehir")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Sistem Girisi")]
    public bool HasLogin { get; set; }
    [Display(Name = "Kullanici Adi")]
    public string? UserName { get; set; }
    [Display(Name = "Sifre")]
    public string? Password { get; set; }
    [Display(Name = "Yetki Rolu")]
    public RoleType? Role { get; set; }
}

public class EmployeeInlineEditViewModel
{
    public int? Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool HasLogin { get; set; }
    public string? UserName { get; set; }
    public RoleType? Role { get; set; }
}

public class EmployeesIndexViewModel
{
    public List<EmployeeInlineEditViewModel> Items { get; set; } = [];
    public EmployeeInlineEditViewModel NewEmployee { get; set; } = new();
}

public class EmployeeGridSaveViewModel
{
    public string PayloadJson { get; set; } = string.Empty;
}

public class ImportUploadViewModel
{
    [Required(ErrorMessage = "Lutfen bir modul seciniz.")]
    [Display(Name = "Yukleme Tipi")]
    public string ModuleName { get; set; } = string.Empty; // "lead", "employee", "account", etc.

    [Required(ErrorMessage = "Lutfen bir dosya seciniz.")]
    [Display(Name = "Dosya")]
    public IFormFile? File { get; set; }

    [Display(Name = "Aciklama")]
    public string Notes { get; set; } = string.Empty;
}

public class ImportPreviewRowViewModel
{
    public int RowNumber { get; set; }
    public Dictionary<string, string> Data { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public bool CanImport => Errors.Count == 0;
}

public class ImportPreviewViewModel
{
    public int BatchId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public List<ImportPreviewRowViewModel> Rows { get; set; } = [];
    public List<string> Columns { get; set; } = [];
}
