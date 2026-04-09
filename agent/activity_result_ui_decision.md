# Activity Result UI Decision

## 1. Amac

Bu belge, saha personelinin aktivite girerken sonuc bilgisini nasil sececegini netlestirir. Amac; formu dogal, hataya kapali ve operasyon gercegine uygun hale getirmektir.

## 2. Alinan Urun Karari

Aktivite formunda sonuc girisi iki asamali olacaktir.

Akis:

1. Once kullaniciya "Gorusuldu mu?" sorusu sorulur.
2. Eger cevap `Gorusulmedi` ise ikinci sonuc alani acilmaz.
3. Eger cevap `Gorusuldu` ise ikinci adimda gorusmenin sonucu sorulur.

Ikinci adimda secilebilecek sonuc tipleri:

- `Olumlu`
- `Olumsuz`
- `Ertelendi`
- `Satis Oldu`

## 3. Neden Bu Karar Alindi

Bu karar su nedenlerle alinmistir:

- Formu saha personeli icin daha dogal hale getirmek
- Veri giris hatalarini azaltmak
- "Gorusulmedi ama olumlu" gibi anlamsiz kombinasyonlari engellemek
- KPI ve dashboard mantigini daha temiz kurmak

## 4. Form Davranisi

MVP form davranisi:

- Ilk alan zorunludur: `Gorusuldu mu?`
- `Gorusulmedi` secildiginde ikinci alan gizli veya pasif kalir
- `Gorusuldu` secildiginde ikinci alan zorunlu hale gelir
- `Satis Oldu` secilse bile ayrica gercek bir satis kaydi olusturulmasi gerekir

## 5. Veri Modeli Etkisi

UI sade olsa da veri modeli iki ayri kavrami korur:

- `contactStatus`
- `outcomeStatus`

Yani ekranda dogal dil kullanilir, veride ise ayri alanlar saklanir.

## 6. Validation Kurali

- `contactStatus = NOT_CONTACTED` ise `outcomeStatus` bos olmalidir
- `contactStatus = CONTACTED` ise `outcomeStatus` zorunlu olmalidir

## 7. Sonuc

Bu proje icin kabul edilen UI karari sudur:

- Aktivite formu once "Gorusuldu mu?" diye sorar
- Yalnizca gorusme olduysa ikinci adimda gorusme sonucu sorulur
- Bu yapi MVP icin standart davranis kabul edilir
