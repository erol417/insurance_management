# Sprint 5 — Dashboard ve KPI Güçlendirme

## Genel Talimat

Bu sprint, mevcut dashboard ekranlarını `kpi_dictionary.md`'deki MVP KPI listesiyle hizalar. Amaç: yöneticinin anlamlı, filtrelenebilir ve doğru KPI'lar görmesi. Mevcut dashboard altyapısı Sprint 3-4'te kısmen güçlendirilmişti — bu sprint onu tamamlar.

**Proje hâlâ MVC + Razor yapısında.** API endpoint'leri ve DTO'lar bu sprint'in konusu değil. DashboardService metotları ve Razor view'ları güncellenerek dashboard zenginleştirilecek.

**Önemli:** `kpi_dictionary.md` 40+ sayfalık detaylı bir KPI sözlüğü. Bu sprint sadece **MVP KPI'larını** kapsıyor. Future phase KPI'ları (bölge bazlı dönüşüm, kişi başı kârlılık, trend endeksi vb.) bu sprint'in dışında.

**Sprint bu 3 parçada uygulanacak — her parça ayrı bir "devam et" komutuyla tetiklenecek. Agent token limitine takılmasın diye küçük parçalar halinde ilerleyecek.**

---

## PARÇA A — Özet Kartlar ve Ortak Filtreleme

### Amaç
Dashboard ana ekranında yöneticinin ilk bakışta göreceği özet kartları KPI sözlüğüyle hizalamak ve tüm kartlara tarih filtresi uygulamak.

### MVP Özet Kartları (kpi_dictionary.md Bölüm 4.6'dan)

Dashboard'un üst kısmında şu kartlar olmalı:

1. **Toplam Aktivite** — `COUNT(activities.id)` — soft delete hariç, "Planlandı" statüsündekiler hariç
2. **Görüşülen Aktivite** — `COUNT(activities.id WHERE contact_status = 'CONTACTED')`
3. **Toplam Satış Adedi** — `SUM(sales.sale_count)` veya `COUNT(sales.id)`
4. **Toplam BES Tahsilatı** — `SUM(sales.collection_amount WHERE product_type = 'BES')`
5. **Aktiviteden Satışa Dönüşüm Oranı** — `COUNT(DISTINCT sales.activity_id WHERE activity_id IS NOT NULL) / COUNT(activities.id)` — yüzde olarak göster
6. **Toplam Masraf** — `SUM(expenses.amount)`

### Ortak Tarih Filtresi

Tüm dashboard ekranlarına ortak bir tarih filtresi ekle:

- Varsayılan: Son 30 gün
- Seçenekler: Bu hafta, Bu ay, Son 30 gün, Son 90 gün, Bu yıl, Özel tarih aralığı
- Filtre değiştiğinde tüm kartlar, grafikler ve tablolar güncellenmeli
- Server-side çalışmalı — form submit veya query string ile

### DashboardService Güncelleme

`DashboardService`'e tarih filtresi parametreleri ekle:

```csharp
public DashboardSummary GetSummary(DateTime startDate, DateTime endDate, int? employeeId = null)
{
    var activities = _db.Activities
        .Where(a => a.ActivityDate >= startDate && a.ActivityDate <= endDate);
    
    var sales = _db.Sales
        .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);
    
    var expenses = _db.Expenses
        .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);

    // Rol bazlı filtreleme (Sprint 4'te yapıldı) korunmalı
    // employeeId varsa ek filtre uygula

    // Planlanmış aktiviteleri hariç tut
    var realActivities = activities
        .Where(a => a.ContactStatusType.Code != "PLANNED");

    return new DashboardSummary
    {
        TotalActivities = realActivities.Count(),
        ContactedActivities = realActivities
            .Count(a => a.ContactStatusType.Code == "CONTACTED"),
        TotalSalesCount = sales.Sum(s => s.SaleCount ?? 1),
        TotalBesCollection = sales
            .Where(s => s.InsuranceProductType.Code == "BES")
            .Sum(s => s.CollectionAmount ?? 0),
        ConversionRate = realActivities.Count() > 0
            ? (decimal)sales.Count(s => s.ActivityId != null) / realActivities.Count()
            : 0,
        TotalExpenses = expenses.Sum(e => e.Amount)
    };
}
```

### View Güncelleme

Dashboard ana view'ında:
- Üst kısımda tarih filtre formu
- Altında 6 özet kart — her biri metrik adı, değeri ve varsa önceki dönemle karşılaştırma
- Dönüşüm oranı yüzde olarak gösterilmeli (örn. "%14.7")
- BES tahsilatı para formatında (örn. "₺418.750")

### Adımlar
1. `DashboardService`'e tarih parametreli `GetSummary` metodu ekle veya mevcut metodu güncelle
2. `DashboardController`'da tarih parametrelerini al ve servise ilet
3. Dashboard view'ında tarih filtre formunu ekle
4. 6 özet kartı yukarıdaki KPI tanımlarına göre güncelle
5. Derle ve test et — farklı tarih aralıkları seçince kartlar değişiyor mu?

---

## PARÇA B — Trend Grafikleri ve Dağılım Tabloları

### Amaç
Özet kartların altına trend grafikleri ve dağılım tabloları eklemek.

### MVP Grafikleri

1. **Aktivite Trendi** — Günlük veya haftalık aktivite sayısı çizgi grafiği
   - X ekseni: tarih
   - Y ekseni: aktivite sayısı
   - Veri: `COUNT(activities.id) GROUP BY activity_date` (günlük) veya haftalık gruplama
   - Planlanmış aktiviteler hariç

2. **Satış Trendi** — Günlük veya haftalık satış adedi çizgi grafiği
   - `COUNT(sales.id) GROUP BY sale_date`

3. **Ürün Bazlı Satış Dağılımı** — Donut veya bar chart
   - `SUM(sales.sale_count) GROUP BY product_type`
   - Her ürün tipi renk kodlu

4. **Masraf Türü Dağılımı** — Donut veya bar chart
   - `SUM(expenses.amount) GROUP BY expense_type`

### MVP Tabloları

5. **Personel Performans Tablosu** — Çok kolonlu karşılaştırma tablosu
   - Sütunlar: Personel Adı | Aktivite | Görüşülen | Satış Adedi | BES Tahsilatı | Masraf | Dönüşüm Oranı
   - `GROUP BY employee_id`
   - Sıralama: satış adedine göre azalan
   - KPI sözlüğü notu: "Tek skor yerine çoklu KPI seti birlikte yorumlanır"

6. **Aktiviteye Bağlı / Bağımsız Satış** — İki sayılı kart veya küçük tablo
   - Bağlı: `COUNT(sales.id WHERE activity_id IS NOT NULL)`
   - Bağımsız: `COUNT(sales.id WHERE activity_id IS NULL)`
   - Veri kalitesi göstergesi olarak kullanılır

### Grafik Kütüphanesi

Mevcut projede grafik kütüphanesi var mı kontrol et. Yoksa şu seçeneklerden birini kullan:
- **Chart.js** (CDN ile — `<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>`)
- Razor view'da `<canvas>` elementi, JavaScript ile grafik çizimi

Alternatif: Eğer grafik kütüphanesi eklemek karmaşıksa, grafikler yerine daha detaylı tablolar kullanılabilir. Yönetici için önemli olan doğru rakamlar, güzel grafikler değil.

### DashboardService Güncelleme

Yeni metotlar ekle:

```csharp
public List<TrendPoint> GetActivityTrend(DateTime startDate, DateTime endDate, int? employeeId = null);
public List<TrendPoint> GetSalesTrend(DateTime startDate, DateTime endDate, int? employeeId = null);
public List<ProductBreakdown> GetProductBreakdown(DateTime startDate, DateTime endDate, int? employeeId = null);
public List<ExpenseBreakdown> GetExpenseTypeBreakdown(DateTime startDate, DateTime endDate, int? employeeId = null);
public List<EmployeePerformanceRow> GetPerformanceTable(DateTime startDate, DateTime endDate);
public SalesLinkageSummary GetSalesLinkage(DateTime startDate, DateTime endDate, int? employeeId = null);
```

ViewModel sınıfları (DashboardViewModels.cs'e ekle):

```csharp
public class TrendPoint
{
    public string Label { get; set; } = string.Empty; // "2026-04-01" veya "Hafta 14"
    public int Count { get; set; }
    public decimal? Amount { get; set; }
}

public class ProductBreakdown
{
    public string ProductName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ExpenseBreakdown
{
    public string ExpenseTypeName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class EmployeePerformanceRow
{
    public string EmployeeName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int ContactedCount { get; set; }
    public int SalesCount { get; set; }
    public decimal BesCollection { get; set; }
    public decimal ExpenseAmount { get; set; }
    public decimal ConversionRate { get; set; }
}

public class SalesLinkageSummary
{
    public int LinkedCount { get; set; }
    public int UnlinkedCount { get; set; }
}
```

### Adımlar
1. ViewModel sınıflarını ekle
2. DashboardService'e yeni metotları implement et
3. DashboardController'da bu metotları çağır ve ViewBag/ViewModel ile view'a ilet
4. Dashboard view'ında grafik ve tabloları render et
5. Tüm grafik ve tablolar tarih filtresine tepki vermeli
6. Derle ve test et

---

## PARÇA C — Ek Filtreler ve Finansal Detay

### Amaç
Tarih filtresine ek olarak personel ve ürün filtresi eklemek. Finansal KPI'ları ürün bazlı detaylandırmak.

### Ek Filtreler

Dashboard filtre paneline ekle:
- **Personel filtresi** — dropdown, tüm aktif personeller (Admin/Manager tüm listeyi görür, FieldSales sadece kendini)
- **Ürün tipi filtresi** — dropdown, tüm ürün tipleri

Bu filtreler tarih filtresiyle birlikte çalışmalı. Seçildiğinde tüm kartlar, grafikler ve tablolar güncellenmeli.

### Finansal Detay Tablosu

Ürün bazlı finansal özet tablosu:

| Ürün | Satış Adedi | Tahsilat | APE | Prim | Üretim | Aylık Ödeme |
|------|------------|---------|-----|------|--------|-------------|
| BES | 15 | ₺418.750 | ₺126.300 | - | - | ₺28.940 |
| Hayat | 8 | ₺96.500 | - | ₺96.500 | - | - |
| Sağlık | 6 | ₺54.750 | - | - | ₺54.750 | - |
| Seyahat | 10 | ₺12.000 | - | - | - | - |
| **Toplam** | **39** | **₺582.000** | **₺126.300** | **₺96.500** | **₺54.750** | **₺28.940** |

Her ürün tipi için yalnızca ilgili finansal alanlar dolu — diğerleri "-" gösterilir (kpi_dictionary.md ve financial_validation_matrix.md'ye uygun).

### DashboardService Güncelleme

```csharp
public List<ProductFinancialRow> GetProductFinancialBreakdown(DateTime startDate, DateTime endDate, int? employeeId = null);
```

```csharp
public class ProductFinancialRow
{
    public string ProductName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal CollectionAmount { get; set; }
    public decimal ApeAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal ProductionAmount { get; set; }
    public decimal MonthlyPaymentAmount { get; set; }
}
```

### Adımlar
1. Controller'da personel ve ürün filtre parametrelerini al
2. DashboardService metotlarına bu parametreleri ilet
3. View'da dropdown filtreleri ekle
4. Finansal detay tablosunu oluştur
5. Tüm bileşenler filtrelere tepki veriyor mu test et
6. Derle ve test et

---

## DOKÜMAN GÜNCELLEME GÖREVLERİ

### DOC-1: `agent/active_tasks.md` Güncelle

`## Completed` bölümüne ekle:

```
- Dashboard ozet kartlari KPI sozlugu ile hizalandi (6 MVP karti)
- Dashboard'a tarih filtresi eklendi (bu hafta, bu ay, son 30 gun, son 90 gun, bu yil, ozel aralik)
- Aktivite ve satis trend grafikleri eklendi
- Urun bazli satis dagilimi grafigi eklendi
- Masraf turu dagilimi grafigi eklendi
- Personel performans karsilastirma tablosu eklendi (cok kolonlu KPI seti)
- Aktiviteye bagli/bagimsiz satis analizi eklendi
- Personel ve urun tipi filtreleri dashboard'a eklendi
- Urun bazli finansal detay tablosu eklendi
- Planlanan aktiviteler dashboard KPI'larindan haric tutuldu
```

### DOC-2: `agent/project_design.md` Güncelle

`## 19.1 Uygulanan Mimari İyileştirmeler` bölümüne ekle:

```markdown
### Sprint 5 — Dashboard ve KPI Güçlendirme (Tamamlandı)

- Dashboard özet kartları `kpi_dictionary.md`'deki MVP KPI tanımlarıyla hizalandı: Toplam Aktivite, Görüşülen Aktivite, Toplam Satış, BES Tahsilatı, Dönüşüm Oranı, Toplam Masraf.
- Tüm dashboard bileşenlerine tarih filtresi eklendi. Personel ve ürün tipi ek filtreleri eklendi.
- Aktivite ve satış trend grafikleri, ürün bazlı dağılım ve masraf türü dağılım grafikleri eklendi.
- Personel performans tablosu çok kolonlu KPI setiyle oluşturuldu (aktivite, görüşülen, satış, BES tahsilatı, masraf, dönüşüm oranı).
- Ürün bazlı finansal detay tablosu oluşturuldu (her ürün için ilgili finansal alanlar).
- Planlanan (PLANNED) aktiviteler KPI hesaplamalarından hariç tutuldu.
- Aktiviteye bağlı/bağımsız satış analizi eklendi (veri kalitesi göstergesi).
```

---

## SPRINT 5 TAMAMLANMA KONTROL LİSTESİ

### Parça A
- [ ] 6 özet kart KPI sözlüğüyle hizalı
- [ ] Tarih filtresi çalışıyor
- [ ] Filtre değişince kartlar güncelleniyor
- [ ] Planlanmış aktiviteler hariç tutuluyor
- [ ] Dönüşüm oranı yüzde olarak gösteriliyor
- [ ] BES tahsilatı para formatında

### Parça B
- [ ] Aktivite trend grafiği/tablosu var
- [ ] Satış trend grafiği/tablosu var
- [ ] Ürün bazlı satış dağılımı var
- [ ] Masraf türü dağılımı var
- [ ] Personel performans tablosu çok kolonlu
- [ ] Aktiviteye bağlı/bağımsız satış sayıları gösteriliyor

### Parça C
- [ ] Personel filtresi çalışıyor
- [ ] Ürün tipi filtresi çalışıyor
- [ ] Ürün bazlı finansal detay tablosu doğru
- [ ] Tüm bileşenler tüm filtrelere tepki veriyor

### Doküman
- [ ] `active_tasks.md` güncellenmiş
- [ ] `project_design.md` güncellenmiş
