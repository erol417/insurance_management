using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class ExpensesController : AppController
{
    private readonly InsuranceManagement.Web.Services.IExpenseService _expenseService;

    public ExpensesController(AppDbContext db, InsuranceManagement.Web.Services.IExpenseService expenseService) : base(db)
    {
        _expenseService = expenseService;
    }

    public IActionResult Index(string? searchTerm = null, int? employeeId = null, string? sortBy = "date", bool isDescending = true, int page = 1, int pageSize = 10, DateTime? start = null, DateTime? end = null, int? expenseTypeId = null)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        int? filterEmployeeId = CurrentEmployeeScopeId();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.EmployeeId = employeeId;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.ExpenseTypes = Db.ExpenseTypes.OrderBy(x => x.DisplayOrder).ToList();

        var items = _expenseService.GetAll(page, pageSize, out var totalCount, searchTerm, employeeId, filterEmployeeId, sortBy, isDescending, start, end, expenseTypeId);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = page;

        return View(new ExpensesIndexViewModel
        {
            NewExpense = new ExpenseInlineEditViewModel
            {
                ExpenseDate = DateTime.Today,
                EmployeeId = currentEmployeeId ?? Db.Employees.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault(),
                ExpenseTypeId = 1 // Default: Travel
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = items.Select(x => new ExpenseInlineEditViewModel
            {
                Id = x.Id,
                Code = x.Code,
                ExpenseDate = x.ExpenseDate,
                EmployeeId = x.EmployeeId,
                ExpenseTypeId = x.ExpenseTypeId,
                Amount = x.Amount,
                Notes = x.Notes
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(ExpenseGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        List<ExpenseInlineEditViewModel>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<ExpenseInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
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
        var nextId = (Db.Expenses.Max(x => (int?)x.Id) ?? 100) + 1;
        var currentEmployeeId = CurrentEmployeeScopeId();
        var canManageAll = HasGlobalEmployeeAccess();

        foreach (var item in payload)
        {
            var formModel = ToExpenseFormModel(item, currentEmployeeId, canManageAll);
            var validationErrors = ValidateInlineExpense(formModel);
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(x => $"{(string.IsNullOrWhiteSpace(item.Code) ? "Yeni Satir" : item.Code)}: {x}"));
                continue;
            }

            if (item.Id.HasValue)
            {
                var expense = Db.Expenses.FirstOrDefault(x => x.Id == item.Id.Value);
                if (expense is null)
                {
                    errors.Add($"#{item.Id.Value} id'li masraf bulunamadi.");
                    continue;
                }

                if (!CanSeeEmployeeData(expense.EmployeeId) || (!canManageAll && formModel.EmployeeId != currentEmployeeId))
                {
                    errors.Add($"{expense.Code}: Bu kaydi guncelleme yetkin yok.");
                    continue;
                }

                expense.ExpenseDate = formModel.ExpenseDate;
                expense.EmployeeId = formModel.EmployeeId;
                expense.ExpenseTypeId = formModel.ExpenseTypeId;
                expense.Amount = formModel.Amount;
                expense.Notes = formModel.Notes;
                QueueAudit("Expense", "Update", expense.Code, $"{expense.Code} masraf kaydi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                if (!CanSeeEmployeeData(formModel.EmployeeId))
                {
                    errors.Add($"{formModel.EmployeeId} id'li personel adina kayit yetkiniz yok.");
                    continue;
                }

                var expense = new Expense
                {
                    Id = nextId,
                    Code = $"EXP-{nextId}",
                    ExpenseDate = formModel.ExpenseDate,
                    EmployeeId = formModel.EmployeeId,
                    ExpenseTypeId = formModel.ExpenseTypeId,
                    Amount = formModel.Amount,
                    Notes = formModel.Notes
                };
                Db.Expenses.Add(expense);
                QueueAudit("Expense", "Create", expense.Code, $"{expense.Code} masraf kaydi toplu grid kaydi ile olusturuldu.");
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
        MarkGridDraftCleared("expense-grid-draft-v1");
        TempData["Flash"] = $"{createdCount} yeni, {updatedCount} mevcut masraf toplu kaydedildi.";
        return RedirectToAction(nameof(Index), new { page = model.Page });
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var expense = _expenseService.GetById(id, CurrentEmployeeScopeId());
        if (expense is null || !CanSeeEmployeeData(expense.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = expense.Employee;
        return View(expense);
    }

    [Authorize(Roles = "Admin,Manager,Operations,FieldSales")]
    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).ToList();

        ViewBag.ExpenseTypes = Db.ExpenseTypes.OrderBy(x => x.DisplayOrder).ToList();
        return View(new ExpenseFormViewModel { EmployeeId = currentEmployeeId ?? 0 });
    }

    [Authorize(Roles = "Admin,Manager,Operations,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ExpenseFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).ToList();

        ViewBag.ExpenseTypes = Db.ExpenseTypes.OrderBy(x => x.DisplayOrder).ToList();

        if (!CanSeeEmployeeData(model.EmployeeId))
        {
            ModelState.AddModelError("EmployeeId", "Sadece kendi adiniza masraf girebilirsiniz.");
        }

        var expense = new Expense
        {
            ExpenseDate = model.ExpenseDate,
            EmployeeId = model.EmployeeId,
            ExpenseTypeId = model.ExpenseTypeId,
            Amount = model.Amount,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _expenseService.Validate(expense);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _expenseService.Create(expense);
        TempData["Success"] = "Masraf kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var expense = Db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(expense.EmployeeId))
        {
            return NotFound();
        }

        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).ToList();

        ViewBag.ExpenseTypes = Db.ExpenseTypes.OrderBy(x => x.DisplayOrder).ToList();
        return View(new ExpenseFormViewModel
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            EmployeeId = expense.EmployeeId,
            ExpenseTypeId = expense.ExpenseTypeId,
            Amount = expense.Amount,
            Notes = expense.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ExpenseFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).ToList();
        ViewBag.ExpenseTypes = Db.ExpenseTypes.OrderBy(x => x.DisplayOrder).ToList();

        var tempExpense = new Expense
        {
            Id = id,
            ExpenseDate = model.ExpenseDate,
            EmployeeId = model.EmployeeId,
            ExpenseTypeId = model.ExpenseTypeId,
            Amount = model.Amount,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _expenseService.Validate(tempExpense);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = _expenseService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null || !CanSeeEmployeeData(existing.EmployeeId))
        {
            return NotFound();
        }

        _expenseService.Update(id, tempExpense);
        TempData["Success"] = "Masraf kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var existing = _expenseService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null)
        {
            return NotFound();
        }
        
        _expenseService.Delete(id);
        TempData["Success"] = "Masraf kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private ExpenseFormViewModel ToExpenseFormModel(ExpenseInlineEditViewModel model, int? currentEmployeeId, bool canManageAll)
    {
        return new ExpenseFormViewModel
        {
            Id = model.Id,
            ExpenseDate = model.ExpenseDate == default ? DateTime.Today : model.ExpenseDate,
            EmployeeId = canManageAll ? model.EmployeeId : currentEmployeeId ?? model.EmployeeId,
            ExpenseTypeId = model.ExpenseTypeId,
            Amount = model.Amount,
            Notes = model.Notes
        };
    }

    private static List<string> ValidateInlineExpense(ExpenseFormViewModel model)
    {
        var errors = new List<string>();

        if (model.ExpenseDate == default)
        {
            errors.Add("Tarih zorunludur.");
        }

        if (model.EmployeeId <= 0)
        {
            errors.Add("Personel secilmelidir.");
        }

        if (model.Amount <= 0)
        {
            errors.Add("Tutar sifirdan buyuk olmalidir.");
        }

        return errors;
    }
}
