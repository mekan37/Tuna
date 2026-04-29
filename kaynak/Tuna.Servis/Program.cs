using System.Text.Json.Serialization;
using Tuna.Alan;
using Tuna.Altyapi;
using Tuna.Raporlama;
using Tuna.Servis;
using Tuna.Uygulama;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddTunaUygulama();
builder.Services.AddTunaAltyapi(builder.Configuration);
builder.Services.AddSingleton<RaporKatalogu>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapKatalogEndpointleri();
app.MapCariEndpointleri();
app.MapStokEndpointleri();
app.MapSatisEndpointleri();
app.MapAlisEndpointleri();
app.MapSatisFaturaEndpointleri();
app.MapFinansEndpointleri();

app.MapGet("/saglik", () => Results.Ok(new { status = "ok", application = "Tuna" }))
    .WithName("Saglik");

var system = app.MapGroup("/api/sistem").WithTags("Sistem");
system.MapGet("/hazirlik", (IBaslangicHazirlikServisi service) => service.GetReadiness())
    .WithName("HazirlikGetir");
system.MapGet("/moduller", (IModulKatalogu catalog) => catalog.GetModules())
    .WithName("ModulleriGetir");

var migration = app.MapGroup("/api/aktarim").WithTags("Aktarim");
migration.MapGet("/plan", (IAktarimPlaniSaglayici provider) => provider.GetPlan())
    .WithName("AktarimPlaniniGetir");

var reporting = app.MapGroup("/api/raporlar").WithTags("Raporlama");
reporting.MapGet("/", (RaporKatalogu catalog) => catalog.GetInitialReports())
    .WithName("RaporlariGetir");

var outbox = app.MapGroup("/api/kuyruk").WithTags("Kuyruk");
outbox.MapGet("/bekleyen", async (IKuyrukDeposu store, CancellationToken cancellationToken) =>
    await store.GetPendingAsync(50, cancellationToken))
    .WithName("BekleyenKuyrukGetir");
outbox.MapPost("/ornekle", async (IKuyrukDeposu store, CancellationToken cancellationToken) =>
{
    var message = new KuyrukMesaji(
        Guid.NewGuid(),
        "tuna.bootstrap",
        $"bootstrap:{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
        """{"source":"api-seed"}""",
        KuyrukMesajiDurumu.Pending,
        0,
        DateTimeOffset.UtcNow);

    await store.EnqueueAsync(message, cancellationToken);
    return Results.Accepted($"/api/kuyruk/bekleyen", message);
})
.WithName("KuyrukOrnekle");

app.Run();
