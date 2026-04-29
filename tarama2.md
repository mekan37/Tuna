# Tarama2 - FoxPro/DBF Depo ERP Sistemi Tam Teknik Analiz ve Yeniden Yazim Raporu

- Rapor tarihi: 2026-04-29 16:45:16
- Kapsam: Mevcut sisteme dokunmadan, dosya/DBF metadata, klasor yapisi, baslatma betikleri, dagitim dosyalari ve log izleri uzerinden analiz.
- Degistirilen tek dosya: `C:\Users\Mustafa\Desktop\tarama2.md`.
- Ana sistem kokleri: `C:\OPA`, `F:\DEPO`, `F:\BATCH`, `F:\LOG`.
- Ana veri kokleri: `F:\DEPO\DATA\01`, `F:\DEPO\DATA\02`, `F:\DEPO\DATA\03`, `F:\DEPO\data120...data926`.
- Mevcut teknoloji: `C:\OPA\OPA.EXE` ana uygulamasi, Visual FoxPro 9 runtime, DBF/CDX/FPT/IDX dosyalari, VBS baslatma/guncelleme, merkezi `F:\BATCH` dagitimi, yardimci entegrasyon EXE dosyalari.
- Hedef teknoloji: .NET backend, PostgreSQL, background worker, modern raporlama, kontrollu ETL/migration ve denetlenebilir entegrasyon katmani.

## 1. Kanit Seviyesi ve Sinirlar

Bu rapor program kaynak kodu okunarak degil, calisan sistem izlerinden ve DBF dosya semalarindan uretilmistir. Bu nedenle rapordaki bulgular uc seviyede isaretlenmelidir:

| Seviye | Anlam | Ornek |
|---|---|---|
| Kesin | Dosya, tablo, alan veya betik dogrudan goruldu | `F:\DEPO\DATA\01\FATURAS.DBF`, `CONFIG.FPW`, `Baskentw.vbs` |
| Guclu Cikarim | Tablo/klasor/log adlari ve hacim islevi cok net gosteriyor | `EFATURA` = e-fatura belge kaydi, `PTSNUM1` = PTS/karekod numara havuzu |
| Dogrulanacak | EXE ic davranisi veya ekran akisi kaynak kod olmadan tam kanitlanamaz | `sqlnop.exe`, `opa.exe`, ana `BASKENT.EXE` davranisi |

Onceki analizde ana EXE eski `BASKENT.EXE` varsayimi ile aranmis ve bulunamadi. Kullanici duzeltmesi ve sistem dogrulamasina gore guncel ana uygulama `C:\OPA\OPA.EXE` dosyasidir. Calisan surec olarak `opa.exe` goruldu; urun adi `Opa Depo Otomasyonu`, surum `3.2.9140`, son degisim `2026-04-28 16:00:23`.

## 2. Yonetici Ozeti

Sistem, ecza deposu/toptan satis odakli bir ERP yapisidir. Ana fonksiyonlar stok, ilac/urun karti, barkod, miad/parti, alis, satis, siparis, fatura, cari hesap, finans, cek/senet/kredi karti, kampanya, puan, e-fatura/e-arsiv, ITS/UTS/PTS/karekod, sevkiyat, sayim, raporlama, bildirim ve denetim/log modulleridir.

Veri mimarisi dosya tabanlidir: kullanicilar veya yardimci uygulamalar ag/paylasim diski uzerindeki DBF dosyalarina dogrudan erisiyor. Bu yapi hizli devreye alinabilir fakat modern sistem icin risklidir: transaction butunlugu sinirli, kilitlenme hassas, indeks bozulmasi olasi, buyuk tablolar 2 GB sinirina yaklasiyor, log/arsiv tablolarinda tarih bazli tablo cogalmasi var.

Yeni .NET + PostgreSQL sisteminde bu modelin aynen kopyalanmasi dogru olmaz. Dogru hedef: normalize edilmis ana tablolar, tarih/olay bazli partition, idempotent entegrasyonlar, merkezi audit log, background worker ve rapor view/materialized view yapisidir.

## 3. Mevcut Fiziksel Mimari

### 3.1 Klasorlerin Gorevi

| Klasor | Gorev | Yeniden Yazim Karsiligi |
|---|---|---|
| `F:\DEPO` | Ana veri ve operasyon kok dizini | PostgreSQL ana veri tabani + dosya depolama |
| `F:\DEPO\DATA\01` | Aktif ana sirket/sube veri seti olarak gorunuyor | `core`, `sales`, `purchase`, `inventory`, `finance`, `einvoice`, `tracktrace` semalari |
| `F:\DEPO\DATA\02`, `DATA\03` | Ek sube/firma/donem veri alanlari | Tenant/sube/firma ayrimi veya partition |
| `F:\DEPO\data120...data926` | Arsiv/donem/sube kopyalari | Arsiv partitionlari veya readonly archive schema |
| `F:\DEPO\Temp` | Gecici DBF/IDX/MEM uretimi | Uygulama cache/temp table/background job workspace |
| `F:\DEPO\TEMP2D` | Karekod/2D gecici islem alani | Tracktrace worker temp/staging |
| `F:\DEPO\LOG` | DBF tabanli uygulama loglari | `audit` semasi + merkezi log sistemi |
| `F:\LOG` | Metin/API/servis loglari | Structured logs + log retention |
| `F:\DEPO\RAPORTNM` | Rapor taslak/tanim dosyalari | Rapor metadata + SQL view/materialized view |
| `F:\DEPO\YAZICI` | Yazici/kuyruk/cikti klasorleri | Print service, PDF queue, belge cikti arsivi |
| `F:\DEPO\FATURADOKUM` | Fatura dokum/cikti alani | Invoice PDF/XML archive |
| `F:\BATCH` | Merkezi program dagitim ve yardimci araclar | CI/CD artifact repository veya deployment package |

### 3.2 `F:\DEPO` Klasor Listesi

| Klasor | Son Degisim |
|---|---|
| `AKTAR` | 2023-10-20 14:49:45 |
| `arsiv` | 2024-05-08 11:20:02 |
| `DATA` | 2025-02-20 11:46:19 |
| `data120` | 2024-05-24 16:41:33 |
| `data121` | 2024-05-24 16:41:39 |
| `DATA122` | 2024-05-24 16:41:44 |
| `data123` | 2024-05-24 16:41:49 |
| `data124` | 2025-01-14 13:51:13 |
| `data125` | 2025-12-29 15:31:53 |
| `data126` | 2026-01-06 16:58:48 |
| `data215` | 2024-05-24 16:41:59 |
| `data216` | 2024-05-24 16:42:03 |
| `data217` | 2024-05-24 16:42:08 |
| `data218` | 2024-05-24 16:42:12 |
| `data219` | 2025-12-17 17:55:30 |
| `data220` | 2024-05-24 16:42:20 |
| `data221` | 2025-07-23 18:49:01 |
| `DATA222` | 2024-10-08 10:50:17 |
| `data223` | 2025-12-30 17:12:59 |
| `data224` | 2025-02-05 15:28:50 |
| `data225` | 2026-02-02 13:38:32 |
| `data226` | 2026-04-28 14:09:21 |
| `data915` | 2024-05-24 16:42:58 |
| `data916` | 2024-05-24 16:43:01 |
| `data917` | 2024-05-24 16:43:06 |
| `data918` | 2024-05-24 16:43:10 |
| `data919` | 2024-05-24 16:43:15 |
| `data920` | 2024-05-24 16:43:19 |
| `data921` | 2024-05-24 16:43:22 |
| `DATA922` | 2024-05-24 16:43:28 |
| `data923` | 2024-11-11 15:03:28 |
| `data924` | 2025-01-25 10:36:56 |
| `data925` | 2025-12-10 09:35:00 |
| `data926` | 2026-04-28 14:32:31 |
| `FATURADOKUM` | 2025-10-10 19:31:31 |
| `LOG` | 2026-04-24 09:19:44 |
| `NODELIST` | 2026-02-09 09:32:04 |
| `RAPORTNM` | 2025-06-20 11:38:12 |
| `Temp` | 2026-04-29 16:55:23 |
| `TEMP2D` | 2026-02-13 10:01:13 |
| `YAZICI` | 2026-01-01 00:02:02 |
| `YUKLE` | 2023-10-20 14:47:14 |

### 3.3 `F:\BATCH` Dagitim Paketi

| Dosya/Klasor | Tip | Boyut | Son Degisim | Rol |
|---|---|---:|---|---|
| `020.json` | -a--- | 1440 | 2026-04-29 16:00:04 | Dagitim/yardimci dosya |
| `Anydesk` | d---- |  | 2026-04-24 18:32:35 | Dagitim/yardimci dosya |
| `checklist.exe` | -a--- | 168312 | 2026-04-01 12:01:16 | Dagitim/yardimci dosya |
| `chilkatax-9.5.0-win32.dll` | -a--- | 10399744 | 2021-02-09 20:42:58 | SOAP/HTTP/entegrasyon kutuphanesi |
| `dcopy.exe` | -a--- | 176708 | 2026-04-24 18:44:49 | Dagitim/yardimci dosya |
| `depo.zip` | -a--- | 8542 | 2026-04-29 16:00:05 | Dagitim/yardimci dosya |
| `eFatura Goruntuleyici` | d---- |  | 2024-02-05 15:11:27 | E-fatura/e-arsiv yardimci modulu |
| `efatura.exe` | -a--- | 1779080 | 2026-04-01 12:00:45 | E-fatura/e-arsiv yardimci modulu |
| `etcpanel.exe` | -a--- | 540911 | 2026-04-29 11:31:11 | Dagitim/yardimci dosya |
| `eticaret.exe` | -a--- | 360068 | 2026-04-29 11:31:12 | OPA/e-ticaret/pazaryeri entegrasyon veya ana uygulama paketi |
| `FATUSERS.CDX` | -a--- | 19126784 | 2026-04-29 01:24:48 | Dagitim/yardimci dosya |
| `FATUSERS.DBF` | -a--- | 470151491 | 2026-04-29 01:24:48 | Dagitim/yardimci dosya |
| `fatusers.zip` | -a--- | 86489688 | 2026-04-29 12:01:46 | Dagitim/yardimci dosya |
| `foxsmp1.fon` | -a--- | 7712 | 2023-10-20 15:08:41 | Visual FoxPro runtime/destek |
| `foxsmp4.fon` | -a--- | 8736 | 2023-10-20 15:08:42 | Visual FoxPro runtime/destek |
| `foxtools.fll` | -a--- | 53248 | 2024-02-29 18:31:46 | Visual FoxPro runtime/destek |
| `FOXUSER.DBF` | -a--- | 665 | 2026-01-31 11:18:20 | Dagitim/yardimci dosya |
| `FOXUSER.FPT` | -a--- | 704 | 2026-01-31 11:18:20 | Dagitim/yardimci dosya |
| `its.exe` | -a--- | 6209715 | 2026-04-01 21:28:46 | ITS/UTS/PTS entegrasyon modulu |
| `Java x64 offline` | d---- |  | 2024-02-05 15:11:24 | Dagitim/yardimci dosya |
| `keepopen.exe` | -a--- | 159611 | 2026-04-24 18:44:50 | Dagitim/yardimci dosya |
| `konsol.exe` | -a--- | 226162 | 2026-04-01 12:01:11 | Dagitim/yardimci dosya |
| `kontrol2d.exe` | -a--- | 1704773 | 2026-04-11 12:00:40 | 2D/karekod kontrol modulu |
| `listebas.exe` | -a--- | 442753 | 2026-04-01 12:01:15 | Dagitim/yardimci dosya |
| `msvcr71.dll` | -a--- | 348160 | 2023-10-20 15:08:30 | Visual FoxPro runtime/destek |
| `opa.exe` | -a--- | 8128831 | 2026-04-29 11:31:08 | OPA/e-ticaret/pazaryeri entegrasyon veya ana uygulama paketi |
| `PDF Bullzip` | d---- |  | 2023-10-20 15:24:42 | Dagitim/yardimci dosya |
| `prnyaz.exe` | -a--- | 128000 | 2026-01-02 16:03:39 | Dagitim/yardimci dosya |
| `program_bilgi_notu.txt` | -a--- | 11757 | 2026-04-29 16:00:06 | Dagitim/yardimci dosya |
| `psdime.dll` | -a--- | 110676 | 2023-10-20 15:08:38 | SOAP/HTTP/entegrasyon kutuphanesi |
| `psoap32.dll` | -a--- | 380928 | 2023-10-20 15:08:39 | SOAP/HTTP/entegrasyon kutuphanesi |
| `psproxy.dll` | -a--- | 73728 | 2023-10-20 15:08:40 | SOAP/HTTP/entegrasyon kutuphanesi |
| `SETUP.CHECKLIST` | d---- |  | 2024-05-08 11:16:34 | Dagitim/yardimci dosya |
| `SETUP.eFatura` | d---- |  | 2024-05-08 11:18:21 | E-fatura/e-arsiv yardimci modulu |
| `SETUP.ITS` | d---- |  | 2025-02-24 12:50:44 | Dagitim/yardimci dosya |
| `SETUP.kontrol2d` | d---- |  | 2024-03-14 21:07:13 | 2D/karekod kontrol modulu |
| `SETUP.kontrol2d.altdepo` | d---- |  | 2024-11-25 17:16:41 | 2D/karekod kontrol modulu |
| `SETUP.OPA` | d---- |  | 2025-10-10 20:50:17 | OPA/e-ticaret/pazaryeri entegrasyon veya ana uygulama paketi |
| `SETUP.prnyaz` | d---- |  | 2025-10-10 20:11:46 | Dagitim/yardimci dosya |
| `sqlnop.exe` | -a--- | 1041278 | 2026-04-29 11:31:10 | SQL/entegrasyon yardimci araci; davranis dogrulanmali |
| `vfp9r.dll` | -a--- | 4710400 | 2023-10-20 15:08:33 | Visual FoxPro runtime/destek |
| `vfp9renu.dll` | -a--- | 1429504 | 2023-10-20 15:08:35 | Visual FoxPro runtime/destek |
| `wkhtmltopdf.exe` | -a--- | 45283328 | 2025-08-15 00:37:41 | HTML/PDF cikti motoru |
| `wkhtmltox.dll` | -a--- | 45069312 | 2025-08-15 00:38:01 | HTML/PDF cikti motoru |
| `WORK.DAT` | -a--- | 2290 | 2024-04-29 14:38:40 | Dagitim/yardimci dosya |

## 4. Baslatma, Guncelleme ve Calisma Mantigi

1. Kullanici `F:\DEPO\Baskentw.vbs` ile sistemi baslatacak sekilde kurgulanmis.
2. Betik `BASKENT.EXE` dosyasinin surumunu `F:\BATCH` ile `C:\TEMP` arasinda karsilastiriyor.
3. Runtime bagimliliklari yerel `C:\TEMP` altina kopyalaniyor: `VFP9R.DLL`, `VFP9RENU.DLL`, `MSVCR71.DLL`, `ChilkatAx`, Fox fontlari.
4. FoxPro konfigurasyonu `CONFIG.FPW`: `CODEPAGE=1254`, temp/sort/edit/program calisma alanlari `C:\TEMP`, resource kapali.
5. Sistem veri dosyalarina `F:\DEPO` uzerinden erisiyor; bu, istemci tarafinda yerel EXE + merkezi DBF veri modelidir.
6. Sayim modu `SAYIM.VBS` ile `BASKENT.EXE SAYIM` parametresi uzerinden ayrilmis.

Yeni sistemde bu akisin karsiligi: istemciye EXE kopyalama yerine web tabanli uygulama veya otomatik guncellenen desktop client; veri erisimi DBF dosyasi yerine API + PostgreSQL transaction modeli olmalidir.

## 5. Veri Hacmi ve Modul Bazli Dagilim

- F:\DEPO\DATA\01 icin metadata okunabilen DBF tablo sayisi: 1130.
- Toplam kayit sayisi yaklasik: 145375709.
- DBF boyutu yaklasik: 10.27 GB.
- Bu sayiya CDX/FPT/IDX indeks ve memo dosyalari dahil degildir; fiziksel disk kullanimi daha yuksektir.

| Modul | Tablo Sayisi | Kayit | DBF MB | Yorum |
|---|---:|---:|---:|---|
| Takip/Karekod/PTS/UTS | 16 | 113504395 | 5239.64 | En yuksek hacim; partition ve batch isleme zorunlu |
| YKKS tarih bazli karekod/satis log | 805 | 19377047 | 2938.72 | Tarih bazli tablo cogalmasi; PostgreSQL partitiona donusmeli |
| Diger/Tanim/Parametre | 24 | 3733591 | 808.85 | Modul bazli normalize edilmeli |
| Satis/Alis/Siparis | 37 | 4049873 | 800.65 | ERP cekirdek hareketleri; transaction butunlugu kritik |
| E-Fatura/E-Arsiv | 8 | 762796 | 336.76 | Resmi belge ve durum gecmisi; audit zorunlu |
| Kampanya/Fiyat/Puan | 7 | 2135104 | 189 | Modul bazli normalize edilmeli |
| Finans/Cari/Tahsilat | 12 | 1222410 | 151.59 | Bakiye ve mutabakat hassasiyeti yuksek |
| Stok/Urun/Miad/Depo | 20 | 341962 | 32.54 | Modul bazli normalize edilmeli |
| Log/Denetim/Bildirim | 3 | 216428 | 16 | Modul bazli normalize edilmeli |
| UTSS tarih bazli UTS servis | 170 | 32103 | 2.04 | Modul bazli normalize edilmeli |
| KK aylik log | 28 | 0 | 0 | Modul bazli normalize edilmeli |

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

## 8. Hedef .NET Mimari

Onerilen mimari ilk fazda moduler monolit olmalidir. Eski sistem tek veri alaninda cok bagli calistigi icin baslangicta mikroservis yapmak veri tutarliligini zorlastirir. Moduler monolit sinirlar net tutulursa daha sonra servislesebilir.

| Proje | Gorev |
|---|---|
| `Tuna.Servis` | REST servis, kimlik, yetki, HTTP endpointleri |
| `Tuna.Uygulama` | Kullanim senaryosu servisleri, transaction yonetimi, dogrulama |
| `Tuna.Alan` | Entity, value object, alan kurali ve event tanimlari |
| `Tuna.Altyapi` | PostgreSQL, harici servis adapterleri, dosya depolama, mail/SMS |
| `Tuna.Isci` | E-fatura, ITS/UTS, bildirim, rapor, mutabakat arka plan isleri |
| `Tuna.Raporlama` | Rapor sorgulari, Excel/PDF ciktilari |
| `Tuna.Aktarim` | DBF okuma, staging, temizlik, aktarim, mutabakat |

## 9. PostgreSQL Hedef Veri Modeli

| Schema | Icerik | Eski Kaynaklar |
|---|---|---|
| `core` | Firma, sube, kullanici, yetki, parametre, belge numarasi | `FIRMA`, `BELGENO`, `KONTKULL`, `nodelist` |
| `catalog` | Urun, barkod, uretici, fiyat listesi, muadil, kampanya | `URUNLER`, `BARKOD`, `URETICI`, `MUADIL`, `KAMPANYA` |
| `inventory` | Stok, stok hareketi, parti, miad, sayim, depo/reyon | `STKZAMAN`, `PARTILER`, `MIADHRK`, `REYONKxx`, `ONSAYIM` |
| `sales` | Siparis, satis fatura, iade, irsaliye, bekleyen isler | `SIPARIS`, `SIPHAR`, `FATURAS`, `DETAYS`, `BEKLEYEN` |
| `purchase` | Alis siparis/fatura ve tedarikci hareketleri | `FATURAG`, `DETAYG`, `KURUMAKT` |
| `finance` | Cari, cari hareket, tahsilat, kredi karti, cek/senet | `FINANS`, `CARIOZT`, `KK*`, `SYMFTR`, `SYMHAR` |
| `einvoice` | E-fatura/e-arsiv belge, mukellef, durum gecmisi | `EFATURA`, `EARSIV`, `EFATKULL`, `EFTRKULL` |
| `tracktrace` | PTS/UTS/karekod, seri/lot/paket, servis loglari | `PTS*`, `UTS*`, `KK*`, `YKKS*`, `UTSS*` |
| `audit` | Islem, hata, kilit, bildirim, entegrasyon loglari | `GIRISLOG`, `KILITLOG`, `MESAJLOG`, `MAILLOG`, `SMSLOG` |
| `reporting` | Rapor tanimlari, rapor cache, materialized view | `RAPORTNM.*`, `FATURADOKUM` |

### 9.1 Partition Zorunlu Alanlar

- `tracktrace.pts_numbers`: `PTSNUM1`, `ptsNum2` gibi dev tablolar icin hash veya tarih partition.
- `tracktrace.ykks_events`: 805 adet `YKKSYYYYMMDD` tablo tek partitioned tabloya donusmeli.
- `tracktrace.utss_events`: 170 adet `UTSSYYYYMMDD` tablo tek partitioned tabloya donusmeli.
- `finance.card_or_qr_events`: `KKLC0001`, `KKLG0001`, `KKDETAY` gibi 20M+ kayitli tablolar partition ve BRIN/B-tree indeks karisimiyle tasarlanmali.
- `audit.integration_logs`: API/servis loglari retention politikasiyla partitionlanmali.

## 10. Eski Tablo - Yeni Modul Esleme

| Eski Tablo | Hedef | Kritik Donusum |
|---|---|---|
| `URUNLER` | `catalog.products` | 94 alan; fiyat/ozel kod/KDV/miad bilgileri alt tablolara ayrilmali |
| `BARKOD` | `catalog.product_barcodes` | Urun-barkod bire/cok iliskisi |
| `URETICI` | `catalog.manufacturers` | Tedarikci/uretici ayrimi netlestirilmeli |
| `FATURAS` | `sales.invoices` | Fatura basligi; belge no, cari, toplam, durum |
| `DETAYS` | `sales.invoice_lines` | Fatura satiri; urun, miktar, fiyat, iskonto, MF, KDV |
| `FATURAG` | `purchase.invoices` | Alis fatura basligi |
| `DETAYG` | `purchase.invoice_lines` | Alis satiri, maliyet ve miad etkisi |
| `SIPARIS`, `SIPHAR` | `sales.orders`, `sales.order_lines` | Siparis baslik/detay |
| `BEKLEYEN` | `sales.pending_orders` veya `warehouse.work_queue` | Depo/fatura oncesi is kuyruğu |
| `FINANS` | `finance.ledger_entries` | Cari/finans hareket defteri |
| `CARIOZT` | `finance.account_balances` veya materialized view | Ozet bakiye olabilir; kaynak hareketten uretilecek |
| `EFATURA`, `EARSIV` | `einvoice.documents` | UUID/durum/gecmis ayri tablolara ayrilmali |
| `EFATKULL`, `EFTRKULL` | `einvoice.taxpayer_registry` | Mukellef/etiket kayitlari |
| `PTSMAIN`, `PTSNUM1`, `ptsNum2` | `tracktrace.pts_*` | Cok buyuk veri; partition, idempotency, batch insert |
| `UTSLOG00`, `UTSDTY00`, `UTSS*` | `tracktrace.uts_*` | Servis log ve detaylari |
| `YKKS*` | `tracktrace.ykks_events` | Tarih bazli fiziksel tablo yerine partition |
| `KKDETAY`, `KKLC0001`, `KKLG0001`, `KKPAKET` | `finance.card_transactions` ve/veya `tracktrace.qr_transactions` | Is kurali kesinlestirme gerekli; hacim kritik |
| `KAMPANYA`, `ACD_KAMPANYA` | `catalog.campaigns`, `campaign_rules`, `campaign_audit` | Kural ve tarihce ayrilmali |
| `MAILLOG`, `SMSLOG`, `MESAJLOG` | `audit.notification_logs` | Kanal, durum, hata, tekrar deneme alanlari |

## 11. API ve Ekran Tasarimi Taslagi

| Alan | Temel Endpoint/Ekran | Not |
|---|---|---|
| Urun | `GET/POST /products`, barkod arama, fiyat/miad sekmeleri | Hizli arama indeksleri zorunlu |
| Cari | `GET/POST /accounts`, ekstre, bakiye, risk | Yetki ve limit kontrolu |
| Siparis | `POST /sales/orders`, `POST /sales/orders/{id}/invoice` | Transaction ve stok rezervasyonu |
| Fatura | `GET /sales/invoices`, PDF/XML, iptal/iade | E-fatura durumuyla bagli |
| Alis | `POST /purchase/invoices` | Maliyet ve stok girisi |
| Stok | stok hareket, sayim, reyon, parti/miad | Barkod okuyucu/mobile destek |
| Finans | tahsilat, cari fis, kredi karti/cek-senet | Immutable ledger |
| E-fatura | gonder, sorgula, arsivle, nusha al | Isci + kuyruk |
| ITS/UTS/PTS | karekod al, bildir, durum sorgula | Idempotent servis adapteri |
| Rapor | hazir raporlar, Excel/PDF export | Background export |
| Sistem | kullanici, rol, parametre, belge seri/no | Audit zorunlu |

## 12. Aktarim Stratejisi

1. DBF dosyalari readonly modda okunur; canli sisteme yazilmaz.
2. Her DBF once `staging_foxpro.<table>` altina ham aktarilir.
3. Alan tipleri donusturulur: `C` trim, `N` numeric parse, `D` nullable date, `L` boolean, `M` text.
4. Bozuk/parse edilemeyen kayitlar `migration_errors` tablosuna yazilir; aktarim durmaz.
5. Normalize hedef tablolara ETL calisir; her adim idempotent olur.
6. Mutabakat: kayit sayisi, toplam miktar, toplam tutar, belge sayisi, stok bakiye, cari bakiye, UUID sayisi, PTS/karekod sayisi.
7. Paralel calisma donemi: eski sistemden okunan raporlarla yeni sistem raporlari karsilastirilir.
8. Canli gecis: eski yazim durdurulur, son delta aktarilir, PostgreSQL yazimi acilir.

## 13. Kabul ve Mutabakat Kriterleri

- `FATURAS` baslik sayisi ve `DETAYS` satir sayisi birebir tutmali.
- Fatura toplam tutarlari, KDV, iskonto, mal fazlasi ve net tutar hesaplari eski raporlarla ayni olmali.
- Stok bakiye raporu urun/miad/parti bazinda ayni sonucu vermeli.
- Cari ekstre ve bakiye raporlari eski sistemle birebir tutmali.
- E-fatura UUID, VKN/TCKN, durum ve tarih bilgileri kayipsiz tasinmali.
- PTS/UTS/karekod tablolarinda kayit sayisi ve tekil seri/lot/paket sayisi mutabik olmali.
- Rapor performansi: arama ekranlari < 1 sn, normal rapor < 5 sn ilk cevap, buyuk rapor background export.
- Audit: kim, ne zaman, hangi belgeyi olusturdu/degistirdi/iptal etti bilgisi tutulmali.

## 14. Kritik Riskler ve Cozumler

| Risk | Etki | Cozum |
|---|---|---|
| Ana EXE bulunamadi | Is kurallarinin bir kismi belirsiz | Ana uygulama temin edilmeli, ekran akislari kaydedilmeli |
| DBF 2 GB siniri | Veri bozulmasi/durma riski | PostgreSQL partition + buyuk tablo aktarim planı |
| Karakter alanda sayisal veri | Yanlis hesap riski | Parse kurallari ve hata staging tablosu |
| Tarih bazli tablo cogalmasi | Bakim ve sorgu zorlugu | Partitioned event table |
| CDX indeks mantigi bilinmiyor | Performans ve tekillik kaybi | CDX key analizi veya uygulama ekran davranisi ile indeks tasarimi |
| Memo/FPT alanlari | Metin kaybi riski | FPT okuma testi ve text/json hedef alanlari |
| Eszamanli DBF kullanimi | Aktarim sirasinda tutarsiz snapshot | Bakim penceresi veya shadow copy/snapshot |
| Resmi entegrasyonlar | Yasal belge riski | Adapter test ortamı, idempotency, durum history |

## 15. Gelistirme Yol Haritasi

### Faz 0 - Kanit Tamamlama
- `C:\OPA\OPA.EXE` ana uygulamasi referans alinir; varsa kaynak kod veya ekran akis kayitlari temin edilir.
- 10-20 kritik ekran icin ekran kaydi alinır: urun, cari, siparis, fatura, alis, tahsilat, e-fatura, karekod, rapor.
- Mevcut kullanici rolleri ve gunluk operasyon senaryolari cikarilir.

### Faz 1 - Veri Sozlugu ve Staging
- Tum DBF semalari ve alan anlamlari veri sozlugune girilir.
- PostgreSQL staging semasi kurulur.
- DBF okuma ve mutabakat CLI/worker yazilir.

### Faz 2 - Cekirdek ERP
- Core, catalog, inventory, sales, purchase, finance modulleri yazilir.
- Eski sistem raporlariyla mutabakat baslar.

### Faz 3 - Entegrasyonlar
- E-fatura/e-arsiv, ITS/UTS/PTS, SMS/mail, PDF/yazici servisleri yazilir.
- Outbox, retry, idempotency ve audit zorunlu hale getirilir.

### Faz 4 - Raporlama ve Performans
- Materialized view, partition, indeks, background export ve cache stratejileri uygulanir.

### Faz 5 - Paralel Calisma ve Canli Gecis
- Son delta aktarimi, mutabakat imzalari ve canli gecis plani uygulanir.

## 16. En Buyuk 100 DBF Tablo

| Tablo | Modul | Kayit | Alan | Kayit Uzunlugu | Boyut MB |
|---|---|---:|---:|---:|---:|
| `PTSNUM1` | Takip/Karekod/PTS/UTS | 25347889 | 8 | 83 | 2006,41 |
| `ptsNum2` | Takip/Karekod/PTS/UTS | 11526629 | 8 | 83 | 912,39 |
| `KKDETAY` | Takip/Karekod/PTS/UTS | 24212217 | 5 | 38 | 877,44 |
| `KKLC0001` | Takip/Karekod/PTS/UTS | 27796694 | 8 | 24 | 636,22 |
| `KKLG0001` | Takip/Karekod/PTS/UTS | 22456674 | 8 | 24 | 513,99 |
| `fatusers` | Diger/Tanim/Parametre | 2207282 | 4 | 213 | 448,37 |
| `FATURAS` | Satis/Alis/Siparis | 644057 | 70 | 578 | 355,02 |
| `DETAYS` | Satis/Alis/Siparis | 2490246 | 30 | 128 | 303,99 |
| `EFTRKULL` | Diger/Tanim/Parametre | 1197583 | 5 | 252 | 287,81 |
| `PTSMAIN` | Takip/Karekod/PTS/UTS | 1308674 | 17 | 191 | 238,38 |
| `EFATURA` | E-Fatura/E-Arsiv | 237635 | 32 | 909 | 206 |
| `ACD_KAMPANYA` | Kampanya/Fiyat/Puan | 1974919 | 20 | 93 | 175,16 |
| `FINANS` | Finans/Cari/Tahsilat | 887186 | 21 | 146 | 123,53 |
| `EFATKULL` | E-Fatura/E-Arsiv | 508817 | 6 | 243 | 117,91 |
| `SATISLAR` | Satis/Alis/Siparis | 424066 | 21 | 135 | 54,6 |
| `KKPAKET` | Takip/Karekod/PTS/UTS | 767407 | 7 | 63 | 46,11 |
| `YKKS20240605` | YKKS tarih bazli karekod/satis log | 282440 | 9 | 159 | 42,83 |
| `ilacay` | Diger/Tanim/Parametre | 150843 | 24 | 272 | 39,13 |
| `DETAYG` | Satis/Alis/Siparis | 172468 | 43 | 206 | 33,88 |
| `YKKS20231202` | YKKS tarih bazli karekod/satis log | 191829 | 9 | 159 | 29,09 |
| `YKKS20241106` | YKKS tarih bazli karekod/satis log | 189106 | 9 | 159 | 28,68 |
| `YKKS20231111` | YKKS tarih bazli karekod/satis log | 186763 | 9 | 159 | 28,32 |
| `YKKS20240730` | YKKS tarih bazli karekod/satis log | 180671 | 9 | 159 | 27,4 |
| `YKKS20240725` | YKKS tarih bazli karekod/satis log | 177511 | 9 | 159 | 26,92 |
| `FTREKNOT` | Satis/Alis/Siparis | 259964 | 4 | 106 | 26,28 |
| `YKKS20240326` | YKKS tarih bazli karekod/satis log | 168604 | 9 | 159 | 25,57 |
| `YKKS20240731` | YKKS tarih bazli karekod/satis log | 166982 | 9 | 159 | 25,32 |
| `YKKS20240606` | YKKS tarih bazli karekod/satis log | 161693 | 9 | 159 | 24,52 |
| `YKKS20241102` | YKKS tarih bazli karekod/satis log | 142785 | 9 | 159 | 21,65 |
| `YKKS20240603` | YKKS tarih bazli karekod/satis log | 139439 | 9 | 159 | 21,14 |
| `YKKS20231115` | YKKS tarih bazli karekod/satis log | 138186 | 9 | 159 | 20,95 |
| `YKKS20231107` | YKKS tarih bazli karekod/satis log | 137009 | 9 | 159 | 20,78 |
| `YKKS20231108` | YKKS tarih bazli karekod/satis log | 133128 | 9 | 159 | 20,19 |
| `YKKS20231114` | YKKS tarih bazli karekod/satis log | 133056 | 9 | 159 | 20,18 |
| `YKKS20250604` | YKKS tarih bazli karekod/satis log | 132531 | 9 | 159 | 20,1 |
| `YKKS20240328` | YKKS tarih bazli karekod/satis log | 132478 | 9 | 159 | 20,09 |
| `YKKS20241104` | YKKS tarih bazli karekod/satis log | 132307 | 9 | 159 | 20,06 |
| `YKKS20241015` | YKKS tarih bazli karekod/satis log | 121914 | 9 | 159 | 18,49 |
| `YKKS20231209` | YKKS tarih bazli karekod/satis log | 121427 | 9 | 159 | 18,41 |
| `YKKS20231110` | YKKS tarih bazli karekod/satis log | 119153 | 9 | 159 | 18,07 |
| `YKKS20250506` | YKKS tarih bazli karekod/satis log | 118833 | 9 | 159 | 18,02 |
| `YKKS20231216` | YKKS tarih bazli karekod/satis log | 117029 | 9 | 159 | 17,75 |
| `YKKS20231225` | YKKS tarih bazli karekod/satis log | 112940 | 9 | 159 | 17,13 |
| `YKKS20250625` | YKKS tarih bazli karekod/satis log | 109345 | 9 | 159 | 16,58 |
| `YKKS20240106` | YKKS tarih bazli karekod/satis log | 109010 | 9 | 159 | 16,53 |
| `YKKS20250113` | YKKS tarih bazli karekod/satis log | 108513 | 9 | 159 | 16,45 |
| `YKKS20221222` | YKKS tarih bazli karekod/satis log | 108411 | 9 | 159 | 16,44 |
| `YKKS20241230` | YKKS tarih bazli karekod/satis log | 106744 | 9 | 159 | 16,19 |
| `YKKS20230906` | YKKS tarih bazli karekod/satis log | 106423 | 9 | 159 | 16,14 |
| `FATURAG` | Satis/Alis/Siparis | 28199 | 52 | 599 | 16,11 |
| `YKKS20240719` | YKKS tarih bazli karekod/satis log | 103954 | 9 | 159 | 15,76 |
| `YKKS20250718` | YKKS tarih bazli karekod/satis log | 102296 | 9 | 159 | 15,51 |
| `YKKS20230617` | YKKS tarih bazli karekod/satis log | 101470 | 9 | 159 | 15,39 |
| `tssipana` | Diger/Tanim/Parametre | 24635 | 24 | 652 | 15,32 |
| `YKKS20240923` | YKKS tarih bazli karekod/satis log | 100455 | 9 | 159 | 15,23 |
| `YKKS20240511` | YKKS tarih bazli karekod/satis log | 99553 | 9 | 159 | 15,1 |
| `YKKS20231208` | YKKS tarih bazli karekod/satis log | 98129 | 9 | 159 | 14,88 |
| `YKKS20240720` | YKKS tarih bazli karekod/satis log | 96818 | 9 | 159 | 14,68 |
| `YKKS20231017` | YKKS tarih bazli karekod/satis log | 95693 | 9 | 159 | 14,51 |
| `YKKS20250215` | YKKS tarih bazli karekod/satis log | 94869 | 9 | 159 | 14,39 |
| `YKKS20240109` | YKKS tarih bazli karekod/satis log | 94528 | 9 | 159 | 14,33 |
| `YKKS20240608` | YKKS tarih bazli karekod/satis log | 94423 | 9 | 159 | 14,32 |
| `YKKS20250510` | YKKS tarih bazli karekod/satis log | 93871 | 9 | 159 | 14,23 |
| `CARIOZT` | Finans/Cari/Tahsilat | 148845 | 5 | 99 | 14,05 |
| `YKKS20231212` | YKKS tarih bazli karekod/satis log | 92302 | 9 | 159 | 14 |
| `YKKS20231230` | YKKS tarih bazli karekod/satis log | 91767 | 9 | 159 | 13,92 |
| `YKKS20241120` | YKKS tarih bazli karekod/satis log | 91621 | 9 | 159 | 13,89 |
| `YKKS20240108` | YKKS tarih bazli karekod/satis log | 90866 | 9 | 159 | 13,78 |
| `YKKS20240703` | YKKS tarih bazli karekod/satis log | 90651 | 9 | 159 | 13,75 |
| `YKKS20241026` | YKKS tarih bazli karekod/satis log | 90560 | 9 | 159 | 13,73 |
| `YKKS20231117` | YKKS tarih bazli karekod/satis log | 88330 | 9 | 159 | 13,39 |
| `YKKS20231007` | YKKS tarih bazli karekod/satis log | 87789 | 9 | 159 | 13,31 |
| `YKKS20240529` | YKKS tarih bazli karekod/satis log | 86346 | 9 | 159 | 13,09 |
| `YKKS20240928` | YKKS tarih bazli karekod/satis log | 85691 | 9 | 159 | 12,99 |
| `YKKS20240210` | YKKS tarih bazli karekod/satis log | 85580 | 9 | 159 | 12,98 |
| `YKKS20231109` | YKKS tarih bazli karekod/satis log | 85171 | 9 | 159 | 12,92 |
| `YKKS20240509` | YKKS tarih bazli karekod/satis log | 84089 | 9 | 159 | 12,75 |
| `YKKS20240131` | YKKS tarih bazli karekod/satis log | 83936 | 9 | 159 | 12,73 |
| `YKKS20240927` | YKKS tarih bazli karekod/satis log | 83833 | 9 | 159 | 12,71 |
| `YKKS20240315` | YKKS tarih bazli karekod/satis log | 81204 | 9 | 159 | 12,31 |
| `YKKS20231118` | YKKS tarih bazli karekod/satis log | 80795 | 9 | 159 | 12,25 |
| `URUNLER` | Stok/Urun/Miad/Depo | 17425 | 94 | 732 | 12,17 |
| `YKKS20241128` | YKKS tarih bazli karekod/satis log | 80089 | 9 | 159 | 12,14 |
| `YKKS20250315` | YKKS tarih bazli karekod/satis log | 79184 | 9 | 159 | 12,01 |
| `YKKS20241014` | YKKS tarih bazli karekod/satis log | 78649 | 9 | 159 | 11,93 |
| `YANCIEF` | E-Fatura/E-Arsiv | 13536 | 32 | 909 | 11,74 |
| `YKKS20231130` | YKKS tarih bazli karekod/satis log | 76275 | 9 | 159 | 11,57 |
| `YKKS20230916` | YKKS tarih bazli karekod/satis log | 74353 | 9 | 159 | 11,28 |
| `FIYATAKT` | Kampanya/Fiyat/Puan | 120549 | 11 | 98 | 11,27 |
| `YKKS20240831` | YKKS tarih bazli karekod/satis log | 73850 | 9 | 159 | 11,2 |
| `YKKS20240305` | YKKS tarih bazli karekod/satis log | 73620 | 9 | 159 | 11,16 |
| `YKKS20250621` | YKKS tarih bazli karekod/satis log | 72720 | 9 | 159 | 11,03 |
| `YKKS20240504` | YKKS tarih bazli karekod/satis log | 72370 | 9 | 159 | 10,97 |
| `TSSIPDTY` | Diger/Tanim/Parametre | 87930 | 16 | 130 | 10,9 |
| `YKKS20240620` | YKKS tarih bazli karekod/satis log | 71800 | 9 | 159 | 10,89 |
| `YKKS20231226` | YKKS tarih bazli karekod/satis log | 71388 | 9 | 159 | 10,83 |
| `YKKS20240610` | YKKS tarih bazli karekod/satis log | 71133 | 9 | 159 | 10,79 |
| `YKKS20231129` | YKKS tarih bazli karekod/satis log | 70064 | 9 | 159 | 10,62 |
| `YKKS20231128` | YKKS tarih bazli karekod/satis log | 69480 | 9 | 159 | 10,54 |
| `YKKS20240601` | YKKS tarih bazli karekod/satis log | 68990 | 9 | 159 | 10,46 |

## 17. Tum DBF Tablo Envanteri

Bu bolum `F:\DEPO\DATA\01` icin okunabilen tum DBF tablolarini listeler. Bu liste yeni sistem veri sozlugunun baslangicidir.

| Modul | Tablo | Kayit | Alan | Kayit Uzunlugu | Boyut MB |
|---|---|---:|---:|---:|---:|
| Diger/Tanim/Parametre | `BELGENO` | 461 | 5 | 33 | 0,01 |
| Diger/Tanim/Parametre | `BOLGE` | 99 | 6 | 34 | 0 |
| Diger/Tanim/Parametre | `EFTRKULL` | 1197583 | 5 | 252 | 287,81 |
| Diger/Tanim/Parametre | `FATIPILC` | 36109 | 30 | 128 | 4,41 |
| Diger/Tanim/Parametre | `fatusers` | 2207282 | 4 | 213 | 448,37 |
| Diger/Tanim/Parametre | `FRMVAAD1` | 1 | 49 | 522 | 0 |
| Diger/Tanim/Parametre | `FTNOLOCK` | 1 | 3 | 27 | 0 |
| Diger/Tanim/Parametre | `FZSATICI` | 0 | 8 | 97 | 0 |
| Diger/Tanim/Parametre | `HIZDTY` | 3980 | 12 | 157 | 0,6 |
| Diger/Tanim/Parametre | `IPTACHSP` | 12361 | 15 | 119 | 1,4 |
| Diger/Tanim/Parametre | `ISIK` | 2 | 1 | 51 | 0 |
| Diger/Tanim/Parametre | `ilacay` | 150843 | 24 | 272 | 39,13 |
| Diger/Tanim/Parametre | `KURUMAKT` | 11979 | 10 | 78 | 0,89 |
| Diger/Tanim/Parametre | `PLSSATIS` | 299 | 2 | 19 | 0,01 |
| Diger/Tanim/Parametre | `PSIKO` | 5 | 6 | 54 | 0 |
| Diger/Tanim/Parametre | `PSIKORPR` | 0 | 13 | 121 | 0 |
| Diger/Tanim/Parametre | `RENK` | 5 | 6 | 37 | 0 |
| Diger/Tanim/Parametre | `RYNSFR` | 8 | 3 | 8 | 0 |
| Diger/Tanim/Parametre | `SERVIS` | 3 | 2 | 36 | 0 |
| Diger/Tanim/Parametre | `TALEPLOG` | 5 | 10 | 51 | 0 |
| Diger/Tanim/Parametre | `TELRPR` | 0 | 10 | 118 | 0 |
| Diger/Tanim/Parametre | `TSSIPDTY` | 87930 | 16 | 130 | 10,9 |
| Diger/Tanim/Parametre | `tssipana` | 24635 | 24 | 652 | 15,32 |
| Diger/Tanim/Parametre | `UYARITIP` | 0 | 2 | 43 | 0 |
| E-Fatura/E-Arsiv | `EARSIV` | 1960 | 27 | 470 | 0,88 |
| E-Fatura/E-Arsiv | `EFATKULL` | 508817 | 6 | 243 | 117,91 |
| E-Fatura/E-Arsiv | `EFATNOT` | 621 | 2 | 217 | 0,13 |
| E-Fatura/E-Arsiv | `EFATURA` | 237635 | 32 | 909 | 206 |
| E-Fatura/E-Arsiv | `GEFATURA` | 0 | 57 | 1170 | 0 |
| E-Fatura/E-Arsiv | `GEFTRDTY` | 0 | 24 | 276 | 0 |
| E-Fatura/E-Arsiv | `YANCIEA` | 227 | 27 | 470 | 0,1 |
| E-Fatura/E-Arsiv | `YANCIEF` | 13536 | 32 | 909 | 11,74 |
| Finans/Cari/Tahsilat | `CARIOZT` | 148845 | 5 | 99 | 14,05 |
| Finans/Cari/Tahsilat | `cephrk` | 90788 | 13 | 102 | 8,83 |
| Finans/Cari/Tahsilat | `CEPILCAY` | 92776 | 6 | 51 | 4,51 |
| Finans/Cari/Tahsilat | `FINANS` | 887186 | 21 | 146 | 123,53 |
| Finans/Cari/Tahsilat | `parlog` | 1202 | 113 | 407 | 0,47 |
| Finans/Cari/Tahsilat | `SYMFTR` | 22 | 24 | 242 | 0,01 |
| Finans/Cari/Tahsilat | `symftrb` | 0 | 24 | 242 | 0 |
| Finans/Cari/Tahsilat | `symftryd` | 62 | 24 | 242 | 0,02 |
| Finans/Cari/Tahsilat | `SYMHAR` | 1430 | 16 | 127 | 0,17 |
| Finans/Cari/Tahsilat | `symharb` | 0 | 16 | 127 | 0 |
| Finans/Cari/Tahsilat | `TAHSILCI` | 99 | 2 | 28 | 0 |
| Finans/Cari/Tahsilat | `TAKAS` | 0 | 21 | 106 | 0 |
| Kampanya/Fiyat/Puan | `ACD_KAMPANYA` | 1974919 | 20 | 93 | 175,16 |
| Kampanya/Fiyat/Puan | `EKPUAN` | 32543 | 8 | 57 | 1,77 |
| Kampanya/Fiyat/Puan | `FIYAT_ADLARI` | 14 | 2 | 15 | 0 |
| Kampanya/Fiyat/Puan | `FIYATAKT` | 120549 | 11 | 98 | 11,27 |
| Kampanya/Fiyat/Puan | `FIYATFRK` | 0 | 2 | 25 | 0 |
| Kampanya/Fiyat/Puan | `KAMPANYA` | 6780 | 24 | 116 | 0,75 |
| Kampanya/Fiyat/Puan | `PLASPUAN` | 299 | 15 | 184 | 0,05 |
| KK aylik log | `kklog401` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog402` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog403` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog404` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog405` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog406` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog407` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog408` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog409` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog410` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog411` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog412` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog501` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog502` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog503` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog504` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog505` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog506` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog507` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog508` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog509` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog510` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog511` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog512` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog601` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog602` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog603` | 0 | 12 | 112 | 0 |
| KK aylik log | `kklog604` | 0 | 12 | 112 | 0 |
| Log/Denetim/Bildirim | `ARSLOG` | 0 | 18 | 271 | 0 |
| Log/Denetim/Bildirim | `GIRISLOG` | 125149 | 10 | 75 | 8,95 |
| Log/Denetim/Bildirim | `rynislem` | 91279 | 11 | 81 | 7,05 |
| Satis/Alis/Siparis | `BEKLEYEN` | 343 | 60 | 1040 | 0,34 |
| Satis/Alis/Siparis | `DETAYG` | 172468 | 43 | 206 | 33,88 |
| Satis/Alis/Siparis | `DETAYS` | 2490246 | 30 | 128 | 303,99 |
| Satis/Alis/Siparis | `E1SIPANA` | 0 | 19 | 421 | 0 |
| Satis/Alis/Siparis | `E1SIPDTY` | 0 | 10 | 224 | 0 |
| Satis/Alis/Siparis | `ESSIPANA` | 2 | 17 | 399 | 0 |
| Satis/Alis/Siparis | `ESSIPDTY` | 2 | 10 | 230 | 0 |
| Satis/Alis/Siparis | `FATURAG` | 28199 | 52 | 599 | 16,11 |
| Satis/Alis/Siparis | `faturai` | 10571 | 74 | 636 | 6,41 |
| Satis/Alis/Siparis | `FATURAS` | 644057 | 70 | 578 | 355,02 |
| Satis/Alis/Siparis | `FBSIPANA` | 0 | 22 | 502 | 0 |
| Satis/Alis/Siparis | `FBSIPDTY` | 0 | 11 | 235 | 0 |
| Satis/Alis/Siparis | `FRXSIPANA` | 63 | 21 | 417 | 0,03 |
| Satis/Alis/Siparis | `FRXSIPDTY` | 150 | 11 | 237 | 0,03 |
| Satis/Alis/Siparis | `FTRACK` | 6325 | 4 | 66 | 0,4 |
| Satis/Alis/Siparis | `FTRDTYEK` | 45 | 11 | 86 | 0 |
| Satis/Alis/Siparis | `FTREKNOT` | 259964 | 4 | 106 | 26,28 |
| Satis/Alis/Siparis | `FZ1SIPANA` | 201 | 26 | 671 | 0,13 |
| Satis/Alis/Siparis | `FZ1SIPDTY` | 447 | 10 | 222 | 0,1 |
| Satis/Alis/Siparis | `FZ1URUN` | 544 | 22 | 560 | 0,29 |
| Satis/Alis/Siparis | `FZ2URUN` | 0 | 22 | 560 | 0 |
| Satis/Alis/Siparis | `IADEFTR` | 0 | 64 | 507 | 0 |
| Satis/Alis/Siparis | `IRSALIYE` | 3469 | 32 | 502 | 1,66 |
| Satis/Alis/Siparis | `NDILAN` | 4534 | 9 | 169 | 0,73 |
| Satis/Alis/Siparis | `NDSIPANA` | 0 | 19 | 459 | 0 |
| Satis/Alis/Siparis | `NDSIPDTY` | 0 | 11 | 235 | 0 |
| Satis/Alis/Siparis | `ONGIRDTY` | 0 | 2 | 31 | 0 |
| Satis/Alis/Siparis | `ONGIRIS` | 0 | 21 | 157 | 0 |
| Satis/Alis/Siparis | `ONSAYIM` | 0 | 17 | 141 | 0 |
| Satis/Alis/Siparis | `OP1SIPANA` | 1 | 23 | 753 | 0 |
| Satis/Alis/Siparis | `OP1SIPDTY` | 1 | 11 | 238 | 0 |
| Satis/Alis/Siparis | `OP2SIPANA` | 2 | 23 | 753 | 0 |
| Satis/Alis/Siparis | `OP2SIPDTY` | 11 | 11 | 238 | 0 |
| Satis/Alis/Siparis | `SATISLAR` | 424066 | 21 | 135 | 54,6 |
| Satis/Alis/Siparis | `SATISPAR` | 37 | 127 | 486 | 0,02 |
| Satis/Alis/Siparis | `SIPARIS` | 548 | 26 | 317 | 0,17 |
| Satis/Alis/Siparis | `SIPHAR` | 3577 | 23 | 135 | 0,46 |
| Stok/Urun/Miad/Depo | `BAKIYE` | 1294 | 11 | 79 | 0,1 |
| Stok/Urun/Miad/Depo | `BARKOD` | 481 | 3 | 35 | 0,02 |
| Stok/Urun/Miad/Depo | `BLOKE` | 233 | 6 | 84 | 0,02 |
| Stok/Urun/Miad/Depo | `ILACAKT` | 80341 | 17 | 117 | 8,96 |
| Stok/Urun/Miad/Depo | `ILACCEP` | 0 | 13 | 74 | 0 |
| Stok/Urun/Miad/Depo | `ILACMIAD` | 32850 | 8 | 58 | 1,82 |
| Stok/Urun/Miad/Depo | `MIADHRK` | 172278 | 7 | 37 | 6,08 |
| Stok/Urun/Miad/Depo | `MUADIL` | 1820 | 2 | 48 | 0,08 |
| Stok/Urun/Miad/Depo | `PARSEL` | 299 | 3 | 10 | 0 |
| Stok/Urun/Miad/Depo | `PARTILER` | 1035 | 21 | 112 | 0,11 |
| Stok/Urun/Miad/Depo | `REYONK02` | 6 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `REYONK03` | 6 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `REYONK04` | 6 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `REYONK05` | 6 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `REYONK06` | 6 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `REYONKAL` | 1 | 4 | 17 | 0 |
| Stok/Urun/Miad/Depo | `stkzaman` | 17425 | 9 | 143 | 2,38 |
| Stok/Urun/Miad/Depo | `TEVZILER` | 15814 | 4 | 30 | 0,45 |
| Stok/Urun/Miad/Depo | `URETICI` | 636 | 28 | 574 | 0,35 |
| Stok/Urun/Miad/Depo | `URUNLER` | 17425 | 94 | 732 | 12,17 |
| Takip/Karekod/PTS/UTS | `KKDETAY` | 24212217 | 5 | 38 | 877,44 |
| Takip/Karekod/PTS/UTS | `KKDETAY1` | 2 | 5 | 38 | 0 |
| Takip/Karekod/PTS/UTS | `KKLC0001` | 27796694 | 8 | 24 | 636,22 |
| Takip/Karekod/PTS/UTS | `KKLG0001` | 22456674 | 8 | 24 | 513,99 |
| Takip/Karekod/PTS/UTS | `KKOZET` | 6989 | 5 | 50 | 0,33 |
| Takip/Karekod/PTS/UTS | `KKPAKET` | 767407 | 7 | 63 | 46,11 |
| Takip/Karekod/PTS/UTS | `kktemp` | 0 | 1 | 101 | 0 |
| Takip/Karekod/PTS/UTS | `KONTKULL` | 55 | 5 | 44 | 0 |
| Takip/Karekod/PTS/UTS | `PTSMAIN` | 1308674 | 17 | 191 | 238,38 |
| Takip/Karekod/PTS/UTS | `PTSNUM00` | 201 | 8 | 83 | 0,02 |
| Takip/Karekod/PTS/UTS | `PTSNUM1` | 25347889 | 8 | 83 | 2006,41 |
| Takip/Karekod/PTS/UTS | `ptsNum2` | 11526629 | 8 | 83 | 912,39 |
| Takip/Karekod/PTS/UTS | `UTSDTY00` | 10460 | 12 | 169 | 1,69 |
| Takip/Karekod/PTS/UTS | `UTSLOG00` | 70504 | 10 | 99 | 6,66 |
| Takip/Karekod/PTS/UTS | `UTSPAKET` | 0 | 8 | 95 | 0 |
| Takip/Karekod/PTS/UTS | `UTSPDT00` | 0 | 9 | 159 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251007` | 289 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251008` | 107 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251009` | 271 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251010` | 24 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251011` | 516 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20251013` | 493 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20251014` | 186 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251015` | 60 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251016` | 231 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251017` | 656 | 7 | 66 | 0,04 |
| UTSS tarih bazli UTS servis | `UTSS20251018` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251020` | 56 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251021` | 106 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251022` | 32 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251023` | 283 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251024` | 4120 | 7 | 66 | 0,26 |
| UTSS tarih bazli UTS servis | `UTSS20251025` | 60 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251027` | 158 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251028` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251030` | 37 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251031` | 230 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251101` | 400 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20251103` | 115 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251104` | 41 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251105` | 277 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251106` | 157 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251107` | 198 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251108` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251110` | 20 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251111` | 108 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251112` | 282 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251113` | 35 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251114` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251115` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251117` | 55 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251118` | 127 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251119` | 4 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251120` | 35 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251121` | 100 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251122` | 1120 | 7 | 66 | 0,07 |
| UTSS tarih bazli UTS servis | `UTSS20251124` | 602 | 7 | 66 | 0,04 |
| UTSS tarih bazli UTS servis | `UTSS20251125` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251126` | 55 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251127` | 14 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251128` | 1506 | 7 | 66 | 0,1 |
| UTSS tarih bazli UTS servis | `UTSS20251129` | 4 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251201` | 311 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251202` | 15 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251203` | 275 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20251204` | 178 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251205` | 219 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251206` | 550 | 7 | 66 | 0,04 |
| UTSS tarih bazli UTS servis | `UTSS20251208` | 85 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251209` | 138 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251210` | 85 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251211` | 97 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251212` | 88 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251213` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251215` | 5 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251216` | 22 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251217` | 414 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20251218` | 89 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251219` | 130 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251220` | 827 | 7 | 66 | 0,05 |
| UTSS tarih bazli UTS servis | `UTSS20251222` | 25 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251223` | 164 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251224` | 3251 | 7 | 66 | 0,21 |
| UTSS tarih bazli UTS servis | `UTSS20251225` | 55 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251226` | 92 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251227` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20251229` | 416 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20251230` | 169 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20251231` | 101 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260102` | 30 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260103` | 55 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260105` | 137 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260106` | 134 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260107` | 322 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260108` | 416 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20260109` | 85 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260110` | 302 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260112` | 101 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260113` | 125 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260114` | 205 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260115` | 151 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260116` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260117` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260119` | 71 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260120` | 31 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260121` | 76 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260122` | 31 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260123` | 60 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260124` | 200 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260126` | 37 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260127` | 30 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260128` | 64 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260129` | 116 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260130` | 39 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260131` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260202` | 62 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260203` | 124 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260204` | 133 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260205` | 149 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260206` | 80 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260207` | 45 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260209` | 45 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260210` | 123 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260211` | 146 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260212` | 129 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260213` | 15 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260214` | 50 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260216` | 22 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260217` | 20 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260218` | 188 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260219` | 22 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260220` | 94 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260221` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260223` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260224` | 20 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260225` | 63 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260226` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260227` | 4 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260228` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260302` | 60 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260303` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260304` | 72 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260305` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260306` | 41 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260307` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260309` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260310` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260311` | 1 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260312` | 21 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260313` | 1 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260314` | 3 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260316` | 42 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260317` | 1 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260318` | 61 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260323` | 54 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260324` | 892 | 7 | 66 | 0,06 |
| UTSS tarih bazli UTS servis | `UTSS20260325` | 129 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260326` | 191 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260327` | 71 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260328` | 400 | 7 | 66 | 0,03 |
| UTSS tarih bazli UTS servis | `UTSS20260330` | 246 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260331` | 40 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260401` | 722 | 7 | 66 | 0,05 |
| UTSS tarih bazli UTS servis | `UTSS20260402` | 128 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260403` | 205 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260404` | 750 | 7 | 66 | 0,05 |
| UTSS tarih bazli UTS servis | `UTSS20260406` | 124 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260407` | 261 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260408` | 139 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260409` | 169 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260410` | 60 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260411` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260413` | 337 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260414` | 330 | 7 | 66 | 0,02 |
| UTSS tarih bazli UTS servis | `UTSS20260415` | 95 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260416` | 61 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260417` | 745 | 7 | 66 | 0,05 |
| UTSS tarih bazli UTS servis | `UTSS20260418` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260420` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260421` | 138 | 7 | 66 | 0,01 |
| UTSS tarih bazli UTS servis | `UTSS20260422` | 20 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260424` | 49 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260425` | 0 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260427` | 61 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260428` | 10 | 7 | 66 | 0 |
| UTSS tarih bazli UTS servis | `UTSS20260429` | 0 | 7 | 66 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20221221` | 100 | 9 | 159 | 0,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20221222` | 108411 | 9 | 159 | 16,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20230217` | 6 | 9 | 159 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20230227` | 12 | 9 | 159 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20230316` | 20 | 9 | 159 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20230328` | 51 | 9 | 159 | 0,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20230505` | 10965 | 9 | 159 | 1,66 |
| YKKS tarih bazli karekod/satis log | `YKKS20230531` | 275 | 9 | 159 | 0,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20230617` | 101470 | 9 | 159 | 15,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20230620` | 20615 | 9 | 159 | 3,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20230621` | 13592 | 9 | 159 | 2,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20230622` | 12319 | 9 | 159 | 1,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20230722` | 279 | 9 | 159 | 0,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20230724` | 162 | 9 | 159 | 0,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20230808` | 28337 | 9 | 159 | 4,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20230809` | 11020 | 9 | 159 | 1,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20230810` | 157 | 9 | 159 | 0,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20230815` | 30548 | 9 | 159 | 4,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20230818` | 53 | 9 | 159 | 0,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20230822` | 12683 | 9 | 159 | 1,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20230828` | 13803 | 9 | 159 | 2,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20230902` | 23 | 9 | 159 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20230906` | 106423 | 9 | 159 | 16,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20230912` | 164 | 9 | 159 | 0,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20230914` | 54 | 9 | 159 | 0,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20230915` | 305 | 9 | 159 | 0,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20230916` | 74353 | 9 | 159 | 11,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20230918` | 71 | 9 | 159 | 0,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20230919` | 276 | 9 | 159 | 0,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20230920` | 25 | 9 | 159 | 0 |
| YKKS tarih bazli karekod/satis log | `YKKS20230926` | 33 | 9 | 159 | 0,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20230928` | 96 | 9 | 159 | 0,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20231003` | 16232 | 9 | 159 | 2,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20231004` | 18516 | 9 | 159 | 2,81 |
| YKKS tarih bazli karekod/satis log | `YKKS20231005` | 24143 | 9 | 159 | 3,66 |
| YKKS tarih bazli karekod/satis log | `YKKS20231006` | 22014 | 9 | 159 | 3,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20231007` | 87789 | 9 | 159 | 13,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20231009` | 13558 | 9 | 159 | 2,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20231010` | 11979 | 9 | 159 | 1,82 |
| YKKS tarih bazli karekod/satis log | `YKKS20231011` | 22572 | 9 | 159 | 3,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20231012` | 12783 | 9 | 159 | 1,94 |
| YKKS tarih bazli karekod/satis log | `YKKS20231013` | 11499 | 9 | 159 | 1,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20231014` | 51439 | 9 | 159 | 7,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20231016` | 34017 | 9 | 159 | 5,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20231017` | 95693 | 9 | 159 | 14,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20231018` | 43569 | 9 | 159 | 6,61 |
| YKKS tarih bazli karekod/satis log | `YKKS20231019` | 65805 | 9 | 159 | 9,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20231020` | 4098 | 9 | 159 | 0,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20231021` | 15589 | 9 | 159 | 2,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20231023` | 15257 | 9 | 159 | 2,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20231024` | 24272 | 9 | 159 | 3,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20231025` | 20985 | 9 | 159 | 3,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20231026` | 19356 | 9 | 159 | 2,94 |
| YKKS tarih bazli karekod/satis log | `YKKS20231027` | 32233 | 9 | 159 | 4,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20231028` | 28312 | 9 | 159 | 4,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20231030` | 14929 | 9 | 159 | 2,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20231031` | 15487 | 9 | 159 | 2,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20231101` | 23442 | 9 | 159 | 3,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20231102` | 34677 | 9 | 159 | 5,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20231103` | 27112 | 9 | 159 | 4,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20231104` | 42869 | 9 | 159 | 6,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20231106` | 13980 | 9 | 159 | 2,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20231107` | 137009 | 9 | 159 | 20,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20231108` | 133128 | 9 | 159 | 20,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20231109` | 85171 | 9 | 159 | 12,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20231110` | 119153 | 9 | 159 | 18,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20231111` | 186763 | 9 | 159 | 28,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20231113` | 55599 | 9 | 159 | 8,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20231114` | 133056 | 9 | 159 | 20,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20231115` | 138186 | 9 | 159 | 20,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20231116` | 28394 | 9 | 159 | 4,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20231117` | 88330 | 9 | 159 | 13,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20231118` | 80795 | 9 | 159 | 12,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20231120` | 16040 | 9 | 159 | 2,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20231121` | 49807 | 9 | 159 | 7,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20231122` | 52821 | 9 | 159 | 8,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20231123` | 13056 | 9 | 159 | 1,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20231124` | 12199 | 9 | 159 | 1,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20231125` | 54640 | 9 | 159 | 8,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20231127` | 15584 | 9 | 159 | 2,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20231128` | 69480 | 9 | 159 | 10,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20231129` | 70064 | 9 | 159 | 10,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20231130` | 76275 | 9 | 159 | 11,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20231201` | 43065 | 9 | 159 | 6,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20231202` | 191829 | 9 | 159 | 29,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20231204` | 25088 | 9 | 159 | 3,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20231205` | 28438 | 9 | 159 | 4,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20231206` | 31411 | 9 | 159 | 4,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20231207` | 26335 | 9 | 159 | 3,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20231208` | 98129 | 9 | 159 | 14,88 |
| YKKS tarih bazli karekod/satis log | `YKKS20231209` | 121427 | 9 | 159 | 18,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20231211` | 14980 | 9 | 159 | 2,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20231212` | 92302 | 9 | 159 | 14 |
| YKKS tarih bazli karekod/satis log | `YKKS20231213` | 38318 | 9 | 159 | 5,81 |
| YKKS tarih bazli karekod/satis log | `YKKS20231214` | 14177 | 9 | 159 | 2,15 |
| YKKS tarih bazli karekod/satis log | `YKKS20231215` | 51326 | 9 | 159 | 7,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20231216` | 117029 | 9 | 159 | 17,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20231218` | 49961 | 9 | 159 | 7,58 |
| YKKS tarih bazli karekod/satis log | `YKKS20231219` | 51421 | 9 | 159 | 7,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20231220` | 34886 | 9 | 159 | 5,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20231221` | 20845 | 9 | 159 | 3,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20231222` | 54239 | 9 | 159 | 8,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20231223` | 21171 | 9 | 159 | 3,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20231225` | 112940 | 9 | 159 | 17,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20231226` | 71388 | 9 | 159 | 10,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20231227` | 31973 | 9 | 159 | 4,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20231228` | 27928 | 9 | 159 | 4,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20231229` | 22111 | 9 | 159 | 3,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20231230` | 91767 | 9 | 159 | 13,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20240102` | 14799 | 9 | 159 | 2,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20240103` | 12458 | 9 | 159 | 1,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20240104` | 20132 | 9 | 159 | 3,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20240105` | 18779 | 9 | 159 | 2,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20240106` | 109010 | 9 | 159 | 16,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20240108` | 90866 | 9 | 159 | 13,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20240109` | 94528 | 9 | 159 | 14,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20240110` | 33726 | 9 | 159 | 5,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20240111` | 20630 | 9 | 159 | 3,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20240112` | 8996 | 9 | 159 | 1,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20240113` | 53062 | 9 | 159 | 8,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20240115` | 15875 | 9 | 159 | 2,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20240116` | 15199 | 9 | 159 | 2,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20240117` | 13552 | 9 | 159 | 2,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20240118` | 28996 | 9 | 159 | 4,4 |
| YKKS tarih bazli karekod/satis log | `YKKS20240119` | 8591 | 9 | 159 | 1,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20240120` | 56819 | 9 | 159 | 8,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20240122` | 6591 | 9 | 159 | 1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240123` | 12872 | 9 | 159 | 1,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20240124` | 15505 | 9 | 159 | 2,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20240125` | 8721 | 9 | 159 | 1,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20240126` | 20217 | 9 | 159 | 3,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20240127` | 35326 | 9 | 159 | 5,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20240129` | 6839 | 9 | 159 | 1,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20240130` | 4240 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20240131` | 83936 | 9 | 159 | 12,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20240201` | 13134 | 9 | 159 | 1,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20240202` | 10915 | 9 | 159 | 1,66 |
| YKKS tarih bazli karekod/satis log | `YKKS20240203` | 50605 | 9 | 159 | 7,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20240205` | 15458 | 9 | 159 | 2,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20240206` | 14005 | 9 | 159 | 2,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20240207` | 19976 | 9 | 159 | 3,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20240208` | 37546 | 9 | 159 | 5,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20240209` | 25733 | 9 | 159 | 3,9 |
| YKKS tarih bazli karekod/satis log | `YKKS20240210` | 85580 | 9 | 159 | 12,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20240212` | 14901 | 9 | 159 | 2,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240213` | 13216 | 9 | 159 | 2 |
| YKKS tarih bazli karekod/satis log | `YKKS20240214` | 20567 | 9 | 159 | 3,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20240215` | 24452 | 9 | 159 | 3,71 |
| YKKS tarih bazli karekod/satis log | `YKKS20240216` | 16955 | 9 | 159 | 2,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240217` | 26998 | 9 | 159 | 4,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20240219` | 11509 | 9 | 159 | 1,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20240220` | 16921 | 9 | 159 | 2,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240221` | 68463 | 9 | 159 | 10,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20240222` | 15572 | 9 | 159 | 2,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20240223` | 7481 | 9 | 159 | 1,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20240224` | 31885 | 9 | 159 | 4,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20240226` | 6887 | 9 | 159 | 1,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20240227` | 7832 | 9 | 159 | 1,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20240228` | 4990 | 9 | 159 | 0,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20240229` | 4306 | 9 | 159 | 0,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20240301` | 13877 | 9 | 159 | 2,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240302` | 48889 | 9 | 159 | 7,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20240304` | 13842 | 9 | 159 | 2,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240305` | 73620 | 9 | 159 | 11,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20240306` | 13955 | 9 | 159 | 2,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20240307` | 9498 | 9 | 159 | 1,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240308` | 12113 | 9 | 159 | 1,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20240309` | 17644 | 9 | 159 | 2,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20240311` | 7932 | 9 | 159 | 1,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20240312` | 9846 | 9 | 159 | 1,49 |
| YKKS tarih bazli karekod/satis log | `YKKS20240313` | 17720 | 9 | 159 | 2,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20240314` | 61336 | 9 | 159 | 9,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20240315` | 81204 | 9 | 159 | 12,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20240316` | 46967 | 9 | 159 | 7,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20240318` | 9952 | 9 | 159 | 1,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20240319` | 21850 | 9 | 159 | 3,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20240320` | 9079 | 9 | 159 | 1,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20240321` | 7625 | 9 | 159 | 1,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20240322` | 19884 | 9 | 159 | 3,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20240323` | 21582 | 9 | 159 | 3,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20240325` | 64987 | 9 | 159 | 9,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20240326` | 168604 | 9 | 159 | 25,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240327` | 6206 | 9 | 159 | 0,94 |
| YKKS tarih bazli karekod/satis log | `YKKS20240328` | 132478 | 9 | 159 | 20,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20240329` | 32247 | 9 | 159 | 4,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20240330` | 28592 | 9 | 159 | 4,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20240401` | 9454 | 9 | 159 | 1,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20240402` | 22613 | 9 | 159 | 3,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20240403` | 12418 | 9 | 159 | 1,88 |
| YKKS tarih bazli karekod/satis log | `YKKS20240404` | 23531 | 9 | 159 | 3,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240405` | 24896 | 9 | 159 | 3,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20240406` | 40803 | 9 | 159 | 6,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20240408` | 3861 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20240415` | 13183 | 9 | 159 | 2 |
| YKKS tarih bazli karekod/satis log | `YKKS20240416` | 9973 | 9 | 159 | 1,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20240417` | 11174 | 9 | 159 | 1,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20240418` | 15132 | 9 | 159 | 2,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20240419` | 61412 | 9 | 159 | 9,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20240420` | 8460 | 9 | 159 | 1,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20240422` | 13456 | 9 | 159 | 2,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20240424` | 12727 | 9 | 159 | 1,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20240425` | 13826 | 9 | 159 | 2,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240426` | 20414 | 9 | 159 | 3,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240427` | 19919 | 9 | 159 | 3,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20240429` | 21440 | 9 | 159 | 3,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20240430` | 23115 | 9 | 159 | 3,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20240502` | 18641 | 9 | 159 | 2,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20240503` | 13773 | 9 | 159 | 2,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20240504` | 72370 | 9 | 159 | 10,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20240506` | 12035 | 9 | 159 | 1,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20240507` | 15339 | 9 | 159 | 2,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20240508` | 45479 | 9 | 159 | 6,9 |
| YKKS tarih bazli karekod/satis log | `YKKS20240509` | 84089 | 9 | 159 | 12,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20240510` | 50579 | 9 | 159 | 7,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20240511` | 99553 | 9 | 159 | 15,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240513` | 49225 | 9 | 159 | 7,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20240514` | 68841 | 9 | 159 | 10,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240515` | 17291 | 9 | 159 | 2,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20240516` | 14332 | 9 | 159 | 2,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20240517` | 8672 | 9 | 159 | 1,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20240518` | 63635 | 9 | 159 | 9,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20240520` | 10558 | 9 | 159 | 1,6 |
| YKKS tarih bazli karekod/satis log | `YKKS20240521` | 9106 | 9 | 159 | 1,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20240522` | 13374 | 9 | 159 | 2,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20240523` | 10357 | 9 | 159 | 1,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240524` | 7703 | 9 | 159 | 1,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20240525` | 62263 | 9 | 159 | 9,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240527` | 17291 | 9 | 159 | 2,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20240528` | 14090 | 9 | 159 | 2,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20240529` | 86346 | 9 | 159 | 13,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20240530` | 34278 | 9 | 159 | 5,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20240531` | 5400 | 9 | 159 | 0,82 |
| YKKS tarih bazli karekod/satis log | `YKKS20240601` | 68990 | 9 | 159 | 10,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20240603` | 139439 | 9 | 159 | 21,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20240604` | 43256 | 9 | 159 | 6,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20240605` | 282440 | 9 | 159 | 42,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20240606` | 161693 | 9 | 159 | 24,52 |
| YKKS tarih bazli karekod/satis log | `YKKS20240607` | 15514 | 9 | 159 | 2,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20240608` | 94423 | 9 | 159 | 14,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20240610` | 71133 | 9 | 159 | 10,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20240611` | 10768 | 9 | 159 | 1,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20240612` | 11692 | 9 | 159 | 1,77 |
| YKKS tarih bazli karekod/satis log | `YKKS20240613` | 10737 | 9 | 159 | 1,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20240614` | 6605 | 9 | 159 | 1 |
| YKKS tarih bazli karekod/satis log | `YKKS20240620` | 71800 | 9 | 159 | 10,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20240621` | 1985 | 9 | 159 | 0,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20240622` | 60995 | 9 | 159 | 9,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20240624` | 5098 | 9 | 159 | 0,77 |
| YKKS tarih bazli karekod/satis log | `YKKS20240625` | 8637 | 9 | 159 | 1,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20240626` | 7099 | 9 | 159 | 1,08 |
| YKKS tarih bazli karekod/satis log | `YKKS20240627` | 55125 | 9 | 159 | 8,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20240628` | 51932 | 9 | 159 | 7,88 |
| YKKS tarih bazli karekod/satis log | `YKKS20240629` | 24672 | 9 | 159 | 3,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20240701` | 45836 | 9 | 159 | 6,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20240702` | 12371 | 9 | 159 | 1,88 |
| YKKS tarih bazli karekod/satis log | `YKKS20240703` | 90651 | 9 | 159 | 13,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20240704` | 29221 | 9 | 159 | 4,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20240705` | 21471 | 9 | 159 | 3,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240706` | 38465 | 9 | 159 | 5,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20240708` | 34608 | 9 | 159 | 5,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20240709` | 21764 | 9 | 159 | 3,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20240710` | 28669 | 9 | 159 | 4,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20240711` | 13348 | 9 | 159 | 2,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20240712` | 13465 | 9 | 159 | 2,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20240713` | 22146 | 9 | 159 | 3,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20240716` | 11507 | 9 | 159 | 1,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20240717` | 11117 | 9 | 159 | 1,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20240718` | 21519 | 9 | 159 | 3,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240719` | 103954 | 9 | 159 | 15,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20240720` | 96818 | 9 | 159 | 14,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20240722` | 10080 | 9 | 159 | 1,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20240723` | 8448 | 9 | 159 | 1,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20240724` | 12252 | 9 | 159 | 1,86 |
| YKKS tarih bazli karekod/satis log | `YKKS20240725` | 177511 | 9 | 159 | 26,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20240726` | 10739 | 9 | 159 | 1,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20240727` | 57794 | 9 | 159 | 8,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20240729` | 10470 | 9 | 159 | 1,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20240730` | 180671 | 9 | 159 | 27,4 |
| YKKS tarih bazli karekod/satis log | `YKKS20240731` | 166982 | 9 | 159 | 25,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20240801` | 18612 | 9 | 159 | 2,82 |
| YKKS tarih bazli karekod/satis log | `YKKS20240802` | 15304 | 9 | 159 | 2,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20240803` | 27306 | 9 | 159 | 4,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20240805` | 22685 | 9 | 159 | 3,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240806` | 10580 | 9 | 159 | 1,6 |
| YKKS tarih bazli karekod/satis log | `YKKS20240807` | 9135 | 9 | 159 | 1,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20240808` | 9480 | 9 | 159 | 1,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240809` | 15063 | 9 | 159 | 2,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20240810` | 20979 | 9 | 159 | 3,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20240812` | 27945 | 9 | 159 | 4,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20240813` | 9054 | 9 | 159 | 1,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20240814` | 15672 | 9 | 159 | 2,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20240815` | 12848 | 9 | 159 | 1,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20240816` | 8739 | 9 | 159 | 1,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20240817` | 767 | 9 | 159 | 0,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20240819` | 8309 | 9 | 159 | 1,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240820` | 8312 | 9 | 159 | 1,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240821` | 8201 | 9 | 159 | 1,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20240822` | 9414 | 9 | 159 | 1,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20240823` | 3577 | 9 | 159 | 0,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20240824` | 31356 | 9 | 159 | 4,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20240826` | 17933 | 9 | 159 | 2,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20240827` | 26459 | 9 | 159 | 4,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20240828` | 8280 | 9 | 159 | 1,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20240829` | 10702 | 9 | 159 | 1,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20240831` | 73850 | 9 | 159 | 11,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20240902` | 17010 | 9 | 159 | 2,58 |
| YKKS tarih bazli karekod/satis log | `YKKS20240903` | 17843 | 9 | 159 | 2,71 |
| YKKS tarih bazli karekod/satis log | `YKKS20240904` | 9790 | 9 | 159 | 1,49 |
| YKKS tarih bazli karekod/satis log | `YKKS20240905` | 10889 | 9 | 159 | 1,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20240906` | 39315 | 9 | 159 | 5,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20240907` | 64239 | 9 | 159 | 9,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20240909` | 8090 | 9 | 159 | 1,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20240910` | 11292 | 9 | 159 | 1,71 |
| YKKS tarih bazli karekod/satis log | `YKKS20240911` | 62924 | 9 | 159 | 9,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20240912` | 10614 | 9 | 159 | 1,61 |
| YKKS tarih bazli karekod/satis log | `YKKS20240913` | 9518 | 9 | 159 | 1,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20240914` | 13526 | 9 | 159 | 2,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20240916` | 7108 | 9 | 159 | 1,08 |
| YKKS tarih bazli karekod/satis log | `YKKS20240917` | 25641 | 9 | 159 | 3,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20240918` | 11339 | 9 | 159 | 1,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20240919` | 13887 | 9 | 159 | 2,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20240920` | 12298 | 9 | 159 | 1,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20240921` | 23532 | 9 | 159 | 3,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20240923` | 100455 | 9 | 159 | 15,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20240924` | 24368 | 9 | 159 | 3,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20240925` | 6850 | 9 | 159 | 1,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20240926` | 9774 | 9 | 159 | 1,48 |
| YKKS tarih bazli karekod/satis log | `YKKS20240927` | 83833 | 9 | 159 | 12,71 |
| YKKS tarih bazli karekod/satis log | `YKKS20240928` | 85691 | 9 | 159 | 12,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20240930` | 17423 | 9 | 159 | 2,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20241001` | 21370 | 9 | 159 | 3,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20241002` | 48752 | 9 | 159 | 7,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20241003` | 13913 | 9 | 159 | 2,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20241004` | 21916 | 9 | 159 | 3,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20241005` | 55335 | 9 | 159 | 8,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20241007` | 13773 | 9 | 159 | 2,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20241008` | 10829 | 9 | 159 | 1,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20241009` | 45934 | 9 | 159 | 6,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20241010` | 54286 | 9 | 159 | 8,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20241011` | 14255 | 9 | 159 | 2,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20241012` | 67376 | 9 | 159 | 10,22 |
| YKKS tarih bazli karekod/satis log | `YKKS20241014` | 78649 | 9 | 159 | 11,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20241015` | 121914 | 9 | 159 | 18,49 |
| YKKS tarih bazli karekod/satis log | `YKKS20241016` | 38285 | 9 | 159 | 5,81 |
| YKKS tarih bazli karekod/satis log | `YKKS20241017` | 48457 | 9 | 159 | 7,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20241018` | 36238 | 9 | 159 | 5,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20241019` | 45133 | 9 | 159 | 6,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20241021` | 7316 | 9 | 159 | 1,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20241022` | 17723 | 9 | 159 | 2,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20241023` | 11022 | 9 | 159 | 1,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20241024` | 61260 | 9 | 159 | 9,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20241025` | 48042 | 9 | 159 | 7,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20241026` | 90560 | 9 | 159 | 13,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20241028` | 7249 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20241030` | 15481 | 9 | 159 | 2,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20241031` | 18436 | 9 | 159 | 2,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20241101` | 64367 | 9 | 159 | 9,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20241102` | 142785 | 9 | 159 | 21,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20241104` | 132307 | 9 | 159 | 20,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20241105` | 9055 | 9 | 159 | 1,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20241106` | 189106 | 9 | 159 | 28,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20241107` | 26446 | 9 | 159 | 4,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20241108` | 45163 | 9 | 159 | 6,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20241109` | 16465 | 9 | 159 | 2,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20241111` | 12321 | 9 | 159 | 1,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20241112` | 6131 | 9 | 159 | 0,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20241113` | 40719 | 9 | 159 | 6,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20241114` | 40750 | 9 | 159 | 6,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20241115` | 3539 | 9 | 159 | 0,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20241116` | 56396 | 9 | 159 | 8,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20241118` | 6442 | 9 | 159 | 0,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20241119` | 8187 | 9 | 159 | 1,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20241120` | 91621 | 9 | 159 | 13,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20241121` | 19467 | 9 | 159 | 2,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20241122` | 3059 | 9 | 159 | 0,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20241123` | 61500 | 9 | 159 | 9,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20241125` | 10089 | 9 | 159 | 1,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20241126` | 4197 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20241127` | 23649 | 9 | 159 | 3,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20241128` | 80089 | 9 | 159 | 12,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20241129` | 16156 | 9 | 159 | 2,45 |
| YKKS tarih bazli karekod/satis log | `YKKS20241130` | 2931 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20241202` | 34344 | 9 | 159 | 5,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20241203` | 7445 | 9 | 159 | 1,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20241204` | 5586 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20241205` | 11156 | 9 | 159 | 1,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20241206` | 11342 | 9 | 159 | 1,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20241207` | 27429 | 9 | 159 | 4,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20241209` | 12874 | 9 | 159 | 1,95 |
| YKKS tarih bazli karekod/satis log | `YKKS20241210` | 9052 | 9 | 159 | 1,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20241211` | 20655 | 9 | 159 | 3,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20241212` | 8378 | 9 | 159 | 1,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20241213` | 4871 | 9 | 159 | 0,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20241214` | 17425 | 9 | 159 | 2,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20241216` | 5938 | 9 | 159 | 0,9 |
| YKKS tarih bazli karekod/satis log | `YKKS20241217` | 26565 | 9 | 159 | 4,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20241218` | 58613 | 9 | 159 | 8,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20241219` | 4173 | 9 | 159 | 0,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20241220` | 15940 | 9 | 159 | 2,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20241221` | 26498 | 9 | 159 | 4,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20241223` | 5533 | 9 | 159 | 0,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20241224` | 11962 | 9 | 159 | 1,81 |
| YKKS tarih bazli karekod/satis log | `YKKS20241225` | 5630 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20241226` | 8709 | 9 | 159 | 1,32 |
| YKKS tarih bazli karekod/satis log | `YKKS20241227` | 18802 | 9 | 159 | 2,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20241228` | 50049 | 9 | 159 | 7,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20241230` | 106744 | 9 | 159 | 16,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20241231` | 54795 | 9 | 159 | 8,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20250102` | 34454 | 9 | 159 | 5,22 |
| YKKS tarih bazli karekod/satis log | `YKKS20250103` | 14270 | 9 | 159 | 2,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20250104` | 18215 | 9 | 159 | 2,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20250106` | 26253 | 9 | 159 | 3,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20250107` | 59729 | 9 | 159 | 9,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20250108` | 11748 | 9 | 159 | 1,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20250109` | 10325 | 9 | 159 | 1,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20250110` | 8762 | 9 | 159 | 1,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20250111` | 48926 | 9 | 159 | 7,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20250113` | 108513 | 9 | 159 | 16,45 |
| YKKS tarih bazli karekod/satis log | `YKKS20250114` | 37893 | 9 | 159 | 5,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20250115` | 15484 | 9 | 159 | 2,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20250116` | 6444 | 9 | 159 | 0,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20250117` | 7251 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250118` | 19956 | 9 | 159 | 3,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20250120` | 11633 | 9 | 159 | 1,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20250121` | 19791 | 9 | 159 | 3 |
| YKKS tarih bazli karekod/satis log | `YKKS20250122` | 13066 | 9 | 159 | 1,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20250123` | 11622 | 9 | 159 | 1,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20250124` | 64613 | 9 | 159 | 9,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20250125` | 14861 | 9 | 159 | 2,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20250127` | 12036 | 9 | 159 | 1,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20250128` | 45291 | 9 | 159 | 6,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20250129` | 14811 | 9 | 159 | 2,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20250130` | 3861 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20250131` | 29664 | 9 | 159 | 4,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20250201` | 21459 | 9 | 159 | 3,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20250203` | 19038 | 9 | 159 | 2,89 |
| YKKS tarih bazli karekod/satis log | `YKKS20250204` | 11761 | 9 | 159 | 1,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20250205` | 12997 | 9 | 159 | 1,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20250206` | 9871 | 9 | 159 | 1,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20250207` | 67835 | 9 | 159 | 10,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20250208` | 20908 | 9 | 159 | 3,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20250210` | 25908 | 9 | 159 | 3,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20250211` | 22669 | 9 | 159 | 3,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20250212` | 9070 | 9 | 159 | 1,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20250213` | 7452 | 9 | 159 | 1,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20250214` | 12164 | 9 | 159 | 1,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20250215` | 94869 | 9 | 159 | 14,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20250217` | 7310 | 9 | 159 | 1,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20250218` | 10290 | 9 | 159 | 1,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20250219` | 6528 | 9 | 159 | 0,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20250220` | 8331 | 9 | 159 | 1,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20250221` | 3041 | 9 | 159 | 0,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20250222` | 29531 | 9 | 159 | 4,48 |
| YKKS tarih bazli karekod/satis log | `YKKS20250224` | 9376 | 9 | 159 | 1,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20250225` | 6087 | 9 | 159 | 0,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20250226` | 4286 | 9 | 159 | 0,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20250227` | 34018 | 9 | 159 | 5,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20250228` | 5713 | 9 | 159 | 0,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20250301` | 29927 | 9 | 159 | 4,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20250303` | 7242 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250304` | 37242 | 9 | 159 | 5,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20250305` | 25920 | 9 | 159 | 3,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20250306` | 45989 | 9 | 159 | 6,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20250307` | 8832 | 9 | 159 | 1,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20250308` | 48398 | 9 | 159 | 7,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20250310` | 5748 | 9 | 159 | 0,87 |
| YKKS tarih bazli karekod/satis log | `YKKS20250311` | 31835 | 9 | 159 | 4,83 |
| YKKS tarih bazli karekod/satis log | `YKKS20250312` | 8511 | 9 | 159 | 1,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20250313` | 9735 | 9 | 159 | 1,48 |
| YKKS tarih bazli karekod/satis log | `YKKS20250314` | 27217 | 9 | 159 | 4,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20250315` | 79184 | 9 | 159 | 12,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20250317` | 22830 | 9 | 159 | 3,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20250318` | 26576 | 9 | 159 | 4,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20250319` | 7397 | 9 | 159 | 1,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20250320` | 5437 | 9 | 159 | 0,82 |
| YKKS tarih bazli karekod/satis log | `YKKS20250321` | 9386 | 9 | 159 | 1,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20250322` | 17937 | 9 | 159 | 2,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20250324` | 6332 | 9 | 159 | 0,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20250325` | 6552 | 9 | 159 | 0,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20250326` | 5625 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20250327` | 8582 | 9 | 159 | 1,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20250328` | 2454 | 9 | 159 | 0,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20250402` | 7240 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250403` | 3278 | 9 | 159 | 0,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20250404` | 5635 | 9 | 159 | 0,86 |
| YKKS tarih bazli karekod/satis log | `YKKS20250405` | 20001 | 9 | 159 | 3,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20250407` | 9021 | 9 | 159 | 1,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20250408` | 18402 | 9 | 159 | 2,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20250409` | 7338 | 9 | 159 | 1,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20250410` | 15377 | 9 | 159 | 2,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20250411` | 6365 | 9 | 159 | 0,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20250412` | 16186 | 9 | 159 | 2,45 |
| YKKS tarih bazli karekod/satis log | `YKKS20250414` | 8159 | 9 | 159 | 1,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20250415` | 5374 | 9 | 159 | 0,82 |
| YKKS tarih bazli karekod/satis log | `YKKS20250416` | 13321 | 9 | 159 | 2,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20250417` | 9690 | 9 | 159 | 1,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20250418` | 7278 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250419` | 18100 | 9 | 159 | 2,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20250421` | 4953 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20250422` | 8767 | 9 | 159 | 1,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20250424` | 7611 | 9 | 159 | 1,15 |
| YKKS tarih bazli karekod/satis log | `YKKS20250425` | 9106 | 9 | 159 | 1,38 |
| YKKS tarih bazli karekod/satis log | `YKKS20250426` | 13540 | 9 | 159 | 2,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20250428` | 4524 | 9 | 159 | 0,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20250429` | 20832 | 9 | 159 | 3,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20250430` | 28593 | 9 | 159 | 4,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20250502` | 10400 | 9 | 159 | 1,58 |
| YKKS tarih bazli karekod/satis log | `YKKS20250503` | 43716 | 9 | 159 | 6,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20250505` | 3796 | 9 | 159 | 0,58 |
| YKKS tarih bazli karekod/satis log | `YKKS20250506` | 118833 | 9 | 159 | 18,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20250507` | 30601 | 9 | 159 | 4,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20250508` | 11407 | 9 | 159 | 1,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20250509` | 7757 | 9 | 159 | 1,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20250510` | 93871 | 9 | 159 | 14,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20250512` | 10550 | 9 | 159 | 1,6 |
| YKKS tarih bazli karekod/satis log | `YKKS20250513` | 22151 | 9 | 159 | 3,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20250514` | 8826 | 9 | 159 | 1,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20250515` | 14294 | 9 | 159 | 2,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20250516` | 4213 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20250517` | 20390 | 9 | 159 | 3,09 |
| YKKS tarih bazli karekod/satis log | `YKKS20250520` | 7468 | 9 | 159 | 1,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20250521` | 10696 | 9 | 159 | 1,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20250522` | 9266 | 9 | 159 | 1,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20250523` | 3727 | 9 | 159 | 0,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20250524` | 13423 | 9 | 159 | 2,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20250526` | 6009 | 9 | 159 | 0,91 |
| YKKS tarih bazli karekod/satis log | `YKKS20250527` | 4793 | 9 | 159 | 0,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20250528` | 4733 | 9 | 159 | 0,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20250529` | 4943 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20250530` | 3036 | 9 | 159 | 0,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20250531` | 294 | 9 | 159 | 0,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20250602` | 8174 | 9 | 159 | 1,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20250603` | 24227 | 9 | 159 | 3,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20250604` | 132531 | 9 | 159 | 20,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250605` | 41507 | 9 | 159 | 6,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20250610` | 4795 | 9 | 159 | 0,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20250611` | 21026 | 9 | 159 | 3,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20250612` | 56097 | 9 | 159 | 8,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20250613` | 7377 | 9 | 159 | 1,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20250614` | 6298 | 9 | 159 | 0,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20250616` | 10298 | 9 | 159 | 1,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20250617` | 26324 | 9 | 159 | 3,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20250618` | 4539 | 9 | 159 | 0,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20250619` | 5231 | 9 | 159 | 0,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20250620` | 14532 | 9 | 159 | 2,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20250621` | 72720 | 9 | 159 | 11,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20250623` | 14635 | 9 | 159 | 2,22 |
| YKKS tarih bazli karekod/satis log | `YKKS20250624` | 18344 | 9 | 159 | 2,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20250625` | 109345 | 9 | 159 | 16,58 |
| YKKS tarih bazli karekod/satis log | `YKKS20250626` | 29450 | 9 | 159 | 4,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20250627` | 27791 | 9 | 159 | 4,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20250628` | 15204 | 9 | 159 | 2,31 |
| YKKS tarih bazli karekod/satis log | `YKKS20250630` | 4778 | 9 | 159 | 0,73 |
| YKKS tarih bazli karekod/satis log | `YKKS20250701` | 6629 | 9 | 159 | 1,01 |
| YKKS tarih bazli karekod/satis log | `YKKS20250702` | 3421 | 9 | 159 | 0,52 |
| YKKS tarih bazli karekod/satis log | `YKKS20250703` | 52705 | 9 | 159 | 7,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20250704` | 1526 | 9 | 159 | 0,23 |
| YKKS tarih bazli karekod/satis log | `YKKS20250705` | 317 | 9 | 159 | 0,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20250707` | 1158 | 9 | 159 | 0,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20250708` | 2995 | 9 | 159 | 0,45 |
| YKKS tarih bazli karekod/satis log | `YKKS20250709` | 32160 | 9 | 159 | 4,88 |
| YKKS tarih bazli karekod/satis log | `YKKS20250710` | 5034 | 9 | 159 | 0,76 |
| YKKS tarih bazli karekod/satis log | `YKKS20250711` | 973 | 9 | 159 | 0,15 |
| YKKS tarih bazli karekod/satis log | `YKKS20250712` | 6148 | 9 | 159 | 0,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20250714` | 1783 | 9 | 159 | 0,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20250716` | 1553 | 9 | 159 | 0,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20250717` | 2425 | 9 | 159 | 0,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20250718` | 102296 | 9 | 159 | 15,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20250719` | 948 | 9 | 159 | 0,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20250721` | 1595 | 9 | 159 | 0,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20250722` | 62583 | 9 | 159 | 9,49 |
| YKKS tarih bazli karekod/satis log | `YKKS20250723` | 13381 | 9 | 159 | 2,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20250724` | 26209 | 9 | 159 | 3,97 |
| YKKS tarih bazli karekod/satis log | `YKKS20250725` | 3091 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20250728` | 40233 | 9 | 159 | 6,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250729` | 17339 | 9 | 159 | 2,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20250730` | 3064 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20250731` | 1236 | 9 | 159 | 0,19 |
| YKKS tarih bazli karekod/satis log | `YKKS20250801` | 9310 | 9 | 159 | 1,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20250802` | 258 | 9 | 159 | 0,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20250804` | 4302 | 9 | 159 | 0,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20250805` | 3476 | 9 | 159 | 0,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20250806` | 2342 | 9 | 159 | 0,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20250807` | 3902 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20250808` | 8506 | 9 | 159 | 1,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20250809` | 1848 | 9 | 159 | 0,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20250811` | 2896 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20250812` | 4747 | 9 | 159 | 0,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20250813` | 3328 | 9 | 159 | 0,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20250814` | 1479 | 9 | 159 | 0,22 |
| YKKS tarih bazli karekod/satis log | `YKKS20250815` | 2278 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20250816` | 248 | 9 | 159 | 0,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20250818` | 4067 | 9 | 159 | 0,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20250819` | 10046 | 9 | 159 | 1,52 |
| YKKS tarih bazli karekod/satis log | `YKKS20250820` | 7668 | 9 | 159 | 1,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20250821` | 6702 | 9 | 159 | 1,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20250822` | 13312 | 9 | 159 | 2,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20250823` | 4177 | 9 | 159 | 0,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20250825` | 13030 | 9 | 159 | 1,98 |
| YKKS tarih bazli karekod/satis log | `YKKS20250826` | 7360 | 9 | 159 | 1,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20250827` | 8016 | 9 | 159 | 1,22 |
| YKKS tarih bazli karekod/satis log | `YKKS20250828` | 2862 | 9 | 159 | 0,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20250829` | 3351 | 9 | 159 | 0,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20250901` | 11162 | 9 | 159 | 1,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20250902` | 7243 | 9 | 159 | 1,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250903` | 10257 | 9 | 159 | 1,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20250904` | 6997 | 9 | 159 | 1,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20250905` | 5238 | 9 | 159 | 0,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20250906` | 3919 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20250908` | 3890 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20250909` | 11203 | 9 | 159 | 1,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20250910` | 9482 | 9 | 159 | 1,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20250911` | 6514 | 9 | 159 | 0,99 |
| YKKS tarih bazli karekod/satis log | `YKKS20250912` | 4307 | 9 | 159 | 0,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20250913` | 1031 | 9 | 159 | 0,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20250915` | 5061 | 9 | 159 | 0,77 |
| YKKS tarih bazli karekod/satis log | `YKKS20250916` | 3781 | 9 | 159 | 0,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20250917` | 4627 | 9 | 159 | 0,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20250918` | 4933 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20250919` | 3168 | 9 | 159 | 0,48 |
| YKKS tarih bazli karekod/satis log | `YKKS20250920` | 3620 | 9 | 159 | 0,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20250922` | 11053 | 9 | 159 | 1,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20250923` | 5572 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20250924` | 9917 | 9 | 159 | 1,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20250925` | 4063 | 9 | 159 | 0,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20250926` | 3786 | 9 | 159 | 0,57 |
| YKKS tarih bazli karekod/satis log | `YKKS20250927` | 13823 | 9 | 159 | 2,1 |
| YKKS tarih bazli karekod/satis log | `YKKS20250929` | 4177 | 9 | 159 | 0,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20250930` | 2233 | 9 | 159 | 0,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20251001` | 11284 | 9 | 159 | 1,71 |
| YKKS tarih bazli karekod/satis log | `YKKS20251002` | 22138 | 9 | 159 | 3,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20251003` | 11558 | 9 | 159 | 1,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20251004` | 24076 | 9 | 159 | 3,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20251006` | 13185 | 9 | 159 | 2 |
| YKKS tarih bazli karekod/satis log | `YKKS20251007` | 17188 | 9 | 159 | 2,61 |
| YKKS tarih bazli karekod/satis log | `YKKS20251008` | 28350 | 9 | 159 | 4,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20251009` | 17070 | 9 | 159 | 2,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20251010` | 5564 | 9 | 159 | 0,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20251011` | 1303 | 9 | 159 | 0,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20251012` | 6000 | 9 | 159 | 0,91 |
| YKKS tarih bazli karekod/satis log | `YKKS20251013` | 5541 | 9 | 159 | 0,84 |
| YKKS tarih bazli karekod/satis log | `YKKS20251014` | 8212 | 9 | 159 | 1,25 |
| YKKS tarih bazli karekod/satis log | `YKKS20251015` | 6109 | 9 | 159 | 0,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20251016` | 9187 | 9 | 159 | 1,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20251017` | 6724 | 9 | 159 | 1,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20251018` | 839 | 9 | 159 | 0,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20251020` | 12693 | 9 | 159 | 1,93 |
| YKKS tarih bazli karekod/satis log | `YKKS20251021` | 4924 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20251022` | 4397 | 9 | 159 | 0,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20251023` | 5217 | 9 | 159 | 0,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20251024` | 27933 | 9 | 159 | 4,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20251025` | 11151 | 9 | 159 | 1,69 |
| YKKS tarih bazli karekod/satis log | `YKKS20251027` | 1713 | 9 | 159 | 0,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20251028` | 850 | 9 | 159 | 0,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20251030` | 1548 | 9 | 159 | 0,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20251031` | 1100 | 9 | 159 | 0,17 |
| YKKS tarih bazli karekod/satis log | `YKKS20251101` | 13416 | 9 | 159 | 2,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20251103` | 6047 | 9 | 159 | 0,92 |
| YKKS tarih bazli karekod/satis log | `YKKS20251104` | 7331 | 9 | 159 | 1,11 |
| YKKS tarih bazli karekod/satis log | `YKKS20251105` | 16644 | 9 | 159 | 2,52 |
| YKKS tarih bazli karekod/satis log | `YKKS20251106` | 11016 | 9 | 159 | 1,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20251107` | 3239 | 9 | 159 | 0,49 |
| YKKS tarih bazli karekod/satis log | `YKKS20251108` | 9601 | 9 | 159 | 1,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20251110` | 23162 | 9 | 159 | 3,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20251111` | 6316 | 9 | 159 | 0,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20251112` | 7465 | 9 | 159 | 1,13 |
| YKKS tarih bazli karekod/satis log | `YKKS20251113` | 4185 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20251114` | 2290 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20251115` | 131 | 9 | 159 | 0,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20251117` | 21750 | 9 | 159 | 3,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20251118` | 6215 | 9 | 159 | 0,94 |
| YKKS tarih bazli karekod/satis log | `YKKS20251119` | 4506 | 9 | 159 | 0,68 |
| YKKS tarih bazli karekod/satis log | `YKKS20251120` | 4079 | 9 | 159 | 0,62 |
| YKKS tarih bazli karekod/satis log | `YKKS20251121` | 14261 | 9 | 159 | 2,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20251122` | 18834 | 9 | 159 | 2,86 |
| YKKS tarih bazli karekod/satis log | `YKKS20251124` | 7960 | 9 | 159 | 1,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20251125` | 5621 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20251126` | 7973 | 9 | 159 | 1,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20251127` | 2861 | 9 | 159 | 0,43 |
| YKKS tarih bazli karekod/satis log | `YKKS20251128` | 5333 | 9 | 159 | 0,81 |
| YKKS tarih bazli karekod/satis log | `YKKS20251129` | 960 | 9 | 159 | 0,15 |
| YKKS tarih bazli karekod/satis log | `YKKS20251201` | 6701 | 9 | 159 | 1,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20251202` | 1799 | 9 | 159 | 0,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20251203` | 6619 | 9 | 159 | 1 |
| YKKS tarih bazli karekod/satis log | `YKKS20251204` | 5977 | 9 | 159 | 0,91 |
| YKKS tarih bazli karekod/satis log | `YKKS20251205` | 3710 | 9 | 159 | 0,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20251206` | 1007 | 9 | 159 | 0,15 |
| YKKS tarih bazli karekod/satis log | `YKKS20251208` | 3004 | 9 | 159 | 0,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20251209` | 5258 | 9 | 159 | 0,8 |
| YKKS tarih bazli karekod/satis log | `YKKS20251210` | 4905 | 9 | 159 | 0,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20251211` | 4249 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20251212` | 3100 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20251213` | 1752 | 9 | 159 | 0,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20251215` | 3483 | 9 | 159 | 0,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20251216` | 3953 | 9 | 159 | 0,6 |
| YKKS tarih bazli karekod/satis log | `YKKS20251217` | 6332 | 9 | 159 | 0,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20251218` | 2587 | 9 | 159 | 0,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20251219` | 8803 | 9 | 159 | 1,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20251220` | 2905 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20251222` | 5690 | 9 | 159 | 0,86 |
| YKKS tarih bazli karekod/satis log | `YKKS20251223` | 7964 | 9 | 159 | 1,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20251224` | 10210 | 9 | 159 | 1,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20251225` | 3628 | 9 | 159 | 0,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20251226` | 7404 | 9 | 159 | 1,12 |
| YKKS tarih bazli karekod/satis log | `YKKS20251227` | 37157 | 9 | 159 | 5,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20251229` | 3114 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20251230` | 5044 | 9 | 159 | 0,77 |
| YKKS tarih bazli karekod/satis log | `YKKS20251231` | 23383 | 9 | 159 | 3,55 |
| YKKS tarih bazli karekod/satis log | `YKKS20260102` | 5901 | 9 | 159 | 0,9 |
| YKKS tarih bazli karekod/satis log | `YKKS20260103` | 478 | 9 | 159 | 0,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20260105` | 6829 | 9 | 159 | 1,04 |
| YKKS tarih bazli karekod/satis log | `YKKS20260106` | 4968 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20260107` | 19902 | 9 | 159 | 3,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20260108` | 10600 | 9 | 159 | 1,61 |
| YKKS tarih bazli karekod/satis log | `YKKS20260109` | 2348 | 9 | 159 | 0,36 |
| YKKS tarih bazli karekod/satis log | `YKKS20260110` | 12498 | 9 | 159 | 1,9 |
| YKKS tarih bazli karekod/satis log | `YKKS20260112` | 6342 | 9 | 159 | 0,96 |
| YKKS tarih bazli karekod/satis log | `YKKS20260113` | 8579 | 9 | 159 | 1,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20260114` | 5631 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20260115` | 8807 | 9 | 159 | 1,34 |
| YKKS tarih bazli karekod/satis log | `YKKS20260116` | 7662 | 9 | 159 | 1,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20260117` | 4426 | 9 | 159 | 0,67 |
| YKKS tarih bazli karekod/satis log | `YKKS20260119` | 9868 | 9 | 159 | 1,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20260120` | 61128 | 9 | 159 | 9,27 |
| YKKS tarih bazli karekod/satis log | `YKKS20260121` | 17465 | 9 | 159 | 2,65 |
| YKKS tarih bazli karekod/satis log | `YKKS20260122` | 18304 | 9 | 159 | 2,78 |
| YKKS tarih bazli karekod/satis log | `YKKS20260123` | 6712 | 9 | 159 | 1,02 |
| YKKS tarih bazli karekod/satis log | `YKKS20260124` | 5199 | 9 | 159 | 0,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20260126` | 2564 | 9 | 159 | 0,39 |
| YKKS tarih bazli karekod/satis log | `YKKS20260127` | 29383 | 9 | 159 | 4,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20260128` | 8442 | 9 | 159 | 1,28 |
| YKKS tarih bazli karekod/satis log | `YKKS20260129` | 7055 | 9 | 159 | 1,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20260130` | 29412 | 9 | 159 | 4,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20260131` | 942 | 9 | 159 | 0,14 |
| YKKS tarih bazli karekod/satis log | `YKKS20260202` | 5687 | 9 | 159 | 0,86 |
| YKKS tarih bazli karekod/satis log | `YKKS20260203` | 4583 | 9 | 159 | 0,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20260204` | 3533 | 9 | 159 | 0,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20260205` | 14347 | 9 | 159 | 2,18 |
| YKKS tarih bazli karekod/satis log | `YKKS20260206` | 7654 | 9 | 159 | 1,16 |
| YKKS tarih bazli karekod/satis log | `YKKS20260207` | 2435 | 9 | 159 | 0,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20260209` | 3544 | 9 | 159 | 0,54 |
| YKKS tarih bazli karekod/satis log | `YKKS20260210` | 5094 | 9 | 159 | 0,77 |
| YKKS tarih bazli karekod/satis log | `YKKS20260211` | 6009 | 9 | 159 | 0,91 |
| YKKS tarih bazli karekod/satis log | `YKKS20260212` | 15359 | 9 | 159 | 2,33 |
| YKKS tarih bazli karekod/satis log | `YKKS20260213` | 4187 | 9 | 159 | 0,64 |
| YKKS tarih bazli karekod/satis log | `YKKS20260214` | 4168 | 9 | 159 | 0,63 |
| YKKS tarih bazli karekod/satis log | `YKKS20260216` | 4616 | 9 | 159 | 0,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20260217` | 3001 | 9 | 159 | 0,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20260218` | 2284 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20260219` | 2302 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20260220` | 1886 | 9 | 159 | 0,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20260221` | 471 | 9 | 159 | 0,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20260223` | 1316 | 9 | 159 | 0,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20260224` | 3884 | 9 | 159 | 0,59 |
| YKKS tarih bazli karekod/satis log | `YKKS20260225` | 9219 | 9 | 159 | 1,4 |
| YKKS tarih bazli karekod/satis log | `YKKS20260226` | 2294 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20260227` | 508 | 9 | 159 | 0,08 |
| YKKS tarih bazli karekod/satis log | `YKKS20260228` | 213 | 9 | 159 | 0,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20260302` | 3371 | 9 | 159 | 0,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20260303` | 3135 | 9 | 159 | 0,48 |
| YKKS tarih bazli karekod/satis log | `YKKS20260304` | 2426 | 9 | 159 | 0,37 |
| YKKS tarih bazli karekod/satis log | `YKKS20260305` | 2888 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20260306` | 3661 | 9 | 159 | 0,56 |
| YKKS tarih bazli karekod/satis log | `YKKS20260307` | 473 | 9 | 159 | 0,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20260309` | 4731 | 9 | 159 | 0,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20260310` | 5209 | 9 | 159 | 0,79 |
| YKKS tarih bazli karekod/satis log | `YKKS20260311` | 2913 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20260312` | 5601 | 9 | 159 | 0,85 |
| YKKS tarih bazli karekod/satis log | `YKKS20260313` | 8185 | 9 | 159 | 1,24 |
| YKKS tarih bazli karekod/satis log | `YKKS20260314` | 462 | 9 | 159 | 0,07 |
| YKKS tarih bazli karekod/satis log | `YKKS20260316` | 4776 | 9 | 159 | 0,72 |
| YKKS tarih bazli karekod/satis log | `YKKS20260317` | 2003 | 9 | 159 | 0,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20260318` | 1366 | 9 | 159 | 0,21 |
| YKKS tarih bazli karekod/satis log | `YKKS20260323` | 2755 | 9 | 159 | 0,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20260324` | 2292 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20260325` | 3069 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20260326` | 4590 | 9 | 159 | 0,7 |
| YKKS tarih bazli karekod/satis log | `YKKS20260327` | 1917 | 9 | 159 | 0,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20260328` | 16222 | 9 | 159 | 2,46 |
| YKKS tarih bazli karekod/satis log | `YKKS20260330` | 3326 | 9 | 159 | 0,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20260331` | 2331 | 9 | 159 | 0,35 |
| YKKS tarih bazli karekod/satis log | `YKKS20260401` | 6996 | 9 | 159 | 1,06 |
| YKKS tarih bazli karekod/satis log | `YKKS20260402` | 22479 | 9 | 159 | 3,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20260403` | 6608 | 9 | 159 | 1 |
| YKKS tarih bazli karekod/satis log | `YKKS20260404` | 1950 | 9 | 159 | 0,3 |
| YKKS tarih bazli karekod/satis log | `YKKS20260406` | 2683 | 9 | 159 | 0,41 |
| YKKS tarih bazli karekod/satis log | `YKKS20260407` | 9362 | 9 | 159 | 1,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20260408` | 2881 | 9 | 159 | 0,44 |
| YKKS tarih bazli karekod/satis log | `YKKS20260409` | 10066 | 9 | 159 | 1,53 |
| YKKS tarih bazli karekod/satis log | `YKKS20260410` | 1309 | 9 | 159 | 0,2 |
| YKKS tarih bazli karekod/satis log | `YKKS20260411` | 501 | 9 | 159 | 0,08 |
| YKKS tarih bazli karekod/satis log | `YKKS20260413` | 3358 | 9 | 159 | 0,51 |
| YKKS tarih bazli karekod/satis log | `YKKS20260414` | 6782 | 9 | 159 | 1,03 |
| YKKS tarih bazli karekod/satis log | `YKKS20260415` | 7126 | 9 | 159 | 1,08 |
| YKKS tarih bazli karekod/satis log | `YKKS20260416` | 4845 | 9 | 159 | 0,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20260417` | 4974 | 9 | 159 | 0,75 |
| YKKS tarih bazli karekod/satis log | `YKKS20260418` | 306 | 9 | 159 | 0,05 |
| YKKS tarih bazli karekod/satis log | `YKKS20260420` | 15102 | 9 | 159 | 2,29 |
| YKKS tarih bazli karekod/satis log | `YKKS20260421` | 3084 | 9 | 159 | 0,47 |
| YKKS tarih bazli karekod/satis log | `YKKS20260422` | 3938 | 9 | 159 | 0,6 |
| YKKS tarih bazli karekod/satis log | `YKKS20260424` | 1728 | 9 | 159 | 0,26 |
| YKKS tarih bazli karekod/satis log | `YKKS20260425` | 2749 | 9 | 159 | 0,42 |
| YKKS tarih bazli karekod/satis log | `YKKS20260427` | 3297 | 9 | 159 | 0,5 |
| YKKS tarih bazli karekod/satis log | `YKKS20260428` | 4860 | 9 | 159 | 0,74 |
| YKKS tarih bazli karekod/satis log | `YKKS20260429` | 466 | 9 | 159 | 0,07 |

## 18. Ana DBF Sema Ciktilari

Asagidaki semalar DBF dosya basliklarindan otomatik cikarilmistir. Veri icerigi rapora alinmamistir; yalnizca tablo/alan metadata kullanilmistir.

### ACD_KAMPANYA
- Dosya: F:\DEPO\DATA\01\ACD_KAMPANYA.DBF
- Kayit: 1974919; Alan: 20; Kayit uzunlugu: 93; Boyut MB: 175.16
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| KAMPANYA | C | 2 | 0 |
| CFIYATI | N | 12 | 0 |
| KM | N | 4 | 0 |
| KMF | N | 4 | 0 |
| KM2 | N | 4 | 0 |
| KMF2 | N | 4 | 0 |
| KM3 | N | 4 | 0 |
| KMF3 | N | 4 | 0 |
| ISLEM | C | 1 | 0 |
| TARIH | D | 8 | 0 |
| SAAT | C | 5 | 0 |
| YAPAN | C | 8 | 0 |
| MFTIP | C | 1 | 0 |
| KM4 | N | 4 | 0 |
| KMF4 | N | 4 | 0 |
| KM5 | N | 4 | 0 |
| KMF5 | N | 5 | 0 |
| TARIH1 | N | 4 | 0 |
| TARIH2 | N | 4 | 0 |

### BAKIYE
- Dosya: F:\DEPO\DATA\01\BAKIYE.DBF
- Kayit: 1294; Alan: 11; Kayit uzunlugu: 79; Boyut MB: 0.1
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| ECZANEKODU | C | 10 | 0 |
| TARIH | D | 8 | 0 |
| STATU | C | 2 | 0 |
| MIKTARI | N | 7 | 0 |
| MALFAZLASI | N | 6 | 0 |
| FIYATI | N | 12 | 0 |
| ACIKLAMA | C | 15 | 0 |
| ALANKISI | C | 1 | 0 |
| GIREN | C | 10 | 0 |
| BAKISONUC | C | 1 | 0 |

### BARKOD
- Dosya: F:\DEPO\DATA\01\BARKOD.DBF
- Kayit: 481; Alan: 3; Kayit uzunlugu: 35; Boyut MB: 0.02
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| BARKODU | C | 20 | 0 |
| ILACKODU | N | 6 | 0 |
| TARIH | D | 8 | 0 |

### BEKLEYEN
- Dosya: F:\DEPO\DATA\01\BEKLEYEN.DBF
- Kayit: 343; Alan: 60; Kayit uzunlugu: 1040; Boyut MB: 0.34
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| IRSALIYENO | N | 7 | 0 |
| TARIH | D | 8 | 0 |
| VETISK | N | 5 | 2 |
| ECZANEKODU | C | 10 | 0 |
| STATU | C | 2 | 0 |
| KALEMSAY | N | 4 | 0 |
| ACIKLAMA | C | 24 | 0 |
| TOPTUTAR | N | 15 | 0 |
| BOLGE | C | 2 | 0 |
| ALTBOLGE | N | 3 | 0 |
| IDOSYA | C | 8 | 0 |
| ISTATU | C | 30 | 0 |
| IONCELIK | C | 1 | 0 |
| IKULLNO | C | 3 | 0 |
| IZAMAN | C | 5 | 0 |
| PARSEL | N | 3 | 0 |
| REYONBITIS | N | 6 | 0 |
| ARALIK | N | 3 | 0 |
| TAKSITSAY | N | 2 | 0 |
| VADE | D | 8 | 0 |
| IMFYT | C | 1 | 0 |
| MFKDV | C | 1 | 0 |
| ISKONTOSU2 | N | 5 | 2 |
| GIREN | C | 8 | 0 |
| FATURANO | N | 7 | 0 |
| BASSAAT | N | 10 | 0 |
| BITSAAT | N | 10 | 0 |
| DUZELTEN | C | 8 | 0 |
| SIPREFNO | C | 16 | 0 |
| SEVKSEKLI | C | 1 | 0 |
| CEPNO | C | 2 | 0 |
| SEVKCEP | C | 2 | 0 |
| FTRTARIH | D | 8 | 0 |
| FTRSAAT | C | 5 | 0 |
| FIRMA | C | 3 | 0 |
| SIRKETTIP | C | 1 | 0 |
| CEPALDI | N | 1 | 0 |
| ITHALDURUM | C | 1 | 0 |
| DOVIZ | C | 4 | 0 |
| DOVIZKUR | N | 12 | 0 |
| KAYNAK | C | 1 | 0 |
| NAVLUN | N | 10 | 0 |
| IPTALFATNO | C | 7 | 0 |
| TAKIPNO | C | 7 | 0 |
| KUTUSAYISI | N | 8 | 0 |
| SEPETSAYI | N | 3 | 0 |
| SEPETLER | C | 48 | 0 |
| IHALEACK1 | C | 150 | 0 |
| IHALEACK2 | C | 150 | 0 |
| IHALEACK3 | C | 150 | 0 |
| IHALEADI | C | 150 | 0 |
| IHALEKYTNO | C | 40 | 0 |
| IHALETRH | D | 8 | 0 |
| VADEGUN | N | 5 | 0 |
| BILGIMAIL | N | 1 | 0 |
| PUANONAY | C | 1 | 0 |
| SEVKKODU | C | 2 | 0 |
| KARGOKODU | C | 16 | 0 |
| PUANKULLAN | N | 12 | 0 |
| VERGINO | C | 15 | 0 |

### BELGENO
- Dosya: F:\DEPO\DATA\01\BELGENO.DBF
- Kayit: 461; Alan: 5; Kayit uzunlugu: 33; Boyut MB: 0.01
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| BELGETIPI | C | 10 | 0 |
| XBNO | N | 10 | 0 |
| YAZICITUR | C | 1 | 0 |
| TARIH | D | 8 | 0 |
| SERINO | C | 3 | 0 |

### DETAYG
- Dosya: F:\DEPO\DATA\01\DETAYG.DBF
- Kayit: 172468; Alan: 43; Kayit uzunlugu: 206; Boyut MB: 33.88
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FISNO | C | 6 | 0 |
| TARIH | D | 8 | 0 |
| FIRMAKODU | C | 10 | 0 |
| ILACKODU | N | 6 | 0 |
| GMIKTAR | C | 7 | 0 |
| GMF | C | 7 | 0 |
| GFIYATI | C | 7 | 0 |
| GIMALFIYAT | C | 7 | 0 |
| KULLANIMDA | L | 1 | 0 |
| ILCNOT | C | 8 | 0 |
| MIAD | C | 4 | 0 |
| MALIYET | C | 7 | 0 |
| CEPNO | C | 2 | 0 |
| ALISKDV | N | 2 | 0 |
| ISKONTO1 | C | 2 | 0 |
| ISKONTO2 | C | 2 | 0 |
| VAADMF | N | 6 | 0 |
| VAADISK1 | N | 5 | 2 |
| VAADISK2 | N | 5 | 2 |
| CIROPRIM1 | N | 5 | 2 |
| CIROPRIM2 | N | 5 | 2 |
| CIROPRIM3 | N | 5 | 2 |
| CIROPRIM4 | N | 5 | 2 |
| VADE | N | 3 | 0 |
| SIPSIRA | C | 2 | 0 |
| SIPARISNO | C | 6 | 0 |
| OTVORAN | N | 5 | 2 |
| KURUMISK | C | 2 | 0 |
| FTRTARIH | D | 8 | 0 |
| FISSIRANO | C | 2 | 0 |
| IMALFIYATI | C | 7 | 0 |
| SONMALIYET | C | 7 | 0 |
| STATU | C | 2 | 0 |
| IRSALIYENO | C | 8 | 0 |
| SERINO | C | 12 | 0 |
| KAPATMIK | C | 7 | 0 |
| KUTUTIPI | C | 1 | 0 |
| L2DVAR | C | 1 | 0 |
| ISKONTO3 | C | 2 | 0 |
| ISKONTO4 | C | 2 | 0 |
| ISKONTO5 | C | 2 | 0 |
| ISKONTO6 | C | 2 | 0 |
| SUBE | C | 2 | 0 |

### DETAYS
- Dosya: F:\DEPO\DATA\01\DETAYS.DBF
- Kayit: 2490246; Alan: 30; Kayit uzunlugu: 128; Boyut MB: 303.99
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ECZANEKODU | C | 10 | 0 |
| ILACTIPI | C | 1 | 0 |
| ISKONTO | C | 1 | 0 |
| STATU | C | 2 | 0 |
| GRUPKODU | C | 1 | 0 |
| ILACKODU | N | 6 | 0 |
| FIYATI | C | 7 | 0 |
| MIKTARI | C | 7 | 0 |
| MALFAZLASI | C | 7 | 0 |
| TARIH | D | 8 | 0 |
| FATURANO | C | 7 | 0 |
| USERNO | C | 3 | 0 |
| REYON | C | 2 | 0 |
| CFIYATI | C | 7 | 0 |
| FIYATNO | C | 1 | 0 |
| NETFIYAT | C | 8 | 0 |
| MIAD | C | 4 | 0 |
| ITHAL | C | 1 | 0 |
| ALISKDV | N | 2 | 0 |
| CEPNO | C | 2 | 0 |
| DFIYATI | C | 8 | 0 |
| SATISKDV | N | 2 | 0 |
| ISKONTO1 | C | 2 | 0 |
| ISKONTO2 | C | 2 | 0 |
| KURUMIND | C | 7 | 0 |
| ECZACIKAR | C | 7 | 0 |
| KUTUTIPI | C | 1 | 0 |
| ADETKUTU | N | 7 | 0 |
| ISKONTO3 | C | 2 | 0 |
| ISKONTO4 | C | 2 | 0 |

### EARSIV
- Dosya: F:\DEPO\DATA\01\EARSIV.DBF
- Kayit: 1960; Alan: 27; Kayit uzunlugu: 470; Boyut MB: 0.88
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FATURANO | C | 7 | 0 |
| ECZANEKODU | C | 10 | 0 |
| TARIH | D | 8 | 0 |
| SAAT | C | 5 | 0 |
| FATURAID | C | 16 | 0 |
| FATURAUUID | C | 40 | 0 |
| BILDIRIM | C | 1 | 0 |
| BILTARIH | D | 8 | 0 |
| BILSAAT | C | 5 | 0 |
| DURUM | C | 1 | 0 |
| SONUC | C | 1 | 0 |
| EANAHTAR | C | 16 | 0 |
| IPTAL | C | 1 | 0 |
| EPOSDRM | C | 1 | 0 |
| CEPNO | C | 2 | 0 |
| IRSALIYEID | C | 16 | 0 |
| IRSTARIH | D | 8 | 0 |
| ADSOYAD | C | 61 | 0 |
| UNVAN | C | 24 | 0 |
| VKN | C | 15 | 0 |
| TELEFON | C | 10 | 0 |
| ADRES | C | 40 | 0 |
| ADRES2 | C | 40 | 0 |
| SEMT | C | 20 | 0 |
| SEHIR | C | 12 | 0 |
| EPOSTA | C | 100 | 0 |
| KAYNAK | C | 1 | 0 |

### EFATKULL
- Dosya: F:\DEPO\DATA\01\EFATKULL.DBF
- Kayit: 508817; Alan: 6; Kayit uzunlugu: 243; Boyut MB: 117.91
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| VKNTCKN | C | 11 | 0 |
| UNVANI | C | 120 | 0 |
| ETIKETI | C | 100 | 0 |
| TIPI | C | 1 | 0 |
| KAYITTRH | D | 8 | 0 |
| GBPK | C | 2 | 0 |

### EFATURA
- Dosya: F:\DEPO\DATA\01\EFATURA.DBF
- Kayit: 237635; Alan: 32; Kayit uzunlugu: 909; Boyut MB: 206
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FATURANO | C | 7 | 0 |
| ECZANEKODU | C | 10 | 0 |
| TARIH | D | 8 | 0 |
| SAAT | C | 5 | 0 |
| FATURAID | C | 16 | 0 |
| FATURAUUID | C | 40 | 0 |
| ENVUUID | C | 40 | 0 |
| BILDIRIM | C | 1 | 0 |
| BILTARIH | D | 8 | 0 |
| BILSAAT | C | 5 | 0 |
| DURUM | C | 1 | 0 |
| SONUC | C | 1 | 0 |
| HATAKODU | C | 5 | 0 |
| SENARYO | C | 1 | 0 |
| ARSIVLENDI | C | 1 | 0 |
| EANAHTAR | C | 16 | 0 |
| GTBREFNO | C | 23 | 0 |
| GTBTSCNO | C | 23 | 0 |
| IRSALIYEID | C | 16 | 0 |
| IRSTARIH | D | 8 | 0 |
| ADSOYAD | C | 61 | 0 |
| UNVAN | C | 24 | 0 |
| VKN | C | 15 | 0 |
| TELEFON | C | 10 | 0 |
| ADRES | C | 40 | 0 |
| ADRES2 | C | 40 | 0 |
| SEMT | C | 20 | 0 |
| SEHIR | C | 12 | 0 |
| EPOSTA | C | 100 | 0 |
| EFATURAPK | C | 100 | 0 |
| KAYNAK | C | 1 | 0 |
| HATAMESAJI | C | 250 | 0 |

### EFTRKULL
- Dosya: F:\DEPO\DATA\01\EFTRKULL.DBF
- Kayit: 1197583; Alan: 5; Kayit uzunlugu: 252; Boyut MB: 287.81
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| VKN | C | 11 | 0 |
| UNVAN | C | 120 | 0 |
| TIPI | C | 1 | 0 |
| PK | C | 100 | 0 |
| TARIH | C | 19 | 0 |

### FATURAG
- Dosya: F:\DEPO\DATA\01\FATURAG.DBF
- Kayit: 28199; Alan: 52; Kayit uzunlugu: 599; Boyut MB: 16.11
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FIRMAKODU | C | 10 | 0 |
| FISNO | C | 6 | 0 |
| TARIH | D | 8 | 0 |
| OZEL | C | 4 | 0 |
| IRSALIYENO | C | 16 | 0 |
| IRSTARIHI | D | 8 | 0 |
| FATURANO | C | 16 | 0 |
| FTRTARIHI | D | 8 | 0 |
| MUHKOD | C | 12 | 0 |
| GENELTUTAR | N | 15 | 0 |
| ISKONTO | N | 15 | 0 |
| GENELISK | N | 15 | 0 |
| KDV | N | 15 | 0 |
| ACIKLAMA | C | 30 | 0 |
| MFKDV | N | 15 | 0 |
| IRSTUTAR | N | 15 | 0 |
| ENTEGRE | N | 1 | 0 |
| CHENTEG | N | 1 | 0 |
| ODEME | D | 8 | 0 |
| ISKONTOTIP | C | 7 | 0 |
| ISKTABITUT | N | 15 | 0 |
| TEDIYETAR | D | 8 | 0 |
| TEDIYETUT | N | 15 | 0 |
| GIRENKULL | C | 8 | 0 |
| ONAYKULL | C | 8 | 0 |
| MAHSUP | N | 7 | 0 |
| OZELKOD | C | 2 | 0 |
| ONAYTAR | D | 8 | 0 |
| OZELISK1 | N | 15 | 0 |
| OZELISK2 | N | 15 | 0 |
| CEPNO | C | 2 | 0 |
| KDVK | N | 15 | 0 |
| MFKDVK | N | 15 | 0 |
| YUVARLAMA | N | 15 | 0 |
| OTV | N | 15 | 0 |
| KDV3 | N | 15 | 0 |
| MFKDV3 | N | 12 | 0 |
| KDV4 | N | 15 | 0 |
| MFKDV4 | N | 15 | 0 |
| KURUMIND | N | 15 | 0 |
| BEDELTUTAR | N | 15 | 0 |
| ITSDURUM | C | 1 | 0 |
| KDV5 | N | 15 | 0 |
| MFKDV5 | N | 15 | 0 |
| FIRMA | C | 3 | 0 |
| DOVIZ | C | 4 | 0 |
| DOVIZKUR | N | 10 | 6 |
| MATRAHN | N | 15 | 0 |
| MATRAHK | N | 15 | 0 |
| MATRAH3 | N | 15 | 0 |
| MATRAH4 | N | 15 | 0 |
| MATRAH5 | N | 15 | 0 |

### FATURAS
- Dosya: F:\DEPO\DATA\01\FATURAS.DBF
- Kayit: 644057; Alan: 70; Kayit uzunlugu: 578; Boyut MB: 355.02
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| NO1 | N | 7 | 0 |
| NO2 | N | 7 | 0 |
| TARIH | D | 8 | 0 |
| ECZANEKODU | C | 10 | 0 |
| ODEMESEKLI | C | 2 | 0 |
| FATURATIPI | C | 1 | 0 |
| FATURATURU | C | 1 | 0 |
| MFISKN | N | 15 | 0 |
| MFISKK | N | 15 | 0 |
| PESINISK | N | 15 | 0 |
| KDVN | N | 15 | 0 |
| KDVK | N | 15 | 0 |
| MFKDVN | N | 12 | 0 |
| MFKDVK | N | 12 | 0 |
| GENELTUTAR | N | 15 | 0 |
| ENTEGRE | N | 1 | 0 |
| TAKSITSAY | N | 2 | 0 |
| ARALIK | N | 3 | 0 |
| FIYATNO | C | 1 | 0 |
| IMFYT | C | 1 | 0 |
| MFKDV | C | 1 | 0 |
| MAHSUP | N | 7 | 0 |
| FIRMA | C | 3 | 0 |
| SAATI | C | 5 | 0 |
| PLASIYER | C | 3 | 0 |
| BOLGE | C | 2 | 0 |
| GIREN | C | 8 | 0 |
| SEVKSEKLI | C | 1 | 0 |
| CEPNO | C | 2 | 0 |
| CEPALDI | N | 1 | 0 |
| REYONSIRA | C | 1 | 0 |
| KDV3 | N | 15 | 0 |
| KDV4 | N | 15 | 0 |
| MFKDV3 | N | 12 | 0 |
| MFKDV4 | N | 12 | 0 |
| MFISK3 | N | 15 | 0 |
| MFISK4 | N | 15 | 0 |
| KURUMIND | C | 8 | 0 |
| ECZACIKAR | C | 8 | 0 |
| KAPAMATUT | C | 8 | 0 |
| TERMINALNO | C | 1 | 0 |
| TAKIPNO | C | 7 | 0 |
| KDV5 | N | 15 | 0 |
| MFKDV5 | N | 12 | 0 |
| MFISK5 | N | 15 | 0 |
| DOVIZ | C | 4 | 0 |
| DOVIZKUR | C | 8 | 0 |
| NAVLUN | N | 9 | 0 |
| TAKIPTUTAR | N | 15 | 0 |
| PTSSONUC | C | 1 | 0 |
| ITSDURUM | C | 1 | 0 |
| BELGENO | C | 16 | 0 |
| INDIRILDI | C | 1 | 0 |
| REFERANSNO | C | 16 | 0 |
| SIPREFNO | C | 16 | 0 |
| KONTROLCU | C | 2 | 0 |
| MATRAHN | N | 15 | 0 |
| MATRAHK | N | 15 | 0 |
| MATRAH3 | N | 15 | 0 |
| MATRAH4 | N | 15 | 0 |
| MATRAH5 | N | 15 | 0 |
| DOVIZTUTAR | N | 15 | 2 |
| PUANISK | N | 12 | 0 |
| SEVKKODU | C | 2 | 0 |
| SEVKLISTNO | C | 4 | 0 |
| TEVKORAN | N | 5 | 2 |
| DUZELTEN | C | 8 | 0 |
| TERMINAL | C | 8 | 0 |
| GRSTARIH | D | 8 | 0 |
| KAYNAK | C | 1 | 0 |

### fatusers
- Dosya: F:\DEPO\DATA\01\fatusers.dbf
- Kayit: 2207282; Alan: 4; Kayit uzunlugu: 213; Boyut MB: 448.37
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| VKN | C | 11 | 0 |
| TIPI | C | 1 | 0 |
| PK | C | 100 | 0 |
| UNVAN | C | 100 | 0 |

### FINANS
- Dosya: F:\DEPO\DATA\01\FINANS.DBF
- Kayit: 887186; Alan: 21; Kayit uzunlugu: 146; Boyut MB: 123.53
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| NO1 | N | 7 | 0 |
| BA | C | 1 | 0 |
| TARIH | D | 8 | 0 |
| HESAPKODU | C | 10 | 0 |
| ODEMEDURUM | C | 1 | 0 |
| ODEMESEKLI | C | 2 | 0 |
| VADE | D | 8 | 0 |
| TAHSILTAR | D | 8 | 0 |
| TAHISLTAR | D | 8 | 0 |
| TAHSILTUT | N | 15 | 0 |
| GENELTUTAR | N | 15 | 0 |
| BELGETURU | C | 2 | 0 |
| ACIKLAMA | C | 20 | 0 |
| BELGENO | C | 8 | 0 |
| FIRMA | C | 3 | 0 |
| KDVTUTAR | N | 15 | 0 |
| KBELGETURU | C | 1 | 0 |
| KBELGENO | C | 7 | 0 |
| OPSIYON | N | 3 | 0 |
| COGALTNO | N | 2 | 0 |
| TAHSILTIPI | C | 1 | 0 |

### FTRACK
- Dosya: F:\DEPO\DATA\01\FTRACK.DBF
- Kayit: 6325; Alan: 4; Kayit uzunlugu: 66; Boyut MB: 0.4
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| NO1 | N | 7 | 0 |
| FIRMAKODU | C | 10 | 0 |
| ACIKLAMA | C | 40 | 0 |
| TARIH | D | 8 | 0 |

### ILACAKT
- Dosya: F:\DEPO\DATA\01\ILACAKT.DBF
- Kayit: 80341; Alan: 17; Kayit uzunlugu: 117; Boyut MB: 8.96
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| TARIH | D | 8 | 0 |
| AMIKTAR | N | 7 | 0 |
| EFIYATI | N | 12 | 0 |
| EIMALFIYAT | N | 14 | 2 |
| YFIYATI | N | 12 | 0 |
| YIMALFIYAT | N | 14 | 2 |
| EMIAD | C | 4 | 0 |
| YMIAD | C | 4 | 0 |
| CEPNO | C | 2 | 0 |
| YAPAN | C | 8 | 0 |
| EKUTUTIPI | C | 1 | 0 |
| YKUTUTIPI | C | 1 | 0 |
| ISLEISK1 | N | 5 | 2 |
| ISLEISK2 | N | 5 | 2 |
| ISLEKRMISK | N | 6 | 0 |
| FISNO | C | 7 | 0 |

### ILACMIAD
- Dosya: F:\DEPO\DATA\01\ILACMIAD.DBF
- Kayit: 32850; Alan: 8; Kayit uzunlugu: 58; Boyut MB: 1.82
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| TARIH | D | 8 | 0 |
| CFIYATI | N | 12 | 0 |
| MIAD | C | 4 | 0 |
| GIREN | N | 8 | 0 |
| CIKAN | N | 8 | 0 |
| CEPNO | C | 2 | 0 |
| SERINO | C | 9 | 0 |

### ilacay
- Dosya: F:\DEPO\DATA\01\ilacay.DBF
- Kayit: 150843; Alan: 24; Kayit uzunlugu: 272; Boyut MB: 39.13
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | C | 6 | 0 |
| DONEM | C | 6 | 0 |
| GIREN | N | 9 | 0 |
| GMF | N | 8 | 0 |
| GTUTAR | N | 20 | 0 |
| SGIREN | N | 9 | 0 |
| SGMF | N | 8 | 0 |
| SGTUTAR | N | 20 | 0 |
| CIKAN | N | 9 | 0 |
| CMF | N | 8 | 0 |
| CTUTAR | N | 20 | 0 |
| SCIKAN | N | 9 | 0 |
| SCMF | N | 8 | 0 |
| SCTUTAR | N | 20 | 0 |
| ICIKAN | N | 9 | 0 |
| ICMF | N | 8 | 0 |
| ICTUTAR | N | 20 | 0 |
| IHLCIKAN | N | 9 | 0 |
| IHLCMF | N | 8 | 0 |
| IHLCTUTAR | N | 20 | 0 |
| IADEMIKTAR | N | 8 | 0 |
| IADEMF | N | 7 | 0 |
| IADETUTAR | N | 20 | 0 |
| CEPNO | C | 2 | 0 |

### KAMPANYA
- Dosya: F:\DEPO\DATA\01\KAMPANYA.DBF
- Kayit: 6780; Alan: 24; Kayit uzunlugu: 116; Boyut MB: 0.75
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| TARIH | D | 8 | 0 |
| KAMPANYA | C | 2 | 0 |
| KAMPANYA2 | C | 2 | 0 |
| KM | N | 4 | 0 |
| KMF | N | 4 | 0 |
| KCM | N | 7 | 0 |
| KCMF | N | 6 | 0 |
| KCTUTAR | N | 12 | 0 |
| BITISTAR | D | 8 | 0 |
| CFIYATI | N | 12 | 0 |
| KM2 | N | 4 | 0 |
| KMF2 | N | 4 | 0 |
| KM3 | N | 4 | 0 |
| KMF3 | N | 4 | 0 |
| SUBE | C | 2 | 0 |
| MFTIP | C | 1 | 0 |
| KM4 | N | 4 | 0 |
| KMF4 | N | 4 | 0 |
| KM5 | N | 4 | 0 |
| KMF5 | N | 4 | 0 |
| TARIH1 | N | 4 | 0 |
| TARIH2 | N | 4 | 0 |
| SIRALAMA | N | 1 | 0 |

### KKDETAY
- Dosya: F:\DEPO\DATA\01\KKDETAY.DBF
- Kayit: 24212217; Alan: 5; Kayit uzunlugu: 38; Boyut MB: 877.44
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| OZETRECNO | C | 4 | 0 |
| SIRANO | C | 20 | 0 |
| DURUM | C | 1 | 0 |
| PAKETSIRA | C | 6 | 0 |
| RECID | C | 6 | 0 |

### KKLC0001
- Dosya: F:\DEPO\DATA\01\KKLC0001.DBF
- Kayit: 27796694; Alan: 8; Kayit uzunlugu: 24; Boyut MB: 636.22
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| DETAYRECNO | C | 4 | 0 |
| HAREKETTUR | C | 1 | 0 |
| BELGENO | C | 7 | 0 |
| ITSSONUC | C | 1 | 0 |
| BELGETARIH | C | 2 | 0 |
| HESAPKODU | C | 1 | 0 |
| UYARIKODU | C | 1 | 0 |
| PAKETSIRA | C | 6 | 0 |

### KKLG0001
- Dosya: F:\DEPO\DATA\01\KKLG0001.DBF
- Kayit: 22456674; Alan: 8; Kayit uzunlugu: 24; Boyut MB: 513.99
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| DETAYRECNO | C | 4 | 0 |
| HAREKETTUR | C | 1 | 0 |
| BELGENO | C | 7 | 0 |
| ITSSONUC | C | 1 | 0 |
| BELGETARIH | C | 2 | 0 |
| HESAPKODU | C | 1 | 0 |
| UYARIKODU | C | 1 | 0 |
| PAKETSIRA | C | 6 | 0 |

### KKPAKET
- Dosya: F:\DEPO\DATA\01\KKPAKET.DBF
- Kayit: 767407; Alan: 7; Kayit uzunlugu: 63; Boyut MB: 46.11
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| PAKETSIRA | C | 6 | 0 |
| PAKETKODU | C | 40 | 0 |
| PAKETTURU | C | 1 | 0 |
| ICERIK | C | 1 | 0 |
| USTPAKET | C | 6 | 0 |
| AGIRLIK | N | 7 | 0 |
| PAKETISLEM | C | 1 | 0 |

### KURUMAKT
- Dosya: F:\DEPO\DATA\01\KURUMAKT.DBF
- Kayit: 11979; Alan: 10; Kayit uzunlugu: 78; Boyut MB: 0.89
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| TARIH | D | 8 | 0 |
| ISLEMIKTAR | N | 7 | 0 |
| ISLEFIYATI | N | 12 | 0 |
| ISLEIMFYT | N | 14 | 2 |
| ISLEISK1 | N | 5 | 2 |
| ISLEISK2 | N | 5 | 2 |
| EKURUMISK | N | 6 | 0 |
| YKURUMISK | N | 6 | 0 |
| YAPAN | C | 8 | 0 |

### MIADHRK
- Dosya: F:\DEPO\DATA\01\MIADHRK.DBF
- Kayit: 172278; Alan: 7; Kayit uzunlugu: 37; Boyut MB: 6.08
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| GC | C | 1 | 0 |
| BELGENO | C | 7 | 0 |
| SIRANO | C | 2 | 0 |
| ILACKODU | N | 6 | 0 |
| MIKTARI | C | 7 | 0 |
| MIAD | C | 4 | 0 |
| SERINO | C | 9 | 0 |

### PARTILER
- Dosya: F:\DEPO\DATA\01\PARTILER.DBF
- Kayit: 1035; Alan: 21; Kayit uzunlugu: 112; Boyut MB: 0.11
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FIRMAKODU | N | 4 | 0 |
| ILACKODU | N | 6 | 0 |
| CFIYATI | N | 12 | 0 |
| CIMALFIYAT | N | 12 | 2 |
| MIKTARI | N | 7 | 0 |
| SATISMIK | N | 7 | 0 |
| TARIH | D | 8 | 0 |
| REYON | C | 2 | 0 |
| RAF | C | 5 | 0 |
| INETSATIS | N | 6 | 0 |
| MIAD | C | 4 | 0 |
| CEPNO | C | 2 | 0 |
| PRTISKONTO | C | 1 | 0 |
| REZERV | C | 5 | 0 |
| YOLDA | C | 5 | 0 |
| PRTITHAL | C | 1 | 0 |
| KILITLI | C | 1 | 0 |
| SONGIRIS | D | 8 | 0 |
| KUTUTIPI | C | 1 | 0 |
| IADEREZERV | N | 7 | 0 |
| DEPOSEVK | N | 7 | 0 |

### PTSMAIN
- Dosya: F:\DEPO\DATA\01\PTSMAIN.DBF
- Kayit: 1308674; Alan: 17; Kayit uzunlugu: 191; Boyut MB: 238.38
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ID | N | 8 | 0 |
| SOURCEGLN | C | 13 | 0 |
| DESTINGLN | C | 13 | 0 |
| ACTIONTYPE | C | 1 | 0 |
| SHIPTOGLN | C | 13 | 0 |
| DOCNUMBER | C | 20 | 0 |
| DOCDATE | D | 8 | 0 |
| NOTE1 | C | 30 | 0 |
| CARRIERTYP | C | 1 | 0 |
| CARRIERLBL | C | 40 | 0 |
| USTPAKETID | N | 8 | 0 |
| ALTBRMSAY | N | 5 | 0 |
| ALTBRMTIPI | C | 1 | 0 |
| KNTEDILDI | L | 1 | 0 |
| TRANSFERID | N | 16 | 0 |
| FISNO | C | 6 | 0 |
| PAKETSIRA | C | 6 | 0 |

### PTSNUM1
- Dosya: F:\DEPO\DATA\01\PTSNUM1.DBF
- Kayit: 25347889; Alan: 8; Kayit uzunlugu: 83; Boyut MB: 2006.41
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| BARKODU | C | 13 | 0 |
| MIADI | D | 8 | 0 |
| SERINO | C | 20 | 0 |
| SIRANO | C | 20 | 0 |
| USTPAKETID | N | 8 | 0 |
| OKUNDU | C | 1 | 0 |
| FISNO | C | 6 | 0 |

### ptsNum2
- Dosya: F:\DEPO\DATA\01\ptsNum2.DBF
- Kayit: 11526629; Alan: 8; Kayit uzunlugu: 83; Boyut MB: 912.39
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| BARKODU | C | 13 | 0 |
| MIADI | D | 8 | 0 |
| SERINO | C | 20 | 0 |
| SIRANO | C | 20 | 0 |
| USTPAKETID | N | 8 | 0 |
| OKUNDU | C | 1 | 0 |
| FISNO | C | 6 | 0 |

### SATISLAR
- Dosya: F:\DEPO\DATA\01\SATISLAR.DBF
- Kayit: 424066; Alan: 21; Kayit uzunlugu: 135; Boyut MB: 54.6
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| TARIH | D | 8 | 0 |
| ELEMAN | C | 3 | 0 |
| BOLGE | C | 2 | 0 |
| KATEGORI | C | 2 | 0 |
| OZELBOLGE | C | 7 | 0 |
| TAHSILCI | C | 3 | 0 |
| GA | C | 8 | 0 |
| GB | C | 8 | 0 |
| GC | C | 8 | 0 |
| GD | C | 8 | 0 |
| GE | C | 8 | 0 |
| GF | C | 8 | 0 |
| KDVLITOP | C | 8 | 0 |
| CEPNO | C | 2 | 0 |
| GKADEME1 | C | 8 | 0 |
| GKADEME2 | C | 8 | 0 |
| GKADEME3 | C | 8 | 0 |
| GKADEME4 | C | 8 | 0 |
| GKADEME5 | C | 8 | 0 |
| GKADEME6 | C | 8 | 0 |
| USTBOLGE | C | 3 | 0 |

### SIPARIS
- Dosya: F:\DEPO\DATA\01\SIPARIS.DBF
- Kayit: 548; Alan: 26; Kayit uzunlugu: 317; Boyut MB: 0.17
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FIRMAKODU | C | 10 | 0 |
| FISNO | C | 6 | 0 |
| TARIH | D | 8 | 0 |
| OZEL | C | 6 | 0 |
| GFISNO | C | 1 | 0 |
| GENELTUTAR | N | 15 | 0 |
| ISKONTO | N | 15 | 0 |
| GENELISK | N | 15 | 0 |
| KDV | N | 15 | 0 |
| ACIKLAMA | C | 30 | 0 |
| MFKDV | N | 15 | 0 |
| IRSTUTAR | N | 15 | 0 |
| ODEME | D | 8 | 0 |
| URETICI | N | 3 | 0 |
| TESLIMAT | D | 8 | 0 |
| KAPALITTR | N | 15 | 0 |
| TUMTUTAR | N | 15 | 0 |
| TAKIPNO | C | 8 | 0 |
| KDVK | N | 15 | 0 |
| MFKDVK | N | 15 | 0 |
| OZELKOD | C | 4 | 0 |
| GIRENKULL | C | 10 | 0 |
| AKTARZAMAN | C | 14 | 0 |
| ALCHSPKODU | C | 10 | 0 |
| KIKNO | C | 20 | 0 |
| IHALENO | C | 20 | 0 |

### SIPHAR
- Dosya: F:\DEPO\DATA\01\SIPHAR.DBF
- Kayit: 3577; Alan: 23; Kayit uzunlugu: 135; Boyut MB: 0.46
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FISNO | C | 6 | 0 |
| TARIH | D | 8 | 0 |
| FIRMAKODU | C | 10 | 0 |
| ILACKODU | N | 6 | 0 |
| SMIKTAR | N | 6 | 0 |
| SMF | N | 5 | 0 |
| SFIYATI | N | 12 | 0 |
| SIMALFIYAT | N | 12 | 0 |
| GFISNO | C | 1 | 0 |
| GMIKTAR | N | 6 | 0 |
| GMF | N | 5 | 0 |
| KMIKTAR | N | 6 | 0 |
| GUN | N | 3 | 0 |
| ILCNOT | C | 8 | 0 |
| KAPATAR | D | 8 | 0 |
| SIPSIRA | C | 2 | 0 |
| ISKONTO1 | C | 2 | 0 |
| ISKONTO2 | C | 2 | 0 |
| CEPNO | C | 2 | 0 |
| ISLEMZAMAN | N | 8 | 0 |
| ISKONTO3 | C | 2 | 0 |
| ISKONTO4 | C | 2 | 0 |
| GIMALFIYAT | N | 12 | 0 |

### stkzaman
- Dosya: F:\DEPO\DATA\01\stkzaman.DBF
- Kayit: 17425; Alan: 9; Kayit uzunlugu: 143; Boyut MB: 2.38
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| STK | C | 17 | 0 |
| WEB | C | 17 | 0 |
| ES | C | 17 | 0 |
| OP1 | C | 17 | 0 |
| OP2 | C | 17 | 0 |
| FZ1 | C | 17 | 0 |
| FZ2 | C | 17 | 0 |
| E1 | C | 17 | 0 |

### TEVZILER
- Dosya: F:\DEPO\DATA\01\TEVZILER.DBF
- Kayit: 15814; Alan: 4; Kayit uzunlugu: 30; Boyut MB: 0.45
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| KOD | C | 10 | 0 |
| ILC | C | 6 | 0 |
| MIK | N | 7 | 0 |
| THDTMK | N | 6 | 0 |

### TSSIPDTY
- Dosya: F:\DEPO\DATA\01\TSSIPDTY.DBF
- Kayit: 87930; Alan: 16; Kayit uzunlugu: 130; Boyut MB: 10.9
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ORDERID | N | 13 | 0 |
| SIPTRH | C | 22 | 0 |
| ORDERTARIH | D | 8 | 0 |
| TSKODU | N | 6 | 0 |
| TSSTATU | C | 2 | 0 |
| TSFIYAT | N | 12 | 2 |
| TSCFIYATI | N | 12 | 2 |
| TSISKE | N | 5 | 2 |
| TSISKK | N | 5 | 2 |
| TSISK1 | N | 5 | 2 |
| TSISK2 | N | 5 | 2 |
| TSMIKTAR | N | 7 | 0 |
| TSMF | N | 7 | 0 |
| TSMIAD | C | 4 | 0 |
| REDTIPI | C | 15 | 0 |
| TSKUTUTIPI | C | 1 | 0 |

### tssipana
- Dosya: F:\DEPO\DATA\01\tssipana.DBF
- Kayit: 24635; Alan: 24; Kayit uzunlugu: 652; Boyut MB: 15.32
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ORDERID | N | 13 | 0 |
| ORDERKLM | N | 5 | 0 |
| ORDERTTR | N | 12 | 2 |
| HESAPKODU | C | 10 | 0 |
| KAYNAK | N | 1 | 0 |
| ODEMEDRM | N | 1 | 0 |
| SIPTRH | C | 22 | 0 |
| ACIKLAMA1 | C | 50 | 0 |
| ACIKLAMA2 | C | 50 | 0 |
| ACIKLAMA3 | C | 50 | 0 |
| ORDERTARIH | D | 8 | 0 |
| ORDERSAAT | C | 5 | 0 |
| PUANONAY | C | 1 | 0 |
| AKTARILDI | N | 1 | 0 |
| ADSOYAD | C | 61 | 0 |
| UNVAN | C | 24 | 0 |
| VKN | C | 15 | 0 |
| TELEFON | C | 10 | 0 |
| ADRES | C | 40 | 0 |
| ADRES2 | C | 40 | 0 |
| SEMT | C | 20 | 0 |
| SEHIR | C | 12 | 0 |
| EPOSTA | C | 100 | 0 |
| EFATURAPK | C | 100 | 0 |

### URETICI
- Dosya: F:\DEPO\DATA\01\URETICI.DBF
- Kayit: 636; Alan: 28; Kayit uzunlugu: 574; Boyut MB: 0.35
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| FIRMAKODU | N | 4 | 0 |
| FIRMAADI | C | 15 | 0 |
| SORUMLU1 | C | 28 | 0 |
| SORUMLU2 | C | 28 | 0 |
| TELEFON1 | C | 28 | 0 |
| TELEFON2 | C | 28 | 0 |
| FAX1 | C | 28 | 0 |
| FAX2 | C | 28 | 0 |
| NOT1 | C | 59 | 0 |
| NOT2 | C | 59 | 0 |
| NOT3 | C | 59 | 0 |
| NOT4 | C | 59 | 0 |
| FRMADI | C | 30 | 0 |
| HESAPKODU | C | 10 | 0 |
| PRIMA | N | 5 | 2 |
| PRIMB | N | 5 | 2 |
| PRIMC | N | 5 | 2 |
| PRIMD | N | 5 | 2 |
| ACIKADI | C | 30 | 0 |
| RAPORADI | C | 15 | 0 |
| PRIME | N | 5 | 2 |
| PRIMF | N | 5 | 2 |
| STOKORANI | N | 4 | 2 |
| IZINZAMANI | C | 14 | 0 |
| SECILDI | L | 1 | 0 |
| B2BKODU | C | 3 | 0 |
| CPORANI | N | 5 | 2 |
| REVIZEDATE | D | 8 | 0 |

### URUNLER
- Dosya: F:\DEPO\DATA\01\URUNLER.DBF
- Kayit: 17425; Alan: 94; Kayit uzunlugu: 732; Boyut MB: 12.17
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ILACKODU | N | 6 | 0 |
| FIRMAKODU | N | 4 | 0 |
| ILACADI | C | 50 | 0 |
| ILACTIPI | C | 1 | 0 |
| ISKONTO | C | 1 | 0 |
| GRUPKODU | C | 1 | 0 |
| TAHDIT | L | 1 | 0 |
| THDTMK | N | 4 | 0 |
| IMALFIYATI | N | 14 | 2 |
| FIYATI | N | 12 | 0 |
| MINIMUM | N | 5 | 0 |
| MIKTARI | N | 7 | 0 |
| ISKORANI | N | 2 | 0 |
| BARKODU | C | 13 | 0 |
| RAF | C | 5 | 0 |
| AGIRLIK | N | 7 | 1 |
| MALKODU | C | 16 | 0 |
| SATISTUR1 | C | 29 | 0 |
| PAKETMKT | N | 5 | 0 |
| OZELKOD | C | 6 | 0 |
| FIYATI2 | N | 12 | 0 |
| FYTTARIH | D | 8 | 0 |
| KTARIH | D | 8 | 0 |
| FIYATI3 | N | 12 | 0 |
| FIYATI4 | N | 12 | 0 |
| JENERIKNO | C | 7 | 0 |
| CIROPR1 | N | 7 | 4 |
| CIROPR2 | N | 7 | 4 |
| CIROPR3 | N | 7 | 4 |
| SATISTUR2 | C | 1 | 0 |
| SATISTUR3 | C | 1 | 0 |
| SATISTUR4 | C | 1 | 0 |
| SATISTUR5 | C | 1 | 0 |
| ITHAL | C | 1 | 0 |
| MIADTAKIP | C | 1 | 0 |
| VADEGUN | N | 3 | 0 |
| GTARIH | D | 8 | 0 |
| INTMIKTAR | N | 6 | 0 |
| ALISISK1 | N | 5 | 2 |
| ALISISK2 | N | 5 | 2 |
| REYON01 | C | 2 | 0 |
| REYON02 | C | 2 | 0 |
| REYON03 | C | 2 | 0 |
| REYON04 | C | 2 | 0 |
| REYON05 | C | 2 | 0 |
| OZELKOD2 | C | 6 | 0 |
| KDVTIPI | C | 1 | 0 |
| MAXIMUM | N | 7 | 0 |
| KURUMISK | N | 6 | 0 |
| BARKODALMA | C | 1 | 0 |
| ARTIISK1 | C | 2 | 0 |
| ARTIISK2 | C | 2 | 0 |
| REYON06 | C | 2 | 0 |
| KUTUTIPI | C | 1 | 0 |
| FIYATI6 | N | 12 | 0 |
| FIYATI7 | N | 12 | 0 |
| FIYATI8 | N | 12 | 0 |
| FIYATI9 | N | 12 | 0 |
| SIPKATMKTR | N | 4 | 0 |
| ACIKADI1 | C | 90 | 0 |
| REYON07 | C | 2 | 0 |
| ARTIISK3 | C | 2 | 0 |
| ARTIISK4 | C | 2 | 0 |
| ALISISK3 | N | 5 | 2 |
| ALISISK4 | N | 5 | 2 |
| MARKASI | C | 20 | 0 |
| KOLIMIKTAR | N | 7 | 0 |
| KARLILIK | N | 5 | 0 |
| MAXSATMIK | N | 7 | 0 |
| MESAJTARIH | D | 8 | 0 |
| BIRIM | C | 4 | 0 |
| URUNGRUP | C | 6 | 0 |
| KSTTARIH | D | 8 | 0 |
| GTIPNO | C | 20 | 0 |
| EKPUAN | N | 5 | 2 |
| SONMIAD | C | 4 | 0 |
| CIKISHIZI | N | 6 | 0 |
| KATEGORI | N | 7 | 0 |
| SAAT1 | C | 5 | 0 |
| SAAT2 | C | 5 | 0 |
| OZELKOD3 | C | 6 | 0 |
| OZELKOD4 | C | 6 | 0 |
| GORSEL | N | 1 | 0 |
| FIYATIA | N | 12 | 0 |
| FIYATIB | N | 12 | 0 |
| FIYATIC | N | 12 | 0 |
| FIYATID | N | 12 | 0 |
| FIYATIE | N | 12 | 0 |
| GIREBILEN | C | 8 | 0 |
| ILANSIRASI | N | 6 | 0 |
| ILANSAYISI | N | 6 | 0 |
| MINILANFYT | N | 9 | 2 |
| PALETMIK | N | 6 | 0 |
| REVIZEDATE | D | 8 | 0 |

### UTSDTY00
- Dosya: F:\DEPO\DATA\01\UTSDTY00.DBF
- Kayit: 10460; Alan: 12; Kayit uzunlugu: 169; Boyut MB: 1.69
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| ID | N | 8 | 0 |
| BARKODU | C | 13 | 0 |
| LOTNO | C | 20 | 0 |
| SERISIRA | C | 20 | 0 |
| ESSIZKOD | C | 70 | 0 |
| MIAD | C | 6 | 0 |
| URETIM | C | 6 | 0 |
| ADET | N | 7 | 0 |
| ILACKODU | N | 6 | 0 |
| USTPAKETID | N | 10 | 0 |
| TIPI | C | 1 | 0 |
| DURUM | C | 1 | 0 |

### UTSLOG00
- Dosya: F:\DEPO\DATA\01\UTSLOG00.DBF
- Kayit: 70504; Alan: 10; Kayit uzunlugu: 99; Boyut MB: 6.66
| Alan | Tip | Uzunluk | Ondalik |
|---|---:|---:|---:|
| DETAYID | N | 10 | 0 |
| BELGENO | C | 7 | 0 |
| HAREKETTUR | C | 1 | 0 |
| TARIH | D | 8 | 0 |
| FIRMAUTSNO | C | 13 | 0 |
| HESAPKODU | C | 10 | 0 |
| ADET | N | 7 | 0 |
| ISLEM | C | 5 | 0 |
| SONUC | C | 1 | 0 |
| BILDIRIMID | C | 36 | 0 |



## 20. C:\OPA Guncel Ana Uygulama ve Menu/Sekme Analizi

Bu bolum kullanici duzeltmesi sonrasinda eklendi. Sistem artik eski `BASKENT.EXE` degil, `C:\OPA\OPA.EXE` uzerinden calisiyor. Analiz sadece okuma ile yapildi; EXE, DBF veya program dosyalarinda degisiklik yapilmadi.

### 20.1 C:\OPA Calisma Paketi

| Dosya | Boyut | Son Degisim | Surum/Urun | Rol |
|---|---:|---|---|---|
| `C:\OPA\OPA.EXE` | 8136001 | 2026-04-28 16:00:23 | Opa Depo Otomasyonu 3.2.9140 | Guncel ana uygulama |
| `C:\OPA\Opa.vbs` | 2192 | 2025-05-26 20:42:33 | - | `F:\BATCH\OPA.EXE` ile `C:\OPA\OPA.EXE` surum kontrolu/guncelleme ve baslatma |
| `C:\OPA\config.fpw` | 183 | 2025-05-26 20:44:06 | CODEPAGE 1254 | FoxPro runtime ayarlari |
| `C:\OPA\VFP9R.DLL` | 4710400 | 2023-03-05 14:45:13 | VFP 9.0.00.2412 | Visual FoxPro runtime |
| `C:\OPA\VFP9RENU.DLL` | 1429504 | 2023-03-05 14:45:14 | VFP 9.0.00.2412 | VFP resource DLL |
| `C:\OPA\MSVCR71.DLL` | 348160 | 2023-03-05 14:45:11 | 7.10.3052.4 | C runtime |
| `C:\OPA\ChilkatAx-9.5.0-win32.dll` | 10399744 | 2021-02-09 20:42:58 | Chilkat 9.5.0.86 | Entegrasyon/HTTP/SOAP/mail/sertifika kutuphanesi adayi |

### 20.2 Opa.vbs Baslatma Akisi

1. `exeName = "OPA.EXE"`, `RunningPath = "C:\OPA\"`, `UpdatePath = "F:\BATCH\"`.
2. `C:\OPA` yoksa olusturmak uzere tasarlanmis.
3. `C:\OPA\OPA.EXE` ve `F:\BATCH\OPA.EXE` surumleri karsilastiriliyor.
4. Surum farki varsa kullaniciya guncelleme mesaji gosterilip `F:\BATCH\OPA.EXE` yerel `C:\OPA` altina kopyalaniyor.
5. VFP runtime ve Chilkat DLL dosyalari eksikse `F:\BATCH` kaynagindan `C:\OPA` altina kopyalaniyor.
6. Uygulama `C:\OPA\OPA.EXE` olarak baslatiliyor. Calisan surec listesinde `opa.exe` aktif goruldu.

### 20.3 Ana Sekmeler

| Sayfa | Gozlenen Caption Izleri | Cikarim |
|---|---|---|
| Page 1 | Stok, Stok Islemleri | Stok, urun, alis, satis, kampanya ve depo operasyonlari |
| Page 2 | Cari, Cari Hesap, Cari Hesap Takibi | Cari kart, cari hareket, toplu cari islemler |
| Page 3 | Cek, Cek Senet, Cek Senet Kredi Karti | Cek/senet/kredi karti takip |
| Page 4 | Finans, Finans Takibi, Finans ve Vade Takibi | Tahsilat, vade, musteri/satici finans raporlari |
| Page 5 | Muhasebe, Resmi Muhasebe | Fis, defter, muhasebe raporlari |
| Page 6 | Ent, Entegrasyon, Entegrasyon Islemleri | Muhasebe/fatura/cari/satis entegrasyonlari |
| Page 7 | Yetki, Yetkilendirme | Kullanici ve yetki islemleri |
| Page 8 | Servis, Servis Programlari | Bilgi ayirma/olusturma, disardan bilgi, yildevir, genel ayarlar |

Ham caption izleri:
- `Page 1: Stok [L]`
- `Page 1: Stok İşlemleri [L]`
- `Page 2: Cari [P]`
- `Page 2: Cari Hesap [P]`
- `Page 2: Cari Hesap Takibi [P]`
- `Page 3: Çek [Ç]`
- `Page 3: Çek Senet [Ç]`
- `Page 3: Çek Senet Kredi Kartı [Ç]`
- `Page 4: Finans [N]`
- `Page 4: Finans Takibi [N]`
- `Page 4: Finans ve Vade Takibi [N]`
- `Page 5: Muhasebe [M]`
- `Page 5: Resmi Muhasebe [M]`
- `Page 6: Ent [O]`
- `Page 6: Entegrasyon [O]`
- `Page 6: Entegrasyon İşlemleri [O]`
- `Page 7: Yetki`
- `Page 7: Yetkilendirme`
- `Page 8: Servis`
- `Page 8: Servis Programları`

### 20.4 Ana Sekme Alt Gruplari / Container Envanteri

| Sekme | Alt Grup / Container |
|---|---|
| Page 1 | `aktarimservisleri` |
| Page 1 | `alimislemleri` |
| Page 1 | `anlasmatakibi` |
| Page 1 | `aykirisatislar` |
| Page 1 | `cikanurun1` |
| Page 1 | `cikanurun2` |
| Page 1 | `envanter` |
| Page 1 | `faturabordrolari` |
| Page 1 | `faturaislemleri` |
| Page 1 | `faturatoplamlari` |
| Page 1 | `firmabilgileri` |
| Page 1 | `firmasiparisleri` |
| Page 1 | `gunlukraporlar` |
| Page 1 | `iptaledilmisfaturalar` |
| Page 1 | `kampanyaislemleri` |
| Page 1 | `kampanyasatis` |
| Page 1 | `kampListMenu` |
| Page 1 | `mustericirotoplamlari` |
| Page 1 | `musteritakipraporlari` |
| Page 1 | `primhesaplamalari` |
| Page 1 | `reyontakibi` |
| Page 1 | `satisciro` |
| Page 1 | `satisraporlari` |
| Page 1 | `siparisislemleri` |
| Page 1 | `sistem` |
| Page 1 | `stok` |
| Page 1 | `stokraporlari` |
| Page 1 | `suberaporlari` |
| Page 1 | `tevziurun` |
| Page 1 | `urunbilgileri` |
| Page 1 | `urungirisleri` |
| Page 1 | `urunlisteleri` |
| Page 1 | `urunraporlari` |
| Page 1 | `urunsatisraporlari` |
| Page 2 | `cari` |
| Page 2 | `carilisteler` |
| Page 2 | `exceldeniceri` |
| Page 2 | `indirilecekKdv` |
| Page 2 | `musteridegerlendirme` |
| Page 2 | `ozelbilgiler` |
| Page 2 | `raporlar` |
| Page 2 | `sistem` |
| Page 2 | `topluIslemler` |
| Page 3 | `ceksenet` |
| Page 3 | `geneldurum` |
| Page 3 | `raporlar` |
| Page 3 | `secimliliste` |
| Page 3 | `sistem` |
| Page 4 | `donemselrapor` |
| Page 4 | `genelraporlar` |
| Page 4 | `musteriislemleri` |
| Page 4 | `musteriraporlari` |
| Page 4 | `musteritakipraporlari` |
| Page 4 | `saticiislemleri` |
| Page 4 | `saticiraporlari` |
| Page 4 | `vadedagilimi` |
| Page 4 | `vadetakibi` |
| Page 5 | `defterler` |
| Page 5 | `fisler` |
| Page 5 | `muhasebe` |
| Page 5 | `raporlar` |
| Page 5 | `sistem` |
| Page 6 | `entegrasyon` |
| Page 6 | `Entkod` |
| Page 6 | `tekrarentegre` |
| Page 7 | `yetki` |
| Page 8 | `bilgiayirma` |
| Page 8 | `bilgiolusturma` |
| Page 8 | `disardanbilgi` |
| Page 8 | `genelayarlar` |
| Page 8 | `servis` |
| Page 8 | `toplubilgi` |
| Page 8 | `yildevir` |

### 20.5 Buton Grubu Yogunlugu

Asagidaki liste, EXE icinde `stkButtons.Add(this.<grup>.command<n>)` izlerinden uretilmistir. Komut sayisi, ilgili alt menunun kac buton/aksiyon barindirdigini gosterir; buton metinleri kaynak kod olmadan her zaman birebir okunamayabilir.

| Grup | Komut Sayisi | Komut Numaralari |
|---|---:|---|
| `aktarimservisleri` | 7 | 1,2,3,4,5,6,7 |
| `alimislemleri` | 7 | 1,2,3,4,5,6,7 |
| `anlasmatakibi` | 11 | 1,2,3,4,5,6,7,8,9,10,11 |
| `aykirisatislar` | 3 | 1,2,3 |
| `cikanurun1` | 3 | 1,2,3 |
| `cikanurun2` | 3 | 1,2,3 |
| `envanter` | 4 | 1,2,3,4 |
| `faturabordrolari` | 4 | 1,2,3,4 |
| `faturaislemleri` | 6 | 1,2,3,4,5,6 |
| `faturatoplamlari` | 3 | 1,2,3 |
| `firmabilgileri` | 9 | 1,2,3,4,5,6,7,8,9 |
| `firmasiparisleri` | 6 | 1,2,3,4,5,6 |
| `gunlukraporlar` | 8 | 1,2,3,4,5,6,7,8 |
| `iptaledilmisfaturalar` | 3 | 1,2,3 |
| `kampanyaislemleri` | 8 | 1,2,3,4,5,6,7,8 |
| `kampanyasatis` | 7 | 1,2,3,4,5,6,7 |
| `kampListMenu` | 2 | 1,2 |
| `konsolfirma` | 4 | 1,2,3,4 |
| `konsolsatis` | 2 | 1,2 |
| `konsolstok` | 2 | 1,2 |
| `konsolurun` | 3 | 1,2,3 |
| `mustericirotoplamlari` | 3 | 1,2,3 |
| `musteritakipraporlari` | 6 | 1,2,3,4,5,6 |
| `primhesaplamalari` | 9 | 1,2,3,4,5,6,7,8,9 |
| `reyontakibi` | 10 | 1,2,3,4,5,6,7,8,9,10 |
| `satisCiro` | 6 | 1,2,3,4,5,6 |
| `satisraporlari` | 12 | 1,2,3,4,5,6,7,8,9,10,11,12 |
| `siparisislemleri` | 9 | 1,2,3,4,5,6,7,8,9 |
| `sistem` | 7 | 1,2,3,4,5,6,7 |
| `stok` | 13 | 1,2,3,4,5,6,7,8,9,10,11,12,13 |
| `stokbakim` | 6 | 1,2,3,4,5,6 |
| `stokislemleri` | 6 | 1,2,3,4,5,6 |
| `stokraporlari` | 5 | 1,2,3,4,5 |
| `suberaporlari` | 4 | 1,2,3,4 |
| `tevziurun` | 2 | 1,2 |
| `urunbilgileri` | 10 | 1,2,3,4,5,6,7,8,9,10 |
| `urungirisleri` | 7 | 1,2,3,4,5,6,7 |
| `urunlisteleri` | 3 | 1,2,3 |
| `urunraporlari` | 9 | 1,2,3,4,5,6,7,8,9 |
| `urunsatisraporlari` | 5 | 1,2,3,4,5 |

### 20.6 Gorunen Form / View Hedefleri

| Hedef | Cikarim |
|---|---|
| `DO FORM formayarlargenel` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formayarlarTicari` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formbelgeno` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formbolgemud` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formCommandConsole` | Komut/konsol veya teknik servis ekrani |
| `DO FORM formfarkvade` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formFilter` | Filtreleme dialogu |
| `DO FORM formkatalog` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formlogin` | Kullanici girisi |
| `DO FORM formmessage` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formortalamavade` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formozelbolge` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formreyonsifre` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formsonnum` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formtahsildar` | Dogru islev icin ekran akisiyle dogrulanmali |
| `DO FORM formview` | Generic formview altyapisiyla acilan ekran/view |
| `DO FORM formVknAra` | Dogru islev icin ekran akisiyle dogrulanmali |

### 20.7 EXE Icinde Gorunen Menu/Rapor/Islem Basliklari

Bu liste `OPA.EXE` icindeki okunabilir metinlerden temizlenerek uretilmistir. Kaynak kod olmadigi icin tam siralama/ust-alt menu hiyerarsisi garanti edilmez; ancak programin kapsadigi ekran ve rapor yuzeyini kuvvetli sekilde gosterir.

| Baslik | Ilgili Modul |
|---|---|
| Alis Fatura Onay | Satis/Fatura/Siparis |
| Alis Faturalari | Satis/Fatura/Siparis |
| Alis Faturasi Entegrasyonu | Satis/Fatura/Siparis |
| Alis Faturasi Karsilastirma Raporu | Satis/Fatura/Siparis |
| Alt Depo Satislari | Stok/Depo/Takip |
| Alt Depo Sevkleri | Stok/Depo/Takip |
| Alt Depo Tanimlari | Stok/Depo/Takip |
| Aylik Urun Giris Cikislari | Stok/Depo/Takip |
| Bekleyen Siparisler | Satis/Fatura/Siparis |
| Bordro Dokumu | Raporlama |
| Bugunku Deger Raporu | Raporlama |
| Calisilmayan Paydas Listesi | Raporlama |
| Cari Fis Girisi | Cari/Finans |
| Cari Hareket Entegrasyonu | Cari/Finans |
| Cari Hesap Bilgi Kontrolu | Cari/Finans |
| Cari Hesap Fis Girisi | Cari/Finans |
| Cari Hesap Kontrolu | Cari/Finans |
| Cari Kartlar | Cari/Finans |
| Cari Muhasebe Kontrolu | Cari/Finans |
| Cari ve Finans Karsilastir | Cari/Finans |
| Cek Senet Listeleri | Cari/Finans |
| Detayli Risk Listesi | Cari/Finans |
| Devir Hizina Gore Stok Listesi | Stok/Depo/Takip |
| Dogrudan Siparis | Satis/Fatura/Siparis |
| Donemsel Ciro Raporu | Raporlama |
| Donemsel Rapor | Raporlama |
| Ecza1 Siparis Takibi | Satis/Fatura/Siparis |
| eTicaret Kullanmayanlar | Entegrasyon/Pazaryeri |
| Excel Kampanya Yukleme | Kampanya/Fiyat/Muhasebe |
| Farmaborsa Siparis Takibi | Satis/Fatura/Siparis |
| Farmazon Rx Siparis Takibi | Satis/Fatura/Siparis |
| Farmazon Siparis Takibi | Satis/Fatura/Siparis |
| Fatura Bordrosu | Satis/Fatura/Siparis |
| Fatura Takibi | Satis/Fatura/Siparis |
| Fatura Toplamlari | Satis/Fatura/Siparis |
| Faturadan Giris Kontrolu | Satis/Fatura/Siparis |
| Firma Alis Satis Raporu | Satis/Fatura/Siparis |
| Firma Satis Raporu | Satis/Fatura/Siparis |
| Firma Siparisleri | Satis/Fatura/Siparis |
| Firma Takip Raporu | Raporlama |
| Firma Urun Raporu | Stok/Depo/Takip |
| Fis Listeleme | Raporlama |
| Fis Listesi | Raporlama |
| Fiyat Dosyasi Yukleme | Kampanya/Fiyat/Muhasebe |
| Fiyat Hareket Raporu - Tarih | Kampanya/Fiyat/Muhasebe |
| Fiyat Hareket Raporu - Urun | Stok/Depo/Takip |
| Fiyat Kontrolu | Kampanya/Fiyat/Muhasebe |
| Gecerli Fiyattan Dusuk Satislar | Satis/Fatura/Siparis |
| Genel Durum - Hareket | Diger/Servis |
| Genel Raporlar - Musteri Ozet Raporu | Raporlama |
| Genel Raporlar - Musteri Risk Listesi | Cari/Finans |
| Genel Raporlar - Plasiyer Risk Listesi | Cari/Finans |
| Genel Raporlar - Vade Dagilimi | Cari/Finans |
| Gerceklesen Tahsilatlar | Cari/Finans |
| Giris Fiyati Karsilastirma | Kampanya/Fiyat/Muhasebe |
| Gunluk Satis Raporu | Satis/Fatura/Siparis |
| Hareket Dokumu | Raporlama |
| Hareket Listesi | Raporlama |
| Hatali Stok Raporu | Stok/Depo/Takip |
| Hazirlanan Siparis Tutari | Satis/Fatura/Siparis |
| Indirilecek KDV Listesi | Cari/Finans |
| ITS Stok - Fiili Stok Karsilastir | Stok/Depo/Takip |
| Kampanya Duzenle 1 | Kampanya/Fiyat/Muhasebe |
| Kampanya Duzenle 1 Statu | Kampanya/Fiyat/Muhasebe |
| Kampanya Duzenle 2 | Kampanya/Fiyat/Muhasebe |
| Kampanya Duzenle 2 Statu | Kampanya/Fiyat/Muhasebe |
| Kampanya Duzenleme 4 | Kampanya/Fiyat/Muhasebe |
| Kampanya Karlilik Kontrolu | Kampanya/Fiyat/Muhasebe |
| Kampanya Kopyala | Kampanya/Fiyat/Muhasebe |
| Kampanyada Yer Almayan Urunler | Stok/Depo/Takip |
| Kapama Raporu | Raporlama |
| Karlilik Raporu | Raporlama |
| KDV Durum Raporu | Kampanya/Fiyat/Muhasebe |
| Kullanici Tanimli Liste - Detay | Raporlama |
| Kullanici Tanimli Liste - Evrak | Raporlama |
| Kural Disi Hediye Edilen Urunler | Stok/Depo/Takip |
| Kurum Iskontosu Hareket Raporu - Tarih | Raporlama |
| Kurum Iskontosu Hareket Raporu - Urun | Stok/Depo/Takip |
| Maliyet Raporu - FIFO | Kampanya/Fiyat/Muhasebe |
| Maliyet Raporu - Ortalama | Kampanya/Fiyat/Muhasebe |
| Menu ile Yarim Birakilan Siparisler | Satis/Fatura/Siparis |
| Miktar Kontrolu | Diger/Servis |
| Musteri Odeme Tercihleri | Cari/Finans |
| Musteri Raporlari - Tahsilat Listesi | Cari/Finans |
| Musteri Takip Raporlari - Donem Icinde Mal Almayanlar | Raporlama |
| Novadan Siparis Takibi | Satis/Fatura/Siparis |
| Odeme Tablosu - Detay | Cari/Finans |
| Oransal Giris/Cikis/Stok Raporu | Stok/Depo/Takip |
| Ozel Fiyat Olusturma | Kampanya/Fiyat/Muhasebe |
| Parti Listesi | Stok/Depo/Takip |
| Plasiyer Tahsilat Ekstresi | Cari/Finans |
| Puan Kontrol Raporu | Raporlama |
| Receteli Ilac Raporu - Musteri | Raporlama |
| Receteli Ilac Raporu - Urun | Stok/Depo/Takip |
| Referans No ile Excel Tahsilat ve Fatura Kapatma | Satis/Fatura/Siparis |
| Reyon Sifreleri | Stok/Depo/Takip |
| Reyon Urun Listesi | Stok/Depo/Takip |
| Satici Raporlari - Alis Fatura Bordrosu | Satis/Fatura/Siparis |
| Satici Raporlari - Giris Gecikme Raporu | Raporlama |
| Satici Raporlari - Odeme Raporu | Cari/Finans |
| Satilamayan Tek Urun Raporu | Stok/Depo/Takip |
| Satis Fatura Iptali | Satis/Fatura/Siparis |
| Satis Fatura Toplamlari | Satis/Fatura/Siparis |
| Satis Faturalari | Satis/Fatura/Siparis |
| Satislara Gore Stoklar | Stok/Depo/Takip |
| Sevkiyat Listesi | Satis/Fatura/Siparis |
| Siparis Avans Bakiyelerinin Hesaplara Donus Fisi | Satis/Fatura/Siparis |
| Siparis Bordrosu | Satis/Fatura/Siparis |
| Siparis Girisi | Satis/Fatura/Siparis |
| Siparis Kapatma | Satis/Fatura/Siparis |
| Stok Hareket Raporu | Stok/Depo/Takip |
| Stok Karsilastirma Listesi | Stok/Depo/Takip |
| Stok Miktari Azalan Urunler | Stok/Depo/Takip |
| Stok Tutarlarina Gore Liste | Stok/Depo/Takip |
| Stok Valor ve Iskonto Listesi | Stok/Depo/Takip |
| Stoksuz Kampanyalari Silme | Stok/Depo/Takip |
| Sube Stok Maliyetleri | Stok/Depo/Takip |
| Sursarj Hareket Raporu - Tarih | Kampanya/Fiyat/Muhasebe |
| Sursarj Hareket Raporu - Urun | Stok/Depo/Takip |
| Tahsilat Gecikmeleri | Cari/Finans |
| Tahsilat Raporu | Cari/Finans |
| Tahsilat Takibi | Cari/Finans |
| Ters Bakiyelerin Siparis Avansina Aktarim Fisi | Satis/Fatura/Siparis |
| Toplam Stok Degeri | Stok/Depo/Takip |
| Toplu Kampanya Iptali | Kampanya/Fiyat/Muhasebe |
| TozSoft Web Siparis Takibi | Satis/Fatura/Siparis |
| Urun Aylik Giris Cikislari | Stok/Depo/Takip |
| Urun Aylik Ozet Olustur | Stok/Depo/Takip |
| Urun Giris Hareketi Duzelt | Stok/Depo/Takip |
| Urun Girisleri | Stok/Depo/Takip |
| Urun Gruplari | Stok/Depo/Takip |
| Urun Kartlari | Stok/Depo/Takip |
| Urun Maliyet Olustur | Stok/Depo/Takip |
| Urun Parti Bilgileri | Stok/Depo/Takip |
| Urun Takip Raporu | Stok/Depo/Takip |
| Vade Farki Listesi | Cari/Finans |
| Vadelerine Gore Stok Listesi | Stok/Depo/Takip |

### 20.8 Rapor Uzerindeki Mimari Etki

- `C:\OPA\OPA.EXE` dogrulamasi, hedef .NET uygulamasinda sekme bazli modul yapisi kurulmasi gerektigini netlestirdi.
- Page 1 cok genis: stok, urun, alis, fatura, satis, kampanya, reyon, tevzi, sube raporlari ve gunluk raporlar ayni ana sekmede toplanmis. Yeni sistemde bu alan `catalog`, `inventory`, `purchase`, `sales`, `warehouse` alt modullerine ayrilmali.
- Page 2 cari hesap, Page 4 finans ve vade takibi ayrik gorunuyor. Yeni sistemde `finance.accounts` ile `finance.ledger_entries` ayrilmali; cari kart ve finans hareket ayni tabloya yigilmemeli.
- Page 5 resmi muhasebe ve Page 6 entegrasyon ayrimi, muhasebe fisleri ile entegrasyon tekrar calistirma/entegrasyon kodlari icin ayri servis ve audit gerektigini gosteriyor.
- Page 8 servis programlari, eski sistemde veri duzeltme/ayirma/olusturma/yildevir gibi operasyonel araclar oldugunu gosteriyor. Yeni sistemde bunlar yetkili admin joblari ve migration/maintenance ekranlari olarak tasarlanmali.

## 21. F:\BATCH SETUP Paketleri Program, Menu ve Yapi Analizi

Bu bolum `F:\BATCH` altinda `SETUP*` ile baslayan kurulum/dagitim klasorlerinin sadece okunarak incelenmesiyle eklendi. Program, DBF, EXE, VBS veya BAT dosyalarinda degisiklik yapilmadi; silme islemi yapilmadi.

### 21.1 SETUP Paket Ozeti

| Paket | Yol | Son Degisim | Ana Islev | Yerel Calisma Klasoru / Baslatma |
|---|---|---|---|---|
| `SETUP.CHECKLIST` | `F:\BATCH\SETUP.CHECKLIST` | 2024-05-08 11:16:34 | Checklist / kontrol listesi | `C:\CHECKLIST\CHECKLIST.EXE` |
| `SETUP.eFatura` | `F:\BATCH\SETUP.eFatura` | 2024-05-08 11:18:21 | E-fatura/e-arsiv aktarim ve UBL/XML islemleri | `C:\EFATURA\EFATURA.EXE` |
| `SETUP.ITS` | `F:\BATCH\SETUP.ITS` | 2025-02-24 12:50:44 | ITS/UTS/PTS servis entegrasyonu | `C:\ITS\ITS.EXE` |
| `SETUP.kontrol2d` | `F:\BATCH\SETUP.kontrol2d` | 2024-03-14 21:07:13 | 2D/karekod kontrol, etiket, fatura kontrol | `C:\KONTROL2D\KONTROL2D.EXE` parametreli modlar |
| `SETUP.kontrol2d.altdepo` | `F:\BATCH\SETUP.kontrol2d.altdepo` | 2024-11-25 17:16:41 | Alt depo 2D/karekod veri paketi | `C:\KONTROL2D\KONTROL2D.EXE` parametreli modlar |
| `SETUP.OPA` | `F:\BATCH\SETUP.OPA` | 2025-10-10 20:50:17 | OPA ana uygulama kurulum paketi | `C:\OPA\OPA.EXE` ve `OPA.EXE NAZPHARMA` |
| `SETUP.prnyaz` | `F:\BATCH\SETUP.prnyaz` | 2025-10-10 20:11:46 | Yazdirma yoneticisi | `.NET prnyaz.exe`; yazici kuyrugu |

### 21.2 Dosya ve Bagimlilik Envanteri

| Paket | Dosya | Tip | Boyut | Surum | Urun/Aciklama | Rol |
|---|---|---|---:|---|---|---|
| `SETUP.CHECKLIST` | `SETUP.CHECKLIST\CHECKLIST.vbs` | .vbs | 1003 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.CHECKLIST` | `SETUP.CHECKLIST\checkList.EXE` | .EXE | 77509 | 4.4.3174 |  /  | Checklist / kontrol listesi uygulamasi |
| `SETUP.CHECKLIST` | `SETUP.CHECKLIST\msvcr71.dll` | .dll | 348160 | 7.10.3052.4 | Microsoft® Visual Studio .NET / Microsoft® C Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.CHECKLIST` | `SETUP.CHECKLIST\vfp9r.dll` | .dll | 4710400 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.CHECKLIST` | `SETUP.CHECKLIST\VFP9RENU.DLL` | .DLL | 1429504 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library Resources | Visual FoxPro runtime/destek |
| `SETUP.eFatura` | `SETUP.eFatura\efatura.EXE` | .EXE | 1776080 | 4.4.1427 |  /  | E-fatura, e-arsiv, UBL/XML ve e-belge modulu |
| `SETUP.eFatura` | `SETUP.eFatura\eFatura.vbs` | .vbs | 774 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.eFatura` | `SETUP.eFatura\GdiPlus.dll` | .dll | 1712128 | 5.1.3102.2180 (xpsp_sp2_rtm.040803-2158) | Microsoft® Windows® Operating System / Microsoft GDI+ | GDI+ grafik/PDF/goruntu bagimliligi |
| `SETUP.eFatura` | `SETUP.eFatura\msvcr71.dll` | .dll | 348160 | 7.10.3052.4 | Microsoft® Visual Studio .NET / Microsoft® C Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.eFatura` | `SETUP.eFatura\vfp9r.dll` | .dll | 4710400 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.eFatura` | `SETUP.eFatura\VFP9RENU.DLL` | .DLL | 1429504 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library Resources | Visual FoxPro runtime/destek |
| `SETUP.ITS` | `SETUP.ITS\C1.C1Zip.2.dll` | .dll | 94208 | 2.0.20062.37 | ComponentOne C1Zip / ComponentOne C1Zip | Yardimci dosya/veri/konfigurasyon |
| `SETUP.ITS` | `SETUP.ITS\chilkat register_win32.bat` | .bat | 172 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.ITS` | `SETUP.ITS\chilkat register_x64.bat` | .bat | 196 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.ITS` | `SETUP.ITS\ChilkatAx-9.5.0-win32.dll` | .dll | 10399744 | 9.5.0.86 | Chilkat ActiveX v9.5.0.86 / Chilkat ActiveX | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\ftp50.ocx` | .ocx | 137352 | 5.0.0.543 | IP*Works! ActiveX Edition Version 5.0 / IP*Works! V5 FTP Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\htmlml60.ocx` | .ocx | 212232 | 6.0.0.2008 | IP*Works! ActiveX Edition Version 6.0 / IP*Works! V6 HTMLMailer Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\Interop.PocketHTTP.dll` | .dll | 9728 | 1.0.0.0 | 'PocketHTTP' tür kitaplığından derleme alındı /  | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\Interop.PocketSOAP.dll` | .dll | 61440 | 1.0.0.0 | 'PocketSOAP' tür kitaplığından derleme alındı /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.ITS` | `SETUP.ITS\Interop.PocketSOAPAttachments.dll` | .dll | 8704 | 1.0.0.0 | 'PocketSOAPAttachments' tür kitaplığından derleme alındı /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.ITS` | `SETUP.ITS\Interop.PSPROXYLib.dll` | .dll | 4096 | 1.0.0.0 | 'PSPROXYLib' tür kitaplığından derleme alındı /  | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\its.exe` | .exe | 4798262 | 3.0.1134 |  /  | ITS/UTS/PTS takip ve servis entegrasyon modulu |
| `SETUP.ITS` | `SETUP.ITS\its.vbs` | .vbs | 604 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.ITS` | `SETUP.ITS\msvcr71.dll` | .dll | 348160 | 7.10.3052.4 | Microsoft® Visual Studio .NET / Microsoft® C Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.ITS` | `SETUP.ITS\pocketHTTP.dll` | .dll | 188416 | 1, 3, 3, 0 | Pocket HTTP / Pocket HTTP Library | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\psDime.dll` | .dll | 110676 | 1, 5, 1, 621 | Attachments Module / Attachments Module | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\pSOAP32.dll` | .dll | 380928 | 1, 5, 5, 2819 | PocketSOAP / SOAP Client for Windows | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\psProxy.dll` | .dll | 73728 | 1, 5, 0, 17 | psProxy / pocketSOAP SOAP Proxy | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\regocx32_64.bat` | .bat | 164 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.ITS` | `SETUP.ITS\regocx32b.bat` | .bat | 666 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.ITS` | `SETUP.ITS\regocx64b.bat` | .bat | 807 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.ITS` | `SETUP.ITS\saxzip.ocx` | .ocx | 552960 | 1.0.1211 | Sax Zip Objects / Sax Zip ActiveX control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.ITS` | `SETUP.ITS\stdole.dll` | .dll | 32408 | 7.00.9466 | Microsoft® Visual Studio .NET /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.ITS` | `SETUP.ITS\vfp9r.dll` | .dll | 4734976 | 9.0.00.5815 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 SP2 Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.ITS` | `SETUP.ITS\VFP9RENU.DLL` | .DLL | 1187840 | 9.0.00.5815 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 SP2 Runtime Library Resources | Visual FoxPro runtime/destek |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\2D Ayarlar.vbs` | .vbs | 651 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\2D Etiket.vbs` | .vbs | 653 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\2D Fatura.vbs` | .vbs | 654 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\2D Giriş.vbs` | .vbs | 650 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\2D Kontrol.vbs` | .vbs | 654 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\C1.C1Zip.2.dll` | .dll | 94208 | 2.0.20062.37 | ComponentOne C1Zip / ComponentOne C1Zip | Yardimci dosya/veri/konfigurasyon |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\chilkat register_win32.bat` | .bat | 172 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\chilkat register_x64.bat` | .bat | 196 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\ChilkatAx-9.5.0-win32.dll` | .dll | 10399744 | 9.5.0.86 | Chilkat ActiveX v9.5.0.86 / Chilkat ActiveX | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\fileml50.ocx` | .ocx | 149640 | 5.0.0.543 | IP*Works! ActiveX Edition Version 5.0 / IP*Works! V5 FileMailer Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\htmlml60.ocx` | .ocx | 212232 | 6.0.0.2008 | IP*Works! ActiveX Edition Version 6.0 / IP*Works! V6 HTMLMailer Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\IDAutomationDMATRIX.DLL` | .DLL | 173392 | 1, 8, 0, 3 | DataMatrix ActiveX and Encoder Module - www.IDAutomation.com / DataMatrix ActiveX and Encoder Module - www.IDAutomation.com | Barkod/DataMatrix ActiveX bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\IDAutomationLinear.dll` | .dll | 181584 | 1, 6, 0, 6 | BarCode / Linear BarCode ActiveX Control --DEMO | Barkod/DataMatrix ActiveX bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\Interop.PocketHTTP.dll` | .dll | 9728 | 1.0.0.0 | Assembly imported from type library 'PocketHTTP'. /  | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\Interop.PocketSOAP.dll` | .dll | 61440 | 1.0.0.0 | Assembly imported from type library 'PocketSOAP'. /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\Interop.PocketSOAPAttachments.dll` | .dll | 8704 | 1.0.0.0 | Assembly imported from type library 'PocketSOAPAttachments'. /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\Interop.PSPROXYLib.dll` | .dll | 4096 | 1.0.0.0 | Assembly imported from type library 'PSPROXYLib'. /  | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\ipport50.ocx` | .ocx | 129160 | 5.0.0.543 | IP*Works! ActiveX Edition Version 5.0 / IP*Works! V5 IPPort Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\kontrol2d.exe` | .exe | 1538870 | 4.5.1123 |  /  | 2D/karekod, etiket, fatura kontrol ve barkod okutma modulu |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\LOG\LOG2022.TXT` | .TXT | 25 |  |  /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\mscomm32.ocx` | .ocx | 105472 | 6.00.8169 | MSComm / MSComm | Yardimci dosya/veri/konfigurasyon |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\msvcr71.dll` | .dll | 348160 | 7.10.3052.4 | Microsoft® Visual Studio .NET / Microsoft® C Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\pocketHTTP.dll` | .dll | 188416 | 1, 3, 3, 0 | Pocket HTTP / Pocket HTTP Library | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\psDime.dll` | .dll | 110676 | 1, 5, 1, 621 | Attachments Module / Attachments Module | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\pSOAP32.dll` | .dll | 380928 | 1, 5, 5, 2819 | PocketSOAP / SOAP Client for Windows | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\psProxy.dll` | .dll | 73728 | 1, 5, 0, 17 | psProxy / pocketSOAP SOAP Proxy | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\reg kontrol2d Win10 x64.bat` | .bat | 617 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\reg kontrol2d Win10 x86.bat` | .bat | 617 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\reg kontrol2d Win7.bat` | .bat | 808 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\reg kontrol2d Windows10.bat` | .bat | 726 |  |  /  | ActiveX/OCX/DLL kayit veya kurulum betigi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\saxzip.ocx` | .ocx | 552960 | 1.0.1211 | Sax Zip Objects / Sax Zip ActiveX control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\smtp50.ocx` | .ocx | 137352 | 5.0.0.543 | IP*Works! ActiveX Edition Version 5.0 / IP*Works! V5 SMTP Control | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\vfp9r.dll` | .dll | 4710400 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.kontrol2d` | `SETUP.kontrol2d\VFP9RENU.DLL` | .DLL | 1429504 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library Resources | Visual FoxPro runtime/destek |
| `SETUP.OPA` | `SETUP.OPA\ChilkatAx-9.5.0-win32.dll` | .dll | 10399744 | 9.5.0.86 | Chilkat ActiveX v9.5.0.86 / Chilkat ActiveX | HTTP/SOAP/FTP/mail/zip ActiveX entegrasyon bagimliligi |
| `SETUP.OPA` | `SETUP.OPA\config.fpw` | .fpw | 183 |  |  /  | Yardimci dosya/veri/konfigurasyon |
| `SETUP.OPA` | `SETUP.OPA\MSVCR71.DLL` | .DLL | 348160 | 7.10.3052.4 | Microsoft® Visual Studio .NET / Microsoft® C Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.OPA` | `SETUP.OPA\NAZPHARMA.vbs` | .vbs | 2229 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.OPA` | `SETUP.OPA\opa.exe` | .exe | 8271895 | 3.2.6840 | Opa Depo Otomasyonu / opa.exe | OPA Depo Otomasyonu ana uygulama paketi |
| `SETUP.OPA` | `SETUP.OPA\Opa.vbs` | .vbs | 2192 |  |  /  | Yerel kopyalama/guncelleme ve baslatma betigi |
| `SETUP.OPA` | `SETUP.OPA\VFP9R.DLL` | .DLL | 4710400 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library | Visual FoxPro runtime/destek |
| `SETUP.OPA` | `SETUP.OPA\VFP9RENU.DLL` | .DLL | 1429504 | 9.0.00.2412 | Microsoft Visual FoxPro / Microsoft Visual FoxPro 9.0 Runtime Library Resources | Visual FoxPro runtime/destek |
| `SETUP.prnyaz` | `SETUP.prnyaz\Microsoft .NET Framework Version 4.0\dotNetFx40_Full_x86_x64.exe` | .exe | 50449456 | 4.0.30319.01 | Microsoft .NET Framework 4 / Microsoft .NET Framework 4 Setup | .NET Framework kurulum paketi |
| `SETUP.prnyaz` | `SETUP.prnyaz\prnyaz.exe` | .exe | 128512 | 1.0.0.0 | prnyaz / prnyaz | Yazdirma yoneticisi / XML-HTML yazici kuyrugu |
| `SETUP.prnyaz` | `SETUP.prnyaz\prnYaz.txt` | .txt | 35 |  |  /  | Yardimci dosya/veri/konfigurasyon |

### 21.3 Baslatma Betikleri ve Parametreler

| Paket | Betik | Yerel Klasor | Guncelleme Kaynagi | Calistirdigi Komut / Parametre |
|---|---|---|---|---|
| SETUP.CHECKLIST | `CHECKLIST.vbs` | `C:\CHECKLIST\` | `F:\BATCH\CHECKLIST.EXE` | `checklist.exe` |
| SETUP.eFatura | `eFatura.vbs` | `C:\EFATURA\` | `F:\BATCH\EFATURA.EXE`, `EFATURA.XSLT` | `EFATURA.EXE` |
| SETUP.ITS | `its.vbs` | `C:\ITS\` | `F:\BATCH\ITS.EXE` | `ITS.EXE` |
| SETUP.kontrol2d | `2D Ayarlar.vbs` | `C:\KONTROL2D\` | `F:\BATCH\KONTROL2D.EXE` | `KONTROL2D.EXE AYAR` |
| SETUP.kontrol2d | `2D Etiket.vbs` | `C:\KONTROL2D\` | `F:\BATCH\KONTROL2D.EXE` | `KONTROL2D.EXE ETIKET` |
| SETUP.kontrol2d | `2D Fatura.vbs` | `C:\KONTROL2D\` | `F:\BATCH\KONTROL2D.EXE` | `KONTROL2D.EXE SIPKONT` |
| SETUP.kontrol2d | `2D Giris.vbs` | `C:\KONTROL2D\` | `F:\BATCH\KONTROL2D.EXE` | `KONTROL2D.EXE ANA` |
| SETUP.kontrol2d | `2D Kontrol.vbs` | `C:\KONTROL2D\` | `F:\BATCH\KONTROL2D.EXE` | `KONTROL2D.EXE FATKONT` |
| SETUP.OPA | `Opa.vbs` | `C:\OPA\` | `F:\BATCH\OPA.EXE` + runtime DLL/FON | `OPA.EXE` |
| SETUP.OPA | `NAZPHARMA.vbs` | `C:\OPA\` | `F:\BATCH\OPA.EXE` + runtime DLL/FON | `OPA.EXE NAZPHARMA` |
| SETUP.prnyaz | `prnYaz.txt` | Program klasoru | Konfigurasyon dosyasi | `xml2HtmlEngine = E`, `dokumSayisi = 2` |

### 21.4 EXE Menu/Ekran/Islem Izleri

Aşağıdaki listeler EXE dosyalarindaki okunabilir metinlerden uretilmistir. Kaynak kod olmadigi icin tam menu sirasi garanti edilmez; fakat program yuzeyi, ekran adlari, servis endpointleri ve islem tipleri hakkinda guclu kanit verir.

#### SETUP.CHECKLIST\checkList.EXE
- Boyut/surum: 77509 byte, version `4.4.3174`, product ``
- ?xml version
- assembly xmlns
- trustInfo xmlns
- publicKeyToken
- checklist
- checklist.exe

#### SETUP.eFatura\efatura.EXE
- Boyut/surum: 1776080 byte, version `4.4.1427`, product ``
- ?xml version
- assembly xmlns
- trustInfo xmlns
- publicKeyToken
- Program kısayolu hatalı. Program C:\eFatura klasöründen çalıştırılmalıdır.ış
- library_efatura
- DO hata WITH LINENO()ş
- efatura.EXEış
- c:\efatura\efatura.prg0ò
- data\efatura.mem0ıù
- DATA\efatura
- EARSIV.MEM0
- efatura.xsltış
- DEPOIDö”
- Depo lisansı tanımsız, sqlNop açılamıyor.
- EFATURA.EXE»ø
- E-fatura Aktarım Modülüış
- ARSIV\
- ALTDEPOö
- glngrs.dbf
- glngrsh.dbf
- İskonto Grupları Dosyası Hatalı...êxış
- ECZA DEPOSUış
- gelenêÆ
- gelenş
- gelenxmlêÆ
- gelenxmlş
- arsiv\gelenş
- gidenxmlêÆ
- gidenxmlş
- yedekxmlêÆ
- yedekxmlş
- xmldataêÆ
- xmldataş
- c:\efatura\log\ış
- logêÆ
- LOGş
- gelenxml\*.*ê
- GELENxml\CC
- yedekxml\*.*ê
- yedekxml\CC
- Hata Alanlarış
- Raporlananlarış
- Temel Faturalarış"
- Hata Alanlarış"
- Hata ış
- ptsdetayış
- hatavar
- frmftrgelenş
- EARSIVUYG
- LOGMODE
- PTSBAKIMYAP
- SIFRE
- _EARSIV
- ZMNGLNKNT
- FATGONHATA
- XMLTRIM
- DIRECTORY_STOK
- ILACFIRMA
- SECFIRMA
- DEPOETKPK
- DEPOETKGB
- DEPOVNO
- DEPOONEK
- DEPOARCADI
- DEPOARCSMT
- DEPOARCSHR
- DEPOARCVNO
- DEPOARCETK
- DEPOADI
- DEPOADIL
- LDEPOADI
- KURUMADI_TR
- KURUMADI_EN1
- KURUMADI_EN2
- DEPOID
- DEPOADIW
- _DEPOGLN
- _DEPOUSERNAME
- _DEPOPASSWORD
- FILE_AC_KULLAN
- DEPOGLN
- ITSKULLAN
- ITSSIFRE
- IADESORU
- LOGINOLDUMU
- PKETIKET
- GBETIKET
- GLNNO
- NORMAL
- GLPTSILK
- GLNOEXIT
- GCACIKSIPARISFORM
- KULLANICI
- ALT_DEPO_TANIM
- GET_ALTDEPO_BILGI
- ITSENABLED
- PUBSTOKBAS
- PUBSTOKMNU
- _DEPOAUTO
- GELEN
- GELENXML
- ARSIV
- GIDENXML
- YEDEKXML
- XMLDATA
- LOGKLS
- KKLOGBOL
- SONPTSADI
- SONPTSNO
- SUREITS
- SUREPTS
- PTSILKSEFER
- PTSVARDIYA
- PTSSONAKT
- ACIKHATAMSJ
- GET_CARI_KART
- SECMENU
- MAINMENU1
- MAINMENU2

#### SETUP.ITS\its.exe
- Boyut/surum: 4798262 byte, version `3.0.1134`, product ``
- ?xml version
- assembly xmlns
- trustInfo xmlns
- publicKeyToken
- Program kısayolu hatalı. Program C:\ITS klasöründen çalıştırılmalıdır.ış
- DO hata WITH LINENO()ş
- ITS.EXEış
- "http://its.iegm.gov.tr/bildirim/BR/v1/UrunDogrulama/Genel"ış@
- http://212.175.173.34:80/UrunDogrulama/UrunDogrulamaReceiverServiceışI
- http://212.175.173.36/PTS/PackageSenderWebServiceış9
- "http://its.iegm.gov.tr/pts/sendpackage"ışS
- http://its.saglik.gov.tr:80/UrunDogrulama/UrunDogrulamaReceiverServiceışL
- http://pts.saglik.gov.tr/PTS/PackageSenderWebServiceış9
- "http://its.iegm.gov.tr/pts/sendpackage"ış
- http://its.saglik.gov.tr/ITSServices/ReceiptNotificationışD
- http://its.saglik.gov.tr/ITSServices/ReturnNotificationışF
- http://its.saglik.gov.tr/ITSServices/DispatchNotificationışF
- http://its.saglik.gov.tr/ITSServices/DispatchCancellationışR
- http://its.saglik.gov.tr/ITSServices/CheckStatusNotificationışB
- http://its.saglik.gov.tr/IhracatIptalBildirim/IhracatIptalReceiverServiceışE
- "http://its.iegm.gov.tr/bildirim/BR/v1/IhracatIptal"ışF
- http://its.saglik.gov.tr/ITSServices/TransferNotificationışM
- http://its.saglik.gov.tr/ITSServices/TransferCancellationışM
- DATA\ITSPARAM
- LOG\
- DATA\BILGILOG.DBF0
- Önce ana iletişim programı açılmalıø
- MKDIR &muhkls.LOG
- DATA\BILGILOG.DBF0ıù
- ALTDEPOö‹
- UTSBAKIM.DBF
- DEPOIDö
- DEPO-KISA-DOSYA-ADIö
- ITS ve ÜTS İletişim Programıış
- ITS İletişim Programıış
- Program ITS veya ÜTS no. ile çalıştırılabilirêxış
- C:\ITS\ITS.EXEêáış
- utstestó
- utsuygulama6ış
- glngrs.dbf
- glngrsh.dbf
- data\glnparam.mem0ıù
- data\glnparam
- İskonto Grupları Dosyası Hatalı...êxış
- ECZA DEPOSUış
- c:\its\gelen\ış
- gelenêÆ
- gelenş
- c:\its\gelenxml\ış
- gelenxmlêÆ
- gelenxmlş
- c:\its\gidenxml\ış
- gidenxmlêÆ
- gidenxmlş
- c:\its\yedekxml\ış
- yedekxmlêÆ
- yedekxmlş
- c:\its\pts\ış
- ptsêÆ
- ptsş
- gelenxml\*.*ê
- GELENxml\CC
- yedekxml\*.*ê
- yedekxml\CC
- ptsNum00ış
- KKLOGILK
- KKLOGILKı
- hatavar
- OTOUTSBIL
- PTSBAKIMYAP
- SIFRE
- _WSXMLNSDOGRULAMA
- _WSXMLNSIHRACAT
- _WSXMLNSDEAKTIVATE
- _WSXMLADDOGRULAMA
- _WSXMLADIHRACAT
- _WSXMLADDEAKTIVATE
- _WSXMLADREFSERV
- _WSXMLADPTSSEND
- _WSXMLNSPTSSEND
- _WSXMLNSSATIS
- _WSXMLNSSATISIPTAL
- _WSXMLNSMALALIM
- _WSXMLNSMALIADE
- _WSXMLNSURETIM
- _WSXMLADMALALIM
- _WSXMLADMALIADE
- _WSXMLADSATIS
- _WSXMLADSATISIPTAL
- _WSXMLADURETIM
- _WSXMLADURUNDURUM
- _WSXMLNSURUNDURUM
- _WSXMLADIHRACATIPTAL
- _WSXMLNSIHRACATIPTAL
- _WSXMLADMALDEVIR
- _WSXMLNSMALDEVIR
- _WSXMLADMALDEVIRIPTAL
- _WSXMLNSMALDEVIRIPTAL
- _DEPOAUTO
- _DEPOGLN
- _DEPOUSERNAME
- _DEPOPASSWORD
- _DEPOSORUMLU
- _DEPOSTS
- _DEPOSTSIADE
- _DEPOALIM
- _DEPOALIMIADE
- _DEPODEVIR
- _DEPOSTTRH
- _DEPOSITRH
- _DEPOALTRH
- _DEPOAITRH
- _DEPODVTRH
- LOGFILESIZE
- SECFIRMA
- GLNERRSTR
- URUNDURUMOK
- ITS_TOKEN_KEY
- ITS_TOKEN_EXPIRE_D
- ITS_TOKEN_EXPIRE_T

#### SETUP.kontrol2d\kontrol2d.exe
- Boyut/surum: 1538870 byte, version `4.5.1123`, product ``
- ?xml version
- assembly xmlns
- trustInfo xmlns
- publicKeyToken
- Program kısayolu hatalı. Program C:\Kontrol2d klasöründen çalıştırılmalıdır.ış
- KONTROL2D.PJT0ıù
- kontrol2d.exeış
- library_kontrol2d
- Do HATA With Lineno()ş
- LOG\ış
- LOG\
- YAZICI\
- Mkdir &LOG2DKLS
- ISLEMLOG.DBF
- Depo bağlantısı sağlanamadı.Ù
- Barkod kontrol sistemiø
- PTScom.dllö[
- C:\KONTROL2D\KONTROL2D.EXEêáış"
- KONTROL2D.EXE
- KONTROL2D.PJT0
- Kullandığınız programın daha yeni bir sürümü mevcuttur. Program kısayolu hatalı olabilir.Ù
- Kontrol2d Eski Sürüm Uyarısıø
- ALTDEPOö
- UTSTOKENbÙ
- depoutsnobÙ
- Use &muhkls.NODELIST
- KONTROLı
- UTSDTY00.DBF
- DEPOIDöÑ
- DEPO-KISA-DOSYA-ADIöÑ
- 2dKontrolış
- utsuygulamaış
- http://its.saglik.gov.tr/ITSServices/ReceiptNotificationışD
- http://its.saglik.gov.tr/ITSServices/ReturnNotificationışF
- http://its.saglik.gov.tr/ITSServices/DispatchNotificationışF
- http://its.saglik.gov.tr/ITSServices/DispatchCancellationışR
- http://its.saglik.gov.tr/ITSServices/CheckStatusNotificationışV
- http://its.saglik.gov.tr/IhracatIptalBildirim/IhracatIptalReceiverServiceışF
- http://its.saglik.gov.tr/ITSServices/TransferNotificationışF
- http://its.saglik.gov.tr/ITSServices/TransferCancellationışS
- http://its.saglik.gov.tr:80/UrunDogrulama/UrunDogrulamaReceiverServiceışL
- http://its.saglik.gov.tr/ITSServices/CheckStatusNotificationışB
- http://its.saglik.gov.tr/IhracatIptalBildirim/IhracatIptalReceiverServiceışE
- "http://its.iegm.gov.tr/bildirim/BR/v1/IhracatIptal"ışF
- http://its.saglik.gov.tr/ITSServices/TransferNotificationışM
- http://its.saglik.gov.tr/ITSServices/TransferCancellationışM
- Tarih hatası! Servisten Stok Firma No ve devir işlemini kontrol ediniz.Ù
- 2D Kontrolø
- İskonto Grupları Dosyası Hatalı !Ù
- AYARLAR.MEM
- Restore From &datakls.ayarlar AddI
- AYAR
- AYARış
- ETIKET
- ETIKETış
- ayar2Dş
- formkontrolÑüø
- HATAFAZ
- HATAPROC
- IPTALYETKI
- KKLOGBOL
- WINDEPOADI
- ITS_TOKEN_KEY
- ITS_TOKEN_EXPIRE_D
- ITS_TOKEN_EXPIRE_T
- ETIKETPRIN
- LOG2DKLS
- LOGKLS
- YAZICIKLS
- LOGSR
- _DEPO
- DIRECTORY_STOK
- DEPOADI
- KURUMADI_TR
- KURUMADI_EN1
- KURUMADI_EN2
- LDEPOADI
- MALFAZLASI
- FATURANO
- MIAD
- YMALFAZLA
- YMIAD
- BARKODU
- KURUMISK
- BARKOD
- SERINO
- DEPOGLNO
- _DEPOGLN
- _DEPOUSERNAME
- _DEPOPASSWORD
- _DEPOSORUMLU
- FILE_AC_KULLAN
- ALT_DEPO_TANIM
- GET_ALTDEPO_BILGI
- DEPOGLN
- PUBITSKULL
- ITSKULLAN
- PUBITSSIFRE
- ITSSIFRE
- _DEPOTOKEN
- UTSTOKEN
- _DEPOUTSNO
- DEPOUTSNO
- CEPDEPOADI
- CEPDEPOTEL
- CEPDEPOFAX
- ITSVAR
- SONGIRUTS
- PUBUTSVAR
- PUBUTSTOUT
- PUBUTSHATA
- PUBUTSHSAY
- PUBUTSBAK
- PUBUTSHTGEC
- DEPOID
- DEPOKISA
- KULLANICI
- GSIFRE
- PUBUTSADR
- ANAGLN

#### SETUP.OPA\opa.exe
- Boyut/surum: 8271895 byte, version `3.2.6840`, product `Opa Depo Otomasyonu`
- ?xml version
- assembly xmlns
- trustInfo xmlns
- publicKeyToken
- llogindone
- PROCEDURE favnormal
- * MESSAGEBOX('SECFIRMA:'
- SECFIRMA
- 'KFIRMA:'
- KFIRMA
- * MESSAGEBOX('directory_stok : '
- directory_stok, 0)
- 'icodepo.ico'
- obj_1Colons.Add(this.mainpageframe.page1.stok)
- obj_2Colons.Add(this.mainpageframe.page1.faturaislemleri)
- obj_2Colons.Add(this.mainpageframe.page1.urunbilgileri)
- obj_2Colons.Add(this.mainpageframe.page1.gunlukraporlar)
- obj_2Colons.Add(this.mainpageframe.page1.stokraporlari)
- obj_2Colons.Add(this.mainpageframe.page1.suberaporlari)
- obj_2Colons.Add(this.mainpageframe.page2.raporlar)
- obj_2Colons.Add(this.mainpageframe.page3.raporlar)
- obj_2Colons.Add(this.mainpageframe.page4.musteriraporlari)
- obj_2Colons.Add(this.mainpageframe.page4.saticiraporlari)
- obj_2Colons.Add(this.mainpageframe.page4.genelraporlar)
- obj_2Colons.Add(this.mainpageframe.page5.raporlar)
- obj_2Colons.Add(this.mainpageframe.page8.genelayarlar)
- obj_3Colons.Add(this.mainpageframe.page1.cikanurun1)
- obj_3Colons.Add(this.mainpageframe.page1.tevziurun)
- obj_3Colons.Add(this.mainpageframe.page1.cikanurun2)
- obj_3Colons.Add(this.mainpageframe.page1.urunlisteleri)
- obj_3Colons.Add(this.mainpageframe.page1.urungirisleri)
- obj_3Colons.Add(this.mainpageframe.page1.faturabordrolari)
- obj_3Colons.Add(this.mainpageframe.page1.faturatoplamlari)
- obj_3Colons.Add(this.mainpageframe.page1.iptaledilmisfaturalar)
- obj_3Colons.Add(this.mainpageframe.page1.satisraporlari)
- obj_3Colons.Add(this.mainpageframe.page1.musteritakipraporlari)
- obj_3Colons.Add(this.mainpageframe.page1.primhesaplamalari)
- obj_3Colons.Add(this.mainpageframe.page1.urunraporlari)
- obj_3Colons.Add(this.mainpageframe.page1.urunsatisraporlari)
- obj_3Colons.Add(this.mainpageframe.page1.stokbakim)
- obj_3Colons.Add(this.mainpageframe.page1.stokislemleri)
- obj_3Colons.Add(this.mainpageframe.page1.konsolurun)
- obj_3Colons.Add(this.mainpageframe.page1.konsolstok)
- obj_3Colons.Add(this.mainpageframe.page4.musteritakipraporlari)
- obj_3Colons.Add(this.mainpageframe.page4.donemselrapor)
- * Son kullanilan modul sekmesi
- * menu
- Public mainmenu
- mainmenu
- DEFINE MENU (mainmenu) in "formmain" BAR
- DEFINE PAD mn_srkt OF (mainmenu) PROMPT "Şirket"
- DEFINE PAD mn_ekrn OF (mainmenu) PROMPT "Ekran"
- DEFINE PAD mn_link OF (mainmenu) PROMPT "Link"
- DEFINE PAD mn_kull OF (mainmenu) PROMPT "Kullanıcı"
- DEFINE PAD mn_prog OF (mainmenu) PROMPT "Program"
- * DEFINE PAD mn_quit OF (mainmenu) PROMPT "Çıkış"
- ON SELECTION PAD mn_srkt OF (mainmenu) do launchFrmSirket
- ON PAD mn_ekrn OF (mainmenu) ACTIVATE POPUP mn_ekrn
- ON PAD mn_link OF (mainmenu) ACTIVATE POPUP mn_link
- ON SELECTION PAD mn_kull OF (mainmenu) do FORM formKull
- ON SELECTION PAD mn_prog OF (mainmenu) do FORM formOpa
- * ON SELECTION PAD mn_quit OF (mainmenu) Do down
- IF NOT THISFORM.lLoginDone
- THISFORM.lLoginDone
- .T.  && Prevent multiple logins
- LOCAL oLoginForm
- DO FORM formlogin NAME oLoginForm LINKED
- thisform.mainpageframe.page1.stok.command1.Enabled
- thisform.mainpageframe.page1.stok.command2.Enabled
- thisform.mainpageframe.page1.stok.command3.Enabled
- thisform.mainpageframe.page1.stok.command4.Enabled
- thisform.mainpageframe.page1.stok.command5.Enabled
- thisform.mainpageframe.page1.stok.command6.Enabled
- thisform.mainpageframe.page1.stok.command7.Enabled
- thisform.mainpageframe.page1.stok.command8.Enabled
- thisform.mainpageframe.page1.stok.command9.Enabled
- thisform.mainpageframe.page1.stok.command10.Enabled
- thisform.mainpageframe.page1.stok.command11.Enabled
- thisform.mainpageframe.page1.stok.command12.Enabled
- thisform.mainpageframe.page1.stok.command13.Enabled
- LOCAL menuRoot, oCounter
- menuRoot
- IF menuRoot
- thisform.mainpageframe.page1.stok.command1.SetFocus
- usrLogSave('nKeyCode : '
- LISTINDEX
- NORMALBTNCOLORœ
- FAVNORMAL
- NORMALBTNCOLOR
- formayarlargenelş
- FORMAYARLARGENEL
- formayarlarTicariş
- FORMAYARLARTICARI
- USRLOGSAVE
- Depo çalışırken satış iskonto bilgileri değiştirilemez.ø
- RAPORLAR
- menu_muhasebe_fisbasış
- @llogindone
- *favnormal
- "Stok"
- 'Stok [L]'
- 'Stok İşlemleri [L]'
- stkButtons.Add(this.stok.command1)
- stkButtons.Add(this.stok.command2)
- stkButtons.Add(this.stok.command3)
- stkButtons.Add(this.stok.command4)
- stkButtons.Add(this.stok.command5)
- stkButtons.Add(this.stok.command6)
- stkButtons.Add(this.stok.command7)
- stkButtons.Add(this.stok.command8)
- stkButtons.Add(this.stok.command9)
- stkButtons.Add(this.stok.command10)
- stkButtons.Add(this.stok.command11)
- stkButtons.Add(this.stok.command12)
- stkButtons.Add(this.stok.command13)
- stkButtons.Add(this.cikanurun1.command1)
- stkButtons.Add(this.cikanurun1.command2)
- stkButtons.Add(this.cikanurun1.command3)
- stkButtons.Add(this.tevziurun.command1)
- stkButtons.Add(this.tevziurun.command2)

#### SETUP.prnyaz\Microsoft .NET Framework Version 4.0\dotNetFx40_Full_x86_x64.exe
- Boyut/surum: 50449456 byte, version `4.0.30319.01`, product `Microsoft .NET Framework 4`
- Failed to allocate log
- Failed while running the progress dialog.
- Failed while running the extract directory selection dialog.
- Failed to allocate memory for logical drives
- Failed to get logical drives
- --- logging level: %s ---
- Logging started: %S
- Logging stopped: %S
- EncodePointer
- DecodePointer
- GetLogicalDriveStringsW
- DialogBoxParamA
- EndDialog
- DialogBoxParamW
- _DecodePointerInternal@4
- _EncodePointerInternal@4
- ?xml version
- assembly xmlns
- publicKeyToken
- trustInfo xmlns
- compatibility xmlns
- B3LoTôn
- LOGU]
- LÑGumaLRr
- öXıTS_9ù
- ıPTSË*
- zÜUTsî
- ptsRfİÑ
- BdÄITsP
- ñlÊ»EùgpDF.ŒHKO…t
- óGlnì
- PTSÇh
- QEfAt
- ó_Ü…“šZXMl
- lIStµA†½
- PkıtSg³
- ²ÚÊÉdxMl
- xMlë
- ÄÅ&’ÄòEPts
- Q3öÛšïBöôp¼ÜloT
- ıTS;
- ÖLOG
- {LogÚ9¿á
- Kß&pdf/
- 0ŞgRGln\XÁÄ
- w! lLoT¼Ã
- şğğ8À!gLN}G/
- ÍòXMLÅ”èç‘
- GLNÇ
- †âLotmÅâ
- VÊÅ‰LoG
- NqÁÅÌloT
- ‹ITsöF;³ı
- «log
- m&KtÔØ•GLn(åÎâÇ«
- PX!mAlJ
- Qn#ITs
- õÈ¼—ITs
- xÒàlOT
- lzÄ]tšŠpdF‹
- xmlè kr
- »mAl%AêdY
- åGlN9
- -PdFNÛßôNÂÙË
- šWKXMLğo
- GLnĞ
- uÔÂKXÖ¼6PTsJ}gŞJ
- @Ç –VPDF– Ö
- –›/LoGz
- zü‰3UTSº@ô'½
- æqPts•§&t
- loG’Ù†Z5•3
- 5(glnÍ
- "ØPpDf
- LOG'p
- IÏÌ#êöpdfÿÚü«
- äFPãRWôÔglN
- ßP„ıŒlOG
- ÁHš1 y…øÒLog¾
- ºä†ŒxmL
- kYLoG
- ”†•ïGÑŠ\UtS:‹Ñ
- å]ÉÂ¼Dq-æloG
- jÖãlOT‡
- ItsT;†Wa
- “PDFw
- ¿¹C#5İv-ÕPDfõ
- U¿ÛItS
- pTSëÌ
- ğAUts
- LoTXo
- U8Å5‚úƒ‘ItS½{
- PTs‹
- #uËçvUtSOÒEc¿
- pTsë
- ŠLOT“{«'
- ùlOT„ÍÆ!z
- Ç 'xMlË
- ’*øÿMLOtÁ
- ınçloGŒV
- ó9rmXëpTSQ–
- LotÅZE'¶]ÖF
- mAl½
- .”EßfaT
- 7utSäğ
- Ã‚ËêÙPtS
- lOTÕVÑt9ù
- logvÀ
- NlOGCÿ?Ïå¾
- PTsÖÊú-•}è7DM
- f@ËXmL
- RBxmL4‚—ñÕ.
- utsµ
- 2æLot’më
- GÆÈBĞ§çëÆgLNÜ
- z¾MalÑE—î-
- ânÌHâIıó5ZÌı KµtîC}üš,1PDFGõÜı·&ÒgÌú
- ‹lotRó
- loTæA
- äÖºĞ0LOg

#### SETUP.prnyaz\prnyaz.exe
- Boyut/surum: 128512 byte, version `1.0.0.0`, product `prnyaz`
- neutral, PublicKeyToken
- bilgisayarAdi
- depoKls
- xmlKls
- xml2HtmlEngine
- kontrol_Tick
- yaziciyaGonder
- xml2html
- menuItem1_Click
- menuItem3_Click
- kontrol
- ContextMenu
- MenuItem
- Menu
- MenuItemCollection
- get_MenuItems
- set_ContextMenu
- DialogResult
- System.Xml
- XmlDocument
- XmlNameTable
- XmlNamespaceManager
- XmlNode
- System.Xml.Xsl
- XmlReader
- System.Xml.XPath
- Normalize
- NormalԀ66Ԁ10Ԁ30
- ]ᴀxml2HtmlEngine
- [xml2HtmlEngine]ᜀdokumSayisiༀ kopya]
- DEPO\ༀYAZICI\ᄀYAZICI\YᤀFATURADOKUM\ᴀHTML2PDF\html\ᤀC:\DOKUMLER\ἀHTML2PDF\arsiv\
- #ༀAyarlarༀ       ༀKapat
- [ἀ] prnYaz hazır ᄁF:\DEPO\܀F:\ᄀG:\DEPO\܀G:\ᄀK:\DEPO\܀K:\܀C:\
- prnYaz bekliyor.䬀Gelen dökümler kuyrukta bekletilecek!欁prnYaz önceden çalıştırılmış. Yeniden çalıştırılamaz.
- 愀Yazıcı kuyruğu tanımlarken boşluk kullanmayınız.㬁1. ve 2. yazıcı kuyruğu aynı.㬁1. ve 3. yazıcı kuyruğu aynı.㬁2. ve 1. yazıcı kuyruğu aynı.
- 㬁2. ve 3. yazıcı kuyruğu aynı.㬁3. ve 1. yazıcı kuyruğu aynı.㬁3. ve 2. yazıcı kuyruğu aynı.
- software\prnyaz\漀 klasörüne erişilemedi, ağ bağlantısını kontrol ediniz.
- Ağ Erişim Hatası
- *.XML
- Yazdırma hatası ἁ yazdırılamadı.
- .XML
- {0:yyyyMMdd-hhmmss}.txt䴁The specified printer has been deleted㔀Settings to access printer㴀 yazdırma hatası.(Hata Kodu :
- Yazdırma hatası!
- yazıcıya gönderilemedi.䜁 çalışmıyor, lütfen kontrol ediniz.ἁYazdırma Hatası
- yazıcıya gönderildi.㔁Döküm yazıcıya gönderildi.܁cbc耀疉爀渀㨀漀愀猀椀猀㨀渀愀洀攀猀㨀猀瀀攀挀椀昀椀挀愀琀椀漀渀㨀甀戀氀㨀猀挀栀攀洀愀㨀砀猀搀㨀䌀漀洀洀漀渀䈀愀猀椀挀䌀漀洀瀀漀渀攀渀琀猀ⴀ
- 뎀prnYaz programını kapatırsanız, yazdırma işleri durur. Programı kapatmak istiyor musunuz?
- prnYaz Kapatma Onayı
- prnyaz Yazıcı
- this.IconἀYazıcı Yönetimi
- Yazdırma Yöneticisi㜁prnyaz.Properties.Resources

### 21.5 SETUP DBF / Veri Dosyasi Semalari

SETUP paketleri sadece EXE degil, bazi hazir DBF sablon/veri dosyalari da tasiyor. Bunlar yeni sistemde migration fixture, temp/staging tablo veya yardimci servis veri modeli olarak ele alinmali.

| DBF | Kayit | Alan | Kayit Uzunlugu | Boyut KB | Alan Ozeti |
|---|---:|---:|---:|---:|---|
| `SETUP.ITS\dtyskl.DBF` | 501 | 9 | 142 | 70 | UPAKETKODU C(40,0); BARKOD C(40,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); KUTUTIPI C(1,0); DUZEY N(2,0); UPAKETSIRA C(6,0); PAKETSIRA C(6,0) |
| `SETUP.ITS\FOXUSER.DBF` | 3 | 7 | 48 | 0,6 | TYPE C(12,0); ID C(12,0); NAME M(4,0); READONLY L(1,0); CKVAL N(6,0); DATA M(4,0); UPDATED D(8,0) |
| `SETUP.ITS\TEMP\itstemp01.DBF` | 1 | 27 | 589 | 1,7 | ILACADI C(30,0); BARKOD C(13,0); MIAD C(6,0); SERINO C(20,0); SIRANO C(20,0); UYARIACIK C(80,0); CIKISTAR D(8,0); ITSSONUC C(1,0); CEVAPKODU C(10,0); MESAJ C(150,0); KAREKOD C(80,0); BILDIRIMID C(8,0); UYARIKODU C(6,0); GIRISTAR D(8,0); GIRISCARI C(10,0); GIRISADI C(30,0); CIKISCARI C(10,0); CIKISADI C(30,0); KKLOGREC N(12,0); DEGISTI C(1,0); ILACKODU N(6,0); KUTUTIPI C(1,0); UPAKETKODU C(20,0); MIADSORUN C(1,0); GLN1 C(13,0); GLN2 C(13,0); RENK C(1,0) |
| `SETUP.ITS\TEMP\kklogydk.DBF` | 0 | 12 | 112 | 0,7 | ILACKODU N(6,0); BARKOD C(14,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); HAREKETTUR C(1,0); BELGENO C(7,0); HESAPKODU C(10,0); TARIH D(8,0); TARIHSAAT C(12,0); ITSSONUC C(1,0); PAKETSIRA C(6,0) |
| `SETUP.ITS\users\01\kbdty.DBF` | 0 | 13 | 280 | 0,7 | TARIH D(8,0); BELGENO C(12,0); HAREKETTUR C(12,0); HESAPKODU C(10,0); ADI C(30,0); UYARIACIK C(60,0); SIRANO C(20,0); SERINO C(20,0); MIAD C(6,0); KAREKOD C(80,0); KKRECNO N(10,0); FRECNO N(10,0); RENK C(1,0) |
| `SETUP.ITS\users\01\utsstok.DBF` | 324 | 10 | 126 | 40,5 | ILACKODU N(6,0); BARKODU C(13,0); ILACADI C(30,0); LOTNO C(20,0); ADET N(10,0); MIAD C(6,0); SERISIRA C(20,0); FIRMAADI C(10,0); FIRMAKODU N(4,0); YFIRMAKODU C(6,0) |
| `SETUP.ITS\yetki.DBF` | 0 | 5 | 53 | 0,4 | USER C(15,0); ONCELIK C(1,0); KULADI C(10,0); SIFRE C(10,0); USERRIGHT C(16,0) |
| `SETUP.kontrol2d\data\eitempa.dbf` | 0 | 49 | 305 | 1,8 | ECZANEKODU C(10,0); ILACTIPI C(1,0); ISKONTO C(1,0); STATU C(2,0); GRUPKODU C(1,0); ILACKODU N(6,0); FIYATI N(13,0); MIKTARI N(6,0); MALFAZLASI N(5,0); TARIH D(8,0); FATURANO C(7,0); USERNO C(3,0); REYON C(2,0); CFIYATI N(12,0); FIYATNO C(1,0); NETFIYAT C(8,0); MIAD C(4,0); ITHAL C(1,0); ALISKDV N(2,0); CEPNO C(2,0); ILACADI C(30,0); VADEGUN N(3,0); RAF C(5,0); ISKONTO1 N(5,2); ISKONTO2 N(5,2); ISKONTO3 N(5,2); ISKONTO4 N(5,2); YMIKTARI N(6,0); YMALFAZLA N(5,0); YCFIYATI N(12,0); YMIAD C(4,0); SONFIYATI N(12,0); FATGRUP C(5,0); HARRECNO N(6,0); PUAN N(9,0); BARKODU C(13,0); TALEP C(1,0); OKUNAN N(6,0); ADETKUTU N(7,0); SEPETKODU C(5,0); REYONSIRA C(1,0); KARSIREYON C(2,0); ILCNOT C(20,0); KURUMISK N(5,2); KUTUTIPI C(1,0); SIRANO N(5,0); RAFOMRU C(10,0); BIRIM C(10,0); AMBALAJMIK N(6,0) |
| `SETUP.kontrol2d\data\eitempn.dbf` | 0 | 49 | 324 | 1,8 | ECZANEKODU C(10,0); ILACTIPI C(1,0); ISKONTO C(1,0); STATU C(2,0); GRUPKODU C(1,0); ILACKODU N(6,0); FIYATI N(12,0); MIKTARI N(6,0); MALFAZLASI N(5,0); TARIH D(8,0); FATURANO C(7,0); USERNO C(3,0); REYON C(2,0); CFIYATI N(12,0); FIYATNO C(1,0); NETFIYAT C(8,0); MIAD C(4,0); ITHAL C(1,0); ALISKDV N(2,0); CEPNO C(2,0); ILACADI C(50,0); VADEGUN N(3,0); RAF C(5,0); ISKONTO1 N(5,2); ISKONTO2 N(5,2); ISKONTO3 N(5,2); ISKONTO4 N(5,2); YMIKTARI N(6,0); YMALFAZLA N(5,0); YCFIYATI N(12,0); YMIAD C(4,0); SONFIYATI N(12,0); FATGRUP C(5,0); HARRECNO N(6,0); PUAN N(9,0); BARKODU C(13,0); TALEP C(1,0); OKUNAN N(6,0); ADETKUTU N(7,0); SEPETKODU C(5,0); REYONSIRA C(1,0); KARSIREYON C(2,0); ILCNOT C(20,0); KURUMISK N(5,2); KUTUTIPI C(1,0); SIRANO N(5,0); RAFORMU C(10,0); BIRIM C(10,0); AMBALAJMIK N(6,0) |
| `SETUP.kontrol2d\data\KONTROL.DBF` | 0 | 5 | 71 | 0,4 | TARIH D(8,0); MODUL C(40,0); SAAT C(8,0); SIRANO N(6,0); USER C(8,0) |
| `SETUP.kontrol2d\data\KONTROLE.DBF` | 1 | 5 | 71 | 0,5 | TARIH D(8,0); MODUL C(40,0); SAAT C(8,0); SIRANO N(6,0); USER C(8,0) |
| `SETUP.kontrol2d\data\KONTROLS.DBF` | 6 | 6 | 151 | 1,4 | TARIH D(8,0); MODUL C(60,0); ISLEM C(60,0); SAAT C(8,0); SIRANO N(6,0); USER C(8,0) |
| `SETUP.kontrol2d\data\LBLSEVK2.DBF` | 1 | 15 | 438 | 1,2 | LBOLGE C(18,0); LSAAT C(60,0); LSEVK C(15,0); LECZANE C(30,0); LECZACI C(61,0); LADRES1 C(40,0); LADRES2 C(40,0); LSEMT C(20,0); LSEHIR C(12,0); LKOD C(7,0); LRENK C(10,0); LSEPET1 C(30,0); LSEPET2 C(30,0); LSAYISAL C(60,0); BARKODPIC G(4,0) |
| `SETUP.kontrol2d\data\sipstruN.DBF` | 0 | 49 | 325 | 1,8 | ECZANEKODU C(10,0); ILACTIPI C(1,0); ISKONTO C(1,0); STATU C(2,0); GRUPKODU C(1,0); ILACKODU N(6,0); FIYATI N(13,0); MIKTARI N(6,0); MALFAZLASI N(5,0); TARIH D(8,0); FATURANO C(7,0); USERNO C(3,0); REYON C(2,0); CFIYATI N(12,0); FIYATNO C(1,0); NETFIYAT C(8,0); MIAD C(4,0); ITHAL C(1,0); ALISKDV N(2,0); CEPNO C(2,0); ILACADI C(50,0); VADEGUN N(3,0); RAF C(5,0); ISKONTO1 N(5,2); ISKONTO2 N(5,2); ISKONTO3 N(5,2); ISKONTO4 N(5,2); YMIKTARI N(6,0); YMALFAZLA N(5,0); YCFIYATI N(12,0); YMIAD C(4,0); SONFIYATI N(12,0); FATGRUP C(5,0); HARRECNO N(6,0); PUAN N(9,0); BARKODU C(13,0); TALEP C(1,0); OKUNAN N(6,0); ADETKUTU N(7,0); SEPETKODU C(5,0); REYONSIRA C(1,0); KARSIREYON C(2,0); ILCNOT C(20,0); KURUMISK N(5,2); KUTUTIPI C(1,0); SIRANO N(5,0); RAFOMRU C(10,0); BIRIM C(10,0); AMBALAJMIK N(6,0) |
| `SETUP.kontrol2d\data\temp2dN.DBF` | 0 | 10 | 162 | 0,6 | ILACKODU N(6,0); BARKOD C(40,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); FISRECNO N(7,0); SONISLEM C(1,0); KUTUTIPI C(1,0); UPAKETKODU C(40,0); OKUTULAN C(20,0) |
| `SETUP.kontrol2d\data\tmpx2dA.DBF` | 0 | 20 | 324 | 0,9 | ILACKODU N(6,0); ILACADI C(30,0); KAREKOD C(70,0); OKUTULAN C(10,0); UPAKETKODU C(40,0); BARKOD C(40,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); FISRECNO N(7,0); LOGRECNO N(9,0); KUTUTIPI C(1,0); SERIMIKTAR N(5,0); SONISLEM C(1,0); ITSSONUC C(1,0); UYARIKODU C(5,0); GLN1 C(13,0); GLN2 C(13,0); BILDIRIMID C(20,0); SEPETKODU C(6,0) |
| `SETUP.kontrol2d\data\tmpx2dN.DBF` | 0 | 23 | 355 | 1 | ILACKODU N(6,0); ILACADI C(30,0); KAREKOD C(70,0); OKUTULAN C(20,0); UPAKETKODU C(20,0); BARKOD C(40,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); FISRECNO N(7,0); LOGRECNO N(10,0); KUTUTIPI C(1,0); SERIMIKTAR N(5,0); SONISLEM C(1,0); ITSSONUC C(1,0); UYARIKODU C(5,0); GLN1 C(13,0); GLN2 C(13,0); BILDIRIMID C(36,0); KOLIDUZEY N(4,0); SEPETKODU C(6,0); DETAYRECNO N(10,0); OZETRECNO N(10,0) |
| `SETUP.kontrol2d\EITEMP.DBF` | 0 | 49 | 324 | 1,6 | ECZANEKODU C(10,0); ILACTIPI C(1,0); ISKONTO C(1,0); STATU C(2,0); GRUPKODU C(1,0); ILACKODU N(6,0); FIYATI N(12,0); MIKTARI N(6,0); MALFAZLASI N(5,0); TARIH D(8,0); FATURANO C(7,0); USERNO C(3,0); REYON C(2,0); CFIYATI N(12,0); FIYATNO C(1,0); NETFIYAT C(8,0); MIAD C(4,0); ITHAL C(1,0); ALISKDV N(2,0); CEPNO C(2,0); ILACADI C(50,0); VADEGUN N(3,0); RAF C(5,0); ISKONTO1 N(5,2); ISKONTO2 N(5,2); ISKONTO3 N(5,2); ISKONTO4 N(5,2); YMIKTARI N(6,0); YMALFAZLA N(5,0); YCFIYATI N(12,0); YMIAD C(4,0); SONFIYATI N(12,0); FATGRUP C(5,0); HARRECNO N(6,0); PUAN N(9,0); BARKODU C(13,0); TALEP C(1,0); OKUNAN N(6,0); ADETKUTU N(7,0); SEPETKODU C(5,0); REYONSIRA C(1,0); KARSIREYON C(2,0); ILCNOT C(20,0); KURUMISK N(5,2); KUTUTIPI C(1,0); SIRANO N(5,0); RAFOMRU C(10,0); BIRIM C(10,0); AMBALAJMIK N(6,0) |
| `SETUP.kontrol2d\EITEMP2D.DBF` | 0 | 5 | 66 | 0,2 | ILACKODU N(6,0); BARKOD C(13,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0) |
| `SETUP.kontrol2d\FOXUSER.DBF` | 22 | 7 | 48 | 1,5 | TYPE C(12,0); ID C(12,0); NAME M(4,0); READONLY L(1,0); CKVAL N(6,0); DATA M(4,0); UPDATED D(8,0) |
| `SETUP.kontrol2d\SIPST2D.DBF` | 0 | 16 | 242 | 0,5 | ILACKODU N(6,0); ILACADI C(50,0); OKUTULAN C(10,0); UPAKETKODU C(40,0); BARKOD C(40,0); SERINO C(20,0); MIAD C(6,0); SIRANO C(20,0); FISRECNO N(7,0); LOGRECNO N(9,0); KUTUTIPI C(1,0); SERIMIKTAR N(5,0); SONISLEM C(1,0); ITSSONUC C(1,0); UYARIKODU C(5,0); BILDIRIMID C(20,0) |
| `SETUP.kontrol2d\sipstru.dbf` | 0 | 49 | 325 | 1,6 | ECZANEKODU C(10,0); ILACTIPI C(1,0); ISKONTO C(1,0); STATU C(2,0); GRUPKODU C(1,0); ILACKODU N(6,0); FIYATI N(13,0); MIKTARI N(6,0); MALFAZLASI N(5,0); TARIH D(8,0); FATURANO C(7,0); USERNO C(3,0); REYON C(2,0); CFIYATI N(12,0); FIYATNO C(1,0); NETFIYAT C(8,0); MIAD C(4,0); ITHAL C(1,0); ALISKDV N(2,0); CEPNO C(2,0); ILACADI C(50,0); VADEGUN N(3,0); RAF C(5,0); ISKONTO1 N(5,2); ISKONTO2 N(5,2); ISKONTO3 N(5,2); ISKONTO4 N(5,2); YMIKTARI N(6,0); YMALFAZLA N(5,0); YCFIYATI N(12,0); YMIAD C(4,0); SONFIYATI N(12,0); FATGRUP C(5,0); HARRECNO N(6,0); PUAN N(9,0); BARKODU C(13,0); TALEP C(1,0); OKUNAN N(6,0); ADETKUTU N(7,0); SEPETKODU C(5,0); REYONSIRA C(1,0); KARSIREYON C(2,0); ILCNOT C(20,0); KURUMISK N(5,2); KUTUTIPI C(1,0); SIRANO N(5,0); RAFOMRU C(10,0); BIRIM C(10,0); AMBALAJMIK N(6,0) |
| `SETUP.OPA\FOXUSER.DBF` | 3 | 7 | 48 | 0,6 | TYPE C(12,0); ID C(12,0); NAME M(4,0); READONLY L(1,0); CKVAL N(6,0); DATA M(4,0); UPDATED D(8,0) |

### 21.6 Hedef .NET/PostgreSQL Tasarimina Etkisi

- SETUP paketleri ana OPA sisteminden bagimsiz gorunse de ayni veri alanina, ayni runtime mantigina ve ayni entegrasyon servislerine bagli yardimci uygulamalardir.
- `efatura.EXE` yeni sistemde `einvoice` modulune; UBL/XML olusturma, gonderim kuyrugu, gelen/gonderilen fatura durumlari, hata kodu/mesaji ve arsiv dosyalari olarak tasinmalidir.
- `its.exe` ve `kontrol2d.exe` yeni sistemde `tracktrace` ve `warehouse` worker servislerine bolunmelidir: token alma, GLN/UTS no, mal alim/satis/iade/devir, PTS paket, karekod dogrulama, barkod okutma, etiket/fatura kontrol.
- `kontrol2d` paketindeki ActiveX bagimliliklari (`IDAutomation`, `MSComm`, `IP*Works`, `PocketSOAP`) modern .NET karsiliklariyla degistirilmelidir; COM registration gerektiren model terk edilmelidir.
- `prnyaz.exe` .NET tabanli ayri yazdirma yoneticisidir. Yeni sistemde print worker/queue servisi olarak tasarlanabilir; XML->HTML/PDF->printer akisi korunmali, ancak kuyruk ve hata loglari merkezi izlenmelidir.
- `CHECKLIST` kucuk fakat ayri dagitilan VFP uygulamasi olarak gorunuyor; yeni sistemde admin/operasyon checklist modulu veya kontrol listesi ekrani olarak birlestirilebilir.
- SETUP paketleri icin deployment modeli merkezi `F:\BATCH` kopyalama yerine CI/CD artifact, versiyonlu paket, otomatik guncelleme ve merkezi loglama seklinde tasarlanmalidir.

## 22. Sonuc

Bu rapor artik sadece dosya listesi degil, mevcut FoxPro/DBF sisteminin yeniden yazilmasi icin teknik ana plan niteligindedir. Yine de sifirdan .NET + PostgreSQL yazilima gecmeden once `C:\OPA\OPA.EXE` kaynak kodu varsa temin edilmeli; yoksa ekran akislarindan ve kullanici senaryolarindan is kurallari dogrulanmalidir. En kritik tasarim kararlari: DBF tablo kopyasini birebir taklit etmemek, PTS/UTS/KK/YKKS verilerini partition etmek, fatura/finans/stok hareketlerini transaction ve audit altina almak, entegrasyonlari worker/outbox/idempotency modeliyle kurmaktir.

