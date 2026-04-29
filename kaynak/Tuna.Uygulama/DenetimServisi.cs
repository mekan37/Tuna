using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class DenetimServisi
{
    private readonly IDenetimKayitDeposu _denetimKayitDeposu;
    private readonly TimeProvider _timeProvider;

    public DenetimServisi(IDenetimKayitDeposu denetimKayitDeposu, TimeProvider timeProvider)
    {
        _denetimKayitDeposu = denetimKayitDeposu;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<DenetimKaydiOzeti>> ListeleAsync(string? modul, string? varlikTuru, string? varlikId, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 200);
        var kayitlar = await _denetimKayitDeposu.ListeleAsync(modul, varlikTuru, varlikId, guvenliLimit, cancellationToken);
        return kayitlar.Select(kayit => kayit.Ozetle()).ToArray();
    }

    public async Task<DenetimKaydiOzeti> KaydetAsync(DenetimKaydiOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var kayit = DenetimKaydi.Olustur(
            istek.Modul,
            istek.IslemTuru,
            istek.VarlikTuru,
            istek.VarlikId,
            istek.Kaynak,
            istek.Aciklama,
            _timeProvider.GetUtcNow());

        await _denetimKayitDeposu.EkleAsync(kayit, cancellationToken);
        return kayit.Ozetle();
    }
}
