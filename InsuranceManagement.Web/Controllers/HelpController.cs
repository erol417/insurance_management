using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

public class HelpController : AppController
{
    public HelpController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index()
    {
        BuildShell();
        var role = User.GetRoleType() ?? RoleType.CallCenter; // Default for safety
        return View(role);
    }

    public IActionResult Guide(string role)
    {
        BuildShell();
        if (Enum.TryParse<RoleType>(role, out var roleType))
        {
            return View("Index", roleType);
        }
        return RedirectToAction(nameof(Index));
    }
}
