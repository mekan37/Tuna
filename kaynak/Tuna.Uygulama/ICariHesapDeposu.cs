using Tuna.Alan;

namespace Tuna.Uygulama;

public interface ICariHesapDeposu
{
    Task<IReadOnlyList<CariHesap>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken);

    Task<CariHesap?> IdIleBulAsync(Guid id, CancellationToken cancellationToken);

    Task<CariHesap?> KodIleBulAsync(string kod, CancellationToken cancellationToken);

    Task EkleAsync(CariHesap cariHesap, CancellationToken cancellationToken);
}
