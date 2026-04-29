using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed record DenetimKaydiOzeti(
    Guid Id,
    string Modul,
    DenetimIslemTuru IslemTuru,
    string VarlikTuru,
    string VarlikId,
    string Kaynak,
    string? Aciklama,
    DateTimeOffset OlusturmaZamani);

public sealed record DenetimKaydiOlusturIstegi(
    string Modul,
    DenetimIslemTuru IslemTuru,
    string VarlikTuru,
    string VarlikId,
    string Kaynak,
    string? Aciklama);

public static class DenetimDonusumleri
{
    public static DenetimKaydiOzeti Ozetle(this DenetimKaydi kayit) =>
        new(
            kayit.Id,
            kayit.Modul,
            kayit.IslemTuru,
            kayit.VarlikTuru,
            kayit.VarlikId,
            kayit.Kaynak,
            kayit.Aciklama,
            kayit.OlusturmaZamani);
}
