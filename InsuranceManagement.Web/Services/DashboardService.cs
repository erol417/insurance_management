using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public ExecutiveDashboardViewModel BuildExecutive(DateTime startDate, DateTime endDate, string dateRange, int? filterEmployeeId = null, int? filterProductTypeId = null)
    {
        var activitiesQuery = _db.Activities
            .Include(x => x.ContactStatusType)
            .Where(x => x.ActivityDate >= startDate && x.ActivityDate <= endDate)
            .AsQueryable();

        var salesQuery = _db.Sales
            .Include(x => x.InsuranceProductType)
            .Where(x => x.SaleDate >= startDate && x.SaleDate <= endDate)
            .AsQueryable();

        var expensesQuery = _db.Expenses
            .Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            activitiesQuery = activitiesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
            salesQuery = salesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
            expensesQuery = expensesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (filterProductTypeId.HasValue)
        {
            salesQuery = salesQuery.Where(x => x.ProductTypeId == filterProductTypeId.Value);
        }

        var activities = activitiesQuery.Include(x => x.Account).ToList();
        var sales = salesQuery.ToList();
        var expenses = expensesQuery.ToList();
        var leadsQuery = _db.Leads.AsQueryable();
        if (filterEmployeeId.HasValue) leadsQuery = leadsQuery.Where(x => x.AssignedEmployeeId == filterEmployeeId.Value);
        var leads = leadsQuery.Include(x => x.LeadStatusType).ToList();
        var accounts = _db.Accounts.ToDictionary(x => x.Id, x => x);

        var realActivities = activities.Where(a => a.ContactStatusType?.Code != "PLANNED").ToList();

        return new ExecutiveDashboardViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            DateRange = dateRange,
            EmployeeId = filterEmployeeId,
            ProductTypeId = filterProductTypeId,

            TotalActivities = realActivities.Count,
            ContactedActivities = realActivities.Count(a => a.ContactStatusType?.Code == "CONTACTED"),
            TotalSalesCount = sales.Sum(s => s.SaleCount),
            TotalBesCollection = sales.Where(s => s.InsuranceProductType?.Code == "BES").Sum(s => s.CollectionAmount ?? 0m),
            ConversionRate = realActivities.Count > 0 
                ? Math.Round((decimal)sales.Count(s => s.ActivityId.HasValue) / realActivities.Count * 100m, 1) 
                : 0,
            TotalExpenses = expenses.Sum(e => e.Amount),

            ActivityTrend = BuildTrend(realActivities.Select(a => a.ActivityDate)),
            SalesTrend = BuildTrend(sales.Select(s => s.SaleDate)),
            SalesLinkage = new SalesLinkageSummary 
            {
                LinkedCount = sales.Count(s => s.ActivityId.HasValue),
                UnlinkedCount = sales.Count(s => !s.ActivityId.HasValue)
            },

            ProductBreakdown = sales.GroupBy(x => x.InsuranceProductType?.Name ?? "Bilinmeyen")
                .Select(g => new ProductBreakdownVm
                {
                    ProductName = g.Key,
                    SaleCount = g.Sum(x => x.SaleCount),
                    Amount = g.Sum(x => x.CollectionAmount ?? x.ProductionAmount ?? x.PremiumAmount ?? x.SaleAmount ?? 0m)
                }).OrderByDescending(x => x.SaleCount).ToList()
        };
    }

    private List<TrendPoint> BuildTrend(IEnumerable<DateTime> dates)
    {
        return dates.GroupBy(d => d.Date)
            .OrderBy(g => g.Key)
            .Select(g => new TrendPoint { Label = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
            .ToList();
    }

    public PerformanceDashboardViewModel BuildPerformance(DateTime startDate, DateTime endDate, string dateRange, int? filterEmployeeId = null, int? filterProductTypeId = null)
    {
        var activitiesQuery = _db.Activities
            .Include(x => x.ContactStatusType)
            .Where(x => x.ActivityDate >= startDate && x.ActivityDate <= endDate)
            .AsQueryable();

        var salesQuery = _db.Sales
            .Where(x => x.SaleDate >= startDate && x.SaleDate <= endDate)
            .AsQueryable();

        var expensesQuery = _db.Expenses
            .Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            activitiesQuery = activitiesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
            salesQuery = salesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
            expensesQuery = expensesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (filterProductTypeId.HasValue)
        {
            salesQuery = salesQuery.Where(x => x.ProductTypeId == filterProductTypeId.Value);
        }

        var actList = activitiesQuery.ToList();
        var saleList = salesQuery.ToList();
        var expList = expensesQuery.ToList();

        return new PerformanceDashboardViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            DateRange = dateRange,
            EmployeeId = filterEmployeeId,
            ProductTypeId = filterProductTypeId,
            Rows = _db.Employees.ToList().Select(x =>
            {
                var employeeActivities = actList.Where(a => a.EmployeeId == x.Id && a.ContactStatusType?.Code != "PLANNED").ToList();
                var employeeSales = saleList.Where(s => s.EmployeeId == x.Id).ToList();
                var employeeExpenses = expList.Where(e => e.EmployeeId == x.Id).ToList();

                return new PerformanceRowVm
                {
                    EmployeeName = x.FullName,
                    Region = x.Region,
                    ActivityCount = employeeActivities.Count,
                    ContactedCount = employeeActivities.Count(a => a.ContactStatusType?.Code == "CONTACTED"),
                    SaleCount = employeeSales.Sum(s => s.SaleCount),
                    ConversionRate = employeeActivities.Count == 0 ? 0m : Math.Round((decimal)employeeSales.Count(s => s.ActivityId.HasValue) / employeeActivities.Count * 100m, 1),
                    CollectionTotal = employeeSales.Sum(s => s.CollectionAmount ?? 0m),
                    BesCollection = employeeSales.Where(s => s.ProductTypeId == 1).Sum(s => s.CollectionAmount ?? 0m),
                    ExpenseTotal = employeeExpenses.Sum(e => e.Amount)
                };
            }).OrderByDescending(x => x.CollectionTotal).ToList()
        };
    }

    public ProductDashboardViewModel BuildProducts(DateTime startDate, DateTime endDate, string dateRange, int? filterEmployeeId = null, int? filterProductTypeId = null)
    {
        var query = _db.Sales
            .Include(x => x.InsuranceProductType)
            .Where(x => x.SaleDate >= startDate && x.SaleDate <= endDate)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (filterProductTypeId.HasValue)
        {
            query = query.Where(x => x.ProductTypeId == filterProductTypeId.Value);
        }

        return new ProductDashboardViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            DateRange = dateRange,
            EmployeeId = filterEmployeeId,
            ProductTypeId = filterProductTypeId,
            Rows = query
                .ToList()
                .GroupBy(x => x.InsuranceProductType)
                .Select(g => new ProductDashboardRowVm
                {
                    ProductName = g.Key?.Name ?? "Bilinmeyen",
                    SaleCount = g.Sum(x => x.SaleCount),
                    CollectionTotal = g.Sum(x => x.CollectionAmount ?? 0m),
                    ApeTotal = g.Sum(x => x.ApeAmount ?? 0m),
                    PremiumTotal = g.Sum(x => x.PremiumAmount ?? 0m),
                    ProductionTotal = g.Sum(x => x.ProductionAmount ?? 0m),
                    BaseSaleTotal = g.Sum(x => x.SaleAmount ?? 0m),
                    PrimaryFinancialMetric = (g.Key?.Id ?? 0) switch
                    {
                        1 => g.Sum(x => x.ApeAmount ?? 0m), // BES
                        2 => g.Sum(x => x.PremiumAmount ?? 0m), // Hayat
                        3 => g.Sum(x => x.ProductionAmount ?? 0m), // Saglik
                        _ => g.Sum(x => x.SaleAmount ?? 0m)
                    }
                }).ToList()
        };
    }

    public ExpenseDashboardViewModel BuildExpenses(DateTime startDate, DateTime endDate, string dateRange, int? filterEmployeeId = null, int? filterProductTypeId = null)
    {
        var expensesQuery = _db.Expenses
            .Include(x => x.ExpenseTypeEntity)
            .Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate)
            .AsQueryable();

        var salesQuery = _db.Sales
            .Where(x => x.SaleDate >= startDate && x.SaleDate <= endDate)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            expensesQuery = expensesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
            salesQuery = salesQuery.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (filterProductTypeId.HasValue)
        {
            salesQuery = salesQuery.Where(x => x.ProductTypeId == filterProductTypeId.Value);
        }

        var expenses = expensesQuery.ToList();
        var sales = salesQuery.ToList();

        return new ExpenseDashboardViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            DateRange = dateRange,
            EmployeeId = filterEmployeeId,
            ProductTypeId = filterProductTypeId,
            TotalExpense = expenses.Sum(x => x.Amount),
            CostPerSale = sales.Count == 0 ? 0m : Math.Round(expenses.Sum(x => x.Amount) / sales.Count, 2),
            Rows = expenses.GroupBy(x => x.ExpenseTypeEntity)
                .Select(g => new ExpenseBreakdownVm
                {
                    ExpenseType = g.Key?.Name ?? "Bilinmeyen",
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
