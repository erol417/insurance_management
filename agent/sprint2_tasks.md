# Sprint 2 — Entity ve Veri Modeli Hizalama

## Genel Talimat

Bu sprint, mevcut entity yapısını ERD tasarımıyla hizalamayı hedefler. Üç ana iş var: referans tabloların oluşturulması, navigation property'lerin eklenmesi ve enum string alanların FK ilişkisine dönüştürülmesi. Sprint sonunda veri modeli ERD ile uyumlu olacak ve controller'lardaki manuel dictionary join'leri ortadan kalkacak.

**Görevler sıralıdır. Her görev tamamlandığında proje derlenip doğrulanmalıdır. Mevcut çalışan ekranlar kırılmamalıdır.**

**ÖNEMLİ UYARI:** Bu sprint, controller'lar ve view'lar dahil birçok dosyayı etkiler. Enum string değerlerini FK int'e çevirmek, dropdown'ları referans tablosundan doldurmak ve listelerde navigation property kullanmak gerekecek. Dikkatlice ve adım adım ilerle.

---

## GÖREV 2.1 — BaseReferenceEntity ve Referans Tablo Entity'leri

### Neden
ERD'de 6 referans tablosu tanımlı ama kodda bunlar `AppEnums.cs`'deki C# enum'ları olarak yaşıyor ve veritabanında `HasConversion<string>()` ile string olarak saklanıyor. Referans tablolar, admin panelinden yönetilebilir, display_order ile sıralanabilir ve is_active ile devre dışı bırakılabilir kayıtlar sağlar.

### Adımlar

1. `Domain/` klasöründe `BaseReferenceEntity.cs` oluştur:

```csharp
namespace InsuranceManagement.Web.Domain;

public abstract class BaseReferenceEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
```

2. `Domain/` klasöründe `ReferenceEntities.cs` dosyası oluştur. Aşağıdaki 6 entity'yi tanımla:

```csharp
namespace InsuranceManagement.Web.Domain;

public class LeadStatusType : BaseReferenceEntity { }

public class LeadSourceType : BaseReferenceEntity { }

public class ActivityContactStatusType : BaseReferenceEntity { }

public class ActivityOutcomeStatusType : BaseReferenceEntity { }

public class InsuranceProductType : BaseReferenceEntity { }

public class ExpenseType : BaseReferenceEntity { }
```

3. Projeyi derle — sadece entity tanımı, henüz DbContext'e eklenmedi.

### Beklenen Sonuç
- 6 referans entity sınıfı oluşturulmuş, hepsi `BaseReferenceEntity`'den türüyor.

---

## GÖREV 2.2 — LeadAssignment Entity

### Neden
ERD'de `lead_assignments` tablosu tanımlı. Mevcut kodda lead atama bilgisi `Lead` entity'si üzerinde `AssignedEmployeeId` alanıyla tutuluyor. Ancak tasarımda atama geçmişi, öncelik, son tarih ve atama notu gibi zengin bir yapı öngörülmüş.

### Adımlar

1. `Domain/` klasöründe `LeadAssignment.cs` oluştur:

```csharp
namespace InsuranceManagement.Web.Domain;

public class LeadAssignment : BaseEntity
{
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    
    public int AssignedEmployeeId { get; set; }
    public Employee AssignedEmployee { get; set; } = null!;
    
    public int AssignedByUserId { get; set; }
    public UserAccount AssignedByUser { get; set; } = null!;
    
    public DateTime AssignedAt { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? AssignmentNote { get; set; }
    public bool IsActive { get; set; } = true;
}
```

2. Projeyi derle.

### Beklenen Sonuç
- `LeadAssignment` entity'si `BaseEntity`'den türüyor, navigation property'leri tanımlı.

---

## GÖREV 2.3 — Transaction Entity'lere Navigation Property ve FK Düzenleme

### Neden
Mevcut entity'lerde navigation property yok. Controller'larda `Db.Employees.ToDictionary(x => x.Id, x => x.FullName)` gibi manuel join yapılıyor. Navigation property'ler eklendiğinde EF Core `.Include()` ile ilişkili verileri doğrudan çekebilir.

Ayrıca enum string olarak saklanan alanlar (ContactStatus, OutcomeStatus, ProductType, ExpenseType, LeadStatus) FK int'e dönüştürülecek.

### Adımlar

**Dikkat: Bu görev mevcut entity'leri önemli ölçüde değiştirir. Her entity'yi tek tek güncelle ve her birinde derleme kontrolü yap.**

#### 2.3.1 — Lead Entity Güncelleme

Mevcut `Lead` entity'sinde şu değişiklikleri yap:

- `Status` enum alanını kaldır, yerine:
  ```csharp
  public int LeadStatusTypeId { get; set; }
  public LeadStatusType LeadStatusType { get; set; } = null!;
  ```

- `Source` string alanını kaldır, yerine:
  ```csharp
  public int LeadSourceTypeId { get; set; }
  public LeadSourceType LeadSourceType { get; set; } = null!;
  ```

- Mevcut `AssignedEmployeeId` alanına navigation property ekle:
  ```csharp
  public int? AssignedEmployeeId { get; set; }
  public Employee? AssignedEmployee { get; set; }
  ```

- LeadAssignment koleksiyonu ekle:
  ```csharp
  public ICollection<LeadAssignment> Assignments { get; set; } = new List<LeadAssignment>();
  ```

#### 2.3.2 — Activity Entity Güncelleme

- `ContactStatus` enum alanını kaldır, yerine:
  ```csharp
  public int ContactStatusTypeId { get; set; }
  public ActivityContactStatusType ContactStatusType { get; set; } = null!;
  ```

- `OutcomeStatus` enum alanını kaldır, yerine:
  ```csharp
  public int OutcomeStatusTypeId { get; set; }
  public ActivityOutcomeStatusType OutcomeStatusType { get; set; } = null!;
  ```

- Mevcut FK alanlarına navigation property ekle:
  ```csharp
  public int EmployeeId { get; set; }
  public Employee Employee { get; set; } = null!;

  public int AccountId { get; set; }
  public Account Account { get; set; } = null!;
  ```

#### 2.3.3 — Sale Entity Güncelleme

- `ProductType` enum alanını kaldır, yerine:
  ```csharp
  public int ProductTypeId { get; set; }
  public InsuranceProductType ProductType { get; set; } = null!;
  ```

- Mevcut FK alanlarına navigation property ekle:
  ```csharp
  public int EmployeeId { get; set; }
  public Employee Employee { get; set; } = null!;

  public int AccountId { get; set; }
  public Account Account { get; set; } = null!;

  public int? ActivityId { get; set; }
  public Activity? Activity { get; set; }
  ```

#### 2.3.4 — Expense Entity Güncelleme

- `ExpenseType` enum alanını kaldır, yerine:
  ```csharp
  public int ExpenseTypeId { get; set; }
  public ExpenseType ExpenseTypeEntity { get; set; } = null!;
  ```

  **Not:** Property adı `ExpenseTypeEntity` olmalı çünkü `ExpenseType` zaten sınıf adı olarak kullanılıyor. Alternatif olarak namespace ile çakışmayı önlemek için farklı bir strateji izlenebilir — agent kendi mimarisine göre en temiz çözümü seçsin.

- Mevcut FK alanlarına navigation property ekle:
  ```csharp
  public int EmployeeId { get; set; }
  public Employee Employee { get; set; } = null!;
  ```

#### 2.3.5 — Employee Entity Güncelleme

Ters navigation koleksiyonları ekle:
```csharp
public ICollection<Activity> Activities { get; set; } = new List<Activity>();
public ICollection<Sale> Sales { get; set; } = new List<Sale>();
public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
public ICollection<LeadAssignment> LeadAssignments { get; set; } = new List<LeadAssignment>();
```

#### 2.3.6 — Account Entity Güncelleme

Ters navigation koleksiyonları ekle:
```csharp
public ICollection<Activity> Activities { get; set; } = new List<Activity>();
public ICollection<Sale> Sales { get; set; } = new List<Sale>();
```

#### 2.3.7 — UserAccount Entity Güncelleme

Employee bağlantısı ekle (ERD'de tanımlı):
```csharp
public int? EmployeeId { get; set; }
public Employee? Employee { get; set; }
```

3. Her entity güncellemesinden sonra derle.

### Beklenen Sonuç
- Tüm transaction entity'lerde FK alanları + navigation property'ler tanımlı.
- Enum string alanlar FK int alanlarına dönüştürülmüş.

---

## GÖREV 2.4 — AppDbContext Güncelleme

### Neden
Yeni entity'ler ve ilişkiler DbContext'e tanıtılmalı, OnModelCreating'de konfigüre edilmeli.

### Adımlar

1. Yeni DbSet'leri ekle:

```csharp
public DbSet<LeadStatusType> LeadStatusTypes => Set<LeadStatusType>();
public DbSet<LeadSourceType> LeadSourceTypes => Set<LeadSourceType>();
public DbSet<ActivityContactStatusType> ActivityContactStatusTypes => Set<ActivityContactStatusType>();
public DbSet<ActivityOutcomeStatusType> ActivityOutcomeStatusTypes => Set<ActivityOutcomeStatusType>();
public DbSet<InsuranceProductType> InsuranceProductTypes => Set<InsuranceProductType>();
public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
public DbSet<LeadAssignment> LeadAssignments => Set<LeadAssignment>();
```

2. `OnModelCreating`'de referans tablolar için ortak konfigürasyon ekle:

```csharp
foreach (var entityType in modelBuilder.Model.GetEntityTypes()
    .Where(t => typeof(BaseReferenceEntity).IsAssignableFrom(t.ClrType)))
{
    modelBuilder.Entity(entityType.ClrType, b =>
    {
        b.Property("Code").HasMaxLength(50).IsRequired();
        b.Property("Name").HasMaxLength(150).IsRequired();
    });
}
```

3. Referans tabloları için tablo isimlerini tanımla:

```csharp
modelBuilder.Entity<LeadStatusType>().ToTable("lead_status_types");
modelBuilder.Entity<LeadSourceType>().ToTable("lead_source_types");
modelBuilder.Entity<ActivityContactStatusType>().ToTable("activity_contact_status_types");
modelBuilder.Entity<ActivityOutcomeStatusType>().ToTable("activity_outcome_status_types");
modelBuilder.Entity<InsuranceProductType>().ToTable("insurance_product_types");
modelBuilder.Entity<ExpenseType>().ToTable("expense_types");
modelBuilder.Entity<LeadAssignment>().ToTable("lead_assignments");
```

4. Mevcut entity konfigürasyonlarını güncelle — enum `HasConversion<string>()` satırlarını kaldır ve FK relationship'leri tanımla. Örneğin Activity için:

```csharp
modelBuilder.Entity<Activity>(entity =>
{
    entity.ToTable("activities");
    entity.HasKey(x => x.Id);
    entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
    entity.Property(x => x.ContactName).HasMaxLength(150);
    entity.Property(x => x.Summary).HasMaxLength(3000).IsRequired();

    entity.HasOne(x => x.Employee)
        .WithMany(e => e.Activities)
        .HasForeignKey(x => x.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.Account)
        .WithMany(a => a.Activities)
        .HasForeignKey(x => x.AccountId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.ContactStatusType)
        .WithMany()
        .HasForeignKey(x => x.ContactStatusTypeId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.OutcomeStatusType)
        .WithMany()
        .HasForeignKey(x => x.OutcomeStatusTypeId)
        .OnDelete(DeleteBehavior.Restrict);
});
```

Aynı pattern'i Sale, Expense, Lead ve LeadAssignment için de uygula. Her ilişkide `OnDelete(DeleteBehavior.Restrict)` kullan — referans veri silindiğinde bağlı kayıtların bozulmasını engeller.

5. LeadAssignment konfigürasyonu:

```csharp
modelBuilder.Entity<LeadAssignment>(entity =>
{
    entity.ToTable("lead_assignments");
    entity.HasKey(x => x.Id);
    entity.Property(x => x.Priority).HasMaxLength(50);
    entity.Property(x => x.AssignmentNote).HasMaxLength(2000);

    entity.HasOne(x => x.Lead)
        .WithMany(l => l.Assignments)
        .HasForeignKey(x => x.LeadId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(x => x.AssignedEmployee)
        .WithMany(e => e.LeadAssignments)
        .HasForeignKey(x => x.AssignedEmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.AssignedByUser)
        .WithMany()
        .HasForeignKey(x => x.AssignedByUserId)
        .OnDelete(DeleteBehavior.Restrict);
});
```

6. Derle.

### Beklenen Sonuç
- Tüm yeni entity'ler DbContext'te tanımlı.
- İlişkiler `OnModelCreating`'de konfigüre edilmiş.
- Mevcut enum `HasConversion<string>()` satırları kaldırılmış.

---

## GÖREV 2.5 — AppEnums Güncelleme

### Neden
Enum'lar artık referans tablolarından gelecek, ama kodda hâlâ tip güvenliği ve DisplayText extension'ları için kullanılıyor olabilir. Pragmatik yaklaşım: enum tanımlarını koruyalım (tip güvenliği için), ama veritabanında artık FK int olarak saklansınlar.

### Adımlar

1. `AppEnums.cs`'deki şu enum'ları **kaldırma, koru**:
   - `RoleType` — bu zaten UserAccount.Role için kullanılıyor, Sprint 2'de değişmiyor
   - `AccountType` — bu da string olarak kalıyor, Sprint 2'de değişmiyor

2. Şu enum'ları **yorum satırına al veya kaldır** (artık referans tablodan geliyorlar):
   - `LeadStatus` → `LeadStatusType` tablosundan
   - `ContactStatus` → `ActivityContactStatusType` tablosundan
   - `OutcomeStatus` → `ActivityOutcomeStatusType` tablosundan
   - `ProductType` → `InsuranceProductType` tablosundan
   - `ExpenseType` → `ExpenseType` tablosundan

3. `DisplayTextExtensions.cs`'deki bu enum'larla ilgili extension metotları kaldır veya güncelle. Artık referans tablonun `Name` alanı kullanılacak.

4. Derle ve tüm etkilenen dosyaları takip et.

### Beklenen Sonuç
- Artık referans tabloya taşınan enum'lar koddan kaldırılmış.
- `RoleType` ve `AccountType` korunmuş.

---

## GÖREV 2.6 — DbSeeder Güncelleme

### Neden
Referans tabloları seed data ile doldurulmalı. Ayrıca mevcut transaction kayıtları (lead, activity, sale, expense) artık enum string yerine FK int kullanıyor, seed verisi buna göre güncellenmeli.

### Adımlar

1. DbSeeder'da referans tabloları seed et. Her tablo için `Code`, `Name`, `DisplayOrder` tanımla:

**LeadStatusType:**
```
NEW, Yeni, 1
READY_FOR_ASSIGNMENT, Atamaya Hazır, 2
ASSIGNED, Atandı, 3
VISITED, Ziyaret Edildi, 4
CONVERTED_TO_ACTIVITY, Aktiviteye Dönüştürüldü, 5
```

**LeadSourceType:**
```
CALL_CENTER, Çağrı Merkezi, 1
MANUAL, Manuel, 2
REFERRAL, Referans, 3
```

**ActivityContactStatusType:**
```
CONTACTED, Görüşüldü, 1
NOT_CONTACTED, Görüşülemedi, 2
```

**ActivityOutcomeStatusType:**
```
NOT_APPLICABLE, Uygulanamaz, 1
POSITIVE, Olumlu, 2
NEGATIVE, Olumsuz, 3
POSTPONED, Ertelendi, 4
SALE_CLOSED, Satış Oldu, 5
```

**InsuranceProductType:**
```
BES, BES (Bireysel Emeklilik), 1
LIFE, Hayat Sigortası, 2
HEALTH, Sağlık Sigortası, 3
TRAVEL, Seyahat Sigortası, 4
OTHER, Diğer, 5
```

**ExpenseType:**
```
TRAVEL, Yol/Ulaşım, 1
MEAL, Yemek, 2
ACCOMMODATION, Konaklama, 3
OTHER, Diğer, 4
```

2. Seed sırasında referans tabloları **önce** seed et (çünkü transaction kayıtları FK ile bunlara bağlanacak).

3. Mevcut seed transaction kayıtlarını güncelle — enum string yerine FK id kullan. Örneğin:
   - `ProductType = ProductType.BES` → `ProductTypeId = 1` (BES'in seed id'si)
   - `ContactStatus = ContactStatus.Contacted` → `ContactStatusTypeId = 1` (CONTACTED'ın seed id'si)

4. Referans tabloların `Id` değerlerini seed'de açıkça belirle (`HasData` veya manuel Id ataması ile) — böylece FK referansları deterministik olur.

5. Derle.

### Beklenen Sonuç
- Tüm referans tablolar seed verisiyle doldurulmuş.
- Transaction seed kayıtları FK int kullanıyor.

---

## GÖREV 2.7 — Controller ve View Güncelleme

### Neden
Controller'lar artık enum yerine referans tablodan veri çekecek. View'lar dropdown'ları referans tablodan dolduracak. Listelerde navigation property sayesinde manuel dictionary join'lere gerek kalmayacak.

### Adımlar

**Bu görev en geniş kapsamlı görevdir. Her modülü tek tek güncelle.**

#### 2.7.1 — Genel Kalıp

Tüm controller'larda şu pattern tekrarlanacak:

**Eski (enum-based dropdown):**
```csharp
ViewBag.ProductTypes = Enum.GetValues<ProductType>();
```

**Yeni (referans tablo-based dropdown):**
```csharp
ViewBag.ProductTypes = Db.InsuranceProductTypes
    .Where(x => x.IsActive)
    .OrderBy(x => x.DisplayOrder)
    .ToList();
```

**Eski (manuel dictionary join):**
```csharp
ViewBag.EmployeeMap = Db.Employees.ToDictionary(x => x.Id, x => x.FullName);
// View'da: EmployeeMap[item.EmployeeId]
```

**Yeni (navigation property ile Include):**
```csharp
var items = Db.Activities
    .Include(x => x.Employee)
    .Include(x => x.Account)
    .Include(x => x.ContactStatusType)
    .Include(x => x.OutcomeStatusType)
    .OrderByDescending(x => x.CreatedAt)
    .ToList();
// View'da: item.Employee.FullName
```

#### 2.7.2 — LeadsController

- Dropdown'ları `LeadStatusTypes` ve `LeadSourceTypes` referans tablolarından doldur.
- Lead listesinde `AssignedEmployee` navigation property'sini `.Include()` ile çek.
- Mevcut `ViewBag.EmployeeMap` dictionary'sini kaldır, navigation property kullan.
- Lead oluşturma/güncelleme formlarında enum string yerine FK int gönder.

#### 2.7.3 — ActivitiesController

- `ContactStatusTypes` ve `OutcomeStatusTypes` dropdown'larını referans tablodan doldur.
- `.Include(x => x.Employee).Include(x => x.Account).Include(x => x.ContactStatusType).Include(x => x.OutcomeStatusType)` kullan.
- Oluşturma/güncelleme formlarında FK int gönder.

#### 2.7.4 — SalesController

- `InsuranceProductTypes` dropdown'ını referans tablodan doldur.
- `.Include(x => x.Employee).Include(x => x.Account).Include(x => x.ProductType)` kullan.

#### 2.7.5 — ExpensesController

- `ExpenseTypes` dropdown'ını referans tablodan doldur.
- `.Include(x => x.Employee).Include(x => x.ExpenseTypeEntity)` kullan.

#### 2.7.6 — DashboardService

- Bu servis mevcut enum'ları kullanarak gruplama ve filtreleme yapıyor olabilir. Enum referanslarını FK/referans tablo yapısına çevir.
- `GroupBy(x => x.ProductType)` → `GroupBy(x => x.ProductType.Code)` veya `.Include(x => x.ProductType)` ile.

#### 2.7.7 — View Güncellemeleri

- Tüm Razor view'larda enum referansları güncellenecek.
- Dropdown'lar `<option>` olarak referans tablodan gelen `Id` ve `Name` kullanacak.
- Liste ekranlarında `item.Employee.FullName` şeklinde navigation property kullanılacak.
- DisplayText extension'ları kaldırıldıysa, bunların kullanıldığı yerleri `item.ContactStatusType.Name` ile değiştir.

**Bu görev büyük ve her modül ayrı test edilmeli. Her controller+view çifti güncellendikten sonra o modülü tarayıcıda test et.**

### Beklenen Sonuç
- Hiçbir controller'da `ToDictionary` ile manuel join kalmamış.
- Dropdown'lar referans tablolardan dolduruluyor.
- Liste ekranlarında navigation property ile ilişkili veriler gösteriliyor.

---

## GÖREV 2.8 — ViewModel Güncelleme

### Neden
ViewModel'lerde enum tipi property'ler var. Bunlar FK int'e dönüşmeli.

### Adımlar

1. `FormViewModels.cs` ve diğer ViewModel dosyalarını tara.
2. Enum tipindeki property'leri `int` veya `int?` olarak değiştir. Örneğin:
   - `public ProductType ProductType { get; set; }` → `public int ProductTypeId { get; set; }`
   - `public ContactStatus ContactStatus { get; set; }` → `public int ContactStatusTypeId { get; set; }`

3. View'lardaki `asp-for` binding'lerini yeni property adlarıyla güncelle.

4. Derle ve her formu test et.

### Beklenen Sonuç
- ViewModel'lerde enum referansı kalmamış, FK int kullanılıyor.

---

## GÖREV 2.9 — Migration ve Test

### Neden
Tüm entity değişiklikleri tamamlandıktan sonra tek bir temiz migration oluşturulmalı.

### Adımlar

1. Mevcut migration'ları sil:
   ```
   rm -rf Data/Migrations/*
   ```

2. Veritabanını sıfırla:
   ```
   dotnet ef database drop --force
   ```

3. Yeni migration oluştur:
   ```
   dotnet ef migrations add Sprint2_ReferenceTablesAndNavProperties
   ```

4. Migration dosyasını incele — 6 referans tablo, lead_assignments tablosu, yeni FK kolonları ve ilişkiler olmalı.

5. Uygulamayı çalıştır.

6. **Kapsamlı test:**
   - Auth: giriş/çıkış
   - Lead: oluştur (status ve source dropdown'ları referans tablodan geliyor mu?), düzenle, sil, atama
   - Activity: oluştur (ContactStatus ve OutcomeStatus dropdown'ları doğru mu?), düzenle, sil
   - Sale: oluştur (ProductType dropdown doğru mu?), düzenle, sil
   - Expense: oluştur (ExpenseType dropdown doğru mu?), düzenle, sil
   - Dashboard: tüm dashboard'lar veri gösteriyor mu?
   - Import: çalışıyor mu?
   - Liste ekranlarında Employee adı, Account adı, durum adları navigation property ile gösteriliyor mu?

### Beklenen Sonuç
- Temiz migration, tüm referans tablolar ve ilişkiler tanımlı.
- Tüm modüller çalışıyor ve referans tablolardan besleniyor.

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne ekle:

```
- 6 referans tablo entity'si olusturuldu (LeadStatusType, LeadSourceType, ActivityContactStatusType, ActivityOutcomeStatusType, InsuranceProductType, ExpenseType)
- BaseReferenceEntity abstract sinifi olusturuldu
- LeadAssignment entity'si olusturuldu
- Tum transaction entity'lere navigation property eklendi
- Enum string alanlar FK int iliskisine donusturuldu
- Controller'lardaki manuel dictionary join'ler navigation property ile degistirildi
- Dropdown'lar referans tablolardan dolduruluyor
- DashboardService referans tablo yapisina uyumlandi
- Migration sifirdan olusturuldu ve dogrulandi
```

`## Current Focus` bölümünü güncelle:

```
## Current Focus

- Servis katmani olusturma (controller'lardan is mantigi ayirma)
- Validation altyapisi kurma
- DTO katmani tanimlama
```

`## Next Up` bölümünü güncelle:

```
## Next Up

- ILeadService, IActivityService, ISaleService, IExpenseService olusturma
- FluentValidation entegrasyonu
- Standart response modeli
- Rol bazli erisim kontrolu sertlestirme
- Soft delete mekanizmasi
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19.1 Uygulanan Mimari İyileştirmeler` bölümüne ekle:

```markdown
### Sprint 2 — Entity ve Veri Modeli Hizalama (Tamamlandı)

Aşağıdaki veri modeli iyileştirmeleri uygulanmıştır:

- **Referans tablolar oluşturuldu**: 6 referans tablo (LeadStatusType, LeadSourceType, ActivityContactStatusType, ActivityOutcomeStatusType, InsuranceProductType, ExpenseType) `BaseReferenceEntity` sınıfından türetilerek oluşturulmuştur. Her tablo `Code`, `Name`, `DisplayOrder`, `IsActive` alanlarını içerir.
- **Enum → FK geçişi tamamlandı**: Daha önce `HasConversion<string>()` ile string olarak saklanan enum alanları, referans tablolara FK int ilişkisi ile bağlanmıştır.
- **Navigation property'ler eklendi**: Tüm transaction entity'lerde (Activity, Sale, Expense, Lead) ilişkili entity'lere navigation property tanımlanmıştır. Controller'lardaki manuel dictionary join'ler kaldırılmıştır.
- **LeadAssignment entity'si eklendi**: Lead atama geçmişi, öncelik ve son tarih bilgileri ayrı tabloda takip edilebilir hale gelmiştir.
- **İlişki konfigürasyonları**: Tüm FK ilişkileri `OnDelete(DeleteBehavior.Restrict)` ile tanımlanmış, referans veri bütünlüğü korunmuştur.
```

---

## SPRINT 2 TAMAMLANMA KONTROL LİSTESİ

- [ ] 6 referans tablo entity'si oluşturulmuş ve BaseReferenceEntity'den türüyor
- [ ] LeadAssignment entity'si oluşturulmuş
- [ ] Tüm transaction entity'lerde navigation property tanımlı
- [ ] Enum string alanlar FK int'e dönüştürülmüş
- [ ] OnModelCreating'de tüm ilişkiler konfigüre edilmiş
- [ ] AppEnums.cs'den taşınan enum'lar kaldırılmış
- [ ] DbSeeder referans tabloları seed ediyor
- [ ] DbSeeder transaction kayıtları FK int kullanıyor
- [ ] Controller'larda manuel dictionary join kalmamış
- [ ] Dropdown'lar referans tablolardan dolduruluyor
- [ ] DashboardService yeni yapıya uyumlu
- [ ] Migration temiz oluşturulmuş
- [ ] Tüm CRUD akışları çalışıyor
- [ ] Dashboard ekranları veri gösteriyor
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
