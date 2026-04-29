using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class KatalogEndpointleri
{
    public static IEndpointRouteBuilder MapKatalogEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var katalog = endpoints.MapGroup("/api/katalog").WithTags("Katalog");

        katalog.MapGet("/urunler", async (
            UrunKatalogServisi servis,
            string? arama,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var urunler = await servis.ListeleAsync(arama, limit ?? 50, cancellationToken);
            return TypedResults.Ok(urunler);
        })
        .WithName("UrunleriListele");

        katalog.MapGet("/urunler/{id:guid}", async (
            Guid id,
            UrunKatalogServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.IdIleGetirAsync(id, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Urun bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("UrunGetir");

        katalog.MapPost("/urunler", async (
            UrunOlusturIstegi istek,
            UrunKatalogServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/katalog/urunler/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Urun olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("UrunOlustur");

        return endpoints;
    }
}
