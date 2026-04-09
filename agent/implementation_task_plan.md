# Implementation Task Plan

## 1. Amac

Bu belge, analiz ve tasarim dokumanlarinda netlesen kararlarin uygulanabilir gelistirme islerine donusturulmus halidir. Amac; ekip icin net bir uygulama yol haritasi cikarmak, hangi isin neden yapildigini ve hangi sirayla ilerlemesi gerektigini gostermek, MVP kapsamindaki teknik ve urunsel task'lari bagimliliklariyla birlikte tanimlamaktir.

Bu dokuman:

- Kod yazimindan once uygulanabilir backlog uretir
- Backend, frontend ve shared islerini ayirir
- MVP ve future phase sinirini korur
- Sprint veya faz bazli planlama icin temel olur

## 2. Planlama Yaklasimi

Task'lar asagidaki mantikla gruplanmistir:

- `EPIC`: Buyuk teslim alani
- `TASK`: Uygulanabilir ana is kalemi
- `SUBTASK`: Gerekirse alt teknik parcasi

Her task icin su bilgiler verilir:

- `Kod`
- `Baslik`
- `Amac`
- `Kapsam`
- `Bagimliliklar`
- `Cikti`
- `Alan`
- `Oncelik`
- `MVP/Future`

Oncelik seviyesi:

- `P0`: Bloklayici ve ilk yapilmasi gereken
- `P1`: Cekirdek MVP isi
- `P2`: MVP sonrasi ama yakin vadeli
- `P3`: Future phase

Alan siniflari:

- `Backend`
- `Frontend`
- `Shared`
- `Dashboard`
- `Security`
- `Data`
- `Ops`

## 3. Ust Duzey Epic Listesi

- EPIC-01: Proje iskeleti ve teknik temel
- EPIC-02: Kimlik, kullanici ve yetki yapisi
- EPIC-03: Referans veri ve organizasyon verisi
- EPIC-04: Account ve temas yapisi
- EPIC-04A: Lead ve atama modulu
- EPIC-05: Aktivite modulu
- EPIC-06: Satis modulu
- EPIC-07: Masraf modulu
- EPIC-08: Dashboard ve KPI sorgulari
- EPIC-09: Lookup ve filtre altyapisi
- EPIC-10: Audit, veri kalitesi ve sertlestirme
- EPIC-11: Excel import
- EPIC-12: UAT, canliya hazirlik ve dokumantasyon

## 4. Fazlara Gore Onerilen Uygulama Sirasi

### Faz 1

- Teknik iskelet
- Auth ve temel rol yapisi
- Ortak API/DTO/validation omurgasi

### Faz 2

- Employee
- Account
- Lookup ve referans veri temeli

### Faz 2A

- Lead ve atama modulu

### Faz 3

- Activity modulu

### Faz 4

- Sale modulu

### Faz 5

- Expense modulu

### Faz 6

- Dashboard ve KPI endpoint'leri

### Faz 7

- Audit
- Veri kalitesi sertlestirmesi
- Import yaklasimi

## 5. Detayli Task Listesi

## EPIC-01: Proje Iskeleti ve Teknik Temel

### TASK-001

- Kod: `TASK-001`
- Baslik: Backend solution ve katmanli iskeleti kurma
- Amac: ASP.NET Core Web API tarafinda domain, application, infrastructure ve api katmanlarini ayirmak
- Kapsam: solution yapisi, proje referanslari, temel startup konfigurasyonu
- Bagimliliklar: Yok
- Cikti: Calisan bos backend iskeleti
- Alan: `Backend`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-002

- Kod: `TASK-002`
- Baslik: Frontend React uygulama iskeletini kurma
- Amac: Feature bazli klasor yapisina sahip TypeScript frontend iskeleti olusturmak
- Kapsam: routing, layout kabugu, ortak servis katmani, temel state ve http yapisi
- Bagimliliklar: Yok
- Cikti: Calisan bos frontend iskeleti
- Alan: `Frontend`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-003

- Kod: `TASK-003`
- Baslik: Ortak API response ve hata standardini uygulama omurgasina tasima
- Amac: `api_contracts.md` kararlarini kod seviyesinde uygulanabilir hale getirmek
- Kapsam: response envelope, hata modeli, validation response standardi
- Bagimliliklar: `TASK-001`
- Cikti: Ortak response/hata altyapisi
- Alan: `Shared`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-004

- Kod: `TASK-004`
- Baslik: PostgreSQL baglanti ve migration omurgasini hazirlama
- Amac: Veritabaniyla kontrollu gelistirme sureci kurmak
- Kapsam: db connection, migration akisi, base configuration
- Bagimliliklar: `TASK-001`
- Cikti: Veritabani baglantili backend altyapisi
- Alan: `Data`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-005

- Kod: `TASK-005`
- Baslik: Ortak audit alanlari ve soft delete kararini temel entity seviyesine tasima
- Amac: Tum transaction kayitlarinda denetlenebilir temel alanlari hazirlamak
- Kapsam: createdAt, createdBy, updatedAt, updatedBy, optional deletedAt, deletedBy
- Bagimliliklar: `TASK-004`
- Cikti: Base entity ve audit omurgasi
- Alan: `Shared`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-02: Kimlik, Kullanici ve Yetki Yapisi

### TASK-006

- Kod: `TASK-006`
- Baslik: Login ve oturum dogrulama altyapisini kurma
- Amac: Sisteme guvenli giris saglamak
- Kapsam: login endpoint, token yapisi, authenticated user response
- Bagimliliklar: `TASK-001`, `TASK-003`
- Cikti: Calisan auth giris akisi
- Alan: `Security`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-007

- Kod: `TASK-007`
- Baslik: User ve role temel veri modelini kurma
- Amac: Uygulama kullanicilarini ve rollerini yonetebilmek
- Kapsam: user entity, role entity, iliski yapisi
- Bagimliliklar: `TASK-004`, `TASK-006`
- Cikti: User/role veri modeli
- Alan: `Security`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-008

- Kod: `TASK-008`
- Baslik: Rol bazli yetki matrisi dokumani ve ilk uygulama kurali
- Amac: Admin, Manager, Operations, FieldSales rol sinirlarini netlestirmek
- Kapsam: endpoint bazli gorunurluk, veri scope kurallari
- Bagimliliklar: `TASK-007`
- Cikti: Rol/yetki matrisi ve temel authorization kurallari
- Alan: `Security`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-009

- Kod: `TASK-009`
- Baslik: Frontend login ve oturum kabugunu tamamlama
- Amac: Kimlik dogrulama sonrasi uygulama icine girisi saglamak
- Kapsam: login ekrani, session saklama, route protection
- Bagimliliklar: `TASK-002`, `TASK-006`
- Cikti: Calisan login deneyimi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-03: Referans Veri ve Organizasyon Verisi

### TASK-010

- Kod: `TASK-010`
- Baslik: Employee veri modeli ve backend CRUD akisini kurma
- Amac: Personel bilgisini sistemde yonetmek
- Kapsam: entity, migration, DTO, CRUD endpoint'leri
- Bagimliliklar: `TASK-004`, `TASK-003`
- Cikti: Employee backend modulu
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-011

- Kod: `TASK-011`
- Baslik: Employee frontend ekranlarini kurma
- Amac: Personel listeleme, detay ve form ekranlarini hazirlamak
- Kapsam: list, detail, create, edit ekranlari
- Bagimliliklar: `TASK-002`, `TASK-010`
- Cikti: Employee frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-012

- Kod: `TASK-012`
- Baslik: Sabit lookup veri modelini ve endpoint'lerini kurma
- Amac: Urun tipi, masraf tipi, activity contact status ve activity outcome status verilerini standart sunmak
- Kapsam: lookup tabloları veya seed mantigi, merkezi lookup endpoint'leri
- Bagimliliklar: `TASK-004`
- Cikti: Calisan lookup temel altyapisi
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-04: Account ve Temas Yapisi

### TASK-013

- Kod: `TASK-013`
- Baslik: Account modeli icin nihai MVP alan kararini netlestirme
- Amac: Individual ve Corporate account zorunlu alanlarini sonlandirmak
- Kapsam: open question kapanisi, validation kural seti
- Bagimliliklar: Yok
- Cikti: Net alan ve validation karari
- Alan: `Shared`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-014

- Kod: `TASK-014`
- Baslik: Account backend CRUD akisini kurma
- Amac: Musteri/firma verisini sistemde yonetmek
- Kapsam: entity, migration, duplicate kontrol kurallari, CRUD endpoint'leri
- Bagimliliklar: `TASK-013`, `TASK-003`, `TASK-004`
- Cikti: Account backend modulu
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-015

- Kod: `TASK-015`
- Baslik: Account contact yapisini kurma
- Amac: Kurumsal hesaplar altinda temas kisilerini yonetmek
- Kapsam: account contact entity, iliski, CRUD veya inline giris davranisi
- Bagimliliklar: `TASK-014`
- Cikti: Account contact backend omurgasi
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-016

- Kod: `TASK-016`
- Baslik: Account frontend ekranlarini kurma
- Amac: Account listeleme, detay ve form akisini tamamlamak
- Kapsam: list, detail, create, edit, duplicate uyari davranisi
- Bagimliliklar: `TASK-002`, `TASK-014`
- Cikti: Account frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-05: Aktivite Modulu

### TASK-017

- Kod: `TASK-017`
- Baslik: Activity result UI ve validation kararlari sonlandirma
- Amac: Aktivite girisinde tek secim mi iki asamali secim mi kullanilacagini kapatmak
- Kapsam: `contactStatus` ve `outcomeStatus` giris davranisi, validation kurallari
- Bagimliliklar: Yok
- Cikti: Net UI ve is kurali karari
- Alan: `Shared`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-018

- Kod: `TASK-018`
- Baslik: Activity backend veri modeli ve CRUD akisini kurma
- Amac: Saha aktivitelerini kaydetmek ve filtrelemek
- Kapsam: entity, migration, DTO, filter, CRUD endpoint'leri
- Bagimliliklar: `TASK-014`, `TASK-012`, `TASK-017`
- Cikti: Activity backend modulu
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-019

- Kod: `TASK-019`
- Baslik: Activity frontend ekranlarini kurma
- Amac: Aktivite olusturma, listeleme, detay ve guncelleme deneyimini sunmak
- Kapsam: form, list, detail, filter, account selection
- Bagimliliklar: `TASK-002`, `TASK-018`
- Cikti: Activity frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-06: Satis Modulu

### TASK-020

- Kod: `TASK-020`
- Baslik: Satis-aktivite iliski kurallarini sonlandirma
- Amac: Aktiviteye bagli ve bagimsiz satis davranisini netlestirmek
- Kapsam: nullable `activityId`, zorunlu warning/justification ihtiyaci, 1-1 veya 1-N tartismasi
- Bagimliliklar: Yok
- Cikti: Net satis baglanti kurallari
- Alan: `Shared`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-021

- Kod: `TASK-021`
- Baslik: Satis urun tipine gore finansal validation kurallarini sonlandirma
- Amac: BES, Hayat, Saglik, Seyahat ve Diger icin zorunlu finansal alanlari netlestirmek
- Kapsam: APE, toplu para, tahsilat, aylik odeme, prim kurallari
- Bagimliliklar: Yok
- Cikti: Net urun bazli validation matrisi
- Alan: `Shared`
- Oncelik: `P0`
- MVP/Future: `MVP`

### TASK-022

- Kod: `TASK-022`
- Baslik: Sale backend veri modeli ve CRUD akisini kurma
- Amac: Satis ve finansal uretim verisini sistemde takip etmek
- Kapsam: entity, migration, DTO, CRUD, product-based validation
- Bagimliliklar: `TASK-018`, `TASK-020`, `TASK-021`
- Cikti: Sale backend modulu
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-023

- Kod: `TASK-023`
- Baslik: Sale frontend ekranlarini kurma
- Amac: Satis olusturma, listeleme, detay ve duzenleme ekranlarini hazirlamak
- Kapsam: urun bazli dinamik alanlar, activity relation secimi
- Bagimliliklar: `TASK-002`, `TASK-022`
- Cikti: Sale frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-07: Masraf Modulu

### TASK-024

- Kod: `TASK-024`
- Baslik: Expense backend veri modeli ve CRUD akisini kurma
- Amac: Personel bazli masraf kaydini standardize etmek
- Kapsam: entity, migration, DTO, CRUD, filtreler
- Bagimliliklar: `TASK-010`, `TASK-012`
- Cikti: Expense backend modulu
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-025

- Kod: `TASK-025`
- Baslik: Expense frontend ekranlarini kurma
- Amac: Masraf formu, listeleme ve detay deneyimini sunmak
- Kapsam: create, edit, list, filter
- Bagimliliklar: `TASK-002`, `TASK-024`
- Cikti: Expense frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-08: Dashboard ve KPI Sorgulari

### TASK-026

- Kod: `TASK-026`
- Baslik: Dashboard ortak filtre request modelini modullerle hizalama
- Amac: Dashboard filtreleri ile CRUD filtrelerini tutarli hale getirmek
- Kapsam: date range, employee, region, product, activity status, linked/unlinked satis filtreleri
- Bagimliliklar: `TASK-018`, `TASK-022`, `TASK-024`
- Cikti: Uygulanabilir dashboard filter contract
- Alan: `Dashboard`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-027

- Kod: `TASK-027`
- Baslik: Dashboard summary ve executive overview endpoint'lerini kurma
- Amac: Ust KPI kartlarini ve yonetici ozet ekranini sunmak
- Kapsam: summary cards, executive overview composite response
- Bagimliliklar: `TASK-022`, `TASK-024`, `TASK-026`
- Cikti: Ilk dashboard endpoint seti
- Alan: `Dashboard`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-028

- Kod: `TASK-028`
- Baslik: Activity dashboard sorgularini kurma
- Amac: Aktivite trendi, contact status ve outcome status kirilimlarini uretmek
- Kapsam: activities dashboard endpoint ve KPI hesaplari
- Bagimliliklar: `TASK-018`, `TASK-026`
- Cikti: Activity dashboard backend
- Alan: `Dashboard`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-029

- Kod: `TASK-029`
- Baslik: Sales ve financial dashboard sorgularini kurma
- Amac: Satis adedi, urun kirilimi ve finansal uretim KPI'larini uretmek
- Kapsam: sales dashboard, financial dashboard
- Bagimliliklar: `TASK-022`, `TASK-026`
- Cikti: Sales ve financial dashboard backend
- Alan: `Dashboard`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-030

- Kod: `TASK-030`
- Baslik: Expense, conversion ve performance dashboard sorgularini kurma
- Amac: Masraf, donusum ve personel performans KPI'larini uretmek
- Kapsam: expenses, conversions, performance endpoint'leri
- Bagimliliklar: `TASK-024`, `TASK-022`, `TASK-018`, `TASK-026`
- Cikti: Kalan dashboard endpoint'leri
- Alan: `Dashboard`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-031

- Kod: `TASK-031`
- Baslik: Dashboard frontend ekranlarini kurma
- Amac: KPI kartlari, grafikler ve performans tablosunu arayuzde gostermek
- Kapsam: executive dashboard, analysis ekranlari, filtre paneli
- Bagimliliklar: `TASK-027`, `TASK-028`, `TASK-029`, `TASK-030`
- Cikti: Dashboard frontend akisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-09: Lookup ve Filtre Altyapisi

### TASK-032

- Kod: `TASK-032`
- Baslik: Merkezi lookup endpoint setini tamamlama
- Amac: Form ve filtre ekranlarinin tekil ve tutarli lookup kaynaklari kullanmasini saglamak
- Kapsam: employees, accounts, product types, expense types, activity statuses, region/city/district
- Bagimliliklar: `TASK-010`, `TASK-012`, `TASK-014`, `TASK-015`
- Cikti: Lookup backend seti
- Alan: `Backend`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-033

- Kod: `TASK-033`
- Baslik: Frontend ortak filtre ve lookup tuketim altyapisini kurma
- Amac: Tum modullerde benzer filtre ve autocomplete davranisi saglamak
- Kapsam: common filter bar, lookup hooks/services, cache davranisi
- Bagimliliklar: `TASK-002`, `TASK-032`
- Cikti: Tekrar kullanilabilir frontend lookup/filter yapisi
- Alan: `Frontend`
- Oncelik: `P1`
- MVP/Future: `MVP`

## EPIC-10: Audit, Veri Kalitesi ve Sertlestirme

### TASK-034

- Kod: `TASK-034`
- Baslik: Audit log kapsam kararini netlestirme
- Amac: Hangi islemlerin audit altina alinacagini belirlemek
- Kapsam: create/update/delete ve kritik alan degisiklikleri
- Bagimliliklar: Yok
- Cikti: Audit kapsam karari
- Alan: `Shared`
- Oncelik: `P2`
- MVP/Future: `MVP`

### TASK-035

- Kod: `TASK-035`
- Baslik: Audit log veri modeli ve listeleme endpoint'ini kurma
- Amac: Kritik islem izlerini yonetici seviyesinde gorunur kilmak
- Kapsam: audit entity, yazim mekanizmasi, liste/detail endpoint'leri
- Bagimliliklar: `TASK-034`, `TASK-005`
- Cikti: Audit backend modulu
- Alan: `Backend`
- Oncelik: `P2`
- MVP/Future: `Future`

### TASK-036

- Kod: `TASK-036`
- Baslik: Veri kalitesi warning ve duplicate kontrol mekanizmalarini sertlestirme
- Amac: Dashboard ve operasyon ekranlarinda veri kalitesi sinyallerini guclendirmek
- Kapsam: duplicate account warning, incomplete financial warning, unlinked sale warning
- Bagimliliklar: `TASK-014`, `TASK-022`, `TASK-027`
- Cikti: Veri kalitesi kurallari ve warning davranislari
- Alan: `Shared`
- Oncelik: `P2`
- MVP/Future: `MVP`

## EPIC-11: Excel Import

### TASK-037

- Kod: `TASK-037`
- Baslik: Excel import stratejisini sonlandirma
- Amac: Gecmis Excel verilerinin tam import mu, analiz referansi mi olacagini netlestirmek
- Kapsam: kaynak dosya siniflandirmasi, mapping, kalite riski
- Bagimliliklar: Yok
- Cikti: Net import strateji karari
- Alan: `Data`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-038

- Kod: `TASK-038`
- Baslik: Import validation akisini tasarlama
- Amac: Dosya yuklenmeden once satir ve alan bazli dogrulama yapabilmek
- Kapsam: import batch, row error modeli, validation response
- Bagimliliklar: `TASK-037`
- Cikti: Import validation tasarimi
- Alan: `Data`
- Oncelik: `P2`
- MVP/Future: `Future`

### TASK-039

- Kod: `TASK-039`
- Baslik: Excel import backend akisini gelistirme
- Amac: Kontrollu veri alimi saglamak
- Kapsam: validate, commit, history endpoint'leri
- Bagimliliklar: `TASK-038`
- Cikti: Import backend modulu
- Alan: `Backend`
- Oncelik: `P3`
- MVP/Future: `Future`

## EPIC-12: UAT, Canliya Hazirlik ve Dokumantasyon

### TASK-040

- Kod: `TASK-040`
- Baslik: MVP kabul senaryolarini cikarma
- Amac: Is birimiyle kabul kriterlerini ortaklastirmak
- Kapsam: activity, sale, expense, dashboard user journeys
- Bagimliliklar: `TASK-018`, `TASK-022`, `TASK-024`, `TASK-031`
- Cikti: UAT senaryo seti
- Alan: `Shared`
- Oncelik: `P1`
- MVP/Future: `MVP`

### TASK-041

- Kod: `TASK-041`
- Baslik: Seed veri ve demo ortam hazirligi
- Amac: Test ve demo surecini hizlandirmak
- Kapsam: sample employees, accounts, activities, sales, expenses
- Bagimliliklar: `TASK-018`, `TASK-022`, `TASK-024`
- Cikti: Demo veri seti
- Alan: `Ops`
- Oncelik: `P2`
- MVP/Future: `MVP`

### TASK-042

- Kod: `TASK-042`
- Baslik: Teknik teslim checklist'i ve canliya gecis hazirligi
- Amac: MVP yayinina yakin temel kalite kapilarini tanimlamak
- Kapsam: env config, migration, basic smoke tests, rollback notlari
- Bagimliliklar: `TASK-040`, `TASK-041`
- Cikti: Release readiness checklist
- Alan: `Ops`
- Oncelik: `P2`
- MVP/Future: `MVP`

## 6. Ilk Sprint Onerisi

Sprint 1:

- `TASK-001`
- `TASK-002`
- `TASK-003`
- `TASK-004`
- `TASK-006`

Sprint 2:

- `TASK-007`
- `TASK-008`
- `TASK-009`
- `TASK-010`
- `TASK-012`
- `TASK-013`

Sprint 3:

- `TASK-014`
- `TASK-015`
- `TASK-016`
- `TASK-017`
- `TASK-018`

Sprint 4:

- `TASK-019`
- `TASK-020`
- `TASK-021`
- `TASK-022`
- `TASK-023`

Sprint 5:

- `TASK-024`
- `TASK-025`
- `TASK-026`
- `TASK-027`
- `TASK-028`

Sprint 6:

- `TASK-029`
- `TASK-030`
- `TASK-031`
- `TASK-032`
- `TASK-033`
- `TASK-040`

Sprint 7:

- `TASK-036`
- `TASK-037`
- `TASK-041`
- `TASK-042`

## 7. Kritik Bloklayicilar

- `TASK-013`: Account zorunlu alan kararinin kapanmasi
- `TASK-017`: Activity result UI davranisinin kapanmasi
- `TASK-020`: Satis-aktivite iliski kuralinin kapanmasi
- `TASK-021`: Urun bazli finansal validation matrisinin kapanmasi
- `TASK-037`: Excel import stratejisinin netlesmesi

Bu bes baslik gecikirse uygulama task'larinda tekrar isleme ve refactor riski artar.

## 8. MVP Disi Ama Planli Isler

- Refresh/logout auth genislemeleri
- Audit log ekranlari
- Import validation ve commit akisinin tamamlanmasi
- Gelismis bolge ve funnel dashboard'lari
- Profitability metrikleri
- Referans veri yonetim ekranlari

## 9. Sonuc

Bu plan ile proje artik belge seviyesinden uygulama seviyesine gecmeye hazirdir. En dogru ilerleme sirasi:

- once iskelet
- sonra cekirdek transaction modulleri
- sonra dashboard
- en son import, audit ve sertlestirme

olarak korunmalidir.

## 10. Lead Modulu Eki

Lead management modulu sisteme eklendigi icin MVP uygulama sirasina su ek task grubu da dahil edilmelidir:

- `TASK-016A`: Lead veri modeli ve status kurallari
- `TASK-016B`: Lead backend CRUD ve atama akisı
- `TASK-016C`: Lead frontend ekranlari

Bu grup, activity modulunden once gelmelidir. Cunku saha personelinin acik isi artik lead atamalarindan dogmaktadir.

## 11. Kapanan Urun Kararlari Eki

Asagidaki bloklayici urun kararlari artik kapanmistir ve uygulama tarafinda sabit kabul edilecektir:

- `Account` modeli MVP'de ortak yapiyla ilerleyecek, `Individual` ve `Corporate` icin minimum ayrisma uygulanacak
- Lead, saha oncesinde varsayilan olarak `Account` olmayacak; donusum zinciri `Lead -> Account + Activity -> Sale` olacak
- Aktivite formu iki asamali akista calisacak: once "Gorusuldu mu?", sonra gerekirse gorusme sonucu
- Bir aktiviteye bagli birden fazla satis acilabilecek
- Satis formu urun tipine gore degisecek
- BES finansal alanlari tek tutara indirgenmeyecek; ayri metrikler halinde tutulacak
- `FieldSales` varsayilan olarak yalnizca kendi kayitlarini gorecek
- Bolge KPI'lari ziyaret bolgesine gore hesaplanacak
- Operations satis kaydinda yalnizca operasyonel duzeltmeler yapabilecek
- MVP'de masraf onay akisi olmayacak
- Ilk sprintte auth ile birlikte temel user/role yonetimi de acilacak

## 12. Uretime Gecis Notu

Belge seviyesindeki kritik MVP kararlarinin kapanmis olmasi nedeniyle proje artik uygulama iskeleti asamasina gecmeye hazirdir.

Onerilen fiili baslangic sirasi:

- `TASK-001`
- `TASK-002`
- `TASK-003`
- `TASK-004`
- `TASK-006`
- `TASK-007`
- `TASK-009`

Bu siradan sonra lead, account ve activity modullerine gecis daha dusuk refactor riskiyle yapilabilir.
