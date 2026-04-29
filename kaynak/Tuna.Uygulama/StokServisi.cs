using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class StokServisi
{
    private readonly IStokDeposu _stokDeposu;
    private readonly IUrunDeposu _urunDeposu;
    private readonly TimeProvider _timeProvider;

    public StokServisi(IStokDeposu stokDeposu, IUrunDeposu urunDeposu, TimeProvider timeProvider)
    {
        _stokDeposu = stokDeposu;
        _urunDeposu = urunDeposu;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<StokHareketOzeti>> HareketleriListeleAsync(Guid? urunId, string? depoKod, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 200);
        var hareketler = await _stokDeposu.HareketleriListeleAsync(urunId, depoKod, guvenliLimit, cancellationToken);
        return hareketler.Select(hareket => hareket.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<StokBakiyeOzeti>> BakiyeGetirAsync(Guid urunId, string depoKod, CancellationToken cancellationToken)
    {
        var urun = await _urunDeposu.IdIleBulAsync(urunId, cancellationToken);
        if (urun is null)
        {
            return UygulamaSonucu<StokBakiyeOzeti>.Hata("stok.urun_bulunamadi", "Urun bulunamadi.");
        }

        var normalizeDepoKod = StokHareketi.NormalizeDepoKod(depoKod);
        var hareketler = await _stokDeposu.UrunHareketleriAsync(urunId, normalizeDepoKod, cancellationToken);
        var bakiye = hareketler.Sum(hareket => hareket.BakiyeEtkisi);

        return UygulamaSonucu<StokBakiyeOzeti>.BasariliSonuc(new StokBakiyeOzeti(
            urun.Id,
            urun.Kod,
            urun.Ad,
            normalizeDepoKod,
            bakiye));
    }

    public async Task<UygulamaSonucu<StokHareketOzeti>> HareketOlusturAsync(StokHareketOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<StokHareketOzeti>.Hata("stok.gecersiz", dogrulamaHatasi);
        }

        var urun = await _urunDeposu.IdIleBulAsync(istek.UrunId, cancellationToken);
        if (urun is null)
        {
            return UygulamaSonucu<StokHareketOzeti>.Hata("stok.urun_bulunamadi", "Urun bulunamadi.");
        }

        var depoKod = StokHareketi.NormalizeDepoKod(istek.DepoKod);
        var mevcutHareketler = await _stokDeposu.UrunHareketleriAsync(istek.UrunId, depoKod, cancellationToken);
        var mevcutBakiye = mevcutHareketler.Sum(hareket => hareket.BakiyeEtkisi);
        if (istek.Tur == StokHareketTuru.Cikis && mevcutBakiye < istek.Miktar)
        {
            return UygulamaSonucu<StokHareketOzeti>.Hata("stok.yetersiz_bakiye", "Stok bakiyesi cikis icin yetersiz.");
        }

        var hareket = StokHareketi.Olustur(
            urun.Id,
            urun.Kod,
            depoKod,
            istek.Tur,
            istek.Miktar,
            istek.Kaynak,
            istek.Aciklama,
            _timeProvider.GetUtcNow());

        await _stokDeposu.EkleAsync(hareket, cancellationToken);
        return UygulamaSonucu<StokHareketOzeti>.BasariliSonuc(hareket.Ozetle());
    }

    private static string? Dogrula(StokHareketOlusturIstegi istek)
    {
        if (istek.UrunId == Guid.Empty)
        {
            return "Urun id zorunludur.";
        }

        if (string.IsNullOrWhiteSpace(istek.DepoKod))
        {
            return "Depo kod zorunludur.";
        }

        if (istek.DepoKod.Trim().Length > 40)
        {
            return "Depo kod en fazla 40 karakter olabilir.";
        }

        if (istek.Miktar <= 0)
        {
            return "Miktar sifirdan buyuk olmalidir.";
        }

        if (string.IsNullOrWhiteSpace(istek.Kaynak))
        {
            return "Kaynak zorunludur.";
        }

        return null;
    }
}
