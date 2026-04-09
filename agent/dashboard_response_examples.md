# Dashboard Response Examples

## 1. Amac

Bu belge, `dashboard_api_plan.md` ve `dashboard_composite_dtos.md` icinde tanimlanan dashboard endpoint'leri icin ornek request ve response payload'larini sunar. Amaç; backend ve frontend tarafinin ayni veri semasi uzerinde calismasini saglamak ve ozellikle yeni `contactStatus` ve `outcomeStatus` modelini response seviyesinde netlestirmektir.

## 2. Genel Notlar

- Tum ornekler MVP kapsamindadir
- Tarihler ornek olarak `2026` uzerinden verilmistir
- Oranlar ham numeric deger olarak doner
- Para ve yuzde formatlamasi frontend tarafinda yapilir
- Aktivite sonuc modeli iki ayridir:
  - `activityContactStatusIds`
  - `activityOutcomeStatusIds`

## 3. Ortak Dashboard Request Ornegi

```json
{
  "dateRange": {
    "startDate": "2026-01-01",
    "endDate": "2026-01-31"
  },
  "timeGrain": "week",
  "employeeIds": [
    "emp-001",
    "emp-004"
  ],
  "productTypeIds": [
    "prod-bes",
    "prod-life"
  ],
  "accountIds": [],
  "activityContactStatusIds": [],
  "activityOutcomeStatusIds": [],
  "expenseTypeIds": [],
  "regions": [
    "Istanbul-Europe"
  ],
  "cities": [
    "Istanbul"
  ],
  "districts": [],
  "includeOnlyLinkedSales": false,
  "includeOnlyUnlinkedSales": false
}
```

## 4. Ortak Meta Ornegi

```json
{
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "activity_date",
    "appliedFilters": {
      "startDate": "2026-01-01",
      "endDate": "2026-01-31",
      "timeGrain": "week",
      "employeeIds": [
        "emp-001",
        "emp-004"
      ],
      "regions": [
        "Istanbul-Europe"
      ]
    },
    "warnings": [
      {
        "code": "UNLINKED_SALES_PRESENT",
        "message": "Secili donemde aktiviteye baglanmamis satis kayitlari bulunmaktadir.",
        "severity": "warning"
      }
    ]
  }
}
```

## 5. `POST /api/dashboard/summary`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "cards": [
      {
        "key": "total_activity_count",
        "title": "Toplam Aktivite",
        "value": 286,
        "unit": "count",
        "format": "number",
        "displayHint": "primary"
      },
      {
        "key": "total_sales_count",
        "title": "Toplam Satis",
        "value": 42,
        "unit": "count",
        "format": "number",
        "displayHint": "primary"
      },
      {
        "key": "total_expense_amount",
        "title": "Toplam Masraf",
        "value": 58240.5,
        "unit": "currency",
        "currencyCode": "TRY",
        "format": "currency",
        "displayHint": "secondary"
      },
      {
        "key": "total_bes_collection_amount",
        "title": "Toplam BES Tahsilati",
        "value": 418750.0,
        "unit": "currency",
        "currencyCode": "TRY",
        "format": "currency",
        "displayHint": "success"
      },
      {
        "key": "activity_to_sale_conversion_rate",
        "title": "Aktiviteden Satisa Donusum",
        "value": 0.147,
        "unit": "ratio",
        "format": "percentage",
        "displayHint": "primary"
      },
      {
        "key": "unlinked_sales_count",
        "title": "Aktiviteye Baglanamayan Satis",
        "value": 7,
        "unit": "count",
        "format": "number",
        "displayHint": "warning"
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "mixed_by_widget",
    "appliedFilters": {
      "startDate": "2026-01-01",
      "endDate": "2026-01-31",
      "timeGrain": "month"
    },
    "warnings": []
  },
  "errors": []
}
```

## 6. `POST /api/dashboard/activities`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "totalActivityCount": 286,
      "contactedActivityCount": 191,
      "notContactedActivityCount": 95,
      "positiveOutcomeCount": 63,
      "negativeOutcomeCount": 38,
      "postponedOutcomeCount": 29,
      "saleClosedOutcomeCount": 16
    },
    "trend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalActivityCount": 61,
        "contactedActivityCount": 39,
        "notContactedActivityCount": 22
      },
      {
        "periodKey": "2026-W02",
        "periodLabel": "08-14 Oca",
        "totalActivityCount": 74,
        "contactedActivityCount": 52,
        "notContactedActivityCount": 22
      }
    ],
    "contactStatusBreakdown": [
      {
        "code": "CONTACTED",
        "label": "Gorusuldu",
        "count": 191,
        "ratio": 0.668
      },
      {
        "code": "NOT_CONTACTED",
        "label": "Gorusulmedi",
        "count": 95,
        "ratio": 0.332
      }
    ],
    "outcomeStatusBreakdown": [
      {
        "code": "POSITIVE",
        "label": "Olumlu",
        "count": 63,
        "ratio": 0.33
      },
      {
        "code": "NEGATIVE",
        "label": "Olumsuz",
        "count": 38,
        "ratio": 0.199
      },
      {
        "code": "POSTPONED",
        "label": "Ertelendi",
        "count": 29,
        "ratio": 0.152
      },
      {
        "code": "SALE_CLOSED",
        "label": "Satis Oldu",
        "count": 16,
        "ratio": 0.084
      }
    ],
    "regionalBreakdown": [
      {
        "region": "Istanbul-Europe",
        "activityCount": 118
      },
      {
        "region": "Istanbul-Anatolia",
        "activityCount": 83
      },
      {
        "region": "Kocaeli",
        "activityCount": 47
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "activity_date",
    "appliedFilters": {
      "startDate": "2026-01-01",
      "endDate": "2026-01-31",
      "timeGrain": "week",
      "regions": [
        "Istanbul-Europe"
      ]
    },
    "warnings": []
  },
  "errors": []
}
```

## 7. `POST /api/dashboard/sales`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "totalSalesCount": 42,
      "linkedSalesCount": 35,
      "unlinkedSalesCount": 7
    },
    "trend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "salesCount": 9
      },
      {
        "periodKey": "2026-W02",
        "periodLabel": "08-14 Oca",
        "salesCount": 11
      }
    ],
    "productBreakdown": [
      {
        "code": "BES",
        "label": "BES",
        "count": 18,
        "ratio": 0.429
      },
      {
        "code": "LIFE",
        "label": "Hayat",
        "count": 12,
        "ratio": 0.286
      },
      {
        "code": "HEALTH",
        "label": "Saglik",
        "count": 7,
        "ratio": 0.167
      },
      {
        "code": "TRAVEL",
        "label": "Seyahat",
        "count": 5,
        "ratio": 0.119
      }
    ],
    "linkedVsUnlinked": [
      {
        "code": "LINKED",
        "label": "Aktiviteye Bagli",
        "count": 35,
        "ratio": 0.833
      },
      {
        "code": "UNLINKED",
        "label": "Bagimsiz",
        "count": 7,
        "ratio": 0.167
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "sale_date",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 8. `POST /api/dashboard/financials`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "totalBesCollectionAmount": 418750.0,
      "totalBesApeAmount": 126300.0,
      "totalBesLumpSumAmount": 82000.0,
      "totalLifePremiumAmount": 96500.0,
      "totalHealthProductionAmount": 54750.0,
      "totalMonthlyPaymentAmount": 28940.0
    },
    "trend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalFinancialAmount": 132500.0
      },
      {
        "periodKey": "2026-W02",
        "periodLabel": "08-14 Oca",
        "totalFinancialAmount": 118250.0
      }
    ],
    "productFinancialBreakdown": [
      {
        "code": "BES",
        "label": "BES",
        "amount": 627050.0,
        "ratio": 0.657
      },
      {
        "code": "LIFE",
        "label": "Hayat",
        "amount": 96500.0,
        "ratio": 0.101
      },
      {
        "code": "HEALTH",
        "label": "Saglik",
        "amount": 54750.0,
        "ratio": 0.057
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "sale_date",
    "appliedFilters": {},
    "warnings": [
      {
        "code": "INCOMPLETE_FINANCIAL_FIELDS",
        "message": "Bazi satis kayitlarinda urun tipine gore beklenen finansal alanlar eksiktir.",
        "severity": "warning"
      }
    ]
  },
  "errors": []
}
```

## 9. `POST /api/dashboard/expenses`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "totalExpenseAmount": 58240.5,
      "expensePerEmployeeAmount": 8320.07,
      "expensePerSaleAmount": 1386.68
    },
    "trend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalExpenseAmount": 14620.25
      },
      {
        "periodKey": "2026-W02",
        "periodLabel": "08-14 Oca",
        "totalExpenseAmount": 13285.0
      }
    ],
    "expenseTypeBreakdown": [
      {
        "code": "TRAVEL",
        "label": "Yol",
        "amount": 26120.0,
        "ratio": 0.448
      },
      {
        "code": "MEAL",
        "label": "Yemek",
        "amount": 14480.5,
        "ratio": 0.249
      },
      {
        "code": "ACCOMMODATION",
        "label": "Konaklama",
        "amount": 11840.0,
        "ratio": 0.203
      },
      {
        "code": "OTHER",
        "label": "Diger",
        "amount": 5800.0,
        "ratio": 0.1
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "expense_date",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 10. `POST /api/dashboard/conversions`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "activityToSaleConversionRate": 0.147,
      "contactToSaleConversionRate": 0.22,
      "expenseToSalesRatio": 1386.68,
      "expenseToCollectionRatio": 0.139
    },
    "employeeConversionRows": [
      {
        "employeeId": "emp-001",
        "employeeName": "Ahmet Yilmaz",
        "activityCount": 52,
        "contactedActivityCount": 39,
        "salesCount": 8,
        "activityToSaleConversionRate": 0.154,
        "contactToSaleConversionRate": 0.205
      }
    ],
    "regionalConversionRows": [
      {
        "region": "Istanbul-Europe",
        "activityCount": 118,
        "salesCount": 21,
        "activityToSaleConversionRate": 0.178
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "activity_date",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 11. `POST /api/dashboard/performance`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summary": {
      "employeeCount": 7,
      "totalActivityCount": 286,
      "totalSalesCount": 42
    },
    "rows": [
      {
        "employeeId": "emp-001",
        "employeeName": "Ahmet Yilmaz",
        "activityCount": 52,
        "contactedActivityCount": 39,
        "salesCount": 8,
        "besCollectionAmount": 96500.0,
        "expenseAmount": 8420.0,
        "activityToSaleConversionRate": 0.154,
        "contactToSaleConversionRate": 0.205
      }
    ],
    "totals": {
      "totalActivityCount": 286,
      "totalSalesCount": 42,
      "totalExpenseAmount": 58240.5,
      "totalBesCollectionAmount": 418750.0
    }
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "mixed_by_section",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 12. `POST /api/dashboard/executive-overview`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "summaryCards": [
      {
        "key": "total_activity_count",
        "title": "Toplam Aktivite",
        "value": 286,
        "unit": "count",
        "format": "number",
        "displayHint": "primary"
      }
    ],
    "activityTrend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalActivityCount": 61
      }
    ],
    "salesTrend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "salesCount": 9
      }
    ],
    "financialTrend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalFinancialAmount": 132500.0
      }
    ],
    "expenseTrend": [
      {
        "periodKey": "2026-W01",
        "periodLabel": "01-07 Oca",
        "totalExpenseAmount": 14620.25
      }
    ],
    "productBreakdown": [
      {
        "code": "BES",
        "label": "BES",
        "count": 18,
        "ratio": 0.429
      }
    ],
    "performanceTable": [
      {
        "employeeId": "emp-001",
        "employeeName": "Ahmet Yilmaz",
        "activityCount": 52,
        "salesCount": 8,
        "expenseAmount": 8420.0
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": "mixed_by_section",
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 13. `GET /api/dashboard/reference-filters`

### Ornek Response

```json
{
  "success": true,
  "data": {
    "employees": [
      {
        "id": "emp-001",
        "code": "EMP-001",
        "label": "Ahmet Yilmaz"
      }
    ],
    "productTypes": [
      {
        "id": "prod-bes",
        "code": "BES",
        "label": "BES"
      }
    ],
    "expenseTypes": [
      {
        "id": "exp-travel",
        "code": "TRAVEL",
        "label": "Yol"
      }
    ],
    "activityContactStatuses": [
      {
        "id": "acs-contacted",
        "code": "CONTACTED",
        "label": "Gorusuldu"
      },
      {
        "id": "acs-not-contacted",
        "code": "NOT_CONTACTED",
        "label": "Gorusulmedi"
      }
    ],
    "activityOutcomeStatuses": [
      {
        "id": "aos-positive",
        "code": "POSITIVE",
        "label": "Olumlu"
      },
      {
        "id": "aos-sale-closed",
        "code": "SALE_CLOSED",
        "label": "Satis Oldu"
      }
    ],
    "regions": [
      {
        "id": "reg-1",
        "code": "IST_EU",
        "label": "Istanbul-Europe"
      }
    ],
    "cities": [
      {
        "id": "city-34",
        "code": "34",
        "label": "Istanbul"
      }
    ]
  },
  "meta": {
    "generatedAt": "2026-04-04T10:30:00Z",
    "dateInterpretation": null,
    "appliedFilters": {},
    "warnings": []
  },
  "errors": []
}
```

## 14. Validation Error Ornegi

```json
{
  "success": false,
  "data": null,
  "meta": {},
  "errors": [
    {
      "code": "INVALID_FILTER_COMBINATION",
      "message": "`includeOnlyLinkedSales` ve `includeOnlyUnlinkedSales` ayni anda true olamaz.",
      "field": "includeOnlyLinkedSales"
    }
  ]
}
```

## 15. Frontend Notlari

- Aktivite dashboard ekraninda temas dagilimi ve sonuc dagilimi ayri grafikler olarak render edilmelidir.
- `SALE_CLOSED` sonucu olsa bile satis gercegi gerekiyorsa satis endpoint'i veya linked sales metriği esas alinmalidir.
- `mixed_by_widget` ve `mixed_by_section` gibi `dateInterpretation` degerleri UI tarafinda acik metin yardimi gerektirebilir.
