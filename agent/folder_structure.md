# Folder Structure Proposal

## 1. Amaç

Bu belge, proje için önerilen repository ve solution klasör yapısını tanımlar. Amaç; backend, frontend ve proje hafızası belgelerini birbirinden net şekilde ayırmak, büyüdükçe dağılmayacak bir iskelet kurmaktır.

Bu aşamada yalnızca öneri sunulmaktadır. Klasörler burada tanımlanan sıraya göre daha sonra oluşturulabilir.

## 2. Repository Kök Yapısı

Önerilen üst düzey yapı:

```text
insurance_management/
  agent/
  backend/
  frontend/
  docs/
  scripts/
```

### Klasör amaçları

- `agent/`: proje hafızası, karar belgeleri, görev planları
- `backend/`: ASP.NET Core Web API solution ve ilgili projeler
- `frontend/`: React + TypeScript uygulaması
- `docs/`: kullanıcı dokümanları, API sözlüğü, import şablon açıklamaları
- `scripts/`: veri hazırlama, import yardımcı scriptleri, operasyonel araçlar

## 3. Agent Klasörü

Önerilen içerik:

```text
agent/
  active_tasks.md
  project_design.md
  rules.md
  erd.md
  folder_structure.md
  delivery_plan.md
```

Not:

- `agent` klasörü yaşayan hafıza alanı gibi düşünülmelidir.
- Kod üretiminden önce ve sonra önemli kararlar burada güncellenmelidir.

## 4. Backend Klasörü

Önerilen yapı:

```text
backend/
  src/
    InsuranceManagement.API/
    InsuranceManagement.Application/
    InsuranceManagement.Domain/
    InsuranceManagement.Infrastructure/
    InsuranceManagement.Contracts/
  tests/
    InsuranceManagement.UnitTests/
    InsuranceManagement.IntegrationTests/
  InsuranceManagement.sln
```

### Backend proje sorumlulukları

#### `InsuranceManagement.API`

- Controller veya endpoint katmanı
- Authentication/Authorization entegrasyonu
- Request/response mapping
- Swagger/OpenAPI
- Global exception handling

#### `InsuranceManagement.Application`

- Use case ve application service katmanı
- DTO'lar
- Validation
- Query/command akışları
- Dashboard ve rapor use case'leri

#### `InsuranceManagement.Domain`

- Entity'ler
- Value object'ler
- Domain servisleri
- İş kuralları
- Enum ve sabitler

#### `InsuranceManagement.Infrastructure`

- EF Core DbContext
- Repository implementasyonları
- PostgreSQL entegrasyonu
- Audit ve import altyapısı
- Kimlik doğrulama ve dış servis adaptörleri

#### `InsuranceManagement.Contracts`

- API contract modelleri
- İstek/cevap şemaları
- Ortak response tipleri

Bu katman opsiyoneldir. Eğer proje küçük kalacaksa DTO'lar doğrudan `Application` içinde yönetilebilir.

## 5. Backend Modülleme Yaklaşımı

Katman bazlı yapı korunurken modül mantığı da dosya düzenine yansıtılmalıdır.

Örnek modüller:

- Auth
- Employees
- Accounts
- Activities
- Sales
- Expenses
- Dashboard
- Imports
- AuditLogs

Örnek iç düzen:

```text
InsuranceManagement.Application/
  Features/
    Activities/
      Commands/
      Queries/
      Dtos/
      Validators/
    Sales/
    Expenses/
    Dashboard/
```

Bu yapı, hem katmanlı hem de modül odaklı okunabilirlik sağlar.

## 6. Frontend Klasörü

Önerilen yapı:

```text
frontend/
  public/
  src/
    app/
    routes/
    layouts/
    pages/
    features/
    components/
    services/
    lib/
    hooks/
    types/
    styles/
  tests/
```

## 7. Frontend İç Yapı Detayı

### `app/`

- Uygulama bootstrap
- Provider'lar
- Router kurulumu
- Global store veya query client kurulumu

### `routes/`

- Route tanımları
- Protected route mantığı
- Rol bazlı erişim geçişi

### `layouts/`

- Main layout
- Auth layout
- Dashboard layout

### `pages/`

Sayfa seviyesindeki route bileşenleri.

Örnek:

- LoginPage
- DashboardPage
- ActivityListPage
- ActivityCreatePage
- SalesListPage
- ExpensesListPage

### `features/`

Modül bazlı UI ve iş akışı parçaları.

Örnek:

```text
features/
  auth/
  employees/
  accounts/
  activities/
  sales/
  expenses/
  dashboard/
  imports/
```

Her feature içinde şunlar olabilir:

- api
- components
- forms
- schemas
- types

### `components/`

Ortak tekrar kullanılabilir bileşenler:

- Table
- FilterBar
- SummaryCard
- Modal
- FormField

### `services/`

- API client
- Auth service
- Dashboard service
- Export/import helper servisleri

### `lib/`

- Utility fonksiyonlar
- Tarih/para formatlayıcılar
- Sabitler

### `hooks/`

- Ortak custom hook'lar

### `types/`

- Global type tanımları

### `styles/`

- Tema değişkenleri
- Ortak stil katmanı

## 8. Docs Klasörü

Önerilen yapı:

```text
docs/
  api/
  product/
  reports/
  imports/
```

İçerik örnekleri:

- API endpoint sözlüğü
- KPI tanım belgeleri
- import kolon eşleme tabloları
- kullanıcı eğitim notları

## 9. Scripts Klasörü

Önerilen amaçlar:

- Excel dönüştürme yardımcı scriptleri
- seed veya lookup veri hazırlama
- bakım scriptleri
- lokal geliştirme yardımcıları

Not:

- Scripts klasörü uygulama iş mantığının yerine geçmemelidir.
- Kalıcı iş kuralları backend içinde tutulmalıdır.

## 10. Alternatif Minimal Yapı

Eğer ekip çok küçük ve hız kritikse şu daha sade yapı ile başlanabilir:

```text
insurance_management/
  agent/
  backend/
  frontend/
```

Sonrasında `docs` ve `scripts` klasörleri ihtiyaç oldukça eklenebilir.

## 11. Önerilen İsimlendirme Kuralları

- Backend proje isimleri: `InsuranceManagement.*`
- Frontend feature klasörleri: çoğul ve domain odaklı
- Markdown dokümanları: snake_case veya açık tekil adlar
- Modül isimleri frontend ve backend'de mümkün olduğunca aynı tutulmalı

## 12. İlk Uygulama Sırası

Klasörler oluşturulurken önerilen sıra:

1. `agent` dokümanlarını tamamla
2. `backend` solution iskeletini kur
3. `frontend` uygulama iskeletini kur
4. `docs` altında KPI ve API sözlüğü aç
5. `scripts` ihtiyaca göre ekle

