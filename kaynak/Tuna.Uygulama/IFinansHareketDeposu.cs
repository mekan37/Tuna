using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IFinansHareketDeposu
{
    Task<IReadOnlyList<FinansHareketi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyList<FinansHareketi>> CariHareketleriAsync(Guid cariHesapId, CancellationToken cancellationToken);

    Task EkleAsync(FinansHareketi hareket, CancellationToken cancellationToken);
}
