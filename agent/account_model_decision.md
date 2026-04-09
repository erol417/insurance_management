# Account Model Decision

## 1. Amac

Bu belge, MVP icin `Account` modelinin nasil kurgulanacagini netlestirir. Amaç; `Individual` ve `Corporate` kayitlari icin asiri erken ayrismaya gitmeden, veri kalitesini koruyan ve uygulama hizini dusurmeyen bir model karari vermektir.

## 2. Alinan Urun Karari

MVP'de ortak `Account` yapisi kurulacaktir.

Karar prensipleri:

- Zorunlu alanlar mumkun oldugunca ortak tutulacaktir.
- `Individual` ve `Corporate` icin yalnizca minimum ayrisma uygulanacaktir.
- Fazla detayli ve erken parcali modelleme MVP'ye alinmayacaktir.

Bu karar, hiz ve sadelik ile veri kalitesi arasinda dengeli MVP yaklasimi olarak kabul edilmistir.

## 3. Neden Bu Karar Alindi

Bu karar su nedenlerle alindi:

- MVP hizini korumak
- Tek account formu ile operasyonu sade tutmak
- FieldSales ve Operations tarafinda gereksiz form karmasasi olusturmamak
- Bireysel ve kurumsal kayitlari ayni ust modelde toplayarak ilk surumu kolaylastirmak
- Gelecekte ayrisma ihtiyaci olursa kontrollu genislemeye alan birakmak

## 4. Onerilen MVP Modeli

Tek bir `Account` entity kullanilacaktir.

Temel alan:

- `accountType`
- `displayName`
- `city`
- `district` nullable
- `phone` veya baska en az bir iletisim bilgisi
- `email` nullable
- `ownerEmployeeId` veya operasyon sahibi
- `status`
- `notes` nullable

## 5. Genel Zorunlu Alanlar

Tum account tipleri icin MVP'de zorunlu tutulmasi onerilen alanlar:

- `accountType`
- `displayName`
- `city`
- en az bir iletisim alani
- `status`

Not:

- `phone` veya `email` ikilisinden en az biri olmali
- tamamen iletisim bilgisiz account acilmasina izin verilmemeli

## 6. Minimum Tip Bazli Ayrisma

## 6.1 Individual

Minimum beklenti:

- `displayName` bireysel kisi adini temsil eder
- `phone` veya `email`

MVP'de opsiyonel kalabilecek alanlar:

- `identityNumber`
- `birthDate`

## 6.2 Corporate

Minimum beklenti:

- `displayName` firma unvanini temsil eder
- `phone` veya `email`

MVP'de opsiyonel ama guclu onerili alanlar:

- `taxNumber`
- `taxOffice`
- `accountContacts`

## 7. Form Davranisi

MVP'de tek `Account Form` kullanilacaktir.

Davranis:

- Kullanici once `accountType` secer
- Formun cekirdek alani her iki tip icin ortaktir
- Tip bazli alanlar sadece minimum seviyede acilir
- Corporate secildiginde temas kisisi ekleme alani gorunebilir ama zorunlu olmak zorunda degildir

## 8. Duplicate Kontrol Etkisi

Bu karar duplicate kontrolunu daha kritik hale getirir.

Onerilen kontrol mantigi:

- `displayName`
- `phone`
- `email`
- varsa `taxNumber`

birlikte duplicate taramasinda kullanilmalidir.

## 9. Lead Ile Iliski

Lead, varsayilan olarak saha oncesinde account olmaz.

Bu nedenle:

- Lead ziyaretten once sadece potansiyel kayittir
- Saha ziyareti sirasinda veya hemen oncesinde account olusabilir
- Account modeli bu gecisi destekleyecek kadar sade ama kontrollu olmalidir

## 10. MVP Disi Gelecek Faz Genislemeleri

Ileri fazda su ayrismalar dusunulebilir:

- Bireysel ve kurumsal account icin farkli detay alanlari
- Kurumsal hesapta birden fazla temas kisisi zorunlulugu
- Vergi/TCKN tabanli daha sert validation
- Sektor, segment, musteri tipi gibi ek alanlar

## 11. Sonuc

Bu proje icin kabul edilen hesap modeli karari sunudur:

- MVP'de ortak `Account` yapisi kurulacak
- Zorunlu alanlar mumkun oldugunca ortak tutulacak
- `Individual` ve `Corporate` icin yalnizca minimum ayrisma uygulanacak

Bu karar, hem operasyonu sade tutar hem de gelecekte kontrollu genislemeye izin verir.
