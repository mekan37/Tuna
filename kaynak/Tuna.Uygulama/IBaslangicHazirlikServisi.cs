namespace Tuna.Uygulama;

public interface IBaslangicHazirlikServisi
{
    BaslangicHazirlik GetReadiness();
}

public sealed record BaslangicHazirlik(
    string ApplicationName,
    string RuntimeTarget,
    IReadOnlyList<string> ReadyCapabilities,
    IReadOnlyList<string> ToBeVerified);
