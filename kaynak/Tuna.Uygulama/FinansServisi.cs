namespace Tuna.Uygulama;

using Tuna.Alan;

public sealed class FinansServisi
{
    private readonly IFinansHareketDeposu _finansHareketDeposu;
    private readonly ICariHesapDeposu _cariHesapDeposu;
    private readonly DenetimServisi _denetimServisi;
    private readonly TimeProvider _timeProvider;

    public FinansServisi(IFinansHareketDeposu finansHareketDeposu, ICariHesapDeposu cariHesapDeposu, DenetimServisi denetimServisi, TimeProvider timeProvider)
    {
        _finansHareketDeposu = finansHareketDeposu;
        _cariHesapDeposu = cariHesapDeposu;
        _denetimServisi = denetimServisi;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<FinansHareketOzeti>> HareketleriListeleAsync(Guid? cariHesapId, int limit, CancellationToken cancellationToken)
    {
        var guvenliLimit = Math.Clamp(limit, 1, 200);
        var hareketler = await _finansHareketDeposu.ListeleAsync(cariHesapId, guvenliLimit, cancellationToken);
        return hareketler.Select(hareket => hareket.Ozetle()).ToArray();
    }

    public async Task<UygulamaSonucu<CariBakiyeOzeti>> CariBakiyeGetirAsync(Guid cariHesapId, CancellationToken cancellationToken)
    {
        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(cariHesapId, cancellationToken);
        if (cariHesap is null)
        {
            return UygulamaSonucu<CariBakiyeOzeti>.Hata("finans.cari_bulunamadi", "Cari hesap bulunamadi.");
        }

        var hareketler = await _finansHareketDeposu.CariHareketleriAsync(cariHesapId, cancellationToken);
        var borcToplam = hareketler.Sum(hareket => hareket.Borc);
        var alacakToplam = hareketler.Sum(hareket => hareket.Alacak);

        return UygulamaSonucu<CariBakiyeOzeti>.BasariliSonuc(new CariBakiyeOzeti(
            cariHesap.Id,
            cariHesap.Kod,
            cariHesap.Unvan,
            borcToplam,
            alacakToplam,
            borcToplam - alacakToplam));
    }

    public Task<UygulamaSonucu<FinansHareketOzeti>> TahsilatOlusturAsync(FinansHareketOlusturIstegi istek, CancellationToken cancellationToken) =>
        HareketOlusturAsync(istek, FinansHareketTuru.Tahsilat, 0, istek.Tutar, "finans.tahsilat", cancellationToken);

    public Task<UygulamaSonucu<FinansHareketOzeti>> OdemeOlusturAsync(FinansHareketOlusturIstegi istek, CancellationToken cancellationToken) =>
        HareketOlusturAsync(istek, FinansHareketTuru.Odeme, istek.Tutar, 0, "finans.odeme", cancellationToken);

    private async Task<UygulamaSonucu<FinansHareketOzeti>> HareketOlusturAsync(
        FinansHareketOlusturIstegi istek,
        FinansHareketTuru tur,
        decimal borc,
        decimal alacak,
        string varsayilanKaynak,
        CancellationToken cancellationToken)
    {
        var dogrulamaHatasi = Dogrula(istek);
        if (dogrulamaHatasi is not null)
        {
            return UygulamaSonucu<FinansHareketOzeti>.Hata("finans.gecersiz", dogrulamaHatasi);
        }

        var cariHesap = await _cariHesapDeposu.IdIleBulAsync(istek.CariHesapId, cancellationToken);
        if (cariHesap is null)
        {
            return UygulamaSonucu<FinansHareketOzeti>.Hata("finans.cari_bulunamadi", "Cari hesap bulunamadi.");
        }

        var hareket = FinansHareketi.Olustur(
            cariHesap,
            tur,
            borc,
            alacak,
            string.IsNullOrWhiteSpace(istek.Kaynak) ? varsayilanKaynak : istek.Kaynak,
            istek.Aciklama,
            _timeProvider.GetUtcNow());

        await _finansHareketDeposu.EkleAsync(hareket, cancellationToken);
        await _denetimServisi.KaydetAsync(new DenetimKaydiOlusturIstegi(
            "Finans",
            DenetimIslemTuru.Olusturma,
            nameof(FinansHareketi),
            hareket.Id.ToString(),
            hareket.Kaynak,
            $"{hareket.Tur} hareketi olusturuldu. Borc: {hareket.Borc}, Alacak: {hareket.Alacak}"), cancellationToken);

        return UygulamaSonucu<FinansHareketOzeti>.BasariliSonuc(hareket.Ozetle());
    }

    private static string? Dogrula(FinansHareketOlusturIstegi istek)
    {
        if (istek.CariHesapId == Guid.Empty)
        {
            return "Cari hesap id zorunludur.";
        }

        if (istek.Tutar <= 0)
        {
            return "Tutar sifirdan buyuk olmalidir.";
        }

        return null;
    }
}
