using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class SalesController : AppController
{
    private readonly InsuranceManagement.Web.Services.ISaleService _saleService;

    public SalesController(AppDbContext db, InsuranceManagement.Web.Services.ISaleService saleService) : base(db)
    {
        _saleService = saleService;
    }

    public IActionResult Index(int page = 1, int pageSize = 10, string? searchTerm = null, int? employeeId = null, string? sortBy = "date", bool isDescending = true, DateTime? start = null, DateTime? end = null, int? productTypeId = null)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        
        int? filterEmployeeId = employeeId;
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            filterEmployeeId = currentEmployeeId.Value;
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.OrderBy(x => x.DisplayName).ToList();
        ViewBag.Activities = Db.Activities.OrderByDescending(x => x.ActivityDate).Take(250).ToList();
        ViewBag.ProductTypes = Db.InsuranceProductTypes.OrderBy(x => x.DisplayOrder).ToList();
        
        ViewBag.SearchTerm = searchTerm;
        ViewBag.EmployeeId = filterEmployeeId;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;

        var filterEmployeeIdField = CurrentEmployeeScopeId();
        var items = _saleService.GetAll(page, pageSize, out var totalCount, searchTerm, null, filterEmployeeId, sortBy, isDescending, start, end, productTypeId);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = page;

        return View(new SalesIndexViewModel
        {
            NewSale = new SaleInlineEditViewModel
            {
                SaleDate = DateTime.Today,
                EmployeeId = currentEmployeeId ?? 0
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = items.Select(x => new SaleInlineEditViewModel
            {
                Id = x.Id,
                Code = x.Code,
                SaleDate = x.SaleDate,
                EmployeeId = x.EmployeeId,
                AccountId = x.AccountId,
                ActivityId = x.ActivityId,
                ProductTypeId = x.ProductTypeId,
                CollectionAmount = x.CollectionAmount,
                ApeAmount = x.ApeAmount,
                LumpSumAmount = x.LumpSumAmount,
                MonthlyPaymentAmount = x.MonthlyPaymentAmount,
                PremiumAmount = x.PremiumAmount,
                ProductionAmount = x.ProductionAmount,
                SaleAmount = x.SaleAmount,
                SaleCount = x.SaleCount,
                Notes = x.Notes
            }).ToList()
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
                sale.ProductTypeId = formModel.ProductTypeId;
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
                    ProductTypeId = formModel.ProductTypeId,
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
        var sale = _saleService.GetById(id, CurrentEmployeeScopeId());
        if (sale is null || !CanSeeEmployeeData(sale.EmployeeId))
        {
            return NotFound();
        }

        ViewBag.Employee = sale.Employee;
        ViewBag.Account = sale.Account;
        ViewBag.Activity = sale.Activity;
        return View(sale);
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpGet]
    public IActionResult Create(int? activityId = null)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        ViewBag.ProductTypes = Db.InsuranceProductTypes.OrderBy(x => x.DisplayOrder).ToList();
        
        var model = new SaleFormViewModel { SaleDate = DateTime.Today };
        
        if (activityId.HasValue)
        {
            var activity = Db.Activities.FirstOrDefault(x => x.Id == activityId.Value);
            if (activity != null)
            {
                model.ActivityId = activity.Id;
                model.AccountId = activity.AccountId ?? 0;
                model.EmployeeId = activity.EmployeeId;
            }
        }
        
        return View(model);
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SaleFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        ViewBag.ProductTypes = Db.InsuranceProductTypes.OrderBy(x => x.DisplayOrder).ToList();

        var sale = new Sale
        {
            SaleDate = model.SaleDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            ActivityId = model.ActivityId,
            ProductTypeId = model.ProductTypeId,
            CollectionAmount = model.CollectionAmount,
            ApeAmount = model.ApeAmount,
            LumpSumAmount = model.LumpSumAmount,
            MonthlyPaymentAmount = model.MonthlyPaymentAmount,
            PremiumAmount = model.PremiumAmount,
            ProductionAmount = model.ProductionAmount,
            SaleAmount = model.SaleAmount,
            SaleCount = model.SaleCount,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _saleService.Validate(sale);
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

        _saleService.Create(sale);
        TempData["Success"] = "Satis kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Operations,SalesManager")]
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

        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        ViewBag.ProductTypes = Db.InsuranceProductTypes.OrderBy(x => x.DisplayOrder).ToList();
        return View(new SaleFormViewModel
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            EmployeeId = sale.EmployeeId,
            AccountId = sale.AccountId,
            ActivityId = sale.ActivityId,
            ProductTypeId = sale.ProductTypeId,
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

    [Authorize(Roles = "Admin,Operations,SalesManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, SaleFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.Accounts = Db.Accounts.ToList();
        ViewBag.Activities = Db.Activities.ToList();
        ViewBag.ProductTypes = Db.InsuranceProductTypes.OrderBy(x => x.DisplayOrder).ToList();

        var tempSale = new Sale
        {
            Id = id,
            SaleDate = model.SaleDate,
            EmployeeId = model.EmployeeId,
            AccountId = model.AccountId,
            ActivityId = model.ActivityId,
            ProductTypeId = model.ProductTypeId,
            CollectionAmount = model.CollectionAmount,
            ApeAmount = model.ApeAmount,
            LumpSumAmount = model.LumpSumAmount,
            MonthlyPaymentAmount = model.MonthlyPaymentAmount,
            PremiumAmount = model.PremiumAmount,
            ProductionAmount = model.ProductionAmount,
            SaleAmount = model.SaleAmount,
            SaleCount = model.SaleCount,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _saleService.Validate(tempSale);
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

        var existing = _saleService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null || !CanSeeEmployeeData(existing.EmployeeId))
        {
            return NotFound();
        }

        _saleService.Update(id, tempSale);
        TempData["Success"] = "Satis kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var existing = _saleService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null)
        {
            return NotFound();
        }
        
        _saleService.Delete(id);
        TempData["Success"] = "Satis kaydi silindi.";
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
            ProductTypeId = model.ProductTypeId,
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
