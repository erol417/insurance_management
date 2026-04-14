# Insurance Management Project Design

## 1. Projenin Amacı ve Temel Vizyonu

Bu projenin amacı, sigorta şirketinin saha satış ve operasyon süreçlerini tek merkezden takip edebileceği web tabanlı bir uygulama oluşturmaktır. Sistem; saha personelinin günlük ziyaretlerini, müşteri/firma görüşmelerini, bu görüşmelerin sonuçlarını, satış dönüşümlerini ve yapılan masrafları kayıt altına almalı; yöneticilere de operasyonel karar almayı destekleyen raporlar ve dashboard'lar sunmalıdır.

Ana hedef, dağınık Excel takibi yerine sürdürülebilir, ölçülebilir ve denetlenebilir bir veri yapısı kurmaktır. Uygulama yalnızca veri girişi yapan bir ekran seti değil, saha aktivitesinden satışa dönüşümün izlenebildiği bir yönetim aracı olmalıdır.

## 2. İş Problemi

Mevcut işleyişte saha ekiplerinin yaptığı aktiviteler, satış çıktıları ve masraflar büyük ölçüde manuel veya parçalı şekilde takip edilmektedir. Bu durum aşağıdaki sorunlara yol açar:

- Aynı müşteri veya firma ile yapılan geçmiş görüşmelerin izlenmesi zorlaşır.
- Saha aktivitesinin gerçek satış performansına etkisi net görülemez.
- Personel verimliliği sadece satış adediyle ölçülür, dönüşüm ve maliyet ilişkisi kaybolur.
- Yönetici raporları gecikmeli, manuel ve hataya açık hazırlanır.
- Excel dosyaları özet veri içerdiğinde, geriye dönük detay analizi sınırlanır.
- Denetim izi ve veri güvenilirliği düşer.

Bu sistemle çözülmek istenen ana problem, operasyonel ham veriyi standartlaştırmak ve bu ham veriden doğru KPI'lar türetebilen bir yapı kurmaktır.

## 3. İş Hedefleri

- Saha personelinin günlük aktivite kayıtlarını standart hale getirmek
- Aktivite, satış ve masraf verilerini ilişkili biçimde tutmak
- Yönetici için hızlı karar destek ekranları üretmek
- Personel bazlı performansı adet, tutar, verimlilik ve maliyet perspektifleriyle izlemek
- Excel bağımlılığını azaltmak ve ileride sistemin ana veri kaynağını uygulamanın kendisi yapmak
- Geçmiş operasyon verisini analiz edilebilir yapıda saklamak

## 4. Kullanıcı Tipleri

### 4.1 Saha Personeli

Sahada müşteri/firma ziyareti yapan, aktivite oluşturan, gerekiyorsa satış kaydı ve masraf kaydı giren kullanıcıdır.

Beklenen ihtiyaçları:

- Hızlı aktivite girişi
- Ziyaret geçmişini görüntüleme
- Satış ve masraf kayıtlarını kolayca ekleme
- Kendi performansını görebilme

### 4.2 Call Center Kullanıcısı

Potansiyel müşteri araştırması yapan, iletişim bilgisi bulan ve lead havuzu oluşturan kullanıcıdır.

Beklenen ihtiyaçları:

- Yeni lead kaydı açma
- Potansiyel müşteri notu ve iletişim bilgisi girme
- Lead listesini güncelleme
- Atamaya hazır kayıtları hazırlama

### 4.3 Operasyon Kullanıcısı

Saha ekibinden gelen verileri kontrol eden, gerektiğinde satış veya müşteri kayıtlarını düzenleyen, veri kalitesini takip eden kullanıcıdır.

Beklenen ihtiyaçları:

- Satış kayıtlarını doğrulama/düzenleme
- Müşteri/firma kayıtlarını konsolide etme
- Eksik veya hatalı girişleri tespit etme
- Excel import süreçlerini yönetme

### 4.4 Satış Müdürü

Lead havuzunu değerlendiren, saha personeline atama yapan ve saha iş yükünü yöneten kullanıcıdır.

Beklenen ihtiyaçları:

- Atama bekleyen lead havuzunu görme
- Lead'leri saha personeline atama
- Personel bazlı açık iş yoğunluğunu izleme
- Lead'den aktiviteye dönüşüm kalitesini izleme

### 4.5 Yönetici

Takımın genel performansını, satış sonuçlarını, masraf yapısını ve trendleri izleyen karar vericidir.

Beklenen ihtiyaçları:

- Dashboard ve özet raporlar
- Tarih, ürün, personel ve bölge bazlı filtreleme
- Aktiviteden satışa dönüşüm oranı
- Masraf/satış ilişkisi
- Hızlı performans karşılaştırmaları

### 4.6 Sistem Yöneticisi

Rol, yetki, tanım tabloları ve sistemsel ayarları yöneten kullanıcıdır.

Beklenen ihtiyaçları:

- Kullanıcı/rol yönetimi
- Referans veri yönetimi
- Log ve audit kayıtlarına erişim

## 5. Temel Kullanım Senaryoları

### 5.0 Lead Toplama ve Atama

Call center potansiyel müşteri araştırması yapar, lead kaydı oluşturur ve iletişim notlarını ekler. Satış müdürü bu lead'leri saha personeline atar. Saha personelinin günlük saha işi bu atanan lead listesinden oluşur.

### 5.1 Günlük Aktivite Girişi

Saha personeli, ziyaret ettiği müşteri veya firmayı seçer ya da yeni kayıt oluşturur. Ziyaret tarihini, görüşme içeriğini, temas durumunu ve gerekiyorsa görüşmenin ticari sonucunu kaydeder.

### 5.2 Aktiviteden Satış Oluşturma

Bir aktivite olumlu sonuçlandığında veya doğrudan satışa dönüştüğünde, ilgili aktivite üzerinden satış kaydı açılır. Böylece satışın hangi saha hareketinden üretildiği izlenebilir.

### 5.3 Bağımsız Satış Kaydı

Bazı durumlarda satış daha sonra operasyon tarafından girilebilir. Bu senaryoda satış kaydı aktivite ile ilişkilendirilmeye çalışılır; ilişki kurulamazsa gerekçeli şekilde bağımsız satış olarak tutulur.

### 5.4 Masraf Girişi

Saha personeli tarih bazlı yol, yemek, konaklama veya diğer masrafları kaydeder. Masraf kayıtları personel bazlı izlenir ve raporlamaya dahil edilir.

### 5.5 Yönetici Analizi

Yönetici belirli bir tarih aralığında toplam aktivite, toplam satış, ürün kırılımı, personel performansı, dönüşüm oranı ve masraf trendlerini inceler.

### 5.6 Excel Referans Verisi ile İlk Uyumlama

Mevcut Excel dosyaları ilk analiz aşamasında referans olarak kullanılır. Amaç, sistemde tutulacak ham veriyi ve KPI tanımlarını bu raporlara bakarak doğrulamak; sistemi Excel çıktısına bağımlı kılmamak olacaktır.

## 6. MVP Kapsamı

MVP, temel operasyonel akışların çalıştığı ve yöneticiye ilk anlamlı raporların sunulduğu sürüm olmalıdır.

### MVP içinde olması gerekenler

- Kullanıcı girişi ve temel rol ayrımı
- Call center tarafından lead havuzu oluşturma
- Satış müdürü tarafından lead atama
- Personel tanımı
- Müşteri/firma temel kayıt yapısı
- Aktivite kayıt oluşturma, listeleme ve detay görüntüleme
- Satış kayıt oluşturma, listeleme ve aktivite ilişkisi
- Masraf kayıt oluşturma ve listeleme
- Temel dashboard metrikleri
- Tarih/personel/ürün bazlı filtreleme
- Excel import için ilk sürüm yaklaşımı veya hazırlık altyapısı
- Audit alanları (created by, created at, updated by, updated at)

### MVP başarı kriterleri

- Saha personeli günlük veriyi sisteme girebilmeli
- Yönetici haftalık ve aylık performansı görüntüleyebilmeli
- Aktivite-satış ilişkisi ölçülebilmeli
- Toplam masraf ve personel başına masraf görülebilmeli

## 7. MVP Dışı Ama Planlanan Özellikler

- Gelişmiş onay akışları
- Masraf onay süreci
- Gelişmiş hedef yönetimi ve kota takibi
- Bölge/segment bazlı performans analizi
- Müşteri temas geçmişi zaman çizelgesi
- Bildirim ve hatırlatma altyapısı
- Mobil uyumun ötesine geçen PWA veya native entegrasyon
- Dosya/fiş ekleme
- Harita tabanlı ziyaret görselleştirme
- Gelişmiş export ve planlanmış rapor gönderimi

## 8. Modül Listesi

### 8.1 Kimlik ve Yetkilendirme

Kullanıcı oturumu, roller, yetkiler ve erişim kontrolünü yönetir.

### 8.2 Personel Yönetimi

Saha personeli ve diğer kullanıcıların temel profil bilgisini tutar.

### 8.3 Müşteri/Firma Yönetimi

Görüşme yapılan kişi, müşteri adayı, bireysel müşteri veya kurumsal firma bilgisini yönetir.

### 8.4 Lead Yönetimi

Call center tarafından bulunan potansiyel kayıtları, statülerini ve saha atamalarını yönetir.

### 8.5 Aktivite Yönetimi

Saha ziyaretleri, görüşmeler, sonuçlar, notlar ve takip aksiyonlarını tutar.

### 8.6 Satış Yönetimi

Satışın ürün tipi, finansal değerleri, satış adedi ve aktivite ilişkisini yönetir.

### 8.7 Masraf Yönetimi

Personel bazlı masraf kayıtlarını yönetir.

### 8.8 Dashboard ve Raporlama

Ham veriden KPI türeterek özet ekranlar sunar.

### 8.9 Excel Import

Geçmiş veya referans verilerin sisteme kontrollü alınmasını sağlar.

### 8.10 Audit ve Loglama

Değişiklik izleri, kritik işlem kayıtları ve hata takibini sağlar.

## 9. Domain Modeli Önerisi

Sistem dört ana iş ekseni üzerine kurulmalıdır:

- Lead
- Aktivite
- Satış
- Masraf

Bu üç eksen ortak boyutlarla ilişkilendirilir:

- Personel
- Müşteri/Firma
- Ürün
- Tarih
- Rol/Kullanıcı

Domain yaklaşımı şu prensipleri korumalımalıdır:

- Aktivite, satıştan ayrı bir kavramdır.
- Satış, mümkün olduğunda bir aktiviteye bağlanmalıdır.
- Masraf, satıştan bağımsız da olabilir ama raporda personel ve tarih ile ilişkilendirilir.
- Dashboard verileri özet tablolar yerine ham işlem kayıtlarından türetilmelidir.
- Aktivite sonucu tek alanlı düşünülmemeli; temas gerçekleşmesi ile görüşme sonucu ayrı kavramlar olarak ele alınmalıdır.

Bu nedenle aktivite modelinde aşağıdaki iki kavram ayrı tutulmalıdır:

- `ContactStatus`
- `OutcomeStatus`

## 10. Ana Entity Listesi

Önerilen temel entity'ler:

- User
- Role
- Permission
- Employee
- Lead
- LeadAssignment
- LeadStatusType
- Account
- AccountContact
- Activity
- ActivityContactStatusType
- ActivityOutcomeStatusType
- ActivityNote
- InsuranceProductType
- Sale
- Expense
- ExpenseType
- ImportBatch
- ImportRow
- AuditLog

MVP için sadeleştirme kararı olarak `Customer` ve `Company` ayrı değil, tek bir `Account` çatısı altında ele alınmalıdır. Kurumsal temas kişileri için `AccountContact` yapısı ihtiyaç ve veri giriş hızı dengesine göre opsiyonel devreye alınabilir.

## 11. Entity İlişkileri

Önerilen ilişki mantığı:

- Bir `Employee`, birçok `Activity` kaydı oluşturabilir.
- Bir `Activity`, bir `Account` ile ilişkili olmalıdır.
- Bir `Activity`, opsiyonel olarak bir `AccountContact` ile ilişkili olabilir.
- Bir `Activity`, bir `ActivityContactStatusType` ile ilişkili olmalıdır.
- Bir `Activity`, bir `ActivityOutcomeStatusType` ile ilişkili olmalıdır.
- Bir `Activity` en fazla bir ana satışa bağlanabilir; ancak ileride bir aktiviteden birden fazla ürün satışı çıkma ihtimali değerlendirilmelidir.
- Bir `Sale`, mümkünse bir `Activity` kaydına bağlı olmalıdır.
- Bir `Sale`, bir `InsuranceProductType` ile ilişkilidir.
- Bir `Sale` için ürün tipine göre değişebilen finansal alanlar tutulur.
- Bir `Employee`, birçok `Expense` kaydı oluşturabilir.
- `Expense`, `ExpenseType` ile ilişkilidir.
- `ImportBatch`, içeri aktarılan dosyanın işlem üst bilgisidir; `ImportRow` satır bazlı sonucu saklar.
- `AuditLog`, kritik entity değişimlerini referanslar.

Aktivite sonucu açısından temel iş kuralı:

- `ContactStatus = NOT_CONTACTED` ise `OutcomeStatus = NOT_APPLICABLE` olmalıdır.
- `OutcomeStatus = SALE_CLOSED` tek başına satış kaydı anlamına gelmez; satış teyidi `Sale` kaydı üzerinden yapılmalıdır.

## 12. Ekran Listesi

### Yönetim ve ortak ekranlar

- Giriş ekranı
- Ana dashboard
- Kullanıcı listesi
- Rol/yetki yönetimi
- Personel listesi ve detay

### Müşteri/Firma ekranları

- Müşteri/firma listesi
- Müşteri/firma oluşturma-düzenleme
- Müşteri/firma detay ve temas geçmişi

### Aktivite ekranları

- Aktivite listesi
- Aktivite oluşturma
- Aktivite detay
- Aktivite düzenleme

### Satış ekranları

- Satış listesi
- Satış oluşturma
- Satış detay
- Satış düzenleme

### Masraf ekranları

- Masraf listesi
- Masraf oluşturma
- Masraf detay/düzenleme

### Raporlama ekranları

- Yönetici dashboard
- Personel performans ekranı
- Ürün kırılım ekranı
- Masraf analizi ekranı
- Trend analizi ekranı

### Veri yönetimi ekranları

- Excel import ekranı
- Import geçmişi ve hata ekranı
- Audit log görüntüleme ekranı

## 13. Dashboard ve KPI Mantığı

Dashboard yapısı doğrudan ham işlem kayıtları üzerinden kurgulanmalı, manuel özet dosyalara veya sabit hesap tablolarına bağımlı olmamalıdır.

### Temel KPI kategorileri

#### Aktivite KPI'ları

- Toplam aktivite sayısı
- Görüşülen/görüşülmeyen sayısı
- Olumlu/olumsuz/ertelenen aktivite sayısı
- Personel başına aktivite
- Günlük/haftalık/aylık aktivite trendi

#### Satış KPI'ları

- Toplam satış adedi
- Toplam prim
- Toplam APE
- Toplam toplu para
- Toplam aylık ödeme
- Toplam tahsilat
- Ürün bazlı satış dağılımı

#### Dönüşüm KPI'ları

- Aktiviteden satışa dönüşüm oranı
- Personel bazlı dönüşüm oranı
- Ürün bazlı dönüşüm oranı
- Aktivite başına satış tutarı

#### Masraf KPI'ları

- Toplam masraf
- Masraf türüne göre dağılım
- Personel başına masraf
- Satış başına masraf
- Aktivite başına masraf

#### Verimlilik KPI'ları

- Personel başına satış adedi
- Personel başına tahsilat
- Personel başına APE
- Masraf / tahsilat oranı
- Masraf / prim oranı

### KPI prensipleri

- Her KPI için açık tanım yapılmalı
- Pay ve payda mantığı belgelendirilmeli
- Hangi tarih alanına göre hesaplandığı net olmalı
- İptal, düzenleme veya silme senaryoları hesaplamaya etkisiyle birlikte tanımlanmalı
- Aktivite KPI'larında temas durumu ile sonuç durumu ayrı analitik kümeler olarak ele alınmalı
- `Satış Oldu` etiketi ile gerçek satış varlığı karıştırılmamalı; satış için `Sale` kaydı esas alınmalı

## 14. Filtreleme Mantığı

Rapor ve liste ekranlarında tutarlı filtreleme kritik önemdedir.

Önerilen ortak filtreler:

- Tarih aralığı
- Personel
- Ürün tipi
- Temas durumu
- Sonuç durumu
- Müşteri/firma
- Kullanıcı tipi / rol

İleri filtreler:

- Bölge veya saha alanı
- Yeni müşteri / mevcut müşteri ayrımı
- Satışı olan / olmayan aktiviteler
- Masraf türü

Filtreleme prensipleri:

- Tüm dashboard kartları seçilen filtre bağlamına uymalı
- Liste ve grafik sayıları tutarlı olmalı
- Varsayılan tarih filtresi ürün tarafından net tanımlanmalı
- Kullanıcıya birleşik “aktivite sonucu” filtresi gösterilse bile backend sorgu mantığında `ContactStatus` ve `OutcomeStatus` ayrımı korunmalı

## 15. Excel Import Yaklaşımı

Excel dosyaları ilk aşamada analiz ve geçiş desteği için kullanılmalıdır; sistemin ana veri modeli Excel şablonuna göre şekillenmemelidir.

### Yaklaşım prensipleri

- Excel kolonları doğrudan entity şeması olarak alınmamalı
- Önce hedef domain alanları belirlenmeli
- Ardından Excel kolonları bu alanlara eşlenmeli
- Ham veri ve özet veri ayrıştırılmalı
- Import sonrası hata raporu üretilebilmeli

### Import süreci önerisi

1. Dosya şablonu tanıma
2. Kolon eşleme
3. Ön doğrulama
4. Satır bazlı parse
5. Referans veri eşleme
6. Hatalı kayıtları ayırma
7. Başarılı kayıtları staging veya ana tabloya alma
8. Import özet raporu üretme

### Dikkat edilmesi gerekenler

- Aynı verinin tekrar import edilmesini önleme stratejisi
- Tarih ve sayı formatlarının standardizasyonu
- Müşteri/firma isim benzerliği nedeniyle oluşabilecek çoğalmalar
- Excel'deki özet ortalama tabloların ham veri yerine geçmemesi

## 16. Rol ve Yetki Mantığı

Mevcut sistem, hem statik rütbe koruması hem de dinamik modül yetkilendirmesi (RBAC) içeren hibrit bir yapı kullanır.

- **Admin**: Sistemdeki tüm yetkilere ve "Master Key" (Süper Yetki) bypass mekanizmasına sahiptir.
- **Dinamik Modüller**: Dashboard, Leads, Accounts, Activities, Sales, Expenses ve Imports modülleri, veritabanındaki `RolePermission` tablosu üzerinden rollere atanabilir.
- **Güvenlik Katmanı**: Yetki kontrolleri `CanAccessPermission` metodu ile merkezi olarak gerçekleştirilir, UI elemanları yetkiye göre otomatik gizlenir.

## 17. Veri Doğrulama Prensipleri

- Zorunlu alanlar ekran ve API seviyesinde doğrulanmalı
- Tarih alanları mantıksal kontrol içermeli
- Finansal alanlar negatif değer almamalı; gerekirse iş kuralı ile ayrıştırılmalı
- Aktivite sonucu ile satış ilişkisi kurallı olmalı
- Ürün tipine göre bazı finansal alanlar zorunlu veya opsiyonel olabilir
- `ContactStatus` zorunlu olmalı
- `OutcomeStatus`, `ContactStatus = CONTACTED` ise zorunlu veya sistemce varsayılanlanmış olmalı
- `NOT_CONTACTED + POSITIVE/NEGATIVE/POSTPONED/SALE_CLOSED` kombinasyonları geçersiz sayılmalı
- `OutcomeStatus = SALE_CLOSED` ise ilgili satış kaydı iş akışı doğrulanmalı
- Personel ve müşteri referansları geçerli olmalı
- Duplicate önleme için müşteri/firma alanlarında normalize eşleme düşünülmeli

## 18. Audit ve Log İhtiyacı

Bu sistem operasyonel karar ve performans takibi için kullanılacağı için audit ihtiyacı yüksektir.

Gerekli audit başlıkları:

- Kaydı kim oluşturdu
- Ne zaman oluşturdu
- Kim güncelledi
- Ne zaman güncelledi
- Hangi alan değişti
- Eski ve yeni değer neydi

## 19. Teknik Mimari Önerisi

### 19.1 Uygulanan Mimari İyileştirmeler

#### Sprint 1 — Temel Temizlik ve Güvenlik (Tamamlandı)
- **AppDataStore kaldırıldı**: In-memory singleton veri deposu tamamen kaldırılmış, tüm veri erişimi `AppDbContext` üzerinden sağlanmaktadır.
- **BaseEntity sınıfı oluşturuldu**: Audit alanları (`CreatedAt`, `CreatedBy` vb.) tüm transaction entity'lerine eklendi.
- **Şifre güvenliği**: Kullanıcı şifreleri BCrypt ile hash'lenerek saklanmaktadır.

#### Sprint 2 — İlişkisel Refaktör (Enum → FK Geçişi) (Tamamlandı)
- **Referans Tablo Mimarisi**: Sabit Enum yapıları veritabanı seviyesinde bağımsız referans tablolarına taşınmıştır.
- **Foreign Key Entegrasyonu**: Tüm transaction tabloları bu yeni referans tablolarına FK ile bağlanmıştır.

#### Sprint 3 — Performans, Planlama ve Servis Katmanı (Tamamlandı)
- **Servis Katmanı Entegrasyonu**: İş mantığı Controller'lardan ayıklanarak servis sınıflarına taşınmıştır.
- **Aktivite Planlama**: İleriye dönük "PLANNED" statüsü ve ajanda akışı eklenmiştir.
- **Soft Delete**: `ISoftDeletable` arayüzü ile veri silme yerine işaretleme altyapısı kurulmuştur.

#### Sprint 4 — Dinamik Yetkilendirme ve Yönetim (Tamamlandı)
- **Dinamik RBAC**: Modül erişimleri veritabanındaki `RolePermission` tablosu üzerinden canlı yönetilebilir hale getirilmiştir.
- **Admin Master Key**: Sistem yöneticileri için veritabanı hatalarına karşı "Master Key" bypass mekanizması eklenmiştir.
- **Database Transition**: Geliştirme kolaylığı için SQLite altyapısına geçilmiş ve veriler kalıcı hale getirilmiştir.
- **Finansal Lokalizasyon**: Ondalık sayı formatlama sorunları `InvariantCulture` ile çözülmüştür.

#### Sprint 5 — Dashboard ve KPI Güçlendirme (TAMAMLANDI)
- **Görsel Analitik (Parça B)**: `Chart.js` ile trend çizgileri, ürün portföyü ve tahsilat dağılım grafikleri.
- **Performans Matrisi (Parça B)**: 7 kolonlu detaylı personel verimlilik tablosu.
- **Gelişmiş Filtreleme (Parça C)**: Tarih + Personel + Ürün Tipi kompozit filtreleme yapısı.
- **Finansal Raporlama (Parça C)**: Ürün bazlı APE, Prim ve Üretim toplamlarını gösteren derinlemesine finansal analiz tablosu.
- **UX/UI Standartları**: "Dashboard Tab-Navigation" ve "Shared Filter Partial" ile premium arayüz deneyimi.
- **Saha/Ofis Veri Ayrıştırma**: Satışların saha aktiviteleriyle olan dijital bağlantısı (Linkage) üzerinden veri kalitesi ölçülmeye başlanmıştır.

## 20. Backend Önerisi
- ASP.NET Core MVC / Web API + EF Core
- Dinamik RBAC ve Servis Katmanı Mimarisi
- IHttpContextAccessor tabanlı otomatik Audit Log
- Merkezi Hata ve Yetki Yönetimi

## 21. Frontend Önerisi
- Razor Pages & Vanilla JS (InsuranceGridDraft)
- Lucide Icons & Premium CSS Aesthetics
- Dinamik menü ve buton gizleme (Yetki bazlı)
- Responsive ve interaktif Dashboard öğeleri

## 22. Veritabanı Stratejisi

Sistem gelişim aşamalarına göre hibrit bir veritabanı yaklaşımı izler:
- **Development (Geliştirme): SQLite** (Zero-Config, taşınabilirlik ve hızlı geliştirme deneyimi için).
- **Production (Üretim): PostgreSQL** (Yüksek ölçeklenebilirlik, kurumsal güvenlik ve çoklu bağlantı desteği için).
- **Süreç**: Proje tamamlandığında bağlantı dizesi değişikliği ile PostgreSQL'e geçiş hedeflenmektedir.

## 23. Fazlara Ayrılmış Geliştirme Planı

### Faz 0 - Analiz ve Tasarım

- İş kurallarını netleştirme
- Domain modelini oluşturma
- KPI tanımlarını doğrulama
- ERD taslağını hazırlama
- Excel alan analizini yapma

### Faz 1 - Temel Altyapı

- Backend solution yapısı
- Frontend uygulama iskeleti
- Authentication ve rol altyapısı
- Ortak response/validation yapısı

### Faz 2 - Çekirdek Operasyon Modülleri

- Personel yönetimi
- Müşteri/firma yapısı
- Aktivite modülü
- Satış modülü
- Masraf modülü

### Faz 3 - Dashboard ve Raporlama

- Yönetici dashboard
- Personel performans ekranları
- Trend analizleri
- Dönüşüm oranı raporları

### Faz 4 - Veri Geçişi ve Operasyonel Sertleştirme

- Excel import
- Audit log
- Hata yönetimi
- Veri kalite kontrolleri

### Faz 5 - İleri Özellikler

- Onay akışları
- Bildirimler
- Hedef/kota yönetimi
- Gelişmiş analitik

## 24. Riskler ve Dikkat Edilmesi Gereken Noktalar

### 24.1 Domain belirsizlikleri

Müşteri, firma, temas kişisi ve poliçe sahibi kavramları birbirine karışabilir. Başta sade model kurulmalı, sonra veri gerçeklerine göre genişletilmelidir.

### 24.2 KPI tanım riski

Aynı metrikin farklı ekiplerce farklı yorumlanması mümkündür. Özellikle APE, tahsilat, prim, satış adedi ve dönüşüm oranı için net tanım şarttır.

### 24.3 Excel bağımlılığı riski

Excel raporlarını veri modeli yerine koymak, hatalı ve kırılgan mimariye neden olur. Excel yalnızca referans ve geçiş aracı olarak kullanılmalıdır.

### 24.4 Veri kalitesi riski

Aynı müşteri/firma için tekrar eden kayıtlar rapor kalitesini bozar. Duplicate önleme ve veri temizleme yaklaşımı baştan düşünülmelidir.

### 24.5 Yetki ve sahiplik riski

Kullanıcıların kendi verisi ile takım verisi arasında erişim farkı iyi tanımlanmazsa güvenlik ve operasyon sorunları oluşur.

### 24.6 Rapor performansı riski

Dashboard sorguları ileride büyüyebilir. Ham veriden türetilen KPI mantığı korunurken sorgu optimizasyonu planlanmalıdır.

### 24.7 Ürün kapsamı riski

İlk aşamada çok fazla detay eklenirse MVP gecikir. Önce çekirdek akışlar bitirilmeli, ileri analitikler sonra eklenmelidir.

## 25. İlk Karar Prensipleri

Bu proje için başlangıç seviyesinde şu tasarım kararları önerilir:

- Aktivite, satış ve masraf ayrı modüller olarak ele alınacak
- Dashboard verileri işlem kayıtlarından türetilecek
- Excel dosyaları yardımcı kaynak olacak, ana veri kaynağı olmayacak
- MVP önceliği hızlı veri girişi ve temel yönetici görünürlüğü olacak
- Mimari sade tutulacak, gereksiz soyutlamadan kaçınılacak
- Aktivite sonucu domain seviyesinde `ContactStatus` ve `OutcomeStatus` olarak iki ayrı kavramla modellenerek veri kalitesi ve KPI doğruluğu korunacak
