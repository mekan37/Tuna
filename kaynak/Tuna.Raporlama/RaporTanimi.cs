namespace Tuna.Raporlama;

public sealed record RaporTanimi(
    string Code,
    string Title,
    string Module,
    string SourceReference,
    bool RequiresBackgroundExport);
