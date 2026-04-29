using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record UrunOzeti(
    Guid Id,
    string Kod,
    string Ad,
    string? Barkod,
    string? Uretici,
    decimal KdvOrani,
    bool Aktif);

public sealed record UrunOlusturIstegi(
    string Kod,
    string Ad,
    string? Barkod,
    string? Uretici,
    decimal KdvOrani);

public static class UrunDonusumleri
{
    public static UrunOzeti Ozetle(this Urun urun) =>
        new(urun.Id, urun.Kod, urun.Ad, urun.Barkod, urun.Uretici, urun.KdvOrani, urun.Aktif);
}
