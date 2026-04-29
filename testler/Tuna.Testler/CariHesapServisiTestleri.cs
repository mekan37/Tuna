using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class CariHesapServisiTestleri
{
    [Fact]
    public async Task Cari_hesap_olusturma_kodu_ve_epostayi_normalize_eder()
    {
        var depo = new TestCariHesapDeposu();
        var servis = new CariHesapServisi(depo, TimeProvider.System);

        var sonuc = await servis.OlusturAsync(
            new CariHesapOlusturIstegi(" mus-001 ", "Test Musteri", "1234567890", null, "INFO@EXAMPLE.COM", 1000),
            CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal("MUS-001", sonuc.Deger!.Kod);
        Assert.Equal("info@example.com", sonuc.Deger.Eposta);
    }

    [Fact]
    public async Task Ayni_cari_kod_ikinci_kez_eklenemez()
    {
        var depo = new TestCariHesapDeposu();
        var servis = new CariHesapServisi(depo, TimeProvider.System);

        await servis.OlusturAsync(new CariHesapOlusturIstegi("MUS-001", "Test Musteri", null, null, null, 1000), CancellationToken.None);
        var sonuc = await servis.OlusturAsync(new CariHesapOlusturIstegi("mus-001", "Baska Musteri", null, null, null, 1000), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("cari.kod_tekrar", sonuc.HataKodu);
    }

    [Fact]
    public async Task Negatif_risk_limiti_reddedilir()
    {
        var depo = new TestCariHesapDeposu();
        var servis = new CariHesapServisi(depo, TimeProvider.System);

        var sonuc = await servis.OlusturAsync(new CariHesapOlusturIstegi("MUS-001", "Test Musteri", null, null, null, -1), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("cari.gecersiz", sonuc.HataKodu);
    }

    [Fact]
    public async Task Vergi_no_uzunlugu_10_veya_11_olmali()
    {
        var depo = new TestCariHesapDeposu();
        var servis = new CariHesapServisi(depo, TimeProvider.System);

        var sonuc = await servis.OlusturAsync(new CariHesapOlusturIstegi("MUS-001", "Test Musteri", "123", null, null, 0), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("cari.gecersiz", sonuc.HataKodu);
    }

    private sealed class TestCariHesapDeposu : ICariHesapDeposu
    {
        private readonly List<CariHesap> _hesaplar = [];

        public Task<IReadOnlyList<CariHesap>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
        {
            var sonuc = _hesaplar
                .Where(cariHesap => arama is null || cariHesap.Kod.Contains(arama, StringComparison.OrdinalIgnoreCase) || cariHesap.Unvan.Contains(arama, StringComparison.OrdinalIgnoreCase))
                .OrderBy(cariHesap => cariHesap.Kod)
                .Take(limit)
                .ToArray();

            return Task.FromResult<IReadOnlyList<CariHesap>>(sonuc);
        }

        public Task<CariHesap?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cariHesap => cariHesap.Id == id));

        public Task<CariHesap?> KodIleBulAsync(string kod, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cariHesap => cariHesap.Kod == CariHesap.NormalizeKod(kod)));

        public Task EkleAsync(CariHesap cariHesap, CancellationToken cancellationToken)
        {
            _hesaplar.Add(cariHesap);
            return Task.CompletedTask;
        }
    }
}
