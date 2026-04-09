using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Operations")]
public class ImportsController : AppController
{
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
            FileName = originalFileName,
            ImportedAt = DateTime.Now,
            ImportedBy = User.Identity?.Name ?? "unknown",
            Status = string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase) ? "PreviewReady" : "Uploaded",
            Notes = BuildImportNotes(model.Notes, model.File.Length, safeFileName)
        };
        Db.ImportBatches.Add(batch);
        QueueAudit("Import", "Create", $"IMP-{batch.Id}", $"{batch.FileName} dosyasi yuklendi.");

        Db.SaveChanges();
        TempData["Flash"] = "Import dosyasi yuklendi ve batch kaydi olusturuldu.";
        if (string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(PreviewLeadImport), new { id = batch.Id });
        }

        TempData["Warning"] = "Excel formatlari yuklenir, ancak toplu onizleme ve iceri alma su an yalnizca CSV sablonu ile aktif.";
        return RedirectToAction(nameof(History));
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
        if (normalizedModule != "lead")
        {
            TempData["Warning"] = "Su an yalnizca lead import sablonu hazir.";
            return RedirectToAction(nameof(Upload));
        }

        var csv = string.Join(Environment.NewLine,
        [
            "DisplayName,City,District,ContactName,Phone,Email,Source,Status,Priority,Note,AssignedEmployee",
            "\"Acme Dis Ticaret\",\"Istanbul\",\"Besiktas\",\"Merve Karahan\",\"05332223344\",\"merve@acme.com\",\"Call Center\",\"ReadyForAssignment\",\"High\",\"IK muduru kontagi alindi\",\"\"",
            "\"Nova Lojistik\",\"Kocaeli\",\"Izmit\",\"Sinem Yalcin\",\"05328887766\",\"sinem@nova.com\",\"Referral\",\"New\",\"Medium\",\"Ilk geri donus bekleniyor\",\"\""
        ]);

        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", "lead_import_template.csv");
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
    public IActionResult PreviewLeadImport(int id)
    {
        BuildShell();
        var batch = Db.ImportBatches.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (batch is null)
        {
            return NotFound();
        }

        var storedFile = GetStoredFilePath(id);
        if (storedFile is null)
        {
            TempData["Warning"] = "Import dosyasi bulunamadi.";
            return RedirectToAction(nameof(History));
        }

        if (!string.Equals(Path.GetExtension(storedFile), ".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "Onizleme ve toplu import su an yalnizca CSV dosyalari icin destekleniyor.";
            return RedirectToAction(nameof(History));
        }

        var preview = BuildLeadPreview(batch, storedFile);
        return View(preview);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CommitLeadImport(int id)
    {
        var batch = Db.ImportBatches.FirstOrDefault(x => x.Id == id);
        if (batch is null)
        {
            return NotFound();
        }

        var storedFile = GetStoredFilePath(id);
        if (storedFile is null)
        {
            TempData["Warning"] = "Import dosyasi bulunamadi.";
            return RedirectToAction(nameof(History));
        }

        var preview = BuildLeadPreview(batch, storedFile);
        if (preview.ValidRows == 0)
        {
            TempData["Warning"] = "Ice aktarilabilecek gecerli satir bulunamadi.";
            return RedirectToAction(nameof(PreviewLeadImport), new { id });
        }

        var nextId = (Db.Leads.Max(x => (int?)x.Id) ?? 100) + 1;
        foreach (var row in preview.Rows.Where(x => x.CanImport))
        {
            var assignedEmployeeId = ResolveEmployeeId(row.AssignedEmployee);
            var status = Enum.Parse<LeadStatus>(row.StatusText, true);
            var lead = new Lead
            {
                Id = nextId,
                Code = $"LD-{nextId}",
                DisplayName = row.DisplayName,
                City = row.City,
                District = row.District,
                ContactName = row.ContactName,
                Phone = row.Phone,
                Email = row.Email,
                Source = string.IsNullOrWhiteSpace(row.Source) ? "Call Center" : row.Source,
                Status = status,
                Priority = string.IsNullOrWhiteSpace(row.Priority) ? "Medium" : row.Priority,
                Note = row.Note,
                AssignedEmployeeId = assignedEmployeeId,
                CreatedAt = DateTime.Today
            };

            Db.Leads.Add(lead);
            nextId++;
        }

        batch.Status = preview.InvalidRows > 0 ? "ImportedWithWarnings" : "Imported";
        batch.Notes = $"{batch.Notes} | Iceri alinan satir: {preview.ValidRows} | Hatali satir: {preview.InvalidRows}";
        QueueAudit("Import", "Commit", $"IMP-{batch.Id}", $"{preview.ValidRows} lead satiri toplu olarak iceri alindi.");
        Db.SaveChanges();

        TempData["Flash"] = $"{preview.ValidRows} lead satiri iceri alindi.";
        if (preview.InvalidRows > 0)
        {
            TempData["Warning"] = $"{preview.InvalidRows} satir hata nedeniyle atlandi.";
        }

        return RedirectToAction("Index", "Leads");
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

    private LeadImportPreviewViewModel BuildLeadPreview(ImportBatch batch, string storedFile)
    {
        var lines = System.IO.File.ReadAllLines(storedFile, System.Text.Encoding.UTF8);
        var rows = new List<LeadImportPreviewRowViewModel>();
        if (lines.Length <= 1)
        {
            return new LeadImportPreviewViewModel
            {
                BatchId = batch.Id,
                FileName = batch.FileName
            };
        }

        var headers = ParseCsvLine(lines[0]);
        var headerMap = headers
            .Select((name, index) => new { name = name.Trim(), index })
            .ToDictionary(x => x.name, x => x.index, StringComparer.OrdinalIgnoreCase);

        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            var values = ParseCsvLine(lines[i]);
            var row = new LeadImportPreviewRowViewModel
            {
                RowNumber = i + 1,
                DisplayName = GetCsvValue(values, headerMap, "DisplayName"),
                City = GetCsvValue(values, headerMap, "City"),
                District = GetCsvValue(values, headerMap, "District"),
                ContactName = GetCsvValue(values, headerMap, "ContactName"),
                Phone = GetCsvValue(values, headerMap, "Phone"),
                Email = GetCsvValue(values, headerMap, "Email"),
                Source = GetCsvValue(values, headerMap, "Source"),
                StatusText = GetCsvValue(values, headerMap, "Status"),
                Priority = GetCsvValue(values, headerMap, "Priority"),
                Note = GetCsvValue(values, headerMap, "Note"),
                AssignedEmployee = GetCsvValue(values, headerMap, "AssignedEmployee")
            };

            ValidateLeadPreviewRow(row);
            rows.Add(row);
        }

        return new LeadImportPreviewViewModel
        {
            BatchId = batch.Id,
            FileName = batch.FileName,
            TotalRows = rows.Count,
            ValidRows = rows.Count(x => x.CanImport),
            InvalidRows = rows.Count(x => !x.CanImport),
            Rows = rows
        };
    }

    private void ValidateLeadPreviewRow(LeadImportPreviewRowViewModel row)
    {
        if (string.IsNullOrWhiteSpace(row.DisplayName))
        {
            row.Errors.Add("DisplayName zorunlu.");
        }

        if (string.IsNullOrWhiteSpace(row.City))
        {
            row.Errors.Add("City zorunlu.");
        }

        if (string.IsNullOrWhiteSpace(row.Phone) && string.IsNullOrWhiteSpace(row.Email))
        {
            row.Errors.Add("Phone veya Email alanlarindan en az biri dolu olmali.");
        }

        if (string.IsNullOrWhiteSpace(row.StatusText))
        {
            row.StatusText = LeadStatus.New.ToString();
        }
        else if (!Enum.TryParse<LeadStatus>(row.StatusText, true, out _))
        {
            row.Errors.Add("Status alani gecersiz.");
        }

        if (!string.IsNullOrWhiteSpace(row.AssignedEmployee) && !ResolveEmployeeId(row.AssignedEmployee).HasValue)
        {
            row.Errors.Add("AssignedEmployee sistemde bulunamadi.");
        }

        if (!string.IsNullOrWhiteSpace(row.Phone))
        {
            if (Db.Leads.Any(x => x.Phone == row.Phone))
            {
                row.Errors.Add("Bu telefon ile mevcut bir lead kaydi var.");
            }
            else if (Db.Accounts.Any(x => x.Phone == row.Phone))
            {
                row.Warnings.Add("Bu telefon mevcut musteri kayitlarinda da bulunuyor.");
            }
        }

        if (!string.IsNullOrWhiteSpace(row.Email))
        {
            if (Db.Leads.Any(x => x.Email == row.Email))
            {
                row.Errors.Add("Bu e-posta ile mevcut bir lead kaydi var.");
            }
            else if (Db.Accounts.Any(x => x.Email == row.Email))
            {
                row.Warnings.Add("Bu e-posta mevcut musteri kayitlarinda da bulunuyor.");
            }
        }

        if (!string.IsNullOrWhiteSpace(row.DisplayName) && Db.Leads.Any(x => x.DisplayName == row.DisplayName))
        {
            row.Warnings.Add("Ayni isimle mevcut lead kaydi bulunuyor.");
        }
    }

    private int? ResolveEmployeeId(string employeeName)
    {
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            return null;
        }

        var employee = Db.Employees.FirstOrDefault(x => x.FullName.ToLower() == employeeName.Trim().ToLower());
        return employee?.Id;
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
