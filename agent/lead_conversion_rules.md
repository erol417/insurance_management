# Lead Conversion Rules

## 1. Amac

Bu belge, bir `Lead` kaydinin ne zaman ve hangi kosullarda `Account` ve `Activity` tarafina donusecegini netlestirir. Amaç; call center'dan gelen potansiyel kayitlar ile sahada islenen gercek operasyon kayitlari arasindaki gecis kurallarini sabitlemektir.

## 2. Alinan Temel Urun Karari

Varsayilan olarak `Lead`, saha oncesinde `Account` olmaz.

Esas karar:

- Lead, saha ziyareti sirasinda veya hemen oncesinde gercekten islenecekse `Account`a donusur
- Lead, dogrudan `Sale`e donusmez
- Lead'in operasyonel ilk gercek donusumu `Activity` uzerinden olur

Bu nedenle temel donusum hattı su sekildedir:

- `Lead -> Account + Activity`
- daha sonra gerekiyorsa `Activity -> Sale`

## 3. Neden Bu Karar Alindi

Bu karar su nedenlerle alindi:

- Potansiyel havuz ile gercek musteri havuzunu ayri tutmak
- Saha oncesi kirli ve eksik veriyi `Account` tablosuna doldurmamak
- Duplicate riskini azaltmak
- Saha personelinin teyit ettigi kayitlari esas almak
- `Lead` modulunu gercek bir on-havuz olarak kullanmak

## 4. Temel Donusum Kurali

Bir lead sahada islenmeye basladiginda sistem su sirayla davranmalidir:

1. Mevcut bir `Account` var mi kontrol edilir
2. Varsa lead o account ile eslestirilir
3. Yoksa yeni `Account` olusturulur
4. Ardindan `Activity` kaydi acilir
5. Lead kaydinin durumu `CONVERTED_TO_ACTIVITY` olarak guncellenir
6. Olusan activity referansi lead kaydina yazilir

## 5. Donusumun Tetiklendigi Durumlar

MVP'de lead donusumu su durumlarda tetiklenebilir:

- Saha personeli `My Assigned Leads` ekranindan `Ziyaret Baslat` aksiyonunu kullanir
- Lead detayindan `Aktiviteye Donustur` aksiyonu kullanilir

Bu noktada sistem kullaniciyi asagidaki karara zorlar:

- mevcut account sec
veya
- yeni account olustur

## 6. Account Zorunlulugu Karari

Lead, `Activity` olusmadan once account ile iliskilenmelidir.

MVP urun karari:

- Accountsuz activity olusmaz
- Bu nedenle lead -> activity gecisinde ya mevcut account secilir ya da yeni account olusturulur

Bu karar sayesinde:

- tum aktiviteler gercek musteri/firma baglami tasir
- dashboard ve gecmis takip daha tutarli olur

## 7. Mevcut Account Ile Eslesme Kurali

Donusum aninda sistem mevcut account onerisi sunabilir.

Eslesme icin kullanilabilecek alanlar:

- `displayName`
- `phone`
- `email`
- varsa `taxNumber`

MVP'de otomatik merge yapilmaz.

Karar:

- sistem eslesme onerisi verebilir
- son karar kullanicidadir

## 8. Lead Status Gecis Kurallari

Onerilen temel akıs:

- `NEW`
- `RESEARCHED`
- `CONTACT_FOUND`
- `READY_FOR_ASSIGNMENT`
- `ASSIGNED`
- `VISIT_SCHEDULED`
- `VISITED`
- `CONVERTED_TO_ACTIVITY`

Alternatif kapanis:

- `DISQUALIFIED`

Kurallar:

- `ASSIGNED` olmadan saha personeli lead'i kendi listesinde gormez
- `VISITED` durumu activity olustugunda veya activity sonrasi ara durum olarak kullanilabilir
- `CONVERTED_TO_ACTIVITY` oldugunda lead operasyonel olarak kapanmis sayilir

## 9. Activity Ile Iliski

Lead donustugunde olusan activity kaydi:

- `leadId` referansi tasiyabilir
- veya lead tarafinda `convertedActivityId` tutulabilir

MVP icin minimum karar:

- lead tarafinda `convertedActivityId` tutulmasi yeterlidir

Bu bag:

- lead kaynagindan sahadaki ilk harekete iz surmeyi saglar

## 10. Satış Ile Iliski

Lead dogrudan satışa donusmez.

Doğru operasyon mantigi:

- lead
- account/activity
- sale

Bu nedenle:

- `Lead -> Sale` kisa yolu MVP'de desteklenmemelidir
- Satis varsa bile once activity kaydi acilmalidir

## 11. UI Davranis Onerisi

`My Assigned Leads` veya `Lead Detail` ekraninda kullanici `Ziyaret Baslat` dediginde:

1. account eslestirme/olusturma adimi gelir
2. sonra activity formu acilir
3. activity kaydi tamamlaninca lead durumu guncellenir

Bu akış tek wizard gibi veya iki adimli ekran gibi tasarlanabilir.

## 12. Veri Kalitesi Kurallari

- Aynı lead birden fazla kez activity'ye donusturulmemelidir
- `CONVERTED_TO_ACTIVITY` lead tekrar aktif atama havuzuna dusmemelidir
- Lead ile account arasindaki eslesme kaydi izlenebilir olmalidir
- Accountsuz lead donusumu tamamlanmis sayilmamalidir

## 13. MVP Disi Gelecek Genislemeler

Ileri fazda su davranislar dusunulebilir:

- tek lead'den birden fazla activity zinciri
- otomatik duplicate account onerisi
- lead scoring
- call center performans KPI'lari
- lead recycle / yeniden atama mantigi

## 14. Sonuc

Bu proje icin net donusum karari sunudur:

- `Lead`, varsayilan olarak saha oncesinde `Account` olmaz
- Lead, saha ziyareti sirasinda veya hemen oncesinde `Account` ile eslestirilir veya yeni account'a donusur
- Ardindan `Activity` olusur
- Satis varsa daha sonra `Activity` uzerinden ilerler

Yani MVP icin kabul edilen dogru zincir:

- `Lead -> Account + Activity -> Sale`
