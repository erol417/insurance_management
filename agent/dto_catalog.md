# DTO Catalog

## 1. Amaç

Bu belge, proje genelinde kullanılacak request ve response DTO'larının envanterini çıkarır. Amaç; backend ve frontend arasında taşınacak veri modellerini modül bazında netleştirmek, endpoint tasarımını kolaylaştırmak ve entity ile API sözleşmesini birbirinden ayırmaktır.

Bu doküman:

- API tasarımı için DTO isimlendirme standardı sunar
- Hangi modülde hangi request/response modellerine ihtiyaç olduğunu listeler
- Liste, detay, create, update, lookup ve dashboard DTO'larını ayırır
- MVP önceliğini koruyarak hangi DTO'ların ilk aşamada gerekli olduğunu gösterir

## 2. DTO Yaklaşımı

### DTO nedir?

DTO, API katmanında veri taşımak için kullanılan sözleşme modelidir. Domain entity ile birebir aynı olmak zorunda değildir. Hatta çoğu durumda aynı olmamalıdır.

### Neden gerekli?

- Entity'lerin API'ye sızmasını engeller
- Liste, detay ve form ihtiyaçlarını ayrı ayrı optimize eder
- Validation sınırlarını netleştirir
- Frontend'in hangi alanı nerede alacağını açık hale getirir
- Gelecekte domain refactor olduğunda API'nin daha kontrollü değişmesini sağlar

### DTO kategorileri

Bu proje için DTO'lar aşağıdaki gruplarda düşünülmelidir:

- Create request DTO
- Update request DTO
- List item DTO
- Detail DTO
- Filter DTO
- Lookup DTO
- Dashboard DTO
- Auth DTO

## 3. İsimlendirme Standardı

Önerilen isim formatı:

- `{Entity}CreateRequestDto`
- `{Entity}UpdateRequestDto`
- `{Entity}ListItemDto`
- `{Entity}DetailDto`
- `{Entity}FilterDto`
- `{Entity}LookupDto`

Örnekler:

- `ActivityCreateRequestDto`
- `ActivityUpdateRequestDto`
- `ActivityListItemDto`
- `ActivityDetailDto`
- `ActivityFilterDto`

Dashboard tarafında:

- `{Area}SummaryDto`
- `{Area}TrendPointDto`
- `{Area}BreakdownItemDto`

Örnekler:

- `DashboardSummaryCardDto`
- `ActivityTrendPointDto`
- `SalesProductBreakdownDto`

## 4. Ortak DTO Prensipleri

- Tüm DTO alan isimleri `camelCase` olacak şekilde serialize edilmelidir
- DTO'lar entity navigation property taşımamalıdır
- Liste DTO'ları hafif tutulmalıdır
- Detail DTO'ları kullanıcıya anlamlı bağlamsal alanlar içerebilir
- Create ve update request DTO'ları ayrı düşünülmelidir
- Filter DTO'ları liste ve dashboard için ayrı tutulmalıdır

## 5. Ortak / Çapraz DTO'lar

## 5.1 `PagedResultDto<T>`

Amaç:

- Liste endpoint'lerinde standart response gövdesi sağlamak

Ana alanlar:

- `items`
- `pagination`

MVP'de var mı?:

- Evet

## 5.2 `PaginationDto`

Ana alanlar:

- `page`
- `pageSize`
- `totalCount`
- `totalPages`

MVP'de var mı?:

- Evet

## 5.3 `LookupItemDto`

Amaç:

- Referans veri ve dropdown alanlarını standartlaştırmak

Ana alanlar:

- `id`
- `code`
- `label`

MVP'de var mı?:

- Evet

## 5.4 `DateRangeDto`

Ana alanlar:

- `startDate`
- `endDate`

MVP'de var mı?:

- Evet

## 5.5 `ApiErrorDto`

Ana alanlar:

- `code`
- `message`
- `field`
- `details`

MVP'de var mı?:

- Evet

## 6. Auth DTO'ları

## 6.1 `LoginRequestDto`

Amaç:

- Kullanıcı giriş isteği

Ana alanlar:

- `username`
- `password`

MVP'de var mı?:

- Evet

## 6.2 `LoginResponseDto`

Ana alanlar:

- `accessToken`
- `expiresAt`
- `user`

MVP'de var mı?:

- Evet

## 6.3 `AuthenticatedUserDto`

Ana alanlar:

- `id`
- `username`
- `displayName`
- `roles`

MVP'de var mı?:

- Evet

## 7. Employee DTO'ları

## 7.1 `EmployeeCreateRequestDto`

Ana alanlar:

- `code`
- `firstName`
- `lastName`
- `phone`
- `email`
- `title`
- `region`
- `hireDate`

MVP'de var mı?:

- Evet

## 7.2 `EmployeeUpdateRequestDto`

Ana alanlar:

- `firstName`
- `lastName`
- `phone`
- `email`
- `title`
- `region`
- `isActive`

MVP'de var mı?:

- Evet

## 7.3 `EmployeeListItemDto`

Amaç:

- Employee list ekranı

Ana alanlar:

- `id`
- `code`
- `fullName`
- `title`
- `region`
- `isActive`

MVP'de var mı?:

- Evet

## 7.4 `EmployeeDetailDto`

Ana alanlar:

- `id`
- `code`
- `firstName`
- `lastName`
- `fullName`
- `phone`
- `email`
- `title`
- `region`
- `hireDate`
- `isActive`

MVP'de var mı?:

- Evet

## 7.5 `EmployeeFilterDto`

Ana alanlar:

- `searchText`
- `region`
- `isActive`

MVP'de var mı?:

- Evet

## 7.6 `EmployeeLookupDto`

Ana alanlar:

- `id`
- `label`

MVP'de var mı?:

- Evet

## 8. Account DTO'ları

## 8.1 `AccountCreateRequestDto`

Ana alanlar:

- `accountType`
- `name`
- `legalName`
- `taxNumber`
- `identityNumber`
- `phone`
- `email`
- `city`
- `district`
- `address`
- `notes`

MVP'de var mı?:

- Evet

## 8.2 `AccountUpdateRequestDto`

Ana alanlar:

- `name`
- `legalName`
- `taxNumber`
- `identityNumber`
- `phone`
- `email`
- `city`
- `district`
- `address`
- `notes`
- `isActive`

MVP'de var mı?:

- Evet

## 8.3 `AccountListItemDto`

Ana alanlar:

- `id`
- `accountType`
- `name`
- `city`
- `district`
- `phone`
- `isActive`

MVP'de var mı?:

- Evet

## 8.4 `AccountDetailDto`

Ana alanlar:

- `id`
- `accountType`
- `name`
- `legalName`
- `taxNumber`
- `identityNumber`
- `phone`
- `email`
- `city`
- `district`
- `address`
- `notes`
- `isActive`
- `contacts`

MVP'de var mı?:

- Evet

## 8.5 `AccountFilterDto`

Ana alanlar:

- `searchText`
- `accountType`
- `city`
- `district`
- `isActive`

MVP'de var mı?:

- Evet

## 8.6 `AccountContactDto`

Ana alanlar:

- `id`
- `fullName`
- `title`
- `phone`
- `email`
- `isPrimary`

MVP'de var mı?:

- Opsiyonel

## 8.7 `AccountLookupDto`

Ana alanlar:

- `id`
- `label`
- `accountType`

MVP'de var mı?:

- Evet

## 9. Activity DTO'ları

## 9.1 `ActivityCreateRequestDto`

Ana alanlar:

- `employeeId`
- `accountId`
- `accountContactId`
- `activityDate`
- `visitedCity`
- `visitedLocation`
- `channel`
- `subject`
- `content`
- `contactStatus`
- `outcomeStatus`
- `followUpDate`
- `contactPersonName`
- `contactPersonTitle`
- `notesSummary`

MVP'de var mı?:

- Evet

Not:

- `contactStatus` zorunludur
- `outcomeStatus`, `contactStatus = CONTACTED` ise zorunlu veya varsayılanlı olmalıdır

## 9.2 `ActivityUpdateRequestDto`

Ana alanlar:

- `accountId`
- `accountContactId`
- `activityDate`
- `visitedCity`
- `visitedLocation`
- `channel`
- `subject`
- `content`
- `contactStatus`
- `outcomeStatus`
- `followUpDate`
- `contactPersonName`
- `contactPersonTitle`
- `notesSummary`

MVP'de var mı?:

- Evet

## 9.3 `ActivityListItemDto`

Ana alanlar:

- `id`
- `activityDate`
- `employeeId`
- `employeeName`
- `accountId`
- `accountName`
- `contactStatus`
- `outcomeStatus`
- `hasSale`

MVP'de var mı?:

- Evet

## 9.4 `ActivityDetailDto`

Ana alanlar:

- `id`
- `employeeId`
- `employeeName`
- `accountId`
- `accountName`
- `accountContactId`
- `accountContactName`
- `activityDate`
- `visitedCity`
- `visitedLocation`
- `channel`
- `subject`
- `content`
- `contactStatus`
- `outcomeStatus`
- `followUpDate`
- `hasSale`
- `linkedSaleId`
- `contactPersonName`
- `contactPersonTitle`
- `notesSummary`
- `createdAt`
- `updatedAt`

MVP'de var mı?:

- Evet

## 9.5 `ActivityFilterDto`

Ana alanlar:

- `employeeIds`
- `accountIds`
- `contactStatuses`
- `outcomeStatuses`
- `startDate`
- `endDate`
- `hasSale`
- `cities`
- `regions`

MVP'de var mı?:

- Evet

## 9.6 `ActivityContactStatusLookupDto`

Ana alanlar:

- `id`
- `code`
- `label`

MVP'de var mı?:

- Evet

## 9.7 `ActivityOutcomeStatusLookupDto`

Ana alanlar:

- `id`
- `code`
- `label`

MVP'de var mı?:

- Evet

## 10. Sale DTO'ları

## 10.1 `SaleCreateRequestDto`

Ana alanlar:

- `employeeId`
- `accountId`
- `activityId`
- `productTypeId`
- `saleDate`
- `policyStartDate`
- `policyEndDate`
- `premiumAmount`
- `apeAmount`
- `lumpSumAmount`
- `monthlyPaymentAmount`
- `collectionAmount`
- `saleCount`
- `currencyCode`
- `description`

MVP'de var mı?:

- Evet

## 10.2 `SaleUpdateRequestDto`

Ana alanlar:

- `activityId`
- `productTypeId`
- `saleDate`
- `policyStartDate`
- `policyEndDate`
- `premiumAmount`
- `apeAmount`
- `lumpSumAmount`
- `monthlyPaymentAmount`
- `collectionAmount`
- `saleCount`
- `currencyCode`
- `description`

MVP'de var mı?:

- Evet

## 10.3 `SaleListItemDto`

Ana alanlar:

- `id`
- `saleDate`
- `employeeId`
- `employeeName`
- `accountId`
- `accountName`
- `productTypeId`
- `productTypeCode`
- `saleCount`
- `activityId`
- `isLinkedToActivity`

MVP'de var mı?:

- Evet

## 10.4 `SaleDetailDto`

Ana alanlar:

- `id`
- `employeeId`
- `employeeName`
- `accountId`
- `accountName`
- `activityId`
- `productTypeId`
- `productTypeCode`
- `productTypeName`
- `saleDate`
- `policyStartDate`
- `policyEndDate`
- `premiumAmount`
- `apeAmount`
- `lumpSumAmount`
- `monthlyPaymentAmount`
- `collectionAmount`
- `saleCount`
- `currencyCode`
- `description`
- `createdAt`
- `updatedAt`

MVP'de var mı?:

- Evet

## 10.5 `SaleFilterDto`

Ana alanlar:

- `employeeIds`
- `accountIds`
- `productTypeIds`
- `startDate`
- `endDate`
- `isLinkedToActivity`
- `cities`
- `regions`

MVP'de var mı?:

- Evet

## 10.6 `SaleProductTypeLookupDto`

Ana alanlar:

- `id`
- `code`
- `label`

MVP'de var mı?:

- Evet

## 11. Expense DTO'ları

## 11.1 `ExpenseCreateRequestDto`

Ana alanlar:

- `employeeId`
- `expenseTypeId`
- `expenseDate`
- `amount`
- `currencyCode`
- `description`
- `relatedActivityId`
- `receiptNo`

MVP'de var mı?:

- Evet

## 11.2 `ExpenseUpdateRequestDto`

Ana alanlar:

- `expenseTypeId`
- `expenseDate`
- `amount`
- `currencyCode`
- `description`
- `relatedActivityId`
- `receiptNo`

MVP'de var mı?:

- Evet

## 11.3 `ExpenseListItemDto`

Ana alanlar:

- `id`
- `expenseDate`
- `employeeId`
- `employeeName`
- `expenseTypeId`
- `expenseTypeName`
- `amount`
- `currencyCode`

MVP'de var mı?:

- Evet

## 11.4 `ExpenseDetailDto`

Ana alanlar:

- `id`
- `employeeId`
- `employeeName`
- `expenseTypeId`
- `expenseTypeName`
- `expenseDate`
- `amount`
- `currencyCode`
- `description`
- `relatedActivityId`
- `receiptNo`
- `createdAt`
- `updatedAt`

MVP'de var mı?:

- Evet

## 11.5 `ExpenseFilterDto`

Ana alanlar:

- `employeeIds`
- `expenseTypeIds`
- `startDate`
- `endDate`
- `regions`

MVP'de var mı?:

- Evet

## 11.6 `ExpenseTypeLookupDto`

Ana alanlar:

- `id`
- `code`
- `label`

MVP'de var mı?:

- Evet

## 12. Dashboard DTO'ları

## 12.1 `DashboardSummaryCardDto`

Ana alanlar:

- `key`
- `title`
- `value`
- `unit`
- `format`
- `currencyCode`
- `displayHint`

MVP'de var mı?:

- Evet

## 12.2 `DashboardFilterRequestDto`

Ana alanlar:

- `dateRange`
- `timeGrain`
- `employeeIds`
- `productTypeIds`
- `accountIds`
- `contactStatuses`
- `outcomeStatuses`
- `expenseTypeIds`
- `regions`
- `cities`
- `districts`
- `includeOnlyLinkedSales`
- `includeOnlyUnlinkedSales`

MVP'de var mı?:

- Evet

## 12.3 `ActivityTrendPointDto`

Ana alanlar:

- `periodKey`
- `periodLabel`
- `totalActivityCount`

MVP'de var mı?:

- Evet

## 12.4 `SalesTrendPointDto`

Ana alanlar:

- `periodKey`
- `periodLabel`
- `salesCount`

MVP'de var mı?:

- Evet

## 12.5 `FinancialTrendPointDto`

Ana alanlar:

- `periodKey`
- `periodLabel`
- `financialProductionAmount`

MVP'de var mı?:

- Evet

## 12.6 `ExpenseTrendPointDto`

Ana alanlar:

- `periodKey`
- `periodLabel`
- `expenseAmount`

MVP'de var mı?:

- Evet

## 12.7 `ActivityResultBreakdownDto`

Ana alanlar:

- `resultCode`
- `resultName`
- `count`

MVP'de var mı?:

- Geçiş amaçlı kullanılabilir

Not:

- Nihai dashboard tasarımında bunun yerine `ContactStatusBreakdownDto` ve `OutcomeStatusBreakdownDto` ayrımı daha doğrudur.

## 12.8 `ContactStatusBreakdownDto`

Ana alanlar:

- `contactStatus`
- `label`
- `count`

MVP'de var mı?:

- Evet

## 12.9 `OutcomeStatusBreakdownDto`

Ana alanlar:

- `outcomeStatus`
- `label`
- `count`

MVP'de var mı?:

- Evet

## 12.10 `ProductBreakdownDto`

Ana alanlar:

- `productCode`
- `productName`
- `salesCount`

MVP'de var mı?:

- Evet

## 12.11 `EmployeePerformanceRowDto`

Ana alanlar:

- `employeeId`
- `employeeName`
- `totalActivityCount`
- `totalSalesCount`
- `conversionRate`
- `totalFinancialProduction`
- `totalExpense`

MVP'de var mı?:

- Evet

## 12.12 `ExecutiveOverviewDto`

Ana alanlar:

- `summaryCards`
- `charts`
- `tables`

MVP'de var mı?:

- Evet

## 13. Import DTO'ları

## 13.1 `ImportUploadRequestDto`

Ana alanlar:

- `importType`
- `file`

MVP'de var mı?:

- Hazırlık aşaması

## 13.2 `ImportBatchListItemDto`

Ana alanlar:

- `id`
- `sourceName`
- `fileName`
- `status`
- `startedAt`
- `completedAt`
- `totalRowCount`
- `successRowCount`
- `failedRowCount`

MVP'de var mı?:

- Future phase yakınında

## 13.3 `ImportRowErrorDto`

Ana alanlar:

- `rowNumber`
- `status`
- `errorMessage`
- `rawPayload`

MVP'de var mı?:

- Future phase yakınında

## 14. Audit DTO'ları

## 14.1 `AuditLogListItemDto`

Ana alanlar:

- `id`
- `entityName`
- `entityId`
- `actionType`
- `changedBy`
- `changedAt`

MVP'de var mı?:

- Future phase yakınında

## 14.2 `AuditLogDetailDto`

Ana alanlar:

- `id`
- `entityName`
- `entityId`
- `actionType`
- `oldValues`
- `newValues`
- `changedBy`
- `changedAt`

MVP'de var mı?:

- Future phase yakınında

## 15. Lookup Stratejisi Notu

Lookup DTO'lar iki şekilde sunulabilir:

- Modül bazlı lookup endpoint'leri
- Merkezi `lookups` endpoint grubu

Bu karar ayrıca ayrı belgede netleştirilebilir. Ancak DTO isimleri merkezi veya modüler stratejiden bağımsız olarak korunabilir.

## 16. MVP İçin Kritik DTO Seti

İlk aşamada mutlaka gerekli DTO'lar:

- Auth DTO'ları
- Employee CRUD DTO'ları
- Account CRUD DTO'ları
- Activity CRUD DTO'ları
- Sale CRUD DTO'ları
- Expense CRUD DTO'ları
- Lookup DTO'ları
- Dashboard summary/trend/performance DTO'ları

## 17. Future Phase DTO'ları

İleri fazda veya ikinci dalgada netleştirilecek DTO'lar:

- Import DTO'ları
- Audit DTO'ları
- Gelişmiş bölge/harita dashboard DTO'ları
- Profitability veya gelişmiş KPI DTO'ları

## 18. Sonuç

Bu katalog ile:

- hangi modülde hangi DTO'ların gerektiği
- hangi DTO'nun hangi amaçla kullanıldığı
- hangi modellerin MVP için kritik olduğu

netleştirilmiş oldu.

Bir sonraki doğal adım, bu katalogdan türeyen `endpoint_to_dto_map.md` veya `lookup_strategy.md` belgesini üretmektir.
