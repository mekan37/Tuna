using Microsoft.AspNetCore.Mvc;
using Tuna.Uygulama;

namespace Tuna.Servis;

public static class FinansEndpointleri
{
    public static IEndpointRouteBuilder MapFinansEndpointleri(this IEndpointRouteBuilder endpoints)
    {
        var finans = endpoints.MapGroup("/api/finans").WithTags("Finans");

        finans.MapGet("/hareketler", async (
            FinansServisi servis,
            Guid? cariHesapId,
            int? limit,
            CancellationToken cancellationToken) =>
        {
            var hareketler = await servis.HareketleriListeleAsync(cariHesapId, limit ?? 100, cancellationToken);
            return Results.Ok(hareketler);
        })
        .WithName("FinansHareketleriListele");

        finans.MapGet("/cari-bakiye/{cariHesapId:guid}", async (
            Guid cariHesapId,
            FinansServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.CariBakiyeGetirAsync(cariHesapId, cancellationToken);
            return sonuc.Basarili
                ? Results.Ok(sonuc.Deger)
                : Results.NotFound(new ProblemDetails
                {
                    Title = "Cari bakiye bulunamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status404NotFound
                });
        })
        .WithName("CariBakiyeGetir");

        finans.MapPost("/tahsilatlar", async (
            FinansHareketOlusturIstegi istek,
            FinansServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.TahsilatOlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/finans/hareketler", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Tahsilat olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("TahsilatOlustur");

        finans.MapPost("/odemeler", async (
            FinansHareketOlusturIstegi istek,
            FinansServisi servis,
            CancellationToken cancellationToken) =>
        {
            var sonuc = await servis.OdemeOlusturAsync(istek, cancellationToken);
            return sonuc.Basarili
                ? Results.Created($"/api/finans/hareketler", sonuc.Deger)
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Odeme olusturulamadi",
                    Detail = sonuc.HataMesaji,
                    Status = StatusCodes.Status400BadRequest
                });
        })
        .WithName("OdemeOlustur");

        return endpoints;
    }
}
