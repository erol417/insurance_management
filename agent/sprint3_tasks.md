# Sprint 3 — Servis Katmanı, Validation ve Soft Delete

## Genel Talimat

Bu sprint, controller'lardaki iş mantığını servis sınıflarına taşımayı, tutarlı bir validation altyapısı kurmayı ve soft delete mekanizmasını eklemeyi hedefler. Sprint sonunda controller'lar yalnızca HTTP request/response ile ilgilenecek, tüm iş kuralları servis katmanında yaşayacak.

**Önemli mimari karar:** Proje şu an MVC + Razor Views yapısında çalışıyor. Henüz ayrı bir API + React frontend'e geçilmedi. Bu nedenle Sprint 3'te tam bir DTO katmanı oluşturmak gereksiz — mevcut ViewModel'ler zaten view'lar ile controller arasındaki veri taşıma görevini yapıyor. Servisler doğrudan entity döndürebilir, controller'lar entity → ViewModel dönüşümünü yapmaya devam edebilir. İleride API katmanına geçildiğinde DTO'lar o zaman eklenir.

**Görevler sıralıdır. Her görev tamamlandığında proje derlenip doğrulanmalıdır.**

---

## GÖREV 3.1 — Services Klasör Yapısı ve Interface Tanımlama

### Neden
Mevcut durumda sadece `DashboardService` var. Her modül için bir servis interface'i ve implementasyonu oluşturulmalı. Interface kullanmak ileride test yazımını ve dependency injection'ı kolaylaştırır.

### Adımlar

1. `Services/` klasörü altında her modül için interface ve implementasyon dosyası oluştur:

```
Services/
├── ILeadService.cs
├── LeadService.cs
├── IActivityService.cs
├── ActivityService.cs
├── ISaleService.cs
├── SaleService.cs
├── IExpenseService.cs
├── ExpenseService.cs
├── IAccountService.cs
├── AccountService.cs
├── IEmployeeService.cs
├── EmployeeService.cs
├── DashboardService.cs          ← mevcut, interface eklenecek
├── IDashboardService.cs         ← yeni
```

2. Her servis interface'inde o modülün temel operasyonlarını tanımla. Aşağıda her modül için beklenen metot listesi var:

#### ILeadService
```csharp
public interface ILeadService
{
    List<Lead> GetAll(int page, int pageSize, out int totalCount);
    Lead? GetById(int id);
    Lead Create(Lead lead);
    Lead? Update(int id, Lead updated);
    bool Delete(int id);
    Lead? Assign(int leadId, int employeeId, int assignedByUserId, string? note);
    Lead? StartVisit(int leadId, int userId);
    List<Lead> GetAssignments(int? employeeId, string? status);
    (bool isValid, List<string> errors) Validate(Lead lead);
    bool CheckDuplicate(string displayName, string? phone, int? excludeId = null);
}
```

#### IActivityService
```csharp
public interface IActivityService
{
    List<Activity> GetAll(int page, int pageSize, out int totalCount);
    Activity? GetById(int id);
    Activity Create(Activity activity);
    Activity? Update(int id, Activity updated);
    bool Delete(int id);  // soft delete
    (bool isValid, List<string> errors) Validate(Activity activity);
}
```

#### ISaleService
```csharp
public interface ISaleService
{
    List<Sale> GetAll(int page, int pageSize, out int totalCount);
    Sale? GetById(int id);
    Sale Create(Sale sale);
    Sale? Update(int id, Sale updated);
    bool Delete(int id);  // soft delete
    (bool isValid, List<string> errors) Validate(Sale sale);
}
```

#### IExpenseService
```csharp
public interface IExpenseService
{
    List<Expense> GetAll(int page, int pageSize, out int totalCount);
    Expense? GetById(int id);
    Expense Create(Expense expense);
    Expense? Update(int id, Expense updated);
    bool Delete(int id);  // soft delete
    (bool isValid, List<string> errors) Validate(Expense expense);
}
```

#### IAccountService
```csharp
public interface IAccountService
{
    List<Account> GetAll(int page, int pageSize, out int totalCount);
    Account? GetById(int id);
    Account Create(Account account);
    Account? Update(int id, Account updated);
    bool Delete(int id);
    bool CheckDuplicate(string displayName, string? taxNumber, int? excludeId = null);
}
```

#### IEmployeeService
```csharp
public interface IEmployeeService
{
    List<Employee> GetAll();
    Employee? GetById(int id);
    Employee Create(Employee employee);
    Employee? Update(int id, Employee updated);
    bool Delete(int id);
}
```

#### IDashboardService
```csharp
public interface IDashboardService
{
    // Mevcut DashboardService'teki public metotların imzalarını buraya taşı
}
```

3. Projeyi derle.

### Beklenen Sonuç
- Tüm servis interface'leri tanımlı.
- Henüz implementasyon yok, sadece sözleşme.

---

## GÖREV 3.2 — Soft Delete Altyapısı

### Neden
ERD'de `activities`, `sales` ve `expenses` tabloları için `deleted_at` ve `deleted_by` alanları tanımlı. Silinen kayıtlar fiziksel olarak silinmemeli, işaretlenip gizlenmeli. Bu audit ve veri bütünlüğü için kritik.

### Adımlar

1. `Domain/` klasöründe `ISoftDeletable.cs` interface'i oluştur:

```csharp
namespace InsuranceManagement.Web.Domain;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
```

2. `Activity`, `Sale` ve `Expense` entity'lerine bu interface'i uygula:

```csharp
public class Activity : BaseEntity, ISoftDeletable
{
    // mevcut property'ler...
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

`Sale` ve `Expense` için de aynısını yap.

3. `AppDbContext.OnModelCreating`'de soft delete alanları için konfigürasyon ekle:

```csharp
foreach (var entityType in modelBuilder.Model.GetEntityTypes()
    .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType)))
{
    modelBuilder.Entity(entityType.ClrType, b =>
    {
        b.Property("DeletedBy").HasMaxLength(150);
    });
}
```

4. `AppDbContext`'e global query filter ekle — soft delete yapılmış kayıtlar varsayılan olarak sorguya dahil edilmesin:

```csharp
// OnModelCreating içinde
modelBuilder.Entity<Activity>().HasQueryFilter(x => x.DeletedAt == null);
modelBuilder.Entity<Sale>().HasQueryFilter(x => x.DeletedAt == null);
modelBuilder.Entity<Expense>().HasQueryFilter(x => x.DeletedAt == null);
```

**Not:** Silinmiş kayıtları görmek gereken yerlerde (örneğin admin audit ekranı) `.IgnoreQueryFilters()` kullanılabilir.

5. Derle.

### Beklenen Sonuç
- Activity, Sale, Expense entity'leri `ISoftDeletable` interface'ini uyguluyor.
- Global query filter tanımlı — soft delete yapılmış kayıtlar varsayılan sorgularda görünmüyor.

---

## GÖREV 3.3 — LeadService Implementasyonu

### Neden
`LeadsController` projenin en büyük controller'ı (600+ satır). İçinde CRUD, atama, dönüşüm (StartVisit), duplicate kontrolü ve validation mantığı var. Tüm bu iş mantığı servis katmanına taşınmalı.

### Adımlar

1. `Services/LeadService.cs` oluştur. Constructor'da `AppDbContext` ve `IHttpContextAccessor` inject et:

```csharp
public class LeadService : ILeadService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeadService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }
}
```

2. `LeadsController`'daki aşağıdaki iş mantıklarını `LeadService`'e taşı:

**GetAll:** Lead listesi çekme, sayfalama, Include'lar.
```csharp
public List<Lead> GetAll(int page, int pageSize, out int totalCount)
{
    var query = _db.Leads
        .Include(x => x.LeadStatusType)
        .Include(x => x.LeadSourceType)
        .Include(x => x.AssignedEmployee)
        .OrderByDescending(x => x.CreatedAt);

    totalCount = query.Count();
    return query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();
}
```

**Create:** Yeni lead oluşturma, Code üretme, validation çağrısı.

**Update:** Lead güncelleme, mevcut kaydı bulma ve alanları güncelleme.

**Delete:** Lead silme (fiziksel silme — lead soft delete kapsamında değil).

**Assign:** Lead'i personele atama. `LeadAssignment` kaydı oluşturma, lead status'unu güncelleme.

**StartVisit:** Lead'den Account + Activity zincirine dönüşüm. Bu controller'daki en karmaşık iş mantığı — servis katmanına taşınması en kritik parça.

**CheckDuplicate:** İsim ve telefon bazlı duplicate kontrolü.

**Validate:** Lead iş kuralları doğrulaması:
- DisplayName zorunlu
- City zorunlu
- LeadStatusTypeId geçerli olmalı
- LeadSourceTypeId geçerli olmalı

3. `LeadsController`'ı güncelle — `AppDbContext` doğrudan kullanımını `ILeadService` çağrısına çevir:

**Eski:**
```csharp
public class LeadsController : AppController
{
    public LeadsController(AppDbContext db) : base(db) { }
    
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var leads = Db.Leads.Include(...).OrderBy(...).Skip(...).Take(...).ToList();
        // ...
    }
}
```

**Yeni:**
```csharp
public class LeadsController : AppController
{
    private readonly ILeadService _leadService;
    
    public LeadsController(AppDbContext db, ILeadService leadService) : base(db)
    {
        _leadService = leadService;
    }
    
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var leads = _leadService.GetAll(page, pageSize, out var totalCount);
        // ViewModel dönüşümü burada kalır
    }
}
```

**Not:** Controller hâlâ `AppController`'dan türüyor ve `Db` property'sine erişebiliyor. Bu geçiş aşamasında bazı küçük işler (ViewBag doldurma gibi) için hâlâ `Db` kullanılabilir. Ama CRUD ve iş kuralları artık servis üzerinden gitmeli.

4. `Program.cs`'de servis kaydını ekle:
```csharp
builder.Services.AddScoped<ILeadService, LeadService>();
```

5. Derle ve Lead modülünü test et — oluştur, düzenle, sil, ata, ziyaret başlat.

### Beklenen Sonuç
- LeadService tüm Lead iş mantığını barındırıyor.
- LeadsController sadece HTTP akışı, ViewModel dönüşümü ve ViewBag hazırlığı yapıyor.
- Tüm Lead CRUD akışları çalışıyor.

---

## GÖREV 3.4 — ActivityService Implementasyonu

### Neden
`ActivitiesController` ikinci büyük controller. ContactStatus/OutcomeStatus iş kuralları (Görüşülemedi ise sonuç NOT_APPLICABLE olmalı vb.) servis katmanında olmalı.

### Adımlar

1. `Services/ActivityService.cs` oluştur.

2. Controller'dan şu mantıkları taşı:
   - GetAll (Include'lar ile)
   - GetById
   - Create (Code üretme, iş kuralları)
   - Update
   - Delete → **Soft delete** uygula:
     ```csharp
     public bool Delete(int id)
     {
         var activity = _db.Activities.Find(id);
         if (activity == null) return false;
         
         var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
         activity.DeletedAt = DateTime.UtcNow;
         activity.DeletedBy = userName;
         _db.SaveChanges();
         return true;
     }
     ```

3. **Validation — Aktivite iş kuralları:**
```csharp
public (bool isValid, List<string> errors) Validate(Activity activity)
{
    var errors = new List<string>();

    if (activity.EmployeeId <= 0)
        errors.Add("Personel seçimi zorunludur.");
    
    if (activity.AccountId <= 0)
        errors.Add("Müşteri/Firma seçimi zorunludur.");
    
    if (activity.ActivityDate == default)
        errors.Add("Aktivite tarihi zorunludur.");
    
    if (string.IsNullOrWhiteSpace(activity.Summary))
        errors.Add("Görüşme içeriği zorunludur.");
    
    if (activity.ContactStatusTypeId <= 0)
        errors.Add("Temas durumu seçimi zorunludur.");

    // Temas durumu kuralı
    var contactStatus = _db.ActivityContactStatusTypes.Find(activity.ContactStatusTypeId);
    var outcomeStatus = _db.ActivityOutcomeStatusTypes.Find(activity.OutcomeStatusTypeId);

    if (contactStatus?.Code == "NOT_CONTACTED" && outcomeStatus?.Code != "NOT_APPLICABLE")
    {
        errors.Add("Görüşülemedi durumunda sonuç yalnızca 'Uygulanamaz' olabilir.");
    }

    if (contactStatus?.Code == "CONTACTED" && activity.OutcomeStatusTypeId <= 0)
    {
        errors.Add("Görüşüldü durumunda görüşme sonucu seçimi zorunludur.");
    }

    return (errors.Count == 0, errors);
}
```

4. Controller'ı güncelle — `IActivityService` inject et, iş mantığını servise yönlendir.

5. `Program.cs`'de kaydet:
```csharp
builder.Services.AddScoped<IActivityService, ActivityService>();
```

6. Derle ve Aktivite modülünü test et.

### Beklenen Sonuç
- ActivityService tüm aktivite iş mantığını barındırıyor.
- Soft delete çalışıyor — silinen aktiviteler listede görünmüyor ama veritabanında duruyor.
- ContactStatus/OutcomeStatus iş kuralları servis seviyesinde doğrulanıyor.

---

## GÖREV 3.5 — SaleService Implementasyonu

### Neden
Satış modülü ürün tipine göre değişen finansal validation kuralları içeriyor. Bu kurallar servis katmanında merkezi olarak yönetilmeli.

### Adımlar

1. `Services/SaleService.cs` oluştur.

2. Controller'dan CRUD ve iş mantığını taşı.

3. Delete → **Soft delete** uygula.

4. **Validation — Ürün bazlı finansal kurallar:**
```csharp
public (bool isValid, List<string> errors) Validate(Sale sale)
{
    var errors = new List<string>();

    if (sale.EmployeeId <= 0)
        errors.Add("Personel seçimi zorunludur.");
    if (sale.AccountId <= 0)
        errors.Add("Müşteri/Firma seçimi zorunludur.");
    if (sale.ProductTypeId <= 0)
        errors.Add("Ürün tipi seçimi zorunludur.");
    if (sale.SaleDate == default)
        errors.Add("Satış tarihi zorunludur.");

    // Ürün tipine göre finansal alan validation
    var productType = _db.InsuranceProductTypes.Find(sale.ProductTypeId);
    
    if (productType?.Code == "BES")
    {
        if (!sale.ApeAmount.HasValue || sale.ApeAmount <= 0)
            errors.Add("BES ürünü için APE tutarı zorunludur.");
        if (!sale.CollectionAmount.HasValue || sale.CollectionAmount < 0)
            errors.Add("BES ürünü için tahsilat tutarı zorunludur.");
    }
    else if (productType?.Code == "LIFE")
    {
        if (!sale.PremiumAmount.HasValue || sale.PremiumAmount <= 0)
            errors.Add("Hayat sigortası için prim tutarı zorunludur.");
    }
    else if (productType?.Code == "HEALTH")
    {
        if (!sale.ProductionAmount.HasValue || sale.ProductionAmount <= 0)
            errors.Add("Sağlık sigortası için üretim tutarı zorunludur.");
    }
    else if (productType?.Code == "TRAVEL" || productType?.Code == "OTHER")
    {
        if (!sale.SaleAmount.HasValue || sale.SaleAmount <= 0)
            errors.Add("Satış tutarı zorunludur.");
    }

    // Negatif tutar kontrolü
    if (sale.PremiumAmount.HasValue && sale.PremiumAmount < 0)
        errors.Add("Prim tutarı negatif olamaz.");
    if (sale.ApeAmount.HasValue && sale.ApeAmount < 0)
        errors.Add("APE tutarı negatif olamaz.");
    if (sale.CollectionAmount.HasValue && sale.CollectionAmount < 0)
        errors.Add("Tahsilat tutarı negatif olamaz.");

    return (errors.Count == 0, errors);
}
```

5. Controller'ı güncelle, `Program.cs`'de kaydet.

6. Derle ve Satış modülünü test et — özellikle BES validation'ını kontrol et.

### Beklenen Sonuç
- SaleService tüm satış iş mantığını barındırıyor.
- Ürün tipine göre finansal validation merkezi olarak çalışıyor.
- Soft delete çalışıyor.

---

## GÖREV 3.6 — ExpenseService Implementasyonu

### Neden
Masraf modülü daha basit ama aynı pattern'i takip etmeli.

### Adımlar

1. `Services/ExpenseService.cs` oluştur.

2. Controller'dan CRUD taşı.

3. Delete → **Soft delete** uygula.

4. **Validation:**
```csharp
public (bool isValid, List<string> errors) Validate(Expense expense)
{
    var errors = new List<string>();

    if (expense.EmployeeId <= 0)
        errors.Add("Personel seçimi zorunludur.");
    if (expense.ExpenseTypeId <= 0)
        errors.Add("Masraf türü seçimi zorunludur.");
    if (expense.ExpenseDate == default)
        errors.Add("Masraf tarihi zorunludur.");
    if (expense.Amount <= 0)
        errors.Add("Tutar sıfırdan büyük olmalıdır.");

    return (errors.Count == 0, errors);
}
```

5. Controller'ı güncelle, `Program.cs`'de kaydet.

6. Derle ve Masraf modülünü test et.

### Beklenen Sonuç
- ExpenseService çalışıyor.
- Soft delete çalışıyor.
- Tutar validation'ı merkezi.

---

## GÖREV 3.7 — AccountService ve EmployeeService Implementasyonu

### Neden
Account ve Employee modülleri daha basit CRUD operasyonları ama tutarlılık için aynı pattern'i takip etmeliler.

### Adımlar

1. `Services/AccountService.cs` oluştur:
   - CRUD operasyonları
   - Duplicate kontrolü (DisplayName + TaxNumber)
   - Account fiziksel silme (soft delete yok — ERD'de tanımlı değil)

2. `Services/EmployeeService.cs` oluştur:
   - CRUD operasyonları
   - Fiziksel silme

3. Controller'ları güncelle, `Program.cs`'de kaydet.

4. Derle ve her iki modülü test et.

### Beklenen Sonuç
- Tüm CRUD modüllerinde tutarlı servis pattern'i uygulanmış.

---

## GÖREV 3.8 — DashboardService Interface Ekleme

### Neden
DashboardService zaten var ve çalışıyor. Sadece interface ekleyerek DI tutarlılığını sağlamak gerekiyor.

### Adımlar

1. `Services/IDashboardService.cs` oluştur — mevcut DashboardService'teki public metotların imzalarını al.

2. `DashboardService`'i `IDashboardService`'den türet.

3. `Program.cs`'de kaydı güncelle:
```csharp
// Eski:
builder.Services.AddScoped<DashboardService>();
// Yeni:
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

4. `DashboardController`'da `DashboardService` yerine `IDashboardService` kullan.

5. Derle ve Dashboard'u test et.

### Beklenen Sonuç
- DashboardService interface üzerinden inject ediliyor.

---

## GÖREV 3.9 — Controller'larda Validation Akışını Standartlaştırma

### Neden
Artık her serviste bir `Validate` metodu var. Controller'lar bu metodu çağırıp hataları `ModelState`'e eklemeli. Bu yaklaşım tüm modüllerde tutarlı olmalı.

### Adımlar

1. Tüm controller'larda Create ve Edit POST action'larında şu pattern'i uygula:

```csharp
[HttpPost]
public IActionResult Create(ActivityFormViewModel model)
{
    var activity = MapToEntity(model);
    var (isValid, errors) = _activityService.Validate(activity);
    
    if (!isValid)
    {
        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);
    }

    if (!ModelState.IsValid)
    {
        PrepareViewBag(); // dropdown'ları tekrar doldur
        return View(model);
    }

    _activityService.Create(activity);
    TempData["Success"] = "Kayıt başarıyla oluşturuldu.";
    return RedirectToAction(nameof(Index));
}
```

2. Mevcut `IValidatableObject` implementasyonlarını ViewModel'lerden kaldır — validation artık servis katmanında.

3. **TempData mesajları:** Başarılı işlemler için `TempData["Success"]`, uyarılar için `TempData["Warning"]` kullan. View'larda bu mesajları gösterecek bir partial view zaten varsa koru, yoksa `_Layout.cshtml`'e basit bir mesaj gösterimi ekle.

4. Tüm modüllerde aynı pattern'i uygula.

5. Derle ve her modülü test et.

### Beklenen Sonuç
- Tüm modüllerde tutarlı validation akışı.
- Validation hataları Türkçe mesajlarla kullanıcıya gösteriliyor.
- ViewModel'lerde `IValidatableObject` kalmamış.

---

## GÖREV 3.10 — Migration ve Kapsamlı Test

### Neden
Soft delete alanları (DeletedAt, DeletedBy) ve global query filter'lar migration gerektiriyor.

### Adımlar

1. Migration oluştur:
```
rm -rf Data/Migrations/*
dotnet ef database drop --force
docker compose down -v && docker compose up -d
dotnet ef migrations add Sprint3_SoftDelete_ServiceLayer
dotnet run
```

2. **Kapsamlı test:**
   - Auth: giriş/çıkış
   - Lead: oluştur, düzenle, sil, ata, ziyaret başlat
   - Aktivite: oluştur, düzenle, **sil (soft delete — silinen kayıt listede görünmemeli ama DB'de duruyor mu kontrol et)**
   - Satış: oluştur (BES validation çalışıyor mu?), düzenle, **sil (soft delete)**
   - Masraf: oluştur, düzenle, **sil (soft delete)**
   - Dashboard: tüm ekranlar veri gösteriyor mu? (Soft delete olan kayıtlar dashboard'da sayılmamalı)
   - Account: oluştur, düzenle, duplicate kontrolü
   - Import: çalışıyor mu?

### Beklenen Sonuç
- Migration temiz.
- Soft delete çalışıyor.
- Tüm modüller servis katmanı üzerinden çalışıyor.
- Validation merkezi ve tutarlı.

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne ekle:

```
- Servis katmani olusturuldu: ILeadService, IActivityService, ISaleService, IExpenseService, IAccountService, IEmployeeService, IDashboardService
- Controller'lardaki is mantigi servis siniflarına tasindi
- Soft delete mekanizmasi eklendi (Activity, Sale, Expense)
- ISoftDeletable interface ve global query filter uygulanmasi yapildi
- Validation is kurallari servis katmaninda merkezilestirildigi
- ViewModel'lerdeki IValidatableObject kaldırıldı
- Ürün tipine göre finansal validation kuralları servis katmanında uygulandı
- Aktivite temas/sonuç iş kuralları servis katmanında doğrulanıyor
- Tüm controller'larda tutarlı validation akışı sağlandı
```

`## Current Focus` bölümünü güncelle:

```
## Current Focus

- Rol bazli erisim kontrolu sertlestirme
- Veri izolasyonu (FieldSales kendi verisi, CallCenter sadece lead)
- Menu rol bazli filtreleme
```

`## Next Up` bölümünü güncelle:

```
## Next Up

- FieldSales → yalnizca kendi kayitlarini gorsun
- CallCenter → yalnizca lead modulune erissin
- Operations → sinirli duzenleme yetkisi
- Menu ve navigasyon rol bazli filtrelensin
- Soft delete yapilmis kayitlari admin gorebilsin
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19.1 Uygulanan Mimari İyileştirmeler` bölümüne ekle:

```markdown
### Sprint 3 — Servis Katmanı, Validation ve Soft Delete (Tamamlandı)

Aşağıdaki mimari iyileştirmeler uygulanmıştır:

- **Servis katmanı oluşturuldu**: Her modül için interface + implementasyon yapısı kurulmuştur (ILeadService/LeadService, IActivityService/ActivityService, ISaleService/SaleService, IExpenseService/ExpenseService, IAccountService/AccountService, IEmployeeService/EmployeeService, IDashboardService/DashboardService). Controller'lar artık doğrudan DbContext kullanmak yerine servis katmanı üzerinden çalışmaktadır.
- **İş kuralları merkezi hale geldi**: Lead atama, aktivite temas/sonuç kuralları, ürün bazlı finansal validation, duplicate kontrolü gibi iş mantıkları servis katmanında toplanmıştır.
- **Soft delete uygulandı**: Activity, Sale ve Expense entity'leri `ISoftDeletable` interface'ini uygulamaktadır. `DeletedAt` ve `DeletedBy` alanları ile silme işlemi kayıt altına alınmakta, global query filter ile silinmiş kayıtlar varsayılan sorgularda gizlenmektedir.
- **Validation standartlaştırıldı**: Her servis `Validate` metodu ile merkezi iş kuralı doğrulaması yapmaktadır. Controller'lar bu metodu çağırıp hataları `ModelState`'e eklemektedir. ViewModel'lerdeki `IValidatableObject` implementasyonları kaldırılmıştır.
```

---

## SPRINT 3 TAMAMLANMA KONTROL LİSTESİ

- [ ] Tüm modüller için servis interface + implementasyon oluşturulmuş
- [ ] LeadService — CRUD, atama, StartVisit, duplicate kontrolü çalışıyor
- [ ] ActivityService — CRUD, soft delete, ContactStatus/OutcomeStatus validation çalışıyor
- [ ] SaleService — CRUD, soft delete, ürün bazlı finansal validation çalışıyor
- [ ] ExpenseService — CRUD, soft delete, tutar validation çalışıyor
- [ ] AccountService — CRUD, duplicate kontrolü çalışıyor
- [ ] EmployeeService — CRUD çalışıyor
- [ ] DashboardService — interface üzerinden inject ediliyor
- [ ] Soft delete — silinen Activity, Sale, Expense listede görünmüyor ama DB'de duruyor
- [ ] Dashboard — soft delete yapılmış kayıtlar sayılmıyor
- [ ] Controller'larda doğrudan DbContext CRUD mantığı kalmamış (ViewBag hazırlığı hariç)
- [ ] Tüm modüllerde tutarlı validation akışı
- [ ] Validation hataları Türkçe
- [ ] ViewModel'lerde IValidatableObject kalmamış
- [ ] Program.cs'de tüm servisler register edilmiş
- [ ] Migration temiz oluşturulmuş
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
