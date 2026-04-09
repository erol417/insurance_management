using InsuranceManagement.Web.Data;
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

    public IActionResult Executive()
    {
        BuildShell();
        return View(_dashboardService.BuildExecutive());
    }

    public IActionResult Performance()
    {
        BuildShell();
        return View(_dashboardService.BuildPerformance());
    }

    public IActionResult Products()
    {
        BuildShell();
        return View(_dashboardService.BuildProducts());
    }

    public IActionResult Expenses()
    {
        BuildShell();
        return View(_dashboardService.BuildExpenses());
    }
}
