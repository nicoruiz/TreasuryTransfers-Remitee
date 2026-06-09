using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Interfaces.Services;

namespace TreasuryTransfers.Api.Endpoints;

public static class TransfersEndpoints
{
    public static WebApplication MapTransferEndpoints(this WebApplication app)
    {
        app.MapPost("/transfers", async (TransferRequest request, ILedgerTransactionService service) =>
        {
            var result = await service.TransferAsync(request);

            return result.IsNew
                ? Results.Created($"/transfers/{result.Response.Id}", result.Response)
                : Results.Ok(result.Response);
        });

        app.MapGet("/transfers", async (ILedgerTransactionService service) =>
        {
            var transactions = await service.GetAllAsync();
            return Results.Ok(transactions);
        });

        return app;
    }
}
