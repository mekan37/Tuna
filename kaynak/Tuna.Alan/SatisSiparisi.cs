namespace Tuna.Alan;

public enum SatisSiparisDurumu
{
    Taslak,
    Onaylandi,
    Iptal
}

public sealed record SatisSiparisSatiri(
    Guid Id,
    Guid UrunId,
    string UrunKod,
    string UrunAd,
    decimal Miktar,
    decimal BirimFiyat,
    decimal KdvOrani)
{
    public decimal NetTutar => decimal.Round(Miktar * BirimFiyat, 2, MidpointRounding.AwayFromZero);

    public decimal KdvTutar => decimal.Round(NetTutar * KdvOrani / 100, 2, MidpointRounding.AwayFromZero);

    public decimal GenelTutar => NetTutar + KdvTutar;
}

public sealed record SatisSiparisi(
    Guid Id,
    string SiparisNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    SatisSiparisDurumu Durum,
    IReadOnlyList<SatisSiparisSatiri> Satirlar,
    DateTimeOffset OlusturmaZamani)
{
    public decimal NetTutar => Satirlar.Sum(satir => satir.NetTutar);

    public decimal KdvTutar => Satirlar.Sum(satir => satir.KdvTutar);

    public decimal GenelTutar => Satirlar.Sum(satir => satir.GenelTutar);

    public static SatisSiparisi Olustur(
        string siparisNo,
        CariHesap cariHesap,
        string depoKod,
        IReadOnlyList<SatisSiparisSatiri> satirlar,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            siparisNo.Trim().ToUpperInvariant(),
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            StokHareketi.NormalizeDepoKod(depoKod),
            SatisSiparisDurumu.Onaylandi,
            satirlar,
            olusturmaZamani);
}
