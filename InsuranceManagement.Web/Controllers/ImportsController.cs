using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InsuranceManagement.Web.Controllers;

[Authorize]
public class ImportsController : AppController
{
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        if (!CanAccessPermission("Imports"))
        {
            context.Result = Forbid();
        }
    }
    private static readonly string[] AllowedExtensions = [".xlsx", ".xls", ".csv"];
    private const long MaxFileSizeBytes = 15 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;

    public ImportsController(AppDbContext db, IWebHostEnvironment environment) : base(db)
    {
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        BuildShell();
        return View(new ImportUploadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upload(ImportUploadViewModel model)
    {
        BuildShell();
        ValidateFile(model.File);

        if (string.IsNullOrWhiteSpace(model.ModuleName))
        {
            ModelState.AddModelError(nameof(ImportUploadViewModel.ModuleName), "Lutfen veri yuklenecek departmani seciniz.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = (Db.ImportBatches.Max(x => (int?)x.Id) ?? 0) + 1;
        var originalFileName = Path.GetFileName(model.File!.FileName);
        var extension = Path.GetExtension(originalFileName);
        var safeFileName = $"{id:D6}_{Path.GetFileNameWithoutExtension(originalFileName)}{extension}".Replace(' ', '_');
        var importDirectory = GetImportDirectory();
        var fullPath = Path.Combine(importDirectory, safeFileName);

        using (var stream = System.IO.File.Create(fullPath))
        {
            model.File.CopyTo(stream);
        }

        var batch = new ImportBatch
        {
            Id = id,
            ModuleName = model.ModuleName.ToLowerInvariant(),
            FileName = originalFileName,
            ImportedAt = DateTime.Now,
            ImportedBy = User.Identity?.Name ?? "unknown",
            Status = "Uploaded",
            Notes = BuildImportNotes(model.Notes, model.File.Length, safeFileName)
        };
        Db.ImportBatches.Add(batch);
        QueueAudit("Import", "Create", $"IMP-{batch.Id}", $"{batch.ModuleName} departmani icin {batch.FileName} dosyasi yuklendi.");

        Db.SaveChanges();
        TempData["Flash"] = $"{batch.ModuleName.ToUpper()} import dosyasi yuklendi. Simdi verileri onizleyin.";
        
        return RedirectToAction(nameof(Preview), new { id = batch.Id });
    }

    public IActionResult History()
    {
        BuildShell();
        return View(Db.ImportBatches.OrderByDescending(x => x.ImportedAt).ToList());
    }

    [HttpGet]
    public IActionResult Template(string module = "lead")
    {
        var normalizedModule = (module ?? "lead").Trim().ToLowerInvariant();
        var fileName = $"{normalizedModule}_import_template.xlsx";
        var sheetName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(normalizedModule) + " Import";

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add(sheetName);
        
        string[] headers = normalizedModule switch
        {
            "employee" => ["FullName", "Region", "City", "HasLogin", "UserName", "Password", "Role"],
            "account" => ["Code", "AccountType", "DisplayName", "City", "District", "Phone", "Email", "TaxNumber", "Status", "OwnerEmployee"],
            "activity" => ["ActivityDate", "Employee", "Account", "ContactName", "ContactStatus", "OutcomeStatus", "Summary"],
            "sale" => ["SaleDate", "Employee", "Account", "ProductType", "CollectionAmount", "ApeAmount", "LumpSumAmount", "MonthlyPaymentAmount", "PremiumAmount", "ProductionAmount", "SaleCount", "Notes"],
            "expense" => ["ExpenseDate", "Employee", "ExpenseType", "Amount", "Notes"],
            _ => ["DisplayName", "City", "District", "ContactName", "Phone", "Email", "Source", "Status", "Priority", "Note", "AssignedEmployee"]
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
        }

        // Sample Data Row
        if (normalizedModule == "lead")
        {
            ws.Cell(2, 1).Value = "Acme Holding";
            ws.Cell(2, 2).Value = "Istanbul";
            ws.Cell(2, 5).Value = "05332221100";
            ws.Cell(2, 7).Value = "CALL_CENTER";
            ws.Cell(2, 8).Value = "NEW";
        }
        else if (normalizedModule == "sale")
        {
            ws.Cell(2, 1).Value = DateTime.Today.ToString("yyyy-MM-dd");
            ws.Cell(2, 2).Value = "Ahmet Yilmaz"; // Employee Name
            ws.Cell(2, 3).Value = "Acme Holding"; // Account Name
            ws.Cell(2, 4).Value = "BES";
            ws.Cell(2, 5).Value = 1500.00;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public IActionResult Download(int id)
    {
        var batch = Db.ImportBatches.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (batch is null)
        {
            return NotFound();
        }

        var importDirectory = GetImportDirectory();
        var storedFile = Directory
            .GetFiles(importDirectory, $"{id:D6}_*")
            .OrderByDescending(System.IO.File.GetLastWriteTimeUtc)
            .FirstOrDefault();

        if (storedFile is null || !System.IO.File.Exists(storedFile))
        {
            TempData["Warning"] = "Bu batch icin fiziksel dosya bulunamadi.";
            return RedirectToAction(nameof(History));
        }

        var contentType = GetContentType(Path.GetExtension(batch.FileName));
        return File(System.IO.File.OpenRead(storedFile), contentType, batch.FileName);
    }

    [HttpGet]
    public IActionResult Preview(int id)
    {
        BuildShell();
        var batch = Db.ImportBatches.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (batch is null) return NotFound();

        var storedFile = GetStoredFilePath(id);
        if (storedFile is null)
        {
            TempData["Warning"] = "Import dosyasi bulunamadi.";
            return RedirectToAction(nameof(History));
        }

        var preview = BuildBatchPreview(batch, storedFile);
        return View(preview);
    }

    private ImportPreviewViewModel BuildBatchPreview(ImportBatch batch, string storedFile)
    {
        var extension = Path.GetExtension(storedFile).ToLowerInvariant();
        var rows = new List<ImportPreviewRowViewModel>();
        var columns = GetColumnsForModule(batch.ModuleName);

        if (extension == ".xlsx" || extension == ".xls")
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook(storedFile);
            var ws = workbook.Worksheet(1);
            var headerRow = ws.Row(1);
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            var lastCol = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
            for (int i = 1; i <= lastCol; i++)
            {
                var val = headerRow.Cell(i).GetString();
                if (!string.IsNullOrWhiteSpace(val)) headerMap[val.Trim()] = i;
            }

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            for (int i = 2; i <= lastRow; i++)
            {
                var rowData = ws.Row(i);
                var row = new ImportPreviewRowViewModel { RowNumber = i };
                foreach (var col in columns)
                {
                    row.Data[col] = GetExcelValue(rowData, headerMap, col);
                }
                ValidateRow(batch.ModuleName, row);
                rows.Add(row);
            }
        }
        else // CSV
        {
            var lines = System.IO.File.ReadAllLines(storedFile, System.Text.Encoding.UTF8);
            if (lines.Length > 1)
            {
                var headers = ParseCsvLine(lines[0]);
                var headerMap = headers.Select((name, index) => new { name = name.Trim(), index }).ToDictionary(x => x.name, x => x.index, StringComparer.OrdinalIgnoreCase);

                for (var i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var values = ParseCsvLine(lines[i]);
                    var row = new ImportPreviewRowViewModel { RowNumber = i + 1 };
                    foreach (var col in columns)
                    {
                        row.Data[col] = GetCsvValue(values, headerMap, col);
                    }
                    ValidateRow(batch.ModuleName, row);
                    rows.Add(row);
                }
            }
        }

        return new ImportPreviewViewModel
        {
            BatchId = batch.Id,
            ModuleName = batch.ModuleName,
            FileName = batch.FileName,
            TotalRows = rows.Count,
            ValidRows = rows.Count(x => x.CanImport),
            InvalidRows = rows.Count(x => !x.CanImport),
            Rows = rows,
            Columns = columns
        };
    }

    private List<string> GetColumnsForModule(string module)
    {
        return module switch
        {
            "employee" => ["FullName", "Region", "City", "HasLogin", "UserName", "Password", "Role"],
            "account" => ["Code", "AccountType", "DisplayName", "City", "District", "Phone", "Email", "TaxNumber", "Status", "OwnerEmployee"],
            "activity" => ["ActivityDate", "Employee", "Account", "ContactName", "ContactStatus", "OutcomeStatus", "Summary"],
            "sale" => ["SaleDate", "Employee", "Account", "ProductType", "CollectionAmount", "ApeAmount", "LumpSumAmount", "MonthlyPaymentAmount", "PremiumAmount", "ProductionAmount", "SaleCount", "Notes"],
            "expense" => ["ExpenseDate", "Employee", "ExpenseType", "Amount", "Notes"],
            _ => ["DisplayName", "City", "District", "ContactName", "Phone", "Email", "Source", "Status", "Priority", "Note", "AssignedEmployee"]
        };
    }

    private void ValidateRow(string module, ImportPreviewRowViewModel row)
    {
        // General checks
        if (module == "lead" || module == "account")
        {
            if (string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("DisplayName"))) row.Errors.Add("DisplayName zorunlu.");
            if (string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("City"))) row.Errors.Add("City zorunlu.");
        }
        else if (module == "employee")
        {
            if (string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("FullName"))) row.Errors.Add("FullName zorunlu.");
            if (string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("Region"))) row.Errors.Add("Region zorunlu.");
        }
        else if (module == "sale" || module == "expense" || module == "activity")
        {
            if (string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("Employee"))) row.Errors.Add("Personel (Employee) zorunlu.");
        }

        // Add more deep validation logic if needed
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CommitImport(int id)
    {
        var batch = Db.ImportBatches.FirstOrDefault(x => x.Id == id);
        if (batch is null) return NotFound();

        var storedFile = GetStoredFilePath(id);
        if (storedFile is null)
        {
            TempData["Warning"] = "Import dosyasi bulunamadi.";
            return RedirectToAction(nameof(History));
        }

        var preview = BuildBatchPreview(batch, storedFile);
        if (preview.ValidRows == 0)
        {
            TempData["Warning"] = "Ice aktarilabilecek gecerli satir bulunamadi.";
            return RedirectToAction(nameof(Preview), new { id });
        }

        // Module specific commit logic
        try
        {
            switch (batch.ModuleName)
            {
                case "lead": CommitLeads(preview); break;
                case "employee": CommitEmployees(preview); break;
                case "account": CommitAccounts(preview); break;
                case "activity": CommitActivities(preview); break;
                case "sale": CommitSales(preview); break;
                case "expense": CommitExpenses(preview); break;
                default: 
                    TempData["Warning"] = "Gecersiz modul."; 
                    return RedirectToAction(nameof(Upload));
            }

            batch.Status = preview.InvalidRows > 0 ? "ImportedWithWarnings" : "Imported";
            batch.Notes = $"{batch.Notes} | Iceri alinan satir: {preview.ValidRows} | Hatali satir: {preview.InvalidRows}";
            QueueAudit("Import", "Commit", $"IMP-{batch.Id}", $"{preview.ValidRows} {batch.ModuleName} satiri toplu olarak iceri alindi.");
            Db.SaveChanges();

            TempData["Flash"] = $"{preview.ValidRows} {batch.ModuleName} satiri iceri alindi.";
            return RedirectToAction("Index", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(batch.ModuleName) + "s");
        }
        catch (Exception ex)
        {
            TempData["Warning"] = "Veritabani kaydi sirasinda hata olustu: " + ex.Message;
            return RedirectToAction(nameof(Preview), new { id });
        }
    }

    private void CommitLeads(ImportPreviewViewModel preview)
    {
        var statusTypes = Db.LeadStatusTypes.ToList();
        var sourceTypes = Db.LeadSourceTypes.ToList();
        var nextId = (Db.Leads.Max(x => (int?)x.Id) ?? 100) + 1;

        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var assignedEmployeeId = ResolveEmployeeId(row.Data.GetValueOrDefault("AssignedEmployee"));
            var statusTypeId = statusTypes.FirstOrDefault(x => string.Equals(x.Code, row.Data.GetValueOrDefault("Status"), StringComparison.OrdinalIgnoreCase))?.Id 
                               ?? statusTypes.FirstOrDefault(x => x.Code == "NEW")?.Id ?? 1;
            var sourceTypeId = sourceTypes.FirstOrDefault(x => string.Equals(x.Code, row.Data.GetValueOrDefault("Source"), StringComparison.OrdinalIgnoreCase))?.Id
                               ?? sourceTypes.FirstOrDefault(x => x.Code == "CALL_CENTER")?.Id ?? 1;

            var lead = new Lead
            {
                Id = nextId++,
                Code = $"LD-{nextId}",
                DisplayName = row.Data.GetValueOrDefault("DisplayName", ""),
                City = row.Data.GetValueOrDefault("City", ""),
                District = row.Data.GetValueOrDefault("District", ""),
                ContactName = row.Data.GetValueOrDefault("ContactName", ""),
                Phone = row.Data.GetValueOrDefault("Phone", ""),
                Email = row.Data.GetValueOrDefault("Email", ""),
                LeadSourceTypeId = sourceTypeId,
                LeadStatusTypeId = statusTypeId,
                Priority = Enum.TryParse<LeadPriority>(row.Data.GetValueOrDefault("Priority"), true, out var p) ? p : LeadPriority.Medium,
                Note = row.Data.GetValueOrDefault("Note", ""),
                AssignedEmployeeId = assignedEmployeeId
            };
            Db.Leads.Add(lead);
        }
    }

    private void CommitEmployees(ImportPreviewViewModel preview)
    {
        var nextId = (Db.Employees.Max(x => (int?)x.Id) ?? 10) + 1;
        var nextUserId = (Db.Users.Max(x => (int?)x.Id) ?? 10) + 1;

        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var emp = new Employee
            {
                Id = nextId++,
                FullName = row.Data.GetValueOrDefault("FullName", ""),
                Region = row.Data.GetValueOrDefault("Region", ""),
                City = row.Data.GetValueOrDefault("City", ""),
                IsActive = true
            };
            Db.Employees.Add(emp);

            if (string.Equals(row.Data.GetValueOrDefault("HasLogin"), "true", StringComparison.OrdinalIgnoreCase) || 
                !string.IsNullOrWhiteSpace(row.Data.GetValueOrDefault("UserName")))
            {
                var roleStr = row.Data.GetValueOrDefault("Role", "FieldSales");
                var user = new UserAccount
                {
                    Id = nextUserId++,
                    EmployeeId = emp.Id,
                    FullName = emp.FullName,
                    UserName = row.Data.GetValueOrDefault("UserName", emp.FullName.Replace(" ", ".").ToLower()),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(row.Data.GetValueOrDefault("Password", "123456")),
                    Role = Enum.TryParse<RoleType>(roleStr, true, out var r) ? r : RoleType.FieldSales
                };
                Db.Users.Add(user);
            }
        }
    }

    private void CommitAccounts(ImportPreviewViewModel preview)
    {
        var nextId = (Db.Accounts.Max(x => (int?)x.Id) ?? 100) + 1;
        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var account = new Account
            {
                Id = nextId++,
                Code = row.Data.GetValueOrDefault("Code", $"ACC-{nextId}"),
                AccountType = Enum.TryParse<AccountType>(row.Data.GetValueOrDefault("AccountType"), true, out var at) ? at : AccountType.Corporate,
                DisplayName = row.Data.GetValueOrDefault("DisplayName", ""),
                City = row.Data.GetValueOrDefault("City", ""),
                District = row.Data.GetValueOrDefault("District"),
                Phone = row.Data.GetValueOrDefault("Phone"),
                Email = row.Data.GetValueOrDefault("Email"),
                TaxNumber = row.Data.GetValueOrDefault("TaxNumber"),
                Status = row.Data.GetValueOrDefault("Status", "Active"),
                OwnerEmployeeId = ResolveEmployeeId(row.Data.GetValueOrDefault("OwnerEmployee")),
                Notes = row.Data.GetValueOrDefault("Notes", "")
            };
            Db.Accounts.Add(account);
        }
    }

    private void CommitActivities(ImportPreviewViewModel preview)
    {
        var nextId = (Db.Activities.Max(x => (int?)x.Id) ?? 100) + 1;
        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var activity = new Activity
            {
                Id = nextId++,
                Code = $"ACT-{nextId}",
                ActivityDate = DateTime.TryParse(row.Data.GetValueOrDefault("ActivityDate"), out var dt) ? dt : DateTime.Now,
                EmployeeId = ResolveEmployeeId(row.Data.GetValueOrDefault("Employee")) ?? 0,
                AccountId = ResolveAccountId(row.Data.GetValueOrDefault("Account")) ?? 0,
                ContactName = row.Data.GetValueOrDefault("ContactName", ""),
                ContactStatusTypeId = 1, // Default
                Summary = row.Data.GetValueOrDefault("Summary", "")
            };
            Db.Activities.Add(activity);
        }
    }

    private void CommitSales(ImportPreviewViewModel preview)
    {
        var nextId = (Db.Sales.Max(x => (int?)x.Id) ?? 100) + 1;
        var pTypes = Db.InsuranceProductTypes.ToList();

        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var sale = new Sale
            {
                Id = nextId++,
                Code = $"SLS-{nextId}",
                SaleDate = DateTime.TryParse(row.Data.GetValueOrDefault("SaleDate"), out var dt) ? dt : DateTime.Now,
                EmployeeId = ResolveEmployeeId(row.Data.GetValueOrDefault("Employee")) ?? 0,
                AccountId = ResolveAccountId(row.Data.GetValueOrDefault("Account")) ?? 0,
                ProductTypeId = pTypes.FirstOrDefault(x => string.Equals(x.Name, row.Data.GetValueOrDefault("ProductType"), StringComparison.OrdinalIgnoreCase))?.Id ?? 1,
                CollectionAmount = TryParseDecimal(row.Data.GetValueOrDefault("CollectionAmount")),
                ApeAmount = TryParseDecimal(row.Data.GetValueOrDefault("ApeAmount")),
                ProductionAmount = TryParseDecimal(row.Data.GetValueOrDefault("ProductionAmount")),
                SaleAmount = TryParseDecimal(row.Data.GetValueOrDefault("SaleAmount")),
                SaleCount = int.TryParse(row.Data.GetValueOrDefault("SaleCount"), out var sc) ? sc : 1,
                Notes = row.Data.GetValueOrDefault("Notes", "")
            };
            Db.Sales.Add(sale);
        }
    }

    private void CommitExpenses(ImportPreviewViewModel preview)
    {
        var nextId = (Db.Expenses.Max(x => (int?)x.Id) ?? 100) + 1;
        var eTypes = Db.ExpenseTypes.ToList();

        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var expense = new Expense
            {
                Id = nextId++,
                Code = $"EXP-{nextId}",
                ExpenseDate = DateTime.TryParse(row.Data.GetValueOrDefault("ExpenseDate"), out var dt) ? dt : DateTime.Now,
                EmployeeId = ResolveEmployeeId(row.Data.GetValueOrDefault("Employee")) ?? 0,
                ExpenseTypeId = eTypes.FirstOrDefault(x => string.Equals(x.Name, row.Data.GetValueOrDefault("ExpenseType"), StringComparison.OrdinalIgnoreCase))?.Id ?? 1,
                Amount = TryParseDecimal(row.Data.GetValueOrDefault("Amount")) ?? 0,
                Notes = row.Data.GetValueOrDefault("Notes", "")
            };
            Db.Expenses.Add(expense);
        }
    }

    private int? ResolveEmployeeId(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return Db.Employees.FirstOrDefault(x => x.FullName.ToLower() == name.Trim().ToLower())?.Id;
    }

    private int? ResolveAccountId(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return Db.Accounts.FirstOrDefault(x => x.DisplayName.ToLower() == name.Trim().ToLower())?.Id;
    }

    private static decimal? TryParseDecimal(string? val)
    {
        if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)) return result;
        if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out result)) return result;
        return null;
    }

    private void ValidateFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(ImportUploadViewModel.File), "Yuklenecek dosya secilmelidir.");
            return;
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(ImportUploadViewModel.File), "Yalnizca .xlsx, .xls veya .csv dosyalari yuklenebilir.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            ModelState.AddModelError(nameof(ImportUploadViewModel.File), "Dosya boyutu 15 MB sinirini asmamalidir.");
        }
    }

    private string GetImportDirectory()
    {
        var directory = Path.Combine(_environment.ContentRootPath, "App_Data", "Imports");
        Directory.CreateDirectory(directory);
        return directory;
    }

    private string? GetStoredFilePath(int batchId)
    {
        var importDirectory = GetImportDirectory();
        return Directory
            .GetFiles(importDirectory, $"{batchId:D6}_*")
            .OrderByDescending(System.IO.File.GetLastWriteTimeUtc)
            .FirstOrDefault();
    }

    private static string BuildImportNotes(string notes, long fileSize, string storedFileName)
    {
        var details = $"Yuklenen dosya: {storedFileName} | Boyut: {fileSize} byte";
        return string.IsNullOrWhiteSpace(notes) ? details : $"{notes} | {details}";
    }

    private static string GetExcelValue(ClosedXML.Excel.IXLRow row, Dictionary<string, int> headerMap, string col)
    {
        if (headerMap.TryGetValue(col, out var index))
        {
            return row.Cell(index).GetString()?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }

    private static string GetCsvValue(List<string> values, Dictionary<string, int> headerMap, string columnName)
    {
        if (!headerMap.TryGetValue(columnName, out var index) || index >= values.Count)
        {
            return string.Empty;
        }

        return values[index].Trim();
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }

        result.Add(current.ToString());
        return result;
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }
}
