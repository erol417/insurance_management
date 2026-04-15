# Active Tasks

## Current Focus

- Raporlama API'larının optimizasyonu ve büyük veri seti testleri
- Excel'e veri aktarımı (Export) ve gelişmiş filtreleme sistemleri
- Mobil uyumluluğun (Responsive) tüm gridlerde mükemmelleştirilmesi

## Completed

- Proje amaci, is problemi ve ana kullanici tipleri tanimlandi
- Ilk urun ve cozum mimarisi yaklasimi `project_design.md` icinde olusturuldu
- Proje calisma anayasasi niteliginde `rules.md` hazirlandi
- Yasayan gorev plani icin `active_tasks.md` ilk surumu olusturuldu
- ERD taslagi olusturuldu
- Onerilen klasor yapisi dokumante edildi
- Backend ve frontend teslim plani cikarildi
- KPI sozlugu olusturuldu
- KPI standardizasyon kararlari belgelendi
- Dashboard API plani olusturuldu
- Dashboard response ornekleri olusturuldu
- Genel API contracts dokumani olusturuldu
- Aktivite sonuc modeli belgelendi
- ERD, activity result kararina gore revize edildi
- Project design dokumani activity result karariyla hizalandi
- DTO katalogu olusturuldu
- Endpoint ile DTO eslesme haritasi olusturuldu
- Lookup stratejisi belgelendi
- Dashboard composite DTO yapisi belgelendi
- Dashboard response ornekleri yeni activity status modeline gore hizalandi
- Dashboard API plani yeni activity filter modeliyle hizalandi
- Uygulanabilir gelistirme gorev plani cikarildi
- Kullanim senaryolari ve operasyonel kullanim akislarI belgelendi
- Kullanici yolculugu haritasi belgelendi
- Onboarding rehberi hazirlandi
- Lead yonetimi tasarimi sisteme dahil edildi
- Lead DTO katalogu cikarildi
- Lead endpoint haritasi cikarildi
- Lead KPI ek notu olusturuldu
- Account modeli MVP karari belgelendi
- Lead donusum kurallari belgelendi
- Satis-aktivite iliski kurali belgelendi
- Urun bazli finansal validation matrisi belgelendi
- Rol bazli gorunurluk karari belgelendi
- Bolge KPI kurali belgelendi
- Aktivite sonuc UI karari belgelendi
- Operations satis duzenleme siniri belgelendi
- Masraf onay karari belgelendi
- BES finansal alan standardi belgelendi
- Rol ve yetki matrisi belgelendi
- Ilk sprint kapsam karari belgelendi
- .NET Core MVC proje iskeleti kuruldu ve uygulama ayaga kaldirildi
- Giris, dashboard, lead, musteri, aktivite, satis, masraf, personel, admin ve import ekranlari calisir hale getirildi
- Arayuz metinleri buyuk olcude Turkcelestirildi
- EF Core veri katmani ve PostgreSQL/InMemory provider secimi eklendi
- AppDbContext, seeding ve ilk migration yapisi olusturuldu
- Docker tabanli PostgreSQL ve pgAdmin gelistirme ortami
- AppDataStore singleton kaldirildi, tum veri erisimi AppDbContext uzerinden saglandi
- BaseEntity abstract sinifi olusturuldu; tum transaction entity'leri (AuditLog haric) bu siniftan turetildi
- CreatedAt, CreatedBy, UpdatedAt, UpdatedBy audit alanlari tum entity'lere eklendi
- Audit alanlari AppDbContext.SaveChanges icerisinde otomatik dolduruluyor
- Sifre hashleme BCrypt ile uygulamaya alindi, plain text sifre kaldirildi
- Migration sifirdan olusturuldu ve PostgreSQL ile dogrulandi
- Controller'lardaki manuel audit atamalari temizlendi
- Dashboard servis tasarimi ve MVP metrik implementasyonu
- Import altyapisi (CSV parser, onizleme gridi, dry-run engine)
- Lead -> Account -> Activity dönüşüm zinciri
- FieldSales (Saha satıs) rolü bazlı veri izolasyonu
- Audit interceptor entegrasyonu (Create/Update logicleri icine) temel audit kayitlari eklendi
- Lead ve musteri formlarinda duplicate uyari mantigi eklendi
- Import modulu fiziksel dosya yukleme ve indirme akisina tasindi
- Referans tablolari olusturuldu (lead_status_types, activity_contact_status_types, activity_outcome_status_types, insurance_product_types, expense_types, lead_source_types)
- Enum -> FK gecisi tamamlandi; tum controller, view ve servisler yeni yapiya tasindi
- Navigation property'ler ve LeadAssignment tablosu eklendi
- Dashboard ve metrik hesaplamalari yeni FK yapisiyla hizalandi
- Veri tabani migration'lari resetlenerek temiz bir iliskisel sema olusturuldu
- **Sprint 3 - Dashboard & Performans & Planlama:**
    - Servis katmanı tam operasyonel hale getirildi (ILeadService, IActivityService, ISaleService, IExpenseService vb.)
    - Personel Performans Dashboard'u (Employees/Details) tüm veri setleriyle (Lead, Satış, Masraf) yayına alındı
    - Rol bazlı veriye erişim sınırı (Admin & Kendi Kaydı) EmployeesController seviyesinde uygulandı
    - Aktivite Planlama Modülü: "PLANNED" statüsü ve Ajanda/Randevu akışı eklendi
    - Lead Not Sistemi: Kronolojik sıra, yazar bilgisi, scrollable UI ve anchor refocus özelliği eklendi
    - UI/Tasarım Fix: Sidebar sünme ve layout stretching sorunları global CSS ile giderildi
    - Soft Delete: Tüm ana tablolarda `ISoftDeletable` ve `DeletedAt/By` alanları üzerinden altyapı kuruldu
    - Validation: Aktivite durumsal doğrulama kuralları (Planlandı/Görüşülemedi durumunda sonuç muafiyeti) eklendi
    - Audit: `AppController.QueueAudit` üzerinden `IAuditService` merkezi loglama yapısı kuruldu
- **Sprint 4 - Dinamik Yetkilendirme ve Yönetim:**
    - **Dinamik RBAC:** Yetki sistemi veritabanına (`RolePermission`) bağlandı; rollerin modül erişimleri canlı yönetilebilir hale getirildi.
    - **Yönetim Paneli:** Kullanıcı ve Rol yönetimi arayüzleri "Premium" tasarım standartlarıyla yenilendi.
    - **Finansal Grid Fix:** TR/EN kültürler arası ondalık sayı (virgül/nokta) görüntüleme sorunu giderildi.
    - **Security Transition:** Veritabanı SQLite'a taşınarak zero-config ve kalıcı veri yapısı sağlandı.
    - **Master Key:** Admin rütbesi için "Süper Anahtar" bypass mekanizması eklendi.
    - **Delegated Imports:** Veri Aktarımı (Import) modülü dinamik yetki tablosuna tam entegre edildi.
- **Sprint 5 — Dashboard ve KPI Güçlendirme (Parça C Tamamlandı)**
    - **Gelişmiş Filtreleme:** Personel ve Ürün Tipi bazlı dropdown filtreleri tüm ekranlara (Executive, Performance, Products, Expenses) entegre edildi.
    - **Yetki Bazlı Filtreleme:** Saha satış personelinin filtrelemesi sadece kendi verileriyle kısıtlandı (Admin/Manager tam yetkili).
    - **Finansal Detay Tablosu:** Ürün bazlı APE, Prim ve Üretim değerlerini ayrıştıran çapraz finansal matris tablosu oluşturuldu.
    - **Kod Standardizasyonu:** Filtreleme bloğu `shared partial` yapıya dönüştürülerek UI tutarlılığı ve bakım kolaylığı sağlandı.
- **Sprint 6 — Lead Detail Hub (360° Lead Sayfası)**
    - **Hub Mimarisi:** Lead'in tüm verilerini (Bilgi, Müşteri, Atama, Randevu, Satış) tek sayfada birleştiren Hub tasarımı yayınlandı.
    - **Analitik Paneller:** Aktivite Timeline formuna (Real-time history) ve Satış özetlerine Lead Hub üzerinden hızlı bakış sağlandı.
    - **Dinamik Aksiyon Çubuğu:** Durum geçiş mekanizmaları Hub üzerinden rol bazlı butonlarla (Ziyaret Başlat, İptal, Planla vb.) fonksiyonel hale getirildi.
    - **Ziyaret Planlama & Çakışma Yönetimi:** Personel ataması yapılarak planlanan ziyaretler (saat, dakika, süre), takvimsel çakışma uyarıları (Conflict checking API) kullanılarak modal/inline arayüz üzerinden sağlandı.
- **Görsel Analitik**: `Chart.js` kütüphanesi ile zenginleştirilmiş ana dashboard:
    - Günlük bazlı Aktivite vs Satış trend çizgileri.
    - Ürün portföyü ve tahsilat dağılımı (Donut/Bar chart).
- **Detaylı Performans Matrisi**: Personel tablosu; "Görüşülen Aktivite" ve "Aktiviteye Bağlı Satış" verimliliği ekseninde 7 kolonlu yapıya taşınmıştır.
- **Navigasyon ve UX**: Ekranlar arası geçiş deneyimi için "Tab-Navigation" yapısı kurulmuştur.
- **Veri Kalitesi Paneli**: Satışların aktiviteyle olan ilişkisi (Linkage) analiz edilerek verinin "saha" çıkışlı olup olmadığı görselleştirilmiştir.

## Next Up

- Import modulu icin kolon esleme ve satir onizleme adimini ekleme
- Personel sifre yenileme ve profil guncelleme ekranlari
- Excel'e veri aktarimi (Export) fonksiyonlari
- Rapor ekranlari (Sube, Bolge, Personel bazli kirilimlar)
- Otomatik test ve yayin hazirligina gecme

## Backlog

- Auth
- Lead management
- Personel yonetimi
- Musteri yonetimi
- Aktivite modulu
- Satis modulu
- Masraf modulu
- Dashboard
- Excel import
- Audit log
- Rapor ekranlari
- Referans veri yonetimi
- Veri kalite kurallari
- Otomatik testler
- Yayin ve ortam yapilandirmasi

## Decisions

- Proje ilk asamada belge ve domain netlestirme odakli ilerleyecek
- Aktivite, satis ve masraf modulleri ayri ama iliskili kurgulanacak
- Dashboard verileri ham islem kayitlarindan turetilecek
- Excel dosyalari referans olarak kullanilacak, ana veri modeli Excel'e gore kurulmayacak
- MVP onceligi cekirdek operasyon akislarinda olacak
- Teknik hedef yigini ASP.NET Core Web API + React TypeScript + PostgreSQL olacak
- MVP icin musteri/firma modeli `Account` merkezli sade yapiyla baslayacak
- Call center -> satis muduru -> saha personeli akisi icin `Lead Management` modulu sisteme dahil edilecek
- Saha personelinin acik isleri atanmis lead kayitlarindan ve takip gerektiren aktivitelerden olusabilecek
- Satisin aktiviteye baglanmasi MVP'de opsiyonel, surec kalitesi acisindan hedeflenen davranis olacak
- Bir aktiviteye bagli birden fazla satis acilabilmesine izin verilecek
- Aktivite sonucu domain seviyesinde `contactStatus` ve `outcomeStatus` olarak iki ayri kavramla ele alinacak
- Aktivite formu once "Gorusuldu mu?" sorusunu soracak, yalnizca gorusulduyse ikinci sonuc alani acilacak
- Satis formu urun tipine gore degisecek ve yalnizca ilgili finansal alanlari zorunlu kilacak
- BES finansal verileri tek alana indirgenmeyecek; tahsilat, APE, toplu para ve aylik odeme ayri tutulacak
- `SALE_CLOSED` sonucu gercek satis kaydinin yerine gecmeyecek; satis dogrulamasi `sales` tablosundan yapilacak
- Saha personeli varsayilan olarak yalnizca kendi kayitlarini gorecek
- Bolge KPI'larinda esas kaynak ziyaret bolgesi olacak
- Operations satis kaydinda yalnizca operasyonel duzeltmeler yapabilecek; cekirdek satis alanlari kontrollu olacak
- MVP'de masraf onay akisi olmayacak, masraflar once kayit ve raporlama ekseninde ilerleyecek
- Ilk sprintte auth ile birlikte temel user/role yonetimi de acilacak
- Geliştirme sürecinde "Zero-Config" ve "Persistence" için SQLite veritabanı kararlaştırıldı.
- Canlı (Production) ortam için PostgreSQL hedefi korunacak; SQLite geliştirme aşamasındaki hızı artırmak içindir.
- Dinamik yetki yönetiminde "Modül Bazlı" kontrol esas alınacak (Dashboard, Leads, Sales vb.).
- Admin rütbesi, sistem bütünlüğü için tüm yetki kısıtlamalarından muaf tutulacak (Hardcoded Master Key).

## Open Questions

- Yok. Bloklayici MVP urun kararlari kapatildi.
