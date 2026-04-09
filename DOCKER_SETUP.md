# Docker Kurulumu

Bu proje gelistirme ortami icin PostgreSQL ve pgAdmin'i Docker ile calistirir.

## Servisler

- PostgreSQL
  - Container: `insurance-management-db`
  - Port: `5433`
  - Veritabani: `insurance_management`
- pgAdmin
  - Container: `insurance-management-pgadmin`
  - Port: `5050`

## Ilk Kurulum

Proje kok klasorunde:

```powershell
docker compose up -d
```

Container durumunu kontrol etmek icin:

```powershell
docker compose ps
```

Loglari izlemek icin:

```powershell
docker compose logs -f postgres
```

## pgAdmin Girisi

Adres:

```text
http://localhost:5050
```

Varsayilan giris:

- E-posta: `admin@insurance.com`
- Sifre: `admin123`

pgAdmin icinden yeni server eklerken:

- Host: `postgres`
- Port: `5432`
- Database: `insurance_management`
- Username: `postgres`
- Password: `postgres`

## Uygulamayi PostgreSQL ile Calistirma

Docker servislerini kaldirdiktan sonra MVC uygulamayi ayaga kaldir:

```powershell
dotnet run --project .\InsuranceManagement.Web\InsuranceManagement.Web.csproj
```

Uygulama host makineden container'a su baglanti ile erisir:

- Host: `localhost`
- Port: `5433`
- Database: `insurance_management`
- Username: `postgres`
- Password: `postgres`

Uygulama Development ortaminda otomatik olarak:

- `PostgreSql` provider kullanir
- `Database.Migrate()` cagirir
- seed veriyi yukler

## Sifirlama

Sadece container'lari durdurmak:

```powershell
docker compose down
```

Veritabani verisini de tamamen sifirlamak:

```powershell
docker compose down -v
```
