using Tuna.Uygulama;

namespace Tuna.Raporlama;

public sealed class OperasyonOzetiServisi
{
    private readonly IUrunDeposu _urunDeposu;
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly IStokDeposu _stokDeposu;
    private readonly ISatisSiparisDeposu _satisSiparisDeposu;
    private readonly ISatisFaturaDeposu _satisFaturaDeposu;
    private readonly IAlisFaturaDeposu _alisFaturaDeposu;
    private readonly IFinansHareketDeposu _finansHareketDeposu;
    private readonly IDenetimKayitDeposu _denetimKayitDeposu;

    public OperasyonOzetiServisi(
        IUrunDeposu urunDeposu,
        ICariHesapDeposu cariHesapDeposu,
        IStokDeposu stokDeposu,
        ISatisSiparisDeposu satisSiparisDeposu,
        ISatisFaturaDeposu satisFaturaDeposu,
        IAlisFaturaDeposu alisFaturaDeposu,
        IFinansHareketDeposu finansHareketDeposu,
        IDenetimKayitDeposu denetimKayitDeposu)
    {
        _urunDeposu = urunDeposu;
        _cariHesapDeposu = cariHesapDeposu;
        _stokDeposu = stokDeposu;
        _satisSiparisDeposu = satisSiparisDeposu;
        _satisFaturaDeposu = satisFaturaDeposu;
        _alisFaturaDeposu = alisFaturaDeposu;
        _finansHareketDeposu = finansHareketDeposu;
        _denetimKayitDeposu = denetimKayitDeposu;
    }

    public async Task<OperasyonOzeti> GetirAsync(CancellationToken cancellationToken)
    {
        var urunler = await _urunDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var cariHesaplar = await _cariHesapDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var stokHareketleri = await _stokDeposu.HareketleriListeleAsync(null, null, 10_000, cancellationToken);
        var satisSiparisleri = await _satisSiparisDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var satisFaturalari = await _satisFaturaDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var alisFaturalari = await _alisFaturaDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var finansHareketleri = await _finansHareketDeposu.ListeleAsync(null, 10_000, cancellationToken);
        var denetimKayitlari = await _denetimKayitDeposu.ListeleAsync(null, null, null, 10_000, cancellationToken);

        return new OperasyonOzeti(
            urunler.Count,
            cariHesaplar.Count,
            stokHareketleri.Count,
            satisSiparisleri.Count,
            satisFaturalari.Count,
            alisFaturalari.Count,
            finansHareketleri.Count,
            denetimKayitlari.Count,
            finansHareketleri.Sum(hareket => hareket.BakiyeEtkisi));
    }
}
