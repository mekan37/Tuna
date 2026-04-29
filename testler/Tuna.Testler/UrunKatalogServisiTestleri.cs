using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class UrunKatalogServisiTestleri
{
    [Fact]
    public async Task Urun_olusturma_kodu_normalize_eder()
    {
        var depo = new TestUrunDeposu();
        var servis = new UrunKatalogServisi(depo, TimeProvider.System);

        var sonuc = await servis.OlusturAsync(
            new UrunOlusturIstegi(" abc-123 ", "Test Urunu", null, null, 10),
            CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal("ABC-123", sonuc.Deger!.Kod);
    }

    [Fact]
    public async Task Ayni_urun_kodu_ikinci_kez_eklenemez()
    {
        var depo = new TestUrunDeposu();
        var servis = new UrunKatalogServisi(depo, TimeProvider.System);

        await servis.OlusturAsync(new UrunOlusturIstegi("ABC-123", "Test Urunu", null, null, 10), CancellationToken.None);
        var sonuc = await servis.OlusturAsync(new UrunOlusturIstegi("abc-123", "Baska Urun", null, null, 10), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("urun.kod_tekrar", sonuc.HataKodu);
    }

    [Fact]
    public async Task Urun_listeleme_limit_degerini_uygular()
    {
        var depo = new TestUrunDeposu();
        var servis = new UrunKatalogServisi(depo, TimeProvider.System);

        await servis.OlusturAsync(new UrunOlusturIstegi("A", "A Urunu", null, null, 10), CancellationToken.None);
        await servis.OlusturAsync(new UrunOlusturIstegi("B", "B Urunu", null, null, 10), CancellationToken.None);

        var sonuc = await servis.ListeleAsync(null, 1, CancellationToken.None);

        Assert.Single(sonuc);
    }

    private sealed class TestUrunDeposu : IUrunDeposu
    {
        private readonly List<Urun> _urunler = [];

        public Task<IReadOnlyList<Urun>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
        {
            var sonuc = _urunler
                .Where(urun => arama is null || urun.Kod.Contains(arama, StringComparison.OrdinalIgnoreCase) || urun.Ad.Contains(arama, StringComparison.OrdinalIgnoreCase))
                .OrderBy(urun => urun.Kod)
                .Take(limit)
                .ToArray();

            return Task.FromResult<IReadOnlyList<Urun>>(sonuc);
        }

        public Task<Urun?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_urunler.SingleOrDefault(urun => urun.Id == id));

        public Task<Urun?> KodIleBulAsync(string kod, CancellationToken cancellationToken) =>
            Task.FromResult(_urunler.SingleOrDefault(urun => urun.Kod == Urun.NormalizeKod(kod)));

        public Task EkleAsync(Urun urun, CancellationToken cancellationToken)
        {
            _urunler.Add(urun);
            return Task.CompletedTask;
        }
    }
}
