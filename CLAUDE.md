# CLAUDE.md - Claude Calisma Talimatlari

Bu proje klasoru, `tarama2.md` kaynak raporunun ajanlar icin bolunmus halidir. Amac, eski FoxPro/DBF depo ERP sistemini sifirdan modern .NET + PostgreSQL mimarisiyle tasarlamak ve gelistirmektir.
## Uygulama Adi

Yeni yazilacak uygulamanin resmi adi Tuna olacaktir. Claude tarafindan uretilen mimari, kod, dokuman ve isimlendirme onerileri bu adi temel almalidir.


## Mutlaka Oku

- Genel harita: `README.md`
- Ajan kurallari: `00-kurulum-ve-ajan-rehberi.md`
- Hedef mimari: `04-hedef-mimari-veri-modeli.md`
- Veri kaynaklari: `05-dbf-envanteri.md`, `06-ana-dbf-semalari.md`
- Ekran ve entegrasyon izleri: `07-opa-menu-analizi.md`, `08-setup-paketleri.md`

## Varsayim Disiplini

Rapor kaynak koddan degil, dosya/DBF/EXE izlerinden uretilmistir. Bu nedenle kesin kanit, guclu cikarim ve dogrulanacak alanlari birbirinden ayir. Yeni sistem tasariminda raporu kaynak kanit olarak kullan, fakat eksik is kurallarini kullanici senaryosu veya ekran akisi ile dogrulanacak kabul et.

## Hedef Mimari Beklentisi

- Backend: ASP.NET Core / .NET
- Veritabani: PostgreSQL
- Entegrasyon: background worker, outbox, retry, idempotency
- Loglama: structured log + audit tablolari
- Aktarim: staging + hedef model + mutabakat
- Raporlama: view/materialized view + rapor metadata



