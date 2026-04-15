using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : AppController
{
    public AdminController(AppDbContext db) : base(db)
    {
    }

    public IActionResult Users()
    {
        BuildShell();
        return View(Db.Users.ToList());
    }

    public IActionResult AddUser()
    {
        BuildShell();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(string fullName, string userName, string password)
    {
        if (Db.Users.Any(x => x.UserName == userName))
        {
            ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten alınmış.");
            BuildShell();
            return View();
        }

        var user = new UserAccount
        {
            FullName = fullName,
            UserName = userName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = RoleType.SystemSpecialist, // Automatically assign system role
            EmployeeId = null // Never linked to any personnel in this flow
        };

        Db.Users.Add(user);
        Db.SaveChanges();

        TempData["Success"] = "Yeni kullanıcı başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Users));
    }

    public IActionResult EditUser(int id)
    {
        BuildShell();
        var user = Db.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditUser(int id, string fullName, string userName, InsuranceManagement.Web.Domain.RoleType role, string? password)
    {
        var user = Db.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) return NotFound();

        user.FullName = fullName;
        user.UserName = userName;
        user.Role = role;

        if (!string.IsNullOrWhiteSpace(password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        Db.SaveChanges();
        TempData["Success"] = "Kullanıcı başarıyla güncellendi.";
        return RedirectToAction(nameof(Users));
    }

    public IActionResult Roles()
    {
        BuildShell();
        var permissions = Db.RolePermissions.ToList();
        
        // --- DIAGNOSTIC LOGGING ---
        Console.WriteLine("--- YETKI TEŞHİS RAPORU ---");
        Console.WriteLine($"Toplam Yetki Kaydı: {permissions.Count}");
        foreach (var role in Enum.GetValues<RoleType>())
        {
            var count = permissions.Count(x => x.Role == role);
            Console.WriteLine($"- {role}: {count} yetki");
        }
        Console.WriteLine("---------------------------");

        return View(permissions);
    }

    [HttpPost]
    public async Task<IActionResult> SyncPermissions()
    {
        try 
        {
            Console.WriteLine(">>> [SYNC] MANUEL ONARIM BASLADI <<<");
            
            // 1. Temizlik
            var all = Db.RolePermissions.ToList();
            Db.RolePermissions.RemoveRange(all);
            await Db.SaveChangesAsync();
            Console.WriteLine($">>> [SYNC] {all.Count} eski kayit silindi.");

            // 2. Moduller (Lucide Icon Isimleri ile)
            var modules = new[] {
                new { Key = "Dashboard", Name = "Dashboard", Icon = "layout-dashboard" },
                new { Key = "Leads", Name = "Lead Havuzu", Icon = "target" },
                new { Key = "Accounts", Name = "Musteriler", Icon = "building-2" },
                new { Key = "Activities", Name = "Aktiviteler", Icon = "calendar-days" },
                new { Key = "Sales", Name = "Satislar", Icon = "banknote" },
                new { Key = "Expenses", Name = "Masraflar", Icon = "receipt" },
                new { Key = "Employees", Name = "Personeller", Icon = "users" },
                new { Key = "Admin", Name = "Yonetim", Icon = "settings" },
                new { Key = "Imports", Name = "Veri Import", Icon = "file-up" }
            };

            // 3. Yazma
            int totalAdded = 0;
            foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
            {
                foreach (var mod in modules)
                {
                    var isAllowed = true;
                    var access = "Tam Yetki";
                    if (role != RoleType.Admin && role != RoleType.SystemSpecialist && mod.Key == "Admin")
                    {
                        isAllowed = false;
                        access = "Yasakli";
                    }

                    if (role == RoleType.SystemSpecialist && (mod.Key != "Dashboard" && mod.Key != "Admin"))
                    {
                        access = "Izleme";
                    }

                    Db.RolePermissions.Add(new RolePermission
                    {
                        Role = role,
                        ModuleKey = mod.Key,
                        ModuleName = mod.Name,
                        Icon = mod.Icon,
                        AccessLevel = access,
                        IsAllowed = isAllowed,
                        Tooltip = GetStandardTooltip(access)
                    });
                    totalAdded++;
                }
            }

            await Db.SaveChangesAsync();
            Console.WriteLine($">>> [SYNC] ISLEM TAMAM: {totalAdded} yetki olusturuldu!");
            TempData["Success"] = $"İşlem Başarılı: {totalAdded} yetki kaydı sıfırdan oluşturuldu.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> [SYNC-HATA] !!! {ex.Message} !!!");
            TempData["Error"] = "Onarım başarısız: " + ex.Message;
        }
        
        return RedirectToAction(nameof(Roles));
    }

    private string GetStandardTooltip(string accessLevel)
    {
        return accessLevel switch
        {
            "Tam Yetki" => "Kayıt ekleme, silme ve düzenleme dahil tüm yetkilere sahiptir.",
            "Sadece Izleme" => "Kayıtları görebilir ancak hiçbir değişiklik yapamaz.",
            "Izleme" => "Sistem denetimi için sadece izleme yetkisi.",
            "Kendi Kayitlari" => "Sadece kendi oluşturduğu veya kendisine atanan kayıtları yönetebilir.",
            "Ekip Kayitlari" => "Kendi ekibine ait tüm kayıtları görebilir ve yönetebilir.",
            "Hizli Giris" => "Kısıtlı yetki ile sadece hızlı veri girişi yapabilir.",
            "Veri Analitigi" => "Modül verilerini analiz etme ve raporlama yetkisi.",
            "Dosya Yukleme" => "Sadece belge ve dosya ekleme işlemlerini yapabilir.",
            "Yasakli" => "Bu modüle giriş yetkisi tamamen kapatılmıştır.",
            _ => "Bu modül için özel yetki seviyesi."
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateRolePermissions(List<PermissionUpdateModel> updates)
    {
        if (updates == null || !updates.Any()) return RedirectToAction(nameof(Roles));

        foreach (var update in updates)
        {
            var permission = Db.RolePermissions.FirstOrDefault(x => x.Id == update.Id);
            if (permission != null)
            {
                permission.IsAllowed = update.IsAllowed;
                permission.AccessLevel = update.AccessLevel;
                permission.Tooltip = string.IsNullOrWhiteSpace(update.Tooltip) 
                                     ? GetStandardTooltip(update.AccessLevel) 
                                     : update.Tooltip;
            }
        }

        Db.SaveChanges();
        TempData["Success"] = "Tum yetkiler başarıyla kaydedildi.";
        return RedirectToAction(nameof(Roles));
    }

    public class PermissionUpdateModel
    {
        public int Id { get; set; }
        public bool IsAllowed { get; set; }
        public string AccessLevel { get; set; } = string.Empty;
        public string? Tooltip { get; set; }
    }

    public IActionResult ReferenceData()
    {
        BuildShell();
        return View();
    }

    public IActionResult Audit()
    {
        BuildShell();
        return View(Db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(200).ToList());
    }
}
