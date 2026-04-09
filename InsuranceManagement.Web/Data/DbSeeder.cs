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

        db.Employees.AddRange(
        [
            new Employee { Id = 1, FullName = "Ahmet Yilmaz", Region = "Istanbul Europe", City = "Istanbul" },
            new Employee { Id = 2, FullName = "Elif Kara", Region = "Istanbul Anatolia", City = "Istanbul" },
            new Employee { Id = 3, FullName = "Murat Demir", Region = "Kocaeli", City = "Kocaeli" },
            new Employee { Id = 4, FullName = "Zeynep Acar", Region = "Bursa", City = "Bursa" }
        ]);

        db.Users.AddRange(
        [
            new UserAccount { Id = 1, UserName = "admin", Password = "admin123", FullName = "Sistem Yoneticisi", Role = RoleType.Admin },
            new UserAccount { Id = 2, UserName = "manager", Password = "manager123", FullName = "Genel Yonetici", Role = RoleType.Manager },
            new UserAccount { Id = 3, UserName = "sales", Password = "sales123", FullName = "Satis Muduru", Role = RoleType.SalesManager },
            new UserAccount { Id = 4, UserName = "ops", Password = "ops123", FullName = "Operasyon Uzmani", Role = RoleType.Operations },
            new UserAccount { Id = 5, UserName = "field1", Password = "field123", FullName = "Ahmet Yilmaz", Role = RoleType.FieldSales, EmployeeId = 1 },
            new UserAccount { Id = 6, UserName = "field2", Password = "field123", FullName = "Elif Kara", Role = RoleType.FieldSales, EmployeeId = 2 },
            new UserAccount { Id = 7, UserName = "call", Password = "call123", FullName = "Call Center Kullanici", Role = RoleType.CallCenter }
        ]);

        db.Accounts.AddRange(
        [
            new Account { Id = 101, Code = "ACC-101", AccountType = AccountType.Corporate, DisplayName = "Acme Dis Ticaret A.S.", City = "Istanbul", District = "Besiktas", Phone = "0212 111 11 11", Email = "ik@acme.com", TaxNumber = "1234567890", OwnerEmployeeId = 1, Notes = "Kurumsal BES potansiyeli.", Status = "Active" },
            new Account { Id = 102, Code = "ACC-102", AccountType = AccountType.Corporate, DisplayName = "Nova Lojistik", City = "Kocaeli", District = "Izmit", Phone = "0262 222 22 22", Email = "info@nova.com", TaxNumber = "9876543210", OwnerEmployeeId = 3, Notes = "Hayat ve saglik urunleri icin acik firsat.", Status = "Active" },
            new Account { Id = 103, Code = "ACC-103", AccountType = AccountType.Individual, DisplayName = "Burcu Cetin", City = "Istanbul", District = "Kadikoy", Phone = "0532 555 11 22", Email = "burcu@example.com", OwnerEmployeeId = 2, Notes = "Bireysel hayat urunu ilgisi.", Status = "Active" }
        ]);

        db.Leads.AddRange(
        [
            new Lead { Id = 101, Code = "LD-101", DisplayName = "A Plus Holding", Source = "Call Center", City = "Istanbul", District = "Sisli", ContactName = "Gizem Tan", Phone = "0532 111 20 20", Email = "gizem@aplus.com", Status = LeadStatus.Assigned, Priority = "High", Note = "IK muduruyle gorusme zemini hazir.", AssignedEmployeeId = 1, CreatedAt = DateTime.Today.AddDays(-5), ScheduledVisitDate = DateTime.Today.AddDays(1) },
            new Lead { Id = 102, Code = "LD-102", DisplayName = "Acme aday kaydi", Source = "Call Center", City = "Istanbul", District = "Besiktas", ContactName = "Merve Karahan", Phone = "0533 222 33 44", Email = "merve@acme.com", Status = LeadStatus.ReadyForAssignment, Priority = "High", Note = "IK muduru kontagi alindi.", CreatedAt = DateTime.Today.AddDays(-3) },
            new Lead { Id = 103, Code = "LD-103", DisplayName = "Nova Lojistik", Source = "Call Center", City = "Kocaeli", District = "Izmit", ContactName = "Sinem Yalcin", Phone = "0532 888 77 66", Email = "sinem@nova.com", Status = LeadStatus.Assigned, Priority = "Medium", Note = "Sali gunu ziyaret bekleniyor.", AssignedEmployeeId = 1, CreatedAt = DateTime.Today.AddDays(-4), ScheduledVisitDate = DateTime.Today.AddDays(2) },
            new Lead { Id = 104, Code = "LD-104", DisplayName = "Burcu Cetin", Source = "Referral", City = "Istanbul", District = "Kadikoy", ContactName = "Burcu Cetin", Phone = "0532 555 11 22", Email = "burcu@example.com", Status = LeadStatus.New, Priority = "Medium", Note = "Arama geri donusu bekliyor.", CreatedAt = DateTime.Today.AddDays(-1) }
        ]);

        db.Activities.AddRange(
        [
            new Activity { Id = 101, Code = "ACT-101", ActivityDate = DateTime.Today.AddDays(-6), EmployeeId = 1, AccountId = 101, ContactName = "Merve Karahan", ContactStatus = ContactStatus.Contacted, OutcomeStatus = OutcomeStatus.Positive, Summary = "Kurum BES sunumu ve ihtiyac analizi yapildi.", LeadId = 101 },
            new Activity { Id = 102, Code = "ACT-102", ActivityDate = DateTime.Today.AddDays(-4), EmployeeId = 2, AccountId = 103, ContactName = "Burcu Cetin", ContactStatus = ContactStatus.Contacted, OutcomeStatus = OutcomeStatus.Postponed, Summary = "Hayat urunu icin teklif sunuldu, tekrar aranacak." },
            new Activity { Id = 103, Code = "ACT-103", ActivityDate = DateTime.Today.AddDays(-2), EmployeeId = 3, AccountId = 102, ContactName = "Sinem Yalcin", ContactStatus = ContactStatus.NotContacted, Summary = "Firma yerinde yetkiliye ulasilamadi." }
        ]);

        db.Sales.AddRange(
        [
            new Sale { Id = 101, Code = "SAL-101", SaleDate = DateTime.Today.AddDays(-5), EmployeeId = 1, AccountId = 101, ActivityId = 101, ProductType = ProductType.Bes, CollectionAmount = 150000m, ApeAmount = 85000m, LumpSumAmount = 25000m, MonthlyPaymentAmount = 12000m, SaleCount = 1, Notes = "Kurumsal BES paketi." },
            new Sale { Id = 102, Code = "SAL-102", SaleDate = DateTime.Today.AddDays(-3), EmployeeId = 2, AccountId = 103, ProductType = ProductType.Life, PremiumAmount = 18500m, CollectionAmount = 18500m, SaleCount = 1, Notes = "Bireysel hayat sigortasi." },
            new Sale { Id = 103, Code = "SAL-103", SaleDate = DateTime.Today.AddDays(-1), EmployeeId = 1, AccountId = 101, ActivityId = 101, ProductType = ProductType.Health, ProductionAmount = 27000m, CollectionAmount = 27000m, SaleCount = 1, Notes = "Ayni ziyarette ek saglik urunu." }
        ]);

        db.Expenses.AddRange(
        [
            new Expense { Id = 101, Code = "EXP-101", ExpenseDate = DateTime.Today.AddDays(-3), EmployeeId = 1, ExpenseType = ExpenseType.Travel, Amount = 1450m, Notes = "Istanbul Anadolu gezi gideri" },
            new Expense { Id = 102, Code = "EXP-102", ExpenseDate = DateTime.Today.AddDays(-2), EmployeeId = 1, ExpenseType = ExpenseType.Meal, Amount = 620m, Notes = "Musteri ziyareti yemek gideri" },
            new Expense { Id = 103, Code = "EXP-103", ExpenseDate = DateTime.Today.AddDays(-1), EmployeeId = 3, ExpenseType = ExpenseType.Travel, Amount = 980m, Notes = "Kocaeli saha ziyaretleri" }
        ]);

        db.ImportBatches.AddRange(
        [
            new ImportBatch { Id = 1, FileName = "Aktivite Raporu.xlsx", ImportedAt = DateTime.Today.AddDays(-10), ImportedBy = "ops", Status = "Reference Analyzed", Notes = "Ham veri siniflandirildi." },
            new ImportBatch { Id = 2, FileName = "Satis Raporu.xlsx", ImportedAt = DateTime.Today.AddDays(-9), ImportedBy = "ops", Status = "Reference Analyzed", Notes = "KPI karsilastirma icin etiketlendi." }
        ]);

        await db.SaveChangesAsync();
    }
}
