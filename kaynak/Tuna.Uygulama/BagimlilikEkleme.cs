using Microsoft.Extensions.DependencyInjection;

namespace Tuna.Uygulama;

public static class DependencyInjection
{
    public static IServiceCollection AddTunaUygulama(this IServiceCollection services)
    {
        services.AddSingleton<IModulKatalogu, ModulKatalogu>();
        services.AddSingleton<IAktarimPlaniSaglayici, AktarimPlaniSaglayici>();
        services.AddSingleton<IBaslangicHazirlikServisi, BaslangicHazirlikServisi>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<UrunKatalogServisi>();
        services.AddScoped<CariHesapServisi>();
        services.AddScoped<StokServisi>();
        services.AddScoped<SatisSiparisServisi>();
        services.AddScoped<AlisFaturaServisi>();
        services.AddScoped<SatisFaturaServisi>();
        services.AddScoped<FinansServisi>();
        services.AddScoped<DenetimServisi>();
        return services;
    }
}
