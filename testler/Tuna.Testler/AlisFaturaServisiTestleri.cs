using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class AlisFaturaServisiTestleri
{
    [Fact]
    public async Task Alis_faturasi_toplamlari_hesaplar_ve_stok_girisi_olusturur()
    {
        var cari = CariHesap.Olustur("TED-001", "Test Tedarikci", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 20, DateTimeOffset.UtcNow);
        var stokDeposu = new TestStokDeposu([]);
        var finansDeposu = new TestFinansHareketDeposu();
        var denetimDeposu = new TestDenetimKayitDeposu();
        var servis = ServisOlustur([cari], [urun], stokDeposu, finansDeposu, denetimDeposu);

        var sonuc = await servis.OlusturAsync(new AlisFaturaOlusturIstegi(
            "alf-001",
            cari.Id,
            "ANA",
            [new AlisFaturaSatirOlusturIstegi(urun.Id, 5, 100)]), CancellationToken.None);

        var hareketler = await stokDeposu.UrunHareketleriAsync(urun.Id, "ANA", CancellationToken.None);
        var finansHareketleri = await finansDeposu.CariHareketleriAsync(cari.Id, CancellationToken.None);
        var denetimKayitlari = await denetimDeposu.ListeleAsync("Alis", nameof(AlisFaturasi), null, 10, CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal("ALF-001", sonuc.Deger!.FaturaNo);
        Assert.Equal(500, sonuc.Deger.NetTutar);
        Assert.Equal(100, sonuc.Deger.KdvTutar);
        Assert.Equal(600, sonuc.Deger.GenelTutar);
        Assert.Single(hareketler);
        Assert.Equal(5, hareketler.Single().BakiyeEtkisi);
        Assert.Single(finansHareketleri);
        Assert.Equal(600, finansHareketleri.Single().Alacak);
        Assert.Single(denetimKayitlari);
    }

    [Fact]
    public async Task Ayni_alis_fatura_no_ikinci_kez_kullanilamaz()
    {
        var cari = CariHesap.Olustur("TED-001", "Test Tedarikci", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 20, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([cari], [urun], new TestStokDeposu([]), new TestFinansHareketDeposu(), new TestDenetimKayitDeposu());
        var istek = new AlisFaturaOlusturIstegi("ALF-001", cari.Id, "ANA", [new AlisFaturaSatirOlusturIstegi(urun.Id, 1, 100)]);

        await servis.OlusturAsync(istek, CancellationToken.None);
        var sonuc = await servis.OlusturAsync(istek, CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("alis.fatura_no_tekrar", sonuc.HataKodu);
    }

    [Fact]
    public async Task Cari_hesap_yoksa_alis_faturasi_reddedilir()
    {
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 20, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([], [urun], new TestStokDeposu([]), new TestFinansHareketDeposu(), new TestDenetimKayitDeposu());

        var sonuc = await servis.OlusturAsync(new AlisFaturaOlusturIstegi(
            "ALF-001",
            Guid.NewGuid(),
            "ANA",
            [new AlisFaturaSatirOlusturIstegi(urun.Id, 1, 100)]), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("alis.cari_bulunamadi", sonuc.HataKodu);
    }

    [Fact]
    public async Task Urun_yoksa_alis_faturasi_reddedilir()
    {
        var cari = CariHesap.Olustur("TED-001", "Test Tedarikci", null, null, null, 0, DateTimeOffset.UtcNow);
        var servis = ServisOlustur([cari], [], new TestStokDeposu([]), new TestFinansHareketDeposu(), new TestDenetimKayitDeposu());

        var sonuc = await servis.OlusturAsync(new AlisFaturaOlusturIstegi(
            "ALF-001",
            cari.Id,
            "ANA",
            [new AlisFaturaSatirOlusturIstegi(Guid.NewGuid(), 1, 100)]), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("alis.urun_bulunamadi", sonuc.HataKodu);
    }

    private static AlisFaturaServisi ServisOlustur(
        IReadOnlyList<CariHesap> cariler,
        IReadOnlyList<Urun> urunler,
        TestStokDeposu stokDeposu,
        TestFinansHareketDeposu finansDeposu,
        TestDenetimKayitDeposu denetimDeposu)
    {
        var faturaDeposu = new TestAlisFaturaDeposu();
        var cariDeposu = new TestCariHesapDeposu(cariler);
        var urunDeposu = new TestUrunDeposu(urunler);
        return new AlisFaturaServisi(faturaDeposu, cariDeposu, urunDeposu, stokDeposu, finansDeposu, new DenetimServisi(denetimDeposu, TimeProvider.System), TimeProvider.System);
    }

    private sealed class TestAlisFaturaDeposu : IAlisFaturaDeposu
    {
        private readonly List<AlisFaturasi> _faturalar = [];

        public Task<IReadOnlyList<AlisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<AlisFaturasi>>(_faturalar.Where(fatura => cariHesapId is null || fatura.CariHesapId == cariHesapId).Take(limit).ToArray());

        public Task<AlisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_faturalar.SingleOrDefault(fatura => fatura.Id == id));

        public Task<AlisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken) =>
            Task.FromResult(_faturalar.SingleOrDefault(fatura => fatura.FaturaNo == faturaNo.Trim().ToUpperInvariant()));

        public Task EkleAsync(AlisFaturasi fatura, CancellationToken cancellationToken)
        {
            _faturalar.Add(fatura);
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

    private sealed class TestFinansHareketDeposu : IFinansHareketDeposu
    {
        private readonly List<FinansHareketi> _hareketler = [];

        public Task<IReadOnlyList<FinansHareketi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FinansHareketi>>(_hareketler.Where(hareket => cariHesapId is null || hareket.CariHesapId == cariHesapId).Take(limit).ToArray());

        public Task<IReadOnlyList<FinansHareketi>> CariHareketleriAsync(Guid cariHesapId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FinansHareketi>>(_hareketler.Where(hareket => hareket.CariHesapId == cariHesapId).ToArray());

        public Task EkleAsync(FinansHareketi hareket, CancellationToken cancellationToken)
        {
            _hareketler.Add(hareket);
            return Task.CompletedTask;
        }
    }

    private sealed class TestDenetimKayitDeposu : IDenetimKayitDeposu
    {
        private readonly List<DenetimKaydi> _kayitlar = [];

        public Task<IReadOnlyList<DenetimKaydi>> ListeleAsync(string? modul, string? varlikTuru, string? varlikId, int limit, CancellationToken cancellationToken)
        {
            var sonuc = _kayitlar
                .Where(kayit => modul is null || kayit.Modul == modul)
                .Where(kayit => varlikTuru is null || kayit.VarlikTuru == varlikTuru)
                .Where(kayit => varlikId is null || kayit.VarlikId == varlikId)
                .Take(limit)
                .ToArray();

            return Task.FromResult<IReadOnlyList<DenetimKaydi>>(sonuc);
        }

        public Task EkleAsync(DenetimKaydi kayit, CancellationToken cancellationToken)
        {
            _kayitlar.Add(kayit);
            return Task.CompletedTask;
        }
    }
}
