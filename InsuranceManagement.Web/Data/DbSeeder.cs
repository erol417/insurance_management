using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any())
        {
            return;
        }

        // 1. SEED REFERENCE TABLES FIRST
        db.LeadStatusTypes.AddRange([
            new LeadStatusType { Id = 1, Code = "NEW", Name = "Yeni", DisplayOrder = 1 },
            new LeadStatusType { Id = 2, Code = "READY_FOR_ASSIGNMENT", Name = "Atamaya Hazır", DisplayOrder = 2 },
            new LeadStatusType { Id = 3, Code = "ASSIGNED", Name = "Atandı", DisplayOrder = 3 },
            new LeadStatusType { Id = 4, Code = "VISITED", Name = "Ziyaret Edildi", DisplayOrder = 4 },
            new LeadStatusType { Id = 5, Code = "CONVERTED_TO_ACTIVITY", Name = "Aktiviteye Dönüştürüldü", DisplayOrder = 5 },
            new LeadStatusType { Id = 9, Code = "DISQUALIFIED", Name = "Elenmiş", DisplayOrder = 9 }
        ]);

        db.LeadSourceTypes.AddRange([
            new LeadSourceType { Id = 1, Code = "CALL_CENTER", Name = "Çağrı Merkezi", DisplayOrder = 1 },
            new LeadSourceType { Id = 2, Code = "MANUAL", Name = "Manuel", DisplayOrder = 2 },
            new LeadSourceType { Id = 3, Code = "REFERRAL", Name = "Referans", DisplayOrder = 3 }
        ]);

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

        await db.SaveChangesAsync(); // Save references to get IDs if not explicit (they are explicit here)

        // 2. SEED TRANSACTION DATA
        db.Employees.AddRange(
        [
            new Employee { Id = 1, FullName = "Ahmet Yilmaz", Region = "Istanbul Europe", City = "Istanbul" },
            new Employee { Id = 2, FullName = "Elif Kara", Region = "Istanbul Anatolia", City = "Istanbul" },
            new Employee { Id = 3, FullName = "Murat Demir", Region = "Kocaeli", City = "Kocaeli" },
            new Employee { Id = 4, FullName = "Zeynep Acar", Region = "Bursa", City = "Bursa" }
        ]);

        db.Users.AddRange(
        [
            new UserAccount { Id = 1, UserName = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Sistem Yoneticisi", Role = RoleType.Admin },
            new UserAccount { Id = 2, UserName = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Genel Yonetici", Role = RoleType.Manager },
            new UserAccount { Id = 3, UserName = "sales", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Satis Muduru", Role = RoleType.SalesManager },
            new UserAccount { Id = 4, UserName = "ops", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Operasyon Uzmani", Role = RoleType.Operations },
            new UserAccount { Id = 5, UserName = "field1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Ahmet Yilmaz", Role = RoleType.FieldSales, EmployeeId = 1 },
            new UserAccount { Id = 6, UserName = "field2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Elif Kara", Role = RoleType.FieldSales, EmployeeId = 2 },
            new UserAccount { Id = 7, UserName = "call", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), FullName = "Call Center Kullanici", Role = RoleType.CallCenter }
        ]);

        db.Accounts.AddRange(
        [
            new Account { Id = 101, Code = "ACC-101", AccountType = AccountType.Corporate, DisplayName = "Acme Dis Ticaret A.S.", City = "Istanbul", District = "Besiktas", Phone = "0212 111 11 11", Email = "ik@acme.com", TaxNumber = "1234567890", OwnerEmployeeId = 1, Notes = "Kurumsal BES potansiyeli.", Status = "Active" },
            new Account { Id = 102, Code = "ACC-102", AccountType = AccountType.Corporate, DisplayName = "Nova Lojistik", City = "Kocaeli", District = "Izmit", Phone = "0262 222 22 22", Email = "info@nova.com", TaxNumber = "9876543210", OwnerEmployeeId = 3, Notes = "Hayat ve saglik urunleri icin acik firsat.", Status = "Active" },
            new Account { Id = 103, Code = "ACC-103", AccountType = AccountType.Individual, DisplayName = "Burcu Cetin", City = "Istanbul", District = "Kadikoy", Phone = "0532 555 11 22", Email = "burcu@example.com", OwnerEmployeeId = 2, Notes = "Bireysel hayat urunu ilgisi.", Status = "Active" }
        ]);

        db.Leads.AddRange(
        [
            new Lead { Id = 101, Code = "LD-101", DisplayName = "A Plus Holding", City = "Istanbul", District = "Sisli", ContactName = "Gizem Tan", Phone = "0532 111 20 20", Email = "gizem@aplus.com", Priority = LeadPriority.High, Note = "IK muduruyle gorusme zemini hazir.", AssignedEmployeeId = 1, CreatedAt = DateTime.Today.AddDays(-5), ScheduledVisitDate = DateTime.Today.AddDays(1),
                LeadStatusTypeId = 3, 
                LeadSourceTypeId = 1 },
            new Lead { Id = 102, Code = "LD-102", DisplayName = "Acme aday kaydi", City = "Istanbul", District = "Besiktas", ContactName = "Merve Karahan", Phone = "0533 222 33 44", Email = "merve@acme.com", Priority = LeadPriority.High, Note = "IK muduru kontagi alindi.", CreatedAt = DateTime.Today.AddDays(-3),
                LeadStatusTypeId = 2,
                LeadSourceTypeId = 1 },
            new Lead { Id = 103, Code = "LD-103", DisplayName = "Nova Lojistik", City = "Kocaeli", District = "Izmit", ContactName = "Sinem Yalcin", Phone = "0532 888 77 66", Email = "sinem@nova.com", Priority = LeadPriority.Medium, Note = "Sali gunu ziyaret bekleniyor.", AssignedEmployeeId = 1, CreatedAt = DateTime.Today.AddDays(-4), ScheduledVisitDate = DateTime.Today.AddDays(2),
                LeadStatusTypeId = 3,
                LeadSourceTypeId = 1 },
            new Lead { Id = 104, Code = "LD-104", DisplayName = "Burcu Cetin", City = "Istanbul", District = "Kadikoy", ContactName = "Burcu Cetin", Phone = "0532 555 11 22", Email = "burcu@example.com", Priority = LeadPriority.Medium, Note = "Arama geri donusu bekliyor.", CreatedAt = DateTime.Today.AddDays(-1),
                LeadStatusTypeId = 1,
                LeadSourceTypeId = 3 }
        ]);

        db.Activities.AddRange(
        [
            new Activity { Id = 101, Code = "ACT-101", ActivityDate = DateTime.Today.AddDays(-6), EmployeeId = 1, AccountId = 101, ContactName = "Merve Karahan", Summary = "Kurum BES sunumu ve ihtiyac analizi yapildi.", LeadId = 101,
                ContactStatusTypeId = 1,
                OutcomeStatusTypeId = 2 },
            new Activity { Id = 102, Code = "ACT-102", ActivityDate = DateTime.Today.AddDays(-4), EmployeeId = 2, AccountId = 103, ContactName = "Burcu Cetin", Summary = "Hayat urunu icin teklif sunuldu, tekrar aranacak.",
                ContactStatusTypeId = 1,
                OutcomeStatusTypeId = 4 },
            new Activity { Id = 103, Code = "ACT-103", ActivityDate = DateTime.Today.AddDays(-2), EmployeeId = 3, AccountId = 102, ContactName = "Sinem Yalcin", Summary = "Firma yerinde yetkiliye ulasilamadi.",
                ContactStatusTypeId = 2 }
        ]);

        db.Sales.AddRange(
        [
            new Sale { Id = 101, Code = "SAL-101", SaleDate = DateTime.Today.AddDays(-5), EmployeeId = 1, AccountId = 101, ActivityId = 101, CollectionAmount = 150000m, ApeAmount = 85000m, LumpSumAmount = 25000m, MonthlyPaymentAmount = 12000m, SaleCount = 1, Notes = "Kurumsal BES paketi.",
                ProductTypeId = 1 },
            new Sale { Id = 102, Code = "SAL-102", SaleDate = DateTime.Today.AddDays(-3), EmployeeId = 2, AccountId = 103, PremiumAmount = 18500m, CollectionAmount = 18500m, SaleCount = 1, Notes = "Bireysel hayat sigortasi.",
                ProductTypeId = 2 },
            new Sale { Id = 103, Code = "SAL-103", SaleDate = DateTime.Today.AddDays(-1), EmployeeId = 1, AccountId = 101, ActivityId = 101, ProductionAmount = 27000m, CollectionAmount = 27000m, SaleCount = 1, Notes = "Ayni ziyarette ek saglik urunu.",
                ProductTypeId = 3 }
        ]);

        db.Expenses.AddRange(
        [
            new Expense { Id = 101, Code = "EXP-101", ExpenseDate = DateTime.Today.AddDays(-3), EmployeeId = 1, Amount = 1450m, Notes = "Istanbul Anadolu gezi gideri",
                ExpenseTypeId = 1 },
            new Expense { Id = 102, Code = "EXP-102", ExpenseDate = DateTime.Today.AddDays(-2), EmployeeId = 1, Amount = 620m, Notes = "Musteri ziyareti yemek gideri",
                ExpenseTypeId = 2 },
            new Expense { Id = 103, Code = "EXP-103", ExpenseDate = DateTime.Today.AddDays(-1), EmployeeId = 3, Amount = 980m, Notes = "Kocaeli saha ziyaretleri",
                ExpenseTypeId = 1 }
        ]);

        db.ImportBatches.AddRange(
        [
            new ImportBatch { Id = 1, FileName = "Aktivite Raporu.xlsx", ImportedAt = DateTime.Today.AddDays(-10), ImportedBy = "ops", Status = "Reference Analyzed", Notes = "Ham veri siniflandirildi." },
            new ImportBatch { Id = 2, FileName = "Satis Raporu.xlsx", ImportedAt = DateTime.Today.AddDays(-9), ImportedBy = "ops", Status = "Reference Analyzed", Notes = "KPI karsilastirma icin etiketlendi." }
        ]);

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
            Console.WriteLine($"[SEED-CHECK] {roleStr} kontrol ediliyor...");
            
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
            Console.WriteLine($"[SEED-OK] Başarıyla {addedCount} yeni kayıt eklendi.");
        }
        else
        {
            Console.WriteLine("[SEED-INFO] Herhangi bir yeni kayıt eklenmedi (Zaten güncel).");
        }
    }
}
