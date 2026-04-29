# Yonetici Ozeti ve Sinirlar

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

Kanit seviyesi, varsayimlar, sistemin genel islevleri ve yeniden yazim icin temel riskleri tanimlar.

## Kaynak Bolumler

- tarama2.md bolum 1
- tarama2.md bolum 2

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

