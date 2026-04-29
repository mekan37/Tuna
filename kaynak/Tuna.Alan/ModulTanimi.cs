namespace Tuna.Alan;

public sealed record ModulTanimi(
    UygulamaModulu Module,
    string Schema,
    string DisplayName,
    string Responsibility,
    IReadOnlyList<string> LegacySources,
    IReadOnlyList<ReferansKaniti> References);
