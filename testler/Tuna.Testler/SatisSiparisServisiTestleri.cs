using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class SatisSiparisServisiTestleri
{
    [Fact]
    public async Task Satis_siparisi_toplamlari_hesaplar()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([cari], [urun], [StokGiris(urun, 10)]);

        var sonuc = await servis.OlusturAsync(new SatisSiparisOlusturIstegi(
            "sip-001",
            cari.Id,
            "ANA",
            [new SatisSiparisSatirOlusturIstegi(urun.Id, 2, 100)]), CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal("SIP-001", sonuc.Deger!.SiparisNo);
        Assert.Equal(200, sonuc.Deger.NetTutar);
        Assert.Equal(20, sonuc.Deger.KdvTutar);
        Assert.Equal(220, sonuc.Deger.GenelTutar);
    }

    [Fact]
    public async Task Stok_yetersizse_satis_siparisi_olusturulamaz()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([cari], [urun], [StokGiris(urun, 1)]);

        var sonuc = await servis.OlusturAsync(new SatisSiparisOlusturIstegi(
            "SIP-001",
            cari.Id,
            "ANA",
            [new SatisSiparisSatirOlusturIstegi(urun.Id, 2, 100)]), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("satis.stok_yetersiz", sonuc.HataKodu);
    }

    [Fact]
    public async Task Ayni_siparis_no_ikinci_kez_kullanilamaz()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([cari], [urun], [StokGiris(urun, 10)]);
        var istek = new SatisSiparisOlusturIstegi("SIP-001", cari.Id, "ANA", [new SatisSiparisSatirOlusturIstegi(urun.Id, 1, 100)]);

        await servis.OlusturAsync(istek, CancellationToken.None);
        var sonuc = await servis.OlusturAsync(istek, CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("satis.siparis_no_tekrar", sonuc.HataKodu);
    }

    [Fact]
    public async Task Cari_hesap_yoksa_satis_siparisi_reddedilir()
    {
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([], [urun], [StokGiris(urun, 10)]);

        var sonuc = await servis.OlusturAsync(new SatisSiparisOlusturIstegi(
            "SIP-001",
            Guid.NewGuid(),
            "ANA",
            [new SatisSiparisSatirOlusturIstegi(urun.Id, 1, 100)]), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("satis.cari_bulunamadi", sonuc.HataKodu);
    }

    private static StokHareketi StokGiris(Urun urun, decimal miktar) =>
        StokHareketi.Olustur(urun.Id, urun.Kod, "ANA", StokHareketTuru.Giris, miktar, "test", null, DateTimeOffset.UtcNow);

    private static SatisSiparisServisi ServisOlustur(
        IReadOnlyList<CariHesap> cariler,
        IReadOnlyList<Urun> urunler,
        IReadOnlyList<StokHareketi> stokHareketleri)
    {
        var siparisDeposu = new TestSatisSiparisDeposu();
        var cariDeposu = new TestCariHesapDeposu(cariler);
        var urunDeposu = new TestUrunDeposu(urunler);
        var stokDeposu = new TestStokDeposu(stokHareketleri);
        return new SatisSiparisServisi(siparisDeposu, cariDeposu, urunDeposu, stokDeposu, TimeProvider.System);
    }

    private sealed class TestSatisSiparisDeposu : ISatisSiparisDeposu
    {
        private readonly List<SatisSiparisi> _siparisler = [];

        public Task<IReadOnlyList<SatisSiparisi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<SatisSiparisi>>(_siparisler.Where(siparis => cariHesapId is null || siparis.CariHesapId == cariHesapId).Take(limit).ToArray());

        public Task<SatisSiparisi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_siparisler.SingleOrDefault(siparis => siparis.Id == id));

        public Task<SatisSiparisi?> SiparisNoIleBulAsync(string siparisNo, CancellationToken cancellationToken) =>
            Task.FromResult(_siparisler.SingleOrDefault(siparis => siparis.SiparisNo == siparisNo.Trim().ToUpperInvariant()));

        public Task EkleAsync(SatisSiparisi siparis, CancellationToken cancellationToken)
        {
            _siparisler.Add(siparis);
            return Task.CompletedTask;
        }
    }

    private sealed class TestCariHesapDeposu : ICariHesapDeposu
    {
        private readonly List<CariHesap> _hesaplar;

        public TestCariHesapDeposu(IReadOnlyList<CariHesap> hesaplar)
        {
            _hesaplar = [.. hesaplar];
        }

        public Task<IReadOnlyList<CariHesap>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CariHesap>>(_hesaplar.Take(limit).ToArray());

        public Task<CariHesap?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cari => cari.Id == id));

        public Task<CariHesap?> KodIleBulAsync(string kod, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cari => cari.Kod == CariHesap.NormalizeKod(kod)));

        public Task EkleAsync(CariHesap cariHesap, CancellationToken cancellationToken)
        {
            _hesaplar.Add(cariHesap);
            return Task.CompletedTask;
        }
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
        private readonly List<StokHareketi> _hareketler;

        public TestStokDeposu(IReadOnlyList<StokHareketi> hareketler)
        {
            _hareketler = [.. hareketler];
        }

        public Task<IReadOnlyList<StokHareketi>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<StokHareketi>>(_hareketler.Take(limit).ToArray());

        public Task<IReadOnlyList<StokHareketi>> UrunHareketleriAsync(Guid urunId, string depoKod, CancellationToken cancellationToken)
        {
            var normalizeDepoKod = StokHareketi.NormalizeDepoKod(depoKod);
            var sonuc = _hareketler.Where(hareket => hareket.UrunId == urunId && hareket.DepoKod == normalizeDepoKod).ToArray();
            return Task.FromResult<IReadOnlyList<StokHareketi>>(sonuc);
        }

        public Task EkleAsync(StokHareketi hareket, CancellationToken cancellationToken)
        {
            _hareketler.Add(hareket);
            return Task.CompletedTask;
        }
    }
}
