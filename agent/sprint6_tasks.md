# Sprint 6 — Lead Detail Hub (360° Lead Sayfası)

## Genel Talimat

Bu sprint, Lead modülünün kullanıcı deneyimini köklü şekilde değiştirir. Mevcut Lead detay sayfası basit bir bilgi görüntüleme ekranı iken, bu sprint sonunda o lead'le ilgili **her şeyi** tek sayfada gösteren bir 360° çalışma merkezi olacak.

**Bu sprint Kanban board içermiyor.** Kanban board bir sonraki sprint'te (Sprint 7) yapılacak. Bu sprint'in odağı tamamen Lead Detail Hub sayfası.

**Neden bu önemli:** Gerçek operasyonda kullanıcılar "Lead X ile ilgili ne oldu?" sorusunu sürekli soruyor. Şu an bu sorunun cevabı için Lead detay, Account detay, Activity listesi, Sale listesi arasında gezinmek gerekiyor. Hub sayfasıyla tek tıkla her şey görünecek.

**Sprint 3 parçaya bölünmüş — agent token limitine takılmasın diye küçük parçalar halinde ilerleyecek.**

---

## PARÇA A — Lead Detail Hub Sayfası Temel Yapısı

### Amaç
Mevcut Lead detay sayfasını genişleterek 360° bir hub'a dönüştürmek. Tek sayfada lead'in tüm bağlamını göstermek.

### Hub Sayfası Bölümleri

Sayfa yukarıdan aşağıya şu bölümlerden oluşacak:

#### 1. Üst Bölüm — Lead Bilgi Kartı
- Lead kodu, potansiyel adı, telefon, email
- Şehir / İlçe
- Kaynak (Çağrı Merkezi, Manuel, Referans)
- Mevcut durum (badge olarak — renkli etiket)
- Oluşturulma tarihi, oluşturan kullanıcı
- Lead notu

#### 2. Durum ve Aksiyon Çubuğu
Lead'in mevcut durumuna göre yapılabilecek aksiyonları gösteren butonlar:

| Mevcut Durum | Görünecek Aksiyonlar |
|---|---|
| NEW | Araştır → RESEARCHED |
| RESEARCHED | İletişim Bulundu → CONTACT_FOUND |
| CONTACT_FOUND | Atamaya Hazır → READY_FOR_ASSIGNMENT |
| READY_FOR_ASSIGNMENT | Personele Ata (modal açılır) |
| ASSIGNED | Ziyaret Planla, Ziyaret Başlat |
| VISIT_SCHEDULED | Ziyaret Başlat |
| VISITED | Aktiviteye Dönüştür |
| CONVERTED_TO_ACTIVITY | (aksiyon yok — tamamlanmış) |
| PLANNED | Gerçekleştir |

Her durum için "Disqualify" (İptal Et) butonu da her zaman görünmeli — lead'i pipeline'dan çıkarmak için.

**Durum geçişleri `LeadService` üzerinden yapılmalı.** Controller'da doğrudan status değiştirmek yerine `leadService.ChangeStatus(leadId, newStatusCode)` çağrılmalı.

#### 3. Atama Bilgisi Paneli
- Atanan personel adı ve tarihi
- Atayan kişi
- Öncelik ve son tarih (varsa)
- Atama notu
- Eğer atama yoksa: "Henüz atanmadı" mesajı
- Atama geçmişi (varsa birden fazla atama yapıldıysa — `LeadAssignment` tablosundan)

#### 4. Bağlı Müşteri/Firma Paneli
- Eğer lead bir Account ile eşleştirilmişse: Account adı, tipi, şehir, telefon — tıklanınca Account detaya gider
- Eğer eşleştirilmemişse: "Henüz müşteri kaydı oluşturulmadı" mesajı

#### 5. Aktivite Geçmişi (Timeline)
- Bu lead'den oluşturulmuş aktivitelerin listesi (zaman sırasına göre)
- Her aktivite kartında: tarih, personel, temas durumu, sonuç, özet
- Tıklanınca aktivite detayına gider
- Eğer aktivite yoksa: "Henüz aktivite oluşturulmadı"

#### 6. Satışlar Paneli
- Bu lead'e bağlı account üzerinden yapılmış satışlar
- Her satış kartında: tarih, ürün tipi, tutar, personel
- Eğer satış yoksa: "Henüz satış kaydı yok"

#### 7. Notlar ve Geçmiş
- Lead'e eklenen notlar (varsa)
- Durum değişikliği geçmişi (opsiyonel — audit log'dan çekilebilir)

### Teknik Yaklaşım

**Yeni bir view oluştur:** `Views/Leads/Hub.cshtml`

**Mevcut Lead detay sayfasını koru** — Hub sayfası ayrı bir action olacak. Lead listesindeki "Detay" butonları artık Hub sayfasına yönlenecek.

**ViewModel:** Yeni bir `LeadHubViewModel` oluştur:

```csharp
public class LeadHubViewModel
{
    // Lead temel bilgileri
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Note { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusCode { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // Atama bilgisi
    public LeadAssignmentInfo? CurrentAssignment { get; set; }
    public List<LeadAssignmentInfo> AssignmentHistory { get; set; } = new();

    // Bağlı müşteri
    public LinkedAccountInfo? LinkedAccount { get; set; }

    // Aktivite geçmişi
    public List<LeadActivityInfo> Activities { get; set; } = new();

    // Satışlar
    public List<LeadSaleInfo> Sales { get; set; } = new();

    // Durum aksiyonları
    public List<string> AvailableActions { get; set; } = new();

    // Atama için personel listesi
    public List<Employee> AvailableEmployees { get; set; } = new();
}

public class LeadAssignmentInfo
{
    public string EmployeeName { get; set; } = string.Empty;
    public string AssignedByName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Note { get; set; }
    public bool IsActive { get; set; }
}

public class LinkedAccountInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
}

public class LeadActivityInfo
{
    public int Id { get; set; }
    public DateTime ActivityDate { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ContactStatus { get; set; } = string.Empty;
    public string OutcomeStatus { get; set; } = string.Empty;
    public string? Summary { get; set; }
}

public class LeadSaleInfo
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
}
```

**LeadService'e yeni metot:**

```csharp
LeadHubViewModel GetHubData(int leadId);
bool ChangeStatus(int leadId, string newStatusCode, string? note = null);
```

`GetHubData` metodu:
- Lead'i tüm navigation property'leriyle çek
- `LeadAssignment` tablosundan atama geçmişini çek
- Bağlı account varsa bilgilerini çek
- Bağlı account'un aktivitelerini çek (veya lead'den doğrudan oluşturulan aktiviteleri)
- Aktivitelere bağlı satışları çek
- Mevcut duruma göre `AvailableActions` listesini doldur

**LeadsController'a yeni action:**

```csharp
public IActionResult Hub(int id)
{
    var model = _leadService.GetHubData(id);
    if (model == null)
    {
        TempData["Warning"] = "Lead bulunamadı.";
        return RedirectToAction(nameof(Index));
    }
    BuildShell();
    return View(model);
}
```

### Adımlar
1. `LeadHubViewModel` ve alt sınıfları oluştur
2. `ILeadService`'e `GetHubData` ve `ChangeStatus` metot imzalarını ekle
3. `LeadService`'te implement et
4. `LeadsController`'a `Hub` action ekle
5. `Views/Leads/Hub.cshtml` oluştur — Bölüm 1-4 (Lead bilgi kartı, durum aksiyonları, atama, bağlı müşteri)
6. Lead listesindeki "Detay" butonlarını Hub'a yönlendir
7. Derle ve test et

---

## PARÇA B — Aktivite Timeline ve Satış Paneli

### Amaç
Hub sayfasına aktivite geçmişi ve satış panelini eklemek.

### Adımlar

1. `GetHubData` metodunda aktivite ve satış verilerini çek:

```csharp
// Lead'e bağlı aktiviteler
// Öncelik 1: Lead'den doğrudan oluşturulan aktiviteler (converted_activity_id)
// Öncelik 2: Lead'in bağlı olduğu account'un aktiviteleri
var activities = _db.Activities
    .Include(a => a.Employee)
    .Include(a => a.ContactStatusType)
    .Include(a => a.OutcomeStatusType)
    .Where(a => a.AccountId == lead.LinkedAccountId || a.Id == lead.ConvertedActivityId)
    .OrderByDescending(a => a.ActivityDate)
    .ToList();

// Satışlar — aktivitelere bağlı veya account üzerinden
var sales = _db.Sales
    .Include(s => s.Employee)
    .Include(s => s.InsuranceProductType)
    .Where(s => s.AccountId == lead.LinkedAccountId)
    .OrderByDescending(s => s.SaleDate)
    .ToList();
```

2. `Hub.cshtml`'e aktivite timeline bölümünü ekle:
   - Dikey timeline görünümü (tarih sırasına göre)
   - Her aktivite kartında: tarih, personel, temas durumu (renkli badge), sonuç, özet
   - Kart tıklanınca `/Activities/Details/{id}` sayfasına git
   - Aktivite yoksa boş durum mesajı

3. Satış panelini ekle:
   - Satış kartları — tarih, ürün tipi, tutar, personel
   - Kart tıklanınca `/Sales/Details/{id}` sayfasına git
   - Satış yoksa boş durum mesajı

4. Derle ve test et

---

## PARÇA C — Durum Geçişleri ve Quick Actions

### Amaç
Hub sayfasındaki aksiyon butonlarını çalışır hale getirmek. Lead'in durumunu pipeline'da ilerletmek.

### Adımlar

1. **LeadService.ChangeStatus** implementasyonu:

```csharp
public bool ChangeStatus(int leadId, string newStatusCode, string? note = null)
{
    var lead = _db.Leads.Find(leadId);
    if (lead == null) return false;

    var newStatus = _db.LeadStatusTypes.FirstOrDefault(s => s.Code == newStatusCode);
    if (newStatus == null) return false;

    // Geçiş kuralı kontrolü
    var validTransitions = GetValidTransitions(lead.LeadStatusType?.Code ?? "");
    if (!validTransitions.Contains(newStatusCode))
        return false;

    lead.LeadStatusTypeId = newStatus.Id;
    _db.SaveChanges();

    // Audit log
    _auditService?.LogAction("Lead", lead.Id.ToString(), "StatusChange",
        $"Status changed to {newStatusCode}" + (note != null ? $" - {note}" : ""));

    return true;
}

private List<string> GetValidTransitions(string currentStatus)
{
    return currentStatus switch
    {
        "NEW" => new() { "RESEARCHED", "DISQUALIFIED" },
        "RESEARCHED" => new() { "CONTACT_FOUND", "DISQUALIFIED" },
        "CONTACT_FOUND" => new() { "READY_FOR_ASSIGNMENT", "DISQUALIFIED" },
        "READY_FOR_ASSIGNMENT" => new() { "ASSIGNED", "DISQUALIFIED" },
        "ASSIGNED" => new() { "VISIT_SCHEDULED", "VISITED", "DISQUALIFIED" },
        "VISIT_SCHEDULED" => new() { "VISITED", "DISQUALIFIED" },
        "VISITED" => new() { "CONVERTED_TO_ACTIVITY", "DISQUALIFIED" },
        _ => new()
    };
}
```

2. **LeadsController'a durum geçiş action'ı:**

```csharp
[HttpPost]
public IActionResult ChangeStatus(int id, string newStatus, string? note)
{
    var result = _leadService.ChangeStatus(id, newStatus, note);
    if (!result)
        TempData["Warning"] = "Durum değişikliği yapılamadı.";
    else
        TempData["Success"] = "Lead durumu güncellendi.";

    return RedirectToAction(nameof(Hub), new { id });
}
```

3. **Atama modal/form:** "Personele Ata" butonu tıklandığında:
   - Küçük bir form açılır (modal veya inline)
   - Personel seçimi, öncelik, not alanları
   - Kaydet → `LeadService.Assign` çağrılır
   - Sayfa yenilenir, atama bilgisi panelde görünür

4. **Ziyaret Başlat:** Mevcut `StartVisit` mantığı korunur ama Hub sayfasından tetiklenir ve sonra Hub'a geri döner.

5. **Hub sayfasında aksiyon butonlarının görünürlüğü:**
   - `AvailableActions` listesine göre sadece geçerli aksiyonlar gösterilir
   - Her buton uygun renk kodunda (ilerleme = yeşil, iptal = kırmızı)
   - Tamamlanmış lead'de (CONVERTED_TO_ACTIVITY) sadece bilgi gösterilir, aksiyon yok

6. **Rol bazlı kısıtlama:**
   - Durum geçişleri: Admin, SalesManager, CallCenter (kendi oluşturduğu için)
   - Atama: Admin, SalesManager
   - Ziyaret Başlat: Admin, FieldSales (kendi atanmış lead'i)

7. Derle ve test et

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne ekle:

```
- Lead Detail Hub (360 derece sayfa) olusturuldu
- Hub sayfasinda lead bilgi karti, durum aksiyonlari, atama bilgisi, bagli musteri, aktivite timeline ve satis paneli yer aliyor
- Lead durum gecis mekanizmasi servis katmaninda uygulanidi (ChangeStatus + gecis kurallari)
- Hub sayfasinda rol bazli aksiyon gorunurlugu saglandi
- Lead listesinden Hub sayfasina yonlendirme yapildi
- Atama modali Hub sayfasindan calisir hale getirildi
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19.1 Uygulanan Mimari İyileştirmeler` bölümüne ekle:

```markdown
### Sprint 6 — Lead Detail Hub 360° (Tamamlandı)

- Lead Detail Hub sayfası oluşturuldu: Tek sayfada lead bilgileri, durum aksiyonları, atama bilgisi, bağlı müşteri, aktivite geçmişi ve satış paneli görüntülenmektedir.
- Lead durum geçiş mekanizması servis katmanında merkezi olarak yönetilmektedir (ChangeStatus metodu + geçiş kuralları matrisi).
- Hub sayfasında rol bazlı aksiyon görünürlüğü sağlanmıştır (atama sadece SalesManager, ziyaret sadece FieldSales vb.).
- Mevcut Lead listesindeki detay butonları Hub sayfasına yönlendirilmiştir.
```

---

## SPRINT 6 TAMAMLANMA KONTROL LİSTESİ

### Parça A
- [ ] LeadHubViewModel ve alt sınıfları oluşturulmuş
- [ ] LeadService.GetHubData metodu çalışıyor
- [ ] Hub.cshtml sayfası oluşturulmuş
- [ ] Lead bilgi kartı görünüyor
- [ ] Durum badge'i doğru renkte
- [ ] Atama bilgisi paneli görünüyor (atanmışsa)
- [ ] Bağlı müşteri paneli görünüyor (varsa)
- [ ] Lead listesinden Hub'a yönlendirme çalışıyor

### Parça B
- [ ] Aktivite timeline bölümü görünüyor
- [ ] Aktivite kartları tıklanınca detaya gidiyor
- [ ] Satış paneli görünüyor
- [ ] Satış kartları tıklanınca detaya gidiyor
- [ ] Aktivite/satış yoksa boş durum mesajı

### Parça C
- [ ] Durum geçiş butonları çalışıyor (NEW → RESEARCHED → CONTACT_FOUND → ...)
- [ ] Geçersiz geçiş engellenmiş
- [ ] Atama modali/formu çalışıyor
- [ ] Ziyaret Başlat Hub'dan çalışıyor
- [ ] İptal Et (DISQUALIFIED) butonu çalışıyor
- [ ] Rol bazlı buton görünürlüğü doğru
- [ ] Durum değişince Hub sayfası güncelleniyor
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
