namespace Tuna.Raporlama;

public sealed class RaporKatalogu
{
    public IReadOnlyList<RaporTanimi> GetInitialReports() =>
    [
        new("sales.daily", "Gunluk Satis Raporu", "sales", "07-opa-menu-analizi.md > 20.7 EXE Icinde Gorunen Menu/Rapor/Islem Basliklari", false),
        new("inventory.balance", "Stok Bakiye Raporu", "inventory", "04-hedef-mimari-veri-modeli.md > 13. Kabul ve Mutabakat Kriterleri", false),
        new("finance.account_statement", "Cari Ekstre", "finance", "04-hedef-mimari-veri-modeli.md > 13. Kabul ve Mutabakat Kriterleri", false),
        new("tracktrace.pts_reconciliation", "PTS/Karekod Mutabakat Raporu", "tracktrace", "04-hedef-mimari-veri-modeli.md > 13. Kabul ve Mutabakat Kriterleri", true)
    ];
}
