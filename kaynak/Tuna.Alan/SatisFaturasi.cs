namespace Tuna.Alan;

public enum SatisFaturaDurumu
{
    Onaylandi,
    Iptal
}

public sealed record SatisFaturaSatiri(
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

public sealed record SatisFaturasi(
    Guid Id,
    string FaturaNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    SatisFaturaDurumu Durum,
    IReadOnlyList<SatisFaturaSatiri> Satirlar,
    DateTimeOffset OlusturmaZamani)
{
    public decimal NetTutar => Satirlar.Sum(satir => satir.NetTutar);

    public decimal KdvTutar => Satirlar.Sum(satir => satir.KdvTutar);

    public decimal GenelTutar => Satirlar.Sum(satir => satir.GenelTutar);

    public static SatisFaturasi Olustur(
        string faturaNo,
        CariHesap cariHesap,
        string depoKod,
        IReadOnlyList<SatisFaturaSatiri> satirlar,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            faturaNo.Trim().ToUpperInvariant(),
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            StokHareketi.NormalizeDepoKod(depoKod),
            SatisFaturaDurumu.Onaylandi,
            satirlar,
            olusturmaZamani);
}
