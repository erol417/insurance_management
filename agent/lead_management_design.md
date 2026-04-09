# Lead Management Design

## 1. Amac

Bu belge, call center tarafinda bulunan potansiyel musterilerin sisteme nasil alinacagini, satis muduru tarafindan saha personeline nasil atanacagini ve bu kayitlarin ne zaman aktiviteye donusecegini tanimlar.

Bu dokumanin amaci, mevcut `account -> activity -> sale` akisini bir adim geriye cekerek gercek operasyonu dogru modellemektir.

## 2. Is Problemi

Mevcut operasyon akisi yalnizca saha ziyaretinden sonraki kayitlari ele alir. Oysa gercek hayatta:

- call center potansiyel musteri arastirir
- kontak bilgisi toplar
- potansiyel liste olusturur
- satis muduru bu kayitlari saha personeline atar
- saha personeli ziyaret eder
- ziyaret sonrasi activity olusur

Bu adimlar modellenmediginde acik is mantigi, atama disiplini ve lead kalitesi olculemez.

## 3. Temel Karar

Sisteme `Lead Management` adinda ayri bir modul eklenmelidir.

Bu modul:

- activity'den ayridir
- account'tan once veya account ile birlikte var olabilir
- saha personelinin ziyaret etmedigi potansiyel havuzu temsil eder

## 4. Onerilen Akis

1. Call center yeni lead kaydi olusturur.
2. Potansiyel musteri bilgileri ve kontak notlari girilir.
3. Satis muduru lead'i degerlendirir.
4. Uygun lead saha personeline atanir.
5. Saha personeli `Atanan Leadlerim` ekraninda bu kaydi gorur.
6. Ziyaret gerceklestiginde lead activity'ye donusur.
7. Gerekirse account kaydi olusturulur veya mevcut account ile eslestirilir.
8. Sonraki surec activity, sale ve expense modulleriyle devam eder.

## 5. MVP Icin Onerilen Lead Status'leri

- `NEW`
- `RESEARCHED`
- `CONTACT_FOUND`
- `READY_FOR_ASSIGNMENT`
- `ASSIGNED`
- `VISIT_SCHEDULED`
- `VISITED`
- `DISQUALIFIED`
- `CONVERTED_TO_ACTIVITY`

## 6. Gerekli Alanlar

### Lead

- code
- potential name
- phone
- email
- city
- district
- note
- source
- current status
- owner user
- linked account nullable
- converted activity nullable

### Lead Assignment

- lead id
- assigned employee id
- assigned by user id
- assigned at
- priority
- due date
- assignment note

## 7. MVP Ekranlari

- Leads List
- Lead Detail
- Lead Form
- Lead Assignment Queue
- My Assigned Leads

## 8. Sonuc

Bu modul eklenmeden sistem saha operasyonunu kismen takip eder. Bu modul eklendiginde ise call center'dan saha satisina kadar uzanan tam operasyon zinciri izlenebilir hale gelir.
