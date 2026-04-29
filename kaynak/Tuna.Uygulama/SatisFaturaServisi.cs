using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class SatisFaturaServisi
{
    private readonly ISatisFaturaDeposu _faturaDeposu;
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly IUrunDeposu _urunDeposu;
    private readonly IStokDeposu _stokDeposu;
    private readonly IFinansHareketDeposu _finansHareketDeposu;
    private readonly DenetimServisi _denetimServisi;
    private readonly TimeProvider _timeProvider;

    public SatisFaturaServisi(
        ISatisFaturaDeposu faturaDeposu,
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

    public async Task<IReadOnlyList<SatisFaturaOzeti>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 100);
        var faturalar = await _faturaDeposu.ListeleAsync(cariHesapId, guvenliLimit, cancellationToken);
        return faturalar.Select(fatura => fatura.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<SatisFaturaOzeti>> IdIleGetirAsync(Guid id, CancellationToken cancellationToken)
    {
        var fatura = await _faturaDeposu.IdIleBulAsync(id, cancellationToken);
        return fatura is null
            ? UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.fatura_bulunamadi", "Satis faturasi bulunamadi.")
            : UygulamaSonucu<SatisFaturaOzeti>.BasariliSonuc(fatura.Ozetle());
    }

    public async Task<UygulamaSonucu<SatisFaturaOzeti>> OlusturAsync(SatisFaturaOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.gecersiz", dogrulamaHatasi);
        }

        var faturaNo = istek.FaturaNo.Trim().ToUpperInvariant();
        var mevcut = await _faturaDeposu.FaturaNoIleBulAsync(faturaNo, cancellationToken);
        if (mevcut is not null)
        {
            return UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.fatura_no_tekrar", "Ayni satis fatura no daha once kullanilmis.");
        }

        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(istek.CariHesapId, cancellationToken);
        if (cariHesap is null)
        {
            return UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.cari_bulunamadi", "Cari hesap bulunamadi.");
        }

        var depoKod = StokHareketi.NormalizeDepoKod(istek.DepoKod);
        var satirlar = new List<SatisFaturaSatiri>();
        foreach (var satirIstegi in istek.Satirlar)
        {
            var urun = await _urunDeposu.IdIleBulAsync(satirIstegi.UrunId, cancellationToken);
            if (urun is null)
            {
                return UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.urun_bulunamadi", "Fatura satirindaki urun bulunamadi.");
            }

            var hareketler = await _stokDeposu.UrunHareketleriAsync(urun.Id, depoKod, cancellationToken);
            var stokBakiye = hareketler.Sum(hareket => hareket.BakiyeEtkisi);
            if (stokBakiye < satirIstegi.Miktar)
            {
                return UygulamaSonucu<SatisFaturaOzeti>.Hata("satis.stok_yetersiz", $"{urun.Kod} icin stok bakiyesi yetersiz.");
            }

            satirlar.Add(new SatisFaturaSatiri(
                Guid.NewGuid(),
                urun.Id,
                urun.Kod,
                urun.Ad,
                satirIstegi.Miktar,
                satirIstegi.BirimFiyat,
                urun.KdvOrani));
        }

        var now = _timeProvider.GetUtcNow();
        var fatura = SatisFaturasi.Olustur(faturaNo, cariHesap, depoKod, satirlar, now);
        await _faturaDeposu.EkleAsync(fatura, cancellationToken);

        foreach (var satir in fatura.Satirlar)
        {
            var hareket = StokHareketi.Olustur(
                satir.UrunId,
                satir.UrunKod,
                fatura.DepoKod,
                StokHareketTuru.Cikis,
                satir.Miktar,
                $"satis.fatura:{fatura.FaturaNo}",
                "Satis faturasi stok cikisi",
                now);

            await _stokDeposu.EkleAsync(hareket, cancellationToken);
        }

        var finansHareketi = FinansHareketi.Olustur(
            cariHesap,
            FinansHareketTuru.SatisFaturasi,
            fatura.GenelTutar,
            0,
            $"satis.fatura:{fatura.FaturaNo}",
            "Satis faturasi cari borc hareketi",
            now);

        await _finansHareketDeposu.EkleAsync(finansHareketi, cancellationToken);

        await _denetimServisi.KaydetAsync(new DenetimKaydiOlusturIstegi(
            "Satis",
            DenetimIslemTuru.Olusturma,
            nameof(SatisFaturasi),
            fatura.Id.ToString(),
            $"satis.fatura:{fatura.FaturaNo}",
            $"Satis faturasi olusturuldu. Genel tutar: {fatura.GenelTutar}"), cancellationToken);

        return UygulamaSonucu<SatisFaturaOzeti>.BasariliSonuc(fatura.Ozetle());
    }

    private static string? Dogrula(SatisFaturaOlusturIstegi istek)
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
