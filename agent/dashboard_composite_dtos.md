# Dashboard Composite DTOs

## 1. Amac

Bu belge, dashboard endpoint'lerinin dondurecegi composite response DTO yapilarini netlestirir. Amaç; kart, grafik ve tablo verilerini tek endpoint altinda toplarken hem frontend'in rahat render edebilmesini hem de backend'in UI'ya asiri bagimli hale gelmemesini saglamaktir.

Bu belge:

- `dashboard_api_plan.md` ile uyumludur
- `endpoint_to_dto_map.md` icindeki dashboard response isimlerini detaylandirir
- `dashboard_response_examples.md` icin veri semasi referansi olur

## 2. Tasarim Ilkeleri

- Her dashboard endpoint'i kendi alanina ozel bir ust DTO donmelidir
- DTO'lar widget degil veri anlami etrafinda tasarlanmalidir
- Kart, grafik ve tablo alanlari mantiksal gruplar halinde tutulmalidir
- Ayni alt DTO, farkli dashboard endpoint'lerinde tekrar kullanilabilir
- Activity tarafinda `contactStatus` ve `outcomeStatus` daima ayri kirilimlar olarak ele alinmalidir

## 3. Ortak Alt DTO'lar

### 3.1 `DashboardSummaryCardDto`

Alanlar:

- `key`
- `title`
- `value`
- `unit`
- `format`
- `currencyCode`
- `displayHint`

### 3.2 `TrendPointDto` ailesi

Ortak mantik:

- `periodKey`
- `periodLabel`
- alan bazli metric degerleri

Turevler:

- `ActivityTrendPointDto`
- `SalesTrendPointDto`
- `FinancialTrendPointDto`
- `ExpenseTrendPointDto`

### 3.3 Breakdown DTO ailesi

Ortak mantik:

- `code`
- `label`
- `count` veya `amount`
- `ratio`

Turevler:

- `ContactStatusBreakdownDto`
- `OutcomeStatusBreakdownDto`
- `ProductBreakdownDto`
- `ExpenseTypeBreakdownDto`
- `LinkedUnlinkedSalesBreakdownDto`
- `ProductFinancialBreakdownDto`

### 3.4 Table row DTO ailesi

- `EmployeePerformanceRowDto`
- `EmployeeConversionRowDto`
- `RegionalConversionRowDto`

## 4. `DashboardSummaryResponseDto`

Amaç:

- Ust KPI kartlarini donmek

Yapi:

```json
{
  "cards": [
    {
      "key": "total_activity_count",
      "title": "Toplam Aktivite",
      "value": 286,
      "unit": "count",
      "format": "number",
      "displayHint": "primary"
    }
  ]
}
```

## 5. `ActivityDashboardResponseDto`

Amaç:

- Aktivite trendi, temas durumu dagilimi, sonuc durumu dagilimi ve bolgesel aktivite dagilimini donmek

Yapi:

```json
{
  "summary": {
    "totalActivityCount": 286,
    "contactedActivityCount": 191,
    "notContactedActivityCount": 95,
    "positiveOutcomeCount": 63,
    "negativeOutcomeCount": 38,
    "postponedOutcomeCount": 29,
    "saleClosedOutcomeCount": 16
  },
  "trend": [],
  "contactStatusBreakdown": [],
  "outcomeStatusBreakdown": [],
  "regionalBreakdown": []
}
```

Karar:

- `contactStatusBreakdown` ile `outcomeStatusBreakdown` asla tek listede birlestirilmemelidir

## 6. `SalesDashboardResponseDto`

Amaç:

- Satis sayisi, urun dagilimi ve aktivite baglantisi durumunu gostermek

Yapi:

```json
{
  "summary": {
    "totalSalesCount": 42,
    "linkedSalesCount": 35,
    "unlinkedSalesCount": 7
  },
  "trend": [],
  "productBreakdown": [],
  "linkedVsUnlinked": []
}
```

## 7. `FinancialDashboardResponseDto`

Amaç:

- Finansal uretim KPI'larini tek yapida toplamak

Yapi:

```json
{
  "summary": {
    "totalBesCollectionAmount": 418750.0,
    "totalBesApeAmount": 126300.0,
    "totalBesLumpSumAmount": 82000.0,
    "totalLifePremiumAmount": 96500.0,
    "totalHealthProductionAmount": 54750.0,
    "totalMonthlyPaymentAmount": 28940.0
  },
  "trend": [],
  "productFinancialBreakdown": []
}
```

## 8. `ExpenseDashboardResponseDto`

Amaç:

- Masraf toplami, trendi ve masraf tipi dagilimini gostermek

Yapi:

```json
{
  "summary": {
    "totalExpenseAmount": 58240.5,
    "expensePerEmployeeAmount": 8320.07,
    "expensePerSaleAmount": 1386.68
  },
  "trend": [],
  "expenseTypeBreakdown": []
}
```

## 9. `ConversionDashboardResponseDto`

Amaç:

- Donusum oranlarini toplam, personel ve bolge seviyesinde gostermek

Yapi:

```json
{
  "summary": {
    "activityToSaleConversionRate": 0.147,
    "contactToSaleConversionRate": 0.22,
    "expenseToSalesRatio": 1386.68,
    "expenseToCollectionRatio": 0.139
  },
  "employeeConversionRows": [],
  "regionalConversionRows": []
}
```

## 10. `PerformanceDashboardResponseDto`

Amaç:

- Tek skor yerine yorumlanabilir cok kolonlu performans tablosu sunmak

Yapi:

```json
{
  "summary": {
    "employeeCount": 7,
    "totalActivityCount": 286,
    "totalSalesCount": 42
  },
  "rows": [],
  "totals": {
    "totalActivityCount": 286,
    "totalSalesCount": 42,
    "totalExpenseAmount": 58240.5,
    "totalBesCollectionAmount": 418750.0
  }
}
```

## 11. `ExecutiveOverviewDto`

Amaç:

- Yonetici ana ekrani icin temel ozet verileri tek cagrida donmek

Yapi:

```json
{
  "summaryCards": [],
  "activityTrend": [],
  "salesTrend": [],
  "financialTrend": [],
  "expenseTrend": [],
  "productBreakdown": [],
  "performanceTable": []
}
```

Karar:

- Bu endpoint drill-down yerine ana ekran hizli yukleme endpoint'i gibi dusunulmelidir
- Cok detayli dagilimlar kendi endpoint'lerinde kalmalidir

## 12. `DashboardReferenceFiltersDto`

Amaç:

- Dashboard filtre panelinin ilk yuklemede ihtiyac duydugu toplu lookup verisini donmek

Yapi:

```json
{
  "employees": [],
  "productTypes": [],
  "expenseTypes": [],
  "activityContactStatuses": [],
  "activityOutcomeStatuses": [],
  "regions": [],
  "cities": []
}
```

## 13. Meta Ve Warning Kurallari

Tum composite DTO'lar standart API zarfi icinde donmelidir:

```json
{
  "success": true,
  "data": {},
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "activity_date",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

Uyari ornekleri:

- `UNLINKED_SALES_PRESENT`
- `INCOMPLETE_FINANCIAL_FIELDS`
- `PARTIAL_REGION_DATA`

## 14. MVP Kararlari

- MVP'de widget konfigürasyon DTO'su donulmeyecek
- MVP'de teklesik personel skoru olmayacak
- MVP'de activity kirilimlari iki ayri eksende donecek:
  - `contactStatusBreakdown`
  - `outcomeStatusBreakdown`
- MVP'de dashboard response'lari okunabilir ve tahmin edilebilir sabit semalar kullanacak

## 15. Sonuc

Dashboard composite DTO stratejisi, frontend'e yeterince acik ama backend'i gorsel sunuma kilitlemeyecek kadar sade tutulmalidir. Bu projede en kritik karar, activity tarafinda temas durumu ile sonuc durumunun ayri veri yapilari olarak korunmasidir.
