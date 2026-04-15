using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Account> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, string? status = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false)
    {
        var query = _db.Accounts
            .Include(x => x.OwnerEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.DisplayName.Contains(searchTerm) || x.Code.Contains(searchTerm) || (x.Phone != null && x.Phone.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.OwnerEmployeeId == employeeId.Value);
        }

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.OwnerEmployeeId == filterEmployeeId.Value);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                "displayname" => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
                "type" => isDescending ? query.OrderByDescending(x => x.AccountType) : query.OrderBy(x => x.AccountType),
                "city" => isDescending ? query.OrderByDescending(x => x.City) : query.OrderBy(x => x.City),
                "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                "employee" => isDescending ? query.OrderByDescending(x => x.OwnerEmployee != null ? x.OwnerEmployee.FullName : "") : query.OrderBy(x => x.OwnerEmployee != null ? x.OwnerEmployee.FullName : ""),
                "date" => isDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
                _ => query.OrderBy(x => x.DisplayName)
            };
        }
        else
        {
            query = query.OrderBy(x => x.DisplayName);
        }

        totalCount = query.Count();
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Account? GetById(int id, int? filterEmployeeId = null)
    {
        var query = _db.Accounts
            .Include(x => x.OwnerEmployee)
            .Include(x => x.Activities)
            .Include(x => x.Sales)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.OwnerEmployeeId == filterEmployeeId.Value);
        }

        return query.FirstOrDefault(x => x.Id == id);
    }

    public Account Create(Account account)
    {
        var id = (_db.Accounts.Max(x => (int?)x.Id) ?? 100) + 1;
        account.Id = id;
        account.Code = $"ACC-{id}";

        _db.Accounts.Add(account);
        _auditService.Log("Account", "Create", account.Code, $"Musteri kaydi olusturuldu: {account.DisplayName}.");
        _db.SaveChanges();
        return account;
    }

    public Account? Update(int id, Account updated)
    {
        var account = _db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null) return null;

        account.DisplayName = updated.DisplayName;
        account.City = updated.City;
        account.District = updated.District;
        account.Phone = updated.Phone;
        account.Email = updated.Email;
        account.TaxNumber = updated.TaxNumber;
        account.AccountType = updated.AccountType;
        account.OwnerEmployeeId = updated.OwnerEmployeeId;
        account.Status = updated.Status;
        account.Notes = updated.Notes;

        _auditService.Log("Account", "Update", account.Code, $"Musteri kaydi guncellendi: {account.DisplayName}.");
        _db.SaveChanges();
        return account;
    }

    public bool Delete(int id)
    {
        var account = _db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null) return false;

        // Account physical delete for now as per entities (didn't make it ISoftDeletable yet)
        _db.Accounts.Remove(account);
        _auditService.Log("Account", "Delete", account.Code, $"Musteri kaydi silindi: {account.DisplayName}.");
        _db.SaveChanges();
        return true;
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Account account)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(account.DisplayName))
            errors.Add(nameof(account.DisplayName), "Musteri adi bos birakilamaz.");
        
        if (string.IsNullOrWhiteSpace(account.City))
            errors.Add(nameof(account.City), "Sehir alani zorunludur.");
        
        if (account.OwnerEmployeeId <= 0)
            errors.Add(nameof(account.OwnerEmployeeId), "Portfoy yoneticisi zorunludur.");

        return (errors.Count == 0, errors);
    }
    public List<string> CheckDuplicate(string displayName, string? phone, string? email, string? taxNumber, int? currentId)
    {
        var warnings = new List<string>();
        var query = _db.Accounts.AsQueryable();

        if (currentId.HasValue)
        {
            query = query.Where(x => x.Id != currentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(displayName) && query.Any(x => x.DisplayName == displayName))
        {
            warnings.Add("Ayni musteri/firma ismiyle baska bir kayit bulundu.");
        }

        if (!string.IsNullOrWhiteSpace(phone) && query.Any(x => x.Phone == phone))
        {
            warnings.Add("Bu telefon numarasi baska bir musteri kaydinda kullaniliyor.");
        }

        if (!string.IsNullOrWhiteSpace(email) && query.Any(x => x.Email == email))
        {
            warnings.Add("Bu e-posta adresi baska bir musteri kaydinda kullaniliyor.");
        }

        if (!string.IsNullOrWhiteSpace(taxNumber) && query.Any(x => x.TaxNumber == taxNumber))
        {
            warnings.Add("Bu vergi numarasi ile mevcut bir musteri kaydi var.");
        }

        return warnings;
    }

    public List<Activity> GetPlannedVisits(int accountId)
    {
        return _db.Activities
            .Include(a => a.Employee)
            .Include(a => a.Lead)
            .Where(a => a.AccountId == accountId && a.ContactStatusTypeId == 3) // PLANNED
            .OrderBy(a => a.PlannedAt ?? a.ActivityDate)
            .ToList();
    }
}
