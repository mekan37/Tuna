using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekSatisFaturaDeposu : ISatisFaturaDeposu
{
    private readonly ConcurrentDictionary<Guid, SatisFaturasi> _faturalar = new();

    public Task<IReadOnlyList<SatisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _faturalar.Values.AsEnumerable();

        if (cariHesapId is not null)
        {
            sorgu = sorgu.Where(fatura => fatura.CariHesapId == cariHesapId);
        }

        var sonuc = sorgu
            .OrderByDescending(fatura => fatura.OlusturmaZamani)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<SatisFaturasi>>(sonuc);
    }

    public Task<SatisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken)
    {
        _faturalar.TryGetValue(id, out var fatura);
        return Task.FromResult(fatura);
    }

    public Task<SatisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken)
    {
        var normalizeFaturaNo = faturaNo.Trim().ToUpperInvariant();
        var fatura = _faturalar.Values.SingleOrDefault(item => item.FaturaNo == normalizeFaturaNo);
        return Task.FromResult(fatura);
    }

    public Task EkleAsync(SatisFaturasi fatura, CancellationToken cancellationToken)
    {
        _faturalar.TryAdd(fatura.Id, fatura);
        return Task.CompletedTask;
    }
}
