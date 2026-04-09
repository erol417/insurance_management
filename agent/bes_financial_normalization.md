# BES Financial Normalization

## 1. Amac

Bu belge, BES satislarinda takip edilen finansal alanlarin nasil ele alinacagini netlestirir.

## 2. Alinan Urun Karari

BES finansal alanlari tek bir genel tutar alanina indirgenmeyecektir.

Ayri izlenecek temel alanlar:

- `collectionAmount`
- `apeAmount`
- `lumpSumAmount`
- `monthlyPaymentAmount`
- `saleCount`

## 3. Neden Bu Karar Alindi

Bu karar su nedenlerle alinmistir:

- BES KPI'lari tek metrikle dogru anlatilamaz
- tahsilat, APE, toplu para ve aylik odeme farkli is anlami tasir
- dashboard tarafinda bu alanlar ayri ayri raporlanmak istenir

## 4. MVP Validation Prensibi

BES satisinda:

- ortak satis alanlari zorunludur
- finansal alanlardan en az bir anlamli BES metrigi girilmelidir

MVP minimum kural:

- `collectionAmount` veya `monthlyPaymentAmount` alanlarindan en az biri dolu olmalidir

Ek alanlar:

- `apeAmount` opsiyonel ama takip edilir
- `lumpSumAmount` opsiyonel ama takip edilir

## 5. KPI Etkisi

Dashboard ve raporlarda:

- toplam BES tahsilati `collectionAmount` toplamindan
- toplam BES APE `apeAmount` toplamindan
- toplam BES toplu para `lumpSumAmount` toplamindan
- BES aylik odeme toplami `monthlyPaymentAmount` toplamindan uretilir

Bu alanlar birbirine donusturulmez ve tek toplama zorlanmaz.

## 6. Sonuc

Bu proje icin kabul edilen karar sudur:

- BES finansal verileri ayri alanlar halinde saklanir
- tek bir normalize "BES tutari" alani kullanilmaz
- KPI'lar bu alanlardan ayri ayri uretilir
