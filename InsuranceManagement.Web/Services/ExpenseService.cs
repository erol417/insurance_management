using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExpenseService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Expense> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false, DateTime? startDate = null, DateTime? endDate = null, int? expenseTypeId = null)
    {
        var query = _db.Expenses
            .Include(x => x.Employee)
            .Include(x => x.ExpenseTypeEntity)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var st = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Code.ToLower().Contains(st) || (x.Notes != null && x.Notes.ToLower().Contains(st)));
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
            query = query.Where(x => x.ExpenseDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.ExpenseDate <= endDate.Value);
        }

        if (expenseTypeId.HasValue)
        {
            query = query.Where(x => x.ExpenseTypeId == expenseTypeId.Value);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                "date" => isDescending ? query.OrderByDescending(x => x.ExpenseDate) : query.OrderBy(x => x.ExpenseDate),
                "employee" => isDescending ? query.OrderByDescending(x => x.Employee.FullName) : query.OrderBy(x => x.Employee.FullName),
                "type" => isDescending ? query.OrderByDescending(x => x.ExpenseTypeEntity.Name) : query.OrderBy(x => x.ExpenseTypeEntity.Name),
                "amount" => isDescending ? query.OrderByDescending(x => x.Amount) : query.OrderBy(x => x.Amount),
                _ => query.OrderByDescending(x => x.ExpenseDate)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.ExpenseDate);
        }

        totalCount = query.Count();
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Expense? GetById(int id, int? filterEmployeeId = null)
    {
        var query = _db.Expenses
            .Include(x => x.Employee)
            .Include(x => x.ExpenseTypeEntity)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        return query.FirstOrDefault(x => x.Id == id);
    }

    public Expense Create(Expense expense)
    {
        var id = (_db.Expenses.IgnoreQueryFilters().Max(x => (int?)x.Id) ?? 100) + 1;
        expense.Id = id;
        expense.Code = $"EXP-{id}";

        _db.Expenses.Add(expense);
        _auditService.Log("Expense", "Create", expense.Code, $"Harcama olusturuldu. Tutar: {expense.Amount}.");
        _db.SaveChanges();
        return expense;
    }

    public Expense? Update(int id, Expense updated)
    {
        var expense = _db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null) return null;

        expense.ExpenseDate = updated.ExpenseDate;
        expense.EmployeeId = updated.EmployeeId;
        expense.ExpenseTypeId = updated.ExpenseTypeId;
        expense.Amount = updated.Amount;
        expense.Notes = updated.Notes;

        _auditService.Log("Expense", "Update", expense.Code, "Harcama guncellendi.");
        _db.SaveChanges();
        return expense;
    }

    public bool Delete(int id)
    {
        var expense = _db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null) return false;

        expense.DeletedAt = DateTime.UtcNow;
        expense.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        _auditService.Log("Expense", "Delete", expense.Code, "Harcama silindi (soft delete).");
        _db.SaveChanges();
        return true;
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Expense expense)
    {
        var errors = new Dictionary<string, string>();

        if (expense.Amount <= 0)
            errors.Add(nameof(expense.Amount), "Harcama tutari 0'dan buyuk olmalidir.");
        
        if (expense.EmployeeId <= 0)
            errors.Add(nameof(expense.EmployeeId), "Personel secimi zorunludur.");
        
        if (expense.ExpenseTypeId <= 0)
            errors.Add(nameof(expense.ExpenseTypeId), "Harcama tipi secimi zorunludur.");

        return (errors.Count == 0, errors);
    }
}
