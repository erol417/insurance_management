# Backend Frontend Delivery Plan

## 1. Amaç

Bu belge, backend ve frontend tarafında geliştirme sırasını, modül sınırlarını ve uygulanabilir bir teslim planını tanımlar. Amaç, doğrudan kod üretmeden önce ekip için net bir uygulama yol haritası çıkarmaktır.

## 2. Genel Yaklaşım

Bu proje için en uygun yaklaşım, backend ve frontend'i aynı anda tamamen doldurmaya çalışmak yerine, modül bazlı dikey dilimler halinde ilerlemektir.

Önerilen geliştirme prensibi:

- Önce iskelet
- Sonra çekirdek transaction modülleri
- Sonra dashboard
- Sonra import, audit ve sertleştirme

Bu yaklaşım sayesinde hem kullanıcı erken değer görür hem de mimari kontrollü büyür.

## 3. Backend Planı

## 3.1 Backend Hedefi

Backend; iş kurallarını koruyan, DTO ve validation kullanan, rol/yetki kontrolleri açık, dashboard sorgularını destekleyen bir API katmanı sunmalıdır.

## 3.2 Backend Modülleri

Önerilen modül listesi:

- Auth
- Users / Roles
- Employees
- Accounts
- Activities
- Sales
- Expenses
- Dashboard
- Imports
- AuditLogs

## 3.3 Backend Fazları

### Faz 1 - Temel Teknik İskelet

Hedef:

- Solution yapısını kurmak
- Katmanları ayırmak
- PostgreSQL bağlantısını hazırlamak
- Authentication temelini oluşturmak
- Standart API response yapısını belirlemek
- Exception handling ve validation temelini kurmak

Çıktılar:

- Çalışan boş API
- Sağlık kontrol endpoint'i
- Login iskeleti
- Ortak DTO/response modeli

### Faz 2 - Referans ve Organizasyon Verisi

Hedef:

- Employee
- Account
- Reference tables

Çıktılar:

- Personel CRUD
- Müşteri/firma CRUD
- Ürün tipi, aktivite sonucu ve masraf tipi endpoint'leri

### Faz 3 - Aktivite Modülü

Hedef:

- Aktivite kayıt akışını ayağa kaldırmak

Gereken use case'ler:

- Aktivite oluşturma
- Aktivite güncelleme
- Aktivite listeleme
- Aktivite detay görüntüleme
- Aktivite filtreleme

Kritik kurallar:

- Aktivite tarih ve personel zorunlu
- Aktivite sonucu zorunlu
- Müşteri/firma ilişkisi zorunlu

### Faz 4 - Satış Modülü

Hedef:

- Aktiviteye bağlı veya bağımsız satış kaydını desteklemek

Gereken use case'ler:

- Satış oluşturma
- Satış güncelleme
- Satış listeleme
- Satış detay görüntüleme
- Ürün bazlı filtreleme

Kritik kurallar:

- Satış ürün tipi zorunlu
- Finansal alan validation'ı ürün tipine göre çalışmalı
- Aktivite bağlantısı varsa tutarlılık kontrol edilmeli

### Faz 5 - Masraf Modülü

Hedef:

- Personel bazlı masraf girişini ve görüntülemeyi sağlamak

Gereken use case'ler:

- Masraf oluşturma
- Masraf güncelleme
- Masraf listeleme
- Tarih ve masraf türü filtreleri

Kritik kurallar:

- Tutar pozitif olmalı
- Personel ve tarih zorunlu olmalı

### Faz 6 - Dashboard ve Raporlama

Hedef:

- Yöneticinin kullanabileceği ilk KPI setini üretmek

İlk dashboard endpoint grupları:

- Özet kartlar
- Aktivite trendi
- Satış trendi
- Ürün kırılımı
- Personel performansı
- Masraf özeti
- Dönüşüm oranı

Teknik not:

- Dashboard sorguları transaction CRUD endpoint'lerinden ayrılmalıdır.
- Hesaplama mantığı tek yerde toplanmalıdır.

### Faz 7 - Import ve Audit

Hedef:

- Excel kaynaklı veri alımı ve denetim izi desteği

Gerekenler:

- Import batch yönetimi
- Satır bazlı hata raporu
- Audit log yazımı
- Kritik değişiklik görünürlüğü

## 3.4 Backend API Grupları

Önerilen route grupları:

- `/api/auth`
- `/api/users`
- `/api/roles`
- `/api/employees`
- `/api/accounts`
- `/api/activities`
- `/api/sales`
- `/api/expenses`
- `/api/dashboard`
- `/api/imports`
- `/api/audit-logs`

## 3.5 Backend Teknik Kararlar

- DTO zorunlu olacak
- Validation zorunlu olacak
- Soft delete yalnızca gerçekten ihtiyaç olan transaction tablolarda uygulanacak
- Audit alanları ortak base yapı ile yönetilecek
- Dashboard hesapları mümkün olduğunca sorgu bazlı üretilecek

## 4. Frontend Planı

## 4.1 Frontend Hedefi

Frontend, saha personeli için hızlı veri girişi; yönetici için hızlı analiz; operasyon için veri düzeltme deneyimi sunmalıdır.

## 4.2 Frontend Ana Bölümler

- Auth alanı
- Uygulama kabuğu
- Ortak liste ve filtre bileşenleri
- Modül bazlı veri giriş formları
- Dashboard ve grafik ekranları

## 4.3 Frontend Ekran Ağacı

### Auth

- Login

### Dashboard

- Executive Dashboard
- Team Performance
- Product Breakdown
- Expense Analysis

### Employees

- Employee List
- Employee Detail
- Employee Form

### Accounts

- Account List
- Account Detail
- Account Form

### Activities

- Activity List
- Activity Detail
- Activity Create
- Activity Edit

### Sales

- Sale List
- Sale Detail
- Sale Create
- Sale Edit

### Expenses

- Expense List
- Expense Detail
- Expense Create
- Expense Edit

### Administration

- User List
- Role Matrix
- Reference Data Management

### Imports

- Import Upload
- Import History
- Import Error Detail

### Audit

- Audit Log List
- Audit Log Detail

## 4.4 Frontend Geliştirme Fazları

### Faz 1 - Uygulama İskeleti

- Router
- Layout
- Auth guard
- Menü yapısı
- Tema ve temel UI seti

### Faz 2 - Ortak Bileşenler

- Table wrapper
- Filter bar
- Date range picker
- Summary card
- Status badge
- Empty/error/loading state bileşenleri

### Faz 3 - Çekirdek Form ve Liste Ekranları

Öncelik sırası:

1. Activities
2. Sales
3. Expenses
4. Accounts
5. Employees

Gerekçe:

- Sistem değeri ilk olarak saha akışında oluşur.

### Faz 4 - Dashboard

- KPI kartları
- Trend chart'lar
- Personel karşılaştırma tabloları
- Ürün kırılım görselleri

### Faz 5 - Yönetim ve Veri Kalitesi Ekranları

- Import ekranları
- Audit log görüntüleme
- Rol/yetki yönetimi

## 4.5 Frontend UX Prensipleri

- Saha personeli için az tıklama
- Formlarda gereksiz alan kalabalığı olmamalı
- Tarih ve filtre kullanımı tutarlı olmalı
- Liste ekranları export ihtiyacına açık tasarlanmalı
- Yönetici dashboard'unda hem özet hem drill-down mümkün olmalı

## 4.6 Form Tasarım Notları

### Activity Form

Zorunlu alanlar:

- Personel
- Müşteri/firma
- Aktivite tarihi
- İçerik
- Sonuç

Opsiyonel alanlar:

- Temas kişisi
- Konum
- Takip tarihi

### Sale Form

Zorunlu alanlar:

- Personel
- Müşteri/firma
- Ürün
- Satış tarihi

Koşullu alanlar:

- Prim
- APE
- Toplu para
- Aylık ödeme
- Tahsilat

### Expense Form

Zorunlu alanlar:

- Personel
- Tarih
- Masraf türü
- Tutar

## 5. Backend ve Frontend Arasında Sözleşme Prensibi

- API şemaları net tutulmalı
- Liste endpoint'lerinde ortak pagination modeli olmalı
- Enum benzeri alanlar referans endpoint'lerle veya tutarlı sözlük yapısıyla sunulmalı
- Dashboard filtre modeli tek tip olmalı

## 6. Sprint / İş Paketleri Önerisi

Önerilen iş paketleri:

### Paket 1

- Mimari iskelet
- Auth temel yapı
- Employee ve Account temel modeli

### Paket 2

- Activity backend
- Activity frontend

### Paket 3

- Sale backend
- Sale frontend

### Paket 4

- Expense backend
- Expense frontend

### Paket 5

- Dashboard backend
- Dashboard frontend

### Paket 6

- Import
- Audit
- Veri kalite sertleştirme

## 7. Riskler

- Domain açık soruları çözülmeden kodlamaya geçilirse geri dönüş maliyeti artar
- KPI tanımları net değilse dashboard tekrar tekrar değişir
- Frontend formlar ürün tipine göre koşullu alanları yönetmekte karmaşık hale gelebilir
- Excel import erken aşamada zorlanırsa MVP kapsamı sapabilir

## 8. İlk Öneri

En sağlıklı ilk uygulama sırası şudur:

1. ERD'yi karar seviyesine yaklaştır
2. KPI sözlüğünü çıkar
3. Backend solution ve frontend app iskeletini kur
4. Activity modülünü ilk çalışan dikey dilim olarak geliştir
5. Ardından Sales ve Expenses modüllerini ekle
6. Dashboard'u bu ham veriler üzerine oturt

