using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class LeadService : ILeadService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountService _accountService;
    private readonly IActivityService _activityService;

    public LeadService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor, 
        IAccountService accountService, IActivityService activityService)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
        _accountService = accountService;
        _activityService = activityService;
    }

    public List<Lead> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? statusId = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false)
    {
        var query = _db.Leads
            .Include(x => x.LeadStatusType)
            .Include(x => x.LeadSourceType)
            .Include(x => x.AssignedEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.DisplayName.Contains(searchTerm) || x.Code.Contains(searchTerm) || x.Phone.Contains(searchTerm));
        }

        if (statusId.HasValue)
        {
            query = query.Where(x => x.LeadStatusTypeId == statusId.Value);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.AssignedEmployeeId == employeeId.Value);
        }

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.AssignedEmployeeId == filterEmployeeId.Value);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                "displayname" => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
                "city" => isDescending ? query.OrderByDescending(x => x.City) : query.OrderBy(x => x.City),
                "source" => isDescending ? query.OrderByDescending(x => x.LeadSourceType.Name) : query.OrderBy(x => x.LeadSourceType.Name),
                "status" => isDescending ? query.OrderByDescending(x => x.LeadStatusType.Name) : query.OrderBy(x => x.LeadStatusType.Name),
                "priority" => isDescending ? query.OrderByDescending(x => x.Priority) : query.OrderBy(x => x.Priority),
                "employee" => isDescending ? query.OrderByDescending(x => x.AssignedEmployee != null ? x.AssignedEmployee.FullName : "") : query.OrderBy(x => x.AssignedEmployee != null ? x.AssignedEmployee.FullName : ""),
                "date" => isDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }

        totalCount = query.Count();
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Lead? GetById(int id, int? filterEmployeeId = null)
    {
        var query = _db.Leads
            .Include(x => x.LeadStatusType)
            .Include(x => x.LeadSourceType)
            .Include(x => x.AssignedEmployee)
            .Include(x => x.Assignments)
                .ThenInclude(a => a.AssignedEmployee)
            .Include(x => x.Notes.OrderByDescending(n => n.CreatedAt))
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.AssignedEmployeeId == filterEmployeeId.Value);
        }

        return query.FirstOrDefault(x => x.Id == id);
    }

    public Lead Create(Lead lead)
    {
        var id = (_db.Leads.IgnoreQueryFilters().Max(x => (int?)x.Id) ?? 100) + 1;
        lead.Id = id;
        lead.Code = $"LD-{id}";

        if (lead.AssignedEmployeeId.HasValue && (lead.LeadStatusTypeId == 1 || lead.LeadStatusTypeId == 2))
        {
            lead.LeadStatusTypeId = 3; // Assigned
        }

        _db.Leads.Add(lead);

        if (!string.IsNullOrWhiteSpace(lead.Note))
        {
            var note = new LeadNote
            {
                LeadId = lead.Id,
                Content = lead.Note,
                CreatedBy = lead.CreatedBy ?? "Sistem (Oluşturan)",
                CreatedAt = DateTime.UtcNow
            };
            _db.LeadNotes.Add(note);
            lead.Note = string.Empty; // Move to structured notes
        }

        _auditService.Log("Lead", "Create", lead.Code, $"{lead.DisplayName} lead kaydi olusturuldu.");
        _db.SaveChanges();
        return lead;
    }

    public Lead? Update(int id, Lead updated)
    {
        var lead = _db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null) return null;

        lead.DisplayName = updated.DisplayName;
        lead.City = updated.City;
        lead.District = updated.District;
        lead.ContactName = updated.ContactName;
        lead.Phone = updated.Phone;
        lead.Email = updated.Email;
        lead.LeadSourceTypeId = updated.LeadSourceTypeId;
        lead.LeadStatusTypeId = updated.LeadStatusTypeId;
        lead.Priority = updated.Priority;
        lead.AssignedEmployeeId = updated.AssignedEmployeeId;
        
        // Handle Note migration: If a note is provided during Edit, move it to the notes collection with a timestamp.
        if (!string.IsNullOrWhiteSpace(updated.Note))
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistem";
            var note = new LeadNote
            {
                LeadId = lead.Id,
                Content = updated.Note,
                CreatedBy = userName + " (Düzenleme)"
            };
            _db.LeadNotes.Add(note);
            lead.Note = string.Empty; // Clear the legacy field once moved
        }

        if (lead.AssignedEmployeeId.HasValue && (lead.LeadStatusTypeId == 1 || lead.LeadStatusTypeId == 2))
        {
            lead.LeadStatusTypeId = 3;
        }

        _auditService.Log("Lead", "Update", lead.Code, $"{lead.DisplayName} lead kaydi guncellendi.");
        _db.SaveChanges();
        return lead;
    }

    public bool Delete(int id)
    {
        var lead = _db.Leads.FirstOrDefault(x => x.Id == id);
        if (lead is null) return false;

        lead.DeletedAt = DateTime.UtcNow;
        lead.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        _auditService.Log("Lead", "Delete", lead.Code, $"{lead.DisplayName} lead kaydi silindi (soft delete).");
        _db.SaveChanges();
        return true;
    }

    public Lead? Assign(int leadId, int employeeId, int assignedByUserId, string? note)
    {
        var lead = _db.Leads.FirstOrDefault(x => x.Id == leadId);
        if (lead is null) return null;

        var employee = _db.Employees.FirstOrDefault(x => x.Id == employeeId);
        if (employee is null) return null;

        var assignment = new LeadAssignment
        {
            LeadId = lead.Id,
            AssignedEmployeeId = employee.Id,
            AssignedByUserId = assignedByUserId,
            AssignedAt = DateTime.UtcNow, // DB Context normalizes
            AssignmentNote = note,
            Priority = lead.Priority
        };
        _db.LeadAssignments.Add(assignment);

        lead.AssignedEmployeeId = employee.Id;
        lead.LeadStatusTypeId = (int)LeadStatus.Assigned; // Assigned
        _auditService.Log("Lead", "Assign", lead.Code, $"{lead.DisplayName} leadi {employee.FullName} personeline atandi.");
        _db.SaveChanges();
        return lead;
    }

    public Lead? StartVisit(int leadId, int employeeId)
    {
        var lead = _db.Leads.FirstOrDefault(x => x.Id == leadId);
        if (lead is null) return null;

        var derivedEmployeeId = lead.AssignedEmployeeId ?? employeeId;

        // Use AccountService
        var account = new Account
        {
            AccountType = AccountType.Corporate,
            DisplayName = lead.DisplayName,
            City = lead.City,
            District = lead.District,
            Phone = lead.Phone,
            Email = lead.Email,
            OwnerEmployeeId = derivedEmployeeId,
            Notes = $"Lead donusumu: {lead.Code}",
            Status = "Active"
        };
        _accountService.Create(account);

        // Use ActivityService
        var activity = new Activity
        {
            ActivityDate = DateTime.UtcNow,
            EmployeeId = derivedEmployeeId,
            AccountId = account.Id,
            LeadId = lead.Id,
            ContactStatusTypeId = 1, // CONTACTED
            OutcomeStatusTypeId = 1, // NOT_APPLICABLE
            ContactName = lead.ContactName,
            Summary = $"Lead ziyareti baslatildi: {lead.DisplayName}"
        };
        _activityService.Create(activity);

        lead.LeadStatusTypeId = (int)LeadStatus.Converted; // ConvertedToActivity
        lead.AssignedEmployeeId = derivedEmployeeId; // <--- FIX: Ensure lead is marked as assigned
        lead.ConvertedAccountId = account.Id;
        lead.ConvertedActivityId = activity.Id;
        
        _auditService.Log("Lead", "Convert", lead.Code, $"{lead.DisplayName} leadi {account.Code} ve {activity.Code} kayitlarina donusturuldu.");

        _db.SaveChanges();
        return lead;
    }

    public List<Lead> GetAssignments(int? employeeId, string? status)
    {
        var query = _db.Leads
            .Include(x => x.LeadStatusType)
            .Include(x => x.LeadSourceType)
            .Include(x => x.AssignedEmployee)
            .AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(x => x.AssignedEmployeeId == employeeId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            // Simple mapping for this logic scope, it was handled manually in controller
            if (status == "New") query = query.Where(x => x.LeadStatusTypeId == 1);
            else if (status == "Assigned") query = query.Where(x => x.LeadStatusTypeId == 2 || x.LeadStatusTypeId == 3);
            else if (status == "Completed") query = query.Where(x => x.LeadStatusTypeId >= 4);
        }

        return query.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Lead lead)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(lead.DisplayName))
            errors.Add(nameof(lead.DisplayName), "Lead adi/unvani bos birakilamaz.");

        if (string.IsNullOrWhiteSpace(lead.City))
            errors.Add(nameof(lead.City), "Sehir alani bos birakilamaz.");

        if (string.IsNullOrWhiteSpace(lead.Phone) && string.IsNullOrWhiteSpace(lead.Email))
            errors.Add(string.Empty, "Telefon veya e-posta alanlarindan en az biri dolu olmalidir.");

        if (lead.LeadStatusTypeId <= 0)
            errors.Add(nameof(lead.LeadStatusTypeId), "Durum secimi zorunludur.");
            
        if (lead.LeadSourceTypeId <= 0)
            errors.Add(nameof(lead.LeadSourceTypeId), "Kaynak secimi zorunludur.");

        return (errors.Count == 0, errors);
    }

    public bool CheckDuplicate(string displayName, string? phone, int? excludeId = null)
    {
        var leads = _db.Leads.Where(x => !excludeId.HasValue || x.Id != excludeId.Value);

        if (!string.IsNullOrWhiteSpace(displayName) && leads.Any(x => x.DisplayName == displayName))
            return true;

        if (!string.IsNullOrWhiteSpace(phone))
        {
            if (leads.Any(x => x.Phone == phone)) return true;
            if (_db.Accounts.Any(x => x.Phone == phone)) return true;
        }

        return false;
    }

    public void AddNote(int leadId, string content, string? author = null)
    {
        var lead = _db.Leads.FirstOrDefault(x => x.Id == leadId);
        if (lead == null) return;

        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        var note = new LeadNote
        {
            LeadId = leadId,
            Content = content,
            CreatedBy = author ?? user
        };

        _db.LeadNotes.Add(note);
        _db.SaveChanges();
        
        _auditService.Log("Lead", "NoteAdded", lead.Code, $"{lead.DisplayName} leadine yeni not eklendi.");
    }

    public LeadHubViewModel? GetHubData(int leadId)
    {
        var lead = _db.Leads
            .Include(x => x.LeadStatusType)
            .Include(x => x.LeadSourceType)
            .Include(x => x.AssignedEmployee)
            .Include(x => x.Assignments)
                .ThenInclude(a => a.AssignedEmployee)
            .Include(x => x.Assignments)
                .ThenInclude(a => a.AssignedByUser)
            .Include(x => x.Notes)
            .FirstOrDefault(x => x.Id == leadId);

        if (lead == null) return null;

        var model = new LeadHubViewModel
        {
            Id = lead.Id,
            Code = lead.Code,
            DisplayName = lead.DisplayName,
            Phone = lead.Phone,
            Email = lead.Email,
            City = lead.City,
            District = lead.District,
            Note = lead.Note,
            StatusName = lead.LeadStatusType.Name,
            StatusCode = lead.LeadStatusType.Code,
            SourceName = lead.LeadSourceType.Name,
            CreatedAt = lead.CreatedAt,
            CreatedBy = lead.CreatedBy,
            ContactName = lead.ContactName,
            Priority = lead.Priority.ToString(),
            ConvertedActivityId = lead.ConvertedActivityId,
            Notes = lead.Notes.OrderByDescending(x => x.CreatedAt).Select(n => new LeadNoteInfo
            {
                Id = n.Id,
                Content = n.Content,
                CreatedBy = n.CreatedBy,
                CreatedAt = n.CreatedAt,
                IsSystemNote = n.CreatedBy.StartsWith("Sistem")
            }).ToList(),
            AvailableEmployees = _db.Employees.Where(x => x.IsActive).OrderBy(x => x.FullName).ToList()
        };

        // Assignments
        if (lead.Assignments.Any())
        {
            var ordered = lead.Assignments.OrderByDescending(x => x.AssignedAt).ToList();
            var latest = ordered.First();
            model.CurrentAssignment = new LeadAssignmentInfo
            {
                EmployeeId = latest.AssignedEmployeeId,
                EmployeeName = latest.AssignedEmployee?.FullName ?? "Bilinmiyor",
                AssignedByName = latest.AssignedByUser?.FullName ?? "Sistem",
                AssignedAt = latest.AssignedAt,
                Priority = latest.Priority.ToString(),
                DueDate = latest.DueDate,
                Note = latest.AssignmentNote,
                IsActive = latest.IsActive
            };

            model.AssignmentHistory = ordered.Select(a => new LeadAssignmentInfo
            {
                EmployeeId = a.AssignedEmployeeId,
                EmployeeName = a.AssignedEmployee?.FullName ?? "Bilinmiyor",
                AssignedByName = a.AssignedByUser?.FullName ?? "Sistem",
                AssignedAt = a.AssignedAt,
                Priority = a.Priority.ToString(),
                DueDate = a.DueDate,
                Note = a.AssignmentNote,
                IsActive = a.IsActive
            }).ToList();
        }
        else if (lead.AssignedEmployeeId.HasValue)
        {
            model.CurrentAssignment = new LeadAssignmentInfo
            {
                EmployeeName = lead.AssignedEmployee?.FullName ?? "Bilinmiyor",
                AssignedByName = "Sistem (Başlangıç Ataması)",
                AssignedAt = lead.CreatedAt,
                Priority = lead.Priority.ToString(),
                DueDate = lead.ScheduledVisitDate,
                Note = "Sistem tarafından atandı.",
                IsActive = true
            };
        }

        // Linked Account
        if (lead.ConvertedAccountId.HasValue)
        {
            var account = _db.Accounts.Find(lead.ConvertedAccountId.Value);
            if (account != null)
            {
                model.LinkedAccount = new LinkedAccountInfo
                {
                    Id = account.Id,
                    Name = account.DisplayName,
                    AccountType = account.AccountType.ToString(),
                    City = account.City,
                    Phone = account.Phone
                };
            }
        }

        // Activities (both from lead directly or from account)
        var accountId = lead.ConvertedAccountId;
        model.Activities = _db.Activities
            .Include(a => a.Employee)
            .Include(a => a.ContactStatusType)
            .Include(a => a.OutcomeStatusType)
            .Where(a => a.LeadId == lead.Id || (accountId.HasValue && a.AccountId == accountId.Value))
            .OrderByDescending(a => a.ActivityDate)
            .Select(a => new LeadActivityInfo
            {
                Id = a.Id,
                ActivityDate = a.ActivityDate,
                EmployeeName = a.Employee != null ? a.Employee.FullName : "Bilinmiyor",
                ContactStatus = a.ContactStatusType != null ? a.ContactStatusType.Name : "Bilinmiyor",
                ContactStatusCode = a.ContactStatusType != null ? a.ContactStatusType.Code : "",
                OutcomeStatus = a.OutcomeStatusType != null ? a.OutcomeStatusType.Name : "-",
                Summary = a.Summary
            })
            .ToList();

        // Sales (from linked account or direct lead activities)
        if (accountId.HasValue)
        {
            model.Sales = _db.Sales
                .Include(s => s.Employee)
                .Include(s => s.InsuranceProductType)
                .Where(s => s.AccountId == accountId.Value)
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new LeadSaleInfo
                {
                    Id = s.Id,
                    SaleDate = s.SaleDate,
                    ProductType = s.InsuranceProductType != null ? s.InsuranceProductType.Name : "Bilinmiyor",
                    EmployeeName = s.Employee != null ? s.Employee.FullName : "Bilinmiyor",
                    Amount = s.SaleAmount ?? s.CollectionAmount ?? 0m
                })
                .ToList();
        }

        model.AvailableActions = GetValidTransitions(lead.LeadStatusType.Code);
        
        // Populate upcoming planned visits for the assigned employee
        if (lead.AssignedEmployeeId.HasValue)
        {
            var empId = lead.AssignedEmployeeId.Value;
            var now = DateTime.Now;
            model.EmployeeUpcomingVisits = _db.Activities
                .Where(a => a.EmployeeId == empId && a.PlannedAt != null && a.PlannedAt >= now)
                .OrderBy(a => a.PlannedAt)
                .Take(5)
                .Select(a => new EmployeePlannedVisitInfo
                {
                    PlannedDate = a.PlannedAt.Value,
                    LeadName = a.Lead != null ? a.Lead.DisplayName : (a.Account != null ? a.Account.DisplayName : "Bilinmeyen"),
                    DurationMinutes = a.DurationMinutes
                })
                .ToList();
        }

        return model;
    }

    public bool ChangeStatus(int leadId, string newStatusCode, string? note = null)
    {
        var lead = _db.Leads.Include(l => l.LeadStatusType).FirstOrDefault(l => l.Id == leadId);
        if (lead == null) return false;

        var newStatus = _db.LeadStatusTypes.FirstOrDefault(s => s.Code == newStatusCode);
        if (newStatus == null) return false;

        // Validation of transition
        var validTransitions = GetValidTransitions(lead.LeadStatusType?.Code ?? "NEW");
        if (!validTransitions.Contains(newStatusCode) && newStatusCode != "DISQUALIFIED" && newStatusCode != "VISITED")
            return false;

        lead.LeadStatusTypeId = newStatus.Id;
        
        if (!string.IsNullOrWhiteSpace(note))
        {
            AddNote(leadId, note, "Sistem (Durum Degisikligi)");
        }

        _auditService.Log("Lead", "StatusUpdate", lead.Code, $"{lead.DisplayName} durumu {newStatusCode} olarak guncellendi.");
        _db.SaveChanges();
        return true;
    }

    private List<string> GetValidTransitions(string currentStatus)
    {
        return currentStatus switch
        {
            "NEW" => new() { "RESEARCHED", "DISQUALIFIED" },
            "RESEARCHED" => new() { "CONTACT_FOUND", "DISQUALIFIED" },
            "CONTACT_FOUND" => new() { "READY_FOR_ASSIGNMENT", "DISQUALIFIED" },
            "READY_FOR_ASSIGNMENT" => new() { "ASSIGNED", "DISQUALIFIED" },
            "ASSIGNED" => new() { "VISIT_SCHEDULED", "DISQUALIFIED" },
            "VISIT_SCHEDULED" => new() { "DISQUALIFIED" },
            "VISITED" => new() { "CONVERTED_TO_ACTIVITY", "DISQUALIFIED" },
            "CONVERTED_TO_ACTIVITY" => new(),
            "DISQUALIFIED" => new(),
            _ => new()
        };
    }
}
