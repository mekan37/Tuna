using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class UrunKatalogServisi
{
    private readonly IUrunDeposu _urunDeposu;
    private readonly TimeProvider _timeProvider;

    public UrunKatalogServisi(IUrunDeposu urunDeposu, TimeProvider timeProvider)
    {
        _urunDeposu = urunDeposu;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<UrunOzeti>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 100);
        var urunler = await _urunDeposu.ListeleAsync(arama, guvenliLimit, cancellationToken);
        return urunler.Select(urun => urun.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<UrunOzeti>> IdIleGetirAsync(Guid id, CancellationToken cancellationToken)
    {
        var urun = await _urunDeposu.IdIleBulAsync(id, cancellationToken);
        return urun is null
            ? UygulamaSonucu<UrunOzeti>.Hata("urun.bulunamadi", "Urun bulunamadi.")
            : UygulamaSonucu<UrunOzeti>.BasariliSonuc(urun.Ozetle());
    }

    public async Task<UygulamaSonucu<UrunOzeti>> OlusturAsync(UrunOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<UrunOzeti>.Hata("urun.gecersiz", dogrulamaHatasi);
        }

        var kod = Urun.NormalizeKod(istek.Kod);
        var mevcut = await _urunDeposu.KodIleBulAsync(kod, cancellationToken);
        if (mevcut is not null)
        {
            return UygulamaSonucu<UrunOzeti>.Hata("urun.kod_tekrar", "Ayni urun kodu daha once kullanilmis.");
        }

        var urun = Urun.Olustur(
            istek.Kod,
            istek.Ad,
            istek.Barkod,
            istek.Uretici,
            istek.KdvOrani,
            _timeProvider.GetUtcNow());

        await _urunDeposu.EkleAsync(urun, cancellationToken);
        return UygulamaSonucu<UrunOzeti>.BasariliSonuc(urun.Ozetle());
    }

    private static string? Dogrula(UrunOlusturIstegi istek)
    {
        if (string.IsNullOrWhiteSpace(istek.Kod))
        {
            return "Urun kodu zorunludur.";
        }

        if (istek.Kod.Trim().Length > 40)
        {
            return "Urun kodu en fazla 40 karakter olabilir.";
        }

        if (string.IsNullOrWhiteSpace(istek.Ad))
        {
            return "Urun adi zorunludur.";
        }

        if (istek.Ad.Trim().Length > 250)
        {
            return "Urun adi en fazla 250 karakter olabilir.";
        }

        if (istek.KdvOrani < 0 || istek.KdvOrani > 100)
        {
            return "KDV orani 0 ile 100 arasinda olmalidir.";
        }

        return null;
    }
}
