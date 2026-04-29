using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record FinansHareketOzeti(
    Guid Id,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    FinansHareketTuru Tur,
    decimal Borc,
    decimal Alacak,
    decimal BakiyeEtkisi,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani);

public sealed record CariBakiyeOzeti(
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    decimal BorcToplam,
    decimal AlacakToplam,
    decimal Bakiye);

public sealed record FinansHareketOlusturIstegi(
    Guid CariHesapId,
    decimal Tutar,
    string Kaynak,
    string? Aciklama);

public static class FinansDonusumleri
{
    public static FinansHareketOzeti Ozetle(this FinansHareketi hareket) =>
        new(
            hareket.Id,
            hareket.CariHesapId,
            hareket.CariKod,
            hareket.CariUnvan,
            hareket.Tur,
            hareket.Borc,
            hareket.Alacak,
            hareket.BakiyeEtkisi,
            hareket.Kaynak,
            hareket.Aciklama,
            hareket.OlusturmaZamani);
}
