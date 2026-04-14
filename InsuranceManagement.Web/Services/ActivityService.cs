using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace InsuranceManagement.Web.Services;

public class ActivityService : IActivityService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Activity> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false, DateTime? startDate = null, DateTime? endDate = null, string? status = null)
    {
        var query = _db.Activities
            .Include(x => x.Employee)
            .Include(x => x.Account)
            .Include(x => x.Lead)
            .Include(x => x.ContactStatusType)
            .Include(x => x.OutcomeStatusType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var st = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Summary.ToLower().Contains(st) || x.Code.ToLower().Contains(st) || x.ContactName.ToLower().Contains(st) || (x.Account != null && x.Account.DisplayName.ToLower().Contains(st)));
        }

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(x => x.ActivityDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.ActivityDate <= endDate.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(x => x.ContactStatusType != null && x.ContactStatusType.Code == status);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                "date" => isDescending ? query.OrderByDescending(x => x.ActivityDate) : query.OrderBy(x => x.ActivityDate),
                "employee" => isDescending ? query.OrderByDescending(x => x.Employee.FullName) : query.OrderBy(x => x.Employee.FullName),
                "customer" => isDescending ? query.OrderByDescending(x => x.Account.DisplayName) : query.OrderBy(x => x.Account.DisplayName),
                "contact" => isDescending ? query.OrderByDescending(x => x.ContactName) : query.OrderBy(x => x.ContactName),
                "status" => isDescending ? query.OrderByDescending(x => x.ContactStatusType.Name) : query.OrderBy(x => x.ContactStatusType.Name),
                "outcome" => isDescending ? query.OrderByDescending(x => x.OutcomeStatusType.Name) : query.OrderBy(x => x.OutcomeStatusType.Name),
                _ => query.OrderByDescending(x => x.ActivityDate)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.ActivityDate);
        }

        totalCount = query.Count();
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Activity? GetById(int id, int? filterEmployeeId = null)
    {
        var query = _db.Activities
            .Include(x => x.Employee)
            .Include(x => x.Account)
            .Include(x => x.Lead)
            .Include(x => x.ContactStatusType)
            .Include(x => x.OutcomeStatusType)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        return query.FirstOrDefault(x => x.Id == id);
    }

    public Activity Create(Activity activity)
    {
        var id = (_db.Activities.IgnoreQueryFilters().Max(x => (int?)x.Id) ?? 100) + 1;
        activity.Id = id;
        activity.Code = $"ACT-{id}";

        var contactStatus = _db.ActivityContactStatusTypes.Find(activity.ContactStatusTypeId);
        if (contactStatus?.Code == "NOT_CONTACTED")
        {
            var notApplicable = _db.ActivityOutcomeStatusTypes.FirstOrDefault(x => x.Code == "NOT_APPLICABLE");
            if (notApplicable != null)
            {
                activity.OutcomeStatusTypeId = notApplicable.Id;
            }
        }

        _db.Activities.Add(activity);
        _auditService.Log("Activity", "Create", activity.Code, $"Aktivite olusturuldu. Musteri #{activity.AccountId}, personel #{activity.EmployeeId}.");
        _db.SaveChanges();
        return activity;
    }

    public Activity? Update(int id, Activity updated)
    {
        var activity = _db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null) return null;

        activity.ActivityDate = updated.ActivityDate;
        activity.EmployeeId = updated.EmployeeId;
        activity.AccountId = updated.AccountId;
        activity.LeadId = updated.LeadId;
        activity.ContactName = updated.ContactName;
        activity.ContactStatusTypeId = updated.ContactStatusTypeId;
        activity.OutcomeStatusTypeId = updated.OutcomeStatusTypeId;
        activity.Summary = updated.Summary;

        var contactStatus = _db.ActivityContactStatusTypes.Find(activity.ContactStatusTypeId);
        if (contactStatus?.Code == "NOT_CONTACTED")
        {
            var notApplicable = _db.ActivityOutcomeStatusTypes.FirstOrDefault(x => x.Code == "NOT_APPLICABLE");
            if (notApplicable != null)
            {
                activity.OutcomeStatusTypeId = notApplicable.Id;
            }
        }

        _auditService.Log("Activity", "Update", activity.Code, $"Aktivite guncellendi.");
        _db.SaveChanges();
        return activity;
    }

    public bool Delete(int id)
    {
        var activity = _db.Activities.FirstOrDefault(x => x.Id == id);
        if (activity is null) return false;

        activity.DeletedAt = DateTime.UtcNow;
        activity.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        _auditService.Log("Activity", "Delete", activity.Code, $"Aktivite silindi (soft delete).");
        _db.SaveChanges();
        return true;
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Activity activity)
    {
        var errors = new Dictionary<string, string>();

        if (activity.EmployeeId <= 0)
            errors.Add(nameof(activity.EmployeeId), "Personel secimi zorunludur.");
        
        if (activity.AccountId <= 0)
            errors.Add(nameof(activity.AccountId), "Musteri/Firma secimi zorunludur.");
        
        if (activity.ActivityDate == default)
            errors.Add(nameof(activity.ActivityDate), "Aktivite tarihi zorunludur.");
        
        if (string.IsNullOrWhiteSpace(activity.Summary))
            errors.Add(nameof(activity.Summary), "Gorusme icerigi zorunludur.");
        
        if (activity.ContactStatusTypeId <= 0)
            errors.Add(nameof(activity.ContactStatusTypeId), "Temas durumu secimi zorunludur.");

        var contactStatus = _db.ActivityContactStatusTypes.Find(activity.ContactStatusTypeId);
        var outcomeStatus = activity.OutcomeStatusTypeId.HasValue 
            ? _db.ActivityOutcomeStatusTypes.Find(activity.OutcomeStatusTypeId.Value) 
            : null;

        if (contactStatus?.Code == "CONTACTED" && (!activity.OutcomeStatusTypeId.HasValue || activity.OutcomeStatusTypeId <= 0))
        {
            errors.Add(nameof(activity.OutcomeStatusTypeId), "Gorusuldu durumunda gorusme sonucu secimi zorunludur.");
        }

        // If NOT_CONTACTED, we don't care about outcome value during validation 
        // because we force it to NOT_APPLICABLE in Create/Update methods anyway.
        // This prevents confusing error messages for a disabled field.

        return (errors.Count == 0, errors);
    }
}
