using Microsoft.Extensions.DependencyInjection;
using Tuna.Uygulama;

var services = new ServiceCollection()
    .AddTunaUygulama()
    .BuildServiceProvider();

var planProvider = services.GetRequiredService<IAktarimPlaniSaglayici>();
var moduleCatalog = services.GetRequiredService<IModulKatalogu>();

Console.WriteLine("Tuna DBF aktarim baslangici");
Console.WriteLine();
Console.WriteLine("Moduller:");

foreach (var module in moduleCatalog.GetModules())
{
    Console.WriteLine($"- {module.Schema}: {module.DisplayName} ({string.Join(", ", module.LegacySources)})");
}

Console.WriteLine();
Console.WriteLine("Aktarim plani:");

foreach (var stage in planProvider.GetPlan())
{
    Console.WriteLine($"{stage.Order}. {stage.Name} - {stage.Description}");
}
