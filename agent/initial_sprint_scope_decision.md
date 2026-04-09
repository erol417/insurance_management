# Initial Sprint Scope Decision

## 1. Amac

Bu belge, ilk sprintte hangi temel alanlarin birlikte acilacagini netlestirir.

## 2. Alinan Karar

Ilk sprintte auth ile birlikte temel user/role yonetimi de acilacaktir.

## 3. Kapsam

Ilk sprintte hedeflenen temel teslimler:

- backend solution iskeleti
- frontend uygulama iskeleti
- login ve oturum dogrulama temeli
- user ve role veri modeli
- temel authorization omurgasi
- ortak API response ve hata standardi
- PostgreSQL baglanti ve migration temeli

## 4. Neden Bu Karar Alindi

Bu karar su nedenlerle alinmistir:

- rol bazli gorunurluk kurallari erken kurulmazsa moduller tekrar ele alinmak zorunda kalir
- lead, activity, sale ve expense modullerinin hemen hepsi kullanici ve yetki baglamina dayanir
- sonradan auth eklemek yerine erken kurmak daha az refactor riski tasir

## 5. Sonuc

Bu proje icin kabul edilen ilk sprint karari sudur:

- auth ve temel user/role yonetimi ayri fazlara bolunmeyecek
- ilk sprintte birlikte acilacaktir
