using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuna.Uygulama;

namespace Tuna.Altyapi;

public static class DependencyInjection
{
    public static IServiceCollection AddTunaAltyapi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresAyarlari>(configuration.GetSection(PostgresAyarlari.SectionName));
        services.AddSingleton<IKuyrukDeposu, BellekKuyrukDeposu>();
        services.AddSingleton<IUrunDeposu, BellekUrunDeposu>();
        return services;
    }
}
