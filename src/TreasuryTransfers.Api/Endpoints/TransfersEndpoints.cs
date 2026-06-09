using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Interfaces.Services;

namespace TreasuryTransfers.Api.Endpoints;

public static class TransfersEndpoints
{
    public static WebApplication MapTransferEndpoints(this WebApplication app)
    {
        app.MapPost("/transfers", async (TransferRequest request, ILedgerTransactionService service) =>
        {
            var response = await service.TransferAsync(request);
            
            return Results.Created($"/transfers/{response.Id}", response);
        });

        return app;
    }
}
