# Financial Validation Matrix

## 1. Amac

Bu belge, satış kaydı oluşturulurken seçilen ürün tipine göre hangi finansal alanların isteneceğini ve hangilerinin zorunlu olacağını netleştirir.

## 2. Alınan Ürün Kararı

Seçilen ürün tipine göre satış formu değişmelidir.

Karar:

- her satışta aynı alanlar gösterilmeyecek
- ürün tipine göre form alanları değişecek
- her ürün için yalnızca gerçekten gerekli alanlar zorunlu olacak

## 3. Neden Bu Karar Alındı

Çünkü farklı ürünlerde takip edilen finansal bilgi aynı değildir.

Örnek:

- BES satışında APE önemli olabilir
- Hayat satışında prim önemli olabilir
- Seyahat satışında daha sade bir finansal yapı yeterli olabilir

Bu nedenle tek tip satış formu gerçek operasyonu doğru yansıtmaz.

## 4. Genel Kural

Tüm satış kayıtlarında ortak zorunlu alanlar:

- `productType`
- `saleDate`
- `account`
- `employee`

Aktivite bağlantısı:

- mümkünse girilmeli
- MVP'de opsiyonel olabilir

## 5. Ürün Bazlı Kural Seti

## 5.1 BES

Gösterilecek başlıca alanlar:

- tahsilat
- APE
- toplu para
- aylık ödeme
- satış adedi

Zorunlu öneri:

- en az bir finansal temel alan
- tahsilat veya aylık ödeme bilgisinden biri

APE ve toplu para:

- MVP'de ürün bazlı girilebilir
- her zaman zorunlu olmak zorunda değildir
- ama varsa takip edilmelidir

## 5.2 Hayat

Gösterilecek başlıca alanlar:

- prim
- tahsilat
- satış adedi

Zorunlu öneri:

- prim

## 5.3 Sağlık

Gösterilecek başlıca alanlar:

- üretim tutarı
- tahsilat
- satış adedi

Zorunlu öneri:

- üretim tutarı veya tahsilat

## 5.4 Seyahat

Gösterilecek başlıca alanlar:

- satış tutarı
- tahsilat
- satış adedi

Zorunlu öneri:

- satış tutarı veya tahsilat

## 5.5 Diğer

Gösterilecek başlıca alanlar:

- satış tutarı
- tahsilat
- satış adedi
- açıklama

Zorunlu öneri:

- satış tutarı
- açıklama

## 6. UI Davranışı

Satış formunda kullanıcı önce ürün tipini seçer.

Sonra:

- ilgili finansal alanlar görünür
- gereksiz alanlar gizlenir
- zorunlu alanlar sadece seçilen ürün için uygulanır

## 7. Dashboard Etkisi

Bu karar sayesinde:

- ürün bazlı toplamlar daha doğru hesaplanır
- eksik veri uyarıları daha anlamlı olur
- kullanıcı gereksiz alan doldurmak zorunda kalmaz

## 8. Sonuç

Bu proje için kabul edilen kural şudur:

- satış formu ürün tipine göre değişecek
- her ürün için sadece gerçekten gerekli finansal alanlar zorunlu olacak
