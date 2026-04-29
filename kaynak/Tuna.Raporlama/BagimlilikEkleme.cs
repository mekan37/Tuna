using Microsoft.Extensions.DependencyInjection;

namespace Tuna.Raporlama;

public static class DependencyInjection
{
    public static IServiceCollection AddTunaRaporlama(this IServiceCollection services)
    {
        services.AddSingleton<RaporKatalogu>();
        services.AddScoped<OperasyonOzetiServisi>();
        return services;
    }
}
