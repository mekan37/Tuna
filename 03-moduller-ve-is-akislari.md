# Moduller ve Is Akislari

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

ERP modul kapsamlarini ve satis, alis, e-fatura, karekod gibi ana is akisi izlerini toplar.

## Kaynak Bolumler

- tarama2.md bolum 6
- tarama2.md bolum 7

## 6. Programin Tum Ana Modulleri

### 6.1 Core / Sistem Parametreleri

- Firma, sube, kullanici, belge numarasi, dosya yolu, yazici, kilit ve parametre tablolarini kapsar.
- Kanitlar: `FIRMA`, `BELGENO`, `KONTKULL`, `MSJKULL`, `nodelist`, `KLASOR.MEM`, bilgisayar adli `.XM/.XY` kilit/durum dosyalari.
- Yeni sistemde `core.companies`, `core.branches`, `core.users`, `core.permissions`, `core.number_sequences`, `core.workstations` tablolarina ayrilmalidir.

### 6.2 Stok / Urun / Barkod / Miad

- Ana urun karti `URUNLER`; barkod eslesmesi `BARKOD`; uretici/firma `URETICI`; muadil `MUADIL`; stok zaman damgasi `STKZAMAN`; miad ve parti `PARTILER`, `ILACMIAD`, `MIADHRK` ile izleniyor.
- Ilac terminolojisi baskin: `ILACKODU`, miad, mal fazlasi, fiyat, KDV, uretici, muadil, barkod.
- Yeni sistemde urun karti ile fiyat, barkod, miad, parti ve stok hareketi ayrilmali; karakter olarak tutulan miktar/fiyat alanlari numeric yapilmalidir.

### 6.3 Alis / Tedarik

- `FATURAG` alis fatura basligi, `DETAYG` alis fatura detayidir.
- Detaylarda tedarikci, urun, giris miktari, mal fazlasi, alis fiyati, maliyet, iskonto, KDV, miad, seri, kutu tipi ve sube izleri var.
- Bu modul stok girisini, maliyet guncellemesini ve finans/cari borc hareketini tetikler.

### 6.4 Satis / Siparis / Faturalama

- `SIPARIS`, `SIPHAR`, `BEKLEYEN`, `FATURAS`, `DETAYS`, `SATISLAR`, `IRSALIYE`, `BELGENO` ana tablolardir.
- Fatura basligi 70 alanli, detay 30 alanli ve milyonlarca hareketlidir. Bu, sistemin ana operasyon merkezinin satis oldugunu gosterir.
- Akis: cari secimi -> siparis -> stok/kampanya/miad kontrolu -> bekleyen/sevkiyat -> fatura -> cari/finans/stok/e-fatura/karekod guncelleme.

### 6.5 Cari / Finans / Tahsilat

- `FINANS`, `CARIOZT`, `TAHSILCI`, `PARSEL`, `SYMFTR`, `SYMHAR`, `TAKAS`, `cephrk`, `CEPILCAY` finans ve cari izlerini gosteriyor.
- `KKDETAY`, `KKLC0001`, `KKLG0001`, `KKPAKET`, `KKOZET` cok yuksek hacimli oldugu icin kredi karti/karekod/odeme paket hareketlerinin kritik parcasi olarak ayrica modellenmelidir.
- Yeni sistemde hareket tablosu immutable olmalidir; bakiye tablolar view/materialized view veya kontrollu ozet tablosu olarak uretilmelidir.

### 6.6 Kampanya / Fiyat / Puan

- `KAMPANYA`, `ACD_KAMPANYA`, `FIYATAKT`, `FIYAT_ADLARI`, `EKPUAN`, `PLASPUAN`, `PLSSATIS` alanlari kampanya, fiyat, mal fazlasi, puan ve plasiyer performansini gosteriyor.
- `ACD_KAMPANYA` 1.9M kayit ile kampanya degisim/aksiyon tarihcesi gibi davranir.
- Yeni sistemde kampanya kural motoru gerekir: kosul, sonuc, gecerlilik tarihi, urun/cari kapsam, mal fazlasi, iskonto, puan etkisi.

### 6.7 E-Fatura / E-Arsiv / Resmi Belge

- `EFATURA`, `EARSIV`, `EFATKULL`, `EFTRKULL`, `EFATNOT`, `YANCIEF`, `YANCIEA` resmi belge entegrasyon alanidir.
- UUID, VKN/TCKN, etiket, durum, gonderim/bildirim tarihi, portal sorgu ve nusha goruntuleme davranislari mevcut.
- Hedefte belge durumu event history olarak tutulmali; XML/PDF arsiv dosyalari veri tabaninda degil object storage/dosya deposunda saklanmali, DB sadece referans tutmalidir.

### 6.8 ITS / UTS / PTS / Karekod / 2D Kontrol

- `PTSMAIN`, `PTSNUM1`, `ptsNum2`, `PTSNUM00`, `UTSLOG00`, `UTSDTY00`, `UTSSYYYYMMDD`, `KKDETAY`, `KKLC0001`, `KKLG0001`, `KKPAKET` bu alanin ana kanitlaridir.
- `kontrol2d.exe`, `its.exe`, `TEMP2D`, `F:\LOG\*_kontrol2d_*`, `F:\LOG\*_its_*` loglari bu modulu destekliyor.
- Bu veri alaninda en buyuk tablo `PTSNUM1` yaklasik 2 GB ve 25M kayit. PostgreSQL hedefinde tarih/islem tipi/sube bazli partition, uygun indeks ve bulk insert stratejisi zorunlu.

### 6.9 Depo Operasyon / Sevkiyat / Yazici

- `TEVZILER`, `PARSEL`, `REYONKxx`, `TSSIPANA`, `TSSIPDTY`, `YAZICI\Yxx` klasorleri sevkiyat, toplama, parsel, reyon ve cikti kuyrugu yapisini gosteriyor.
- Yeni sistemde mobil depo ekranlari, barkod okuyucu entegrasyonu, print queue ve fatura/irsaliye cikti servisi tasarlanmalidir.

### 6.10 Raporlama

- `RAPORTNM.*`, `RAPORTNM` klasoru, `wkhtmltopdf`, `FATURADOKUM`, program bilgi notundaki rapor maddeleri raporlama katmanini gosterir.
- Hedefte raporlar uygulama koduna gomulmemeli; SQL view/materialized view + rapor tanimi + yetki modeliyle ele alinmalidir.

### 6.11 Bildirim / Mesaj / Mail / SMS

- `MESAJLOG`, `MAILLOG`, `SMSLOG`, `MSJKULL`, `F:\LOG\kullanici.log` dosyalari kullanici ve servis bildirimlerini gosteriyor.
- Yeni sistemde bildirim outbox pattern ile yazilmali; tekrar gonderim, hata, durum ve kanal bilgisi izlenmelidir.

### 6.12 Log / Audit / Kilit

- `GIRISLOG`, `KILITLOG`, `islemlog`, `hatalog`, `RISKLOG`, `F:\LOG\errors_main.log`, API servis loglari ve bilgisayar adli kilit dosyalari mevcut.
- PostgreSQL hedefinde audit log immutable olmalidir; uygulama loglari OpenTelemetry/structured logging ile merkezi toplanmalidir.

## 7. Ana Is Akislari

### 7.1 Siparis ve Satis Faturasi

1. Cari/musteri secilir; borc, limit, statu, fiyat tipi, puan ve kampanya kosullari kontrol edilir.
2. Urun barkod veya kodla bulunur; stok, miad, parti, bloke, reyon ve kampanya kontrol edilir.
3. Siparis basligi/detayi olusur; gerekirse `BEKLEYEN` veya sevkiyat/parsel/reyon akisi devreye girer.
4. Fatura kesiminde belge numarasi alinir, `FATURAS` ve `DETAYS` benzeri baslik/detay hareketleri yazilir.
5. Stok dusulur, finans/cari hareket olusur, kampanya/puan etkileri islenir.
6. E-fatura/e-arsiv gerekiyorsa resmi belge kaydi ve gonderim kuyruğu olusur.
7. Karekod/PTS/UTS bildirimleri ve yazici/PDF ciktilari uretilir.

### 7.2 Alis Faturasi ve Stok Girisi

1. Tedarikci/firma ve belge bilgisi girilir.
2. Alis detaylarinda urun, miktar, mal fazlasi, fiyat, iskonto, KDV, miad, seri/parti islenir.
3. Stok artar, maliyet/son maliyet guncellenir, finans borc hareketi olusur.
4. Miad/parti ve takip sistemi kayitlari olusur.

### 7.3 E-Fatura Durum Sorgu

1. Fatura kaydi resmi belge kuyruguna duser.
2. Mukellef listesi ve etiket bilgisi kontrol edilir.
3. Gonderim/durum sorgu yardimci entegrasyon moduluyle yapilir.
4. UUID, durum, hata, portal/nusha bilgileri saklanir.

### 7.4 Karekod / PTS / UTS

1. Barkod/karekod okuma veya toplu servis cevabi alinir.
2. Seri/lot/paket numarasi PTS/UTS/KK tablolarina yazilir.
3. Hatalar `F:\LOG` ve DBF log tablolarina kaydedilir.
4. Yeni sistemde bu akisin idempotent olmasi gerekir: ayni bildirim iki kez gelirse cift kayit olusmamalidir.

