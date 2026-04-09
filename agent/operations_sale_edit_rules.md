# Operations Sale Edit Rules

## 1. Amac

Bu belge, Operations kullanicisinin satis kayitlari uzerinde hangi degisiklikleri yapabilecegini netlestirir.

## 2. Alinan Urun Karari

Operations satis kaydini tamamen serbest sekilde duzenleyemeyecektir.

MVP karari:

- Operations eksik veya operasyonel bilgileri duzeltebilir
- cekirdek satis gercegini degistiren alanlar sinirli olacaktir

## 3. Duzenlenebilecek Alanlar

Operations tarafinin duzenleyebilmesi uygun olan alanlar:

- `notes`
- eksik aciklama alanlari
- dokuman veya referans numarasi benzeri operasyonel alanlar
- `activityId` baglama veya duzeltme
- veri giris hatasi niteligindeki ikincil alanlar

## 4. Sinirli veya Yasakli Alanlar

Operations tarafinin serbestce degistirmemesi gereken alanlar:

- `productType`
- `ownerEmployeeId`
- ana finansal degerler
- `saleDate`
- account baglamini degistiren kritik iliski alanlari

Bu alanlar icin:

- ya sadece `SalesManager`, `Manager` veya `Admin` yetkili olur
- ya da duzeltme talebi akisi kullanilir

## 5. Neden Bu Karar Alindi

Bu karar su nedenlerle alinmistir:

- satis gercegini sonradan bozmayi engellemek
- KPI ve finansal raporlarin guvenilirligini korumak
- operasyon ekibine gerekli duzeltme esnekligini yine de vermek

## 6. Audit Gereksinimi

Operations tarafinin yaptigi satis duzenlemeleri audit kaydina dusmelidir.

Ozellikle izlenmesi gerekenler:

- hangi alan degisti
- eski deger
- yeni deger
- degistiren kullanici
- degisiklik zamani

## 7. Sonuc

Bu proje icin kabul edilen kural sudur:

- Operations satis kaydini sinirsiz duzenleyemez
- operasyonel tamamlama ve baglama duzeltmeleri yapabilir
- cekirdek satis alanlari daha ust yetkiyle korunur
