# API Contracts

## 1. Amaç

Bu belge, proje genelinde kullanılacak API sözleşme standartlarını tanımlar. Amaç; backend ve frontend arasında modülden bağımsız ortak bir dil kurmak, response yapılarının ve hata davranışlarının tutarlı olmasını sağlamak, yeni endpoint geliştirirken her ekip üyesinin aynı temel kurallara göre ilerlemesini mümkün kılmaktır.

Bu doküman:

- Tüm modüller için genel API standardını belirler
- `dashboard_api_plan.md` ve `dashboard_response_examples.md` ile uyumlu çalışır
- CRUD, lookup, dashboard ve yönetim endpoint'leri için ortak çerçeve sunar

## 2. Temel API Tasarım İlkeleri

- API sözleşmeleri entity'leri doğrudan dışarı sızdırmamalıdır
- DTO kullanımı zorunlu olmalıdır
- Başarılı ve başarısız cevaplar tutarlı formatta dönmelidir
- Liste endpoint'lerinde sayfalama, sıralama ve filtreleme standardize edilmelidir
- Validation hataları alan bazlı dönmelidir
- Auth ve yetki ihlalleri net HTTP statüleriyle raporlanmalıdır
- API sözleşmesi frontend'in ihtiyacına göre kullanılabilir ama backend'i UI'ya aşırı bağımlı hale getirmemelidir
- Dashboard endpoint'leri transaction CRUD endpoint'lerinden ayrı tutulmalıdır

## 3. API Kök Yapısı

Önerilen temel route formatı:

```text
/api/{module}
/api/{module}/{id}
/api/{module}/{action}
```

Örnek modüller:

- `/api/auth`
- `/api/users`
- `/api/roles`
- `/api/employees`
- `/api/accounts`
- `/api/activities`
- `/api/sales`
- `/api/expenses`
- `/api/dashboard`
- `/api/imports`
- `/api/audit-logs`

## 4. İsimlendirme Kuralları

### Route isimlendirme

- Route segmentleri `kebab-case` olmalıdır
- Modül adları çoğul kullanılmalıdır
- Action bazlı özel endpoint'ler gerektiğinde açık isim verilmelidir

Örnekler:

- `/api/employees`
- `/api/accounts`
- `/api/activities`
- `/api/dashboard/executive-overview`
- `/api/imports/validate`

### JSON alan isimlendirme

- Request ve response alanları `camelCase` olmalıdır

Örnek:

```json
{
  "employeeId": "emp-001",
  "activityDate": "2026-01-10",
  "resultTypeId": "art-01"
}
```

## 5. HTTP Method Kuralları

- `GET`: veri okuma
- `POST`: yeni kayıt oluşturma veya kompleks sorgu/filter request
- `PUT`: tam güncelleme gerekiyorsa kullanılabilir
- `PATCH`: kısmi güncelleme
- `DELETE`: silme veya soft delete tetikleme

Ürün kararı:

- Basit listeleme için `GET`
- Karmaşık filtreli dashboard veya rapor sorguları için `POST`
- Form tabanlı kayıt oluşturma için `POST`
- Kısmi güncelleme yaklaşımı daha esnek olduğu için modül güncellemelerinde öncelik `PATCH`

## 6. HTTP Status Kodları

Önerilen kullanım:

- `200 OK`: başarılı okuma veya güncelleme
- `201 Created`: başarılı oluşturma
- `204 No Content`: içeriksiz başarılı silme/işlem sonucu
- `400 Bad Request`: format veya işlenemeyen request
- `401 Unauthorized`: kimlik doğrulama yok
- `403 Forbidden`: yetki yok
- `404 Not Found`: kayıt bulunamadı
- `409 Conflict`: duplicate veya iş kuralı çakışması
- `422 Unprocessable Entity`: validation hataları
- `500 Internal Server Error`: beklenmeyen sunucu hatası

## 7. Ortak Response Zarfı

Tüm endpoint'lerde ortak response yapısı kullanılmalıdır.

### Başarılı response

```json
{
  "success": true,
  "data": {},
  "meta": {},
  "errors": []
}
```

### Başarısız response

```json
{
  "success": false,
  "data": null,
  "meta": {},
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Validation failed.",
      "field": "activityDate"
    }
  ]
}
```

### Alan açıklamaları

- `success`: işlemin başarılı olup olmadığını gösterir
- `data`: endpoint'in asıl çıktısı
- `meta`: sayfalama, tarih yorumu, uyarılar, işlem zamanı gibi ek bilgiler
- `errors`: hata listesi

## 8. Hata Modeli

Önerilen ortak hata nesnesi:

```json
{
  "code": "DUPLICATE_ACCOUNT",
  "message": "Aynı vergi numarasına sahip başka bir hesap kaydı bulunmaktadır.",
  "field": "taxNumber",
  "details": null
}
```

### Hata alanları

- `code`: sabit ve makine tarafından işlenebilir hata kodu
- `message`: kullanıcıya veya log'a uygun açıklama
- `field`: alan bazlı hata varsa ilgili alan
- `details`: opsiyonel teknik ek bilgi

### Hata kategorileri

- Validation errors
- Business rule errors
- Authorization errors
- Not found errors
- Conflict errors
- System errors

## 9. Validation Hatası Standardı

Validation hataları mümkün olduğunca alan bazlı döndürülmelidir.

Örnek:

```json
{
  "success": false,
  "data": null,
  "meta": {},
  "errors": [
    {
      "code": "REQUIRED",
      "message": "Aktivite tarihi zorunludur.",
      "field": "activityDate"
    },
    {
      "code": "INVALID_DATE_RANGE",
      "message": "Bitiş tarihi başlangıç tarihinden küçük olamaz.",
      "field": "dateRange.endDate"
    }
  ]
}
```

## 10. Listeleme Sözleşmesi

Tüm liste endpoint'leri ortak sayfalama ve sıralama modeli kullanmalıdır.

### Örnek request query parametreleri

```text
GET /api/activities?page=1&pageSize=20&sortBy=activityDate&sortDirection=desc
```

### Örnek liste response

```json
{
  "success": true,
  "data": {
    "items": [],
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalCount": 286,
      "totalPages": 15
    }
  },
  "meta": {},
  "errors": []
}
```

### Pagination kuralları

- `page` varsayılanı `1`
- `pageSize` varsayılanı `20`
- Maksimum `pageSize` sınırı tanımlanmalıdır, öneri `100`

## 11. Filtreleme Sözleşmesi

CRUD liste endpoint'lerinde iki yaklaşım olabilir:

- Basit filtreler için query string
- Karmaşık filtreler için `POST /search` veya modül bazlı filtre endpoint'i

Öneri:

- Basit liste filtreleri query string ile çözülsün
- Dashboard ve kompleks raporlar body tabanlı filtre contract kullansın

### Basit filtre örneği

```text
GET /api/sales?employeeId=emp-001&productTypeId=prod-bes&startDate=2026-01-01&endDate=2026-01-31
```

### Kompleks filtre örneği

```text
POST /api/dashboard/sales
```

```json
{
  "dateRange": {
    "startDate": "2026-01-01",
    "endDate": "2026-01-31"
  },
  "employeeIds": ["emp-001"],
  "productTypeIds": ["prod-bes"]
}
```

## 12. Sıralama Sözleşmesi

Tüm liste endpoint'lerinde sıralama parametreleri aynı adlarla kullanılmalıdır:

- `sortBy`
- `sortDirection`

Örnek değerler:

- `sortBy=createdAt`
- `sortDirection=asc`
- `sortDirection=desc`

## 13. Kaynak Bazlı CRUD Endpoint Örnekleri

## 13.1 Employees

- `GET /api/employees`
- `GET /api/employees/{id}`
- `POST /api/employees`
- `PATCH /api/employees/{id}`
- `DELETE /api/employees/{id}`

## 13.2 Accounts

- `GET /api/accounts`
- `GET /api/accounts/{id}`
- `POST /api/accounts`
- `PATCH /api/accounts/{id}`
- `DELETE /api/accounts/{id}`

## 13.3 Activities

- `GET /api/activities`
- `GET /api/activities/{id}`
- `POST /api/activities`
- `PATCH /api/activities/{id}`
- `DELETE /api/activities/{id}`

## 13.4 Sales

- `GET /api/sales`
- `GET /api/sales/{id}`
- `POST /api/sales`
- `PATCH /api/sales/{id}`
- `DELETE /api/sales/{id}`

## 13.5 Expenses

- `GET /api/expenses`
- `GET /api/expenses/{id}`
- `POST /api/expenses`
- `PATCH /api/expenses/{id}`
- `DELETE /api/expenses/{id}`

## 14. Lookup Endpoint Standartları

Referans veriler için hafif response yapıları kullanılmalıdır.

Örnek endpoint'ler:

- `GET /api/lookups/product-types`
- `GET /api/lookups/activity-result-types`
- `GET /api/lookups/expense-types`

Örnek response:

```json
{
  "success": true,
  "data": [
    {
      "id": "prod-bes",
      "code": "BES",
      "label": "BES"
    }
  ],
  "meta": {},
  "errors": []
}
```

## 15. Create Contract Kuralları

### Örnek `POST /api/activities`

Request:

```json
{
  "employeeId": "emp-001",
  "accountId": "acc-018",
  "accountContactId": null,
  "activityDate": "2026-01-14",
  "visitedCity": "Istanbul",
  "visitedLocation": "Kadikoy",
  "content": "Kurumsal BES görüşmesi yapıldı.",
  "resultTypeId": "art-03",
  "followUpDate": "2026-01-20",
  "contactPersonName": "Murat Aydın",
  "notesSummary": "Teklif bekleniyor."
}
```

Başarılı response:

```json
{
  "success": true,
  "data": {
    "id": "act-101",
    "employeeId": "emp-001",
    "accountId": "acc-018",
    "activityDate": "2026-01-14",
    "resultTypeId": "art-03",
    "createdAt": "2026-01-14T09:15:00Z"
  },
  "meta": {},
  "errors": []
}
```

## 16. Update Contract Kuralları

### Örnek `PATCH /api/sales/{id}`

Request:

```json
{
  "activityId": "act-101",
  "collectionAmount": 18500.00,
  "apeAmount": 6200.00,
  "description": "Eksik finansal alanlar güncellendi."
}
```

Başarılı response:

```json
{
  "success": true,
  "data": {
    "id": "sal-087",
    "updatedAt": "2026-01-15T10:40:00Z"
  },
  "meta": {},
  "errors": []
}
```

### Update kuralı

- PATCH yalnızca gönderilen alanları günceller
- Null gönderimi alanı temizleme anlamına geliyorsa bu davranış açık tanımlanmalıdır

## 17. Delete Contract Kuralları

Soft delete uygulanan tablolar için silme sonrası response sade olmalıdır.

Örnek:

```json
{
  "success": true,
  "data": {
    "id": "exp-044",
    "deleted": true
  },
  "meta": {},
  "errors": []
}
```

## 18. Auth Contract

### Login endpoint

- `POST /api/auth/login`

Örnek request:

```json
{
  "username": "manager01",
  "password": "******"
}
```

Örnek response:

```json
{
  "success": true,
  "data": {
    "accessToken": "jwt-token",
    "expiresAt": "2026-04-04T13:00:00Z",
    "user": {
      "id": "usr-001",
      "username": "manager01",
      "displayName": "Satış Müdürü",
      "roles": ["Manager"]
    }
  },
  "meta": {},
  "errors": []
}
```

### Auth header standardı

```text
Authorization: Bearer {token}
```

## 19. Yetki Hatası Örneği

```json
{
  "success": false,
  "data": null,
  "meta": {},
  "errors": [
    {
      "code": "FORBIDDEN",
      "message": "Bu kaynağa erişim yetkiniz bulunmamaktadır.",
      "field": null
    }
  ]
}
```

## 20. Audit ve Meta Alanları

Detay endpoint'lerinde ihtiyaç halinde aşağıdaki alanlar gösterilebilir:

- `createdAt`
- `createdBy`
- `updatedAt`
- `updatedBy`

Ancak bu alanlar her listede zorunlu dönmemelidir. Liste payload'ı gereksiz şişirilmemelidir.

## 21. Tarih ve Para Formatı Kuralları

- Tarih alanları ISO 8601 uyumlu dönmelidir
- Sadece tarih gereken yerlerde `YYYY-MM-DD`
- Tarih-saat gereken yerlerde UTC ISO string
- Para alanları numeric dönmelidir, string dönülmemelidir
- Para formatlaması frontend sorumluluğunda olmalıdır

## 22. Null ve Boş Değer Davranışı

- Liste alanları boşsa `[]`
- Tek nesne yoksa `null`
- Sayısal toplam alanları hesaplanabiliyor ama değer yoksa `0`
- Oran alanları tanımsız ise `null`

## 23. Idempotency ve Conflict Notları

- Duplicate oluşturma riskinin olduğu alanlarda `409 Conflict` kullanılmalıdır
- Özellikle account, import ve bazı sales kayıtlarında tekrar kayıt kontrolü düşünülmelidir
- Import işlemleri için ileride idempotency key yaklaşımı eklenebilir

## 24. Dashboard ile Genel API İlişkisi

- Dashboard response standardı, genel response zarfının özel bir uygulamasıdır
- Dashboard endpoint'leri `meta.dateInterpretation` ve `meta.warnings` alanlarını aktif kullanır
- CRUD endpoint'leri ise daha sade `meta` ile dönebilir

## 25. Versiyonlama Notu

MVP aşamasında ayrı URL versiyonlaması zorunlu değildir. Ancak ileride ihtiyaç olursa şu yaklaşım kullanılabilir:

- `/api/v1/...`

MVP kararı:

- İlk aşamada URL versiyonlaması olmadan başlanabilir
- Kırıcı değişiklik ihtiyacında versiyonlama devreye alınmalıdır

## 26. Açık Kararlar

- Ortak lookup endpoint'leri ayrı `lookups` modülü altında mı olacak, yoksa modül bazlı mı dağılacak?
- PATCH request'lerinde `null` alan temizleme davranışı global mi, modül bazlı mı tanımlanacak?
- Liste endpoint'lerinde tüm modüller için aynı `pagination` nesnesi mi kullanılacak?
- Concurrency kontrolü için `rowVersion` benzeri alan gerekecek mi?

## 27. Sonuç

Bu belge ile proje için:

- ortak response zarfı
- hata modeli
- validation standardı
- listeleme, filtreleme ve sıralama sözleşmesi
- CRUD endpoint yaklaşımı
- auth contract
- tarih ve para formatı kuralları

tek bir referans altında toplanmış oldu.
