using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekDenetimKayitDeposu : IDenetimKayitDeposu
{
    private readonly ConcurrentDictionary<Guid, DenetimKaydi> _kayitlar = new();

    public Task<IReadOnlyList<DenetimKaydi>> ListeleAsync(string? modul, string? varlikTuru, string? varlikId, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _kayitlar.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(modul))
        {
            sorgu = sorgu.Where(kayit => kayit.Modul.Equals(modul.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(varlikTuru))
        {
            sorgu = sorgu.Where(kayit => kayit.VarlikTuru.Equals(varlikTuru.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(varlikId))
        {
            sorgu = sorgu.Where(kayit => kayit.VarlikId.Equals(varlikId.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var sonuc = sorgu
            .OrderByDescending(kayit => kayit.OlusturmaZamani)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<DenetimKaydi>>(sonuc);
    }

    public Task EkleAsync(DenetimKaydi kayit, CancellationToken cancellationToken)
    {
        _kayitlar.TryAdd(kayit.Id, kayit);
        return Task.CompletedTask;
    }
}
