using Tuna.Uygulama;

namespace Tuna.Servis;

public static class DenetimEndpointleri
{
    public static IEndpointRouteBuilder MapDenetimEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var denetim = endpoints.MapGroup("/api/denetim").WithTags("Denetim");

        denetim.MapGet("/kayitlar", async (
            DenetimServisi servis,
            string? modul,
            string? varlikTuru,
            string? varlikId,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var kayitlar = await servis.ListeleAsync(modul, varlikTuru, varlikId, limit ?? 100, cancellationToken);
            return Results.Ok(kayitlar);
        })
        .WithName("DenetimKayitlariListele");

        return endpoints;
    }
}
