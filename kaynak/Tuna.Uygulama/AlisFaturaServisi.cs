using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class AlisFaturaServisi
{
    private readonly IAlisFaturaDeposu _faturaDeposu;
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly IUrunDeposu _urunDeposu;
    private readonly IStokDeposu _stokDeposu;
    private readonly IFinansHareketDeposu _finansHareketDeposu;
    private readonly DenetimServisi _denetimServisi;
    private readonly TimeProvider _timeProvider;

    public AlisFaturaServisi(
        IAlisFaturaDeposu faturaDeposu,
        ICariHesapDeposu cariHesapDeposu,
        IUrunDeposu urunDeposu,
        IStokDeposu stokDeposu,
        IFinansHareketDeposu finansHareketDeposu,
        DenetimServisi denetimServisi,
        TimeProvider timeProvider)
    {
        _faturaDeposu = faturaDeposu;
        _cariHesapDeposu = cariHesapDeposu;
        _urunDeposu = urunDeposu;
        _stokDeposu = stokDeposu;
        _finansHareketDeposu = finansHareketDeposu;
        _denetimServisi = denetimServisi;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<AlisFaturaOzeti>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 100);
        var faturalar = await _faturaDeposu.ListeleAsync(cariHesapId, guvenliLimit, cancellationToken);
        return faturalar.Select(fatura => fatura.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<AlisFaturaOzeti>> IdIleGetirAsync(Guid id, CancellationToken cancellationToken)
    {
        var fatura = await _faturaDeposu.IdIleBulAsync(id, cancellationToken);
        return fatura is null
            ? UygulamaSonucu<AlisFaturaOzeti>.Hata("alis.fatura_bulunamadi", "Alis faturasi bulunamadi.")
            : UygulamaSonucu<AlisFaturaOzeti>.BasariliSonuc(fatura.Ozetle());
    }

    public async Task<UygulamaSonucu<AlisFaturaOzeti>> OlusturAsync(AlisFaturaOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<AlisFaturaOzeti>.Hata("alis.gecersiz", dogrulamaHatasi);
        }

        var faturaNo = istek.FaturaNo.Trim().ToUpperInvariant();
        var mevcut = await _faturaDeposu.FaturaNoIleBulAsync(faturaNo, cancellationToken);
        if (mevcut is not null)
        {
            return UygulamaSonucu<AlisFaturaOzeti>.Hata("alis.fatura_no_tekrar", "Ayni alis fatura no daha once kullanilmis.");
        }

        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(istek.CariHesapId, cancellationToken);
        if (cariHesap is null)
        {
            return UygulamaSonucu<AlisFaturaOzeti>.Hata("alis.cari_bulunamadi", "Cari hesap bulunamadi.");
        }

        var depoKod = StokHareketi.NormalizeDepoKod(istek.DepoKod);
        var satirlar = new List<AlisFaturaSatiri>();
        foreach (var satirIstegi in istek.Satirlar)
        {
            var urun = await _urunDeposu.IdIleBulAsync(satirIstegi.UrunId, cancellationToken);
            if (urun is null)
            {
                return UygulamaSonucu<AlisFaturaOzeti>.Hata("alis.urun_bulunamadi", "Fatura satirindaki urun bulunamadi.");
            }

            satirlar.Add(new AlisFaturaSatiri(
                Guid.NewGuid(),
                urun.Id,
                urun.Kod,
                urun.Ad,
                satirIstegi.Miktar,
                satirIstegi.BirimFiyat,
                urun.KdvOrani));
        }

        var now = _timeProvider.GetUtcNow();
        var fatura = AlisFaturasi.Olustur(faturaNo, cariHesap, depoKod, satirlar, now);
        await _faturaDeposu.EkleAsync(fatura, cancellationToken);

        foreach (var satir in fatura.Satirlar)
        {
            var hareket = StokHareketi.Olustur(
                satir.UrunId,
                satir.UrunKod,
                fatura.DepoKod,
                StokHareketTuru.Giris,
                satir.Miktar,
                $"alis.fatura:{fatura.FaturaNo}",
                "Alis faturasi stok girisi",
                now);

            await _stokDeposu.EkleAsync(hareket, cancellationToken);
        }

        var finansHareketi = FinansHareketi.Olustur(
            cariHesap,
            FinansHareketTuru.AlisFaturasi,
            0,
            fatura.GenelTutar,
            $"alis.fatura:{fatura.FaturaNo}",
            "Alis faturasi cari alacak hareketi",
            now);

        await _finansHareketDeposu.EkleAsync(finansHareketi, cancellationToken);

        await _denetimServisi.KaydetAsync(new DenetimKaydiOlusturIstegi(
            "Alis",
            DenetimIslemTuru.Olusturma,
            nameof(AlisFaturasi),
            fatura.Id.ToString(),
            $"alis.fatura:{fatura.FaturaNo}",
            $"Alis faturasi olusturuldu. Genel tutar: {fatura.GenelTutar}"), cancellationToken);

        return UygulamaSonucu<AlisFaturaOzeti>.BasariliSonuc(fatura.Ozetle());
    }

    private static string? Dogrula(AlisFaturaOlusturIstegi istek)
    {
        if (string.IsNullOrWhiteSpace(istek.FaturaNo))
        {
            return "Fatura no zorunludur.";
        }

        if (istek.CariHesapId == Guid.Empty)
        {
            return "Cari hesap id zorunludur.";
        }

        if (string.IsNullOrWhiteSpace(istek.DepoKod))
        {
            return "Depo kod zorunludur.";
        }

        if (istek.Satirlar.Count == 0)
        {
            return "En az bir fatura satiri zorunludur.";
        }

        if (istek.Satirlar.Any(satir => satir.UrunId == Guid.Empty || satir.Miktar <= 0 || satir.BirimFiyat < 0))
        {
            return "Fatura satirlarinda urun, pozitif miktar ve negatif olmayan fiyat zorunludur.";
        }

        return null;
    }
}
