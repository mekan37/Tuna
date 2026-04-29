namespace Tuna.Alan;

public sealed record CariHesap(
    Guid Id,
    string Kod,
    string Unvan,
    string? VergiNo,
    string? Telefon,
    string? Eposta,
    decimal RiskLimiti,
    bool Aktif,
    DateTimeOffset OlusturmaZamani)
{
    public static CariHesap Olustur(
        string kod,
        string unvan,
        string? vergiNo,
        string? telefon,
        string? eposta,
        decimal riskLimiti,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            NormalizeKod(kod),
            unvan.Trim(),
            string.IsNullOrWhiteSpace(vergiNo) ? null : vergiNo.Trim(),
            string.IsNullOrWhiteSpace(telefon) ? null : telefon.Trim(),
            string.IsNullOrWhiteSpace(eposta) ? null : eposta.Trim().ToLowerInvariant(),
            riskLimiti,
            true,
            olusturmaZamani);

    public static string NormalizeKod(string kod) => kod.Trim().ToUpperInvariant();
}
