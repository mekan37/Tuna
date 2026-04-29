using Tuna.Raporlama;

namespace Tuna.Servis;

public static class RaporlamaEndpointleri
{
    public static IEndpointRouteBuilder MapRaporlamaEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var raporlar = endpoints.MapGroup("/api/raporlar").WithTags("Raporlama");

        raporlar.MapGet("/", (RaporKatalogu katalog) => katalog.GetInitialReports())
            .WithName("RaporlariGetir");

        raporlar.MapGet("/operasyon-ozeti", async (
            OperasyonOzetiServisi servis,
            CancellationToken cancellationToken) =>
        {
            var ozet = await servis.GetirAsync(cancellationToken);
            return Results.Ok(ozet);
        })
        .WithName("OperasyonOzetiGetir");

        return endpoints;
    }
}
