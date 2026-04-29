# OPA Uygulama ve Menu Analizi

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

C:\OPA uygulama paketi, baslatma akisi, ana sekmeler, menu/form izleri ve mimari etkileri kapsar.

## Kaynak Bolumler

- tarama2.md bolum 20

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

