using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekCariHesapDeposu : ICariHesapDeposu
{
    private readonly ConcurrentDictionary<Guid, CariHesap> _hesaplar = new();

    public BellekCariHesapDeposu()
    {
        EkleBaslangicVerisi(CariHesap.Olustur("MUS-0001", "Ornek Musteri Ecza Deposu", "1234567890", "02120000000", "musteri@example.com", 250000, DateTimeOffset.UtcNow));
        EkleBaslangicVerisi(CariHesap.Olustur("TED-0001", "Ornek Tedarikci Medikal", "12345678901", "03120000000", "tedarikci@example.com", 0, DateTimeOffset.UtcNow));
    }

    public Task<IReadOnlyList<CariHesap>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
    {
        var sorgu = _hesaplar.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var temizArama = arama.Trim();
            sorgu = sorgu.Where(cariHesap =>
                cariHesap.Kod.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ||
                cariHesap.Unvan.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ||
                (cariHesap.VergiNo?.Contains(temizArama, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var sonuc = sorgu
            .OrderBy(cariHesap => cariHesap.Kod)
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<CariHesap>>(sonuc);
    }

    public Task<CariHesap?> IdIleBulAsync(Guid id, CancellationToken cancellationToken)
    {
        _hesaplar.TryGetValue(id, out var cariHesap);
        return Task.FromResult(cariHesap);
    }

    public Task<CariHesap?> KodIleBulAsync(string kod, CancellationToken cancellationToken)
    {
        var normalizeKod = CariHesap.NormalizeKod(kod);
        var cariHesap = _hesaplar.Values.SingleOrDefault(item => item.Kod == normalizeKod);
        return Task.FromResult(cariHesap);
    }

    public Task EkleAsync(CariHesap cariHesap, CancellationToken cancellationToken)
    {
        _hesaplar.TryAdd(cariHesap.Id, cariHesap);
        return Task.CompletedTask;
    }

    private void EkleBaslangicVerisi(CariHesap cariHesap) => _hesaplar.TryAdd(cariHesap.Id, cariHesap);
}
