namespace Tuna.Alan;

public enum DenetimIslemTuru
{
    Olusturma,
    Guncelleme,
    Iptal,
    Sistem
}

public sealed record DenetimKaydi(
    Guid Id,
    string Modul,
    DenetimIslemTuru IslemTuru,
    string VarlikTuru,
    string VarlikId,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani)
{
    public static DenetimKaydi Olustur(
        string modul,
        DenetimIslemTuru islemTuru,
        string varlikTuru,
        string varlikId,
        string kaynak,
        string? aciklama,
        DateTimeOffset olusturmaZamani) =>
        new(
            Guid.NewGuid(),
            modul.Trim(),
            islemTuru,
            varlikTuru.Trim(),
            varlikId.Trim(),
            kaynak.Trim(),
            string.IsNullOrWhiteSpace(aciklama) ? null : aciklama.Trim(),
            olusturmaZamani);
}
