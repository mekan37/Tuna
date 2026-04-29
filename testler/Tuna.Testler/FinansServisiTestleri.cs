using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class FinansServisiTestleri
{
    [Fact]
    public async Task Cari_bakiye_borc_eksi_alacak_olarak_hesaplanir()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var cariDeposu = new TestCariHesapDeposu([cari]);
        var finansDeposu = new TestFinansHareketDeposu([
            FinansHareketi.Olustur(cari, FinansHareketTuru.SatisFaturasi, 100, 0, "test", null, DateTimeOffset.UtcNow),
            FinansHareketi.Olustur(cari, FinansHareketTuru.Tahsilat, 0, 40, "test", null, DateTimeOffset.UtcNow)
        ]);
        var servis = new FinansServisi(finansDeposu, cariDeposu, new DenetimServisi(new TestDenetimKayitDeposu(), TimeProvider.System), TimeProvider.System);

        var sonuc = await servis.CariBakiyeGetirAsync(cari.Id, CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal(100, sonuc.Deger!.BorcToplam);
        Assert.Equal(40, sonuc.Deger.AlacakToplam);
        Assert.Equal(60, sonuc.Deger.Bakiye);
    }

    [Fact]
    public async Task Bilinmeyen_cari_icin_bakiye_reddedilir()
    {
        var servis = new FinansServisi(new TestFinansHareketDeposu([]), new TestCariHesapDeposu([]), new DenetimServisi(new TestDenetimKayitDeposu(), TimeProvider.System), TimeProvider.System);

        var sonuc = await servis.CariBakiyeGetirAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(sonuc.Basarili);
        Assert.Equal("finans.cari_bulunamadi", sonuc.HataKodu);
    }

    [Fact]
    public async Task Tahsilat_cari_alacak_hareketi_olusturur()
    {
        var cari = CariHesap.Olustur("MUS-001", "Test Musteri", null, null, null, 0, DateTimeOffset.UtcNow);
        var finansDeposu = new TestFinansHareketDeposu([]);
        var denetimDeposu = new TestDenetimKayitDeposu();
        var servis = new FinansServisi(finansDeposu, new TestCariHesapDeposu([cari]), new DenetimServisi(denetimDeposu, TimeProvider.System), TimeProvider.System);

        var sonuc = await servis.TahsilatOlusturAsync(new FinansHareketOlusturIstegi(cari.Id, 75, "kasa", "nakit tahsilat"), CancellationToken.None);
        var denetimKayitlari = await denetimDeposu.ListeleAsync("Finans", nameof(FinansHareketi), null, 10, CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal(FinansHareketTuru.Tahsilat, sonuc.Deger!.Tur);
        Assert.Equal(0, sonuc.Deger.Borc);
        Assert.Equal(75, sonuc.Deger.Alacak);
        Assert.Single(denetimKayitlari);
    }

    [Fact]
    public async Task Odeme_cari_borc_hareketi_olusturur()
    {
        var cari = CariHesap.Olustur("TED-001", "Test Tedarikci", null, null, null, 0, DateTimeOffset.UtcNow);
        var finansDeposu = new TestFinansHareketDeposu([]);
        var servis = new FinansServisi(finansDeposu, new TestCariHesapDeposu([cari]), new DenetimServisi(new TestDenetimKayitDeposu(), TimeProvider.System), TimeProvider.System);

        var sonuc = await servis.OdemeOlusturAsync(new FinansHareketOlusturIstegi(cari.Id, 50, "banka", "havale"), CancellationToken.None);

        Assert.True(sonuc.Basarili);
        Assert.Equal(FinansHareketTuru.Odeme, sonuc.Deger!.Tur);
        Assert.Equal(50, sonuc.Deger.Borc);
        Assert.Equal(0, sonuc.Deger.Alacak);
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

    private sealed class TestFinansHareketDeposu : IFinansHareketDeposu
    {
        private readonly List<FinansHareketi> _hareketler;

        public TestFinansHareketDeposu(IReadOnlyList<FinansHareketi> hareketler)
        {
            _hareketler = [.. hareketler];
        }

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
