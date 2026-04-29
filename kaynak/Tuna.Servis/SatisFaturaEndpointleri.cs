using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class SatisFaturaEndpointleri
{
    public static IEndpointRouteBuilder MapSatisFaturaEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var fatura = endpoints.MapGroup("/api/satis/faturalar").WithTags("Satis");

        fatura.MapGet("/", async (
            SatisFaturaServisi servis,
            Guid? cariHesapId,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var faturalar = await servis.ListeleAsync(cariHesapId, limit ?? 50, cancellationToken);
            return Results.Ok(faturalar);
        })
        .WithName("SatisFaturalariListele");

        fatura.MapGet("/{id:guid}", async (
            Guid id,
            SatisFaturaServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.IdIleGetirAsync(id, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Satis faturasi bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("SatisFaturaGetir");

        fatura.MapPost("/", async (
            SatisFaturaOlusturIstegi istek,
            SatisFaturaServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/satis/faturalar/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Satis faturasi olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("SatisFaturaOlustur");

        return endpoints;
    }
}
