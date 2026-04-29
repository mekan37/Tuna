# AGENTS.md - Codex Calisma Talimatlari

Bu klasor, mevcut FoxPro/DBF Depo ERP sisteminin .NET + PostgreSQL ile Tuna adli yeni uygulama olarak yeniden yazimi icin referans dokuman setidir.

## Uygulama Adi

Yeni uygulamanin resmi adi Tuna olacaktir. Kod, paket, servis, API, dokuman ve veritabani adlandirmalarinda bu isim esas alinmalidir.

## Okuma Sirasi

1. `README.md`
2. `00-kurulum-ve-ajan-rehberi.md`
3. `01-yonetici-ozeti-ve-sinirlar.md`
4. `02-fiziksel-mimari.md`
5. `03-moduller-ve-is-akislari.md`
6. `04-hedef-mimari-veri-modeli.md`
7. `05-dbf-envanteri.md`
8. `06-ana-dbf-semalari.md`
9. `07-opa-menu-analizi.md`
10. `08-setup-paketleri.md`
11. `09-sonuc.md`

## Kod Uretim Ilkeleri

- DBF dosya yapisini birebir kopyalama; hedef PostgreSQL modeli normalize, transaction guvenli ve audit izlenebilir olmalidir.
- Buyuk hareket tablolarinda partition ve indeks stratejisi tasarla.
- E-fatura, ITS/UTS/PTS, yazdirma ve dis entegrasyonlari worker/outbox/idempotency modeliyle kur.
- Aktarim icin once staging, sonra temiz hedef tablo, sonra mutabakat raporu uret.
- Belirsiz is kurallarini `Dogrulanacak` olarak isaretle; EXE ic davranislarini kesin kabul etme.

## Referans Bicimi

Karar verirken dosya ve bolum basligini birlikte belirt. Ornek: `04-hedef-mimari-veri-modeli.md > 12. Aktarim Stratejisi`.


