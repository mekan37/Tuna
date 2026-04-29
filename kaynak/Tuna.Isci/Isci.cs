namespace Tuna.Isci;

using Tuna.Uygulama;

public class Isci : BackgroundService
{
    private readonly ILogger<Isci> _logger;
    private readonly IKuyrukDeposu _outboxStore;

    public Isci(ILogger<Isci> logger, IKuyrukDeposu outboxStore)
    {
        _logger = logger;
        _outboxStore = outboxStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var pending = await _outboxStore.GetPendingAsync(25, stoppingToken);
            _logger.LogInformation("Tuna isci kalp atisi. Bekleyen kuyruk mesajlari: {PendingCount}", pending.Count);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
