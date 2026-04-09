using InsuranceManagement.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

public class HomeController : AppController
{
    public HomeController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToAction("Login", "Auth");
        }

        return RedirectToAction("Executive", "Dashboard");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
