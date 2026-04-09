using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

public class ActivitiesController : AppController
{
    public ActivitiesController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        BuildShell();
        var itemsQuery = Db.Activities.AsQueryable();
        var currentEmployeeId = CurrentEmployeeScopeId();
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            itemsQuery = itemsQuery.Where(x => x.EmployeeId == currentEmployeeId.Value);
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.OrderBy(x => x.DisplayName).ToList();
        ViewBag.Leads = Db.Leads.OrderByDescending(x => x.CreatedAt).Take(200).ToList();

        var totalCount = itemsQuery.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        return View(new ActivitiesIndexViewModel
        {
            NewActivity = new ActivityInlineEditViewModel
            {
                ActivityDate = DateTime.Today,
                EmployeeId = currentEmployeeId ?? Db.Employees.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault()
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = itemsQuery
                .OrderByDescending(x => x.ActivityDate)
                .ThenByDescending(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ActivityInlineEditViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    ActivityDate = x.ActivityDate,
                    EmployeeId = x.EmployeeId,
                    AccountId = x.AccountId,
                    LeadId = x.LeadId,
                    ContactName = x.ContactName,
                    ContactStatus = x.ContactStatus,
                    OutcomeStatus = x.OutcomeStatus,
                    Summary = x.Summary
                })
                .ToList()
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
                activity.ContactStatus = formModel.ContactStatus;
                activity.OutcomeStatus = formModel.OutcomeStatus;
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
                    ContactStatus = formModel.ContactStatus,
                    OutcomeStatus = formModel.OutcomeStatus,
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
        var activity = Db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null || !CanSeeEmployeeData(activity.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = Db.Employees.FirstOrDefault(x => x.Id == activity.EmployeeId);
        ViewBag.Account = Db.Accounts.FirstOrDefault(x => x.Id == activity.AccountId);
        ViewBag.Sales = Db.Sales.Where(x => x.ActivityId == id).ToList();
        return View(activity);
    }

    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        var employeeId = User.GetEmployeeId();
        ViewBag.Leads = Db.Leads.Where(x => x.AssignedEmployeeId == employeeId || User.IsInRole("Admin") || User.IsInRole("SalesManager")).ToList();
        return View(new ActivityFormViewModel { EmployeeId = employeeId ?? Db.Employees.OrderBy(x => x.Id).First().Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ActivityFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();

        if (model.ContactStatus == ContactStatus.Contacted && model.OutcomeStatus is null)
        {
            ModelState.AddModelError(nameof(model.OutcomeStatus), "Gorusuldu ise sonuc secilmelidir.");
        }

        if (model.ContactStatus == ContactStatus.NotContacted)
        {
            model.OutcomeStatus = null;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Activities.Max(x => (int?)x.Id) ?? 100) + 1;
        var activity = new Activity
        {
            Id = id,
            Code = $"ACT-{id}",
            ActivityDate = model.ActivityDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            LeadId = model.LeadId,
            ContactName = model.ContactName,
            ContactStatus = model.ContactStatus,
            OutcomeStatus = model.OutcomeStatus,
            Summary = model.Summary
        };
        Db.Activities.Add(activity);
        QueueAudit("Activity", "Create", activity.Code, $"Aktivite olusturuldu. Musteri #{activity.AccountId}, personel #{activity.EmployeeId}.");

        Db.SaveChanges();
        TempData["Flash"] = "Aktivite kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

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

        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();
        return View(new ActivityFormViewModel
        {
            Id = activity.Id,
            ActivityDate = activity.ActivityDate,
            EmployeeId = activity.EmployeeId,
            AccountId = activity.AccountId,
            LeadId = activity.LeadId,
            ContactName = activity.ContactName,
            ContactStatus = activity.ContactStatus,
            OutcomeStatus = activity.OutcomeStatus,
            Summary = activity.Summary
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ActivityFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Leads = Db.Leads.ToList();

        if (model.ContactStatus == ContactStatus.Contacted && model.OutcomeStatus is null)
        {
            ModelState.AddModelError(nameof(model.OutcomeStatus), "Gorusuldu ise sonuc secilmelidir.");
        }

        if (model.ContactStatus == ContactStatus.NotContacted)
        {
            model.OutcomeStatus = null;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var activity = Db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(activity.EmployeeId))
        {
            return NotFound();
        }

        activity.ActivityDate = model.ActivityDate;
        activity.EmployeeId = model.EmployeeId;
        activity.AccountId = model.AccountId;
        activity.LeadId = model.LeadId;
        activity.ContactName = model.ContactName;
        activity.ContactStatus = model.ContactStatus;
        activity.OutcomeStatus = model.OutcomeStatus;
        activity.Summary = model.Summary;
        QueueAudit("Activity", "Update", activity.Code, $"{activity.Code} aktivitesi guncellendi.");
        Db.SaveChanges();

        TempData["Flash"] = "Aktivite kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var activity = Db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(activity.EmployeeId))
        {
            return NotFound();
        }

        QueueAudit("Activity", "Delete", activity.Code, $"{activity.Code} aktivitesi silindi.");
        Db.Activities.Remove(activity);
        Db.SaveChanges();
        TempData["Flash"] = "Aktivite kaydi silindi.";
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
            ContactStatus = model.ContactStatus,
            OutcomeStatus = model.OutcomeStatus,
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

        if (model.ContactStatus == ContactStatus.Contacted && model.OutcomeStatus is null)
        {
            errors.Add("Gorusuldu ise sonuc secilmelidir.");
        }

        if (model.ContactStatus == ContactStatus.NotContacted)
        {
            model.OutcomeStatus = null;
        }

        return errors;
    }
}
