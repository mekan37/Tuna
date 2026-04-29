using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record StokBakiyeOzeti(
    Guid UrunId,
    string UrunKod,
    string UrunAd,
    string DepoKod,
    decimal Miktar);

public sealed record StokHareketOzeti(
    Guid Id,
    Guid UrunId,
    string UrunKod,
    string DepoKod,
    StokHareketTuru Tur,
    decimal Miktar,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani);

public sealed record StokHareketOlusturIstegi(
    Guid UrunId,
    string DepoKod,
    StokHareketTuru Tur,
    decimal Miktar,
    string Kaynak,
    string? Aciklama);

public static class StokDonusumleri
{
    public static StokHareketOzeti Ozetle(this StokHareketi hareket) =>
        new(
            hareket.Id,
            hareket.UrunId,
            hareket.UrunKod,
            hareket.DepoKod,
            hareket.Tur,
            hareket.Miktar,
            hareket.Kaynak,
            hareket.Aciklama,
            hareket.OlusturmaZamani);
}
