using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class AktarimPlaniSaglayici : IAktarimPlaniSaglayici
{
    public IReadOnlyList<AktarimAsamasi> GetPlan() =>
    [
        new(1, AktarimAsamaTuru.SaltOkunurAnlikKopya, "DBF salt okunur anlik kopya", "Canli FoxPro/DBF veri alanina yazmadan tutarli okuma penceresi hazirlanir.", ["Anlik kopya zamani kayit altinda", "Kaynak kok dizin dogrulandi"]),
        new(2, AktarimAsamaTuru.HamSahneleme, "Ham sahneleme aktarimi", "Her DBF kaydi staging_foxpro.raw_records altina JSON payload olarak aktarilir.", ["Kaynak tablo kayit sayisi", "Sahneleme kayit sayisi"]),
        new(3, AktarimAsamaTuru.TipDonusumu, "Tip donusumu", "C/N/D/L/M FoxPro tipleri trim, numeric, nullable date, boolean ve text kurallariyla parse edilir.", ["Parse hatalari migration_errors tablosunda", "Aktarim parse hatasinda durmaz"]),
        new(4, AktarimAsamaTuru.HedefeNormalizeAktarim, "Normalize hedef aktarimi", "URUNLER, FATURAS, DETAYS, FINANS ve PTS kaynaklari hedef semalara ayrilir.", ["Idempotent tekrar calisma", "Hedef unique constraint ihlali yok"]),
        new(5, AktarimAsamaTuru.Mutabakat, "Mutabakat", "Kayit sayisi, tutar, stok, cari bakiye, UUID ve karekod sayilari karsilastirilir.", ["FATURAS/DETAYS sayisi", "Finans borc/alacak toplamlari", "PTS tekil seri sayisi"]),
        new(6, AktarimAsamaTuru.ParalelCalisma, "Paralel calisma", "Eski raporlarla yeni raporlar ayni donem icin karsilastirilir.", ["Fatura toplam raporu", "Stok bakiye raporu", "Cari ekstre raporu"]),
        new(7, AktarimAsamaTuru.CanliGecis, "Canli gecis", "Eski yazim durdurulur, son delta aktarilir, PostgreSQL yazimi acilir.", ["Son delta mutabik", "Rollback plani onayli"])
    ];
}
