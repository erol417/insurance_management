namespace InsuranceManagement.Web.ViewModels;

public class ExecutiveDashboardViewModel
{
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
    public List<PerformanceRowVm> Rows { get; set; } = [];
}

public class PerformanceRowVm
{
    public string EmployeeName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int SaleCount { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal CollectionTotal { get; set; }
    public decimal ExpenseTotal { get; set; }
}

public class ProductDashboardViewModel
{
    public List<ProductDashboardRowVm> Rows { get; set; } = [];
}

public class ProductDashboardRowVm
{
    public string ProductName { get; set; } = string.Empty;
    public int SaleCount { get; set; }
    public decimal CollectionTotal { get; set; }
    public decimal PrimaryFinancialMetric { get; set; }
}

public class ExpenseDashboardViewModel
{
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
