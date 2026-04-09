# Lookup Strategy

## 1. Amac

Bu belge, uygulamada kullanilacak lookup verilerinin nasil modellenmesi, sunulmasi ve tuketilmesi gerektigini netlestirir. Amaç; dropdown, autocomplete, filtre paneli ve referans veri ihtiyaclarini tutarli bir stratejiyle yonetmektir.

Bu projede lookup stratejisi kritik cunku:

- Aktivite, satis ve masraf modulleri cok sayida referans veri kullanir
- Dashboard filtreleri ayni referans veri setlerini tekrar kullanir
- Frontend'in ayni listeyi farkli endpoint'lerden farkli formatta almasi engellenmelidir
- Excel import ve validation tarafinda da ayni referans anahtarlar kullanilacaktir

## 2. Temel Karar

MVP'de lookup endpoint'leri merkezi `/api/lookups/*` yapisi altinda toplanmalidir.

Neden:

- Frontend tarafinda kesif kolaylasir
- Ortak DTO formati korunur
- Referans veriler moduller arasi tekrar edilmez
- Dashboard filtre ekranlari ile form ekranlari ayni kaynagi kullanir

Ancak domain sahipligi backend icinde korunmalidir. Yani public route merkezi olsa da veri sahipligi uygulama katmaninda ilgili module aittir.

## 3. Lookup Kategorileri

Bu projede lookup verileri dort ana gruba ayrilir:

- Sabit sistem lookup'lari
- Yonetilebilir referans lookup'lari
- Arama destekli operasyonel lookup'lar
- Dashboard toplu filtre lookup'lari

### 3.1 Sabit sistem lookup'lari

Kod listesi gibi davranir, nadiren degisir.

Ornekler:

- `product-types`
- `expense-types`
- `activity-contact-statuses`
- `activity-outcome-statuses`

### 3.2 Yonetilebilir referans lookup'lari

Ileri fazda admin tarafindan yonetilebilir.

Ornekler:

- bolge
- sehir
- ilce

### 3.3 Arama destekli operasyonel lookup'lar

Kayit sayisi artis gosterecegi icin liste yerine arama odakli calismalidir.

Ornekler:

- accounts
- employees
- account contacts

### 3.4 Dashboard toplu filtre lookup'lari

Tek ekranda birden fazla filtrenin ilk yuklemede alinmasi gereken durumlarda kullanilir.

Ornek:

- `/api/dashboard/reference-filters`

## 4. DTO Standardi

Temel lookup DTO formati:

```json
{
  "id": "prod-bes",
  "code": "BES",
  "label": "BES",
  "isActive": true,
  "group": null,
  "meta": {}
}
```

Kararlar:

- Tum lookup response'lari en az `id`, `code`, `label` icermelidir
- Gerekliyse `isActive` ve `meta` alanlari eklenebilir
- Frontend, gosterim icin `label` kullanmali; is kurali icin `code` ve `id` degerlerini karistirmamalidir

## 5. Endpoint Stratejisi

### 5.1 Genel lookup endpoint'leri

- `GET /api/lookups/product-types`
- `GET /api/lookups/expense-types`
- `GET /api/lookups/activity-contact-statuses`
- `GET /api/lookups/activity-outcome-statuses`
- `GET /api/lookups/regions`
- `GET /api/lookups/cities`
- `GET /api/lookups/districts`

### 5.2 Arama destekli lookup endpoint'leri

- `GET /api/lookups/accounts?q=...`
- `GET /api/lookups/employees?q=...`
- `GET /api/lookups/account-contacts?accountId=...&q=...`

Karar:

- Kayit hacmi buyuyebilecek lookup'larda tum listeyi donmek yerine arama parametresi desteklenmelidir
- Minimum karakter siniri frontend ve backend tarafinda uygulanabilir

### 5.3 Dashboard toplu lookup endpoint'i

- `GET /api/dashboard/reference-filters`

Bu endpoint su veri gruplarini tek response icinde donebilir:

- employees
- productTypes
- expenseTypes
- activityContactStatuses
- activityOutcomeStatuses
- regions
- cities

## 6. Caching Ve Yenileme Kurali

Lookup verileri iki davranis grubuna ayrilmalidir:

- Neredeyse sabit lookup'lar
- Sik guncellenebilen operasyonel lookup'lar

### Neredeyse sabit lookup'lar

Ornek:

- product types
- expense types
- activity status tipleri

Karar:

- Frontend oturum seviyesinde cache edebilir
- Backend response cache veya memory cache kullanabilir

### Sik guncellenebilen operasyonel lookup'lar

Ornek:

- accounts
- account contacts
- employees

Karar:

- Kisa sureli cache veya hic cache uygulanabilir
- Arama sonucunun guncelligi daha onemlidir

## 7. Yetki Ve Gorunurluk

Lookup endpoint'leri role gore filtrelenmelidir.

Kurallar:

- `FieldSales`, sadece erismesi gereken employee/account kayitlarini gorebilmelidir
- `Manager`, kendi ekip ve yetki alaniyla sinirli kayit gorebilir
- `Admin`, tum lookup verilerine erisebilir
- Dashboard filtre lookup'lari da ayni yetki sinirlarini uygulamalidir

## 8. Veri Kalitesi Kurallari

- `label` bos olamaz
- `code` sistem lookup'larinda sabit ve tekil olmalidir
- Pasif kayitlar default olarak gizlenmeli, gerekirse `includeInactive=true` ile acilmalidir
- Duplicate account lookup riskini azaltmak icin hesap lookup'larinda ikincil aciklayici alanlar donulebilir

Ornek account lookup:

```json
{
  "id": "acc-102",
  "code": "ACC-102",
  "label": "Acme Dis Ticaret A.S.",
  "meta": {
    "accountType": "CORPORATE",
    "city": "Istanbul",
    "taxNumberMasked": "34****789"
  }
}
```

## 9. Frontend Tuketim Kurallari

- Form ekranlari, kendi ihtiyaclari olan lookup'lari parca parca cekmelidir
- Dashboard ilk yuklemede `reference-filters` endpoint'ini kullanabilir
- Buyuk lookup'lar autocomplete ile calismalidir
- Frontend, lookup `label` degerlerini hardcode etmemelidir

## 10. MVP Lookup Seti

MVP'de zorunlu lookup endpoint'leri:

- `/api/lookups/employees`
- `/api/lookups/accounts`
- `/api/lookups/product-types`
- `/api/lookups/expense-types`
- `/api/lookups/activity-contact-statuses`
- `/api/lookups/activity-outcome-statuses`
- `/api/lookups/regions`
- `/api/lookups/cities`
- `/api/lookups/districts`
- `/api/dashboard/reference-filters`

## 11. Future Phase

Ileri fazda eklenebilecekler:

- Admin tarafindan yonetilebilir referans veri ekranlari
- Lookup versiyonlama
- Daha gelismis arama skorlama ve alias eslestirme
- Excel import icin toplu referans eslestirme yardim endpoint'leri

## 12. Sonuc

Bu projede lookup stratejisi:

- public API seviyesinde merkezi
- backend sahipligi seviyesinde moduler
- frontend tuketimi seviyesinde tutarli
- dashboard ve transaction ekranlari arasinda paylasilmis

olmalidir.
