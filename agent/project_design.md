# Insurance Management Project Design

## 1. Projenin AmacÄ± ve Temel Vizyonu

Bu projenin amacÄ±, sigorta ÅŸirketinin saha satÄ±ÅŸ ve operasyon sÃ¼reÃ§lerini tek merkezden takip edebileceÄŸi web tabanlÄ± bir uygulama oluÅŸturmaktÄ±r. Sistem; saha personelinin gÃ¼nlÃ¼k ziyaretlerini, mÃ¼ÅŸteri/firma gÃ¶rÃ¼ÅŸmelerini, bu gÃ¶rÃ¼ÅŸmelerin sonuÃ§larÄ±nÄ±, satÄ±ÅŸ dÃ¶nÃ¼ÅŸÃ¼mlerini ve yapÄ±lan masraflarÄ± kayÄ±t altÄ±na almalÄ±; yÃ¶neticilere de operasyonel karar almayÄ± destekleyen raporlar ve dashboard'lar sunmalÄ±dÄ±r.

Ana hedef, daÄŸÄ±nÄ±k Excel takibi yerine sÃ¼rdÃ¼rÃ¼lebilir, Ã¶lÃ§Ã¼lebilir ve denetlenebilir bir veri yapÄ±sÄ± kurmaktÄ±r. Uygulama yalnÄ±zca veri giriÅŸi yapan bir ekran seti deÄŸil, saha aktivitesinden satÄ±ÅŸa dÃ¶nÃ¼ÅŸÃ¼mÃ¼n izlenebildiÄŸi bir yÃ¶netim aracÄ± olmalÄ±dÄ±r.

## 2. Ä°ÅŸ Problemi

Mevcut iÅŸleyiÅŸte saha ekiplerinin yaptÄ±ÄŸÄ± aktiviteler, satÄ±ÅŸ Ã§Ä±ktÄ±larÄ± ve masraflar bÃ¼yÃ¼k Ã¶lÃ§Ã¼de manuel veya parÃ§alÄ± ÅŸekilde takip edilmektedir. Bu durum aÅŸaÄŸÄ±daki sorunlara yol aÃ§ar:

- AynÄ± mÃ¼ÅŸteri veya firma ile yapÄ±lan geÃ§miÅŸ gÃ¶rÃ¼ÅŸmelerin izlenmesi zorlaÅŸÄ±r.
- Saha aktivitesinin gerÃ§ek satÄ±ÅŸ performansÄ±na etkisi net gÃ¶rÃ¼lemez.
- Personel verimliliÄŸi sadece satÄ±ÅŸ adediyle Ã¶lÃ§Ã¼lÃ¼r, dÃ¶nÃ¼ÅŸÃ¼m ve maliyet iliÅŸkisi kaybolur.
- YÃ¶netici raporlarÄ± gecikmeli, manuel ve hataya aÃ§Ä±k hazÄ±rlanÄ±r.
- Excel dosyalarÄ± Ã¶zet veri iÃ§erdiÄŸinde, geriye dÃ¶nÃ¼k detay analizi sÄ±nÄ±rlanÄ±r.
- Denetim izi ve veri gÃ¼venilirliÄŸi dÃ¼ÅŸer.

Bu sistemle Ã§Ã¶zÃ¼lmek istenen ana problem, operasyonel ham veriyi standartlaÅŸtÄ±rmak ve bu ham veriden doÄŸru KPI'lar tÃ¼retebilen bir yapÄ± kurmaktÄ±r.

## 3. Ä°ÅŸ Hedefleri

- Saha personelinin gÃ¼nlÃ¼k aktivite kayÄ±tlarÄ±nÄ± standart hale getirmek
- Aktivite, satÄ±ÅŸ ve masraf verilerini iliÅŸkili biÃ§imde tutmak
- YÃ¶netici iÃ§in hÄ±zlÄ± karar destek ekranlarÄ± Ã¼retmek
- Personel bazlÄ± performansÄ± adet, tutar, verimlilik ve maliyet perspektifleriyle izlemek
- Excel baÄŸÄ±mlÄ±lÄ±ÄŸÄ±nÄ± azaltmak ve ileride sistemin ana veri kaynaÄŸÄ±nÄ± uygulamanÄ±n kendisi yapmak
- GeÃ§miÅŸ operasyon verisini analiz edilebilir yapÄ±da saklamak

## 4. KullanÄ±cÄ± Tipleri

### 4.1 Saha Personeli

Sahada mÃ¼ÅŸteri/firma ziyareti yapan, aktivite oluÅŸturan, gerekiyorsa satÄ±ÅŸ kaydÄ± ve masraf kaydÄ± giren kullanÄ±cÄ±dÄ±r.

Beklenen ihtiyaÃ§larÄ±:

- HÄ±zlÄ± aktivite giriÅŸi
- Ziyaret geÃ§miÅŸini gÃ¶rÃ¼ntÃ¼leme
- SatÄ±ÅŸ ve masraf kayÄ±tlarÄ±nÄ± kolayca ekleme
- Kendi performansÄ±nÄ± gÃ¶rebilme

### 4.2 Call Center KullanÄ±cÄ±sÄ±

Potansiyel mÃ¼ÅŸteri araÅŸtÄ±rmasÄ± yapan, iletiÅŸim bilgisi bulan ve lead havuzu oluÅŸturan kullanÄ±cÄ±dÄ±r.

Beklenen ihtiyaÃ§larÄ±:

- Yeni lead kaydÄ± aÃ§ma
- Potansiyel mÃ¼ÅŸteri notu ve iletiÅŸim bilgisi girme
- Lead listesini gÃ¼ncelleme
- Atamaya hazÄ±r kayÄ±tlarÄ± hazÄ±rlama

### 4.3 Operasyon KullanÄ±cÄ±sÄ±

Saha ekibinden gelen verileri kontrol eden, gerektiÄŸinde satÄ±ÅŸ veya mÃ¼ÅŸteri kayÄ±tlarÄ±nÄ± dÃ¼zenleyen, veri kalitesini takip eden kullanÄ±cÄ±dÄ±r.

Beklenen ihtiyaÃ§larÄ±:

- SatÄ±ÅŸ kayÄ±tlarÄ±nÄ± doÄŸrulama/dÃ¼zenleme
- MÃ¼ÅŸteri/firma kayÄ±tlarÄ±nÄ± konsolide etme
- Eksik veya hatalÄ± giriÅŸleri tespit etme
- Excel import sÃ¼reÃ§lerini yÃ¶netme

### 4.4 SatÄ±ÅŸ MÃ¼dÃ¼rÃ¼

Lead havuzunu deÄŸerlendiren, saha personeline atama yapan ve saha iÅŸ yÃ¼kÃ¼nÃ¼ yÃ¶neten kullanÄ±cÄ±dÄ±r.

Beklenen ihtiyaÃ§larÄ±:

- Atama bekleyen lead havuzunu gÃ¶rme
- Lead'leri saha personeline atama
- Personel bazlÄ± aÃ§Ä±k iÅŸ yoÄŸunluÄŸunu izleme
- Lead'den aktiviteye dÃ¶nÃ¼ÅŸÃ¼m kalitesini izleme

### 4.5 YÃ¶netici

TakÄ±mÄ±n genel performansÄ±nÄ±, satÄ±ÅŸ sonuÃ§larÄ±nÄ±, masraf yapÄ±sÄ±nÄ± ve trendleri izleyen karar vericidir.

Beklenen ihtiyaÃ§larÄ±:

- Dashboard ve Ã¶zet raporlar
- Tarih, Ã¼rÃ¼n, personel ve bÃ¶lge bazlÄ± filtreleme
- Aktiviteden satÄ±ÅŸa dÃ¶nÃ¼ÅŸÃ¼m oranÄ±
- Masraf/satÄ±ÅŸ iliÅŸkisi
- HÄ±zlÄ± performans karÅŸÄ±laÅŸtÄ±rmalarÄ±

### 4.6 Sistem YÃ¶neticisi

Rol, yetki, tanÄ±m tablolarÄ± ve sistemsel ayarlarÄ± yÃ¶neten kullanÄ±cÄ±dÄ±r.

Beklenen ihtiyaÃ§larÄ±:

- KullanÄ±cÄ±/rol yÃ¶netimi
- Referans veri yÃ¶netimi
- Log ve audit kayÄ±tlarÄ±na eriÅŸim

## 5. Temel KullanÄ±m SenaryolarÄ±

### 5.0 Lead Toplama ve Atama

Call center potansiyel mÃ¼ÅŸteri araÅŸtÄ±rmasÄ± yapar, lead kaydÄ± oluÅŸturur ve iletiÅŸim notlarÄ±nÄ± ekler. SatÄ±ÅŸ mÃ¼dÃ¼rÃ¼ bu lead'leri saha personeline atar. Saha personelinin gÃ¼nlÃ¼k saha iÅŸi bu atanan lead listesinden oluÅŸur.

### 5.1 GÃ¼nlÃ¼k Aktivite GiriÅŸi

Saha personeli, ziyaret ettiÄŸi mÃ¼ÅŸteri veya firmayÄ± seÃ§er ya da yeni kayÄ±t oluÅŸturur. Ziyaret tarihini, gÃ¶rÃ¼ÅŸme iÃ§eriÄŸini, temas durumunu ve gerekiyorsa gÃ¶rÃ¼ÅŸmenin ticari sonucunu kaydeder.

### 5.2 Aktiviteden SatÄ±ÅŸ OluÅŸturma

Bir aktivite olumlu sonuÃ§landÄ±ÄŸÄ±nda veya doÄŸrudan satÄ±ÅŸa dÃ¶nÃ¼ÅŸtÃ¼ÄŸÃ¼nde, ilgili aktivite Ã¼zerinden satÄ±ÅŸ kaydÄ± aÃ§Ä±lÄ±r. BÃ¶ylece satÄ±ÅŸÄ±n hangi saha hareketinden Ã¼retildiÄŸi izlenebilir.

### 5.3 BaÄŸÄ±msÄ±z SatÄ±ÅŸ KaydÄ±

BazÄ± durumlarda satÄ±ÅŸ daha sonra operasyon tarafÄ±ndan girilebilir. Bu senaryoda satÄ±ÅŸ kaydÄ± aktivite ile iliÅŸkilendirilmeye Ã§alÄ±ÅŸÄ±lÄ±r; iliÅŸki kurulamazsa gerekÃ§eli ÅŸekilde baÄŸÄ±msÄ±z satÄ±ÅŸ olarak tutulur.

### 5.4 Masraf GiriÅŸi

Saha personeli tarih bazlÄ± yol, yemek, konaklama veya diÄŸer masraflarÄ± kaydeder. Masraf kayÄ±tlarÄ± personel bazlÄ± izlenir ve raporlamaya dahil edilir.

### 5.5 YÃ¶netici Analizi

YÃ¶netici belirli bir tarih aralÄ±ÄŸÄ±nda toplam aktivite, toplam satÄ±ÅŸ, Ã¼rÃ¼n kÄ±rÄ±lÄ±mÄ±, personel performansÄ±, dÃ¶nÃ¼ÅŸÃ¼m oranÄ± ve masraf trendlerini inceler.

### 5.6 Excel Referans Verisi ile Ä°lk Uyumlama

Mevcut Excel dosyalarÄ± ilk analiz aÅŸamasÄ±nda referans olarak kullanÄ±lÄ±r. AmaÃ§, sistemde tutulacak ham veriyi ve KPI tanÄ±mlarÄ±nÄ± bu raporlara bakarak doÄŸrulamak; sistemi Excel Ã§Ä±ktÄ±sÄ±na baÄŸÄ±mlÄ± kÄ±lmamak olacaktÄ±r.

## 6. MVP KapsamÄ±

MVP, temel operasyonel akÄ±ÅŸlarÄ±n Ã§alÄ±ÅŸtÄ±ÄŸÄ± ve yÃ¶neticiye ilk anlamlÄ± raporlarÄ±n sunulduÄŸu sÃ¼rÃ¼m olmalÄ±dÄ±r.

### MVP iÃ§inde olmasÄ± gerekenler

- KullanÄ±cÄ± giriÅŸi ve temel rol ayrÄ±mÄ±
- Call center tarafÄ±ndan lead havuzu oluÅŸturma
- SatÄ±ÅŸ mÃ¼dÃ¼rÃ¼ tarafÄ±ndan lead atama
- Personel tanÄ±mÄ±
- MÃ¼ÅŸteri/firma temel kayÄ±t yapÄ±sÄ±
- Aktivite kayÄ±t oluÅŸturma, listeleme ve detay gÃ¶rÃ¼ntÃ¼leme
- SatÄ±ÅŸ kayÄ±t oluÅŸturma, listeleme ve aktivite iliÅŸkisi
- Masraf kayÄ±t oluÅŸturma ve listeleme
- Temel dashboard metrikleri
- Tarih/personel/Ã¼rÃ¼n bazlÄ± filtreleme
- Excel import iÃ§in ilk sÃ¼rÃ¼m yaklaÅŸÄ±mÄ± veya hazÄ±rlÄ±k altyapÄ±sÄ±
- Audit alanlarÄ± (created by, created at, updated by, updated at)

### MVP baÅŸarÄ± kriterleri

- Saha personeli gÃ¼nlÃ¼k veriyi sisteme girebilmeli
- YÃ¶netici haftalÄ±k ve aylÄ±k performansÄ± gÃ¶rÃ¼ntÃ¼leyebilmeli
- Aktivite-satÄ±ÅŸ iliÅŸkisi Ã¶lÃ§Ã¼lebilmeli
- Toplam masraf ve personel baÅŸÄ±na masraf gÃ¶rÃ¼lebilmeli

## 7. MVP DÄ±ÅŸÄ± Ama Planlanan Ã–zellikler

- GeliÅŸmiÅŸ onay akÄ±ÅŸlarÄ±
- Masraf onay sÃ¼reci
- GeliÅŸmiÅŸ hedef yÃ¶netimi ve kota takibi
- BÃ¶lge/segment bazlÄ± performans analizi
- MÃ¼ÅŸteri temas geÃ§miÅŸi zaman Ã§izelgesi
- Bildirim ve hatÄ±rlatma altyapÄ±sÄ±
- Mobil uyumun Ã¶tesine geÃ§en PWA veya native entegrasyon
- Dosya/fiÅŸ ekleme
- Harita tabanlÄ± ziyaret gÃ¶rselleÅŸtirme
- GeliÅŸmiÅŸ export ve planlanmÄ±ÅŸ rapor gÃ¶nderimi

## 8. ModÃ¼l Listesi

### 8.1 Kimlik ve Yetkilendirme

KullanÄ±cÄ± oturumu, roller, yetkiler ve eriÅŸim kontrolÃ¼nÃ¼ yÃ¶netir.

### 8.2 Personel YÃ¶netimi

Saha personeli ve diÄŸer kullanÄ±cÄ±larÄ±n temel profil bilgisini tutar.

### 8.3 MÃ¼ÅŸteri/Firma YÃ¶netimi

GÃ¶rÃ¼ÅŸme yapÄ±lan kiÅŸi, mÃ¼ÅŸteri adayÄ±, bireysel mÃ¼ÅŸteri veya kurumsal firma bilgisini yÃ¶netir.

### 8.4 Lead YÃ¶netimi

Call center tarafÄ±ndan bulunan potansiyel kayÄ±tlarÄ±, statÃ¼lerini ve saha atamalarÄ±nÄ± yÃ¶netir.

### 8.5 Aktivite YÃ¶netimi

Saha ziyaretleri, gÃ¶rÃ¼ÅŸmeler, sonuÃ§lar, notlar ve takip aksiyonlarÄ±nÄ± tutar.

### 8.6 SatÄ±ÅŸ YÃ¶netimi

SatÄ±ÅŸÄ±n Ã¼rÃ¼n tipi, finansal deÄŸerleri, satÄ±ÅŸ adedi ve aktivite iliÅŸkisini yÃ¶netir.

### 8.7 Masraf YÃ¶netimi

Personel bazlÄ± masraf kayÄ±tlarÄ±nÄ± yÃ¶netir.

### 8.8 Dashboard ve Raporlama

Ham veriden KPI tÃ¼reterek Ã¶zet ekranlar sunar.

### 8.9 Excel Import

GeÃ§miÅŸ veya referans verilerin sisteme kontrollÃ¼ alÄ±nmasÄ±nÄ± saÄŸlar.

### 8.10 Audit ve Loglama

DeÄŸiÅŸiklik izleri, kritik iÅŸlem kayÄ±tlarÄ± ve hata takibini saÄŸlar.

## 9. Domain Modeli Ã–nerisi

Sistem dÃ¶rt ana iÅŸ ekseni Ã¼zerine kurulmalÄ±dÄ±r:

- Lead
- Aktivite
- SatÄ±ÅŸ
- Masraf

Bu Ã¼Ã§ eksen ortak boyutlarla iliÅŸkilendirilir:

- Personel
- MÃ¼ÅŸteri/Firma
- ÃœrÃ¼n
- Tarih
- Rol/KullanÄ±cÄ±

Domain yaklaÅŸÄ±mÄ± ÅŸu prensipleri korumalÄ±malÄ±dÄ±r:

- Aktivite, satÄ±ÅŸtan ayrÄ± bir kavramdÄ±r.
- SatÄ±ÅŸ, mÃ¼mkÃ¼n olduÄŸunda bir aktiviteye baÄŸlanmalÄ±dÄ±r.
- Masraf, satÄ±ÅŸtan baÄŸÄ±msÄ±z da olabilir ama raporda personel ve tarih ile iliÅŸkilendirilir.
- Dashboard verileri Ã¶zet tablolar yerine ham iÅŸlem kayÄ±tlarÄ±ndan tÃ¼retilmelidir.
- Aktivite sonucu tek alanlÄ± dÃ¼ÅŸÃ¼nÃ¼lmemeli; temas gerÃ§ekleÅŸmesi ile gÃ¶rÃ¼ÅŸme sonucu ayrÄ± kavramlar olarak ele alÄ±nmalÄ±dÄ±r.

Bu nedenle aktivite modelinde aÅŸaÄŸÄ±daki iki kavram ayrÄ± tutulmalÄ±dÄ±r:

- `ContactStatus`
- `OutcomeStatus`

## 10. Ana Entity Listesi

Ã–nerilen temel entity'ler:

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

MVP iÃ§in sadeleÅŸtirme kararÄ± olarak `Customer` ve `Company` ayrÄ± deÄŸil, tek bir `Account` Ã§atÄ±sÄ± altÄ±nda ele alÄ±nmalÄ±dÄ±r. Kurumsal temas kiÅŸileri iÃ§in `AccountContact` yapÄ±sÄ± ihtiyaÃ§ ve veri giriÅŸ hÄ±zÄ± dengesine gÃ¶re opsiyonel devreye alÄ±nabilir.

## 11. Entity Ä°liÅŸkileri

Ã–nerilen iliÅŸki mantÄ±ÄŸÄ±:

- Bir `Employee`, birÃ§ok `Activity` kaydÄ± oluÅŸturabilir.
- Bir `Activity`, bir `Account` ile iliÅŸkili olmalÄ±dÄ±r.
- Bir `Activity`, opsiyonel olarak bir `AccountContact` ile iliÅŸkili olabilir.
- Bir `Activity`, bir `ActivityContactStatusType` ile iliÅŸkili olmalÄ±dÄ±r.
- Bir `Activity`, bir `ActivityOutcomeStatusType` ile iliÅŸkili olmalÄ±dÄ±r.
- Bir `Activity` en fazla bir ana satÄ±ÅŸa baÄŸlanabilir; ancak ileride bir aktiviteden birden fazla Ã¼rÃ¼n satÄ±ÅŸÄ± Ã§Ä±kma ihtimali deÄŸerlendirilmelidir.
- Bir `Sale`, mÃ¼mkÃ¼nse bir `Activity` kaydÄ±na baÄŸlÄ± olmalÄ±dÄ±r.
- Bir `Sale`, bir `InsuranceProductType` ile iliÅŸkilidir.
- Bir `Sale` iÃ§in Ã¼rÃ¼n tipine gÃ¶re deÄŸiÅŸebilen finansal alanlar tutulur.
- Bir `Employee`, birÃ§ok `Expense` kaydÄ± oluÅŸturabilir.
- `Expense`, `ExpenseType` ile iliÅŸkilidir.
- `ImportBatch`, iÃ§eri aktarÄ±lan dosyanÄ±n iÅŸlem Ã¼st bilgisidir; `ImportRow` satÄ±r bazlÄ± sonucu saklar.
- `AuditLog`, kritik entity deÄŸiÅŸimlerini referanslar.

Aktivite sonucu aÃ§Ä±sÄ±ndan temel iÅŸ kuralÄ±:

- `ContactStatus = NOT_CONTACTED` ise `OutcomeStatus = NOT_APPLICABLE` olmalÄ±dÄ±r.
- `OutcomeStatus = SALE_CLOSED` tek baÅŸÄ±na satÄ±ÅŸ kaydÄ± anlamÄ±na gelmez; satÄ±ÅŸ teyidi `Sale` kaydÄ± Ã¼zerinden yapÄ±lmalÄ±dÄ±r.

## 12. Ekran Listesi

### YÃ¶netim ve ortak ekranlar

- GiriÅŸ ekranÄ±
- Ana dashboard
- KullanÄ±cÄ± listesi
- Rol/yetki yÃ¶netimi
- Personel listesi ve detay

### MÃ¼ÅŸteri/Firma ekranlarÄ±

- MÃ¼ÅŸteri/firma listesi
- MÃ¼ÅŸteri/firma oluÅŸturma-dÃ¼zenleme
- MÃ¼ÅŸteri/firma detay ve temas geÃ§miÅŸi

### Aktivite ekranlarÄ±

- Aktivite listesi
- Aktivite oluÅŸturma
- Aktivite detay
- Aktivite dÃ¼zenleme

### SatÄ±ÅŸ ekranlarÄ±

- SatÄ±ÅŸ listesi
- SatÄ±ÅŸ oluÅŸturma
- SatÄ±ÅŸ detay
- SatÄ±ÅŸ dÃ¼zenleme

### Masraf ekranlarÄ±

- Masraf listesi
- Masraf oluÅŸturma
- Masraf detay/dÃ¼zenleme

### Raporlama ekranlarÄ±

- YÃ¶netici dashboard
- Personel performans ekranÄ±
- ÃœrÃ¼n kÄ±rÄ±lÄ±m ekranÄ±
- Masraf analizi ekranÄ±
- Trend analizi ekranÄ±

### Veri yÃ¶netimi ekranlarÄ±

- Excel import ekranÄ±
- Import geÃ§miÅŸi ve hata ekranÄ±
- Audit log gÃ¶rÃ¼ntÃ¼leme ekranÄ±

## 13. Dashboard ve KPI MantÄ±ÄŸÄ±

Dashboard yapÄ±sÄ± doÄŸrudan ham iÅŸlem kayÄ±tlarÄ± Ã¼zerinden kurgulanmalÄ±, manuel Ã¶zet dosyalara veya sabit hesap tablolarÄ±na baÄŸÄ±mlÄ± olmamalÄ±dÄ±r.

### Temel KPI kategorileri

#### Aktivite KPI'larÄ±

- Toplam aktivite sayÄ±sÄ±
- GÃ¶rÃ¼ÅŸÃ¼len/gÃ¶rÃ¼ÅŸÃ¼lmeyen sayÄ±sÄ±
- Olumlu/olumsuz/ertelenen aktivite sayÄ±sÄ±
- Personel baÅŸÄ±na aktivite
- GÃ¼nlÃ¼k/haftalÄ±k/aylÄ±k aktivite trendi

#### SatÄ±ÅŸ KPI'larÄ±

- Toplam satÄ±ÅŸ adedi
- Toplam prim
- Toplam APE
- Toplam toplu para
- Toplam aylÄ±k Ã¶deme
- Toplam tahsilat
- ÃœrÃ¼n bazlÄ± satÄ±ÅŸ daÄŸÄ±lÄ±mÄ±

#### DÃ¶nÃ¼ÅŸÃ¼m KPI'larÄ±

- Aktiviteden satÄ±ÅŸa dÃ¶nÃ¼ÅŸÃ¼m oranÄ±
- Personel bazlÄ± dÃ¶nÃ¼ÅŸÃ¼m oranÄ±
- ÃœrÃ¼n bazlÄ± dÃ¶nÃ¼ÅŸÃ¼m oranÄ±
- Aktivite baÅŸÄ±na satÄ±ÅŸ tutarÄ±

#### Masraf KPI'larÄ±

- Toplam masraf
- Masraf tÃ¼rÃ¼ne gÃ¶re daÄŸÄ±lÄ±m
- Personel baÅŸÄ±na masraf
- SatÄ±ÅŸ baÅŸÄ±na masraf
- Aktivite baÅŸÄ±na masraf

#### Verimlilik KPI'larÄ±

- Personel baÅŸÄ±na satÄ±ÅŸ adedi
- Personel baÅŸÄ±na tahsilat
- Personel baÅŸÄ±na APE
- Masraf / tahsilat oranÄ±
- Masraf / prim oranÄ±

### KPI prensipleri

- Her KPI iÃ§in aÃ§Ä±k tanÄ±m yapÄ±lmalÄ±
- Pay ve payda mantÄ±ÄŸÄ± belgelendirilmeli
- Hangi tarih alanÄ±na gÃ¶re hesaplandÄ±ÄŸÄ± net olmalÄ±
- Ä°ptal, dÃ¼zenleme veya silme senaryolarÄ± hesaplamaya etkisiyle birlikte tanÄ±mlanmalÄ±
- Aktivite KPI'larÄ±nda temas durumu ile sonuÃ§ durumu ayrÄ± analitik kÃ¼meler olarak ele alÄ±nmalÄ±
- `SatÄ±ÅŸ Oldu` etiketi ile gerÃ§ek satÄ±ÅŸ varlÄ±ÄŸÄ± karÄ±ÅŸtÄ±rÄ±lmamalÄ±; satÄ±ÅŸ iÃ§in `Sale` kaydÄ± esas alÄ±nmalÄ±

## 14. Filtreleme MantÄ±ÄŸÄ±

Rapor ve liste ekranlarÄ±nda tutarlÄ± filtreleme kritik Ã¶nemdedir.

Ã–nerilen ortak filtreler:

- Tarih aralÄ±ÄŸÄ±
- Personel
- ÃœrÃ¼n tipi
- Temas durumu
- SonuÃ§ durumu
- MÃ¼ÅŸteri/firma
- KullanÄ±cÄ± tipi / rol

Ä°leri filtreler:

- BÃ¶lge veya saha alanÄ±
- Yeni mÃ¼ÅŸteri / mevcut mÃ¼ÅŸteri ayrÄ±mÄ±
- SatÄ±ÅŸÄ± olan / olmayan aktiviteler
- Masraf tÃ¼rÃ¼

Filtreleme prensipleri:

- TÃ¼m dashboard kartlarÄ± seÃ§ilen filtre baÄŸlamÄ±na uymalÄ±
- Liste ve grafik sayÄ±larÄ± tutarlÄ± olmalÄ±
- VarsayÄ±lan tarih filtresi Ã¼rÃ¼n tarafÄ±ndan net tanÄ±mlanmalÄ±
- KullanÄ±cÄ±ya birleÅŸik â€œaktivite sonucuâ€ filtresi gÃ¶sterilse bile backend sorgu mantÄ±ÄŸÄ±nda `ContactStatus` ve `OutcomeStatus` ayrÄ±mÄ± korunmalÄ±

## 15. Excel Import YaklaÅŸÄ±mÄ±

Excel dosyalarÄ± ilk aÅŸamada analiz ve geÃ§iÅŸ desteÄŸi iÃ§in kullanÄ±lmalÄ±dÄ±r; sistemin ana veri modeli Excel ÅŸablonuna gÃ¶re ÅŸekillenmemelidir.

### YaklaÅŸÄ±m prensipleri

- Excel kolonlarÄ± doÄŸrudan entity ÅŸemasÄ± olarak alÄ±nmamalÄ±
- Ã–nce hedef domain alanlarÄ± belirlenmeli
- ArdÄ±ndan Excel kolonlarÄ± bu alanlara eÅŸlenmeli
- Ham veri ve Ã¶zet veri ayrÄ±ÅŸtÄ±rÄ±lmalÄ±
- Import sonrasÄ± hata raporu Ã¼retilebilmeli

### Import sÃ¼reci Ã¶nerisi

1. Dosya ÅŸablonu tanÄ±ma
2. Kolon eÅŸleme
3. Ã–n doÄŸrulama
4. SatÄ±r bazlÄ± parse
5. Referans veri eÅŸleme
6. HatalÄ± kayÄ±tlarÄ± ayÄ±rma
7. BaÅŸarÄ±lÄ± kayÄ±tlarÄ± staging veya ana tabloya alma
8. Import Ã¶zet raporu Ã¼retme

### Dikkat edilmesi gerekenler

- AynÄ± verinin tekrar import edilmesini Ã¶nleme stratejisi
- Tarih ve sayÄ± formatlarÄ±nÄ±n standardizasyonu
- MÃ¼ÅŸteri/firma isim benzerliÄŸi nedeniyle oluÅŸabilecek Ã§oÄŸalmalar
- Excel'deki Ã¶zet ortalama tablolarÄ±n ham veri yerine geÃ§memesi

## 16. Rol ve Yetki MantÄ±ÄŸÄ±

Mevcut sistem, hem statik rÃ¼tbe korumasÄ± hem de dinamik modÃ¼l yetkilendirmesi (RBAC) iÃ§eren hibrit bir yapÄ± kullanÄ±r.

- **Admin**: Sistemdeki tÃ¼m yetkilere ve "Master Key" (SÃ¼per Yetki) bypass mekanizmasÄ±na sahiptir.
- **Dinamik ModÃ¼ller**: Dashboard, Leads, Accounts, Activities, Sales, Expenses ve Imports modÃ¼lleri, veritabanÄ±ndaki `RolePermission` tablosu Ã¼zerinden rollere atanabilir.
- **GÃ¼venlik KatmanÄ±**: Yetki kontrolleri `CanAccessPermission` metodu ile merkezi olarak gerÃ§ekleÅŸtirilir, UI elemanlarÄ± yetkiye gÃ¶re otomatik gizlenir.

## 17. Veri DoÄŸrulama Prensipleri

- Zorunlu alanlar ekran ve API seviyesinde doÄŸrulanmalÄ±
- Tarih alanlarÄ± mantÄ±ksal kontrol iÃ§ermeli
- Finansal alanlar negatif deÄŸer almamalÄ±; gerekirse iÅŸ kuralÄ± ile ayrÄ±ÅŸtÄ±rÄ±lmalÄ±
- Aktivite sonucu ile satÄ±ÅŸ iliÅŸkisi kurallÄ± olmalÄ±
- ÃœrÃ¼n tipine gÃ¶re bazÄ± finansal alanlar zorunlu veya opsiyonel olabilir
- `ContactStatus` zorunlu olmalÄ±
- `OutcomeStatus`, `ContactStatus = CONTACTED` ise zorunlu veya sistemce varsayÄ±lanlanmÄ±ÅŸ olmalÄ±
- `NOT_CONTACTED + POSITIVE/NEGATIVE/POSTPONED/SALE_CLOSED` kombinasyonlarÄ± geÃ§ersiz sayÄ±lmalÄ±
- `OutcomeStatus = SALE_CLOSED` ise ilgili satÄ±ÅŸ kaydÄ± iÅŸ akÄ±ÅŸÄ± doÄŸrulanmalÄ±
- Personel ve mÃ¼ÅŸteri referanslarÄ± geÃ§erli olmalÄ±
- Duplicate Ã¶nleme iÃ§in mÃ¼ÅŸteri/firma alanlarÄ±nda normalize eÅŸleme dÃ¼ÅŸÃ¼nÃ¼lmeli

## 18. Audit ve Log Ä°htiyacÄ±

Bu sistem operasyonel karar ve performans takibi iÃ§in kullanÄ±lacaÄŸÄ± iÃ§in audit ihtiyacÄ± yÃ¼ksektir.

Gerekli audit baÅŸlÄ±klarÄ±:

- KaydÄ± kim oluÅŸturdu
- Ne zaman oluÅŸturdu
- Kim gÃ¼ncelledi
- Ne zaman gÃ¼ncelledi
- Hangi alan deÄŸiÅŸti
- Eski ve yeni deÄŸer neydi

## 19. Teknik Mimari Ã–nerisi

### 19.1 Uygulanan Mimari Ä°yileÅŸtirmeler

#### Sprint 1 â€” Temel Temizlik ve GÃ¼venlik (TamamlandÄ±)
- **AppDataStore kaldÄ±rÄ±ldÄ±**: In-memory singleton veri deposu tamamen kaldÄ±rÄ±lmÄ±ÅŸ, tÃ¼m veri eriÅŸimi `AppDbContext` Ã¼zerinden saÄŸlanmaktadÄ±r.
- **BaseEntity sÄ±nÄ±fÄ± oluÅŸturuldu**: Audit alanlarÄ± (`CreatedAt`, `CreatedBy` vb.) tÃ¼m transaction entity'lerine eklendi.
- **Åifre gÃ¼venliÄŸi**: KullanÄ±cÄ± ÅŸifreleri BCrypt ile hash'lenerek saklanmaktadÄ±r.

#### Sprint 2 â€” Ä°liÅŸkisel RefaktÃ¶r (Enum â†’ FK GeÃ§iÅŸi) (TamamlandÄ±)
- **Referans Tablo Mimarisi**: Sabit Enum yapÄ±larÄ± veritabanÄ± seviyesinde baÄŸÄ±msÄ±z referans tablolarÄ±na taÅŸÄ±nmÄ±ÅŸtÄ±r.
- **Foreign Key Entegrasyonu**: TÃ¼m transaction tablolarÄ± bu yeni referans tablolarÄ±na FK ile baÄŸlanmÄ±ÅŸtÄ±r.

#### Sprint 3 â€” Performans, Planlama ve Servis KatmanÄ± (TamamlandÄ±)
- **Servis KatmanÄ± Entegrasyonu**: Ä°ÅŸ mantÄ±ÄŸÄ± Controller'lardan ayÄ±klanarak servis sÄ±nÄ±flarÄ±na taÅŸÄ±nmÄ±ÅŸtÄ±r.
- **Aktivite Planlama**: Ä°leriye dÃ¶nÃ¼k "PLANNED" statÃ¼sÃ¼ ve ajanda akÄ±ÅŸÄ± eklenmiÅŸtir.
- **Soft Delete**: `ISoftDeletable` arayÃ¼zÃ¼ ile veri silme yerine iÅŸaretleme altyapÄ±sÄ± kurulmuÅŸtur.

#### Sprint 4 â€” Dinamik Yetkilendirme ve YÃ¶netim (TamamlandÄ±)
- **Dinamik RBAC**: ModÃ¼l eriÅŸimleri veritabanÄ±ndaki `RolePermission` tablosu Ã¼zerinden canlÄ± yÃ¶netilebilir hale getirilmiÅŸtir.
- **Admin Master Key**: Sistem yÃ¶neticileri iÃ§in veritabanÄ± hatalarÄ±na karÅŸÄ± "Master Key" bypass mekanizmasÄ± eklenmiÅŸtir.
- **Database Transition**: GeliÅŸtirme kolaylÄ±ÄŸÄ± iÃ§in SQLite altyapÄ±sÄ±na geÃ§ilmiÅŸ ve veriler kalÄ±cÄ± hale getirilmiÅŸtir.
- **Finansal Lokalizasyon**: OndalÄ±k sayÄ± formatlama sorunlarÄ± `InvariantCulture` ile Ã§Ã¶zÃ¼lmÃ¼ÅŸtÃ¼r.

#### Sprint 5 â€” Dashboard ve KPI GÃ¼Ã§lendirme (TAMAMLANDI)
- **GÃ¶rsel Analitik (ParÃ§a B)**: `Chart.js` ile trend Ã§izgileri, Ã¼rÃ¼n portfÃ¶yÃ¼ ve tahsilat daÄŸÄ±lÄ±m grafikleri.
- **Performans Matrisi (ParÃ§a B)**: 7 kolonlu detaylÄ± personel verimlilik tablosu.
- **GeliÅŸmiÅŸ Filtreleme (ParÃ§a C)**: Tarih + Personel + ÃœrÃ¼n Tipi kompozit filtreleme yapÄ±sÄ±.
- **Finansal Raporlama (ParÃ§a C)**: ÃœrÃ¼n bazlÄ± APE, Prim ve Ãœretim toplamlarÄ±nÄ± gÃ¶steren derinlemesine finansal analiz tablosu.
- **UX/UI StandartlarÄ±**: "Dashboard Tab-Navigation" ve "Shared Filter Partial" ile premium arayÃ¼z deneyimi.
- **Saha/Ofis Veri AyrÄ±ÅŸtÄ±rma**: SatÄ±ÅŸlarÄ±n saha aktiviteleriyle olan dijital baÄŸlantÄ±sÄ± (Linkage) Ã¼zerinden veri kalitesi Ã¶lÃ§Ã¼lmeye baÅŸlanmÄ±ÅŸtÄ±r.

## 20. Backend Ã–nerisi
- ASP.NET Core MVC / Web API + EF Core
- Dinamik RBAC ve Servis KatmanÄ± Mimarisi
- IHttpContextAccessor tabanlÄ± otomatik Audit Log
- Merkezi Hata ve Yetki YÃ¶netimi

## 21. Frontend Ã–nerisi
- Razor Pages & Vanilla JS (InsuranceGridDraft)
- Lucide Icons & Premium CSS Aesthetics
- Dinamik menÃ¼ ve buton gizleme (Yetki bazlÄ±)
- Responsive ve interaktif Dashboard Ã¶ÄŸeleri

## 22. VeritabanÄ± Stratejisi

Sistem geliÅŸim aÅŸamalarÄ±na gÃ¶re hibrit bir veritabanÄ± yaklaÅŸÄ±mÄ± izler:
- **Development (GeliÅŸtirme): SQLite** (Zero-Config, taÅŸÄ±nabilirlik ve hÄ±zlÄ± geliÅŸtirme deneyimi iÃ§in).
- **Production (Ãœretim): PostgreSQL** (YÃ¼ksek Ã¶lÃ§eklenebilirlik, kurumsal gÃ¼venlik ve Ã§oklu baÄŸlantÄ± desteÄŸi iÃ§in).
- **SÃ¼reÃ§**: Proje tamamlandÄ±ÄŸÄ±nda baÄŸlantÄ± dizesi deÄŸiÅŸikliÄŸi ile PostgreSQL'e geÃ§iÅŸ hedeflenmektedir.

## 23. Fazlara AyrÄ±lmÄ±ÅŸ GeliÅŸtirme PlanÄ±

### Faz 0 - Analiz ve TasarÄ±m

- Ä°ÅŸ kurallarÄ±nÄ± netleÅŸtirme
- Domain modelini oluÅŸturma
- KPI tanÄ±mlarÄ±nÄ± doÄŸrulama
- ERD taslaÄŸÄ±nÄ± hazÄ±rlama
- Excel alan analizini yapma

### Faz 1 - Temel AltyapÄ±

- Backend solution yapÄ±sÄ±
- Frontend uygulama iskeleti
- Authentication ve rol altyapÄ±sÄ±
- Ortak response/validation yapÄ±sÄ±

### Faz 2 - Ã‡ekirdek Operasyon ModÃ¼lleri

- Personel yÃ¶netimi
- MÃ¼ÅŸteri/firma yapÄ±sÄ±
- Aktivite modÃ¼lÃ¼
- SatÄ±ÅŸ modÃ¼lÃ¼
- Masraf modÃ¼lÃ¼

### Faz 3 - Dashboard ve Raporlama

- YÃ¶netici dashboard
- Personel performans ekranlarÄ±
- Trend analizleri
- DÃ¶nÃ¼ÅŸÃ¼m oranÄ± raporlarÄ±

### Faz 4 - Veri GeÃ§iÅŸi ve Operasyonel SertleÅŸtirme

- Excel import
- Audit log
- Hata yÃ¶netimi
- Veri kalite kontrolleri

### Faz 5 - Ä°leri Ã–zellikler

- Onay akÄ±ÅŸlarÄ±
- Bildirimler
- Hedef/kota yÃ¶netimi
- GeliÅŸmiÅŸ analitik

## 24. Riskler ve Dikkat Edilmesi Gereken Noktalar

### 24.1 Domain belirsizlikleri

MÃ¼ÅŸteri, firma, temas kiÅŸisi ve poliÃ§e sahibi kavramlarÄ± birbirine karÄ±ÅŸabilir. BaÅŸta sade model kurulmalÄ±, sonra veri gerÃ§eklerine gÃ¶re geniÅŸletilmelidir.

### 24.2 KPI tanÄ±m riski

AynÄ± metrikin farklÄ± ekiplerce farklÄ± yorumlanmasÄ± mÃ¼mkÃ¼ndÃ¼r. Ã–zellikle APE, tahsilat, prim, satÄ±ÅŸ adedi ve dÃ¶nÃ¼ÅŸÃ¼m oranÄ± iÃ§in net tanÄ±m ÅŸarttÄ±r.

### 24.3 Excel baÄŸÄ±mlÄ±lÄ±ÄŸÄ± riski

Excel raporlarÄ±nÄ± veri modeli yerine koymak, hatalÄ± ve kÄ±rÄ±lgan mimariye neden olur. Excel yalnÄ±zca referans ve geÃ§iÅŸ aracÄ± olarak kullanÄ±lmalÄ±dÄ±r.

### 24.4 Veri kalitesi riski

AynÄ± mÃ¼ÅŸteri/firma iÃ§in tekrar eden kayÄ±tlar rapor kalitesini bozar. Duplicate Ã¶nleme ve veri temizleme yaklaÅŸÄ±mÄ± baÅŸtan dÃ¼ÅŸÃ¼nÃ¼lmelidir.

### 24.5 Yetki ve sahiplik riski

KullanÄ±cÄ±larÄ±n kendi verisi ile takÄ±m verisi arasÄ±nda eriÅŸim farkÄ± iyi tanÄ±mlanmazsa gÃ¼venlik ve operasyon sorunlarÄ± oluÅŸur.

### 24.6 Rapor performansÄ± riski

Dashboard sorgularÄ± ileride bÃ¼yÃ¼yebilir. Ham veriden tÃ¼retilen KPI mantÄ±ÄŸÄ± korunurken sorgu optimizasyonu planlanmalÄ±dÄ±r.

### 24.7 ÃœrÃ¼n kapsamÄ± riski

Ä°lk aÅŸamada Ã§ok fazla detay eklenirse MVP gecikir. Ã–nce Ã§ekirdek akÄ±ÅŸlar bitirilmeli, ileri analitikler sonra eklenmelidir.

## 25. Ä°lk Karar Prensipleri

Bu proje iÃ§in baÅŸlangÄ±Ã§ seviyesinde ÅŸu tasarÄ±m kararlarÄ± Ã¶nerilir:

- Aktivite, satÄ±ÅŸ ve masraf ayrÄ± modÃ¼ller olarak ele alÄ±nacak
- Dashboard verileri iÅŸlem kayÄ±tlarÄ±ndan tÃ¼retilecek
- Excel dosyalarÄ± yardÄ±mcÄ± kaynak olacak, ana veri kaynaÄŸÄ± olmayacak
- MVP Ã¶nceliÄŸi hÄ±zlÄ± veri giriÅŸi ve temel yÃ¶netici gÃ¶rÃ¼nÃ¼rlÃ¼ÄŸÃ¼ olacak
- Mimari sade tutulacak, gereksiz soyutlamadan kaÃ§Ä±nÄ±lacak
- Aktivite sonucu domain seviyesinde `ContactStatus` ve `OutcomeStatus` olarak iki ayrÄ± kavramla modellenerek veri kalitesi ve KPI doÄŸruluÄŸu korunacak

### Sprint 6 — Lead Detail Hub 360° (Tamamlandı)
- Lead Detail Hub sayfası oluşturuldu: Tek sayfada lead bilgileri, durum aksiyonları, atama bilgisi, bağlı müşteri, aktivite geçmişi ve satış paneli görüntülenmektedir.
- Lead durum geçiş mekanizması servis katmanında merkezi olarak yönetilmektedir (ChangeStatus metodu + geçiş kuralları matrisi).
- Hub sayfasında rol bazlı aksiyon görünürlüğü sağlanmıştır (atama sadece SalesManager, ziyaret sadece FieldSales vb.).
- Ziyaret Planlama özelliği eklendi (tarih/saat). Çakışma kontrolü API'si yazıldı ve arayüze bağlandı.
- Mevcut Lead listesindeki detay butonları Hub sayfasına yönlendirilmiştir.

