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
        ViewBag.AppShell = new AppShellViewModel
        {
            CurrentUserName = User.Identity?.Name ?? "Kullanici",
            CurrentRole = role,
            Groups = BuildNavigation(role)
        };
    }

    protected void QueueAudit(string module, string actionType, string entityCode, string detail)
    {
        var nextId = (Db.AuditLogs.Max(x => (int?)x.Id) ?? 0) + 1;
        Db.AuditLogs.Add(new AuditLog
        {
            Id = nextId,
            CreatedAt = DateTime.Now,
            UserName = User.Identity?.Name ?? "system",
            Module = module,
            ActionType = actionType,
            EntityCode = entityCode,
            Detail = detail
        });
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

    private static List<NavGroupVm> BuildNavigation(RoleType? role)
    {
        var groups = new List<NavGroupVm>
        {
            new()
            {
                Title = "Gosterge Panelleri",
                Items =
                [
                    new NavItemVm { Label = "Yonetici Ozeti", Controller = "Dashboard", Action = "Executive" },
                    new NavItemVm { Label = "Personel Performansi", Controller = "Dashboard", Action = "Performance" },
                    new NavItemVm { Label = "Urun Kirilimi", Controller = "Dashboard", Action = "Products" },
                    new NavItemVm { Label = "Masraf Analizi", Controller = "Dashboard", Action = "Expenses" }
                ]
            }
        };

        if (role is RoleType.Admin or RoleType.Manager or RoleType.SalesManager or RoleType.CallCenter or RoleType.FieldSales)
        {
            groups.Add(new NavGroupVm
            {
                Title = "Lead Akisi",
                Items =
                [
                    new NavItemVm { Label = "Lead Havuzu", Controller = "Leads", Action = "Index" },
                    new NavItemVm { Label = "Atamalar", Controller = "Leads", Action = "Assignments" },
                    new NavItemVm { Label = "Atanan Leadlerim", Controller = "Leads", Action = "MyAssigned" }
                ]
            });
        }

        groups.Add(new NavGroupVm
        {
            Title = "Operasyon",
            Items =
            [
                new NavItemVm { Label = "Personeller", Controller = "Employees", Action = "Index" },
                new NavItemVm { Label = "Musteriler", Controller = "Accounts", Action = "Index" },
                new NavItemVm { Label = "Aktiviteler", Controller = "Activities", Action = "Index" },
                new NavItemVm { Label = "Satislar", Controller = "Sales", Action = "Index" },
                new NavItemVm { Label = "Masraflar", Controller = "Expenses", Action = "Index" }
            ]
        });

        if (role is RoleType.Admin or RoleType.Manager or RoleType.Operations)
        {
            groups.Add(new NavGroupVm
            {
                Title = "Yonetim",
                Items =
                [
                    new NavItemVm { Label = "Kullanicilar", Controller = "Admin", Action = "Users" },
                    new NavItemVm { Label = "Rol Matrisi", Controller = "Admin", Action = "Roles" },
                    new NavItemVm { Label = "Referans Veriler", Controller = "Admin", Action = "ReferenceData" },
                    new NavItemVm { Label = "Audit Kayitlari", Controller = "Admin", Action = "Audit" },
                    new NavItemVm { Label = "Import Yukleme", Controller = "Imports", Action = "Upload" },
                    new NavItemVm { Label = "Import Gecmisi", Controller = "Imports", Action = "History" }
                ]
            });
        }

        return groups;
    }
}
