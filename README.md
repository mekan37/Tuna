# Tuna - FoxPro/DBF Depo ERP Yeniden Yazim Referans Seti

Bu klasor, `tarama2.md` raporunu Codex ve Claude gibi ajanlarin daha net okuyup referanslayabilmesi icin bolunmus Markdown dosyalarindan olusur. Amac, mevcut FoxPro/DBF tabanli depo ERP sisteminin sifirdan .NET + PostgreSQL mimarisiyle kurulmasina teknik temel saglamaktir.
## Uygulama Adi

Yeni yazilacak uygulamanin resmi adi Tuna olacaktir. Kod, dokuman, proje adlari, servis basliklari, veritabani varsayilan adlandirmalari ve ajan ciktisi bu adi esas almalidir. Proje ve klasor adlari Turkce ASCII olmalidir; i, u, g, s, c, o karakterleri kullanilir.


## Dosya Haritasi

| Dosya | Rol |
|---|---|
| `00-kurulum-ve-ajan-rehberi.md` | Ajan okuma sirasi, kurallar, hedef kurulum iskeleti |
| `01-yonetici-ozeti-ve-sinirlar.md` | Kanit seviyesi, sinirlar, yonetici ozeti |
| `02-fiziksel-mimari.md` | Klasorler, batch paketi, baslatma mantigi, veri hacmi |
| `03-moduller-ve-is-akislari.md` | ERP modulleri ve ana is akislarinin kanitlari |
| `04-hedef-mimari-veri-modeli.md` | .NET 10 mimari, PostgreSQL model, servis/ekran, aktarim, yol haritasi |
| `05-dbf-envanteri.md` | En buyuk tablolar ve tum DBF tablo envanteri |
| `06-ana-dbf-semalari.md` | Ana DBF tablo semalari |
| `07-opa-menu-analizi.md` | C:\OPA ana uygulama, menu, sekme ve ekran izleri |
| `08-setup-paketleri.md` | F:\BATCH SETUP paketleri, yardimci uygulamalar ve bagimliliklar |
| `09-sonuc.md` | Sonuc ve kritik tasarim kararlari |
| `tarama2.md` | Orijinal tam rapor kopyasi |

## Referanslama Standardi

Yeni kod veya mimari dokuman yazarken dosya adini ve ilgili bolum basligini birlikte kullan. Ornek: `04-hedef-mimari-veri-modeli.md > 9. PostgreSQL Hedef Veri Modeli`.

## Oncelikli Uygulama Sirasi

1. Veri sozlugu ve staging migration tasarimi.
2. Core, stok, cari, satis, alis ve finans modulleri.
3. E-fatura/e-arsiv ve ITS/UTS/PTS worker servisleri.
4. Yazdirma, raporlama, audit ve mutabakat katmani.
5. Paralel calisma, veri karsilastirma ve canli gecis.

## Kritik Ilke

Yeni sistem, DBF dosya yapisinin kopyasi olmamalidir. Rapor, eski sistemi anlamak icin kaynak; hedef tasarim ise tutarli transaction, audit, partition, worker ve idempotent entegrasyon prensipleriyle kurulmalidir.



