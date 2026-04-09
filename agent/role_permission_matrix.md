# Role Permission Matrix

## 1. Amac

Bu belge, MVP icin temel rol sinirlarini ve veri gorunurlugunu netlestirir.

## 2. Roller

- `Admin`
- `Manager`
- `SalesManager`
- `Operations`
- `FieldSales`
- `CallCenter`

## 3. Rol Bazli Ozet

### 3.1 Admin

- tum modulleri gorur
- kullanici ve rol yonetir
- referans verileri yonetir
- kritik duzeltmeleri yapabilir

### 3.2 Manager

- tum operasyonu ve dashboard'u gorur
- ekip performansini izler
- satis ve masraf raporlarini gorur
- genis veri gorunurlugune sahiptir

### 3.3 SalesManager

- lead havuzunu gorur
- lead atamasi yapar
- saha ekip performansini gorur
- satislar uzerinde yonetsel duzeltme yetkisi olabilir

### 3.4 Operations

- account, activity, sale ve expense kayitlarini operasyonel olarak izler
- eksik kayitlari tamamlar
- satista cekirdek alanlari sinirsiz duzenleyemez

### 3.5 FieldSales

- varsayilan olarak yalnizca kendi lead, activity, sale ve expense kayitlarini gorur
- kendine atanan leadleri takip eder
- kendi satis ve masraf kayitlarini olusturur

### 3.6 CallCenter

- lead olusturur ve gunceller
- lead arastirma notlari ve kontak bilgisi girer
- sahaya ait activity, sale ve expense modullerinde islem yapmaz

## 4. Modul Bazli Yetki Matrisi

| Modul | Admin | Manager | SalesManager | Operations | FieldSales | CallCenter |
| --- | --- | --- | --- | --- | --- | --- |
| Auth / User / Role | Tam | Okuma sinirli | Yok | Yok | Yok | Yok |
| Lead | Tam | Okuma | Tam | Okuma / destek | Kendi atananlari | Olustur / guncelle |
| Lead Assignment | Tam | Okuma | Tam | Destek | Yok | Yok |
| Account | Tam | Okuma | Okuma | Olustur / guncelle | Kendi baglaminda | Yok |
| Activity | Tam | Okuma | Okuma | Izleme / duzeltme | Kendi kayitlari | Yok |
| Sale | Tam | Okuma | Yonetsel duzeltme | Sinirli duzeltme | Kendi kayitlari | Yok |
| Expense | Tam | Okuma | Okuma | Izleme | Kendi kayitlari | Yok |
| Dashboard | Tam | Tam | Ekip ve saha | Operasyonel gorunum | Kendi ozetleri | Lead odakli ozet |
| Lookups | Tam | Okuma | Okuma | Okuma | Okuma | Okuma |
| Import | Tam | Okuma | Okuma | Isletimsel kullanim | Yok | Yok |
| Audit | Tam | Okuma | Sinirli | Sinirli | Yok | Yok |

## 5. Veri Scope Kurali

- `FieldSales` kendi sahibi oldugu veya kendisine atanmis kayitlari gorur
- `CallCenter` yalnizca lead tarafi verilerini gorur
- `Operations` operasyonel kayitlari genis scope ile gorur
- `SalesManager` lead ve saha akislarini takim seviyesinde gorur
- `Manager` butun KPI ve raporlari gorur

## 6. Sonuc

Bu belge MVP icin temel rol ve yetki referansidir. Kodlama asamasinda endpoint ve UI gorunurluk kurallari bu matrise gore uygulanacaktir.
