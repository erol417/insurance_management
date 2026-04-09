# Endpoint To DTO Map

## 1. Amac

Bu belge, API endpoint'leri ile request/response DTO'lari arasindaki eslesmeyi tek yerde toplar. Amac; hangi endpoint'in hangi veri sozlesmesini kullandigini, hangi roller tarafindan cagrilacagini, MVP kapsamina girip girmedigini ve hangi notlarla uygulanmasi gerektigini netlestirmektir.

Bu dokuman:

- `api_contracts.md` ile uyumludur
- `dto_catalog.md` icindeki DTO envanterinin endpoint bazli kullanim haritasini verir
- Backend controller ve application service tasarimini hizlandirir
- Frontend entegrasyonunda hangi payload'in beklenecegini netlestirir

## 2. Okuma Rehberi

Tablolardaki sutunlar:

- `Method`: HTTP method
- `Endpoint`: route kalibi
- `Request DTO`: body veya query/filter karsiligi ana DTO
- `Response DTO`: `data` alaninda donmesi beklenen ana DTO
- `Roles`: erisim beklenen roller
- `MVP`: ilk faz kapsami
- `Notes`: kritik uygulama notlari

Terminoloji:

- `None`: body gerektirmeyen endpoint
- `query + pagination`: query string ile gelen filtre ve sayfalama yapisi
- `ApiEnvelope<T>`: `api_contracts.md` icindeki standart response zarfi

## 3. Genel Eslesme Kararlari

- Tum endpoint response'lari standart `ApiEnvelope<T>` mantigiyla donmelidir.
- Liste endpoint'leri `PagedResultDto<TItem>` kullanmalidir.
- Create ve update endpoint'leri entity detail DTO donmelidir.
- Lookup endpoint'leri hafif veri tasimalidir; temel beklenti `LookupItemDto` veya ondan tureyen alanlardir.
- Dashboard endpoint'leri CRUD DTO'larindan ayri, raporlama odakli composite DTO'lar kullanmalidir.
- Delete endpoint'leri icin ortak `DeleteResultDto` tanimi onerilir.

## 4. Auth Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| POST | `/api/auth/login` | `LoginRequestDto` | `LoginResponseDto` | Public | Yes | Kullanici girisi |
| POST | `/api/auth/refresh` | `RefreshTokenRequestDto` | `LoginResponseDto` | Authenticated | Future | Token yenileme |
| POST | `/api/auth/logout` | `LogoutRequestDto` | `OperationResultDto` | Authenticated | Future | Token/session invalidation |
| GET | `/api/auth/me` | None | `AuthenticatedUserDto` | Authenticated | Yes | Oturumdaki kullanici bilgisi |

## 5. User And Role Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/users` | `UserFilterDto` + query + pagination | `PagedResultDto<UserListItemDto>` | Admin | Yes | Uygulama kullanicilari |
| GET | `/api/users/{id}` | None | `UserDetailDto` | Admin | Yes | Detay |
| POST | `/api/users` | `UserCreateRequestDto` | `UserDetailDto` | Admin | Yes | Employee ile iliski kurulabilir |
| PATCH | `/api/users/{id}` | `UserUpdateRequestDto` | `UserDetailDto` | Admin | Yes | Durum/iletisim guncelleme |
| PATCH | `/api/users/{id}/roles` | `UserRoleAssignmentRequestDto` | `UserRoleAssignmentResultDto` | Admin | Yes | Rol atama |
| GET | `/api/roles` | None | `RoleListItemDto[]` | Admin | Yes | Sabit veya yonetilebilir rol listesi |

## 6. Employee Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/employees` | `EmployeeFilterDto` + query + pagination | `PagedResultDto<EmployeeListItemDto>` | Admin, Manager, Operations | Yes | Listeleme |
| GET | `/api/employees/{id}` | None | `EmployeeDetailDto` | Admin, Manager, Operations | Yes | Detay |
| POST | `/api/employees` | `EmployeeCreateRequestDto` | `EmployeeDetailDto` | Admin | Yes | Yeni saha veya ofis personeli |
| PATCH | `/api/employees/{id}` | `EmployeeUpdateRequestDto` | `EmployeeDetailDto` | Admin | Yes | Kismi guncelleme |
| DELETE | `/api/employees/{id}` | None | `DeleteResultDto` | Admin | Yes | Soft delete onerilir |

## 7. Account Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/accounts` | `AccountFilterDto` + query + pagination | `PagedResultDto<AccountListItemDto>` | Admin, Manager, Operations, FieldSales scoped | Yes | Liste |
| GET | `/api/accounts/{id}` | None | `AccountDetailDto` | Admin, Manager, Operations, FieldSales scoped | Yes | Detay |
| POST | `/api/accounts` | `AccountCreateRequestDto` | `AccountDetailDto` | Admin, Operations, FieldSales | Yes | Individual/Corporate ayrimi request icinde tasinabilir |
| PATCH | `/api/accounts/{id}` | `AccountUpdateRequestDto` | `AccountDetailDto` | Admin, Operations | Yes | Duplicate kontrolu uygulanmali |
| DELETE | `/api/accounts/{id}` | None | `DeleteResultDto` | Admin, Operations | Yes | Soft delete onerilir |

## 8. Account Contact Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/accounts/{accountId}/contacts` | query optional | `AccountContactListItemDto[]` | Admin, Manager, Operations, FieldSales scoped | Yes | Hesap altindaki temas kisileri |
| POST | `/api/accounts/{accountId}/contacts` | `AccountContactCreateRequestDto` | `AccountContactDetailDto` | Admin, Operations, FieldSales | Yes | Kurumsal hesapta onemli |
| PATCH | `/api/account-contacts/{id}` | `AccountContactUpdateRequestDto` | `AccountContactDetailDto` | Admin, Operations | Yes | |
| DELETE | `/api/account-contacts/{id}` | None | `DeleteResultDto` | Admin, Operations | Yes | |

## 9. Activity Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/activities` | `ActivityFilterDto` + query + pagination | `PagedResultDto<ActivityListItemDto>` | Admin, Manager, Operations, FieldSales scoped | Yes | Liste |
| GET | `/api/activities/{id}` | None | `ActivityDetailDto` | Admin, Manager, Operations, FieldSales scoped | Yes | Detay |
| POST | `/api/activities` | `ActivityCreateRequestDto` | `ActivityDetailDto` | Admin, Operations, FieldSales | Yes | `contactStatus` zorunlu, `outcomeStatus` kosullu |
| PATCH | `/api/activities/{id}` | `ActivityUpdateRequestDto` | `ActivityDetailDto` | Admin, Operations, FieldSales scoped | Yes | Contact/outcome kural seti kontrol edilmeli |
| DELETE | `/api/activities/{id}` | None | `DeleteResultDto` | Admin, Operations, FieldSales scoped | Yes | Soft delete onerilir |

## 10. Sale Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/sales` | `SaleFilterDto` + query + pagination | `PagedResultDto<SaleListItemDto>` | Admin, Manager, Operations, FieldSales scoped | Yes | Liste |
| GET | `/api/sales/{id}` | None | `SaleDetailDto` | Admin, Manager, Operations, FieldSales scoped | Yes | Detay |
| POST | `/api/sales` | `SaleCreateRequestDto` | `SaleDetailDto` | Admin, Operations, FieldSales | Yes | `activityId` MVP'de opsiyonel |
| PATCH | `/api/sales/{id}` | `SaleUpdateRequestDto` | `SaleDetailDto` | Admin, Operations | Yes | Urun tipine gore finansal validation |
| DELETE | `/api/sales/{id}` | None | `DeleteResultDto` | Admin, Operations | Yes | Soft delete onerilir |

## 11. Expense Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/expenses` | `ExpenseFilterDto` + query + pagination | `PagedResultDto<ExpenseListItemDto>` | Admin, Manager, Operations, FieldSales scoped | Yes | Liste |
| GET | `/api/expenses/{id}` | None | `ExpenseDetailDto` | Admin, Manager, Operations, FieldSales scoped | Yes | Detay |
| POST | `/api/expenses` | `ExpenseCreateRequestDto` | `ExpenseDetailDto` | Admin, Operations, FieldSales | Yes | MVP'de onay akisi olmayabilir |
| PATCH | `/api/expenses/{id}` | `ExpenseUpdateRequestDto` | `ExpenseDetailDto` | Admin, Operations, FieldSales scoped | Yes | Tutar ve tarih validation'i gerekli |
| DELETE | `/api/expenses/{id}` | None | `DeleteResultDto` | Admin, Operations | Yes | Soft delete onerilir |

## 12. Lookup Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/lookups/employees` | query optional | `EmployeeLookupDto[]` | Authenticated | Yes | Dropdown |
| GET | `/api/lookups/accounts` | query optional | `AccountLookupDto[]` | Authenticated | Yes | Dropdown, arama destekli olabilir |
| GET | `/api/lookups/product-types` | None | `SaleProductTypeLookupDto[]` | Authenticated | Yes | BES/Hayat/Saglik/Seyahat/Diger |
| GET | `/api/lookups/expense-types` | None | `ExpenseTypeLookupDto[]` | Authenticated | Yes | Yol/Yemek/Konaklama/Diger |
| GET | `/api/lookups/activity-contact-statuses` | None | `ActivityContactStatusLookupDto[]` | Authenticated | Yes | `CONTACTED`, `NOT_CONTACTED` gibi |
| GET | `/api/lookups/activity-outcome-statuses` | None | `ActivityOutcomeStatusLookupDto[]` | Authenticated | Yes | `POSITIVE`, `NEGATIVE`, `POSTPONED`, `SALE_CLOSED` |
| GET | `/api/lookups/regions` | None | `RegionLookupDto[]` | Authenticated | Yes | Dashboard ve filtre ekranlari |
| GET | `/api/lookups/cities` | `CityLookupFilterDto` | `CityLookupDto[]` | Authenticated | Yes | Bolgeye gore filtrelenebilir |
| GET | `/api/lookups/districts` | `DistrictLookupFilterDto` | `DistrictLookupDto[]` | Authenticated | Yes | Sehir bazli filtrelenebilir |

## 13. Dashboard Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| POST | `/api/dashboard/executive-overview` | `DashboardFilterRequestDto` | `ExecutiveOverviewDto` | Manager, Admin | Yes | Ana dashboard birlesik payload |
| POST | `/api/dashboard/summary` | `DashboardFilterRequestDto` | `DashboardSummaryResponseDto` | Manager, Admin, Operations partial | Yes | Ust KPI kartlari |
| POST | `/api/dashboard/activities` | `DashboardFilterRequestDto` | `ActivityDashboardResponseDto` | Manager, Admin, Operations | Yes | Aktivite trend ve dagilimlari |
| POST | `/api/dashboard/sales` | `DashboardFilterRequestDto` | `SalesDashboardResponseDto` | Manager, Admin, Operations | Yes | Satis trend ve urun kirilimlari |
| POST | `/api/dashboard/financials` | `DashboardFilterRequestDto` | `FinancialDashboardResponseDto` | Manager, Admin, Operations | Yes | Tahsilat, APE, prim ve uretim ozetleri |
| POST | `/api/dashboard/expenses` | `DashboardFilterRequestDto` | `ExpenseDashboardResponseDto` | Manager, Admin, Operations | Yes | Masraf trend ve kirilimlari |
| POST | `/api/dashboard/conversions` | `DashboardFilterRequestDto` | `ConversionDashboardResponseDto` | Manager, Admin | Yes | Donusum oranlari |
| POST | `/api/dashboard/performance` | `DashboardFilterRequestDto` | `PerformanceDashboardResponseDto` | Manager, Admin | Yes | Cok kolonlu personel performans tablosu |
| GET | `/api/dashboard/reference-filters` | None | `DashboardReferenceFiltersDto` | Authenticated | Yes | Dashboard filtre dropdown verisi |

## 14. Dashboard Composite DTO Icerigi

Onerilen composite response DTO icerikleri:

- `DashboardSummaryResponseDto`
  - `cards: DashboardSummaryCardDto[]`
- `ActivityDashboardResponseDto`
  - `summary: ActivitySummaryDto`
  - `trend: ActivityTrendPointDto[]`
  - `contactStatusBreakdown: ContactStatusBreakdownDto[]`
  - `outcomeStatusBreakdown: OutcomeStatusBreakdownDto[]`
  - `regionalBreakdown: RegionalActivityBreakdownDto[]`
- `SalesDashboardResponseDto`
  - `summary: SalesSummaryDto`
  - `trend: SalesTrendPointDto[]`
  - `productBreakdown: ProductBreakdownDto[]`
  - `linkedVsUnlinked: LinkedUnlinkedSalesBreakdownDto[]`
- `FinancialDashboardResponseDto`
  - `summary: FinancialSummaryDto`
  - `trend: FinancialTrendPointDto[]`
  - `productFinancialBreakdown: ProductFinancialBreakdownDto[]`
- `ExpenseDashboardResponseDto`
  - `summary: ExpenseSummaryDto`
  - `trend: ExpenseTrendPointDto[]`
  - `expenseTypeBreakdown: ExpenseTypeBreakdownDto[]`
- `ConversionDashboardResponseDto`
  - `summary: ConversionSummaryDto`
  - `employeeConversionRows: EmployeeConversionRowDto[]`
  - `regionalConversionRows: RegionalConversionRowDto[]`
- `PerformanceDashboardResponseDto`
  - `summary: PerformanceSummaryDto`
  - `rows: EmployeePerformanceRowDto[]`
  - `totals: EmployeePerformanceTotalsDto`
- `ExecutiveOverviewDto`
  - `summaryCards: DashboardSummaryCardDto[]`
  - `activityTrend: ActivityTrendPointDto[]`
  - `salesTrend: SalesTrendPointDto[]`
  - `financialTrend: FinancialTrendPointDto[]`
  - `expenseTrend: ExpenseTrendPointDto[]`
  - `productBreakdown: ProductBreakdownDto[]`
  - `performanceTable: EmployeePerformanceRowDto[]`

## 15. Import Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| POST | `/api/imports/validate` | `ImportUploadRequestDto` | `ImportValidationResultDto` | Admin, Operations | Future | Excel dosyasini on-kontrol eder |
| POST | `/api/imports/commit` | `ImportCommitRequestDto` | `ImportCommitResultDto` | Admin, Operations | Future | Dogrulanan batch'i sisteme yazar |
| GET | `/api/imports` | query + pagination | `PagedResultDto<ImportBatchListItemDto>` | Admin, Operations | Future | Batch listesi |
| GET | `/api/imports/{id}` | None | `ImportBatchDetailDto` | Admin, Operations | Future | Batch ozeti |
| GET | `/api/imports/{id}/errors` | None | `ImportRowErrorDto[]` | Admin, Operations | Future | Hata satirlari |

## 16. Audit Endpoints

| Method | Endpoint | Request DTO | Response DTO | Roles | MVP | Notes |
|---|---|---|---|---|---|---|
| GET | `/api/audit-logs` | `AuditLogFilterDto` + query + pagination | `PagedResultDto<AuditLogListItemDto>` | Admin | Future | Liste |
| GET | `/api/audit-logs/{id}` | None | `AuditLogDetailDto` | Admin | Future | Detay |

## 17. MVP Oncelik Sirasi

MVP ilk dalga:

- Auth
- Users and roles
- Employees
- Accounts and account contacts
- Activities
- Sales
- Expenses
- Lookups

MVP ikinci dalga:

- Dashboard endpoints
- Dashboard composite DTO sonlandirma

Future:

- Import endpoints
- Audit endpoints
- Refresh/logout gibi auth genislemeleri

## 18. Kritik Uygulama Notlari

### Activity endpoint'leri

- `ActivityCreateRequestDto` ve `ActivityUpdateRequestDto`, tek bir sonuc alani yerine `contactStatus` ve `outcomeStatus` kullanmalidir.
- `outcomeStatus`, `contactStatus = CONTACTED` degilse bos birakilabilir veya kural bazli sinirlanmalidir.

### Sale endpoint'leri

- `SaleCreateRequestDto.activityId` nullable olmalidir.
- `SaleDetailDto` icinde `isLinkedToActivity` gibi turetilmis bir alan faydali olabilir.
- Finansal alanlar urun tipine gore kosullu zorunlu olmalidir.

### Expense endpoint'leri

- Masraf verisinin KPI etkisi oldugu icin `expenseDate`, `employeeId`, `expenseTypeId` ve `amount` alanlari zorunlu olmalidir.

### Dashboard endpoint'leri

- Tum dashboard response'lari standart response zarfi icinde donmelidir.
- Composite DTO'lar frontend'e uygun ama UI'ya asiri bagimli olmayan seviyede tasarlanmalidir.

## 19. Acik Noktalar

- `DeleteResultDto` ortak DTO olarak kataloga eklenmeli mi?
- User/role DTO seti ayri detay belgesinde mi genisletilmeli?
- Lookup endpoint'leri tamamen merkezi mi olacak, yoksa modullere gore alt-gruplar da tanimlanacak mi?
- Dashboard composite DTO'lari ayri bir detay dokumana ayrilmali mi?

## 20. Sonuc

Bu belge ile:

- endpoint ve DTO eslesmeleri
- rol bazli erisim beklentisi
- MVP kapsami
- composite dashboard response yaklasimi

tek yerde gorunur hale gelmistir.
