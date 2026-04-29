namespace Tuna.Alan;

public enum KanitSeviyesi
{
    Kesin = 1,
    GucluCikarim = 2,
    Dogrulanacak = 3
}

public sealed record ReferansKaniti(
    string Document,
    string Section,
    KanitSeviyesi Level,
    string Note);
