# Kurulum ve Ajan Rehberi

Bu klasor, mevcut FoxPro/DBF Depo ERP sistemini sifirdan Tuna adli modern bir uygulama olarak yeniden kurmak icin Codex ve Claude tarafindan okunacak bolunmus referans setidir.

## Uygulama Adi

Yeni sistemin resmi uygulama adi Tuna olacaktir. Ajanlar proje, servis, dokuman ve varsayilan veritabani isimlendirmelerinde Tuna adini esas almalidir. Klasor ve uygulama adlari Turkce ASCII yazilir; i, u, g, s, c, o karakterleri kullanilir.

## Kullanim Sirasi

1. `README.md` dosyasindan kapsami ve dosya haritasini oku.
2. Once `01-yonetici-ozeti-ve-sinirlar.md` ile kanit seviyelerini ve sistem sinirlarini anla.
3. `02-fiziksel-mimari.md` ile mevcut klasorleri, calisma paketlerini ve veri hacmini kavra.
4. `03-moduller-ve-is-akislari.md` ile is modullerini ve ana surecleri cikar.
5. `04-hedef-mimari-veri-modeli.md` ile .NET/PostgreSQL hedef tasarimini baz al.
6. `05-dbf-envanteri.md` ve `06-ana-dbf-semalari.md` dosyalarini migration ve veri sozlugu icin referans kullan.
7. `07-opa-menu-analizi.md` ve `08-setup-paketleri.md` dosyalarini ekran, yardimci uygulama ve entegrasyon kapsamlarini belirlemek icin kullan.
8. `09-sonuc.md` dosyasindaki kritik kararlarla uygulama planini kapat.

## Ajan Kurallari

- Eski DBF tablo yapisini birebir yeni veritabanina kopyalama; hedef modelde normalize tablo, transaction, audit, outbox ve partition kullan.
- Kaynak kod bulunmadigi varsayimiyla hareket et; EXE davranislari icin rapordaki kanit seviyesini koru.
- Is kurali bilinmiyorsa dosya/tablo adi uzerinden kesin hukum verme; `Dogrulanacak` olarak isaretle.
- Aktarim tasarlarken once staging tablo, sonra temiz hedef tablo, sonra mutabakat raporu uret.
- E-fatura, ITS/UTS/PTS, yazdirma ve entegrasyon islemlerini background worker + idempotency modeliyle ele al.
- Finans, fatura, stok ve karekod hareketlerinde audit zorunludur.

## Hedef Kurulum Iskeleti

- Backend: ASP.NET Core / .NET 10, moduler katmanli mimari.
- Veritabani: PostgreSQL, sema bazli ayrim, buyuk hareket tablolarinda partition.
- Isci: entegrasyon, yazdirma, aktarim ve rapor yenileme isleri.
- Servis: stok, cari, satis, alis, finans, e-fatura, tracktrace, raporlama, admin.
- Raporlama: SQL view/materialized view + merkezi rapor metadata.
- Loglama: structured log, audit tablolari, hata ve mutabakat kayitlari.

## Dosya Uretim Notu

`tarama2.md` orijinal kaynak olarak bu klasore kopyalanmistir. Diger `.md` dosyalari ayni raporun bolunmus, ajanlarin daha kolay referans alabilecegi parcalaridir.


