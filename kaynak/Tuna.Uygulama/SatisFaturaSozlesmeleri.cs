using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record SatisFaturaSatirOlusturIstegi(
    Guid UrunId,
    decimal Miktar,
    decimal BirimFiyat);

public sealed record SatisFaturaOlusturIstegi(
    string FaturaNo,
    Guid CariHesapId,
    string DepoKod,
    IReadOnlyList<SatisFaturaSatirOlusturIstegi> Satirlar);

public sealed record SatisFaturaSatirOzeti(
    Guid Id,
    Guid UrunId,
    string UrunKod,
    string UrunAd,
    decimal Miktar,
    decimal BirimFiyat,
    decimal KdvOrani,
    decimal NetTutar,
    decimal KdvTutar,
    decimal GenelTutar);

public sealed record SatisFaturaOzeti(
    Guid Id,
    string FaturaNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    SatisFaturaDurumu Durum,
    decimal NetTutar,
    decimal KdvTutar,
    decimal GenelTutar,
    IReadOnlyList<SatisFaturaSatirOzeti> Satirlar,
    DateTimeOffset OlusturmaZamani);

public static class SatisFaturaDonusumleri
{
    public static SatisFaturaOzeti Ozetle(this SatisFaturasi fatura) =>
        new(
            fatura.Id,
            fatura.FaturaNo,
            fatura.CariHesapId,
            fatura.CariKod,
            fatura.CariUnvan,
            fatura.DepoKod,
            fatura.Durum,
            fatura.NetTutar,
            fatura.KdvTutar,
            fatura.GenelTutar,
            fatura.Satirlar.Select(satir => new SatisFaturaSatirOzeti(
                satir.Id,
                satir.UrunId,
                satir.UrunKod,
                satir.UrunAd,
                satir.Miktar,
                satir.BirimFiyat,
                satir.KdvOrani,
                satir.NetTutar,
                satir.KdvTutar,
                satir.GenelTutar)).ToArray(),
            fatura.OlusturmaZamani);
}
