using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IUrunDeposu
{
    Task<IReadOnlyList<Urun>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken);

    Task<Urun?> IdIleBulAsync(Guid id, CancellationToken cancellationToken);

    Task<Urun?> KodIleBulAsync(string kod, CancellationToken cancellationToken);

    Task EkleAsync(Urun urun, CancellationToken cancellationToken);
}
