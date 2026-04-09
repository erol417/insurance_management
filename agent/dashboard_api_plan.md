# Dashboard API Plan

## 1. Amac

Bu belge, dashboard ve raporlama tarafinda kullanilacak API sozlesmesini urun seviyesinde tanimlar. Amac, kod yazmadan once hangi endpoint gruplarinin olacagini, hangi filtre modelinin kullanilacagini, hangi KPI setlerinin hangi response yapisiyla donecegini ve MVP kapsaminda hangi sinirlarin korunacagini netlestirmektir.

Bu dokuman:

- `kpi_dictionary.md` ile uyumlu calisir
- `dashboard_composite_dtos.md` ile birlikte dashboard response semasini tamamlar
- Backend dashboard modulu icin sorgu ve response omurgasi olusturur
- Frontend dashboard ekranlarinin veri beklentisini standardize eder

## 2. Temel Yaklasim

Dashboard API tasariminda temel ilke, transaction CRUD endpoint'lerinden ayri, raporlama odakli bir sorgu katmani olusturmaktir.

Urun karari:

- Dashboard endpoint'leri `/api/dashboard/*` altinda toplanacaktir
- Tum dashboard endpoint'leri ortak filtre modeli kullanacaktir
- KPI hesaplari ham islem verisinden uretilecektir
- Ayni KPI farkli endpoint'lerde farkli formul ile donmeyecektir
- Dashboard response'lari frontend'in kolay render edebilecegi ama UI'ya asiri bagimli olmayan yapida olacaktir

## 3. Dashboard API Tasarim Ilkeleri

- Tek endpoint icinde gereksiz asiri veri tasinmamali
- Kart, grafik ve tablo verileri mantiksal olarak ayrilmalidir
- Filtre modeli ortak ve tekrar kullanilabilir olmalidir
- Tarih alani yorumu endpoint bazinda acik olmalidir
- Rol bazli veri gorunurlugu API seviyesinde uygulanmalidir
- Activity tarafinda temas durumu ve sonuc durumu daima ayri ele alinmalidir

## 4. Route Stratejisi

Onerilen temel route gruplari:

- `/api/dashboard/summary`
- `/api/dashboard/activities`
- `/api/dashboard/sales`
- `/api/dashboard/financials`
- `/api/dashboard/expenses`
- `/api/dashboard/conversions`
- `/api/dashboard/performance`
- `/api/dashboard/reference-filters`

MVP icin ayrica tek ekrandan dashboard yuklemeyi kolaylastiran birlesik endpoint onerilir:

- `/api/dashboard/executive-overview`

Bu endpoint, yonetici ana ekrani icin gerekli temel kart, grafik ve tablo verilerini tek response icinde donebilir.

## 5. Ortak Filtre Modeli

Tum dashboard endpoint'lerinde ortak bir filtre contract kullanilmalidir.

Onerilen mantiksal filtre alanlari:

```json
{
  "dateRange": {
    "startDate": "2026-01-01",
    "endDate": "2026-01-31"
  },
  "timeGrain": "month",
  "employeeIds": [],
  "productTypeIds": [],
  "accountIds": [],
  "activityContactStatusIds": [],
  "activityOutcomeStatusIds": [],
  "expenseTypeIds": [],
  "regions": [],
  "cities": [],
  "districts": [],
  "includeOnlyLinkedSales": false,
  "includeOnlyUnlinkedSales": false
}
```

### Filtre alani aciklamalari

- `dateRange`: zorunlu tarih araligi
- `timeGrain`: `day`, `week`, `month`, `year`
- `employeeIds`: secili personel
- `productTypeIds`: urun tipi filtreleri
- `accountIds`: musteri/firma filtreleri
- `activityContactStatusIds`: temas durumu filtreleri
- `activityOutcomeStatusIds`: sonuc durumu filtreleri
- `expenseTypeIds`: masraf turu filtreleri
- `regions`, `cities`, `districts`: cografi filtreler
- `includeOnlyLinkedSales`: yalnizca aktiviteye bagli satislar
- `includeOnlyUnlinkedSales`: yalnizca aktiviteye baglanmamis satislar

### Filtre davranis kurallari

- `includeOnlyLinkedSales` ve `includeOnlyUnlinkedSales` ayni anda `true` olamaz
- Bos liste, o filtreyi uygulama anlamina gelir
- Ayni endpoint tum filtre alanlarini kullanmak zorunda degildir
- Activity status filtreleri yalnizca ilgili dashboard endpoint'lerinde dikkate alinabilir
- Kullanilmayan filtreler response metadata icinde belirtilebilir

## 6. Tarih Yorumlama Kurali

Urun karari olarak her endpoint kendi ana tarih alanina bagli calisacaktir:

- Aktivite endpoint'leri: `activity_date`
- Satis endpoint'leri: `sale_date`
- Finansal endpoint'ler: `sale_date`
- Masraf endpoint'leri: `expense_date`
- Donusum endpoint'leri: varsayilan olarak `activity_date`
- Executive overview ve summary endpoint'leri: `mixed_by_section` veya `mixed_by_widget`

Onemli not:

- Frontend ayni ekranda birden fazla tarih mantigi kullanan veri gosteriyorsa bunu kullaniciya acikca belirtmelidir
- API response icinde `dateInterpretation` alani donulmelidir

## 7. Ortak Response Zarfi

Tum dashboard endpoint'leri ortak ust response mantigina sahip olmalidir.

Onerilen yapi:

```json
{
  "success": true,
  "data": {},
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "sale_date",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

### Meta alanlari

- `generatedAt`: verinin uretildigi zaman
- `dateInterpretation`: response'un hangi tarih mantigiyla uretildigi
- `appliedFilters`: gercekten uygulanan filtre ozeti
- `warnings`: veri kalitesi veya yorum uyari listesi

## 8. Veri Odakli Response Yaklasimi

Frontend tarafinda esnek ekran kurulumu onemlidir; ancak backend response'lari widget konfigürasyonu degil, veri anlami etrafinda tasarlanmalidir.

Bu nedenle:

- Backend, kart/grafik/tabloya uygun veri yapisini doner
- Frontend, son gorsel secimi kendisi verir
- Composite DTO'lar `dashboard_composite_dtos.md` icindeki sabit semalara gore kurulur

## 9. MVP Endpoint Seti

## 9.1 `POST /api/dashboard/executive-overview`

### Amac

Yonetici ana dashboard ekraninda gorulecek temel kart, grafik ve tabloları tek cagrida donmek.

### Icerik

- Summary cards
- Activity trend
- Sales trend
- Financial production trend
- Expense trend
- Product breakdown
- Employee performance table

### Onerilen response yaklasimi

- `ExecutiveOverviewDto`

### Uyari

- Bu endpoint cok fazla veri tasimamalidir
- Drill-down detaylari alan bazli endpoint'lere birakilmalidir

## 9.2 `POST /api/dashboard/summary`

### Amac

Ust KPI kartlarini donmek.

### MVP kartlari

- Toplam aktivite
- Toplam satis adedi
- Toplam masraf
- Toplam BES tahsilati
- Aktiviteden satisa donusum orani
- Aktiviteye baglanamayan satis sayisi

### Onerilen response

- `DashboardSummaryResponseDto`

## 9.3 `POST /api/dashboard/activities`

### Amac

Aktivite KPI'larini, trendleri ve status dagilimlarini donmek.

### MVP kapsami

- Toplam aktivite
- Gorusulen aktivite
- Gorusulemeyen aktivite
- Olumlu sonuc sayisi
- Olumsuz sonuc sayisi
- Ertelenen sonuc sayisi
- Satis oldu sonuc sayisi
- Gunluk/haftalik/aylik aktivite trendi
- Bolge bazli aktivite

### Onerilen response bolumleri

- `summary`
- `trend`
- `contactStatusBreakdown`
- `outcomeStatusBreakdown`
- `regionalBreakdown`

### Onerilen response tipi

- `ActivityDashboardResponseDto`

### Kritik karar

- `contactStatusBreakdown` ile `outcomeStatusBreakdown` tek listede birlestirilmeyecektir
- `SALE_CLOSED` sonucu, gercek satis kaydi yerine gecmeyecektir

## 9.4 `POST /api/dashboard/sales`

### Amac

Satis hacmi ve satis dagilimi verilerini donmek.

### MVP kapsami

- Toplam satis adedi
- Urun bazli satis adedi
- Personel bazli satis adedi
- Tarih bazli satis trendi
- Aktiviteden dogan satis sayisi
- Aktiviteye baglanamayan satis sayisi

### Response bolumleri

- `summary`
- `trend`
- `productBreakdown`
- `linkedVsUnlinked`

### Onerilen response tipi

- `SalesDashboardResponseDto`

## 9.5 `POST /api/dashboard/financials`

### Amac

Finansal uretim KPI'larini donmek.

### MVP kapsami

- Toplam BES tahsilati
- Toplam BES APE
- Toplam BES toplu para
- Toplam hayat primi
- Toplam saglik uretimi
- Urun bazli toplam finansal uretim
- Aylik odeme toplami
- Finansal uretim trendi

### Response bolumleri

- `summary`
- `trend`
- `productFinancialBreakdown`

### Onerilen response tipi

- `FinancialDashboardResponseDto`

### Onemli karar

- Response icinde kullanilan finansal alan yorumlari acik donmelidir
- Gerekirse `metricDefinitionNote` benzeri ek alanlar future phase'de dusunulebilir

## 9.6 `POST /api/dashboard/expenses`

### Amac

Masraf ozetlerini, tur dagilimini ve trend verilerini donmek.

### MVP kapsami

- Toplam masraf
- Personel bazli masraf
- Masraf turune gore toplam
- Gunluk/haftalik/aylik masraf
- Satis basina masraf

### Response bolumleri

- `summary`
- `trend`
- `expenseTypeBreakdown`

### Onerilen response tipi

- `ExpenseDashboardResponseDto`

## 9.7 `POST /api/dashboard/conversions`

### Amac

Donusum KPI'larini donmek.

### MVP kapsami

- Aktiviteden satisa donusum orani
- Gorusmeden satisa donusum orani
- Personel bazli donusum orani

### Future phase genislemeleri

- Bolge bazli donusum orani
- Masraf/tahsilat orani
- Gelismis funnel analizleri

### Response bolumleri

- `summary`
- `employeeConversionRows`
- `regionalConversionRows`

### Onerilen response tipi

- `ConversionDashboardResponseDto`

## 9.8 `POST /api/dashboard/performance`

### Amac

Personel performans tablosu uretmek.

### MVP yaklasimi

Tek bir birlesik skor donulmeyecektir. Bunun yerine cok kolonlu performans tablosu donulecektir.

### Onerilen sutunlar

- `employeeId`
- `employeeName`
- `activityCount`
- `contactedActivityCount`
- `salesCount`
- `besCollectionAmount`
- `expenseAmount`
- `activityToSaleConversionRate`
- `contactToSaleConversionRate`

### Onerilen response tipi

- `PerformanceDashboardResponseDto`

## 9.9 `GET /api/dashboard/reference-filters`

### Amac

Dashboard filtrelerinde kullanilacak lookup verilerini donmek.

### Kapsam

- employees
- productTypes
- expenseTypes
- activityContactStatuses
- activityOutcomeStatuses
- regions
- cities
- districts

### Gerekce

- Frontend farkli kaynaklardan filtre verisi toplamaya calismamalidir
- Dashboard filtre deneyimi tek endpoint ile sadelesir

### Onerilen response tipi

- `DashboardReferenceFiltersDto`

## 10. Response Veri Tipi Standartlari

### Sayisal tipler

- `count` alanlari integer doner
- Parasal alanlar decimal doner
- Oran alanlari `0.00 - 1.00` araliginda raw numeric doner
- Frontend yuzde gosterimini kendisi yapar

### Null davranisi

- Hesaplanamayan oranlar `null` doner
- Bos sonuc listeleri `[]` doner
- Bos ozet alanlari tercihen `0` doner; ancak anlamsal olarak tanimsiz ise `null` kullanilabilir

### Format bilgisi

Kart veya tablo hucresi seviyesinde su alanlar bulunabilir:

- `unit`
- `format`
- `currencyCode`

## 11. Uyari ve Veri Kalitesi Mekanizmasi

Dashboard API yalnizca sayi donmemeli, veri kalitesi uyarilarini da iletebilmelidir.

Onerilen uyari ornekleri:

- Aktiviteye baglanamayan satis sayisi yuksek
- Secili donemde bazi BES satislarinda APE eksik
- Bolge verisi eksik oldugu icin bolgesel dagilim kismi uretildi
- Onceki donem verisi olmadigi icin trend orani hesaplanamadi

Onerilen yapi:

```json
{
  "code": "UNLINKED_SALES_HIGH",
  "message": "Secili donemde aktiviteye baglanamayan satis orani yuksektir.",
  "severity": "warning"
}
```

## 12. Rol Bazli Gorunurluk

### Manager

- Tum dashboard endpoint'lerine tam erisim

### Admin

- Tum dashboard endpoint'lerine tam erisim

### Operations

- Ozet kartlar
- Aktivite, satis, masraf ve temel finansal ekranlar
- Personel performansina sinirli erisim urun kararina gore acilabilir

### FieldSales

- Yalnizca kendi verisi ile sinirli dashboard erisimi
- Yonetici toplu performans tablolarina erisim olmamalidir

## 13. MVP Icin Onerilen Dashboard Ekranlari ve Endpoint Eslesmesi

### Executive Dashboard

- `/api/dashboard/executive-overview`
- `/api/dashboard/summary`

### Activity Analysis

- `/api/dashboard/activities`

### Sales Analysis

- `/api/dashboard/sales`

### Financial Production

- `/api/dashboard/financials`

### Expense Analysis

- `/api/dashboard/expenses`

### Team Performance

- `/api/dashboard/performance`
- `/api/dashboard/conversions`

## 14. Future Phase Endpoint Genislemeleri

Ileri fazda eklenebilecek endpoint'ler:

- `/api/dashboard/regions`
- `/api/dashboard/trends`
- `/api/dashboard/funnel`
- `/api/dashboard/profitability`
- `/api/dashboard/export`

### Future phase kullanim ornekleri

- Harita bazli bolge ekrani
- Hedef ve kota karsilastirmalari
- Funnel analizi
- Gelismis profitability metrikleri

## 15. Acik Urun Sorulari

- Donusum orani ekraninda varsayilan tarih mantigi kesin olarak `activity_date` mi olacak?
- Bolge analizinde esas kaynak calisan bolgesi mi, ziyaret bolgesi mi olacak?
- Financial production ekraninda urun bazli farkli finansal alanlar tek grafikte mi, ayri widget'larda mi gosterilecek?
- Operations rolu personel karsilastirma tablosunun tamamini gorebilecek mi?

## 16. Sonuc

Bu plan ile dashboard tarafi icin:

- ortak filtre contract'i
- ortak response zarfi
- MVP endpoint seti
- endpoint bazli response yapilari
- rol bazli gorunurluk
- veri kalitesi uyari yapisi

netlestirilmis oldu.
