using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class DenetimServisiTestleri
{
    [Fact]
    public async Task Denetim_kaydi_olusturur_ve_listeleyebilir()
    {
        var depo = new TestDenetimKayitDeposu();
        var servis = new DenetimServisi(depo, TimeProvider.System);

        var kayit = await servis.KaydetAsync(new DenetimKaydiOlusturIstegi(
            "Satis",
            DenetimIslemTuru.Olusturma,
            "SatisFaturasi",
            "1",
            "test",
            "test kaydi"), CancellationToken.None);
        var liste = await servis.ListeleAsync("Satis", "SatisFaturasi", "1", 10, CancellationToken.None);

        Assert.Equal("Satis", kayit.Modul);
        Assert.Single(liste);
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
