using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class AccountsController : AppController
{
    private readonly InsuranceManagement.Web.Services.IAccountService _accountService;

    public AccountsController(AppDbContext db, InsuranceManagement.Web.Services.IAccountService accountService) : base(db)
    {
        _accountService = accountService;
    }

    public IActionResult Index(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null, string? sortBy = "displayname", bool isDescending = false)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;
        ViewBag.SortBy = sortBy;
        ViewBag.IsDescending = isDescending;

        var filterEmployeeId = CurrentEmployeeScopeId();
        var items = _accountService.GetAll(page, pageSize, out var totalCount, searchTerm, status, null, filterEmployeeId, sortBy, isDescending);
        
        // Final permission filtering if service doesn't handle it
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            items = items.Where(x => x.OwnerEmployeeId == currentEmployeeId.Value).ToList();
            totalCount = items.Count; // This is a bit inefficient for big data but okay for now
        }

        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = page;

        return View(new AccountsIndexViewModel
        {
            NewAccount = new AccountInlineEditViewModel
            {
                AccountType = AccountType.Corporate,
                OwnerEmployeeId = currentEmployeeId,
                Status = "Aktif"
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = items.Select(x => new AccountInlineEditViewModel
            {
                Id = x.Id,
                Code = x.Code,
                AccountType = x.AccountType,
                DisplayName = x.DisplayName,
                City = x.City,
                District = x.District,
                Phone = x.Phone,
                Email = x.Email,
                TaxNumber = x.TaxNumber,
                Status = x.Status,
                OwnerEmployeeId = x.OwnerEmployeeId ?? 0,
                Notes = x.Notes
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BulkSave(AccountGridSaveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PayloadJson))
        {
            TempData["Warning"] = "Kaydedilecek degisiklik bulunamadi.";
            return RedirectToAction(nameof(Index), new { page = model.Page });
        }

        List<AccountInlineEditViewModel>? payload;
        try
        {
            payload = JsonSerializer.Deserialize<List<AccountInlineEditViewModel>>(model.PayloadJson, BulkSaveJsonOptions);
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
        var nextId = (Db.Accounts.Max(x => (int?)x.Id) ?? 100) + 1;
        var currentEmployeeId = CurrentEmployeeScopeId();
        var canManageAll = HasGlobalEmployeeAccess();

        foreach (var item in payload)
        {
            var formModel = ToAccountFormModel(item, currentEmployeeId, canManageAll);
            var validationErrors = ValidateInlineAccount(formModel);
            if (validationErrors.Count > 0)
            {
                errors.AddRange(validationErrors.Select(x => $"{(string.IsNullOrWhiteSpace(item.Code) ? "Yeni Satir" : item.Code)}: {x}"));
                continue;
            }

            if (item.Id.HasValue)
            {
                var account = Db.Accounts.FirstOrDefault(x => x.Id == item.Id.Value);
                if (account is null)
                {
                    errors.Add($"#{item.Id.Value} id'li musteri bulunamadi.");
                    continue;
                }

                if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
                {
                    errors.Add($"{account.Code}: Bu kaydi guncelleme yetkin yok.");
                    continue;
                }

                warnings.AddRange(_accountService.CheckDuplicate(formModel.DisplayName ?? string.Empty, formModel.Phone, formModel.Email, formModel.TaxNumber, item.Id).Select(x => $"{account.Code}: {x}"));
                account.AccountType = formModel.AccountType;
                account.DisplayName = formModel.DisplayName;
                account.City = formModel.City;
                account.District = formModel.District;
                account.Phone = formModel.Phone;
                account.Email = formModel.Email;
                account.TaxNumber = formModel.TaxNumber;
                account.OwnerEmployeeId = formModel.OwnerEmployeeId;
                account.Status = formModel.Status;
                account.Notes = formModel.Notes;
                QueueAudit("Account", "Update", account.Code, $"{account.DisplayName} kaydi toplu grid kaydi ile guncellendi.");
                updatedCount++;
            }
            else
            {
                warnings.AddRange(_accountService.CheckDuplicate(formModel.DisplayName ?? string.Empty, formModel.Phone, formModel.Email, formModel.TaxNumber, null).Select(x => $"Yeni Musteri: {x}"));
                var account = new Account
                {
                    Id = nextId,
                    Code = $"ACC-{nextId}",
                    AccountType = formModel.AccountType,
                    DisplayName = formModel.DisplayName,
                    City = formModel.City,
                    District = formModel.District,
                    Phone = formModel.Phone,
                    Email = formModel.Email,
                    TaxNumber = formModel.TaxNumber,
                    OwnerEmployeeId = formModel.OwnerEmployeeId,
                    Status = formModel.Status,
                    Notes = formModel.Notes
                };
                Db.Accounts.Add(account);
                QueueAudit("Account", "Create", account.Code, $"{account.DisplayName} kaydi toplu grid kaydi ile olusturuldu.");
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
        MarkGridDraftCleared("account-grid-draft-v1");
        TempData["Flash"] = $"{createdCount} yeni, {updatedCount} mevcut musteri toplu kaydedildi.";
        if (warnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", warnings.Distinct());
        }
        return RedirectToAction(nameof(Index), new { page = model.Page });
    }

    public IActionResult Details(int id)
    {
        BuildShell();
        var account = _accountService.GetById(id, CurrentEmployeeScopeId());
        if (account is null)
        {
            return NotFound();
        }

        if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
        {
            return NotFound();
        }

        ViewBag.Owner = account.OwnerEmployee;
        ViewBag.Activities = account.Activities
            .OrderByDescending(x => x.ActivityDate)
            .ToList();
        ViewBag.Sales = account.Sales
            .OrderByDescending(x => x.SaleDate)
            .ToList();
        return View(account);
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        ViewBag.DuplicateWarnings = Array.Empty<string>();
        return View(new AccountFormViewModel());
    }

    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AccountFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        
        var account = new Account
        {
            AccountType = model.AccountType,
            DisplayName = model.DisplayName ?? string.Empty,
            City = model.City ?? string.Empty,
            District = model.District,
            Phone = model.Phone,
            Email = model.Email,
            TaxNumber = model.TaxNumber,
            OwnerEmployeeId = HasGlobalEmployeeAccess() ? model.OwnerEmployeeId : (CurrentEmployeeScopeId() ?? model.OwnerEmployeeId),
            Status = model.Status,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _accountService.Validate(account);
        if (!isValid)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.Key, err.Value);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.DuplicateWarnings = _accountService.CheckDuplicate(model.DisplayName ?? string.Empty, model.Phone, model.Email, model.TaxNumber, null);
            return View(model);
        }

        var duplicates = _accountService.CheckDuplicate(model.DisplayName ?? string.Empty, model.Phone, model.Email, model.TaxNumber, null);
        if (duplicates.Any() && !Request.Form.ContainsKey("ignoreDuplicates"))
        {
            ViewBag.DuplicateWarnings = duplicates;
            return View(model);
        }

        _accountService.Create(account);
        TempData["Success"] = "Musteri kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        BuildShell();
        var account = Db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null)
        {
            return NotFound();
        }

        if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
        {
            return NotFound();
        }

        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();
        var model = new AccountFormViewModel
        {
            Id = account.Id,
            AccountType = account.AccountType,
            DisplayName = account.DisplayName,
            City = account.City,
            District = account.District,
            Phone = account.Phone,
            Email = account.Email,
            TaxNumber = account.TaxNumber,
            OwnerEmployeeId = account.OwnerEmployeeId,
            Status = account.Status,
            Notes = account.Notes
        };
        ViewBag.DuplicateWarnings = _accountService.CheckDuplicate(model.DisplayName ?? string.Empty, model.Phone, model.Email, model.TaxNumber, id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, AccountFormViewModel model)
    {
        BuildShell();
        var currentEmployeeId = CurrentEmployeeScopeId();
        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();

        var duplicates = _accountService.CheckDuplicate(model.DisplayName ?? string.Empty, model.Phone, model.Email, model.TaxNumber, id);
        if (duplicates.Any() && !Request.Form.ContainsKey("ignoreDuplicates"))
        {
            ViewBag.DuplicateWarnings = duplicates;
            return View(model);
        }
        
        var tempAccount = new Account
        {
            Id = id,
            AccountType = model.AccountType,
            DisplayName = model.DisplayName ?? string.Empty,
            City = model.City ?? string.Empty,
            District = model.District,
            Phone = model.Phone,
            Email = model.Email,
            TaxNumber = model.TaxNumber,
            OwnerEmployeeId = model.OwnerEmployeeId,
            Status = model.Status,
            Notes = model.Notes ?? string.Empty
        };

        var (isValid, errors) = _accountService.Validate(tempAccount);
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

        var existing = _accountService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null || (existing.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(existing.OwnerEmployeeId.Value)))
        {
            return NotFound();
        }

        _accountService.Update(id, tempAccount);
        TempData["Success"] = "Musteri kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var existing = _accountService.GetById(id, CurrentEmployeeScopeId());
        if (existing is null)
        {
            return NotFound();
        }
        
        _accountService.Delete(id);
        TempData["Success"] = "Musteri kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private AccountFormViewModel ToAccountFormModel(AccountInlineEditViewModel model, int? currentEmployeeId, bool canManageAll)
    {
        return new AccountFormViewModel
        {
            Id = model.Id,
            AccountType = model.AccountType,
            DisplayName = model.DisplayName,
            City = model.City,
            District = model.District,
            Phone = model.Phone,
            Email = model.Email,
            TaxNumber = model.TaxNumber,
            OwnerEmployeeId = canManageAll ? model.OwnerEmployeeId : currentEmployeeId,
            Status = string.IsNullOrWhiteSpace(model.Status) ? "Aktif" : model.Status,
            Notes = model.Notes
        };
    }

    private static List<string> ValidateInlineAccount(AccountFormViewModel model)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(model.DisplayName))
        {
            errors.Add("Ad veya unvan zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(model.City))
        {
            errors.Add("Sehir zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.Email))
        {
            errors.Add("Telefon veya e-posta alanlarindan en az biri dolu olmalidir.");
        }

        return errors;
    }
}
