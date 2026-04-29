using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record SatisSiparisSatirOlusturIstegi(
    Guid UrunId,
    decimal Miktar,
    decimal BirimFiyat);

public sealed record SatisSiparisOlusturIstegi(
    string SiparisNo,
    Guid CariHesapId,
    string DepoKod,
    IReadOnlyList<SatisSiparisSatirOlusturIstegi> Satirlar);

public sealed record SatisSiparisSatirOzeti(
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

public sealed record SatisSiparisOzeti(
    Guid Id,
    string SiparisNo,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    string DepoKod,
    SatisSiparisDurumu Durum,
    decimal NetTutar,
    decimal KdvTutar,
    decimal GenelTutar,
    IReadOnlyList<SatisSiparisSatirOzeti> Satirlar,
    DateTimeOffset OlusturmaZamani);

public static class SatisDonusumleri
{
    public static SatisSiparisOzeti Ozetle(this SatisSiparisi siparis) =>
        new(
            siparis.Id,
            siparis.SiparisNo,
            siparis.CariHesapId,
            siparis.CariKod,
            siparis.CariUnvan,
            siparis.DepoKod,
            siparis.Durum,
            siparis.NetTutar,
            siparis.KdvTutar,
            siparis.GenelTutar,
            siparis.Satirlar.Select(satir => new SatisSiparisSatirOzeti(
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
            siparis.OlusturmaZamani);
}
