namespace Tuna.Alan;

public enum FinansHareketTuru
{
    SatisFaturasi,
    AlisFaturasi,
    Tahsilat,
    Odeme,
    Duzeltme
}

public sealed record FinansHareketi(
    Guid Id,
    Guid CariHesapId,
    string CariKod,
    string CariUnvan,
    FinansHareketTuru Tur,
    decimal Borc,
    decimal Alacak,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani)
{
    public decimal BakiyeEtkisi => Borc - Alacak;

    public static FinansHareketi Olustur(
        CariHesap cariHesap,
        FinansHareketTuru tur,
        decimal borc,
        decimal alacak,
        string kaynak,
        string? aciklama,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            tur,
            borc,
            alacak,
            kaynak.Trim(),
            string.IsNullOrWhiteSpace(aciklama) ? null : aciklama.Trim(),
            olusturmaZamani);
}
