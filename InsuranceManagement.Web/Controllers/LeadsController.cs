using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

public class LeadsController : AppController
{
    public LeadsController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        BuildShell();
        ViewBag.EmployeeMap = Db.Employees.ToDictionary(x => x.Id, x => x.FullName);
        ViewBag.Employees = Db.Employees.OrderBy(x => x.FullName).ToList();
        var totalCount = Db.Leads.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);
        return View(new LeadsIndexViewModel
        {
            NewLead = new LeadInlineEditViewModel(),
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = Db.Leads
                .OrderByDescending(x => x.CreatedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new LeadInlineEditViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    DisplayName = x.DisplayName,
                    City = x.City,
                    District = x.District,
                    ContactName = x.ContactName,
                    Phone = x.Phone,
                    Email = x.Email,
                    Source = x.Source,
                    Status = x.Status,
                    Priority = x.Priority,
                    Note = x.Note,
                    AssignedEmployeeId = x.AssignedEmployeeId
                })
                .ToList()
        });
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(LeadGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        List<LeadInlineEditViewModel>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<LeadInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
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

        var warnings = new List<string>();
        var errors = new List<string>();
        var createdCount = 0;
        var updatedCount = 0;
        var nextId = (Db.Leads.Max(x => (int?)x.Id) ?? 100) + 1;

        foreach (var item in payload)
        {
            var formModel = ToLeadFormModel(item);
            var validationErrors = ValidateInlineLead(formModel);
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(x => $"{item.DisplayName ?? "Yeni Satir"}: {x}"));
                continue;
            }

            if (item.Id.HasValue)
            {
                var lead = Db.Leads.FirstOrDefault(x => x.Id == item.Id.Value);
                if (lead is null)
                {
                    errors.Add($"#{item.Id.Value} id'li lead bulunamadi.");
                    continue;
                }

                warnings.AddRange(BuildDuplicateWarnings(formModel, item.Id).Select(x => $"{lead.Code}: {x}"));
                lead.DisplayName = formModel.DisplayName;
                lead.City = formModel.City;
                lead.District = formModel.District;
                lead.ContactName = formModel.ContactName;
                lead.Phone = formModel.Phone;
                lead.Email = formModel.Email;
                lead.Source = formModel.Source;
                lead.Status = formModel.Status;
                lead.Priority = formModel.Priority;
                lead.Note = formModel.Note;
                lead.AssignedEmployeeId = item.AssignedEmployeeId;
                QueueAudit("Lead", "Update", lead.Code, $"{lead.DisplayName} lead kaydi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                warnings.AddRange(BuildDuplicateWarnings(formModel, null).Select(x => $"Yeni Lead: {x}"));
                var lead = new Lead
                {
                    Id = nextId,
                    Code = $"LD-{nextId}",
                    DisplayName = formModel.DisplayName,
                    City = formModel.City,
                    District = formModel.District,
                    ContactName = formModel.ContactName,
                    Phone = formModel.Phone,
                    Email = formModel.Email,
                    Source = formModel.Source,
                    Status = formModel.Status,
                    Priority = formModel.Priority,
                    Note = formModel.Note,
                    AssignedEmployeeId = item.AssignedEmployeeId,
                    CreatedAt = DateTime.Today
                };
                Db.Leads.Add(lead);
                QueueAudit("Lead", "Create", lead.Code, $"{lead.DisplayName} lead kaydi toplu grid kaydi ile olusturuldu.");
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
        MarkGridDraftCleared("lead-grid-draft-v1");
        TempData["Flash"] = $"{createdCount} yeni, {updatedCount} mevcut lead kaydi toplu kaydedildi.";
        if (warnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", warnings.Distinct());
        }

        return RedirectToAction(nameof(Index), new { page = model.Page });
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuickCreate(LeadInlineEditViewModel model)
    {
        var formModel = ToLeadFormModel(model);
        var duplicateWarnings = BuildDuplicateWarnings(formModel, null);
        var validationErrors = ValidateInlineLead(formModel);
        if (validationErrors.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", validationErrors);
            return RedirectToAction(nameof(Index));
        }

        var id = (Db.Leads.Max(x => (int?)x.Id) ?? 100) + 1;
        var lead = new Lead
        {
            Id = id,
            Code = $"LD-{id}",
            DisplayName = formModel.DisplayName,
            City = formModel.City,
            District = formModel.District,
            ContactName = formModel.ContactName,
            Phone = formModel.Phone,
            Email = formModel.Email,
            Source = formModel.Source,
            Status = formModel.Status,
            Priority = formModel.Priority,
            Note = formModel.Note,
            AssignedEmployeeId = model.AssignedEmployeeId,
            CreatedAt = DateTime.Today
        };
        Db.Leads.Add(lead);
        QueueAudit("Lead", "Create", lead.Code, $"{lead.DisplayName} lead kaydi grid uzerinden olusturuldu.");
        Db.SaveChanges();

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }

        TempData["Flash"] = "Lead satiri eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuickUpdate(LeadInlineEditViewModel model)
    {
        if (!model.Id.HasValue)
        {
            return NotFound();
        }

        var lead = Db.Leads.FirstOrDefault(x => x.Id == model.Id.Value);
        if (lead is null)
        {
            return NotFound();
        }

        var formModel = ToLeadFormModel(model);
        var duplicateWarnings = BuildDuplicateWarnings(formModel, model.Id);
        var validationErrors = ValidateInlineLead(formModel);
        if (validationErrors.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", validationErrors);
            return RedirectToAction(nameof(Index));
        }

        lead.DisplayName = formModel.DisplayName;
        lead.City = formModel.City;
        lead.District = formModel.District;
        lead.ContactName = formModel.ContactName;
        lead.Phone = formModel.Phone;
        lead.Email = formModel.Email;
        lead.Source = formModel.Source;
        lead.Status = formModel.Status;
        lead.Priority = formModel.Priority;
        lead.Note = formModel.Note;
        lead.AssignedEmployeeId = model.AssignedEmployeeId;
        QueueAudit("Lead", "Update", lead.Code, $"{lead.DisplayName} lead kaydi grid uzerinden guncellendi.");
        Db.SaveChanges();

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }

        TempData["Flash"] = "Lead satiri guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        ViewBag.Employee = lead.AssignedEmployeeId.HasValue ? Db.Employees.FirstOrDefault(x => x.Id == lead.AssignedEmployeeId.Value) : null;
        ViewBag.ConvertedAccount = lead.ConvertedAccountId.HasValue ? Db.Accounts.FirstOrDefault(x => x.Id == lead.ConvertedAccountId.Value) : null;
        ViewBag.ConvertedActivity = lead.ConvertedActivityId.HasValue ? Db.Activities.FirstOrDefault(x => x.Id == lead.ConvertedActivityId.Value) : null;
        return View(lead);
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.DuplicateWarnings = Array.Empty<string>();
        return View(new LeadFormViewModel());
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(LeadFormViewModel model)
    {
        BuildShell();
        var duplicateWarnings = BuildDuplicateWarnings(model, null);
        ViewBag.DuplicateWarnings = duplicateWarnings;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Leads.Max(x => (int?)x.Id) ?? 100) + 1;
        var lead = new Lead
        {
            Id = id,
            Code = $"LD-{id}",
            DisplayName = model.DisplayName,
            City = model.City,
            District = model.District,
            ContactName = model.ContactName,
            Phone = model.Phone,
            Email = model.Email,
            Source = model.Source,
            Status = model.Status,
            Priority = model.Priority,
            Note = model.Note,
            CreatedAt = DateTime.Today
        };
        Db.Leads.Add(lead);
        QueueAudit("Lead", "Create", lead.Code, $"{lead.DisplayName} lead kaydi olusturuldu.");

        Db.SaveChanges();
        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }
        TempData["Flash"] = "Lead kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        var model = new LeadFormViewModel
        {
            Id = lead.Id,
            DisplayName = lead.DisplayName,
            City = lead.City,
            District = lead.District,
            ContactName = lead.ContactName,
            Phone = lead.Phone,
            Email = lead.Email,
            Source = lead.Source,
            Status = lead.Status,
            Priority = lead.Priority,
            Note = lead.Note
        };
        ViewBag.DuplicateWarnings = BuildDuplicateWarnings(model, id);
        return View(model);
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, LeadFormViewModel model)
    {
        BuildShell();
        var duplicateWarnings = BuildDuplicateWarnings(model, id);
        ViewBag.DuplicateWarnings = duplicateWarnings;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        lead.DisplayName = model.DisplayName;
        lead.City = model.City;
        lead.District = model.District;
        lead.ContactName = model.ContactName;
        lead.Phone = model.Phone;
        lead.Email = model.Email;
        lead.Source = model.Source;
        lead.Status = model.Status;
        lead.Priority = model.Priority;
        lead.Note = model.Note;
        QueueAudit("Lead", "Update", lead.Code, $"{lead.DisplayName} lead kaydi guncellendi.");
        Db.SaveChanges();

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }
        TempData["Flash"] = "Lead kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin,Manager,SalesManager")]
    [HttpGet]
    public IActionResult Assignments()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.AssignedLeads = Db.Leads
            .Where(x => x.Status == LeadStatus.Assigned || x.Status == LeadStatus.VisitScheduled)
            .OrderBy(x => x.ScheduledVisitDate ?? DateTime.MaxValue)
            .ThenBy(x => x.DisplayName)
            .ToList();
        return View(Db.Leads
            .Where(x => x.Status == LeadStatus.ReadyForAssignment)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.DisplayName)
            .ToList());
    }

    [Authorize(Roles = "Admin,Manager,SalesManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Assign(int leadId, int employeeId, DateTime? scheduledVisitDate, string priority, string note)
    {
        var lead = Db.Leads.FirstOrDefault(x => x.Id == leadId);
        if (lead is null)
        {
            return NotFound();
        }

        lead.AssignedEmployeeId = employeeId;
        lead.ScheduledVisitDate = scheduledVisitDate;
        lead.Priority = string.IsNullOrWhiteSpace(priority) ? lead.Priority : priority;
        lead.Note = string.IsNullOrWhiteSpace(note) ? lead.Note : note;
        lead.Status = LeadStatus.Assigned;
        var employee = Db.Employees.FirstOrDefault(x => x.Id == employeeId);
        QueueAudit("Lead", "Assign", lead.Code, $"{lead.DisplayName} leadi {employee?.FullName ?? "Bilinmeyen Personel"} kişisine atandi.");
        Db.SaveChanges();

        TempData["Flash"] = "Lead atamasi kaydedildi.";
        return RedirectToAction(nameof(Assignments));
    }

    [Authorize(Roles = "FieldSales")]
    public IActionResult MyAssigned()
    {
        BuildShell();
        var employeeId = User.GetEmployeeId();
        var leads = Db.Leads.Where(x => x.AssignedEmployeeId == employeeId && (x.Status == LeadStatus.Assigned || x.Status == LeadStatus.VisitScheduled)).ToList();
        return View(leads);
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        QueueAudit("Lead", "Delete", lead.Code, $"{lead.DisplayName} lead kaydi silindi.");
        Db.Leads.Remove(lead);
        Db.SaveChanges();
        TempData["Flash"] = "Lead kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "FieldSales,Admin,SalesManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartVisit(int id)
    {
        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        var employeeId = lead.AssignedEmployeeId ?? User.GetEmployeeId() ?? 1;
        var accountId = (Db.Accounts.Max(x => (int?)x.Id) ?? 100) + 1;
        var account = new Account
        {
            Id = accountId,
            Code = $"ACC-{accountId}",
            AccountType = AccountType.Corporate,
            DisplayName = lead.DisplayName,
            City = lead.City,
            District = lead.District,
            Phone = lead.Phone,
            Email = lead.Email,
            OwnerEmployeeId = employeeId,
            Notes = $"Lead donusumu: {lead.Code}",
            Status = "Active"
        };
        Db.Accounts.Add(account);

        var activityId = (Db.Activities.Max(x => (int?)x.Id) ?? 100) + 1;
        var activity = new Activity
        {
            Id = activityId,
            Code = $"ACT-{activityId}",
            ActivityDate = DateTime.Today,
            EmployeeId = employeeId,
            AccountId = accountId,
            LeadId = lead.Id,
            ContactName = lead.ContactName,
            ContactStatus = ContactStatus.Contacted,
            OutcomeStatus = OutcomeStatus.Positive,
            Summary = $"Lead ziyareti baslatildi: {lead.DisplayName}"
        };
        Db.Activities.Add(activity);

        lead.Status = LeadStatus.ConvertedToActivity;
        lead.ConvertedAccountId = accountId;
        lead.ConvertedActivityId = activityId;
        QueueAudit("Lead", "Convert", lead.Code, $"{lead.DisplayName} leadi {account.Code} ve {activity.Code} kayitlarina donusturuldu.");

        Db.SaveChanges();
        TempData["Flash"] = "Lead account + activity zincirine donusturuldu.";
        return RedirectToAction("Details", "Activities", new { id = activityId });
    }

    private List<string> BuildDuplicateWarnings(LeadFormViewModel model, int? currentId)
    {
        var warnings = new List<string>();
        var leads = Db.Leads.Where(x => !currentId.HasValue || x.Id != currentId.Value);

        if (!string.IsNullOrWhiteSpace(model.DisplayName) &&
            leads.Any(x => x.DisplayName == model.DisplayName))
        {
            warnings.Add("Ayni isimle baska bir lead kaydi bulundu.");
        }

        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            if (leads.Any(x => x.Phone == model.Phone))
            {
                warnings.Add("Bu telefon numarasi baska bir lead kaydinda kullaniliyor.");
            }

            if (Db.Accounts.Any(x => x.Phone == model.Phone))
            {
                warnings.Add("Bu telefon numarasi mevcut musteri kayitlarinda da bulunuyor.");
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            if (leads.Any(x => x.Email == model.Email))
            {
                warnings.Add("Bu e-posta adresi baska bir lead kaydinda kullaniliyor.");
            }

            if (Db.Accounts.Any(x => x.Email == model.Email))
            {
                warnings.Add("Bu e-posta adresi mevcut musteri kayitlarinda da bulunuyor.");
            }
        }

        return warnings;
    }

    private static LeadFormViewModel ToLeadFormModel(LeadInlineEditViewModel model)
    {
        return new LeadFormViewModel
        {
            Id = model.Id,
            DisplayName = model.DisplayName,
            City = model.City,
            District = model.District,
            ContactName = model.ContactName,
            Phone = model.Phone,
            Email = model.Email,
            Source = model.Source,
            Status = model.Status,
            Priority = model.Priority,
            Note = model.Note
        };
    }

    private static List<string> ValidateInlineLead(LeadFormViewModel model)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(model.DisplayName))
        {
            errors.Add("Lead adi/unvani bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(model.City))
        {
            errors.Add("Sehir alani bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.Email))
        {
            errors.Add("Telefon veya e-posta alanlarindan en az biri dolu olmalidir.");
        }

        return errors;
    }
}
