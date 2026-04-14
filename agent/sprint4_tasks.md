# Sprint 4 — Rol Bazlı Erişim Kontrolü ve Veri İzolasyonu

## Genel Talimat

Bu sprint, `role_permission_matrix.md` ve `visibility_rules.md` dokümanlarındaki kuralları koda uygular. Amaç: her rol yalnızca yetkili olduğu modüllere erişsin, her kullanıcı yalnızca görmesi gereken verileri görsün.

**Önemli not:** Sprint 3'te agent bazı rol bazlı işleri zaten kısmen yapmış — dashboard'da FieldSales kendi rakamlarını görüyor, masraf girişinde FieldSales sadece kendi adına girebiliyor. Bu sprint bu parçalı işleri sistemleştirip tüm modüllere tutarlı şekilde yayacak.

**6 roller:** Admin, Manager, SalesManager, Operations, FieldSales, CallCenter

**Temel prensip:** Yetki kontrolü iki katmanda uygulanacak:
1. **Erişim kontrolü** — Controller action'larında `[Authorize(Roles=...)]` ile hangi roller hangi action'a erişebilir
2. **Veri izolasyonu** — Servis katmanında kullanıcının rolüne ve kimliğine göre veri filtreleme

**Görevler sıralıdır. Her görev tamamlandığında proje derlenip doğrulanmalıdır.**

---

## GÖREV 4.1 — Yetki Bilgisi Altyapısı

### Neden
Servis katmanının veri filtreleme yapabilmesi için giriş yapan kullanıcının rolünü, userId'sini ve bağlı employeeId'sini bilmesi gerekiyor. Şu an bu bilgi `ClaimsPrincipal` üzerinden taşınıyor ama servislerde standart bir erişim yolu yok.

### Adımlar

1. Mevcut `ClaimsPrincipalExtensions.cs`'i kontrol et. Aşağıdaki extension metotların olduğundan emin ol (yoksa ekle):

```csharp
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
        => int.Parse(principal.FindFirst("UserId")?.Value ?? "0");

    public static string GetRole(this ClaimsPrincipal principal)
        => principal.FindFirst(ClaimTypes.Role)?.Value ?? "";

    public static int? GetEmployeeId(this ClaimsPrincipal principal)
    {
        var val = principal.FindFirst("EmployeeId")?.Value;
        return string.IsNullOrEmpty(val) ? null : int.Parse(val);
    }

    public static string GetFullName(this ClaimsPrincipal principal)
        => principal.FindFirst("FullName")?.Value ?? "";

    public static bool IsInRole(this ClaimsPrincipal principal, params string[] roles)
        => roles.Any(r => principal.IsInRole(r));
}
```

2. `AuthController.Login`'da Claims oluşturulurken `UserId`, `EmployeeId`, `FullName` ve `Role` claim'lerinin doğru eklendiğini doğrula. Özellikle `EmployeeId` — FieldSales kullanıcıları için bu zorunlu çünkü veri izolasyonu buna bağlı.

3. Derle.

### Beklenen Sonuç
- Tüm claim extension metotları kullanıma hazır.

---

## GÖREV 4.2 — Controller Erişim Kontrolü (Authorize Attribute)

### Neden
Mevcut durumda controller action'larında yetki kontrolü ya eksik ya da yetersiz. `role_permission_matrix.md`'deki matris koda uygulanmalı.

### Adımlar

Aşağıdaki tabloyu referans alarak her controller'a `[Authorize(Roles=...)]` attribute ekle:

#### LeadsController
```csharp
// Sınıf seviyesi — tüm giriş yapmış kullanıcılar erişebilir
[Authorize]
public class LeadsController : AppController
{
    // Index (Liste) — tüm roller görebilir ama veri scope farklı olacak (Görev 4.3'te)
    public IActionResult Index() { ... }

    // Create — Admin, CallCenter, Operations
    [Authorize(Roles = "Admin,CallCenter,Operations")]
    [HttpPost]
    public IActionResult Create() { ... }

    // BulkSave — Admin, SalesManager, CallCenter
    [Authorize(Roles = "Admin,SalesManager,CallCenter")]
    [HttpPost]
    public IActionResult BulkSave() { ... }

    // Assign — Admin, SalesManager, Manager
    [Authorize(Roles = "Admin,SalesManager,Manager")]
    [HttpPost]
    public IActionResult Assign() { ... }

    // Assignments sayfası — Admin, SalesManager, Manager
    [Authorize(Roles = "Admin,SalesManager,Manager")]
    public IActionResult Assignments() { ... }

    // StartVisit — Admin, SalesManager, FieldSales
    [Authorize(Roles = "Admin,SalesManager,FieldSales")]
    [HttpPost]
    public IActionResult StartVisit() { ... }

    // Delete — Admin
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Delete() { ... }
}
```

#### ActivitiesController
```csharp
[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class ActivitiesController : AppController
{
    // Create — Admin, Operations, FieldSales
    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    public IActionResult Create() { ... }

    // Edit — Admin, Operations, FieldSales (kendi kaydı — veri izolasyonu Görev 4.3'te)
    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    public IActionResult Edit() { ... }

    // Delete — Admin, Operations
    [Authorize(Roles = "Admin,Operations")]
    [HttpPost]
    public IActionResult Delete() { ... }
}
```

#### SalesController
```csharp
[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class SalesController : AppController
{
    // Create — Admin, Operations, FieldSales
    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    public IActionResult Create() { ... }

    // Edit — Admin, Operations (sınırlı), SalesManager (yönetsel)
    [Authorize(Roles = "Admin,Operations,SalesManager")]
    [HttpPost]
    public IActionResult Edit() { ... }

    // Delete — Admin
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Delete() { ... }
}
```

#### ExpensesController
```csharp
[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class ExpensesController : AppController
{
    // Create — Admin, FieldSales (kendi adına)
    [Authorize(Roles = "Admin,Manager,Operations,FieldSales")]
    [HttpPost]
    public IActionResult Create() { ... }

    // Delete — Admin
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Delete() { ... }
}
```

#### AccountsController
```csharp
[Authorize(Roles = "Admin,Manager,SalesManager,Operations,FieldSales")]
public class AccountsController : AppController
{
    // Create/Edit — Admin, Operations, FieldSales (kendi bağlamında)
    [Authorize(Roles = "Admin,Operations,FieldSales")]
    [HttpPost]
    public IActionResult Create() { ... }
}
```

#### EmployeesController
```csharp
[Authorize(Roles = "Admin,Manager,SalesManager,Operations")]
public class EmployeesController : AppController { ... }
```

#### DashboardController
```csharp
[Authorize]  // Tüm roller erişebilir ama göreceği veri farklı (Görev 4.3'te)
public class DashboardController : AppController { ... }
```

#### AdminController (Audit, Kullanıcı Yönetimi)
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : AppController { ... }
```

#### ImportsController
```csharp
[Authorize(Roles = "Admin,Operations")]
public class ImportsController : AppController { ... }
```

**CallCenter özel durumu:** CallCenter kullanıcıları yalnızca Lead modülüne erişebilir. Diğer controller'lardaki sınıf seviyesi `[Authorize]` attribute'ları zaten CallCenter'ı dışarıda bırakıyor.

Derle ve her rolden giriş yaparak erişim kontrolünü doğrula.

### Beklenen Sonuç
- Tüm controller action'larında `role_permission_matrix.md`'ye uygun `[Authorize]` attribute'ları tanımlı.
- Yetkisiz erişimde kullanıcı `/Auth/AccessDenied` sayfasına yönlendiriliyor.

---

## GÖREV 4.3 — Servis Katmanında Veri İzolasyonu

### Neden
Erişim kontrolü "bu sayfayı görebilir misin?" sorusunu cevaplıyor. Ama veri izolasyonu "bu sayfadaki hangi kayıtları görebilirsin?" sorusunu cevaplıyor. FieldSales yalnızca kendi kayıtlarını görmeli.

### Adımlar

1. Her servisteki `GetAll` metoduna kullanıcı bilgisi parametresi ekle. `IHttpContextAccessor` zaten inject edilmiş durumda — buradan rol ve employeeId çekilebilir.

2. **LeadService.GetAll** — veri filtreleme:

```csharp
public List<Lead> GetAll(int page, int pageSize, out int totalCount)
{
    var user = _httpContextAccessor.HttpContext?.User;
    var role = user?.GetRole();
    var employeeId = user?.GetEmployeeId();

    var query = _db.Leads
        .Include(x => x.LeadStatusType)
        .Include(x => x.LeadSourceType)
        .Include(x => x.AssignedEmployee)
        .AsQueryable();

    // Veri izolasyonu
    if (role == "FieldSales" && employeeId.HasValue)
    {
        query = query.Where(x => x.AssignedEmployeeId == employeeId.Value);
    }
    else if (role == "CallCenter")
    {
        // CallCenter tüm lead'leri görebilir (kendi oluşturduklarını ve havuzdakileri)
    }

    query = query.OrderByDescending(x => x.CreatedAt);
    totalCount = query.Count();

    return query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();
}
```

3. **ActivityService.GetAll** — veri filtreleme:

```csharp
if (role == "FieldSales" && employeeId.HasValue)
{
    query = query.Where(x => x.EmployeeId == employeeId.Value);
}
```

4. **SaleService.GetAll** — aynı pattern:

```csharp
if (role == "FieldSales" && employeeId.HasValue)
{
    query = query.Where(x => x.EmployeeId == employeeId.Value);
}
```

5. **ExpenseService.GetAll** — aynı pattern:

```csharp
if (role == "FieldSales" && employeeId.HasValue)
{
    query = query.Where(x => x.EmployeeId == employeeId.Value);
}
```

6. **GetById** metotlarında da izolasyon kontrolü yap — FieldSales başkasının kaydına doğrudan URL ile erişememeli:

```csharp
public Activity? GetById(int id)
{
    var activity = _db.Activities
        .Include(...)
        .FirstOrDefault(x => x.Id == id);

    if (activity == null) return null;

    var user = _httpContextAccessor.HttpContext?.User;
    var role = user?.GetRole();
    var employeeId = user?.GetEmployeeId();

    // FieldSales başkasının kaydını göremez
    if (role == "FieldSales" && employeeId.HasValue && activity.EmployeeId != employeeId.Value)
        return null;

    return activity;
}
```

7. **Edit ve Delete** action'larında da aynı kontrol — FieldSales sadece kendi kaydını düzenleyebilir/silebilir:

Controller'da:
```csharp
var entity = _service.GetById(id);
if (entity == null)
{
    TempData["Warning"] = "Kayıt bulunamadı veya bu kaydı görüntüleme yetkiniz yok.";
    return RedirectToAction(nameof(Index));
}
```

8. **DashboardService** — Sprint 3'te zaten kısmen yapılmış. Kontrol et ve eksik varsa tamamla:
   - FieldSales: kendi aktivite, satış, masraf verileri
   - Manager/Admin: tüm veriler
   - SalesManager: ekip ve saha verileri
   - Operations: operasyonel görünüm

9. Derle ve test et.

### Beklenen Sonuç
- FieldSales kullanıcısı giriş yaptığında sadece kendi lead, aktivite, satış ve masraf kayıtlarını görüyor.
- URL ile başkasının kaydına erişmeye çalışınca "Kayıt bulunamadı" görüyor.
- Manager/Admin tüm verileri görüyor.

---

## GÖREV 4.4 — Menü ve Navigasyon Rol Bazlı Filtreleme

### Neden
Kullanıcı erişemediği modülün menü linkini görmemeli. CallCenter kullanıcısı sadece Lead menüsünü görmeli. FieldSales Admin veya Import menüsünü görmemeli.

### Adımlar

1. `AppController.BuildShell()` metodunu veya `_Layout.cshtml`'deki menü oluşturma mantığını güncelle. Kullanıcının rolüne göre menü öğelerini filtrele:

```csharp
// Menü öğeleri rol bazlı
var menuItems = new List<MenuItem>();

if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("SalesManager") 
    || User.IsInRole("Operations") || User.IsInRole("FieldSales") || User.IsInRole("CallCenter"))
{
    menuItems.Add(new MenuItem("Lead Havuzu", "/Leads"));
}

if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("SalesManager") 
    || User.IsInRole("Operations") || User.IsInRole("FieldSales"))
{
    menuItems.Add(new MenuItem("Aktiviteler", "/Activities"));
    menuItems.Add(new MenuItem("Satışlar", "/Sales"));
    menuItems.Add(new MenuItem("Masraflar", "/Expenses"));
    menuItems.Add(new MenuItem("Müşteriler", "/Accounts"));
}

if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("SalesManager") 
    || User.IsInRole("Operations"))
{
    menuItems.Add(new MenuItem("Personel", "/Employees"));
}

if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("SalesManager") 
    || User.IsInRole("Operations") || User.IsInRole("FieldSales") || User.IsInRole("CallCenter"))
{
    menuItems.Add(new MenuItem("Dashboard", "/Dashboard"));
}

if (User.IsInRole("Admin") || User.IsInRole("Operations"))
{
    menuItems.Add(new MenuItem("Import", "/Imports"));
}

if (User.IsInRole("Admin"))
{
    menuItems.Add(new MenuItem("Yönetim", "/Admin"));
}
```

2. Eğer menü `_Layout.cshtml`'de statik HTML olarak yazılmışsa, Razor `@if` bloklarıyla filtrele:

```html
@if (User.IsInRole("Admin") || User.IsInRole("Manager") || ...)
{
    <li><a href="/Activities">Aktiviteler</a></li>
}
```

3. Her rolden giriş yaparak menüyü doğrula:
   - **Admin:** tüm menüler
   - **Manager:** Lead, Aktivite, Satış, Masraf, Müşteri, Personel, Dashboard
   - **SalesManager:** Lead, Aktivite, Satış, Masraf, Müşteri, Personel, Dashboard
   - **Operations:** Lead, Aktivite, Satış, Masraf, Müşteri, Personel, Dashboard, Import
   - **FieldSales:** Lead, Aktivite, Satış, Masraf, Müşteri, Dashboard (kendi verileri)
   - **CallCenter:** Lead, Dashboard (lead odaklı)

### Beklenen Sonuç
- Her rol yalnızca yetkili olduğu modüllerin menü linklerini görüyor.

---

## GÖREV 4.5 — FieldSales Kayıt Sahipliği Kontrolü

### Neden
FieldSales kullanıcısı aktivite, satış veya masraf oluştururken `EmployeeId` olarak sadece kendi bağlı olduğu employee'yi seçebilmeli. Başka bir personel adına kayıt oluşturamamalı.

### Adımlar

1. FieldSales kullanıcısı kayıt oluşturduğunda, `EmployeeId` otomatik olarak kendi `EmployeeId`'si ile doldurulmalı. Formda personel dropdown'ı ya gizlenmeli ya da sadece kendi ismi görünmeli.

2. Controller'daki Create action'larda:

```csharp
if (User.GetRole() == "FieldSales")
{
    var employeeId = User.GetEmployeeId();
    if (employeeId.HasValue)
    {
        model.EmployeeId = employeeId.Value;
        // Dropdown'da sadece kendi ismini göster
        ViewBag.Employees = _db.Employees
            .Where(e => e.Id == employeeId.Value)
            .ToList();
    }
}
else
{
    ViewBag.Employees = _db.Employees
        .Where(e => e.IsActive)
        .OrderBy(e => e.FullName)
        .ToList();
}
```

3. POST action'da güvenlik kontrolü — FieldSales başka birinin EmployeeId'sini gönderemez:

```csharp
if (User.GetRole() == "FieldSales")
{
    var myEmployeeId = User.GetEmployeeId();
    if (entity.EmployeeId != myEmployeeId)
    {
        TempData["Warning"] = "Yalnızca kendi adınıza kayıt oluşturabilirsiniz.";
        return RedirectToAction(nameof(Index));
    }
}
```

4. Bu kontrolü Activity, Sale ve Expense Create/Edit action'larında uygula.

5. Derle ve test et.

### Beklenen Sonuç
- FieldSales kullanıcısı sadece kendi adına kayıt oluşturabiliyor.
- Formda personel listesi sadece kendini gösteriyor.

---

## GÖREV 4.6 — Migration ve Kapsamlı Test

### Neden
Bu sprint'te entity'lere yeni alan eklenmedi, sadece davranış değişiklikleri var. Ama soft delete'e Lead eklenmişse ve daha önce migration yapılmadıysa migration gerekebilir.

### Adımlar

1. Entity'lerde yeni alan eklendiyse migration oluştur. Eğer sadece davranış değişikliği varsa migration gerekmez.

2. **Kapsamlı rol bazlı test:**

Her test için ilgili kullanıcıyla giriş yap. DbSeeder'da her rol için bir demo kullanıcı olmalı.

#### Admin ile test:
- Tüm menüler görünüyor mu?
- Tüm modüllerde CRUD yapılabiliyor mu?
- Tüm kayıtlar (tüm personellerin) görünüyor mu?
- Import ve Yönetim menüleri erişilebilir mi?

#### FieldSales ile test:
- Menüde sadece Lead, Aktivite, Satış, Masraf, Müşteri, Dashboard görünüyor mu?
- Lead listesinde sadece kendine atanmış lead'ler var mı?
- Aktivite listesinde sadece kendi aktiviteleri var mı?
- Satış listesinde sadece kendi satışları var mı?
- Masraf listesinde sadece kendi masrafları var mı?
- Yeni aktivite oluştururken personel dropdown'ında sadece kendi ismi var mı?
- URL'ye `/Admin` yazınca erişim reddedildi sayfası mı geliyor?
- URL'ye başka birinin aktivite ID'sini yazınca "Kayıt bulunamadı" mı geliyor?
- Dashboard'da sadece kendi rakamları mı görünüyor?

#### CallCenter ile test:
- Menüde sadece Lead ve Dashboard görünüyor mu?
- Lead oluşturabiliyor mu?
- URL'ye `/Activities` yazınca erişim reddedildi mi?
- URL'ye `/Sales` yazınca erişim reddedildi mi?

#### Manager ile test:
- Tüm kayıtları görebiliyor mu?
- Düzenleme/silme yapamıyor mu? (Sadece okuma)

#### Operations ile test:
- Kayıtları görebiliyor mu?
- Sınırlı düzenleme yapabiliyor mu?
- Import erişilebilir mi?

### Beklenen Sonuç
- Her rol yalnızca yetkili olduğu ekranları görüyor ve yetkili olduğu verilere erişiyor.

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne ekle:

```
- Tum controller'lara rol bazli [Authorize] attribute eklendi
- Servis katmaninda veri izolasyonu uygulanidi (FieldSales kendi verisi)
- CallCenter yalnizca Lead modulune erisebiliyor
- Menu ve navigasyon rol bazli filtrelendi
- FieldSales kayit sahipligi kontrolu eklendi (baskasi adina kayit olusturamaz)
- GetById metotlarinda rol bazli erisim kontrolu eklendi
- URL ile yetkisiz kayda erisim engellendi
```

`## Current Focus` bölümünü güncelle:

```
## Current Focus

- Dashboard ve KPI guclendirme
- KPI sozlugu ile hizalama
- Filtreleme altyapisi
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19.1 Uygulanan Mimari İyileştirmeler` bölümüne ekle:

```markdown
### Sprint 4 — Rol Bazlı Erişim Kontrolü ve Veri İzolasyonu (Tamamlandı)

- Tüm controller action'larına `role_permission_matrix.md`'ye uygun `[Authorize(Roles=...)]` attribute'ları eklendi.
- Servis katmanında veri izolasyonu uygulandı: FieldSales yalnızca kendi kayıtlarını (lead, aktivite, satış, masraf) görebilir ve düzenleyebilir.
- CallCenter kullanıcıları yalnızca Lead modülüne erişebilir; diğer modüller erişim reddeder.
- Menü ve navigasyon kullanıcının rolüne göre dinamik olarak filtrelenmektedir.
- FieldSales kayıt sahipliği kontrolü: kullanıcı başka bir personel adına kayıt oluşturamaz.
- URL ile yetkisiz erişim engellendi: GetById metotlarında rol bazlı kontrol yapılmaktadır.
```

---

## SPRINT 4 TAMAMLANMA KONTROL LİSTESİ

- [ ] Tüm controller'larda [Authorize(Roles=...)] attribute tanımlı
- [ ] FieldSales — Lead listesinde sadece kendine atanmış lead'ler
- [ ] FieldSales — Aktivite/Satış/Masraf listesinde sadece kendi kayıtları
- [ ] FieldSales — Başka personel adına kayıt oluşturamıyor
- [ ] FieldSales — URL ile başkasının kaydına erişemiyor
- [ ] FieldSales — Formda personel dropdown'ında sadece kendi ismi
- [ ] FieldSales — Dashboard'da kendi rakamları
- [ ] CallCenter — Sadece Lead ve Dashboard menüsü görünüyor
- [ ] CallCenter — /Activities, /Sales, /Expenses erişim reddediliyor
- [ ] Manager — Tüm verileri görebiliyor, CRUD sınırlı
- [ ] Operations — Import erişilebilir, sınırlı düzenleme
- [ ] Admin — Tam erişim
- [ ] Menü rol bazlı filtreleniyor
- [ ] AccessDenied sayfası çalışıyor
- [ ] Migration (gerekliyse) temiz
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
