using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IStokDeposu
{
    Task<IReadOnlyList<StokHareketi>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyList<StokHareketi>> UrunHareketleriAsync(Guid urunId, string depoKod, CancellationToken cancellationToken);

    Task EkleAsync(StokHareketi hareket, CancellationToken cancellationToken);
}
