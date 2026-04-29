using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class ModulKatalogu : IModulKatalogu
{
    private static readonly ReferansKaniti TargetModel = new(
        "04-hedef-mimari-veri-modeli.md",
        "9. PostgreSQL Hedef Veri Modeli",
        KanitSeviyesi.Kesin,
        "Hedef sema ve modul ayrimi referans alindi.");

    public IReadOnlyList<ModulTanimi> GetModules() =>
    [
        Module(UygulamaModulu.Cekirdek, "core", "Sistem", "Firma, sube, kullanici, yetki, parametre ve belge numaralari.", ["FIRMA", "BELGENO", "KONTKULL", "nodelist"]),
        Module(UygulamaModulu.Katalog, "catalog", "Urun Katalogu", "Urun, barkod, uretici, fiyat listesi, muadil ve kampanya temelleri.", ["URUNLER", "BARKOD", "URETICI", "MUADIL", "KAMPANYA"]),
        Module(UygulamaModulu.Stok, "inventory", "Stok", "Stok hareketi, parti, miad, sayim, depo ve reyon verileri.", ["STKZAMAN", "PARTILER", "MIADHRK", "REYONKxx", "ONSAYIM"]),
        Module(UygulamaModulu.Satis, "sales", "Satis", "Siparis, satis faturasi, iade, irsaliye ve bekleyen isler.", ["SIPARIS", "SIPHAR", "FATURAS", "DETAYS", "BEKLEYEN"]),
        Module(UygulamaModulu.Alis, "purchase", "Alis", "Alis faturalari, tedarikci hareketleri ve maliyet etkileri.", ["FATURAG", "DETAYG", "KURUMAKT"]),
        Module(UygulamaModulu.Finans, "finance", "Finans", "Cari hareket defteri, tahsilat, kredi karti, cek ve senet.", ["FINANS", "CARIOZT", "KK*", "SYMFTR", "SYMHAR"]),
        Module(UygulamaModulu.EFatura, "einvoice", "E-Fatura", "E-fatura/e-arsiv belge, mukellef ve durum gecmisi.", ["EFATURA", "EARSIV", "EFATKULL", "EFTRKULL"]),
        Module(UygulamaModulu.Izleme, "tracktrace", "ITS/UTS/PTS", "Karekod, seri, lot, paket ve servis loglari.", ["PTS*", "UTS*", "KK*", "YKKS*", "UTSS*"]),
        Module(UygulamaModulu.Denetim, "audit", "Denetim", "Islem, hata, kilit, bildirim, entegrasyon loglari ve outbox.", ["GIRISLOG", "KILITLOG", "MESAJLOG", "MAILLOG", "SMSLOG"]),
        Module(UygulamaModulu.Raporlama, "reporting", "Raporlama", "Rapor tanimlari, cache ve materialized view altyapisi.", ["RAPORTNM.*", "FATURADOKUM"]),
        Module(UygulamaModulu.Aktarim, "staging_foxpro", "Aktarim", "DBF ham staging, hata kaydi ve mutabakat akisinin izlenmesi.", ["Tum DBF envanteri"])
    ];

    private static ModulTanimi Module(
        UygulamaModulu module,
        string schema,
        string displayName,
        string responsibility,
        IReadOnlyList<string> legacySources) =>
        new(module, schema, displayName, responsibility, legacySources, [TargetModel]);
}
