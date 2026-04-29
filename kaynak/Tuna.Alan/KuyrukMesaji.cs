namespace Tuna.Alan;

public enum KuyrukMesajiDurumu
{
    Pending,
    Processing,
    Completed,
    Failed
}

public sealed record KuyrukMesaji(
    Guid Id,
    string Topic,
    string IdempotencyKey,
    string PayloadJson,
    KuyrukMesajiDurumu Status,
    int AttemptCount,
    DateTimeOffset AvailableAt);
