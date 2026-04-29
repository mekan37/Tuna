using Tuna.Alan;

namespace Tuna.Uygulama;

public interface ISatisSiparisDeposu
{
    Task<IReadOnlyList<SatisSiparisi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken);

    Task<SatisSiparisi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken);

    Task<SatisSiparisi?> SiparisNoIleBulAsync(string siparisNo, CancellationToken cancellationToken);

    Task EkleAsync(SatisSiparisi siparis, CancellationToken cancellationToken);
}
