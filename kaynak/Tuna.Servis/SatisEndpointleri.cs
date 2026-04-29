using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class SatisEndpointleri
{
    public static IEndpointRouteBuilder MapSatisEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var satis = endpoints.MapGroup("/api/satis").WithTags("Satis");

        satis.MapGet("/siparisler", async (
            SatisSiparisServisi servis,
            Guid? cariHesapId,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var siparisler = await servis.ListeleAsync(cariHesapId, limit ?? 50, cancellationToken);
            return Results.Ok(siparisler);
        })
        .WithName("SatisSiparisleriListele");

        satis.MapGet("/siparisler/{id:guid}", async (
            Guid id,
            SatisSiparisServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.IdIleGetirAsync(id, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Satis siparisi bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("SatisSiparisGetir");

        satis.MapPost("/siparisler", async (
            SatisSiparisOlusturIstegi istek,
            SatisSiparisServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/satis/siparisler/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Satis siparisi olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("SatisSiparisOlustur");

        return endpoints;
    }
}
