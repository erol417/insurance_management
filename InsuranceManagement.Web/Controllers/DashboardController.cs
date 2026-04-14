using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

public class DashboardController : AppController
{
    private readonly DashboardService _dashboardService;

    public DashboardController(AppDbContext db, DashboardService dashboardService) : base(db)
    {
        _dashboardService = dashboardService;
    }

    public IActionResult Executive(string range = "last30", DateTime? start = null, DateTime? end = null, int? employeeId = null, int? productTypeId = null)
    {
        BuildShell();
        var (startDate, endDate) = GetRangeDates(range, start, end);
        var filterId = employeeId ?? CurrentEmployeeScopeId();
        var model = _dashboardService.BuildExecutive(startDate, endDate, range, filterId, productTypeId);
        
        PopulateFilters(model);
        return View(model);
    }

    public IActionResult Performance(string range = "last30", DateTime? start = null, DateTime? end = null, int? employeeId = null, int? productTypeId = null)
    {
        BuildShell();
        var (startDate, endDate) = GetRangeDates(range, start, end);
        var filterId = employeeId ?? CurrentEmployeeScopeId();
        var model = _dashboardService.BuildPerformance(startDate, endDate, range, filterId, productTypeId);
        
        PopulateFilters(model);
        return View(model);
    }

    public IActionResult Products(string range = "last30", DateTime? start = null, DateTime? end = null, int? employeeId = null, int? productTypeId = null)
    {
        BuildShell();
        var (startDate, endDate) = GetRangeDates(range, start, end);
        var filterId = employeeId ?? CurrentEmployeeScopeId();
        var model = _dashboardService.BuildProducts(startDate, endDate, range, filterId, productTypeId);
        
        PopulateFilters(model);
        return View(model);
    }

    public IActionResult Expenses(string range = "last30", DateTime? start = null, DateTime? end = null, int? employeeId = null, int? productTypeId = null)
    {
        BuildShell();
        var (startDate, endDate) = GetRangeDates(range, start, end);
        var filterId = employeeId ?? CurrentEmployeeScopeId();
        var model = _dashboardService.BuildExpenses(startDate, endDate, range, filterId, productTypeId);
        
        PopulateFilters(model);
        return View(model);
    }

    private void PopulateFilters(dynamic model)
    {
        var role = User.GetRoleType();
        var currentEmpId = User.GetEmployeeId();
        int? selectedEmpId = model.EmployeeId;
        int? selectedProdId = model.ProductTypeId;

        var employees = Db.Employees.OrderBy(x => x.FullName).AsQueryable();
        if (role == RoleType.FieldSales)
        {
            employees = employees.Where(x => x.Id == currentEmpId);
        }

        model.PersonnelList = employees.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.FullName,
            Selected = x.Id == selectedEmpId
        }).ToList();

        model.ProductTypeList = Db.InsuranceProductTypes.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == selectedProdId
        }).ToList();
    }

    private (DateTime Start, DateTime End) GetRangeDates(string range, DateTime? start, DateTime? end)
    {
        var today = DateTime.Today;
        var endDate = end ?? today;

        DateTime startDate;
        switch (range.ToLower())
        {
            case "thisweek":
                startDate = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                if (startDate > today) startDate = startDate.AddDays(-7);
                break;
            case "thismonth":
                startDate = new DateTime(today.Year, today.Month, 1);
                break;
            case "thisyear":
                startDate = new DateTime(today.Year, 1, 1);
                break;
            case "last90":
                startDate = today.AddDays(-90);
                break;
            case "custom":
                startDate = start ?? today.AddDays(-30);
                break;
            case "last30":
            default:
                startDate = today.AddDays(-30);
                break;
        }

        return (startDate, endDate);
    }
}
