namespace Tuna.Raporlama;

public sealed record OperasyonOzeti(
    int UrunSayisi,
    int CariHesapSayisi,
    int StokHareketSayisi,
    int SatisSiparisSayisi,
    int SatisFaturaSayisi,
    int AlisFaturaSayisi,
    int FinansHareketSayisi,
    int DenetimKaydiSayisi,
    decimal CariBakiyeToplami);
