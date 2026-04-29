using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekStokDeposu : IStokDeposu
{
    private readonly ConcurrentDictionary<Guid, StokHareketi> _hareketler = new();

    public Task<IReadOnlyList<StokHareketi>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _hareketler.Values.AsEnumerable();

        if (urunId is not null)
        {
            sorgu = sorgu.Where(hareket => hareket.UrunId == urunId);
        }

        if (!string.IsNullOrWhiteSpace(depoKod))
        {
            var normalizeDepoKod = StokHareketi.NormalizeDepoKod(depoKod);
            sorgu = sorgu.Where(hareket => hareket.DepoKod == normalizeDepoKod);
        }

        var sonuc = sorgu
            .OrderByDescending(hareket => hareket.OlusturmaZamani)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<StokHareketi>>(sonuc);
    }

    public Task<IReadOnlyList<StokHareketi>> UrunHareketleriAsync(Guid urunId, string depoKod, CancellationToken cancellationToken)
    {
        var normalizeDepoKod = StokHareketi.NormalizeDepoKod(depoKod);
        var sonuc = _hareketler.Values
            .Where(hareket => hareket.UrunId == urunId && hareket.DepoKod == normalizeDepoKod)
            .OrderBy(hareket => hareket.OlusturmaZamani)
            .ToArray();

        return Task.FromResult<IReadOnlyList<StokHareketi>>(sonuc);
    }

    public Task EkleAsync(StokHareketi hareket, CancellationToken cancellationToken)
    {
        _hareketler.TryAdd(hareket.Id, hareket);
        return Task.CompletedTask;
    }
}
