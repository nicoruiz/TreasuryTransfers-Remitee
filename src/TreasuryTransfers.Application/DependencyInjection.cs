using Microsoft.Extensions.DependencyInjection;
using TreasuryTransfers.Application.Interfaces.Services;
using TreasuryTransfers.Application.Services;

namespace TreasuryTransfers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILedgerTransactionService, LedgerTransactionService>();

        return services;
    }
}
