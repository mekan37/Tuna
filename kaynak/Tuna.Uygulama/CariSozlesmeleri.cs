using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record CariHesapOzeti(
    Guid Id,
    string Kod,
    string Unvan,
    string? VergiNo,
    string? Telefon,
    string? Eposta,
    decimal RiskLimiti,
    bool Aktif);

public sealed record CariHesapOlusturIstegi(
    string Kod,
    string Unvan,
    string? VergiNo,
    string? Telefon,
    string? Eposta,
    decimal RiskLimiti);

public static class CariDonusumleri
{
    public static CariHesapOzeti Ozetle(this CariHesap cariHesap) =>
        new(
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            cariHesap.VergiNo,
            cariHesap.Telefon,
            cariHesap.Eposta,
            cariHesap.RiskLimiti,
            cariHesap.Aktif);
}
