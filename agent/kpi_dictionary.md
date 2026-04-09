# KPI Dictionary

## 1. Kısa Yaklaşım Özeti

Bu proje için KPI sözlüğü, dashboard ve rapor ekranlarının tek doğru referansı olarak kullanılacaktır. Amaç yalnızca metrik isimleri üretmek değil; her KPI için aynı hesaplama mantığını, aynı veri kaynağını, aynı zaman yorumunu ve aynı gösterim biçimini standardize etmektir.

Bu dokümanda alınan temel kararlar:

- KPI'lar yalnızca ham işlem kayıtlarından üretilecektir.
- Aktivite, satış ve masraf verileri ayrı transaction alanları olarak raporlanacaktır.
- Satışın aktiviteye bağlanması MVP'de opsiyonel olacak, ancak raporlamada güçlü kalite göstergesi olarak izlenecektir.
- Aktivite sonucunda `Görüşüldü`, `Görüşülmedi`, `Olumlu`, `Olumsuz`, `Ertelendi`, `Satış Oldu` tek referans kümede tutulacak; ancak raporlamada bunlar farklı analitik kümelerde yorumlanacaktır.
- MVP'de yalnızca güvenilir, ham veriden doğrudan üretilebilen KPI'lar gösterilecektir.
- Daha yorumsal veya daha ileri veri kalitesi gerektiren KPI'lar future phase'e bırakılacaktır.

## 2. KPI Yaklaşımı

### KPI nedir?

KPI, operasyonun performansını takip etmek için tanımlanmış standart ve karar destek odaklı ölçümdür. Her KPI'nın açık bir tanımı, veri kaynağı, formülü ve kullanım amacı olmalıdır.

### Bu projede neden kritik?

Bu sistemin asıl değeri yalnızca veri girişi yapmak değil, saha hareketini yönetsel karara dönüştürmektir. KPI sözlüğü olmazsa:

- Aynı metrik farklı ekranlarda farklı hesaplanabilir
- Yöneticiler aynı kavramı farklı yorumlayabilir
- API ile dashboard sayıları tutarsızlaşabilir
- Excel ile sistem karşılaştırmaları anlamsız hale gelebilir

### Dashboard ve raporların neden KPI sözlüğüne bağlı olması gerekir?

- Tekil doğruluk sağlar
- Geri izlenebilirlik sağlar
- API tasarımını standartlaştırır
- Veri modelini ürün ihtiyacına bağlar
- Sonradan çıkan “bu sayı neden farklı?” tartışmalarını azaltır

### MVP'de olacak KPI'lar

MVP KPI'ları şu özellikleri taşımalıdır:

- Ham veriden doğrudan üretilebilmeli
- Operasyon için hemen anlam üretmeli
- Düşük yorum farkı içermeli
- Veri kalitesi zayıf olduğunda tamamen bozulmamalı

### İleri faza bırakılacak KPI'lar

Future phase KPI'ları şu tür ölçümlerdir:

- Daha derin veri kalitesi gerektirenler
- Hedef/kota veya kârlılık yorumu içerenler
- Bölgesel ve segment bazlı ileri analitikler
- Tahsilat, maliyet ve üretim ilişkisini daha derin model isteyenler

## 3. KPI Kategorileri

- Aktivite KPI'ları
- Satış KPI'ları
- Finansal KPI'lar
- Masraf KPI'ları
- Verimlilik KPI'ları
- Dönüşüm KPI'ları
- Personel performans KPI'ları
- Bölge / ilçe KPI'ları
- Zaman trend KPI'ları

## 4. KPI Standardizasyon Kararları

### 4.1 “Olumlu” ile “Satış Oldu” ayrı statüler mi?

Evet. Ürün kararı olarak bunlar ayrı statülerdir.

- `Olumlu`: görüşme veya temas olumlu ilerledi, ancak satış aynı kayıt anında kesinleşmemiş olabilir
- `Satış Oldu`: aktivite doğrudan satışa dönüştü veya satış kapanışı teyit edildi

Bu ayrım sayesinde hem potansiyel hem gerçekleşen dönüşüm ayrı izlenir.

### 4.2 “Görüşüldü” sonucu ile “Olumlu/Olumsuz” sonucu nasıl modellenmeli?

Ürün kararı:

- `Görüşüldü` ve `Görüşülmedi` temasın gerçekleşip gerçekleşmediğini gösteren operasyonel sonuçlardır
- `Olumlu`, `Olumsuz`, `Ertelendi`, `Satış Oldu` ise görüşme kalitesini veya ticari sonucu ifade eder

MVP'de bunlar tek `activity_result_type` listesinde tutulabilir. Ancak raporlamada iki mantıksal kümeye ayrılır:

- Temas durumu kümesi: `Görüşüldü`, `Görüşülmedi`
- Sonuç durumu kümesi: `Olumlu`, `Olumsuz`, `Ertelendi`, `Satış Oldu`

İleri fazda bu ayrım iki ayrı alan olarak modellenebilir:

- `contact_status`
- `outcome_status`

### 4.3 Satışın aktiviteye bağlanması zorunlu mu, opsiyonel mi?

MVP ürünü için opsiyoneldir; ürün politikası olarak güçlü şekilde teşvik edilir.

- Saha akışında oluşan satışlar mümkün olduğunca aktiviteye bağlanmalıdır
- Operasyon tarafından sonradan girilen veya eşleştirilemeyen satışlar bağımsız kaydedilebilir
- Bu durum ayrıca KPI ile izlenir: `Aktiviteye Bağlanamayan Satış Sayısı`

### 4.4 BES finansal alanlarında hangi metrikler zorunlu tutulmalı?

Ürün kararı:

- BES satışında en az bir finansal üretim alanı zorunlu olmalıdır
- Aşağıdaki alanlar ürün tipine göre desteklenmelidir:
  - `collection_amount`
  - `ape_amount`
  - `lump_sum_amount`
  - `monthly_payment_amount`

MVP doğrulama kararı:

- BES için `collection_amount` veya `ape_amount` alanlarından en az biri dolu olmalı
- `lump_sum_amount` ve `monthly_payment_amount` opsiyonel ama desteklenmeli

### 4.5 Masraf verimliliği KPI'ları MVP'ye dahil edilmeli mi?

Kısmen dahil edilmelidir.

MVP'ye alınacaklar:

- Toplam masraf
- Personel bazlı masraf
- Masraf türüne göre toplam
- Satış başına masraf

Future phase'e bırakılacaklar:

- Masraf / tahsilat oranı
- Kişi başı kârlılık benzeri yorumlayıcı KPI'lar
- Bölge bazlı ileri masraf verimliliği

Gerekçe:

- İlk grup doğrudan hesaplanabilir
- İkinci grup daha güçlü finansal model ve veri kalitesi ister

### 4.6 Dashboard'da hangi KPI'lar kart, hangileri grafik, hangileri tablo olmalı?

Kart olarak gösterilecekler:

- Toplam aktivite
- Toplam satış adedi
- Toplam masraf
- Toplam BES tahsilatı
- Aktiviteden satışa dönüşüm oranı
- Aktiviteye bağlanamayan satış sayısı

Grafik olarak gösterilecekler:

- Günlük/haftalık/aylık aktivite trendi
- Günlük/haftalık/aylık satış trendi
- Günlük/haftalık/aylık finansal üretim
- Ürün bazlı satış dağılımı
- Masraf türüne göre dağılım
- Bölge bazlı aktivite veya satış dağılımı

Tablo olarak gösterilecekler:

- Personel performans karşılaştırması
- Bölge bazlı özet tablo
- Ürün bazlı finansal özet
- Aktiviteye bağlı / bağımsız satış analizi

## 5. Ayrıntılı KPI Sözlüğü

## 5.1 Aktivite KPI'ları

### KPI Adı
Toplam Aktivite Sayısı

- Açıklama: Seçilen filtre bağlamında oluşturulmuş toplam aktivite kaydı sayısıdır.
- İş amacı: Saha ekibinin temas hacmini ölçmek.
- Hesaplama formülü: `COUNT(activities.id)`
- Veri kaynağı: `activities`
- Gerekli alanlar: `id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, hesap/müşteri, bölge, ilçe, aktivite sonucu
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: soft delete kayıtları hariç tutulmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Üst özet kart + trend grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations; FieldSales kendi verisi için

### KPI Adı
Görüşülen Aktivite Sayısı

- Açıklama: Sonucu `Görüşüldü` olan aktivite sayısıdır.
- İş amacı: Fiilen temas kurulabilen ziyaret hacmini görmek.
- Hesaplama formülü: `COUNT(activities.id WHERE result_type = 'Interviewed')`
- Veri kaynağı: `activities`, `activity_result_types`
- Gerekli alanlar: `id`, `result_type_id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, hesap/müşteri
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: `Olumlu` veya `Olumsuz` ile çakışan veri giriş mantığı varsa raporlamada standardizasyon uygulanmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + stacked trend
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations; FieldSales kendi verisi için

### KPI Adı
Görüşülmeyen Aktivite Sayısı

- Açıklama: Sonucu `Görüşülmedi` olan aktivite sayısıdır.
- İş amacı: Boşa giden ziyaret veya ulaşılamayan temas oranını izlemek.
- Hesaplama formülü: `COUNT(activities.id WHERE result_type = 'NotInterviewed')`
- Veri kaynağı: `activities`, `activity_result_types`
- Gerekli alanlar: `id`, `result_type_id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, hesap/müşteri
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Planlı ama gerçekleşmeyen ziyaretler ile gerçek saha gidilen ama görüşülemeyen kayıtlar ileride ayrıştırılabilir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + stacked trend
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Olumlu Sonuçlanan Aktivite Sayısı

- Açıklama: Sonucu `Olumlu` olan aktivite sayısıdır.
- İş amacı: Satış öncesi olumlu potansiyel üretimini izlemek.
- Hesaplama formülü: `COUNT(activities.id WHERE result_type = 'Positive')`
- Veri kaynağı: `activities`, `activity_result_types`
- Gerekli alanlar: `id`, `result_type_id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, hesap/müşteri
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: `Satış Oldu` bu KPI'ya dahil edilmez; ayrı izlenir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + sonuç dağılım grafiği
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Olumsuz Sonuçlanan Aktivite Sayısı

- Açıklama: Sonucu `Olumsuz` olan aktivite sayısıdır.
- İş amacı: Kaybedilen fırsatları ve düşük verimli saha temaslarını izlemek.
- Hesaplama formülü: `COUNT(activities.id WHERE result_type = 'Negative')`
- Veri kaynağı: `activities`, `activity_result_types`
- Gerekli alanlar: `id`, `result_type_id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, hesap/müşteri
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Satışa dönüşmeyen tüm aktiviteler olumsuz değildir; `Ertelendi` ayrıca izlenmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Sonuç dağılım grafiği + detay tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Personel Başına Aktivite

- Açıklama: Seçilen dönem içindeki toplam aktivitenin aktif personel sayısına veya filtrelenen personel kümesine bölünmesiyle hesaplanır.
- İş amacı: Saha temposunu kişi başında görmek.
- Hesaplama formülü: `COUNT(activities.id) / COUNT(DISTINCT activities.employee_id)`
- Veri kaynağı: `activities`, `employees`
- Gerekli alanlar: `id`, `employee_id`, `activity_date`
- Filtrelenebilir boyutlar: tarih, bölge, personel grubu
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Paydada yalnızca seçilen filtreye dahil ve aktivite üreten personel kullanılmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Özet kart + personel tablo sütunu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Günlük / Haftalık / Aylık Aktivite

- Açıklama: Seçilen zaman kırılımında aktivite sayısının dağılımıdır.
- İş amacı: Zaman içindeki saha yoğunluğunu izlemek.
- Hesaplama formülü: `COUNT(activities.id) GROUP BY time_bucket(activity_date)`
- Veri kaynağı: `activities`
- Gerekli alanlar: `id`, `activity_date`
- Filtrelenebilir boyutlar: personel, bölge, sonuç türü
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Aynı sayfa içinde tek bir tarih mantığı kullanılmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Çizgi veya kolon grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations; FieldSales kendi verisi için

### KPI Adı
Bölge Bazlı Aktivite

- Açıklama: Aktivite sayısının bölge veya şehir düzeyinde dağılımıdır.
- İş amacı: Saha yükünü coğrafi olarak görmek.
- Hesaplama formülü: `COUNT(activities.id) GROUP BY visited_city or employee.region`
- Veri kaynağı: `activities`, `employees`, `accounts`
- Gerekli alanlar: `id`, `visited_city` veya `employee.region`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, ilçe, sonuç türü
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Bölge alanının kaynağı net standartlaştırılmalıdır; çalışan bölgesi ile ziyaret bölgesi karıştırılmamalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Bar chart + bölge özet tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.2 Satış KPI'ları

### KPI Adı
Toplam Satış Adedi

- Açıklama: Seçilen bağlamda oluşan toplam satış kayıt adedidir.
- İş amacı: Genel satış hacmini izlemek.
- Hesaplama formülü: `SUM(sales.sale_count)`; `sale_count` kullanılmıyorsa `COUNT(sales.id)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `id`, `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge, hesap/müşteri, aktiviteye bağlılık
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: MVP'de `sale_count` varsayılan 1 ise kayıt sayısı ile karışabilir; tutarlılık korunmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Özet kart + trend grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations; FieldSales kendi verisi için

### KPI Adı
Ürün Bazlı Satış Adedi

- Açıklama: Satışların ürün tipine göre dağılımıdır.
- İş amacı: Hangi ürünlerin üretildiğini görmek.
- Hesaplama formülü: `SUM(sales.sale_count) GROUP BY product_type_id`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Ürün kodları değişirse tarihsel karşılaştırma etkilenmemelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Donut/bar chart + tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Personel Bazlı Satış Adedi

- Açıklama: Personel bazında üretilen satış adedidir.
- İş amacı: Çalışan performansını satış adedi perspektifinden karşılaştırmak.
- Hesaplama formülü: `SUM(sales.sale_count) GROUP BY employee_id`
- Veri kaynağı: `sales`, `employees`
- Gerekli alanlar: `employee_id`, `sale_count`, `sale_date`
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Sadece adet görmek yanıltıcı olabilir; finansal KPI ile birlikte yorumlanmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Sıralı bar chart + personel tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Tarih Bazlı Satış Sayısı

- Açıklama: Satış sayısının zaman içindeki dağılımıdır.
- İş amacı: Satış trendini izlemek.
- Hesaplama formülü: `SUM(sales.sale_count) GROUP BY time_bucket(sale_date)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: personel, ürün, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Satış tarihi ile tahsilat tarihi karıştırılmamalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Çizgi grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Aktiviteden Doğan Satış Sayısı

- Açıklama: Bir aktivite kaydı ile ilişkili satış sayısıdır.
- İş amacı: Saha faaliyetinden doğrudan üretilen satışları ölçmek.
- Hesaplama formülü: `COUNT(sales.id WHERE activity_id IS NOT NULL)` veya `SUM(sale_count WHERE activity_id IS NOT NULL)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `id`, `activity_id`, `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Aktivite ilişkisi hatalı veya eksikse KPI kalite göstergesi olarak da okunmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + bağlı/bağımsız satış karşılaştırma grafiği
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Aktiviteye Bağlanamayan Satış Sayısı

- Açıklama: Herhangi bir aktivite kaydına bağlanmamış satış sayısıdır.
- İş amacı: Veri kalitesini ve süreç dışı satış girişlerini izlemek.
- Hesaplama formülü: `COUNT(sales.id WHERE activity_id IS NULL)` veya `SUM(sale_count WHERE activity_id IS NULL)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `id`, `activity_id`, `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Bu KPI performans düşüklüğü değil, veri bağlantı kalitesi göstergesidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Uyarı kartı + tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

## 5.3 Finansal KPI'lar

### KPI Adı
Toplam BES Tahsilatı

- Açıklama: BES ürün tipi için girilen toplam tahsilat tutarıdır.
- İş amacı: BES satışlarının tahsilat bazlı finansal çıktısını izlemek.
- Hesaplama formülü: `SUM(sales.collection_amount WHERE product_type = 'BES')`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `collection_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, hesap/müşteri
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tahsilat ayrı transaction olarak modellenmediği için MVP'de satış üzerindeki alan üzerinden okunur.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Özet kart + trend grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Toplam BES APE

- Açıklama: BES ürün tipi için girilen toplam APE tutarıdır.
- İş amacı: BES üretiminin standardize satış katkısını izlemek.
- Hesaplama formülü: `SUM(sales.ape_amount WHERE product_type = 'BES')`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `ape_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: APE tanımı sistem dışı ekiplerle de yazılı olarak teyit edilmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + ürün finansal tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Toplam BES Toplu Para

- Açıklama: BES için kaydedilmiş toplu para tutarlarının toplamıdır.
- İş amacı: BES işlemlerindeki toplu ödeme hacmini görmek.
- Hesaplama formülü: `SUM(sales.lump_sum_amount WHERE product_type = 'BES')`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `lump_sum_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Boş alanlar sıfır kabul edilmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart veya finansal özet tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Toplam Hayat Primi

- Açıklama: Hayat ürün tipi için toplam prim tutarıdır.
- İş amacı: Hayat sigortası finansal üretimini izlemek.
- Hesaplama formülü: `SUM(sales.premium_amount WHERE product_type = 'LIFE')`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `premium_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Prim ile tahsilat aynı KPI değildir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + ürün finansal grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Toplam Sağlık Üretimi

- Açıklama: Sağlık ürünleri için tanımlı finansal üretim alanlarının toplamıdır. MVP'de bu alan `premium_amount` olarak kabul edilir.
- İş amacı: Sağlık ürün performansını finansal tutar üzerinden izlemek.
- Hesaplama formülü: `SUM(sales.premium_amount WHERE product_type = 'HEALTH')`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `premium_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Ürün bazlı finansal alan mantığı ileride genişletilirse bu KPI güncellenmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + ürün bazlı tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Ürün Bazlı Toplam Finansal Üretim

- Açıklama: Her ürün için ilgili finansal üretim alanının toplamıdır.
- İş amacı: Ürünler arasında finansal karşılaştırma yapmak.
- Hesaplama formülü:
  - BES için öncelik: `SUM(collection_amount)` ve `SUM(ape_amount)`
  - LIFE/HEALTH/TRAVEL/OTHER için: `SUM(premium_amount)`
- Veri kaynağı: `sales`, `insurance_product_types`
- Gerekli alanlar: `product_type_id`, `collection_amount`, `ape_amount`, `premium_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tek bir finansal alanla tüm ürünleri karşılaştırmak yanıltıcı olabilir; dashboard açıklamasında hangi alanın kullanıldığı belirtilmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Tablo + ürün bazlı bar chart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Aylık Ödeme Toplamı

- Açıklama: Aylık ödeme alanı dolu satışların toplam aylık ödeme tutarıdır.
- İş amacı: Düzenli ödeme taahhütlerini izlemek.
- Hesaplama formülü: `SUM(sales.monthly_payment_amount)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `monthly_payment_amount`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Bu KPI tahsilat değildir; geleceğe dönük ödeme yapısını gösterir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Finansal kart + trend grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Gün / Hafta / Ay / Yıl Bazında Finansal Üretim

- Açıklama: Seçilen finansal alanların zaman içindeki dağılımıdır.
- İş amacı: Üretimin dönemsel hareketini görmek.
- Hesaplama formülü: `SUM(financial_metric) GROUP BY time_bucket(sale_date)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `sale_date`, ilgili finansal alan
- Filtrelenebilir boyutlar: ürün, personel, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Aynı grafikte farklı finansal kavramlar karıştırılmamalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Çizgi/alan grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.4 Masraf KPI'ları

### KPI Adı
Toplam Masraf

- Açıklama: Seçilen bağlamdaki toplam masraf tutarıdır.
- İş amacı: Saha operasyonunun toplam maliyetini görmek.
- Hesaplama formülü: `SUM(expenses.amount)`
- Veri kaynağı: `expenses`
- Gerekli alanlar: `amount`, `expense_date`
- Filtrelenebilir boyutlar: tarih, personel, masraf türü, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Onay dışı veya iptal edilmiş masraflar ileride ayrı ele alınabilir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Kart + trend grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Personel Bazlı Masraf

- Açıklama: Personel başına toplam masraf tutarıdır.
- İş amacı: Masraf dağılımını çalışan bazında görmek.
- Hesaplama formülü: `SUM(expenses.amount) GROUP BY employee_id`
- Veri kaynağı: `expenses`, `employees`
- Gerekli alanlar: `employee_id`, `amount`, `expense_date`
- Filtrelenebilir boyutlar: tarih, masraf türü, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tek başına düşük veya yüksek masraf performans anlamı taşımaz; aktivite ve satış ile birlikte yorumlanmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Sıralı bar chart + tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Masraf Türüne Göre Toplam

- Açıklama: Masrafların tür bazlı dağılımıdır.
- İş amacı: Yol, yemek, konaklama ve diğer masraf yapısını anlamak.
- Hesaplama formülü: `SUM(expenses.amount) GROUP BY expense_type_id`
- Veri kaynağı: `expenses`, `expense_types`
- Gerekli alanlar: `expense_type_id`, `amount`, `expense_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tür seçimi veri girişinde standardize edilmezse kalite bozulur.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Donut/bar chart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Günlük / Haftalık / Aylık Masraf

- Açıklama: Masraf tutarlarının zaman içindeki dağılımıdır.
- İş amacı: Operasyon maliyet trendini izlemek.
- Hesaplama formülü: `SUM(expenses.amount) GROUP BY time_bucket(expense_date)`
- Veri kaynağı: `expenses`
- Gerekli alanlar: `amount`, `expense_date`
- Filtrelenebilir boyutlar: personel, masraf türü, bölge
- Zaman kırılımı: gün, hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Giderin işlenme tarihi ile harcama tarihi ayrışırsa yalnızca `expense_date` kullanılmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Çizgi/kolon grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin, Operations

### KPI Adı
Ziyaret Başına Masraf

- Açıklama: Toplam masrafın toplam aktivite sayısına bölünmesiyle hesaplanan ortalama masraftır.
- İş amacı: Aktivite maliyet seviyesini görmek.
- Hesaplama formülü: `SUM(expenses.amount) / COUNT(activities.id)`
- Veri kaynağı: `expenses`, `activities`
- Gerekli alanlar: `amount`, `expense_date`, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Masraf ile aktivitenin birebir ilişkilendirilmesi gerekmez; aynı tarih bağlamı üzerinden oran hesaplanır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Verimlilik kartı
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Satış Başına Masraf

- Açıklama: Toplam masrafın toplam satış adedine bölünmesiyle hesaplanan ortalama masraftır.
- İş amacı: Satış üretim maliyetini anlamak.
- Hesaplama formülü: `SUM(expenses.amount) / SUM(sales.sale_count)`
- Veri kaynağı: `expenses`, `sales`
- Gerekli alanlar: `amount`, `expense_date`, `sale_count`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Satış adedi çok düşük dönemlerde oran aşırı oynak olabilir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Verimlilik kartı + personel karşılaştırma tablosu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.5 Verimlilik KPI'ları

### KPI Adı
Masraf / Satış Oranı

- Açıklama: Toplam masrafın toplam satış adedine veya toplam finansal üretime oranlanmış verimlilik göstergesidir. MVP standardı adet bazlıdır.
- İş amacı: Operasyonel maliyet etkinliğini görmek.
- Hesaplama formülü: `SUM(expenses.amount) / SUM(sales.sale_count)`
- Veri kaynağı: `expenses`, `sales`
- Gerekli alanlar: `amount`, `sale_count`, tarih alanları
- Filtrelenebilir boyutlar: tarih, personel, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: İsimde “oran” geçse de sonuç parasal değer olabilir; UI'da açık etiketlenmelidir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Verimlilik tablo sütunu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Masraf / Tahsilat Oranı

- Açıklama: Toplam masrafın toplam tahsilata oranıdır.
- İş amacı: Nakit üretime göre maliyet etkinliğini görmek.
- Hesaplama formülü: `SUM(expenses.amount) / SUM(sales.collection_amount)`
- Veri kaynağı: `expenses`, `sales`
- Gerekli alanlar: `amount`, `collection_amount`, tarih alanları
- Filtrelenebilir boyutlar: tarih, personel, bölge, ürün
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tahsilat verisi tüm ürünlerde eşit kaliteyle tutulmuyorsa yanıltıcı olabilir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Finansal verimlilik kartı
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Kişi Başı Üretim

- Açıklama: Toplam finansal üretimin satış yapan personel sayısına bölünmesidir. MVP standardı ürün bazlı uygun finansal alanların toplamına dayanır.
- İş amacı: Finansal üretimi çalışan bazında normalize görmek.
- Hesaplama formülü: `SUM(financial_production_metric) / COUNT(DISTINCT sales.employee_id)`
- Veri kaynağı: `sales`, `employees`
- Gerekli alanlar: `employee_id`, ilgili finansal alan, `sale_date`
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Hangi finansal alanın kullanıldığı raporda açıkça yazılmalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Tablo sütunu + kart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Kişi Başı Tahsilat

- Açıklama: Toplam tahsilatın tahsilat üreten personel sayısına bölünmesidir.
- İş amacı: Personel bazında nakit üretim verimini görmek.
- Hesaplama formülü: `SUM(sales.collection_amount) / COUNT(DISTINCT sales.employee_id)`
- Veri kaynağı: `sales`
- Gerekli alanlar: `collection_amount`, `employee_id`, `sale_date`
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tüm ürünlerde tahsilat alanı kullanılmıyorsa sınırlı yorum yapılmalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Personel performans tablosu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Kişi Başı Kârlılık Benzeri Yorumlanabilir Metrik

- Açıklama: Finansal üretimden masraf düşülerek kişi bazlı yorumlanan türev metriktir.
- İş amacı: Kişi bazında kaba ekonomik katkı görmek.
- Hesaplama formülü: `SUM(financial_metric) - SUM(expenses.amount)` veya kişi başına normalize türevleri
- Veri kaynağı: `sales`, `expenses`
- Gerekli alanlar: ilgili finansal alanlar, `amount`, `employee_id`, tarih alanları
- Filtrelenebilir boyutlar: tarih, personel, bölge, ürün
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Bu gerçek kârlılık değildir; muhasebesel kâr ile karıştırılmamalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Yorumlayıcı tablo metriği
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.6 Dönüşüm KPI'ları

### KPI Adı
Aktiviteden Satışa Dönüşüm Oranı

- Açıklama: Toplam aktivite içinde satışa dönüşen aktivite oranıdır.
- İş amacı: Saha aktivitesinin satış üretme gücünü görmek.
- Hesaplama formülü: `COUNT(DISTINCT sales.activity_id) / COUNT(activities.id)`
- Veri kaynağı: `activities`, `sales`
- Gerekli alanlar: `activities.id`, `sales.activity_id`, ilgili tarih alanları
- Filtrelenebilir boyutlar: tarih, personel, bölge, ürün
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Pay ve payda aynı filtre bağlamında çalışmalıdır; satış tarihi yerine temel olarak aktivite tarihi bağlamı kullanılmalıdır.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Ana verimlilik kartı + trend
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Görüşmeden Satışa Dönüşüm Oranı

- Açıklama: Görüşüldü statüsündeki aktivitelerin satışa dönüşme oranıdır.
- İş amacı: Temas kurulmuş fırsatların satış üretme başarısını görmek.
- Hesaplama formülü: `COUNT(DISTINCT sales.activity_id linked to interviewed activities) / COUNT(activities.id WHERE result_type = 'Interviewed')`
- Veri kaynağı: `activities`, `sales`, `activity_result_types`
- Gerekli alanlar: `activities.id`, `result_type_id`, `sales.activity_id`
- Filtrelenebilir boyutlar: tarih, personel, bölge, ürün
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: `Olumlu` ve `Satış Oldu` statüleriyle çakışan veri kalitesi sorunları varsa dikkatli yorumlanmalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Verimlilik kartı
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Personel Bazlı Dönüşüm Oranı

- Açıklama: Personelin yaptığı aktivitelerden satışa dönenlerin oranıdır.
- İş amacı: Çalışan bazlı saha verimliliğini görmek.
- Hesaplama formülü: `COUNT(DISTINCT sales.activity_id GROUP BY employee_id) / COUNT(activities.id GROUP BY employee_id)`
- Veri kaynağı: `activities`, `sales`
- Gerekli alanlar: `employee_id`, `activities.id`, `sales.activity_id`, tarih alanları
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Aktivitesi az olan personelde oran aşırı oynak olabilir; adet ile birlikte gösterilmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Personel tablo sütunu + bar chart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Bölge Bazlı Dönüşüm Oranı

- Açıklama: Bölge bazında aktivitelerin satışa dönüşme oranıdır.
- İş amacı: Coğrafi verimliliği görmek.
- Hesaplama formülü: `COUNT(DISTINCT sales.activity_id by region) / COUNT(activities.id by region)`
- Veri kaynağı: `activities`, `sales`, `employees` veya `accounts`
- Gerekli alanlar: bölge alanı, `activities.id`, `sales.activity_id`
- Filtrelenebilir boyutlar: tarih, personel, ürün, ilçe
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Bölge kaynağı standart değilse KPI güvenilmez olur.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Harita benzeri veya bölge tablo
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.7 Personel Performans KPI'ları

### KPI Adı
Personel Performans Skoru

- Açıklama: Tek bir skor yerine personel performans tablosunda çoklu KPI seti birlikte yorumlanır.
- İş amacı: Tek metrikle yanıltıcı değerlendirme yerine dengeli karşılaştırma yapmak.
- Hesaplama formülü: Tek skor önerilmez; aşağıdaki KPI'lar birlikte sunulur:
  - aktivite sayısı
  - satış adedi
  - dönüşüm oranı
  - finansal üretim
  - masraf
- Veri kaynağı: `activities`, `sales`, `expenses`
- Gerekli alanlar: `employee_id`, tarih alanları, ilgili metrik alanları
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: hafta, ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: MVP'de birleşik skor üretilmeyecektir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Personel karşılaştırma tablosu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Personel Bazlı Finansal Üretim

- Açıklama: Personelin ürettiği toplam finansal tutardır.
- İş amacı: Çalışan bazlı finansal katkıyı görmek.
- Hesaplama formülü: `SUM(financial_metric) GROUP BY employee_id`
- Veri kaynağı: `sales`
- Gerekli alanlar: `employee_id`, ilgili finansal alan, `sale_date`
- Filtrelenebilir boyutlar: tarih, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Ürün bazlı finansal alan farklılıkları net etiketlenmelidir.
- MVP'de var mı?: Evet
- Dashboard'da nasıl gösterilmeli?: Personel performans tablosu
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.8 Bölge / İlçe KPI'ları

### KPI Adı
Bölge Bazlı Satış Adedi

- Açıklama: Satışların bölgelere göre dağılımıdır.
- İş amacı: Coğrafi satış yoğunluğunu görmek.
- Hesaplama formülü: `SUM(sales.sale_count) GROUP BY region`
- Veri kaynağı: `sales`, `employees` veya `accounts`
- Gerekli alanlar: bölge alanı, `sale_count`, `sale_date`
- Filtrelenebilir boyutlar: tarih, personel, ürün, ilçe
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Satış bölgesi tanımı standart olmalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Bölge bar chart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
İlçe Bazlı Aktivite

- Açıklama: İlçe seviyesinde aktivite yoğunluğunu gösterir.
- İş amacı: Mikro saha planlaması yapmak.
- Hesaplama formülü: `COUNT(activities.id) GROUP BY district`
- Veri kaynağı: `activities`, `accounts`
- Gerekli alanlar: ilçe alanı, `activity_date`
- Filtrelenebilir boyutlar: tarih, personel, bölge, sonuç türü
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: İlçe verisi doldurulma kalitesi yeterli değilse future phase'e bırakılmalıdır.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Tablo veya detay bar chart
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 5.9 Zaman Trend KPI'ları

### KPI Adı
Aylık Aktivite Trend Endeksi

- Açıklama: Aylık aktivite değişimini önceki döneme göre yüzdesel gösterir.
- İş amacı: Artış/azalış trendini hızlı okumak.
- Hesaplama formülü: `(current_period_activity - previous_period_activity) / previous_period_activity`
- Veri kaynağı: `activities`
- Gerekli alanlar: `activity_date`
- Filtrelenebilir boyutlar: personel, bölge, sonuç türü
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Önceki dönem sıfır ise oran tanımsız olur; özel gösterim gerekir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Trend badge
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Aylık Satış Trend Endeksi

- Açıklama: Satışların önceki döneme göre artış/azalış yüzdesidir.
- İş amacı: Satış ivmesini izlemek.
- Hesaplama formülü: `(current_period_sales - previous_period_sales) / previous_period_sales`
- Veri kaynağı: `sales`
- Gerekli alanlar: `sale_date`, `sale_count`
- Filtrelenebilir boyutlar: personel, ürün, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Küçük hacimli ekiplerde oynaklık yüksektir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Trend badge + çizgi grafik
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

### KPI Adı
Aylık Masraf Trend Endeksi

- Açıklama: Masrafın önceki döneme göre değişim yüzdesidir.
- İş amacı: Maliyet artışını erken görmek.
- Hesaplama formülü: `(current_period_expense - previous_period_expense) / previous_period_expense`
- Veri kaynağı: `expenses`
- Gerekli alanlar: `expense_date`, `amount`
- Filtrelenebilir boyutlar: personel, tür, bölge
- Zaman kırılımı: ay, yıl
- Uyarılar / dikkat edilmesi gerekenler: Tek seferlik yüksek giderler trendi bozabilir.
- MVP'de var mı?: Hayır
- Dashboard'da nasıl gösterilmeli?: Trend badge
- Hangi kullanıcı rolleri görmeli?: Manager, Admin

## 6. MVP KPI Listesi

MVP'de yer alacak KPI'lar:

- Toplam Aktivite Sayısı
- Görüşülen Aktivite Sayısı
- Görüşülmeyen Aktivite Sayısı
- Olumlu Sonuçlanan Aktivite Sayısı
- Olumsuz Sonuçlanan Aktivite Sayısı
- Personel Başına Aktivite
- Günlük / Haftalık / Aylık Aktivite
- Bölge Bazlı Aktivite
- Toplam Satış Adedi
- Ürün Bazlı Satış Adedi
- Personel Bazlı Satış Adedi
- Tarih Bazlı Satış Sayısı
- Aktiviteden Doğan Satış Sayısı
- Aktiviteye Bağlanamayan Satış Sayısı
- Toplam BES Tahsilatı
- Toplam BES APE
- Toplam BES Toplu Para
- Toplam Hayat Primi
- Toplam Sağlık Üretimi
- Ürün Bazlı Toplam Finansal Üretim
- Aylık Ödeme Toplamı
- Gün / Hafta / Ay / Yıl Bazında Finansal Üretim
- Toplam Masraf
- Personel Bazlı Masraf
- Masraf Türüne Göre Toplam
- Günlük / Haftalık / Aylık Masraf
- Satış Başına Masraf
- Aktiviteden Satışa Dönüşüm Oranı
- Personel Bazlı Dönüşüm Oranı
- Personel Bazlı Finansal Üretim

## 7. Future Phase KPI Listesi

İleri faza bırakılacak KPI'lar:

- Ziyaret Başına Masraf
- Masraf / Satış Oranı
- Masraf / Tahsilat Oranı
- Kişi Başı Üretim
- Kişi Başı Tahsilat
- Kişi Başı Kârlılık Benzeri Yorumlanabilir Metrik
- Görüşmeden Satışa Dönüşüm Oranı
- Bölge Bazlı Dönüşüm Oranı
- Personel Performans Skoru
- Bölge Bazlı Satış Adedi
- İlçe Bazlı Aktivite
- Aylık Aktivite Trend Endeksi
- Aylık Satış Trend Endeksi
- Aylık Masraf Trend Endeksi

## 8. Uygulama ve API İçin Referans Notları

- Dashboard sorgularında tarih alanı varsayılanı KPI bazlı belirlenmelidir:
  - Aktivite KPI'ları için `activity_date`
  - Satış ve finansal KPI'lar için `sale_date`
  - Masraf KPI'ları için `expense_date`
- Aynı dashboard ekranında farklı tarih mantıkları kullanılıyorsa bu kullanıcıya açıkça belirtilmelidir.
- KPI endpoint'leri tekil metrik mantığına değil, kategori bazlı özet dönebilecek şekilde tasarlanmalıdır.
- Personel performansı ekranı tek bir skor yerine çoklu KPI kolonları ile kurulmalıdır.
- Financial KPI açıklamalarında kullanılan alan adı ve iş anlamı ekranda görünür olmalıdır.
