using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IDenetimKayitDeposu
{
    Task<IReadOnlyList<DenetimKaydi>> ListeleAsync(string? modul, string? varlikTuru, string? varlikId, int limit, CancellationToken cancellationToken);

    Task EkleAsync(DenetimKaydi kayit, CancellationToken cancellationToken);
}
