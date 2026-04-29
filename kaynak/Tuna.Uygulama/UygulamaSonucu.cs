namespace Tuna.Uygulama;

public sealed record UygulamaSonucu<T>(T? Deger, string? HataKodu, string? HataMesaji)
{
    public bool Basarili => HataKodu is null;

    public static UygulamaSonucu<T> BasariliSonuc(T deger) => new(deger, null, null);

    public static UygulamaSonucu<T> Hata(string hataKodu, string hataMesaji) => new(default, hataKodu, hataMesaji);
}
