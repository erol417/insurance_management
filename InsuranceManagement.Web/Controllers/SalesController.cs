using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

public class SalesController : AppController
{
    public SalesController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        BuildShell();
        var itemsQuery = Db.Sales.AsQueryable();
        var currentEmployeeId = CurrentEmployeeScopeId();
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            itemsQuery = itemsQuery.Where(x => x.EmployeeId == currentEmployeeId.Value);
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.OrderBy(x => x.DisplayName).ToList();
        ViewBag.Activities = Db.Activities.OrderByDescending(x => x.ActivityDate).Take(250).ToList();

        var totalCount = itemsQuery.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        return View(new SalesIndexViewModel
        {
            NewSale = new SaleInlineEditViewModel
            {
                SaleDate = DateTime.Today,
                EmployeeId = currentEmployeeId ?? Db.Employees.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault(),
                ProductType = ProductType.Bes,
                SaleCount = 1
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = itemsQuery
                .OrderByDescending(x => x.SaleDate)
                .ThenByDescending(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SaleInlineEditViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    SaleDate = x.SaleDate,
                    EmployeeId = x.EmployeeId,
                    AccountId = x.AccountId,
                    ActivityId = x.ActivityId,
                    ProductType = x.ProductType,
                    CollectionAmount = x.CollectionAmount,
                    ApeAmount = x.ApeAmount,
                    LumpSumAmount = x.LumpSumAmount,
                    MonthlyPaymentAmount = x.MonthlyPaymentAmount,
                    PremiumAmount = x.PremiumAmount,
                    ProductionAmount = x.ProductionAmount,
                    SaleAmount = x.SaleAmount,
                    SaleCount = x.SaleCount,
                    Notes = x.Notes
                })
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(SaleGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        List<SaleInlineEditViewModel>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<SaleInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
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
        var nextId = (Db.Sales.Max(x => (int?)x.Id) ?? 100) + 1;
        var currentEmployeeId = CurrentEmployeeScopeId();
        var canManageAll = HasGlobalEmployeeAccess();

        foreach (var item in payload)
        {
            var formModel = ToSaleFormModel(item, currentEmployeeId, canManageAll);
            var validationErrors = ValidateInlineSale(formModel);
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(x => $"{(string.IsNullOrWhiteSpace(item.Code) ? "Yeni Satir" : item.Code)}: {x}"));
                continue;
            }

            if (item.Id.HasValue)
            {
                var sale = Db.Sales.FirstOrDefault(x => x.Id == item.Id.Value);
                if (sale is null)
                {
                    errors.Add($"#{item.Id.Value} id'li satis bulunamadi.");
                    continue;
                }

                if (!CanSeeEmployeeData(sale.EmployeeId) || (!canManageAll && formModel.EmployeeId != currentEmployeeId))
                {
                    errors.Add($"{sale.Code}: Bu kaydi guncelleme yetkin yok.");
                    continue;
                }

                sale.SaleDate = formModel.SaleDate;
                sale.EmployeeId = formModel.EmployeeId;
                sale.AccountId = formModel.AccountId;
                sale.ActivityId = formModel.ActivityId;
                sale.ProductType = formModel.ProductType;
                sale.CollectionAmount = formModel.CollectionAmount;
                sale.ApeAmount = formModel.ApeAmount;
                sale.LumpSumAmount = formModel.LumpSumAmount;
                sale.MonthlyPaymentAmount = formModel.MonthlyPaymentAmount;
                sale.PremiumAmount = formModel.PremiumAmount;
                sale.ProductionAmount = formModel.ProductionAmount;
                sale.SaleAmount = formModel.SaleAmount;
                sale.SaleCount = formModel.SaleCount;
                sale.Notes = formModel.Notes;
                QueueAudit("Sale", "Update", sale.Code, $"{sale.Code} satis kaydi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                var sale = new Sale
                {
                    Id = nextId,
                    Code = $"SAL-{nextId}",
                    SaleDate = formModel.SaleDate,
                    EmployeeId = formModel.EmployeeId,
                    AccountId = formModel.AccountId,
                    ActivityId = formModel.ActivityId,
                    ProductType = formModel.ProductType,
                    CollectionAmount = formModel.CollectionAmount,
                    ApeAmount = formModel.ApeAmount,
                    LumpSumAmount = formModel.LumpSumAmount,
                    MonthlyPaymentAmount = formModel.MonthlyPaymentAmount,
                    PremiumAmount = formModel.PremiumAmount,
                    ProductionAmount = formModel.ProductionAmount,
                    SaleAmount = formModel.SaleAmount,
                    SaleCount = formModel.SaleCount,
                    Notes = formModel.Notes
                };
                Db.Sales.Add(sale);
                QueueAudit("Sale", "Create", sale.Code, $"{sale.Code} satis kaydi toplu grid kaydi ile olusturuldu.");
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
        MarkGridDraftCleared("sale-grid-draft-v1");
        TempData["Flash"] = $"{createdCount} yeni, {updatedCount} mevcut satis toplu kaydedildi.";
        return RedirectToAction(nameof(Index), new { page = model.Page });
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var sale = Db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null || !CanSeeEmployeeData(sale.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = Db.Employees.FirstOrDefault(x => x.Id == sale.EmployeeId);
        ViewBag.Account = Db.Accounts.FirstOrDefault(x => x.Id == sale.AccountId);
        ViewBag.Activity = sale.ActivityId.HasValue ? Db.Activities.FirstOrDefault(x => x.Id == sale.ActivityId.Value) : null;
        return View(sale);
    }

    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        return View(new SaleFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SaleFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Sales.Max(x => (int?)x.Id) ?? 100) + 1;
        var sale = new Sale
        {
            Id = id,
            Code = $"SAL-{id}",
            SaleDate = model.SaleDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            ActivityId = model.ActivityId,
            ProductType = model.ProductType,
            CollectionAmount = model.CollectionAmount,
            ApeAmount = model.ApeAmount,
            LumpSumAmount = model.LumpSumAmount,
            MonthlyPaymentAmount = model.MonthlyPaymentAmount,
            PremiumAmount = model.PremiumAmount,
            ProductionAmount = model.ProductionAmount,
            SaleAmount = model.SaleAmount,
            SaleCount = model.SaleCount,
            Notes = model.Notes
        };
        Db.Sales.Add(sale);
        QueueAudit("Sale", "Create", sale.Code, $"{sale.ProductType} satis kaydi olusturuldu.");

        Db.SaveChanges();
        TempData["Flash"] = "Satis kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var sale = Db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(sale.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        return View(new SaleFormViewModel
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            EmployeeId = sale.EmployeeId,
            AccountId = sale.AccountId,
            ActivityId = sale.ActivityId,
            ProductType = sale.ProductType,
            CollectionAmount = sale.CollectionAmount,
            ApeAmount = sale.ApeAmount,
            LumpSumAmount = sale.LumpSumAmount,
            MonthlyPaymentAmount = sale.MonthlyPaymentAmount,
            PremiumAmount = sale.PremiumAmount,
            ProductionAmount = sale.ProductionAmount,
            SaleAmount = sale.SaleAmount,
            SaleCount = sale.SaleCount,
            Notes = sale.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, SaleFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var sale = Db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(sale.EmployeeId))
        {
            return NotFound();
        }

        sale.SaleDate = model.SaleDate;
        sale.EmployeeId = model.EmployeeId;
        sale.AccountId = model.AccountId;
        sale.ActivityId = model.ActivityId;
        sale.ProductType = model.ProductType;
        sale.CollectionAmount = model.CollectionAmount;
        sale.ApeAmount = model.ApeAmount;
        sale.LumpSumAmount = model.LumpSumAmount;
        sale.MonthlyPaymentAmount = model.MonthlyPaymentAmount;
        sale.PremiumAmount = model.PremiumAmount;
        sale.ProductionAmount = model.ProductionAmount;
        sale.SaleAmount = model.SaleAmount;
        sale.SaleCount = model.SaleCount;
        sale.Notes = model.Notes;
        QueueAudit("Sale", "Update", sale.Code, $"{sale.Code} satis kaydi guncellendi.");
        Db.SaveChanges();

        TempData["Flash"] = "Satis kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var sale = Db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null)
        {
            return NotFound();
        }

        if (!CanSeeEmployeeData(sale.EmployeeId))
        {
            return NotFound();
        }

        QueueAudit("Sale", "Delete", sale.Code, $"{sale.Code} satis kaydi silindi.");
        Db.Sales.Remove(sale);
        Db.SaveChanges();
        TempData["Flash"] = "Satis kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private SaleFormViewModel ToSaleFormModel(SaleInlineEditViewModel model, int? currentEmployeeId, bool canManageAll)
    {
        return new SaleFormViewModel
        {
            Id = model.Id,
            SaleDate = model.SaleDate == default ? DateTime.Today : model.SaleDate,
            EmployeeId = canManageAll ? model.EmployeeId : currentEmployeeId ?? model.EmployeeId,
            AccountId = model.AccountId,
            ActivityId = model.ActivityId,
            ProductType = model.ProductType,
            CollectionAmount = model.CollectionAmount,
            ApeAmount = model.ApeAmount,
            LumpSumAmount = model.LumpSumAmount,
            MonthlyPaymentAmount = model.MonthlyPaymentAmount,
            PremiumAmount = model.PremiumAmount,
            ProductionAmount = model.ProductionAmount,
            SaleAmount = model.SaleAmount,
            SaleCount = model.SaleCount <= 0 ? 1 : model.SaleCount,
            Notes = model.Notes
        };
    }

    private static List<string> ValidateInlineSale(SaleFormViewModel model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, validationResults, true);
        return validationResults.Select(x => x.ErrorMessage ?? "Gecersiz satis kaydi.").ToList();
    }
}
