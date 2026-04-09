# Sales Linking Rules

## 1. Amac

Bu belge, `Activity` ile `Sale` arasındaki ilişki kurallarını netleştirir.

## 2. Alınan Ürün Kararı

Bir aktivite kaydına bağlı birden fazla satış açılabilmelidir.

Yani doğru ilişki yapısı:

- `Activity 1 -> N Sales`

## 3. Neden Bu Karar Alındı

Gerçek hayatta tek bir görüşmede:

- hem BES
- hem hayat
- hem başka ürünler

aynı müşteri veya firma için birlikte satılabilir.

Bu nedenle `1 aktivite = en fazla 1 satış` yaklaşımı gerçek operasyonu eksik modeller.

## 4. Kuralın Anlamı

- her satış kaydı en fazla bir aktiviteye bağlanabilir
- ama bir aktivite birden fazla satış üretebilir
- satışlar ürün bazlı ayrı kayıtlar olarak tutulmalıdır

## 5. UI ve Kayıt Davranışı

Aktivite detay ekranından:

- kullanıcı ilk satış kaydını açabilir
- aynı aktiviteye bağlı ikinci veya üçüncü satış kaydını da açabilir

Örnek:

- aynı görüşmede BES satıldı
- aynı görüşmede Hayat satıldı

Bu durumda iki ayrı sale kaydı açılır ama ikisi de aynı activity'ye bağlı olur.

## 6. KPI Etkisi

Bu karar sonrası şu iki kavram ayrı düşünülmelidir:

- satış yapan aktivite sayısı
- toplam satış adedi

Çünkü bir aktivite birden fazla satış üretebilir.

## 7. Sonuç

Bu proje için kabul edilen ilişki kuralı şudur:

- bir aktiviteye bağlı birden fazla satış açılabilir
- doğru ilişki yapısı `Activity 1 -> N Sales` olacaktır
