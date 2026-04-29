# SETUP Paketleri ve Yardimci Uygulamalar

> Kaynak: C:\Users\Mustafa\Desktop\tarama2.md. Bu dosya, Codex/Claude tarafindan referans alinacak bolunmus dokuman setinin parcasidir.

## Amac

F:\BATCH SETUP paketleri, yardimci EXE dosyalari, bagimliliklar ve yeni sistemdeki karsiliklarini aciklar.

## Kaynak Bolumler

- tarama2.md bolum 21

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

