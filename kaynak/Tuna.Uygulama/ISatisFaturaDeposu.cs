using Tuna.Alan;

namespace Tuna.Uygulama;

public interface ISatisFaturaDeposu
{
    Task<IReadOnlyList<SatisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken);

    Task<SatisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken);

    Task<SatisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken);

    Task EkleAsync(SatisFaturasi fatura, CancellationToken cancellationToken);
}
