using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Controllers;

[Authorize]
public class EmployeesController : AppController
{
    private readonly InsuranceManagement.Web.Services.IEmployeeService _employeeService;

    public EmployeesController(AppDbContext db, InsuranceManagement.Web.Services.IEmployeeService employeeService) : base(db)
    {
        _employeeService = employeeService;
    }

    [Authorize(Roles = "Admin,Manager,SalesManager,Operations")]
    public IActionResult Index(string? searchTerm = null, string? sortBy = "name", bool isDescending = false)
    {
        BuildShell();
        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;

        var employees = _employeeService.GetAll(searchTerm, sortBy, isDescending);
        var users = Db.Users.Where(x => x.EmployeeId != null).ToList();

        var model = new EmployeesIndexViewModel
        {
            Items = employees.Select(e =>
            {
                var user = users.FirstOrDefault(u => u.EmployeeId == e.Id);
                return new EmployeeInlineEditViewModel
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Region = e.Region,
                    City = e.City,
                    HasLogin = user != null,
                    UserName = user?.UserName,
                    Role = user?.Role
                };
            }).ToList(),
            NewEmployee = new EmployeeInlineEditViewModel()
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(EmployeeGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            return RedirectToAction(nameof(Index));
        }

        List<EmployeeInlineEditViewModel>? payload;
        try
        {
            payload = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
        }
        catch
        {
            TempData["Warning"] = "Veri formati gecersiz.";
            return RedirectToAction(nameof(Index));
        }

        if (payload == null || payload.Count == 0) return RedirectToAction(nameof(Index));

        int addedCount = 0;
        int updatedCount = 0;
        var nextId = (Db.Employees.Max(x => (int?)x.Id) ?? 10) + 1;
        var nextUserId = (Db.Users.Max(x => (int?)x.Id) ?? 100) + 1;

        foreach (var item in payload)
        {
            if (item.Id == null) // New Record
            {
                var emp = new Employee
                {
                    Id = nextId++,
                    FullName = item.FullName,
                    Region = item.Region,
                    City = item.City,
                    IsActive = true
                };
                Db.Employees.Add(emp);
                
                if (item.HasLogin && !string.IsNullOrWhiteSpace(item.UserName))
                {
                    Db.Users.Add(new UserAccount
                    {
                        Id = nextUserId++,
                        EmployeeId = emp.Id,
                        FullName = emp.FullName,
                        UserName = item.UserName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Default password for inline add
                        Role = item.Role ?? RoleType.FieldSales
                    });
                }
                addedCount++;
            }
            else // Update Existing
            {
                var emp = Db.Employees.Find(item.Id);
                if (emp != null)
                {
                    emp.FullName = item.FullName;
                    emp.Region = item.Region;
                    emp.City = item.City;

                    var user = Db.Users.FirstOrDefault(u => u.EmployeeId == emp.Id);
                    if (item.HasLogin)
                    {
                        if (user == null)
                        {
                            Db.Users.Add(new UserAccount
                            {
                                Id = nextUserId++,
                                EmployeeId = emp.Id,
                                FullName = emp.FullName,
                                UserName = item.UserName ?? emp.FullName.Replace(" ", ".").ToLower(),
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                                Role = item.Role ?? RoleType.FieldSales
                            });
                        }
                        else
                        {
                            user.FullName = emp.FullName;
                            user.UserName = item.UserName ?? user.UserName;
                            user.Role = item.Role ?? user.Role;
                        }
                    }
                    else if (user != null)
                    {
                        Db.Users.Remove(user);
                    }
                    updatedCount++;
                }
            }
        }

        Db.SaveChanges();
        TempData["Success"] = $"Degisiklikler kaydedildi: {addedCount} yeni, {updatedCount} guncelleme.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id, DateTime? weekStart = null)
    {
        BuildShell();
        
        // Access Control: Admin/Manager can see everyone, others only themselves
        var isAuthorized = User.IsInRole("Admin") || User.IsInRole("Manager") || 
                           User.IsInRole("SalesManager") || User.IsInRole("Operations");
        
        var userEmployeeId = User.Claims.FirstOrDefault(c => c.Type == "employeeId")?.Value;
        if (!isAuthorized && (userEmployeeId == null || userEmployeeId != id.ToString()))
        {
            return Forbid();
        }

        var employee = _employeeService.GetById(id);
        if (employee is null)
        {
            return NotFound();
        }

        ViewBag.ActivitiesCount = employee.Activities.Count;
        ViewBag.SalesCount = employee.Sales.Count;
        ViewBag.ExpensesSum = employee.Expenses.Sum(x => x.Amount);

        // Ground Truth for open leads is the Leads table where AssignedEmployeeId matches
        // and status is not final (Converted=5, Disqualified=9)
        ViewBag.OpenLeadsCount = Db.Leads.Count(x => 
            x.AssignedEmployeeId == id && 
            x.LeadStatusTypeId != 5 && 
            x.LeadStatusTypeId != 9);
        
        // Gather related data for the view
        ViewBag.RelatedLeads = Db.Leads
            .Include(x => x.LeadStatusType)
            .Where(x => x.AssignedEmployeeId == id && x.LeadStatusTypeId != 5 && x.LeadStatusTypeId != 9)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
            
        ViewBag.RelatedSales = Db.Sales
            .Include(x => x.InsuranceProductType)
            .Where(x => x.EmployeeId == id)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
            
        ViewBag.RelatedExpenses = Db.Expenses
            .Include(x => x.ExpenseTypeEntity)
            .Where(x => x.EmployeeId == id)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
            
        ViewBag.PlannedActivities = Db.Activities
            .Include(x => x.Account)
            .Include(x => x.Lead)
            .Where(x => x.EmployeeId == id && x.ContactStatusTypeId == 3) // 3 is PLANNED
            .OrderBy(x => x.ActivityDate)
            .ToList();

        // Calculate start of current week (Monday) if weekStart not provided
        var today = DateTime.Today;
        var startOfWeek = weekStart ?? today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday) startOfWeek = startOfWeek.AddDays(-7);
        
        ViewBag.WeekStart = startOfWeek;
        ViewBag.WeeklyCalendar = _employeeService.GetWeeklyCalendar(id, startOfWeek);

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
        
        var employee = new Employee
        {
            FullName = model.FullName ?? string.Empty,
            Region = model.Region ?? string.Empty,
            City = model.City ?? string.Empty
        };

        var (isValid, errors) = _employeeService.Validate(employee);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (model.HasLogin)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                ModelState.AddModelError("UserName", "Kullanici adi zorunludur.");
            }
            else if (Db.Users.Any(x => x.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Bu kullanici adi zaten kullaniliyor.");
            }
            if (string.IsNullOrWhiteSpace(model.Password)) ModelState.AddModelError("Password", "Sifre zorunludur.");
            if (model.Role == null) ModelState.AddModelError("Role", "Rol secimi zorunludur.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _employeeService.Create(employee);

        if (model.HasLogin)
        {
            var userId = (Db.Users.Max(x => (int?)x.Id) ?? 0) + 1;
            Db.Users.Add(new UserAccount
            {
                Id = userId,
                EmployeeId = employee.Id,
                FullName = model.FullName!,
                UserName = model.UserName!,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role!.Value
            });
            Db.SaveChanges();
        }

        TempData["Success"] = "Personel kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var employee = Db.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null)
        {
            return NotFound();
        }

        var user = Db.Users.FirstOrDefault(x => x.EmployeeId == id);

        var model = new EmployeeFormViewModel
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Region = employee.Region,
            City = employee.City,
            HasLogin = user != null,
            UserName = user?.UserName,
            Role = user?.Role
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, EmployeeFormViewModel model)
    {
        BuildShell();
        var existingEmployee = _employeeService.GetById(id);
        if (existingEmployee is null)
        {
            return NotFound();
        }

        var tempEmployee = new Employee
        {
            Id = id,
            FullName = model.FullName ?? string.Empty,
            Region = model.Region ?? string.Empty,
            City = model.City ?? string.Empty
        };

        var (isValid, errors) = _employeeService.Validate(tempEmployee);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        var user = Db.Users.FirstOrDefault(x => x.EmployeeId == id);

        if (model.HasLogin)
        {
            int? existingUserId = user?.Id;
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                ModelState.AddModelError("UserName", "Kullanici adi zorunludur.");
            }
            else if (Db.Users.Any(x => x.UserName == model.UserName && x.Id != existingUserId))
            {
                ModelState.AddModelError("UserName", "Bu kullanici adi baska bir hesaba ait.");
            }

            if (user == null && string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Yeni hesap icin sifre zorunludur.");
            }

            if (model.Role == null) ModelState.AddModelError("Role", "Rol secimi zorunludur.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _employeeService.Update(id, tempEmployee);

        if (model.HasLogin)
        {
            if (user == null)
            {
                var userId = (Db.Users.Max(x => (int?)x.Id) ?? 0) + 1;
                Db.Users.Add(new UserAccount
                {
                    Id = userId,
                    EmployeeId = id,
                    FullName = model.FullName!,
                    UserName = model.UserName!,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = model.Role!.Value
                });
            }
            else
            {
                user.FullName = model.FullName!;
                user.UserName = model.UserName!;
                user.Role = model.Role!.Value;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }
            }
        }
        else if (user != null)
        {
            Db.Users.Remove(user);
        }

        Db.SaveChanges();
        TempData["Success"] = "Personel kaydi guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var success = _employeeService.Delete(id);
        if (!success) return NotFound();
        
        TempData["Success"] = "Personel kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
