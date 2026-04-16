using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Controllers;

[Authorize]
public class SearchController : AppController
{
    public SearchController(AppDbContext db) : base(db) { }

    public async Task<IActionResult> Index(string q)
    {
        BuildShell();
        ViewBag.Query = q;

        if (string.IsNullOrWhiteSpace(q))
        {
            return View(new SearchResultsViewModel());
        }

        var searchTerm = q.ToLower();

        // Search Leads
        var leads = await Db.Leads
            .Include(x => x.LeadStatusType)
            .Where(x => x.DisplayName.ToLower().Contains(searchTerm) || x.Code.ToLower().Contains(searchTerm) || x.Phone.Contains(q))
            .Take(10)
            .ToListAsync();

        // Search Accounts
        var accounts = await Db.Accounts
            .Where(x => x.DisplayName.ToLower().Contains(searchTerm) || x.Code.ToLower().Contains(searchTerm) || x.Phone!.Contains(q))
            .Take(10)
            .ToListAsync();

        // Search Employees
        var employees = await Db.Employees
            .Where(x => x.FullName.ToLower().Contains(searchTerm))
            .Take(5)
            .ToListAsync();

        var model = new SearchResultsViewModel
        {
            Leads = leads,
            Accounts = accounts,
            Employees = employees,
            TotalCount = leads.Count + accounts.Count + employees.Count
        };

        return View(model);
    }
}

public class SearchResultsViewModel
{
    public List<Lead> Leads { get; set; } = [];
    public List<Account> Accounts { get; set; } = [];
    public List<Employee> Employees { get; set; } = [];
    public int TotalCount { get; set; }
}
