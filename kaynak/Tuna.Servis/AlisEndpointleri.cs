using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class AlisEndpointleri
{
    public static IEndpointRouteBuilder MapAlisEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var alis = endpoints.MapGroup("/api/alis").WithTags("Alis");

        alis.MapGet("/faturalar", async (
            AlisFaturaServisi servis,
            Guid? cariHesapId,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var faturalar = await servis.ListeleAsync(cariHesapId, limit ?? 50, cancellationToken);
            return Results.Ok(faturalar);
        })
        .WithName("AlisFaturalariListele");

        alis.MapGet("/faturalar/{id:guid}", async (
            Guid id,
            AlisFaturaServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.IdIleGetirAsync(id, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Alis faturasi bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("AlisFaturaGetir");

        alis.MapPost("/faturalar", async (
            AlisFaturaOlusturIstegi istek,
            AlisFaturaServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/alis/faturalar/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Alis faturasi olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("AlisFaturaOlustur");

        return endpoints;
    }
}
