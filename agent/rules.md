# Project Rules

## 1. Belgenin Amacı

Bu belge, proje boyunca ürün kararlarının, teknik tercihlerinin ve uygulama yaklaşımının tutarlı kalmasını sağlamak için hazırlanmıştır. Amaç yalnızca kodlama standardı vermek değil; bu projede nasıl düşünüleceğini, hangi önceliklerin korunacağını ve hangi prensiplere göre ilerlenmesi gerektiğini netleştirmektir.

Bu belge proje anayasası olarak kabul edilmelidir. Yeni iş alınırken, mimari karar verilirken veya geliştirme yapılırken önce bu kurallar dikkate alınmalıdır.

## 2. Genel Çalışma Kuralları

- Kodlamaya başlamadan önce `project_design.md` dikkate alınmalı ve yapılacak iş onunla uyumlu olmalıdır.
- Her yeni iş önce `active_tasks.md` içinde planlanmalı, öncelik ve bağlam belirtilmelidir.
- Belirsiz noktalarda doğrudan kod yazmak yerine ürün kararı, varsayım veya açık soru not edilmelidir.
- Proje boyunca tutarlı isimlendirme kullanılmalıdır.
- Gereksiz karmaşık mimari kurulmamalıdır.
- MVP önceliği korunmalıdır.
- Önce çalışan çekirdek akışlar kurulmalıdır.
- İş kuralları koddan önce düşünülmeli; ekran veya API tasarımı bu kuralları yansıtmalıdır.
- Aynı problem için farklı modüllerde farklı isimlendirme veya farklı davranış oluşmamalıdır.
- Dokümantasyon yaşayan bir artefakt olarak güncel tutulmalıdır.

## 3. Önceliklendirme Prensipleri

- İlk öncelik, saha operasyonunun günlük veri girişini mümkün kılan temel akışlardır.
- İkinci öncelik, yöneticinin anlamlı KPI görebileceği raporlama ve dashboard ihtiyaçlarıdır.
- Üçüncü öncelik, operasyonel kaliteyi artıran audit, import ve veri temizleme kabiliyetleridir.
- İleri özellikler, çekirdek akışlar stabil olduktan sonra ele alınmalıdır.

## 4. Teknik Kurallar

### 4.1 Zorunlu Teknoloji Yığını

- Backend: ASP.NET Core Web API
- Frontend: React + TypeScript
- Veritabanı: PostgreSQL

### 4.2 Mimari Kurallar

- Entity ilişkileri açık ve sade kurulmalıdır.
- DTO kullanılmalıdır.
- Validation yapılmalıdır.
- API response yapısı standart olmalıdır.
- Soft delete gereken yerlerde düşünülmelidir.
- Audit log altyapısı planlanmalıdır.
- Dashboard verileri ham kayıtlardan türetilmelidir.
- Excel özet dosyalarına bağımlı mimari kurulmamalıdır.
- Katmanlı yapı tercih edilmeli, ancak gereksiz soyutlama üretilmemelidir.
- İş kuralları backend tarafında korunmalıdır.
- Raporlama sorguları transaction akışlarından kavramsal olarak ayrıştırılmalıdır.

### 4.3 API Tasarım Kuralları

- İstek ve cevap modelleri entity'lerden doğrudan sızmamalıdır.
- Liste endpoint'lerinde sayfalama, filtreleme ve sıralama düşünülmelidir.
- Başarılı ve başarısız cevaplar tutarlı formatta dönmelidir.
- Validation hataları standart bir hata modeli ile sunulmalıdır.
- Kimlik doğrulama ve yetki kontrolü API seviyesinde net uygulanmalıdır.

### 4.4 Veritabanı Kuralları

- Transaction tabloları normalize yapıda tutulmalıdır.
- Referans tabloları ayrı tanımlanmalıdır.
- Denormalizasyon yalnızca raporlama veya performans gerekçesi netse düşünülmelidir.
- Aynı müşteriyi veya firmayı çoğaltan kontrolsüz yapıdan kaçınılmalıdır.
- Tarih, para ve sayısal alanlarda tutarlı veri tipi kullanılmalıdır.

## 5. Ürün Kuralları

- Lead, Account ve Activity aynı şey değildir; ayrı ama ilişkili kavramlar olarak ele alınmalıdır.
- Call center tarafından bulunan potansiyel kayıtlar önce `Lead` olarak tutulmalıdır.
- Satış müdürü lead atama akışını kullanarak saha personeline iş vermelidir.
- Saha personelinin açık işleri yalnızca activity değil, atanmış lead kayıtlarından da oluşabilmelidir.
- Aktivite, satış ve masraf birbirinden ayrı ama ilişkili modüller olarak ele alınmalıdır.
- Bir aktivite satışa dönüşebilir.
- Satışlar mümkün olduğunca aktiviteyle ilişkilendirilmelidir.
- Müşteri/firma tekrar eden kayıtları önleyecek mantık düşünülmelidir.
- Personel performansı sadece adetle değil verimlilikle ölçülebilmelidir.
- Yönetici ekranı gerçek operasyonel karar almayı desteklemelidir.
- KPI'lar açık tanım olmadan geliştirilmemelidir.
- Dashboard'da gösterilen sayıların kaynağı geri izlenebilir olmalıdır.
- Raporlama dili operasyon ekibinin kullandığı kavramlarla uyumlu olmalıdır.

## 6. Domain ve Modelleme Kuralları

- Domain model önce iş akışına göre kurulmalı, Excel kolonlarına göre kurulmamaldır.
- Aynı kavram için tek bir baskın isim belirlenmeli ve her yerde o isim kullanılmalıdır.
- Aktivite sonucu ile satış oluşumu arasındaki iş ilişkisi modelde açık olmalıdır.
- Ürün bazlı finansal alanlar gerektiğinde esnek ama denetlenebilir şekilde tasarlanmalıdır.
- İlk sürümde fazla genelleştirme yapılmamalıdır.
- İleride genişleme ihtimali olan alanlar not edilmeli, ancak bugünden karmaşıklaştırılmamalıdır.

## 7. Excel ve Veri Geçişi Kuralları

- Excel dosyaları referans veri kaynağı olarak ele alınmalıdır, ana sistem tasarımını belirlememelidir.
- Ham veri ve özet veri ayrımı korunmalıdır.
- Excel import süreci doğrulama, hata raporlama ve tekrar import kontrolü içermelidir.
- Excel raporlarındaki KPI'lar sistem KPI tasarımını doğrulamak için kullanılabilir; ama doğrudan veri modeli yerine geçemez.
- Import edilen veriler izlenebilir batch mantığı ile saklanmalıdır.

## 8. Rol ve Yetki Kuralları

- Rol yapısı en baştan düşünülmelidir; sonradan eklenen kırık yetkilerden kaçınılmalıdır.
- Kullanıcının kendi verisini görmesi ile tüm organizasyonu görmesi ayrı yetki olarak ele alınmalıdır.
- Audit ve kritik yönetim ekranları sınırlı rol erişimine sahip olmalıdır.
- Operasyon kullanıcısının hangi kayıtları düzeltebileceği net tanımlanmalıdır.

## 9. Dashboard ve KPI Kuralları

- Her KPI için açık hesaplama tanımı olmalıdır.
- KPI'lar tarih filtresine ve diğer filtrelere tutarlı tepki vermelidir.
- Ham veri ile dashboard verisi arasında iz sürülebilirlik kurulmalıdır.
- Yönetici ekranı yalnızca sayı göstermemeli, karar destek üretmelidir.
- Trend analizlerinde zaman kırılımları tutarlı tanımlanmalıdır.

## 10. Kod Kalitesi Kuralları

- Okunabilirlik öncelikli olmalıdır.
- Gereksiz abstraction yapılmamalıdır.
- Dosya ve klasör yapısı tutarlı olmalıdır.
- Her modül mantıksal olarak ayrıştırılmalıdır.
- Kod üretirken kısa teknik notlar bırakılabilmelidir.
- Aynı işi yapan yinelenmiş kodlar azaltılmalıdır.
- Karmaşıklık erken aşamada arttırılmamalıdır.
- Refactor ihtiyacı oluşursa, önce davranış korunmalı sonra yapı iyileştirilmelidir.

## 11. Dokümantasyon Kuralları

- Ürün kararları yalnızca sohbet içinde bırakılmamalı, gerekli durumlarda markdown belgelere işlenmelidir.
- Açık sorular `active_tasks.md` içinde görünür tutulmalıdır.
- Önemli mimari kararlar nedenleriyle not edilmelidir.
- Dokümantasyon yüzeysel değil, karar almayı kolaylaştıracak netlikte olmalıdır.

## 12. Uygulama Sırası Kuralları

Projede mümkün olduğunca aşağıdaki sıra korunmalıdır:

1. İş ihtiyacını netleştirme
2. Domain ve ekran akışını netleştirme
3. Teknik iskeleti kurma
4. Çekirdek modülleri geliştirme
5. Dashboard ve raporlama ekleme
6. Import, audit ve sertleştirme
7. İleri özellikler

## 13. Kaçınılması Gereken Yaklaşımlar

- Excel şablonunu doğrudan veritabanı tasarımı yapmak
- Daha MVP netleşmeden ileri seviye altyapı kurmak
- Kullanıcı ihtiyacı netleşmeden aşırı generic modül tasarlamak
- Tek ekran için özel çözüm üretip genel tutarlılığı bozmak
- Rapor metriklerini tanımsız şekilde uygulamak
- Audit ve veri sahipliği ihtiyaçlarını görmezden gelmek

## 14. Karar Alma Prensibi

Yeni bir karar alınırken şu sıra izlenmelidir:

1. Bu karar `project_design.md` ile uyumlu mu?
2. Bu karar MVP'yi hızlandırıyor mu, yoksa gereksiz karmaşıklık mı ekliyor?
3. Bu karar aktivite, satış, masraf ve dashboard ilişkisini bozuyor mu?
4. Bu karar veri kalitesini ve rapor güvenilirliğini nasıl etkiliyor?
5. Bu karar ileride genişlemeye alan bırakırken bugün sade kalabiliyor mu?

## 15. Son İlke

Bu projede amaç yalnızca çalışan ekranlar üretmek değildir. Amaç, saha operasyonundan yönetsel karara kadar uzanan güvenilir bir veri akışı kurmaktır. Bu nedenle her geliştirme; veri kalitesi, izlenebilirlik, rapor doğruluğu ve operasyonel kullanılabilirlik açısından değerlendirilmelidir.
