using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekUrunDeposu : IUrunDeposu
{
    private readonly ConcurrentDictionary<Guid, Urun> _urunler = new();

    public BellekUrunDeposu()
    {
        EkleBaslangicVerisi(Urun.Olustur("ILAC-0001", "Ornek Ilac 500 MG Tablet", "8690000000011", "Ornek Uretici", 10, DateTimeOffset.UtcNow));
        EkleBaslangicVerisi(Urun.Olustur("SARF-0001", "Steril Enjektor 5 ML", "8690000000028", "Medikal Uretici", 20, DateTimeOffset.UtcNow));
    }

    public Task<IReadOnlyList<Urun>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _urunler.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var temizArama = arama.Trim();
            sorgu = sorgu.Where(urun =>
                urun.Kod.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ||
                urun.Ad.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ||
                (urun.Barkod?.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var sonuc = sorgu
            .OrderBy(urun => urun.Kod)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<Urun>>(sonuc);
    }

    public Task<Urun?> IdIleBulAsync(Guid id, CancellationToken cancellationToken)
    {
        _urunler.TryGetValue(id, out var urun);
        return Task.FromResult(urun);
    }

    public Task<Urun?> KodIleBulAsync(string kod, CancellationToken cancellationToken)
    {
        var normalizeKod = Urun.NormalizeKod(kod);
        var urun = _urunler.Values.SingleOrDefault(item => item.Kod == normalizeKod);
        return Task.FromResult(urun);
    }

    public Task EkleAsync(Urun urun, CancellationToken cancellationToken)
    {
        _urunler.TryAdd(urun.Id, urun);
        return Task.CompletedTask;
    }

    private void EkleBaslangicVerisi(Urun urun) => _urunler.TryAdd(urun.Id, urun);
}
