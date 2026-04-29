using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class CariEndpointleri
{
    public static IEndpointRouteBuilder MapCariEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var cari = endpoints.MapGroup("/api/cari").WithTags("Cari");

        cari.MapGet("/hesaplar", async (
            CariHesapServisi servis,
            string? arama,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var hesaplar = await servis.ListeleAsync(arama, limit ?? 50, cancellationToken);
            return Results.Ok(hesaplar);
        })
        .WithName("CariHesaplariListele");

        cari.MapGet("/hesaplar/{id:guid}", async (
            Guid id,
            CariHesapServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.IdIleGetirAsync(id, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Cari hesap bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("CariHesapGetir");

        cari.MapPost("/hesaplar", async (
            CariHesapOlusturIstegi istek,
            CariHesapServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/cari/hesaplar/{sonuc.Deger!.Id}", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Cari hesap olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("CariHesapOlustur");

        return endpoints;
    }
}
