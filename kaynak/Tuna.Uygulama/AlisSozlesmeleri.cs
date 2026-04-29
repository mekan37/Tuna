using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record AlisFaturaSatirOlusturIstegi(
    Guid UrunId,
    decimal Miktar,
    decimal BirimFiyat);

public sealed record AlisFaturaOlusturIstegi(
    string FaturaNo,
    Guid CariHesapId,
    string DepoKod,
    IReadOnlyList<AlisFaturaSatirOlusturIstegi> Satirlar);

public sealed record AlisFaturaSatirOzeti(
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

public sealed record AlisFaturaOzeti(
    Guid Id,
    string FaturaNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    AlisFaturaDurumu Durum,
    decimal NetTutar,
    decimal KdvTutar,
    decimal GenelTutar,
    IReadOnlyList<AlisFaturaSatirOzeti> Satirlar,
    DateTimeOffset OlusturmaZamani);

public static class AlisDonusumleri
{
    public static AlisFaturaOzeti Ozetle(this AlisFaturasi fatura) =>
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
            fatura.Satirlar.Select(satir => new AlisFaturaSatirOzeti(
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
