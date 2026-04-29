# Hedef Mimari, Veri Modeli ve Yol Haritasi

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

Yeni .NET 10/PostgreSQL mimari kararlarini, servis/ekran taslagini, aktarim stratejisini ve fazlari tanimlar.

## Uygulama Adi

Bu hedef mimariyle kurulacak yeni uygulamanin resmi adi Tuna olacaktir. Mimari paketler, servis adlari, servis basliklari ve varsayilan veritabani isimlendirmeleri bu adi temel almalidir. Kod ve klasor adlari Turkce ASCII olmalidir; i, u, g, s, c, o karakterleri kullanilir.

## Kaynak Bolumler

- tarama2.md bolum 8
- tarama2.md bolum 9
- tarama2.md bolum 10
- tarama2.md bolum 11
- tarama2.md bolum 12
- tarama2.md bolum 13
- tarama2.md bolum 14
- tarama2.md bolum 15

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
| `BEKLEYEN` | `sales.pending_orders` veya `warehouse.work_queue` | Depo/fatura oncesi is kuyrugu |
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
| DBF 2 GB siniri | Veri bozulmasi/durma riski | PostgreSQL partition + buyuk tablo aktarim plani |
| Karakter alanda sayisal veri | Yanlis hesap riski | Parse kurallari ve hata staging tablosu |
| Tarih bazli tablo cogalmasi | Bakim ve sorgu zorlugu | Partitioned event table |
| CDX indeks mantigi bilinmiyor | Performans ve tekillik kaybi | CDX key analizi veya uygulama ekran davranisi ile indeks tasarimi |
| Memo/FPT alanlari | Metin kaybi riski | FPT okuma testi ve text/json hedef alanlari |
| Eszamanli DBF kullanimi | Aktarim sirasinda tutarsiz snapshot | Bakim penceresi veya shadow copy/snapshot |
| Resmi entegrasyonlar | Yasal belge riski | Adapter test ortami, idempotency, durum history |

## 15. Gelistirme Yol Haritasi

### Faz 0 - Kanit Tamamlama
- `C:\OPA\OPA.EXE` ana uygulamasi referans alinir; varsa kaynak kod veya ekran akis kayitlari temin edilir.
- 10-20 kritik ekran icin ekran kaydi alinir: urun, cari, siparis, fatura, alis, tahsilat, e-fatura, karekod, rapor.
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


