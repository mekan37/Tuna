using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IKuyrukDeposu
{
    Task<IReadOnlyList<KuyrukMesaji>> GetPendingAsync(int take, CancellationToken cancellationToken);

    Task EnqueueAsync(KuyrukMesaji message, CancellationToken cancellationToken);
}
