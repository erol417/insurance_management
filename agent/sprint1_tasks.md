# Sprint 1 — Temel Temizlik ve Güvenlik

## Genel Talimat

Bu belge, mevcut MVC uygulamasının temel mimari borçlarını temizlemek ve güvenlik altyapısını sağlamlaştırmak için yapılması gereken görevleri içerir. **Görevler sıralıdır ve verilen sırayla uygulanmalıdır.** Her görev tamamlandığında uygulama derlenip çalıştırılarak doğrulanmalıdır. Mevcut çalışan hiçbir ekran veya akış kırılmamalıdır.

Sprint tamamlandıktan sonra belgenin sonunda listelenen **doküman güncelleme görevleri** de yapılmalıdır.

---

## GÖREV 1.1 — AppDataStore'u Kaldır

### Neden
`AppDataStore` geliştirme aşamasından kalan bir in-memory singleton veri deposudur. Tüm controller'lar artık `AppDbContext` üzerinden çalışıyor. Bu ölü bağımlılık hem kafa karışıklığı yaratıyor hem de yanlışlıkla kullanılma riski taşıyor.

### Adımlar

1. Proje genelinde `AppDataStore` referanslarını tara:
   ```
   grep -r "AppDataStore" --include="*.cs" .
   ```
2. Eğer herhangi bir controller veya servis hâlâ `AppDataStore`'a bağımlıysa, o erişimi `AppDbContext` üzerinden yapacak şekilde değiştir.
3. `Infrastructure/AppDataStore.cs` dosyasını sil.
4. `Program.cs` içindeki şu satırı kaldır:
   ```csharp
   builder.Services.AddSingleton<AppDataStore>();
   ```
5. `Program.cs`'den `using InsuranceManagement.Web.Infrastructure;` satırını da artık gereksizse kaldır.
6. Projeyi derle ve çalıştır. Tüm modüllerin (Lead, Activity, Sale, Expense, Dashboard, Import, Auth) hâlâ çalıştığını doğrula.

### Beklenen Sonuç
- Projede `AppDataStore` sınıfı ve referansı hiçbir yerde kalmamış olmalı.
- Uygulama sorunsuz çalışmaya devam etmeli.

---

## GÖREV 1.2 — BaseEntity Oluştur ve Tüm Entity'lere Uygula

### Neden
Mevcut entity'lerde ortak bir base class yok. `CreatedAt` yalnızca `Lead` ve `AuditLog`'da var. `CreatedBy`, `UpdatedAt`, `UpdatedBy` hiçbir entity'de yok. Bu sprint'ten sonraki tüm işler (servis katmanı, audit zenginleştirme, soft delete) bu temele bağlı.

### Adımlar

1. `Domain/` klasöründe `BaseEntity.cs` dosyası oluştur:

```csharp
namespace InsuranceManagement.Web.Domain;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
```

2. Aşağıdaki entity'leri `BaseEntity`'den türet:
   - `UserAccount`
   - `Employee`
   - `Lead`
   - `Account`
   - `Activity`
   - `Sale`
   - `Expense`
   - `ImportBatch`

   Her entity'de zaten var olan `Id` property'sini kaldır (artık base'den geliyor). `Lead`'deki mevcut `CreatedAt` property'sini de kaldır — artık base'den gelecek.

3. **`AuditLog` entity'si `BaseEntity`'den türemeyecek.** AuditLog bir log tablosudur, kendi yapısını korumalıdır. Sadece `Id` property'sini tutmaya devam etsin.

4. `AppDbContext`'e audit alan otomatik doldurma mekanizması ekle. Bunun için önce constructor'a `IHttpContextAccessor` inject et:

```csharp
private readonly IHttpContextAccessor _httpContextAccessor;

public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) 
    : base(options)
{
    _httpContextAccessor = httpContextAccessor;
}
```

5. Mevcut `NormalizeDateTimes()` metodunun çağrıldığı tüm `SaveChanges` override'larında, `NormalizeDateTimes()` çağrısından **önce** audit alanlarını dolduran yeni bir metod çağır:

```csharp
private void ApplyAuditFields()
{
    var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = DateTime.UtcNow;
            entry.Entity.CreatedBy = userName;
        }

        if (entry.State == EntityState.Modified)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
            entry.Entity.UpdatedBy = userName;
        }
    }
}
```

Tüm `SaveChanges` ve `SaveChangesAsync` override'larında:
```csharp
ApplyAuditFields();
NormalizeDateTimes();
```

6. `OnModelCreating`'de BaseEntity'den gelen audit alanları için ortak konfigürasyon ekle (mevcut entity konfigürasyonlarının **sonuna**):

```csharp
foreach (var entityType in modelBuilder.Model.GetEntityTypes()
    .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
{
    modelBuilder.Entity(entityType.ClrType, b =>
    {
        b.Property("CreatedBy").HasMaxLength(150);
        b.Property("UpdatedBy").HasMaxLength(150);
    });
}
```

7. `Program.cs`'de `IHttpContextAccessor` zaten register ediliyor (`builder.Services.AddHttpContextAccessor()`), ek bir işlem gerekmez. Ancak `DbSeeder`'da `AppDbContext` constructor'ının artık `IHttpContextAccessor` beklediğini dikkate al — seeding sırasında `IHttpContextAccessor` scope'dan çözümlenmeli. Eğer sorun olursa, `AppDbContext` constructor'ında `IHttpContextAccessor`'ı nullable yap:

```csharp
public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null) 
    : base(options)
{
    _httpContextAccessor = httpContextAccessor;
}
```

Ve `ApplyAuditFields`'da:
```csharp
var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
```

8. Projeyi derle ve çalıştır.

### Beklenen Sonuç
- Tüm entity'ler (AuditLog hariç) `BaseEntity`'den türüyor.
- Yeni kayıt oluşturulduğunda `CreatedAt` ve `CreatedBy` otomatik doluyor.
- Kayıt güncellendiğinde `UpdatedAt` ve `UpdatedBy` otomatik doluyor.
- Giriş yapmış kullanıcının adı `CreatedBy`/`UpdatedBy` alanlarına yazılıyor.

---

## GÖREV 1.3 — Şifre Hash'leme (BCrypt)

### Neden
Mevcut durumda `AuthController.cs`'de şifre karşılaştırması düz metin (`Password == model.Password`) yapılıyor. Bu ciddi bir güvenlik açığıdır.

### Adımlar

1. BCrypt paketini ekle:
   ```
   dotnet add package BCrypt.Net-Next
   ```

2. `UserAccount` entity'sinde `Password` property adını `PasswordHash` olarak değiştir:
   ```csharp
   public string PasswordHash { get; set; } = string.Empty;
   ```

3. `AppDbContext.OnModelCreating` içindeki `UserAccount` konfigürasyonunda property adını güncelle:
   ```csharp
   entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
   ```

4. `AuthController.cs`'deki login metodunda şifre kontrolünü değiştir:

   **Eski:**
   ```csharp
   var user = db.Users.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
   ```

   **Yeni:**
   ```csharp
   var user = db.Users.FirstOrDefault(x => x.UserName == model.UserName);
   if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
   {
       // login failed
   }
   ```

5. `DbSeeder.cs`'de seed kullanıcıları oluştururken şifreleri hash'le:
   ```csharp
   PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123")
   ```
   
   Tüm seed kullanıcılarında aynı yaklaşımı uygula.

6. Projede `Password` referansı kalan her yeri tara ve `PasswordHash`'e güncelle:
   ```
   grep -r "\.Password" --include="*.cs" .
   ```
   ViewModel'lerde kullanıcı girişi için `Password` property adı kalabilir (bu form alanı), ama entity'de ve DB'de `PasswordHash` olmalı.

7. Eğer projede kullanıcı oluşturma veya şifre güncelleme ekranı varsa, orada da hash'leme uygula.

8. Projeyi derle ve çalıştır. Demo kullanıcılarla giriş yapılabildiğini doğrula.

### Beklenen Sonuç
- Veritabanında hiçbir şifre düz metin olarak tutulmuyor.
- Login akışı BCrypt.Verify ile çalışıyor.
- Tüm seed kullanıcılarıyla giriş yapılabiliyor.

---

## GÖREV 1.4 — Migration Sıfırlama ve Yeniden Oluşturma

### Neden
GÖREV 1.2 ve 1.3'te entity yapısında önemli değişiklikler yapıldı (BaseEntity eklendi, audit alanları eklendi, Password → PasswordHash değişti). Mevcut migration'lar eski yapıya göre oluşturulmuş. MVP aşamasında olduğumuz için migration'ları sıfırdan oluşturmak en temiz yaklaşımdır.

### Adımlar

1. Mevcut migration dosyalarını sil:
   ```
   rm -rf Data/Migrations/*
   ```
   
2. PostgreSQL veritabanını sıfırla (Docker kullanılıyorsa):
   ```
   docker compose down -v
   docker compose up -d
   ```
   
   Veya doğrudan:
   ```
   dotnet ef database drop --force
   ```

3. Yeni migration oluştur:
   ```
   dotnet ef migrations add Sprint1_Init
   ```

4. Migration dosyasını gözden geçir — tüm tablolarda `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` kolonlarının olduğunu, `users` tablosunda `PasswordHash` kolonunun olduğunu doğrula.

5. Uygulamayı çalıştır — `Program.cs`'deki `db.Database.Migrate()` ve `DbSeeder.SeedAsync(db)` otomatik çalışacaktır.

6. Tüm modülleri test et:
   - Auth: giriş/çıkış
   - Lead: oluştur/düzenle/sil
   - Account: oluştur/düzenle
   - Activity: oluştur/düzenle/sil
   - Sale: oluştur/düzenle/sil
   - Expense: oluştur/düzenle/sil
   - Dashboard: verilerin göründüğünü doğrula
   - Import: dosya yüklemenin çalıştığını doğrula
   - Audit: audit log ekranında kayıtların göründüğünü doğrula

### Beklenen Sonuç
- Temiz bir migration dosyası var.
- PostgreSQL'de tüm tablolar yeni şemayla oluşturulmuş.
- Seed verisi yüklenmiş.
- Tüm CRUD akışları çalışıyor.

---

## GÖREV 1.5 — Controller'larda Manuel Audit Kodunu Temizle

### Neden
Bazı controller'larda `CreatedAt = DateTime.UtcNow` gibi manuel atamalar olabilir. Artık `AppDbContext.ApplyAuditFields()` bunu otomatik yapıyor, dolayısıyla controller'lardaki manuel atamalar gereksiz ve hatta çakışma riski taşıyor.

### Adımlar

1. Tüm controller'larda şu pattern'leri ara:
   ```
   grep -rn "CreatedAt\s*=" --include="*.cs" Controllers/
   grep -rn "UpdatedAt\s*=" --include="*.cs" Controllers/
   grep -rn "CreatedBy\s*=" --include="*.cs" Controllers/
   grep -rn "UpdatedBy\s*=" --include="*.cs" Controllers/
   ```

2. Bulunan satırları kaldır. `ApplyAuditFields()` zaten `SaveChanges` çağrısında otomatik dolduracak.

3. **Dikkat:** Eğer bir controller'da `CreatedAt` özel bir değerle set ediliyorsa (örneğin import sırasında kaynak dosyadaki tarihin kullanılması), o durumu değerlendir ve koru. Sadece `DateTime.UtcNow` veya `DateTime.Now` ile yapılan atamaları kaldır.

4. Projeyi derle, çalıştır ve kayıt oluşturma/güncelleme akışlarında audit alanlarının hâlâ düzgün dolduğunu doğrula.

### Beklenen Sonuç
- Controller'larda manuel audit ataması kalmamış.
- Tüm audit alanları tek merkezden (DbContext) yönetiliyor.

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

Sprint 1 kod görevleri tamamlandıktan sonra aşağıdaki dokümanlar güncellenmelidir.

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne şu satırları ekle:

```
- AppDataStore singleton kaldirildi, tum veri erisimi AppDbContext uzerinden saglandi
- BaseEntity abstract sinifi olusturuldu; tum transaction entity'leri (AuditLog haric) bu siniftan turetildi
- CreatedAt, CreatedBy, UpdatedAt, UpdatedBy audit alanlari tum entity'lere eklendi
- Audit alanlari AppDbContext.SaveChanges icerisinde otomatik dolduruluyor
- Sifre hashleme BCrypt ile uygulamaya alindi, plain text sifre kaldirildi
- Migration sifirdan olusturuldu ve PostgreSQL ile dogrulandi
- Controller'lardaki manuel audit atamalari temizlendi
```

`## Current Focus` bölümünü şu şekilde güncelle:

```
## Current Focus

- Entity modelini ERD ile hizalama (referans tablolar, navigation properties)
- Enum degerlerinin referans tablolara tasınması
- Veri modeli kalitesini guclendirme
```

`## Next Up` bölümünü şu şekilde güncelle:

```
## Next Up

- Referans tablolarini olusturma (lead_status_types, activity_contact_status_types, activity_outcome_status_types, insurance_product_types, expense_types, lead_source_types)
- Enum → FK gecisi
- Navigation property ekleme
- LeadAssignment tablosu ekleme
- Servis katmani olusturma (ILeadService, IActivityService, ISaleService, IExpenseService)
- Validation altyapisi (FluentValidation)
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19. Teknik Mimari Önerisi` bölümünden sonra veya uygun bir yere şu yeni bölümü ekle:

```markdown
## 19.1 Uygulanan Mimari İyileştirmeler

### Sprint 1 — Temel Temizlik ve Güvenlik (Tamamlandı)

Aşağıdaki mimari iyileştirmeler uygulanmıştır:

- **AppDataStore kaldırıldı**: In-memory singleton veri deposu tamamen kaldırılmış, tüm veri erişimi `AppDbContext` üzerinden sağlanmaktadır.
- **BaseEntity sınıfı oluşturuldu**: Tüm transaction entity'leri `BaseEntity`'den türemektedir. Bu sınıf `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` alanlarını içerir.
- **Otomatik audit alan doldurma**: `AppDbContext.SaveChanges` override'ı içinde `ChangeTracker` kullanılarak audit alanları otomatik doldurulmaktadır. Giriş yapan kullanıcının adı `IHttpContextAccessor` üzerinden alınmaktadır.
- **Şifre güvenliği**: Kullanıcı şifreleri BCrypt ile hash'lenerek saklanmaktadır. Plain text şifre sistemden tamamen kaldırılmıştır.
- **AuditLog bağımsızlığı**: `AuditLog` entity'si `BaseEntity`'den türetilmemiştir; kendi bağımsız yapısını korumaktadır.
```

### DOC-3: `agent/project_analysis.md` Güncelle

`## 4. Kritik İyileştirme Alanları` bölümünde şu maddeleri `[ÇÖZÜLDÜ]` olarak işaretle veya güncelle:

- `4.1 Mimari Borç — Dual Data Store` → Başlığın yanına `[ÇÖZÜLDÜ — Sprint 1]` ekle
- `4.2 Güvenlik Riskleri` tablosunda `Düz metin şifre` satırını `[ÇÖZÜLDÜ — Sprint 1]` olarak işaretle
- `4.3 Entity Model Eksikleri` tablosunda `CreatedAt/UpdatedAt` ve `CreatedBy/UpdatedBy` satırlarını `[ÇÖZÜLDÜ — Sprint 1]` olarak işaretle

---

## SPRINT 1 TAMAMLANMA KONTROL LİSTESİ

Aşağıdaki tüm maddeler doğrulanmadan sprint tamamlanmış sayılmaz:

- [ ] Projede `AppDataStore` sınıfı ve referansı hiçbir yerde yok
- [ ] Tüm entity'ler (AuditLog hariç) `BaseEntity`'den türüyor
- [ ] `CreatedAt` ve `CreatedBy` yeni kayıt oluşturulduğunda otomatik doluyor
- [ ] `UpdatedAt` ve `UpdatedBy` kayıt güncellendiğinde otomatik doluyor
- [ ] Giriş yapan kullanıcının adı audit alanlarına yazılıyor
- [ ] Şifreler BCrypt ile hash'leniyor
- [ ] Veritabanında plain text şifre yok
- [ ] Demo kullanıcılarla giriş yapılabiliyor
- [ ] Tüm CRUD akışları çalışıyor (Lead, Account, Activity, Sale, Expense)
- [ ] Dashboard ekranları veri gösteriyor
- [ ] Import modülü çalışıyor
- [ ] Audit log ekranı kayıt gösteriyor
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
- [ ] `project_analysis.md` güncellenmiş
- [ ] Controller'larda manuel audit ataması kalmamış
- [ ] Migration temiz ve tekrar oluşturulabilir durumda
