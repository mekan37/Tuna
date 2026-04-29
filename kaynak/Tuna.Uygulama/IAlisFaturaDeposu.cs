using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IAlisFaturaDeposu
{
    Task<IReadOnlyList<AlisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken);

    Task<AlisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken);

    Task<AlisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken);

    Task EkleAsync(AlisFaturasi fatura, CancellationToken cancellationToken);
}
