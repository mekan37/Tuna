using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekFinansHareketDeposu : IFinansHareketDeposu
{
    private readonly ConcurrentDictionary<Guid, FinansHareketi> _hareketler = new();

    public Task<IReadOnlyList<FinansHareketi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _hareketler.Values.AsEnumerable();

        if (cariHesapId is not null)
        {
            sorgu = sorgu.Where(hareket => hareket.CariHesapId == cariHesapId);
        }

        var sonuc = sorgu
            .OrderByDescending(hareket => hareket.OlusturmaZamani)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<FinansHareketi>>(sonuc);
    }

    public Task<IReadOnlyList<FinansHareketi>> CariHareketleriAsync(Guid cariHesapId, CancellationToken cancellationToken)
    {
        var sonuc = _hareketler.Values
            .Where(hareket => hareket.CariHesapId == cariHesapId)
            .OrderBy(hareket => hareket.OlusturmaZamani)
            .ToArray();

        return Task.FromResult<IReadOnlyList<FinansHareketi>>(sonuc);
    }

    public Task EkleAsync(FinansHareketi hareket, CancellationToken cancellationToken)
    {
        _hareketler.TryAdd(hareket.Id, hareket);
        return Task.CompletedTask;
    }
}
