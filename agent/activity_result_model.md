# Activity Result Model

## 1. Amaç

Bu belge, aktivite sonucunun veri modelinde, API sözleşmesinde, form davranışında ve KPI hesaplamalarında nasıl ele alınacağını netleştirmek için hazırlanmıştır. Amaç, şu an açık durumda olan “aktivite sonucu tek alan mı olacak, yoksa iki ayrı alan mı olacak?” sorusuna ürün seviyesinde karar vermek ve bu kararı tüm sistem bileşenlerine yaymaktır.

Bu doküman, özellikle şu alanları etkiler:

- ERD
- Activity create/update request DTO'ları
- Dashboard KPI hesapları
- Filtreleme mantığı
- Raporlama tutarlılığı

## 2. Problem Tanımı

Mevcut iş dilinde aşağıdaki sonuçlar birlikte kullanılıyor:

- Görüşüldü
- Görüşülmedi
- Olumlu
- Olumsuz
- Ertelendi
- Satış Oldu

Bu kavramlar tek seviyeli değil, iki ayrı anlam katmanı içeriyor:

- Temas gerçekleşti mi?
- Görüşmenin ticari sonucu ne oldu?

Bu iki anlamı tek dropdown içinde tutmak MVP için pratik olsa da, veri kalitesi ve KPI yorumunda belirsizlik üretir.

## 3. Nihai Ürün Kararı

### Karar

Sistem uzun vadede iki ayrı alan üzerinden modellenmelidir:

- `contactStatus`
- `outcomeStatus`

Ancak MVP uygulama kararı olarak veri girişini hızlandırmak için aşağıdaki hibrit yaklaşım önerilir:

- Veritabanı ve domain modeli iki ayrı kavrama hazırlanmalı
- İlk sürüm UI tarafında sadeleştirilmiş deneyim kullanılabilir
- Referans sonuç listeleri, bu iki alanı dolduracak şekilde kurallı haritalanmalıdır

Bu sayede hem MVP sade kalır hem de gelecekte büyük refactor ihtiyacı azalır.

## 4. Kavramsal Ayrım

## 4.1 Contact Status

Temasın fiilen kurulup kurulmadığını gösterir.

Önerilen değerler:

- `CONTACTED`
- `NOT_CONTACTED`

Anlamları:

- `CONTACTED`: müşteri/firma ile gerçek görüşme, toplantı veya anlamlı temas gerçekleşti
- `NOT_CONTACTED`: ziyaret edildi ama görüşme olmadı, ulaşılamadı veya temas kurulamadı

## 4.2 Outcome Status

Temas sonrası iş/ticari sonucun ne olduğunu gösterir.

Önerilen değerler:

- `POSITIVE`
- `NEGATIVE`
- `POSTPONED`
- `SALE_CLOSED`
- `NOT_APPLICABLE`

Anlamları:

- `POSITIVE`: olumlu görüşme oldu, takip potansiyeli var
- `NEGATIVE`: olumsuz sonuçlandı
- `POSTPONED`: karar veya işlem ileri tarihe kaldı
- `SALE_CLOSED`: satış gerçekleşti
- `NOT_APPLICABLE`: temas kurulmadığı için sonuç değerlendirmesi yapılamadı

## 5. MVP Modelleme Kararı

### Önerilen MVP yaklaşımı

MVP'de activity entity üzerinde şu iki alan bulunsun:

- `contactStatus`
- `outcomeStatus`

Ancak kullanıcıya ilk sürümde tek sonuç alanı gösterilecekse, seçim backend veya application katmanında aşağıdaki eşleme ile iki alana dönüştürülsün.

## 6. Tek Seçimden Çift Alana Eşleme Tablosu

Önerilen eşleme:

| UI Seçimi | contactStatus | outcomeStatus |
|---|---|---|
| Görüşüldü | CONTACTED | NOT_APPLICABLE |
| Görüşülmedi | NOT_CONTACTED | NOT_APPLICABLE |
| Olumlu | CONTACTED | POSITIVE |
| Olumsuz | CONTACTED | NEGATIVE |
| Ertelendi | CONTACTED | POSTPONED |
| Satış Oldu | CONTACTED | SALE_CLOSED |

Bu eşleme sayesinde:

- `Görüşülmedi` ile `Olumsuz` birbirinden net ayrılır
- `Görüşüldü` tek başına temas bilgisini taşır
- `Olumlu/Olumsuz/Ertelendi/Satış Oldu` temas gerçekleşmiş durumlarda sonuç bilgisi sağlar

## 7. Önerilen Veritabanı Tasarımı

## 7.1 Ayrı Referans Tabloları

Uzun vadeli öneri:

- `activity_contact_status_types`
- `activity_outcome_status_types`

Örnek alanlar:

- `id`
- `code`
- `name`
- `display_order`
- `is_active`

## 7.2 MVP Basitleştirme Seçeneği

Eğer ilk aşamada iki ayrı referans tablo fazla bulunursa:

- referans tabloları yerine enum-benzeri kod sabitleri ile başlanabilir
- ama DTO ve entity alanları yine ayrı tutulmalıdır

Bu yaklaşım, ileride tabloya geçmeyi kolaylaştırır.

## 8. Activity Entity Etkisi

`activities` tablosu için önerilen alan revizyonu:

- `contact_status_code`
- `outcome_status_code`

Opsiyonel destek alanı:

- `legacy_result_type_id`

Not:

- `legacy_result_type_id` yalnızca geçiş amaçlı düşünülmelidir
- Nihai sistemde iki alan yaklaşımı daha temizdir

## 9. Validation Kuralları

### Temel kurallar

- `contactStatus` zorunlu olmalıdır
- `outcomeStatus` yalnızca `contactStatus = CONTACTED` ise anlamlıdır
- `contactStatus = NOT_CONTACTED` ise `outcomeStatus` otomatik olarak `NOT_APPLICABLE` olmalıdır
- `outcomeStatus = SALE_CLOSED` ise satış oluşturma veya satış ilişkisi iş kuralı kontrol edilmelidir

### Örnek validation kararları

- `NOT_CONTACTED + POSITIVE` geçersizdir
- `NOT_CONTACTED + NEGATIVE` geçersizdir
- `NOT_CONTACTED + SALE_CLOSED` geçersizdir
- `CONTACTED + NOT_APPLICABLE` geçerlidir ama yalnızca “Görüşüldü” tipi nötr kayıtlar için kullanılmalıdır

## 10. Form Davranışı Kararı

## 10.1 MVP Form Seçeneği

Kullanıcı deneyimi açısından ilk sürümde iki seçenek vardır:

### Seçenek A - Tek dropdown

Artıları:

- Veri girişi hızlıdır
- Saha personeli için daha pratiktir

Eksileri:

- Kullanıcı iki ayrı kavramı tek seçimde fark etmez
- Daha sonra filtreleme ve rapor açıklaması gerekir

### Seçenek B - İki aşamalı seçim

Önce:

- Temas durumu

Sonra temas olduysa:

- Sonuç durumu

Artıları:

- Veri kalitesi daha yüksektir
- KPI hesapları daha temiz olur

Eksileri:

- İlk form biraz daha uzun olur

### Önerilen ürün kararı

MVP için dengeli çözüm:

- Formda önce `Temas Durumu` gösterilsin
- Eğer `Temas Durumu = Görüşüldü` ise ikinci alan olan `Sonuç Durumu` açılsın

Bu çözüm, hem veri kalitesini artırır hem de kullanıcıyı gereksiz alanla yormaz.

## 11. API Contract Etkisi

Activity create/update request'lerinde önerilen alanlar:

```json
{
  "employeeId": "emp-001",
  "accountId": "acc-018",
  "activityDate": "2026-01-14",
  "content": "Kurumsal BES görüşmesi yapıldı.",
  "contactStatus": "CONTACTED",
  "outcomeStatus": "POSITIVE"
}
```

Eğer MVP geçiş sürecinde tek seçimli UI kullanılırsa backend'e şu yapı da kabul edilebilir:

```json
{
  "employeeId": "emp-001",
  "accountId": "acc-018",
  "activityDate": "2026-01-14",
  "content": "Kurumsal BES görüşmesi yapıldı.",
  "legacyResultSelection": "POSITIVE"
}
```

Ürün kararı:

- Dış API contract mümkünse doğrudan `contactStatus` ve `outcomeStatus` kullanmalıdır
- `legacyResultSelection` yalnızca geçici uyumluluk alanı olmalıdır

## 12. Dashboard ve KPI Etkisi

Bu model KPI tarafında şu netliği sağlar:

### Temas KPI'ları

- Görüşülen aktivite sayısı:
  - `contactStatus = CONTACTED`
- Görüşülmeyen aktivite sayısı:
  - `contactStatus = NOT_CONTACTED`

### Sonuç KPI'ları

- Olumlu sonuçlanan aktivite:
  - `outcomeStatus = POSITIVE`
- Olumsuz sonuçlanan aktivite:
  - `outcomeStatus = NEGATIVE`
- Ertelenen aktivite:
  - `outcomeStatus = POSTPONED`
- Satış olan aktivite:
  - `outcomeStatus = SALE_CLOSED`

### Dönüşüm KPI'ları

- Aktiviteden satışa dönüşüm:
  - tercihen `linked sale exists`
  - yalnızca `SALE_CLOSED` etiketine bağlı kalmamalıdır

Önemli karar:

- `SALE_CLOSED` tek başına satış varlığının yerine geçmez
- gerçek satış için satış kaydı esas alınmalıdır

## 13. Filtreleme Etkisi

Dashboard ve liste ekranlarında filtreler iki seviyeli olmalıdır:

- `contactStatus`
- `outcomeStatus`

UI tarafında gerektiğinde birleşik filtre de sunulabilir:

- “Görüşüldü”
- “Görüşülmedi”
- “Olumlu”
- “Olumsuz”
- “Ertelendi”
- “Satış Oldu”

Ama backend sorgu mantığı iki ayrı alan üzerinden çalışmalıdır.

## 14. Raporlama Yorum Kuralları

- `Görüşüldü` ile `Olumlu` aynı kategori değildir
- `Olumlu` kayıtlar satış değildir
- `Satış Oldu` sonucu, satış kaydı ile teyit edilmelidir
- `Görüşülmedi` kayıtları sonuç analizine dahil edilmemelidir
- Sonuç dağılım grafikleri yalnızca `CONTACTED` aktiviteler üzerinde de gösterilebilir

## 15. Geçiş Stratejisi

Eğer sistem ilk aşamada tek `activity_result_type` ile başlarsa geçiş şu sırayla yapılmalıdır:

1. Eski result type kodları için mapping tablosu hazırlanır
2. `contactStatus` ve `outcomeStatus` alanları eklenir
3. Eski kayıtlar mapping ile dönüştürülür
4. KPI sorguları yeni alanlara taşınır
5. Eski alan kullanım dışı bırakılır

## 16. Önerilen Nihai Referans Kodlar

### Contact status codes

- `CONTACTED`
- `NOT_CONTACTED`

### Outcome status codes

- `NOT_APPLICABLE`
- `POSITIVE`
- `NEGATIVE`
- `POSTPONED`
- `SALE_CLOSED`

## 17. Karar Özeti

Bu belgeye göre alınan net kararlar:

- Aktivite sonucu uzun vadede iki ayrı kavramdır
- Domain modeli iki alanlı kurulmalıdır
- KPI hesapları iki alan mantığına göre yapılmalıdır
- Satış varlığı için `sales` tablosu esas alınmalıdır
- MVP form deneyimi şartlı ikinci alan yaklaşımıyla sade tutulabilir

## 18. ERD Güncelleme Notu

`erd.md` içinde geçen `activity_result_types` yapısı, bu belgeye göre gelecekte şu şekilde revize edilmelidir:

- `activity_contact_status_types`
- `activity_outcome_status_types`

ve `activities` tablosu:

- `contact_status_code`
- `outcome_status_code`

alanları ile güncellenmelidir.

## 19. Sonuç

Bu yaklaşım, aktivite sonucu ile ilgili mevcut belirsizliği azaltır ve şu üç alanı aynı anda güçlendirir:

- veri kalitesi
- KPI doğruluğu
- API/DTO netliği

Ürün, MVP'de sade kalırken ileri faz için doğru yapısal temeli korumuş olur.
