using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InsuranceManagement.Web.Controllers;

public class AccountsController : AppController
{
    public AccountsController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        BuildShell();
        var accountsQuery = Db.Accounts.AsQueryable();
        var currentEmployeeId = CurrentEmployeeScopeId();
        if (!HasGlobalEmployeeAccess() && currentEmployeeId.HasValue)
        {
            accountsQuery = accountsQuery.Where(x => !x.OwnerEmployeeId.HasValue || x.OwnerEmployeeId == currentEmployeeId.Value);
        }

        ViewBag.Employees = HasGlobalEmployeeAccess()
            ? Db.Employees.OrderBy(x => x.FullName).ToList()
            : Db.Employees.Where(x => x.Id == currentEmployeeId).OrderBy(x => x.FullName).ToList();

        var totalCount = accountsQuery.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        return View(new AccountsIndexViewModel
        {
            NewAccount = new AccountInlineEditViewModel
            {
                AccountType = AccountType.Corporate,
                OwnerEmployeeId = currentEmployeeId,
                Status = "Active"
            },
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = accountsQuery
                .OrderByDescending(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountInlineEditViewModel
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
                    OwnerEmployeeId = x.OwnerEmployeeId,
                    Status = x.Status,
                    Notes = x.Notes
                })
                .ToList()
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

                warnings.AddRange(BuildDuplicateWarnings(formModel, item.Id).Select(x => $"{account.Code}: {x}"));
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
                warnings.AddRange(BuildDuplicateWarnings(formModel, null).Select(x => $"Yeni Musteri: {x}"));
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
        var account = Db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null)
        {
            return NotFound();
        }

        if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
        {
            return NotFound();
        }

        ViewBag.Owner = account.OwnerEmployeeId.HasValue ? Db.Employees.FirstOrDefault(x => x.Id == account.OwnerEmployeeId.Value) : null;
        ViewBag.Activities = Db.Activities.Where(x => x.AccountId == account.Id).OrderByDescending(x => x.ActivityDate).ToList();
        ViewBag.Sales = Db.Sales.Where(x => x.AccountId == account.Id).OrderByDescending(x => x.SaleDate).ToList();
        return View(account);
    }

    [HttpGet]
    public IActionResult Create()
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        ViewBag.DuplicateWarnings = Array.Empty<string>();
        return View(new AccountFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AccountFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        var duplicateWarnings = BuildDuplicateWarnings(model, null);
        ViewBag.DuplicateWarnings = duplicateWarnings;

        if (string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(string.Empty, "Telefon veya e-posta alanlarindan en az biri dolu olmalidir.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.Accounts.Max(x => (int?)x.Id) ?? 100) + 1;
        var account = new Account
        {
            Id = id,
            Code = $"ACC-{id}",
            AccountType = model.AccountType,
            DisplayName = model.DisplayName,
            City = model.City,
            District = model.District,
            Phone = model.Phone,
            Email = model.Email,
            TaxNumber = model.TaxNumber,
            OwnerEmployeeId = model.OwnerEmployeeId,
            Status = model.Status,
            Notes = model.Notes
        };
        Db.Accounts.Add(account);
        QueueAudit("Account", "Create", account.Code, $"{account.DisplayName} kaydi olusturuldu.");

        Db.SaveChanges();
        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }
        TempData["Flash"] = "Musteri kaydi olusturuldu.";
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

        ViewBag.Employees = Db.Employees.ToList();
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
        ViewBag.DuplicateWarnings = BuildDuplicateWarnings(model, id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, AccountFormViewModel model)
    {
        BuildShell();
        ViewBag.Employees = Db.Employees.ToList();
        var duplicateWarnings = BuildDuplicateWarnings(model, id);
        ViewBag.DuplicateWarnings = duplicateWarnings;

        if (string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(string.Empty, "Telefon veya e-posta alanlarindan en az biri dolu olmalidir.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var account = Db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null)
        {
            return NotFound();
        }

        if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
        {
            return NotFound();
        }

        account.AccountType = model.AccountType;
        account.DisplayName = model.DisplayName;
        account.City = model.City;
        account.District = model.District;
        account.Phone = model.Phone;
        account.Email = model.Email;
        account.TaxNumber = model.TaxNumber;
        account.OwnerEmployeeId = model.OwnerEmployeeId;
        account.Status = model.Status;
        account.Notes = model.Notes;
        QueueAudit("Account", "Update", account.Code, $"{account.DisplayName} kaydi guncellendi.");
        Db.SaveChanges();

        if (duplicateWarnings.Count > 0)
        {
            TempData["Warning"] = string.Join(" | ", duplicateWarnings);
        }
        TempData["Flash"] = "Musteri kaydi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var account = Db.Accounts.FirstOrDefault(x => x.Id == id);
        if (account is null)
        {
            return NotFound();
        }

        if (account.OwnerEmployeeId.HasValue && !CanSeeEmployeeData(account.OwnerEmployeeId.Value))
        {
            return NotFound();
        }

        QueueAudit("Account", "Delete", account.Code, $"{account.DisplayName} kaydi silindi.");
        Db.Accounts.Remove(account);
        Db.SaveChanges();
        TempData["Flash"] = "Musteri kaydi silindi.";
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
            Status = string.IsNullOrWhiteSpace(model.Status) ? "Active" : model.Status,
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

    private List<string> BuildDuplicateWarnings(AccountFormViewModel model, int? currentId)
    {
        var warnings = new List<string>();
        var accounts = Db.Accounts.Where(x => !currentId.HasValue || x.Id != currentId.Value);

        if (!string.IsNullOrWhiteSpace(model.DisplayName) &&
            accounts.Any(x => x.DisplayName == model.DisplayName))
        {
            warnings.Add("Ayni musteri/firma ismiyle baska bir kayit bulundu.");
        }

        if (!string.IsNullOrWhiteSpace(model.Phone) &&
            accounts.Any(x => x.Phone == model.Phone))
        {
            warnings.Add("Bu telefon numarasi baska bir musteri kaydinda kullaniliyor.");
        }

        if (!string.IsNullOrWhiteSpace(model.Email) &&
            accounts.Any(x => x.Email == model.Email))
        {
            warnings.Add("Bu e-posta adresi baska bir musteri kaydinda kullaniliyor.");
        }

        if (!string.IsNullOrWhiteSpace(model.TaxNumber) &&
            accounts.Any(x => x.TaxNumber == model.TaxNumber))
        {
            warnings.Add("Bu vergi numarasi ile mevcut bir musteri kaydi var.");
        }

        return warnings;
    }
}
