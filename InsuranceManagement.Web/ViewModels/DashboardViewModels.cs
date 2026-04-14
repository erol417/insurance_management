namespace InsuranceManagement.Web.ViewModels;

public class ExecutiveDashboardViewModel
{
    // Part A - 6 MVP Metrics (kpi_dictionary.md Bölüm 4.6)
    public int TotalActivities { get; set; }
    public int ContactedActivities { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal TotalBesCollection { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal TotalExpenses { get; set; }

    // Filtering Context
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DateRange { get; set; } = "last30"; 
    public int? EmployeeId { get; set; }
    public int? ProductTypeId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PersonnelList { get; set; } = [];
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ProductTypeList { get; set; } = [];

    // Part B - Trends & Analysis
    public List<TrendPoint> ActivityTrend { get; set; } = [];
    public List<TrendPoint> SalesTrend { get; set; } = [];
    public SalesLinkageSummary SalesLinkage { get; set; } = new();

    // Legacy/Operational Stats
    public int NewLeadCount { get; set; }
    public int ActivityCount { get; set; }
    public int SaleCount { get; set; }
    public decimal ExpenseTotal { get; set; }
    public int WaitingAssignmentLeadCount { get; set; }
    public int AssignedThisWeekLeadCount { get; set; }
    public int ConvertedLeadCount { get; set; }
    public List<RegionStatVm> RegionalStats { get; set; } = [];
    public List<EmployeeWorkloadVm> EmployeeWorkloads { get; set; } = [];
    public List<ProductBreakdownVm> ProductBreakdown { get; set; } = [];
    public List<string> DataWarnings { get; set; } = [];
}

public class TrendPoint
{
    public string Label { get; set; } = string.Empty; 
    public int Count { get; set; }
    public decimal? Amount { get; set; }
}

public class EmployeePerformanceRow
{
    public string EmployeeName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int ContactedCount { get; set; }
    public int SalesCount { get; set; }
    public decimal BesCollection { get; set; }
    public decimal ExpenseAmount { get; set; }
    public decimal ConversionRate { get; set; }
}

public class SalesLinkageSummary
{
    public int LinkedCount { get; set; }
    public int UnlinkedCount { get; set; }
}

public class RegionStatVm
{
    public string Region { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public decimal ConversionRate { get; set; }
}

public class EmployeeWorkloadVm
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int OpenLeadCount { get; set; }
    public int CompletedLeadCount { get; set; }
    public decimal ConversionRate { get; set; }
}

public class ProductBreakdownVm
{
    public string ProductName { get; set; } = string.Empty;
    public int SaleCount { get; set; }
    public decimal Amount { get; set; }
}

public class PerformanceDashboardViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DateRange { get; set; } = "last30";
    public int? EmployeeId { get; set; }
    public int? ProductTypeId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PersonnelList { get; set; } = [];
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ProductTypeList { get; set; } = [];
    public List<PerformanceRowVm> Rows { get; set; } = [];
}

public class PerformanceRowVm
{
    public string EmployeeName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int ContactedCount { get; set; }
    public int SaleCount { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal CollectionTotal { get; set; }
    public decimal BesCollection { get; set; }
    public decimal ExpenseTotal { get; set; }
}

public class ProductDashboardViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DateRange { get; set; } = "last30";
    public int? EmployeeId { get; set; }
    public int? ProductTypeId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PersonnelList { get; set; } = [];
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ProductTypeList { get; set; } = [];
    public List<ProductDashboardRowVm> Rows { get; set; } = [];
}

public class ProductDashboardRowVm
{
    public string ProductName { get; set; } = string.Empty;
    public int SaleCount { get; set; }
    public decimal CollectionTotal { get; set; }
    public decimal PrimaryFinancialMetric { get; set; }
    
    // Detailed metrics
    public decimal ApeTotal { get; set; }
    public decimal PremiumTotal { get; set; }
    public decimal ProductionTotal { get; set; }
    public decimal BaseSaleTotal { get; set; }
}

public class ExpenseDashboardViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DateRange { get; set; } = "last30";
    public int? EmployeeId { get; set; }
    public int? ProductTypeId { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PersonnelList { get; set; } = [];
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ProductTypeList { get; set; } = [];
    public decimal TotalExpense { get; set; }
    public decimal CostPerSale { get; set; }
    public List<ExpenseBreakdownVm> Rows { get; set; } = [];
}

public class ExpenseBreakdownVm
{
    public string ExpenseType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Total { get; set; }
}
