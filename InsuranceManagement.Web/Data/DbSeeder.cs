using InsuranceManagement.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Ensure all required LeadStatusTypes exist
        var requiredLeadStatuses = new List<LeadStatusType>
        {
            new LeadStatusType { Id = 1, Code = "NEW", Name = "Yeni", DisplayOrder = 1 },
            new LeadStatusType { Id = 6, Code = "RESEARCHED", Name = "Araştırılmış", DisplayOrder = 2 },
            new LeadStatusType { Id = 7, Code = "CONTACT_FOUND", Name = "İletişim Bulundu", DisplayOrder = 3 },
            new LeadStatusType { Id = 2, Code = "READY_FOR_ASSIGNMENT", Name = "Atamaya Hazır", DisplayOrder = 4 },
            new LeadStatusType { Id = 3, Code = "ASSIGNED", Name = "Atandı", DisplayOrder = 5 },
            new LeadStatusType { Id = 8, Code = "VISIT_SCHEDULED", Name = "Ziyaret Planlandı", DisplayOrder = 6 },
            new LeadStatusType { Id = 4, Code = "VISITED", Name = "Ziyaret Edildi", DisplayOrder = 7 },
            new LeadStatusType { Id = 5, Code = "CONVERTED_TO_ACTIVITY", Name = "Aktiviteye Dönüştürüldü", DisplayOrder = 8 },
            new LeadStatusType { Id = 9, Code = "DISQUALIFIED", Name = "Elenmiş", DisplayOrder = 9 }
        };

        var existingStatusCodes = await db.LeadStatusTypes.Select(s => s.Code).ToListAsync();
        foreach (var status in requiredLeadStatuses)
        {
            if (!existingStatusCodes.Contains(status.Code))
            {
                db.LeadStatusTypes.Add(status);
            }
        }

        if (!db.LeadSourceTypes.Any())
        {
            db.LeadSourceTypes.AddRange([
                new LeadSourceType { Id = 1, Code = "CALL_CENTER", Name = "Çağrı Merkezi", DisplayOrder = 1 },
                new LeadSourceType { Id = 2, Code = "MANUAL", Name = "Manuel", DisplayOrder = 2 },
                new LeadSourceType { Id = 3, Code = "REFERRAL", Name = "Referans", DisplayOrder = 3 }
            ]);
        }
        
        await db.SaveChangesAsync();

        // 1. CLEAR OLD MIGRATION PATCHES (Not needed for fresh DB)
        
        if (db.Users.Any())
        {
            return;
        }

        db.ActivityContactStatusTypes.AddRange([
            new ActivityContactStatusType { Id = 1, Code = "CONTACTED", Name = "Görüşüldü", DisplayOrder = 1 },
            new ActivityContactStatusType { Id = 2, Code = "NOT_CONTACTED", Name = "Görüşülemedi", DisplayOrder = 2 },
            new ActivityContactStatusType { Id = 3, Code = "PLANNED", Name = "Planlandı / Randevu", DisplayOrder = 0 }
        ]);

        db.ActivityOutcomeStatusTypes.AddRange([
            new ActivityOutcomeStatusType { Id = 1, Code = "NOT_APPLICABLE", Name = "Uygulanamaz", DisplayOrder = 1 },
            new ActivityOutcomeStatusType { Id = 2, Code = "POSITIVE", Name = "Olumlu", DisplayOrder = 2 },
            new ActivityOutcomeStatusType { Id = 3, Code = "NEGATIVE", Name = "Olumsuz", DisplayOrder = 3 },
            new ActivityOutcomeStatusType { Id = 4, Code = "POSTPONED", Name = "Ertelendi", DisplayOrder = 4 },
            new ActivityOutcomeStatusType { Id = 5, Code = "SALE_CLOSED", Name = "Satış Oldu", DisplayOrder = 5 }
        ]);

        db.InsuranceProductTypes.AddRange([
            new InsuranceProductType { Id = 1, Code = "BES", Name = "BES (Bireysel Emeklilik)", DisplayOrder = 1 },
            new InsuranceProductType { Id = 2, Code = "LIFE", Name = "Hayat Sigortası", DisplayOrder = 2 },
            new InsuranceProductType { Id = 3, Code = "HEALTH", Name = "Sağlık Sigortası", DisplayOrder = 3 },
            new InsuranceProductType { Id = 4, Code = "TRAVEL", Name = "Seyahat Sigortası", DisplayOrder = 4 },
            new InsuranceProductType { Id = 5, Code = "OTHER", Name = "Diğer", DisplayOrder = 5 }
        ]);

        db.ExpenseTypes.AddRange([
            new ExpenseReferenceType { Id = 1, Code = "TRAVEL", Name = "Yol/Ulaşım", DisplayOrder = 1 },
            new ExpenseReferenceType { Id = 2, Code = "MEAL", Name = "Yemek", DisplayOrder = 2 },
            new ExpenseReferenceType { Id = 3, Code = "ACCOMMODATION", Name = "Konaklama", DisplayOrder = 3 },
            new ExpenseReferenceType { Id = 4, Code = "OTHER", Name = "Diğer", DisplayOrder = 4 }
        ]);

        await db.SaveChangesAsync();

        // 2. SEED SYSTEM USERS & BASE EMPLOYEES
        db.Employees.AddRange(
        [
            new Employee { Id = 1, FullName = "Saha Personeli 1", Region = "Istanbul", City = "Istanbul" },
            new Employee { Id = 2, FullName = "Saha Personeli 2", Region = "Ankara", City = "Ankara" }
        ]);

        db.Users.AddRange(
        [
            new UserAccount { Id = 1, UserName = "insurance_admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("insurance_123!admin"), FullName = "Sistem Yoneticisi", Role = RoleType.Admin },
            new UserAccount { Id = 2, UserName = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Genel Yonetici", Role = RoleType.Manager },
            new UserAccount { Id = 3, UserName = "sales", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Satis Muduru", Role = RoleType.SalesManager },
            new UserAccount { Id = 4, UserName = "ops", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Operasyon Uzmani", Role = RoleType.Operations },
            new UserAccount { Id = 5, UserName = "field1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Saha Personeli 1", Role = RoleType.FieldSales, EmployeeId = 1 },
            new UserAccount { Id = 6, UserName = "field2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Saha Personeli 2", Role = RoleType.FieldSales, EmployeeId = 2 },
            new UserAccount { Id = 7, UserName = "call", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), FullName = "Call Center Kullanici", Role = RoleType.CallCenter }
        ]);

        await db.SaveChangesAsync();

        // 3. SEED ROLE PERMISSIONS - VERBOSE & DEFENSIVE
        var modules = new[] {
            new { Key = "Dashboard", Name = "Dashboard", Icon = "📊" },
            new { Key = "Leads", Name = "Lead Havuzu", Icon = "🎯" },
            new { Key = "Accounts", Name = "Musteriler", Icon = "🏢" },
            new { Key = "Activities", Name = "Aktiviteler", Icon = "📅" },
            new { Key = "Sales", Name = "Satislar", Icon = "💰" },
            new { Key = "Expenses", Name = "Masraflar", Icon = "🧾" },
            new { Key = "Employees", Name = "Personeller", Icon = "👥" },
            new { Key = "Admin", Name = "Yonetim", Icon = "⚙️" },
            new { Key = "Imports", Name = "Veri Import", Icon = "📥" }
        };

        int addedCount = 0;
        var existingRoles = db.RolePermissions.Select(x => x.Role).Distinct().ToList();
        
        foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
        {
            string roleStr = role.ToString();
            
            foreach (var mod in modules)
            {
                // Strict check: Try both enum and string logic to be safe
                bool exists = db.RolePermissions.Any(x => x.Role == role && x.ModuleKey == mod.Key);
                
                if (!exists)
                {
                    var isAllowed = true;
                    var access = "Tam Yetki";
                    var tooltip = "Bu modüle tam erişim yetkisi.";

                    // Logic using string for robustness
                    if (roleStr == "Admin" || roleStr == "SystemSpecialist") 
                    { 
                        // Default full for system roles
                    }
                    else if (mod.Key == "Admin") { isAllowed = false; access = "Yasakli"; }
                    
                    if (roleStr == "SystemSpecialist" && (mod.Key == "Leads" || mod.Key == "Activities" || mod.Key == "Sales" || mod.Key == "Expenses" || mod.Key == "Employees" || mod.Key == "Accounts"))
                    {
                        access = "Izleme";
                        tooltip = "Sistem denetimi için sadece izleme yetkisi.";
                    }

                    db.RolePermissions.Add(new RolePermission
                    {
                        Role = role,
                        ModuleKey = mod.Key,
                        ModuleName = mod.Name,
                        Icon = mod.Icon,
                        AccessLevel = access,
                        IsAllowed = isAllowed,
                        Tooltip = tooltip
                    });
                    addedCount++;
                }
            }
        }

        if (addedCount > 0)
        {
            await db.SaveChangesAsync();
        }
    }
}
