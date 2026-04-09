# Use Cases

## 1. Amac

Bu belge, uygulamanin gercek operasyon icinde nasil kullanilacagini aciklar. Amaç; rol bazli kullanim seklini, gunluk is akislarini, hangi ekranin hangi sirayla kullanilacagini ve MVP sonrasinda beklenen operasyon davranisini netlestirmektir.

## 2. Uygulama Genel Kullanim Mantigi

Bu uygulama dort temel operasyon akisi uzerine kurulur:

- Lead kaydi ve atama
- Aktivite kaydi
- Satis kaydi
- Masraf kaydi

Temel kullanim mantigi su sekildedir:

1. Call center lead havuzu olusturur.
2. Satis muduru leadleri saha personeline atar.
3. Saha personeli atanan leadleri ziyaret ederek activity olusturur.
4. Aktivite sonucunda satis varsa ilgili satis kaydi acilir.
5. Gun icindeki yol, yemek, konaklama gibi giderler masraf olarak girilir.
6. Operasyon kullanicisi eksik veya hatali kayitlari kontrol eder.
7. Yonetici dashboard uzerinden performansi ve trendleri izler.

## 3. Rol Bazli Kullanim Ozetleri

## 3.1 Call Center Kullanıcısı

Ana amaci:

- Potansiyel musteri bulmak
- Iletisim bilgisi toplamak
- Lead havuzu olusturmak

Kullandigi ana ekranlar:

- Login
- Leads list
- Lead form
- Lead detail

## 3.2 Satış Müdürü

Ana amaci:

- Lead havuzunu degerlendirmek
- Leadleri uygun saha personeline atamak
- Atama yogunlugunu izlemek

Kullandigi ana ekranlar:

- Leads list
- Lead assignment
- Dashboard

## 3.3 Saha Personeli

Ana amaci:

- Atanmis leadlerini gormek
- Ziyaret kaydi girmek
- Gerekirse satis girmek
- Gunluk masrafini kaydetmek

Kullandigi ana ekranlar:

- Login
- My assigned leads
- Activities list/form/detail
- Sales form/list
- Expenses form/list

## 3.4 Operasyon Kullanıcısı

Ana amaci:

- Gelen kayitlari kontrol etmek
- Eksik veya tutarsiz veriyi duzeltmek
- Bagimsiz satislari uygun aktivitelerle eslestirmek
- Duplicate account risklerini yonetmek

Kullandigi ana ekranlar:

- Accounts list/detail/form
- Activities list/detail
- Sales list/detail/form
- Expenses list/detail
- Import upload/history

## 3.5 Yönetici

Ana amaci:

- Takim performansini gormek
- Donusum oranlarini izlemek
- Urun bazli uretimi takip etmek
- Masraf ve satis iliskisini yorumlamak

Kullandigi ana ekranlar:

- Executive dashboard
- Performance dashboard
- Product breakdown dashboard
- Expense analysis dashboard
- Lead assignment summary

## 3.6 Sistem Yöneticisi

Ana amaci:

- Kullanici ve rol yonetimi
- Referans veri duzeni
- Yetki kapsamlarini kontrol etmek

## 4. Temel Kullanım Senaryoları

## 4.1 Call Center Lead Oluşturma

Senaryo:

- Call center yeni potansiyel musteri bulur ve sisteme lead olarak kaydeder.

Adimlar:

1. Leads list ekraninda arama yapilir.
2. Kayit yoksa yeni lead formu acilir.
3. Potansiyel isim, telefon, sehir, ilce ve not bilgisi girilir.
4. Lead status `READY_FOR_ASSIGNMENT` olacak sekilde kayit tamamlanir.

## 4.2 Satış Müdürü Lead Atama

Senaryo:

- Satis muduru atamaya hazir leadleri saha personeline dagitir.

Adimlar:

1. Leads list ekraninda `READY_FOR_ASSIGNMENT` filtrelenir.
2. Lead detayindan veya assignment ekranindan personel secilir.
3. Atama tarihi, oncelik ve not girilir.
4. Lead status `ASSIGNED` olur.

## 4.3 Saha Personeli Günlük Akışı

Senaryo:

- Saha personeli atanmis leadlerini ziyaret eder, activity olusturur ve gerekirse satis girer.

Adimlar:

1. Kullanici sisteme giris yapar.
2. `My Assigned Leads` ekraninda acik islerini gorur.
3. Lead detayindan ziyaret baslatir.
4. Gerekirse account kaydi olusturur veya mevcut account ile eslestirir.
5. Activity formunda tarih, account, contact status ve outcome status girer.
6. Ziyaret sonucunda satis varsa sales formuna gecer.
7. Gun sonunda masraflarini kaydeder.

## 4.4 Aktiviteden Satış Oluşturma

Senaryo:

- Gorusme olumlu sonuclanir ve satis kaydi olusur.

Adimlar:

1. Activity detail acilir.
2. `Satisa Donustur` aksiyonu kullanilir.
3. Ilgili urun tipi secilir.
4. Urune bagli finansal alanlar doldurulur.
5. Satis kaydi olusturulur.

## 4.5 Bağımsız Satış Kaydı

Senaryo:

- Operasyon daha sonra bir satisi sisteme girer ancak uygun aktivite baglantisi yoktur.

Adimlar:

1. Sales form acilir.
2. Account ve personel secilir.
3. Aktivite bos birakilir.
4. Aciklama ve gerekce ile kayit tamamlanir.

## 4.6 Masraf Girişi

Senaryo:

- Saha personeli gun sonunda yol ve yemek masraflarini girer.

Adimlar:

1. Expenses form acilir.
2. Masraf tipi, tutar ve tarih girilir.
3. Aciklama eklenir.
4. Kayit tamamlanir.

## 4.7 Yönetici Analizi

Senaryo:

- Yonetici haftalik toplanti oncesi ekip performansini ve lead -> activity -> sale hattini inceler.

Adimlar:

1. Executive dashboard acilir.
2. Lead, aktivite, satis ve masraf KPI'lari birlikte yorumlanir.
3. Performance ekranindan personel tablosu incelenir.
4. Gerekirse detay icin lead, activity veya sales list ekranlarina gecilir.

## 5. Kritik İş Mantığı

- Lead, account degildir.
- Lead, activity degildir.
- Atanmis lead saha personelinin acik isi sayilir.
- `SALE_CLOSED` bir activity sonucu olabilir ancak gercek satis kaydi `sales` modulunde olusur.
- Bagimsiz satis istisna olmalidir, norm olmamalidir.

## 6. Sonuç

Bu uygulamanin yeni operasyon formulü sunudur:

- lead bul
- lead ata
- ziyarete donustur
- activity gir
- varsa satis gir
- masraf gir
- dashboard ile yorumla
