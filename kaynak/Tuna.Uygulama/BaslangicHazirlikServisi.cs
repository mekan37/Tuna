namespace Tuna.Uygulama;

public sealed class BaslangicHazirlikServisi : IBaslangicHazirlikServisi
{
    public BaslangicHazirlik GetReadiness() =>
        new(
            "Tuna",
            "net10.0",
            [
                "Moduler monolit cozum iskeleti",
                "PostgreSQL sema taslagi",
                "Aktarim staging ve mutabakat plani",
                "Isci/kuyruk sozlesmeleri",
                "Rapor metadata baslangici"
            ],
            [
                "C:\\OPA\\OPA.EXE ic davranislari ekran kayitlariyla dogrulanacak",
                "KK* tablolarinin finans mi karekod mu agirlikli oldugu dogrulanacak",
                "CDX index anahtarlari ve tekillik kurallari dogrulanacak",
                "E-fatura/ITS test servis sozlesmeleri netlestirilecek"
            ]);
}
