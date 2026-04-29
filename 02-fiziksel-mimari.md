# Fiziksel Mimari ve Veri Hacmi

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

Mevcut klasor yapisi, batch/dagitim paketi, baslatma mantigi ve veri hacmini aciklar.

## Kaynak Bolumler

- tarama2.md bolum 3
- tarama2.md bolum 4
- tarama2.md bolum 5

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

