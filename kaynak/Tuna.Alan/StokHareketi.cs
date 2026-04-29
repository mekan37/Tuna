namespace Tuna.Alan;

public enum StokHareketTuru
{
    Giris,
    Cikis,
    Duzeltme
}

public sealed record StokHareketi(
    Guid Id,
    Guid UrunId,
    string UrunKod,
    string DepoKod,
    StokHareketTuru Tur,
    decimal Miktar,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani)
{
    public decimal BakiyeEtkisi => Tur switch
    {
        StokHareketTuru.Giris => Miktar,
        StokHareketTuru.Cikis => -Miktar,
        StokHareketTuru.Duzeltme => Miktar,
        _ => 0
    };

    public static StokHareketi Olustur(
        Guid urunId,
        string urunKod,
        string depoKod,
        StokHareketTuru tur,
        decimal miktar,
        string kaynak,
        string? aciklama,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            urunId,
            urunKod.Trim().ToUpperInvariant(),
            NormalizeDepoKod(depoKod),
            tur,
            miktar,
            kaynak.Trim(),
            string.IsNullOrWhiteSpace(aciklama) ? null : aciklama.Trim(),
            olusturmaZamani);

    public static string NormalizeDepoKod(string depoKod) => depoKod.Trim().ToUpperInvariant();
}
