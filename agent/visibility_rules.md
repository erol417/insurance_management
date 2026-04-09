# Visibility Rules

## 1. Amac

Bu belge, uygulamada hangi rolün hangi kapsamda veri göreceğini netleştirir.

## 2. Alınan Ürün Kararı

MVP'de saha personeli varsayılan olarak sadece kendi kayıtlarını görür.

Bu karar şu modüller için geçerlidir:

- assigned leads
- activities
- sales
- expenses

## 3. Rol Bazlı Görünürlük

### FieldSales

- sadece kendi kayıtlarını görür
- sadece kendisine atanmış leadleri görür
- sadece kendi aktivitelerini görür
- sadece kendi satış ve masraf kayıtlarını görür

### Operations

- operasyonel ihtiyaç kapsamında daha geniş veri görür
- düzeltme ve kontrol amacıyla kayıt detaylarına erişebilir

### SalesManager

- lead havuzunu ve atama durumlarını görür
- saha dağılımını görür

### Manager

- ekip ve yönetim seviyesinde geniş görünürlük alır

### Admin

- tam görünürlük alır

## 4. Neden Bu Karar Alındı

Bu karar şu nedenlerle alındı:

- saha personelinin ekranlarını sade tutmak
- gereksiz veri kalabalığını azaltmak
- veri gizliliğini korumak
- MVP'de yetki yapısını daha net kurmak

## 5. Sonuç

Bu proje için kabul edilen görünürlük kararı şudur:

- saha personeli varsayılan olarak sadece kendi kayıtlarını görür
- geniş görünürlük yönetici, satış müdürü, operasyon ve admin tarafında açılır
