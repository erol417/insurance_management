# Active Tasks

## Current Focus

- MVC uygulamasini kalici veri katmanina yaklastirma
- PostgreSQL migration ve veri kalitesi altyapisini guclendirme
- Audit log, duplicate kontrolu ve operasyonel duzenleme akislarini derinlestirme

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
- `.NET Core MVC` proje iskeleti kuruldu ve uygulama ayaga kaldirildi
- Giris, dashboard, lead, musteri, aktivite, satis, masraf, personel, admin ve import ekranlari calisir hale getirildi
- Arayuz metinleri buyuk olcude Turkcelestirildi
- EF Core veri katmani ve PostgreSQL/InMemory provider secimi eklendi
- `AppDbContext`, seeding ve ilk migration yapisi olusturuldu
- Docker tabanli PostgreSQL ve pgAdmin gelistirme ortami eklendi
- PostgreSQL ile canli baglanti kuruldu, migration ve seed zinciri dogrulandi
- Lead, musteri, aktivite, satis ve masraf modullerinde duzenle/sil akislari eklendi
- Audit log entity, admin audit ekrani ve temel audit kayitlari eklendi
- Lead ve musteri formlarinda duplicate uyari mantigi eklendi
- Import modulu fiziksel dosya yukleme ve indirme akisina tasindi

## Next Up

- Rol/yetki kontrollerini ekran ve aksiyon bazinda sertlestirme
- Validation, duplicate kontrolu ve veri kalite kurallarini diger modullerde genisletme
- Import icin kolon esleme ve satir onizleme adimini ekleme
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

## Open Questions

- Yok. Bloklayici MVP urun kararlari kapatildi.
