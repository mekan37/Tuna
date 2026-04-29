using Tuna.Alan;
using Tuna.Raporlama;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class OperasyonOzetiServisiTestleri
{
    [Fact]
    public async Task Operasyon_ozeti_canli_depo_sayilarini_hesaplar()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var urun = Urun.Olustur("URUN-001", "Test Urun", null, null, 10, DateTimeOffset.UtcNow);
        var finansHareketi = FinansHareketi.Olustur(cari, FinansHareketTuru.SatisFaturasi, 100, 25, "test", null, DateTimeOffset.UtcNow);
        var servis = new OperasyonOzetiServisi(
            new TestUrunDeposu([urun]),
            new TestCariHesapDeposu([cari]),
            new TestStokDeposu([StokHareketi.Olustur(urun.Id, urun.Kod, "ANA", StokHareketTuru.Giris, 5, "test", null, DateTimeOffset.UtcNow)]),
            new TestSatisSiparisDeposu([]),
            new TestSatisFaturaDeposu([]),
            new TestAlisFaturaDeposu([]),
            new TestFinansHareketDeposu([finansHareketi]),
            new TestDenetimKayitDeposu([
                DenetimKaydi.Olustur("Finans", DenetimIslemTuru.Olusturma, nameof(FinansHareketi), finansHareketi.Id.ToString(), "test", null, DateTimeOffset.UtcNow)
            ]));

        var ozet = await servis.GetirAsync(CancellationToken.None);

        Assert.Equal(1, ozet.UrunSayisi);
        Assert.Equal(1, ozet.CariHesapSayisi);
        Assert.Equal(1, ozet.StokHareketSayisi);
        Assert.Equal(1, ozet.FinansHareketSayisi);
        Assert.Equal(1, ozet.DenetimKaydiSayisi);
        Assert.Equal(75, ozet.CariBakiyeToplami);
    }

    private sealed class TestUrunDeposu : IUrunDeposu
    {
        private readonly IReadOnlyList<Urun> _urunler;

        public TestUrunDeposu(IReadOnlyList<Urun> urunler) => _urunler = urunler;

        public Task<IReadOnlyList<Urun>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Urun>>(_urunler.Take(limit).ToArray());

        public Task<Urun?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_urunler.SingleOrDefault(urun => urun.Id == id));

        public Task<Urun?> KodIleBulAsync(string kod, CancellationToken cancellationToken) =>
            Task.FromResult(_urunler.SingleOrDefault(urun => urun.Kod == Urun.NormalizeKod(kod)));

        public Task EkleAsync(Urun urun, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestCariHesapDeposu : ICariHesapDeposu
    {
        private readonly IReadOnlyList<CariHesap> _hesaplar;

        public TestCariHesapDeposu(IReadOnlyList<CariHesap> hesaplar) => _hesaplar = hesaplar;

        public Task<IReadOnlyList<CariHesap>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CariHesap>>(_hesaplar.Take(limit).ToArray());

        public Task<CariHesap?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cari => cari.Id == id));

        public Task<CariHesap?> KodIleBulAsync(string kod, CancellationToken cancellationToken) =>
            Task.FromResult(_hesaplar.SingleOrDefault(cari => cari.Kod == CariHesap.NormalizeKod(kod)));

        public Task EkleAsync(CariHesap cariHesap, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestStokDeposu : IStokDeposu
    {
        private readonly IReadOnlyList<StokHareketi> _hareketler;

        public TestStokDeposu(IReadOnlyList<StokHareketi> hareketler) => _hareketler = hareketler;

        public Task<IReadOnlyList<StokHareketi>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<StokHareketi>>(_hareketler.Take(limit).ToArray());

        public Task<IReadOnlyList<StokHareketi>> UrunHareketleriAsync(Guid urunId, string depoKod, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<StokHareketi>>(_hareketler.Where(hareket => hareket.UrunId == urunId).ToArray());

        public Task EkleAsync(StokHareketi hareket, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestSatisSiparisDeposu : ISatisSiparisDeposu
    {
        private readonly IReadOnlyList<SatisSiparisi> _siparisler;

        public TestSatisSiparisDeposu(IReadOnlyList<SatisSiparisi> siparisler) => _siparisler = siparisler;

        public Task<IReadOnlyList<SatisSiparisi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<SatisSiparisi>>(_siparisler.Take(limit).ToArray());

        public Task<SatisSiparisi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<SatisSiparisi?>(null);

        public Task<SatisSiparisi?> SiparisNoIleBulAsync(string siparisNo, CancellationToken cancellationToken) => Task.FromResult<SatisSiparisi?>(null);

        public Task EkleAsync(SatisSiparisi siparis, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestSatisFaturaDeposu : ISatisFaturaDeposu
    {
        private readonly IReadOnlyList<SatisFaturasi> _faturalar;

        public TestSatisFaturaDeposu(IReadOnlyList<SatisFaturasi> faturalar) => _faturalar = faturalar;

        public Task<IReadOnlyList<SatisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<SatisFaturasi>>(_faturalar.Take(limit).ToArray());

        public Task<SatisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<SatisFaturasi?>(null);

        public Task<SatisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken) => Task.FromResult<SatisFaturasi?>(null);

        public Task EkleAsync(SatisFaturasi fatura, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestAlisFaturaDeposu : IAlisFaturaDeposu
    {
        private readonly IReadOnlyList<AlisFaturasi> _faturalar;

        public TestAlisFaturaDeposu(IReadOnlyList<AlisFaturasi> faturalar) => _faturalar = faturalar;

        public Task<IReadOnlyList<AlisFaturasi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<AlisFaturasi>>(_faturalar.Take(limit).ToArray());

        public Task<AlisFaturasi?> IdIleBulAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<AlisFaturasi?>(null);

        public Task<AlisFaturasi?> FaturaNoIleBulAsync(string faturaNo, CancellationToken cancellationToken) => Task.FromResult<AlisFaturasi?>(null);

        public Task EkleAsync(AlisFaturasi fatura, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestFinansHareketDeposu : IFinansHareketDeposu
    {
        private readonly IReadOnlyList<FinansHareketi> _hareketler;

        public TestFinansHareketDeposu(IReadOnlyList<FinansHareketi> hareketler) => _hareketler = hareketler;

        public Task<IReadOnlyList<FinansHareketi>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FinansHareketi>>(_hareketler.Take(limit).ToArray());

        public Task<IReadOnlyList<FinansHareketi>> CariHareketleriAsync(Guid cariHesapId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FinansHareketi>>(_hareketler.Where(hareket => hareket.CariHesapId == cariHesapId).ToArray());

        public Task EkleAsync(FinansHareketi hareket, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestDenetimKayitDeposu : IDenetimKayitDeposu
    {
        private readonly IReadOnlyList<DenetimKaydi> _kayitlar;

        public TestDenetimKayitDeposu(IReadOnlyList<DenetimKaydi> kayitlar) => _kayitlar = kayitlar;

        public Task<IReadOnlyList<DenetimKaydi>> ListeleAsync(string? modul, string? varlikTuru, string? varlikId, int limit, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<DenetimKaydi>>(_kayitlar.Take(limit).ToArray());

        public Task EkleAsync(DenetimKaydi kayit, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
