namespace Tuna.Alan;

public sealed record Urun(
    Guid Id,
    string Kod,
    string Ad,
    string? Barkod,
    string? Uretici,
    decimal KdvOrani,
    bool Aktif,
    DateTimeOffset OlusturmaZamani)
{
    public static Urun Olustur(
        string kod,
        string ad,
        string? barkod,
        string? uretici,
        decimal kdvOrani,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            NormalizeKod(kod),
            ad.Trim(),
            string.IsNullOrWhiteSpace(barkod) ? null : barkod.Trim(),
            string.IsNullOrWhiteSpace(uretici) ? null : uretici.Trim(),
            kdvOrani,
            true,
            olusturmaZamani);

    public static string NormalizeKod(string kod) => kod.Trim().ToUpperInvariant();
}
