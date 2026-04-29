using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class StokEndpointleri
{
    public static IEndpointRouteBuilder MapStokEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var stok = endpoints.MapGroup("/api/stok").WithTags("Stok");

        stok.MapGet("/hareketler", async (
            StokServisi servis,
            Guid? urunId,
            string? depoKod,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var hareketler = await servis.HareketleriListeleAsync(urunId, depoKod, limit ?? 100, cancellationToken);
            return Results.Ok(hareketler);
        })
        .WithName("StokHareketleriListele");

        stok.MapGet("/bakiye", async (
            Guid urunId,
            string depoKod,
            StokServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.BakiyeGetirAsync(urunId, depoKod, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Stok bakiyesi bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("StokBakiyeGetir");

        stok.MapPost("/hareketler", async (
            StokHareketOlusturIstegi istek,
            StokServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.HareketOlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/stok/hareketler/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Stok hareketi olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("StokHareketOlustur");

        return endpoints;
    }
}
