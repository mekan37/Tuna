namespace Tuna.Alan;

public enum AktarimAsamaTuru
{
    SaltOkunurAnlikKopya,
    HamSahneleme,
    TipDonusumu,
    HedefeNormalizeAktarim,
    Mutabakat,
    ParalelCalisma,
    CanliGecis
}

public sealed record AktarimAsamasi(
    int Order,
    AktarimAsamaTuru Kind,
    string Name,
    string Description,
    IReadOnlyList<string> AcceptanceChecks);
