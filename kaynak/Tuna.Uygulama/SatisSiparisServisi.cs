using Tuna.Alan;

namespace Tuna.Uygulama;

public sealed class SatisSiparisServisi
{
    private readonly ISatisSiparisDeposu _siparisDeposu;
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly IUrunDeposu _urunDeposu;
    private readonly IStokDeposu _stokDeposu;
    private readonly TimeProvider _timeProvider;

    public SatisSiparisServisi(
        ISatisSiparisDeposu siparisDeposu,
        ICariHesapDeposu cariHesapDeposu,
        IUrunDeposu urunDeposu,
        IStokDeposu stokDeposu,
        TimeProvider timeProvider)
    {
        _siparisDeposu = siparisDeposu;
        _cariHesapDeposu = cariHesapDeposu;
        _urunDeposu = urunDeposu;
        _stokDeposu = stokDeposu;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<SatisSiparisOzeti>> ListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 100);
        var siparisler = await _siparisDeposu.ListeleAsync(cariHesapId, guvenliLimit, cancellationToken);
        return siparisler.Select(siparis => siparis.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<SatisSiparisOzeti>> IdIleGetirAsync(Guid id, CancellationToken cancellationToken)
    {
        var siparis = await _siparisDeposu.IdIleBulAsync(id, cancellationToken);
        return siparis is null
            ? UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.siparis_bulunamadi", "Satis siparisi bulunamadi.")
            : UygulamaSonucu<SatisSiparisOzeti>.BasariliSonuc(siparis.Ozetle());
    }

    public async Task<UygulamaSonucu<SatisSiparisOzeti>> OlusturAsync(SatisSiparisOlusturIstegi istek, CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.gecersiz", dogrulamaHatasi);
        }

        var siparisNo = istek.SiparisNo.Trim().ToUpperInvariant();
        var mevcut = await _siparisDeposu.SiparisNoIleBulAsync(siparisNo, cancellationToken);
        if (mevcut is not null)
        {
            return UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.siparis_no_tekrar", "Ayni siparis no daha once kullanilmis.");
        }

        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(istek.CariHesapId, cancellationToken);
        if (cariHesap is null)
        {
            return UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.cari_bulunamadi", "Cari hesap bulunamadi.");
        }

        var depoKod = StokHareketi.NormalizeDepoKod(istek.DepoKod);
        var satirlar = new List<SatisSiparisSatiri>();
        foreach (var satirIstegi in istek.Satirlar)
        {
            var urun = await _urunDeposu.IdIleBulAsync(satirIstegi.UrunId, cancellationToken);
            if (urun is null)
            {
                return UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.urun_bulunamadi", "Siparis satirindaki urun bulunamadi.");
            }

            var hareketler = await _stokDeposu.UrunHareketleriAsync(urun.Id, depoKod, cancellationToken);
            var stokBakiye = hareketler.Sum(hareket => hareket.BakiyeEtkisi);
            if (stokBakiye < satirIstegi.Miktar)
            {
                return UygulamaSonucu<SatisSiparisOzeti>.Hata("satis.stok_yetersiz", $"{urun.Kod} icin stok bakiyesi yetersiz.");
            }

            satirlar.Add(new SatisSiparisSatiri(
                Guid.NewGuid(),
                urun.Id,
                urun.Kod,
                urun.Ad,
                satirIstegi.Miktar,
                satirIstegi.BirimFiyat,
                urun.KdvOrani));
        }

        var siparis = SatisSiparisi.Olustur(siparisNo, cariHesap, depoKod, satirlar, _timeProvider.GetUtcNow());
        await _siparisDeposu.EkleAsync(siparis, cancellationToken);
        return UygulamaSonucu<SatisSiparisOzeti>.BasariliSonuc(siparis.Ozetle());
    }

    private static string? Dogrula(SatisSiparisOlusturIstegi istek)
    {
        if (string.IsNullOrWhiteSpace(istek.SiparisNo))
        {
            return "Siparis no zorunludur.";
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
            return "En az bir siparis satiri zorunludur.";
        }

        if (istek.Satirlar.Any(satir => satir.UrunId == Guid.Empty || satir.Miktar <= 0 || satir.BirimFiyat < 0))
        {
            return "Siparis satirlarinda urun, pozitif miktar ve negatif olmayan fiyat zorunludur.";
        }

        return null;
    }
}
