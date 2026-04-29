namespace Tuna.Alan;

public enum AlisFaturaDurumu
{
    Onaylandi,
    Iptal
}

public sealed record AlisFaturaSatiri(
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

public sealed record AlisFaturasi(
    Guid Id,
    string FaturaNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    AlisFaturaDurumu Durum,
    IReadOnlyList<AlisFaturaSatiri> Satirlar,
    DateTimeOffset OlusturmaZamani)
{
    public decimal NetTutar => Satirlar.Sum(satir => satir.NetTutar);

    public decimal KdvTutar => Satirlar.Sum(satir => satir.KdvTutar);

    public decimal GenelTutar => Satirlar.Sum(satir => satir.GenelTutar);

    public static AlisFaturasi Olustur(
        string faturaNo,
        CariHesap cariHesap,
        string depoKod,
        IReadOnlyList<AlisFaturaSatiri> satirlar,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            faturaNo.Trim().ToUpperInvariant(),
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            StokHareketi.NormalizeDepoKod(depoKod),
            AlisFaturaDurumu.Onaylandi,
            satirlar,
            olusturmaZamani);
}
