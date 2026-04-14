using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class ActivitiesController : AppController
{
    private readonly InsuranceManagement.Web.Services.IActivityService _activityService;

    public ActivitiesController(AppDbContext db, InsuranceManagement.Web.Services.IActivityService activityService) : base(db)
    {
        _activityService = activityService;
    }

    public IActionResult Index(int page = 1, int pageSize = 10, string? searchTerm = null, int? employeeId = null, string? sortBy = "date", bool isDescending = true)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        
        int? filterEmployeeId = employeeId;
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            filterEmployeeId = currentEmployeeId.Value;
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.OrderBy(x => x.DisplayName).ToList();
        ViewBag.Leads = Db.Leads.OrderByDescending(x => x.CreatedAt).Take(200).ToList();
        ViewBag.ContactStatusTypes = Db.ActivityContactStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.OutcomeStatusTypes = Db.ActivityOutcomeStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        
        ViewBag.SearchTerm = searchTerm;
        ViewBag.EmployeeId = filterEmployeeId;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;

        ViewBag.PlannedActivities = Db.Activities
            .Include(x => x.Account)
            .Include(x => x.Employee)
            .Where(x => x.ContactStatusTypeId == 3 && (!filterEmployeeId.HasValue || x.EmployeeId == filterEmployeeId))
            .OrderBy(x => x.ActivityDate)
            .ToList();

        var items = _activityService.GetAll(page, pageSize, out var totalCount, searchTerm, filterEmployeeId, CurrentEmployeeScopeId(), sortBy, isDescending);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = page;

        return View(new ActivitiesIndexViewModel
        {
            NewActivity = new ActivityInlineEditViewModel 
            { 
               ActivityDate = DateTime.Today,
               EmployeeId = currentEmployeeId ?? 0
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = items.Select(x => new ActivityInlineEditViewModel
            {
                Id = x.Id,
                Code = x.Code,
                ActivityDate = x.ActivityDate,
                EmployeeId = x.EmployeeId,
                AccountId = x.AccountId,
                LeadId = x.LeadId,
                ContactName = x.ContactName,
                ContactStatusTypeId = x.ContactStatusTypeId,
                OutcomeStatusTypeId = x.OutcomeStatusTypeId ?? 0,
                Summary = x.Summary
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(ActivityGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        List<ActivityInlineEditViewModel>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<ActivityInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
        }
        catch
        {
            TempData["Warning"] = "Taslak degisiklikler okunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        if (payload is null || payload.Count == 0)
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        var errors = new List<string>();
        var createdCount = 0;
        var updatedCount = 0;
        var nextId = (Db.Activities.Max(x => (int?)x.Id) ?? 100) + 1;
        var currentEmployeeId = CurrentEmployeeScopeId();
        var canManageAll = HasGlobalEmployeeAccess();

        foreach (var item in payload)
        {
            var formModel = ToActivityFormModel(item, currentEmployeeId, canManageAll);
            var validationErrors = ValidateInlineActivity(formModel);
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(x => $"{(string.IsNullOrWhiteSpace(item.Code) ? "Yeni Satir" : item.Code)}: {x}"));
                continue;
            }

            if (item.Id.HasValue)
            {
                var activity = Db.Activities.FirstOrDefault(x => x.Id == item.Id.Value);
                if (activity is null)
                {
                    errors.Add($"#{item.Id.Value} id'li aktivite bulunamadi.");
                    continue;
                }

                if (!CanSeeEmployeeData(activity.EmployeeId) || (!canManageAll && formModel.EmployeeId != currentEmployeeId))
                {
                    errors.Add($"{activity.Code}: Bu kaydi guncelleme yetkin yok.");
                    continue;
                }

                activity.ActivityDate = formModel.ActivityDate;
                activity.EmployeeId = formModel.EmployeeId;
                activity.AccountId = formModel.AccountId;
                activity.LeadId = formModel.LeadId;
                activity.ContactName = formModel.ContactName;
                activity.ContactStatusTypeId = formModel.ContactStatusTypeId;
                activity.OutcomeStatusTypeId = formModel.OutcomeStatusTypeId;
                activity.Summary = formModel.Summary;
                QueueAudit("Activity", "Update", activity.Code, $"{activity.Code} aktivitesi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                var activity = new Activity
                {
                    Id = nextId,
                    Code = $"ACT-{nextId}",
                    ActivityDate = formModel.ActivityDate,
                    EmployeeId = formModel.EmployeeId,
                    AccountId = formModel.AccountId,
                    LeadId = formModel.LeadId,
                    ContactName = formModel.ContactName,
                    ContactStatusTypeId = formModel.ContactStatusTypeId,
                    OutcomeStatusTypeId = formModel.OutcomeStatusTypeId,
                    Summary = formModel.Summary
                };
                Db.Activities.Add(activity);
                QueueAudit("Activity", "Create", activity.Code, $"{activity.Code} aktivitesi toplu grid kaydi ile olusturuldu.");
                createdCount++;
                nextId++;
            }
        }

        if (errors.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", errors.Distinct());
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        Db.SaveChanges();
        MarkGridDraftCleared("activity-grid-draft-v1");
        TempData["Flash"] = $"{createdCount} yeni, {updatedCount} mevcut aktivite toplu kaydedildi.";
        return RedirectToAction(nameof(Index), new { page = model.Page });
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var activity = _activityService.GetById(id, CurrentEmployeeScopeId());
        if (activity is null || !CanSeeEmployeeData(activity.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = activity.Employee;
        ViewBag.Account = activity.Account;
        ViewBag.Sales = Db.Sales.Include(x => x.InsuranceProductType).Where(x => x.ActivityId == id).ToList();
        return View(activity);
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.ContactStatusTypes = Db.ActivityContactStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.OutcomeStatusTypes = Db.ActivityOutcomeStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        var employeeId = User.GetEmployeeId();
        ViewBag.Leads = Db.Leads.Where(x => x.AssignedEmployeeId == employeeId || User.IsInRole("Admin") || User.IsInRole("SalesManager")).ToList();
        return View(new ActivityFormViewModel { EmployeeId = employeeId ?? Db.Employees.OrderBy(x => x.Id).First().Id });
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ActivityFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();
        ViewBag.ContactStatusTypes = Db.ActivityContactStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.OutcomeStatusTypes = Db.ActivityOutcomeStatusTypes.OrderBy(x => x.DisplayOrder).ToList();

        var activity = new Activity
        {
            ActivityDate = model.ActivityDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            LeadId = model.LeadId,
            ContactName = model.ContactName ?? string.Empty,
            ContactStatusTypeId = model.ContactStatusTypeId,
            OutcomeStatusTypeId = model.OutcomeStatusTypeId,
            Summary = model.Summary ?? string.Empty
        };

        var (isValid, errors) = _activityService.Validate(activity);
        
        if (!isValid)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _activityService.Create(activity);
        TempData["Success"] = "Aktivite kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var activity = Db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(activity.EmployeeId))
        {
            return NotFound();
        }

        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();
        ViewBag.ContactStatusTypes = Db.ActivityContactStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.OutcomeStatusTypes = Db.ActivityOutcomeStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        return View(new ActivityFormViewModel
        {
            Id = activity.Id,
            ActivityDate = activity.ActivityDate,
            EmployeeId = activity.EmployeeId,
            AccountId = activity.AccountId,
            LeadId = activity.LeadId,
            ContactName = activity.ContactName,
            ContactStatusTypeId = activity.ContactStatusTypeId,
            OutcomeStatusTypeId = activity.OutcomeStatusTypeId,
            Summary = activity.Summary
        });
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ActivityFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();
        ViewBag.ContactStatusTypes = Db.ActivityContactStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.OutcomeStatusTypes = Db.ActivityOutcomeStatusTypes.OrderBy(x => x.DisplayOrder).ToList();

        var tempActivity = new Activity
        {
            Id = id,
            ActivityDate = model.ActivityDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            LeadId = model.LeadId,
            ContactName = model.ContactName ?? string.Empty,
            ContactStatusTypeId = model.ContactStatusTypeId,
            OutcomeStatusTypeId = model.OutcomeStatusTypeId,
            Summary = model.Summary ?? string.Empty
        };

        var (isValid, errors) = _activityService.Validate(tempActivity);
        
        if (!isValid)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = _activityService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null)
        {
            return NotFound();
        }

        var updated = _activityService.Update(id, tempActivity);
        if (updated == null)
        {
            return NotFound();
        }

        TempData["Success"] = "Aktivite kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin,Operations")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var existing = _activityService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null)
        {
            return NotFound();
        }

        var success = _activityService.Delete(id);
        if (success)
        {
            TempData["Success"] = "Aktivite silindi.";
        }
        else
        {
            TempData["Warning"] = "Silme islemi basarisiz oldu.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    private ActivityFormViewModel ToActivityFormModel(ActivityInlineEditViewModel model, int? currentEmployeeId, bool canManageAll)
    {
        return new ActivityFormViewModel
        {
            Id = model.Id,
            ActivityDate = model.ActivityDate == default ? DateTime.Today : model.ActivityDate,
            EmployeeId = canManageAll ? model.EmployeeId : currentEmployeeId ?? model.EmployeeId,
            AccountId = model.AccountId,
            LeadId = model.LeadId,
            ContactName = model.ContactName,
            ContactStatusTypeId = model.ContactStatusTypeId,
            OutcomeStatusTypeId = model.OutcomeStatusTypeId,
            Summary = model.Summary
        };
    }

    private static List<string> ValidateInlineActivity(ActivityFormViewModel model)
    {
        var errors = new List<string>();

        if (model.ActivityDate == default)
        {
            errors.Add("Tarih zorunludur.");
        }

        if (model.EmployeeId <= 0)
        {
            errors.Add("Personel secilmelidir.");
        }

        if (model.AccountId <= 0)
        {
            errors.Add("Musteri secilmelidir.");
        }

        if (string.IsNullOrWhiteSpace(model.Summary))
        {
            errors.Add("Ozet zorunludur.");
        }

        if (model.ContactStatusTypeId == 1 && model.OutcomeStatusTypeId is null)
        {
            errors.Add("Gorusuldu ise sonuc secilmelidir.");
        }

        if (model.ContactStatusTypeId == 2)
        {
            model.OutcomeStatusTypeId = null;
        }

        return errors;
    }
}
