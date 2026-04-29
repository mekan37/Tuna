using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class CariHesapServisi
{
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly TimeProvider _timeProvider;

    public CariHesapServisi(ICariHesapDeposu cariHesapDeposu, TimeProvider timeProvider)
    {
        _cariHesapDeposu = cariHesapDeposu;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<CariHesapOzeti>> ListeleAsync(string? arama, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 100);
        var hesaplar = await _cariHesapDeposu.ListeleAsync(arama, guvenliLimit, cancellationToken);
        return hesaplar.Select(cariHesap => cariHesap.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<CariHesapOzeti>> IdIleGetirAsync(Guid id, CancellationToken cancellationToken)
    {
        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(id, cancellationToken);
        return cariHesap is null
            ? UygulamaSonucu<CariHesapOzeti>.Hata("cari.bulunamadi", "Cari hesap bulunamadi.")
            : UygulamaSonucu<CariHesapOzeti>.BasariliSonuc(cariHesap.Ozetle());
    }

    public async Task<UygulamaSonucu<CariHesapOzeti>> OlusturAsync(CariHesapOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<CariHesapOzeti>.Hata("cari.gecersiz", dogrulamaHatasi);
        }

        var kod = CariHesap.NormalizeKod(istek.Kod);
        var mevcut = await _cariHesapDeposu.KodIleBulAsync(kod, cancellationToken);
        if (mevcut is not null)
        {
            return UygulamaSonucu<CariHesapOzeti>.Hata("cari.kod_tekrar", "Ayni cari kod daha once kullanilmis.");
        }

        var cariHesap = CariHesap.Olustur(
            istek.Kod,
            istek.Unvan,
            istek.VergiNo,
            istek.Telefon,
            istek.Eposta,
            istek.RiskLimiti,
            _timeProvider.GetUtcNow());

        await _cariHesapDeposu.EkleAsync(cariHesap, cancellationToken);
        return UygulamaSonucu<CariHesapOzeti>.BasariliSonuc(cariHesap.Ozetle());
    }

    private static string? Dogrula(CariHesapOlusturIstegi istek)
    {
        if (string.IsNullOrWhiteSpace(istek.Kod))
        {
            return "Cari kod zorunludur.";
        }

        if (istek.Kod.Trim().Length > 40)
        {
            return "Cari kod en fazla 40 karakter olabilir.";
        }

        if (string.IsNullOrWhiteSpace(istek.Unvan))
        {
            return "Cari unvan zorunludur.";
        }

        if (istek.Unvan.Trim().Length > 250)
        {
            return "Cari unvan en fazla 250 karakter olabilir.";
        }

        if (istek.RiskLimiti < 0)
        {
            return "Risk limiti negatif olamaz.";
        }

        if (istek.VergiNo is { Length: > 0 } vergiNo && vergiNo.Trim().Length is not (10 or 11))
        {
            return "Vergi no 10 veya 11 karakter olmalidir.";
        }

        return null;
    }
}
