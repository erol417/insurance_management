using InsuranceManagement.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class AdminController : AppController
{
    public AdminController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Users()
    {
        BuildShell();
        return View(Db.Users.ToList());
    }

    public IActionResult Roles()
    {
        BuildShell();
        return View();
    }

    public IActionResult ReferenceData()
    {
        BuildShell();
        return View();
    }

    public IActionResult Audit()
    {
        BuildShell();
        return View(Db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(200).ToList());
    }
}
