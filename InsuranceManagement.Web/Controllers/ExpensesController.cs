using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

public class ExpensesController : AppController
{
    public ExpensesController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        BuildShell();
        var itemsQuery = Db.Expenses.AsQueryable();
        var currentEmployeeId = CurrentEmployeeScopeId();
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            itemsQuery = itemsQuery.Where(x => x.EmployeeId == currentEmployeeId.Value);
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();

        var totalCount = itemsQuery.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        return View(new ExpensesIndexViewModel
        {
            NewExpense = new ExpenseInlineEditViewModel
            {
                ExpenseDate = DateTime.Today,
                EmployeeId = currentEmployeeId ?? Db.Employees.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault()
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = itemsQuery
                .OrderByDescending(x => x.ExpenseDate)
                .ThenByDescending(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExpenseInlineEditViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    ExpenseDate = x.ExpenseDate,
                    EmployeeId = x.EmployeeId,
                    ExpenseType = x.ExpenseType,
                    Amount = x.Amount,
                    Notes = x.Notes
                })
                .ToList()
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
                expense.ExpenseType = formModel.ExpenseType;
                expense.Amount = formModel.Amount;
                expense.Notes = formModel.Notes;
                QueueAudit("Expense", "Update", expense.Code, $"{expense.Code} masraf kaydi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                var expense = new Expense
                {
                    Id = nextId,
                    Code = $"EXP-{nextId}",
                    ExpenseDate = formModel.ExpenseDate,
                    EmployeeId = formModel.EmployeeId,
                    ExpenseType = formModel.ExpenseType,
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
        var expense = Db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null || !CanSeeEmployeeData(expense.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = Db.Employees.FirstOrDefault(x => x.Id == expense.EmployeeId);
        return View(expense);
    }

    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        return View(new ExpenseFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ExpenseFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Expenses.Max(x => (int?)x.Id) ?? 100) + 1;
        var expense = new Expense
        {
            Id = id,
            Code = $"EXP-{id}",
            ExpenseDate = model.ExpenseDate,
            EmployeeId = model.EmployeeId,
            ExpenseType = model.ExpenseType,
            Amount = model.Amount,
            Notes = model.Notes
        };
        Db.Expenses.Add(expense);
        QueueAudit("Expense", "Create", expense.Code, $"{expense.ExpenseType} masraf kaydi olusturuldu.");

        Db.SaveChanges();
        TempData["Flash"] = "Masraf kaydi olusturuldu.";
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

        ViewBag.Employees = Db.Employees.ToList();
        return View(new ExpenseFormViewModel
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            EmployeeId = expense.EmployeeId,
            ExpenseType = expense.ExpenseType,
            Amount = expense.Amount,
            Notes = expense.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ExpenseFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var expense = Db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(expense.EmployeeId))
        {
            return NotFound();
        }

        expense.ExpenseDate = model.ExpenseDate;
        expense.EmployeeId = model.EmployeeId;
        expense.ExpenseType = model.ExpenseType;
        expense.Amount = model.Amount;
        expense.Notes = model.Notes;
        QueueAudit("Expense", "Update", expense.Code, $"{expense.Code} masraf kaydi guncellendi.");
        Db.SaveChanges();

        TempData["Flash"] = "Masraf kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var expense = Db.Expenses.FirstOrDefault(x => x.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(expense.EmployeeId))
        {
            return NotFound();
        }

        QueueAudit("Expense", "Delete", expense.Code, $"{expense.Code} masraf kaydi silindi.");
        Db.Expenses.Remove(expense);
        Db.SaveChanges();
        TempData["Flash"] = "Masraf kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private ExpenseFormViewModel ToExpenseFormModel(ExpenseInlineEditViewModel model, int? currentEmployeeId, bool canManageAll)
    {
        return new ExpenseFormViewModel
        {
            Id = model.Id,
            ExpenseDate = model.ExpenseDate == default ? DateTime.Today : model.ExpenseDate,
            EmployeeId = canManageAll ? model.EmployeeId : currentEmployeeId ?? model.EmployeeId,
            ExpenseType = model.ExpenseType,
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
