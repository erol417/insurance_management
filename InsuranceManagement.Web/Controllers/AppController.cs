using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.Infrastructure.Json;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsuranceManagement.Web.Controllers;

[Authorize]
public abstract class AppController : Controller
{
    protected readonly AppDbContext Db;
    protected static readonly JsonSerializerOptions BulkSaveJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(),
            new NullableIntFromEmptyStringConverter(),
            new NullableDecimalFromEmptyStringConverter(),
            new NullableEnumFromEmptyStringConverterFactory()
        }
    };

    protected AppController(AppDbContext db)
    {
        Db = db;
    }

    protected UserAccount? CurrentAppUser => User.GetUserId() is { } userId ? Db.Users.FirstOrDefault(x => x.Id == userId) : null;

    protected void BuildShell()
    {
        var role = User.GetRoleType();
        if (role == null) return;

        ViewBag.AppShell = new AppShellViewModel
        {
            CurrentUserName = User.Identity?.Name ?? "Kullanici",
            CurrentRole = role,
            Groups = BuildNavigation(role.Value, User.GetEmployeeId())
        };
    }

    protected void QueueAudit(string module, string actionType, string entityCode, string detail)
    {
        var auditService = HttpContext.RequestServices.GetRequiredService<InsuranceManagement.Web.Services.IAuditService>();
        auditService.Log(module, actionType, entityCode, detail);
    }

    protected void MarkGridDraftCleared(string draftKey)
    {
        TempData["ClearGridDraftKey"] = draftKey;
    }

    protected bool CanSeeEmployeeData(int employeeId)
    {
        var role = User.GetRoleType();
        if (role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager or RoleType.Operations)
        {
            return true;
        }

        return role == RoleType.FieldSales && User.GetEmployeeId() == employeeId;
    }

    protected bool HasGlobalEmployeeAccess()
    {
        var role = User.GetRoleType();
        return role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager or RoleType.Operations;
    }

    protected int? CurrentEmployeeScopeId()
    {
        return User.GetRoleType() == RoleType.FieldSales ? User.GetEmployeeId() : null;
    }

    protected bool CanAccessPermission(string key)
    {
        var role = User.GetRoleType();
        if (role == null) return false;

        // Sahici Admin her zaman yetkilidir (Master Key)
        if (role == RoleType.Admin) return true;

        var perms = Db.RolePermissions.Where(x => x.Role == role).ToList();
        return !perms.Any() || perms.Any(x => x.ModuleKey == key && x.IsAllowed);
    }

    protected virtual List<NavGroupVm> BuildNavigation(RoleType role, int? currentEmpId)
    {
        var groups = new List<NavGroupVm>();

        // Dashboard
        if (CanAccessPermission("Dashboard"))
        {
            var dashboardItems = new List<NavItemVm>();
            if (role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager)
            {
                dashboardItems.Add(new NavItemVm { Label = "Yonetici Ozeti", Controller = "Dashboard", Action = "Executive" });
            }
            if (role == RoleType.FieldSales && currentEmpId.HasValue)
            {
                dashboardItems.Add(new NavItemVm { Label = "Performansim", Controller = "Dashboard", Action = "Performance", RouteValues = new { employeeId = currentEmpId } });
            }
            if (role == RoleType.CallCenter)
            {
                dashboardItems.Add(new NavItemVm { Label = "Lead Ozeti", Controller = "Dashboard", Action = "Executive" });
            }
            
            if (dashboardItems.Any())
                groups.Add(new NavGroupVm { Title = "Gosterge Panelleri", Items = dashboardItems });
        }

        // Leads
        if (CanAccessPermission("Leads"))
        {
            var leadItems = new List<NavItemVm>();
            leadItems.Add(new NavItemVm { Label = "Lead Havuzu", Controller = "Leads", Action = "Index" });
            
            if (role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager)
            {
                leadItems.Add(new NavItemVm { Label = "Atamalar", Controller = "Leads", Action = "Assignments" });
            }
            
            if (role == RoleType.FieldSales)
            {
                leadItems.Add(new NavItemVm { Label = "Atanan Leadlerim", Controller = "Leads", Action = "MyAssigned" });
            }

            groups.Add(new NavGroupVm { Title = "Lead Akisi", Items = leadItems });
        }

        // Operation
        var operationItems = new List<NavItemVm>();
        if (CanAccessPermission("Employees") && role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager or RoleType.Operations)
        {
            operationItems.Add(new NavItemVm { Label = "Personeller", Controller = "Employees", Action = "Index" });
        }
        
        if (CanAccessPermission("Accounts"))
        {
            operationItems.Add(new NavItemVm { Label = "Musteriler", Controller = "Accounts", Action = "Index" });
        }

        if (CanAccessPermission("Activities"))
        {
            operationItems.Add(new NavItemVm { Label = "Aktiviteler", Controller = "Activities", Action = "Index" });
        }
        
        if (CanAccessPermission("Sales"))
        {
            operationItems.Add(new NavItemVm { Label = "Satislar", Controller = "Sales", Action = "Index" });
        }
        
        if (CanAccessPermission("Expenses"))
        {
            operationItems.Add(new NavItemVm { Label = "Masraflar", Controller = "Expenses", Action = "Index" });
        }

        if (operationItems.Any())
        {
            groups.Add(new NavGroupVm { Title = "Operasyonel Kayitlar", Items = operationItems });
        }

        // Personal
        if (currentEmpId.HasValue)
        {
            groups.Add(new NavGroupVm
            {
                Title = "Kisisel",
                Items = [ new NavItemVm { Label = "Profilim / Ajandam", Controller = "Employees", Action = "Details", RouteValues = new { id = currentEmpId.Value } } ]
            });
        }

        // Admin / Management Group
        var adminItems = new List<NavItemVm>();
        
        if (role == RoleType.Admin && CanAccessPermission("Admin"))
        {
            adminItems.Add(new NavItemVm { Label = "Kullanicilar", Controller = "Admin", Action = "Users" });
            adminItems.Add(new NavItemVm { Label = "Roller ve Yetkiler", Controller = "Admin", Action = "Roles" });
            adminItems.Add(new NavItemVm { Label = "Referans Veriler", Controller = "Admin", Action = "ReferenceData" });
            adminItems.Add(new NavItemVm { Label = "Audit Kayitlari", Controller = "Admin", Action = "Audit" });
        }
        
        if (CanAccessPermission("Imports"))
        {
            adminItems.Add(new NavItemVm { Label = "Veri Aktarımı", Controller = "Imports", Action = "Upload" });
            adminItems.Add(new NavItemVm { Label = "Aktarım Geçmişi", Controller = "Imports", Action = "History" });
        }

        if (adminItems.Any())
        {
            groups.Add(new NavGroupVm { Title = "Yonetim", Items = adminItems });
        }

        return groups;
    }
}
