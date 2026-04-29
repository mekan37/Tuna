using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class StokServisiTestleri
{
    [Fact]
    public async Task Giris_hareketi_stok_bakiyesini_artirir()
    {
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([urun]);

        var hareket = await servis.HareketOlusturAsync(new StokHareketOlusturIstegi(urun.Id, "ana", StokHareketTuru.Giris, 10, "test", null), CancellationToken.None);
        var bakiye = await servis.BakiyeGetirAsync(urun.Id, "ANA", CancellationToken.None);

        Assert.True(hareket.Basarili);
        Assert.True(bakiye.Basarili);
        Assert.Equal(10, bakiye.Deger!.Miktar);
    }

    [Fact]
    public async Task Cikis_hareketi_stok_bakiyesini_dusurur()
    {
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([urun]);

        await servis.HareketOlusturAsync(new StokHareketOlusturIstegi(urun.Id, "ANA", StokHareketTuru.Giris, 10, "test", null), CancellationToken.None);
        var hareket = await servis.HareketOlusturAsync(new StokHareketOlusturIstegi(urun.Id, "ANA", StokHareketTuru.Cikis, 4, "test", null), CancellationToken.None);
        var bakiye = await servis.BakiyeGetirAsync(urun.Id, "ANA", CancellationToken.None);

        Assert.True(hareket.Basarili);
        Assert.Equal(6, bakiye.Deger!.Miktar);
    }

    [Fact]
    public async Task Yetersiz_stok_cikisi_reddedilir()
    {
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([urun]);

        var hareket = await servis.HareketOlusturAsync(new StokHareketOlusturIstegi(urun.Id, "ANA", StokHareketTuru.Cikis, 1, "test", null), CancellationToken.None);

        Assert.False(hareket.Basarili);
        Assert.Equal("stok.yetersiz_bakiye", hareket.HataKodu);
    }

    [Fact]
    public async Task Bilinmeyen_urun_icin_stok_hareketi_reddedilir()
    {
        var servis = ServisOlustur([]);

        var hareket = await servis.HareketOlusturAsync(new StokHareketOlusturIstegi(Guid.NewGuid(), "ANA", StokHareketTuru.Giris, 1, "test", null), CancellationToken.None);

        Assert.False(hareket.Basarili);
        Assert.Equal("stok.urun_bulunamadi", hareket.HataKodu);
    }

    private static StokServisi ServisOlustur(IReadOnlyList<Urun> urunler)
    {
        var urunDeposu = new TestUrunDeposu(urunler);
        var stokDeposu = new TestStokDeposu();
        return new StokServisi(stokDeposu, urunDeposu, TimeProvider.System);
    }

    private sealed class TestUrunDeposu : IUrunDeposu
    {
        private readonly List<Urun> _urunler;

        public TestUrunDeposu(IReadOnlyList<Urun> urunler)
        {
            _urunler = [.. urunler];
        }

        public Task<IReadOnlyList<Urun>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Urun>>(_urunler.Take(limit).ToArray());

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

    private sealed class TestStokDeposu : IStokDeposu
    {
        private readonly List<StokHareketi> _hareketler = [];

        public Task<IReadOnlyList<StokHareketi>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken)
        {
            var sonuc = _hareketler
                .Where(hareket => urunId is null || hareket.UrunId == urunId)
                .Where(hareket => depoKod is null || hareket.DepoKod == StokHareketi.NormalizeDepoKod(depoKod))
                .Take(limit)
                .ToArray();

            return Task.FromResult<IReadOnlyList<StokHareketi>>(sonuc);
        }

        public Task<IReadOnlyList<StokHareketi>> UrunHareketleriAsync(Guid urunId, string depoKod, CancellationToken cancellationToken)
        {
            var normalizeDepoKod = StokHareketi.NormalizeDepoKod(depoKod);
            var sonuc = _hareketler
                .Where(hareket => hareket.UrunId == urunId && hareket.DepoKod == normalizeDepoKod)
                .ToArray();

            return Task.FromResult<IReadOnlyList<StokHareketi>>(sonuc);
        }

        public Task EkleAsync(StokHareketi hareket, CancellationToken cancellationToken)
        {
            _hareketler.Add(hareket);
            return Task.CompletedTask;
        }
    }
}
