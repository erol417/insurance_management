using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;

namespace InsuranceManagement.Web.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public ExecutiveDashboardViewModel BuildExecutive()
    {
        var activities = _db.Activities.ToList();
        var sales = _db.Sales.ToList();
        var expenses = _db.Expenses.ToList();
        var leads = _db.Leads.ToList();
        var accounts = _db.Accounts.ToDictionary(x => x.Id, x => x);

        return new ExecutiveDashboardViewModel
        {
            NewLeadCount = leads.Count(x => x.CreatedAt >= DateTime.Today.AddDays(-30)),
            ActivityCount = activities.Count,
            SaleCount = sales.Sum(x => x.SaleCount),
            ExpenseTotal = expenses.Sum(x => x.Amount),
            WaitingAssignmentLeadCount = leads.Count(x => x.Status == LeadStatus.ReadyForAssignment),
            AssignedThisWeekLeadCount = leads.Count(x => x.AssignedEmployeeId.HasValue && x.CreatedAt >= DateTime.Today.AddDays(-7)),
            ConvertedLeadCount = leads.Count(x => x.Status == LeadStatus.ConvertedToActivity),
            RegionalStats = activities.GroupBy(x => accounts.TryGetValue(x.AccountId, out var account) ? account.City : "Bilinmiyor")
                .Select(g => new RegionStatVm
                {
                    Region = g.Key,
                    ActivityCount = g.Count(),
                    ConversionRate = BuildConversionRate(g.Select(x => x.Id))
                }).OrderByDescending(x => x.ActivityCount).ToList(),
            EmployeeWorkloads = _db.Employees.ToList().Select(x => new EmployeeWorkloadVm
            {
                EmployeeId = x.Id,
                EmployeeName = x.FullName,
                OpenLeadCount = leads.Count(l => l.AssignedEmployeeId == x.Id && l.Status is LeadStatus.Assigned or LeadStatus.VisitScheduled),
                CompletedLeadCount = leads.Count(l => l.AssignedEmployeeId == x.Id && l.Status == LeadStatus.ConvertedToActivity),
                ConversionRate = BuildEmployeeConversionRate(x.Id)
            }).OrderByDescending(x => x.OpenLeadCount).ToList(),
            ProductBreakdown = sales.GroupBy(x => x.ProductType)
                .Select(g => new ProductBreakdownVm
                {
                    ProductName = g.Key.ToDisplayText(),
                    SaleCount = g.Sum(x => x.SaleCount),
                    Amount = g.Sum(x => x.CollectionAmount ?? x.ProductionAmount ?? x.PremiumAmount ?? x.SaleAmount ?? 0m)
                }).OrderByDescending(x => x.SaleCount).ToList(),
            DataWarnings =
            [
                $"{sales.Count(x => !x.ActivityId.HasValue)} bagimsiz satis kaydi KPI yorumunu etkiliyor.",
                $"{leads.Count(x => x.Status == LeadStatus.ReadyForAssignment)} lead atama bekliyor.",
                $"{expenses.Count(x => x.Amount > 1000m)} yuksek masraf kaydi kontrol gerektiriyor."
            ]
        };
    }

    public PerformanceDashboardViewModel BuildPerformance()
    {
        var activities = _db.Activities.ToList();
        var sales = _db.Sales.ToList();
        var expenses = _db.Expenses.ToList();

        return new PerformanceDashboardViewModel
        {
            Rows = _db.Employees.ToList().Select(x =>
            {
                var employeeActivities = activities.Where(a => a.EmployeeId == x.Id).ToList();
                var employeeSales = sales.Where(s => s.EmployeeId == x.Id).ToList();
                var employeeExpenses = expenses.Where(e => e.EmployeeId == x.Id).ToList();

                return new PerformanceRowVm
                {
                    EmployeeName = x.FullName,
                    Region = x.Region,
                    ActivityCount = employeeActivities.Count,
                    SaleCount = employeeSales.Sum(s => s.SaleCount),
                    ConversionRate = employeeActivities.Count == 0 ? 0m : Math.Round((decimal)employeeSales.Count / employeeActivities.Count * 100m, 1),
                    CollectionTotal = employeeSales.Sum(s => s.CollectionAmount ?? 0m),
                    ExpenseTotal = employeeExpenses.Sum(e => e.Amount)
                };
            }).OrderByDescending(x => x.CollectionTotal).ToList()
        };
    }

    public ProductDashboardViewModel BuildProducts()
    {
        return new ProductDashboardViewModel
        {
            Rows = _db.Sales.ToList().GroupBy(x => x.ProductType)
                .Select(g => new ProductDashboardRowVm
                {
                    ProductName = g.Key.ToDisplayText(),
                    SaleCount = g.Sum(x => x.SaleCount),
                    CollectionTotal = g.Sum(x => x.CollectionAmount ?? 0m),
                    PrimaryFinancialMetric = g.Key switch
                    {
                        ProductType.Bes => g.Sum(x => x.ApeAmount ?? 0m),
                        ProductType.Life => g.Sum(x => x.PremiumAmount ?? 0m),
                        ProductType.Health => g.Sum(x => x.ProductionAmount ?? 0m),
                        _ => g.Sum(x => x.SaleAmount ?? 0m)
                    }
                }).ToList()
        };
    }

    public ExpenseDashboardViewModel BuildExpenses()
    {
        var expenses = _db.Expenses.ToList();
        var sales = _db.Sales.ToList();

        return new ExpenseDashboardViewModel
        {
            TotalExpense = expenses.Sum(x => x.Amount),
            CostPerSale = sales.Count == 0 ? 0m : Math.Round(expenses.Sum(x => x.Amount) / sales.Count, 2),
            Rows = expenses.GroupBy(x => x.ExpenseType)
                .Select(g => new ExpenseBreakdownVm
                {
                    ExpenseType = g.Key.ToDisplayText(),
                    Count = g.Count(),
                    Total = g.Sum(x => x.Amount)
                }).OrderByDescending(x => x.Total).ToList()
        };
    }

    private decimal BuildConversionRate(IEnumerable<int> activityIds)
    {
        var ids = activityIds.ToHashSet();
        if (ids.Count == 0)
        {
            return 0m;
        }

        var linkedSales = _db.Sales.Count(x => x.ActivityId.HasValue && ids.Contains(x.ActivityId.Value));
        return Math.Round((decimal)linkedSales / ids.Count * 100m, 1);
    }

    private decimal BuildEmployeeConversionRate(int employeeId)
    {
        var activityCount = _db.Activities.Count(x => x.EmployeeId == employeeId);
        if (activityCount == 0)
        {
            return 0m;
        }

        var saleCount = _db.Sales.Count(x => x.EmployeeId == employeeId);
        return Math.Round((decimal)saleCount / activityCount * 100m, 1);
    }
}
