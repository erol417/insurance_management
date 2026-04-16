using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.Extensions;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

[Authorize]
public class LeadsController : AppController
{
    private readonly InsuranceManagement.Web.Services.ILeadService _leadService;

    public LeadsController(AppDbContext db, InsuranceManagement.Web.Services.ILeadService leadService) : base(db)
    {
        _leadService = leadService;
    }

    public IActionResult Index(int page = 1, int pageSize = 10, string? searchTerm = null, int? statusId = null, int? employeeId = null, string? sortBy = "date", bool isDescending = true)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.OrderBy(x => x.FullName).ToList();
        ViewBag.StatusTypes = Db.LeadStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.SourceTypes = Db.LeadSourceTypes.OrderBy(x => x.DisplayOrder).ToList();
        
        ViewBag.SearchTerm = searchTerm;
        ViewBag.StatusId = statusId;
        ViewBag.EmployeeId = employeeId;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;
        
        var filterEmployeeId = CurrentEmployeeScopeId();
        var leads = _leadService.GetAll(page, pageSize, out var totalCount, searchTerm, statusId, employeeId, filterEmployeeId, sortBy, isDescending);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = page; 
        
        return View(new LeadsIndexViewModel
        {
            NewLead = new LeadInlineEditViewModel(),
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = leads
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
                    LeadSourceTypeId = x.LeadSourceTypeId,
                    LeadStatusTypeId = x.LeadStatusTypeId,
                    Priority = x.Priority,
                    Note = x.Note,
                    AssignedEmployeeId = x.AssignedEmployeeId,
                    CreatedAt = x.CreatedAt
                })
                .ToList()
        });
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter,Manager,FieldSales")]
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
                lead.LeadSourceTypeId = formModel.LeadSourceTypeId;
                lead.LeadStatusTypeId = formModel.LeadStatusTypeId;
                lead.Priority = formModel.Priority;
                lead.Note = formModel.Note;
                lead.AssignedEmployeeId = item.AssignedEmployeeId;

                // Auto-set status to Assigned (Id: 3) if an employee is assigned
                if (lead.AssignedEmployeeId.HasValue && (lead.LeadStatusTypeId == 1 || lead.LeadStatusTypeId == 2))
                {
                    lead.LeadStatusTypeId = 3;
                }
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
                    LeadSourceTypeId = formModel.LeadSourceTypeId,
                    LeadStatusTypeId = formModel.LeadStatusTypeId,
                    Priority = formModel.Priority,
                    Note = formModel.Note,
                    AssignedEmployeeId = item.AssignedEmployeeId
                };

                // Auto-set status to Assigned (Id: 3) if an employee is assigned
                if (lead.AssignedEmployeeId.HasValue && (lead.LeadStatusTypeId == 1 || lead.LeadStatusTypeId == 2))
                {
                    lead.LeadStatusTypeId = 3;
                }
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuickCreate(LeadInlineEditViewModel model)
    {
        var formModel = ToLeadFormModel(model);
        var duplicateWarnings = BuildDuplicateWarnings(formModel, null);
        
        var lead = new Lead
        {
            DisplayName = formModel.DisplayName,
            City = formModel.City,
            District = formModel.District,
            ContactName = formModel.ContactName ?? string.Empty,
            Phone = formModel.Phone,
            Email = formModel.Email,
            LeadSourceTypeId = formModel.LeadSourceTypeId,
            LeadStatusTypeId = formModel.LeadStatusTypeId,
            Priority = formModel.Priority,
            Note = formModel.Note,
            AssignedEmployeeId = model.AssignedEmployeeId
        };
        
        var (isValid, validationErrors) = _leadService.Validate(lead);
        if (!isValid)
        {
            TempData["Warning"] = string.Join(" | ", validationErrors.Values);
            return RedirectToAction(nameof(Index));
        }

        _leadService.Create(lead);

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }

        TempData["Success"] = "Lead satiri eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter,Manager,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult QuickUpdate(LeadInlineEditViewModel model)
    {
        if (!model.Id.HasValue)
        {
            return NotFound();
        }

        var formModel = ToLeadFormModel(model);
        var duplicateWarnings = BuildDuplicateWarnings(formModel, model.Id);
        
        var tempLead = new Lead
        {
            DisplayName = formModel.DisplayName,
            City = formModel.City,
            District = formModel.District,
            ContactName = formModel.ContactName ?? string.Empty,
            Phone = formModel.Phone,
            Email = formModel.Email,
            LeadSourceTypeId = formModel.LeadSourceTypeId,
            LeadStatusTypeId = formModel.LeadStatusTypeId,
            Priority = formModel.Priority,
            Note = formModel.Note,
            AssignedEmployeeId = model.AssignedEmployeeId
        };

        var (isValid, validationErrors) = _leadService.Validate(tempLead);
        if (!isValid)
        {
            TempData["Warning"] = string.Join(" | ", validationErrors.Values);
            return RedirectToAction(nameof(Index));
        }

        var lead = _leadService.Update(model.Id.Value, tempLead);
        if (lead is null)
        {
            return NotFound();
        }

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }

        TempData["Success"] = "Lead satiri guncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id) => RedirectToAction(nameof(Hub), new { id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddNote(int id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Warning"] = "Not içeriği boş olamaz.";
            return RedirectToAction(nameof(Hub), new { id });
        }

        _leadService.AddNote(id, content);
        TempData["Success"] = "Not eklendi.";
        return RedirectToAction(nameof(Hub), new { id });
    }

    public IActionResult Hub(int id)
    {
        var model = _leadService.GetHubData(id);
        if (model == null)
        {
            TempData["Warning"] = "Lead bulunamadi.";
            return RedirectToAction(nameof(Index));
        }
        BuildShell();
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.StatusTypes = Db.LeadStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.SourceTypes = Db.LeadSourceTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.DuplicateWarnings = Array.Empty<string>();
        return View(new LeadFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(LeadFormViewModel model)
    {
        BuildShell();
        
        var lead = new Lead
        {
            DisplayName = model.DisplayName ?? string.Empty,
            City = model.City ?? string.Empty,
            District = model.District,
            ContactName = model.ContactName ?? string.Empty,
            Phone = model.Phone,
            Email = model.Email,
            LeadSourceTypeId = model.LeadSourceTypeId,
            LeadStatusTypeId = model.LeadStatusTypeId,
            Priority = model.Priority,
            Note = model.Note
        };

        var (isValid, errors) = _leadService.Validate(lead);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.StatusTypes = Db.LeadStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
            ViewBag.SourceTypes = Db.LeadSourceTypes.OrderBy(x => x.DisplayOrder).ToList();
            return View(model);
        }

        _leadService.Create(lead);
        TempData["Success"] = "Lead kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter,Manager,FieldSales")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var lead = Db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null)
        {
            return NotFound();
        }

        ViewBag.StatusTypes = Db.LeadStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
        ViewBag.SourceTypes = Db.LeadSourceTypes.OrderBy(x => x.DisplayOrder).ToList();

        var model = new LeadFormViewModel
        {
            Id = lead.Id,
            DisplayName = lead.DisplayName,
            City = lead.City,
            District = lead.District,
            ContactName = lead.ContactName,
            Phone = lead.Phone,
            Email = lead.Email,
            LeadSourceTypeId = lead.LeadSourceTypeId,
            LeadStatusTypeId = lead.LeadStatusTypeId,
            Priority = lead.Priority,
            Note = lead.Note
        };
        ViewBag.DuplicateWarnings = BuildDuplicateWarnings(model, id);
        return View(model);
    }

    [Authorize(Roles = "Admin,SalesManager,CallCenter,Manager,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, LeadFormViewModel model)
    {
        BuildShell();
        
        var tempLead = new Lead
        {
            Id = id,
            DisplayName = model.DisplayName ?? string.Empty,
            City = model.City ?? string.Empty,
            District = model.District,
            ContactName = model.ContactName ?? string.Empty,
            Phone = model.Phone,
            Email = model.Email,
            LeadSourceTypeId = model.LeadSourceTypeId,
            LeadStatusTypeId = model.LeadStatusTypeId,
            Priority = model.Priority,
            Note = model.Note
        };

        var (isValid, errors) = _leadService.Validate(tempLead);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.StatusTypes = Db.LeadStatusTypes.OrderBy(x => x.DisplayOrder).ToList();
            ViewBag.SourceTypes = Db.LeadSourceTypes.OrderBy(x => x.DisplayOrder).ToList();
            return View(model);
        }

        var existing = _leadService.GetById(id, CurrentEmployeeScopeId());
        if (existing == null) return NotFound();
        
        var updated = _leadService.Update(id, tempLead);
        if (updated == null)
        {
            return NotFound();
        }

        TempData["Success"] = "Lead kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin,SalesManager,Manager")]
    [HttpGet]
    public IActionResult Assignments()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.AssignedLeads = Db.Leads
            .Include(x => x.AssignedEmployee)
            .Include(x => x.LeadStatusType)
            .Where(x => x.AssignedEmployeeId != null && x.LeadStatusTypeId != 5 && x.LeadStatusTypeId != 9)
            .OrderBy(x => x.ScheduledVisitDate ?? DateTime.MaxValue)
            .ThenBy(x => x.DisplayName)
            .ToList();
        var leads = Db.Leads
            .Include(x => x.LeadSourceType)
            .Include(x => x.LeadStatusType)
            .Where(x => x.AssignedEmployeeId == null && (x.LeadStatusTypeId == 1 || x.LeadStatusTypeId == 2))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        // Sort: ReadyForAssignment (Id:2) > New (Id:1)
        return View(leads.OrderByDescending(x => x.LeadStatusTypeId).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(int id, string newStatus, string? note)
    {
        var result = _leadService.ChangeStatus(id, newStatus, note);
        if (!result)
        {
            TempData["Warning"] = "Durum degisikligi yapilamadi. Gecis gecersiz olabilir.";
        }
        else
        {
            TempData["Success"] = "Lead durumu guncellendi.";
        }
        return RedirectToAction(nameof(Hub), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PlanVisit(int leadId, DateTime plannedDate, string plannedTime, int durationMinutes, string? note)
    {
        var lead = _leadService.GetById(leadId);
        if (lead == null) return NotFound();

        if (!lead.AssignedEmployeeId.HasValue) 
        {
            TempData["Warning"] = "Ziyaret planlamak için önce personel atanmalıdır.";
            return RedirectToAction(nameof(Hub), new { id = leadId });
        }

        var timeParts = plannedTime.Split(':');
        var plannedAt = plannedDate.Date.AddHours(int.Parse(timeParts[0])).AddMinutes(int.Parse(timeParts[1]));

        var activity = new Activity
        {
            Code = $"ACT-{DateTime.Now.Ticks.ToString().Substring(8)}",
            LeadId = lead.Id,
            AccountId = lead.ConvertedAccountId,
            EmployeeId = lead.AssignedEmployeeId.Value,
            ActivityDate = DateTime.Now,
            PlannedAt = plannedAt,
            DurationMinutes = durationMinutes,
            Summary = note ?? "Hub üzerinden ziyaret planlandı.",
            ContactStatusTypeId = 3, // PLANNED
            ContactName = lead.ContactName ?? string.Empty
        };

        Db.Activities.Add(activity);
        Db.SaveChanges();
        
        _leadService.ChangeStatus(leadId, "VISIT_SCHEDULED", $"Ziyaret Planlandı: {plannedAt:g}");

        TempData["Success"] = "Ziyaret başarıyla planlandı.";
        return RedirectToAction(nameof(Hub), new { id = leadId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompleteVisit(int leadId, int contactStatusTypeId, int? outcomeStatusTypeId, string note)
    {
        var lead = Db.Leads.FirstOrDefault(l => l.Id == leadId);
        if (lead == null) return NotFound();

        // Find the most recent planned visit
        var activity = Db.Activities.OrderByDescending(x => x.PlannedAt ?? x.ActivityDate).FirstOrDefault(a => a.LeadId == leadId && a.ContactStatusTypeId == 3);
        if (activity != null)
        {
            activity.ActivityDate = DateTime.Now;
            activity.ContactStatusTypeId = contactStatusTypeId;
            if (contactStatusTypeId == 1 && outcomeStatusTypeId.HasValue) // CONTACTED = 1
            {
                activity.OutcomeStatusTypeId = outcomeStatusTypeId;
            }
            else
            {
                activity.OutcomeStatusTypeId = null;
            }
            activity.Summary = note;
            Db.SaveChanges();
        }
        else
        {
             // Fallback if no planned activity exists for some reason
             activity = new Activity
             {
                 Code = $"ACT-{DateTime.Now.Ticks.ToString().Substring(8)}",
                 LeadId = lead.Id,
                 AccountId = lead.ConvertedAccountId,
                 EmployeeId = (lead.AssignedEmployeeId ?? CurrentEmployeeScopeId()) ?? 0,
                 ActivityDate = DateTime.Now,
                 ContactStatusTypeId = contactStatusTypeId,
                 OutcomeStatusTypeId = (contactStatusTypeId == 1) ? outcomeStatusTypeId : null,
                 Summary = note,
                 ContactName = lead.ContactName ?? string.Empty
             };
             Db.Activities.Add(activity);
             Db.SaveChanges();
        }

        _leadService.ChangeStatus(leadId, "VISITED", "Saha Ziyareti tamamlandı.");
        TempData["Success"] = "Görüşme kaydedildi, Lead durumu güncellendi.";
        return RedirectToAction(nameof(Hub), new { id = leadId });
    }

    [HttpGet]
    public IActionResult CheckVisitConflict(int leadId, string datetimeStr)
    {
        if (!DateTime.TryParse(datetimeStr, out var datetime)) return Json(new { hasConflict = false });

        var lead = _leadService.GetById(leadId);
        if (lead == null || !lead.AssignedEmployeeId.HasValue) return Json(new { hasConflict = false });

        var employeeId = lead.AssignedEmployeeId.Value;
        var startRange = datetime.AddMinutes(-30);
        var endRange = datetime.AddMinutes(30);

        var conflict = Db.Activities
            .Include(a => a.Lead)
            .Include(a => a.Account)
            .FirstOrDefault(a => 
                a.EmployeeId == employeeId && 
                a.ContactStatusTypeId == 3 && // PLANNED
                a.PlannedAt != null && 
                a.PlannedAt >= startRange && 
                a.PlannedAt <= endRange);

        if (conflict != null)
        {
            var targetName = conflict.Lead != null ? conflict.Lead.DisplayName : (conflict.Account != null ? conflict.Account.DisplayName : "Bilinmeyen Firma");
            return Json(new { 
                hasConflict = true, 
                message = $"Bu personelin {conflict.PlannedAt:g} saatinde zaten planlanmış bir ziyareti var ({targetName}). Yine de planlamak istiyor musunuz?" 
            });
        }

        return Json(new { hasConflict = false });
    }

    [Authorize(Roles = "Admin,SalesManager,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Assign(int leadId, int employeeId, DateTime? scheduledVisitDate, LeadPriority priority, string note, string? returnUrl = null)
    {
        var lead = _leadService.Assign(leadId, employeeId, User.GetUserId(), note);
        if (lead == null)
        {
            return NotFound();
        }

        lead.Priority = priority;

        // Service set standard logic, but if specialized date was provided:
        if (scheduledVisitDate.HasValue)
        {
            lead.ScheduledVisitDate = scheduledVisitDate;
            Db.SaveChanges(); // Direct DB usage as plan allows for minor things, or I could update service
        }

        TempData["Success"] = "Lead atamasi kaydedildi.";
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        return RedirectToAction(nameof(Hub), new { id = leadId });
    }

    [Authorize(Roles = "FieldSales")]
    public IActionResult MyAssigned()
    {
        BuildShell();
        var employeeId = User.GetEmployeeId();
        var leads = Db.Leads
            .Include(x => x.LeadStatusType)
            .Include(x => x.LeadSourceType)
            .Where(x => x.AssignedEmployeeId == employeeId && (x.LeadStatusTypeId == 3 || x.LeadStatusTypeId == 4))
            .ToList();
        return View(leads);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var success = _leadService.Delete(id);
        if (!success)
        {
            return NotFound();
        }

        TempData["Success"] = "Lead kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,SalesManager,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartVisit(int id)
    {
        var filterId = CurrentEmployeeScopeId();
        var lead = _leadService.GetById(id, filterId);
        if (lead == null)
        {
            return NotFound();
        }

        // If FieldSales, must be assigned to them
        if (User.GetRoleType() == RoleType.FieldSales && lead.AssignedEmployeeId != User.GetEmployeeId())
        {
            return Forbid();
        }

        var leadResult = _leadService.StartVisit(id, User.GetEmployeeId() ?? 0);
        if (leadResult == null)
        {
            return NotFound();
        }

        TempData["Success"] = "Lead ziyareti baslatildi ve donusturuldu.";
        return RedirectToAction(nameof(Hub), new { id = id });
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
            LeadSourceTypeId = model.LeadSourceTypeId,
            LeadStatusTypeId = model.LeadStatusTypeId,
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
