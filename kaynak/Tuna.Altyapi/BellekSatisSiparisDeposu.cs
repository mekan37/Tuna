using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekSatisSiparisDeposu : ISatisSiparisDeposu
{
    private readonly ConcurrentDictionary<Guid, SatisSiparisi> _siparisler = new();

    public Task<IReadOnlyList<SatisSiparisi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _siparisler.Values.AsEnumerable();

        if (cariHesapId is not null)
        {
            sorgu = sorgu.Where(siparis => siparis.CariHesapId == cariHesapId);
        }

        var sonuc = sorgu
            .OrderByDescending(siparis => siparis.OlusturmaZamani)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<SatisSiparisi>>(sonuc);
    }

    public Task<SatisSiparisi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken)
    {
        _siparisler.TryGetValue(id, out var siparis);
        return Task.FromResult(siparis);
    }

    public Task<SatisSiparisi?> SiparisNoIleBulAsync(string siparisNo, CancellationToken cancellationToken)
    {
        var normalizeSiparisNo = siparisNo.Trim().ToUpperInvariant();
        var siparis = _siparisler.Values.SingleOrDefault(item => item.SiparisNo == normalizeSiparisNo);
        return Task.FromResult(siparis);
    }

    public Task EkleAsync(SatisSiparisi siparis, CancellationToken cancellationToken)
    {
        _siparisler.TryAdd(siparis.Id, siparis);
        return Task.CompletedTask;
    }
}
