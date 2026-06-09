using TreasuryTransfers.Application.Interfaces.Services;

namespace TreasuryTransfers.Api.Endpoints;

public static class AccountEndpoints
{
    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("/accounts", async (IAccountService service) =>
        {
            var accounts = await service.GetAllAsync();
            return Results.Ok(accounts);
        });

        return app;
    }
}
