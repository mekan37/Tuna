using System.Collections.Concurrent;
using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public sealed class BellekKuyrukDeposu : IKuyrukDeposu
{
    private readonly ConcurrentDictionary<string, KuyrukMesaji> _messages = new();

    public Task<IReadOnlyList<KuyrukMesaji>> GetPendingAsync(int take, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var pending = _messages.Values
            .Where(message => message.Status == KuyrukMesajiDurumu.Pending && message.AvailableAt <= now)
            .OrderBy(message => message.AvailableAt)
            .Take(take)
            .ToArray();

        return Task.FromResult<IReadOnlyList<KuyrukMesaji>>(pending);
    }

    public Task EnqueueAsync(KuyrukMesaji message, CancellationToken cancellationToken)
    {
        _messages.TryAdd(message.IdempotencyKey, message);
        return Task.CompletedTask;
    }
}
