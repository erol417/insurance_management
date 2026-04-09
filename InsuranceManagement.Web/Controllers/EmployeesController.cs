using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,SalesManager,Operations")]
public class EmployeesController : AppController
{
    public EmployeesController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index()
    {
        BuildShell();
        return View(Db.Employees.ToList());
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var employee = Db.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null)
        {
            return NotFound();
        }

        ViewBag.Activities = Db.Activities.Count(x => x.EmployeeId == id);
        ViewBag.Sales = Db.Sales.Count(x => x.EmployeeId == id);
        ViewBag.Expenses = Db.Expenses.Where(x => x.EmployeeId == id).Sum(x => x.Amount);
        ViewBag.OpenLeads = Db.Leads.Count(x => x.AssignedEmployeeId == id && (x.Status == LeadStatus.Assigned || x.Status == LeadStatus.VisitScheduled));
        return View(employee);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        return View(new EmployeeFormViewModel());
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EmployeeFormViewModel model)
    {
        BuildShell();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Employees.Max(x => (int?)x.Id) ?? 0) + 1;
        Db.Employees.Add(new Employee
        {
            Id = id,
            FullName = model.FullName,
            Region = model.Region,
            City = model.City
        });

        Db.SaveChanges();
        TempData["Flash"] = "Personel kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }
}
