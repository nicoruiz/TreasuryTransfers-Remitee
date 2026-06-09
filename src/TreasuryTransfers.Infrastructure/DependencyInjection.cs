using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TreasuryTransfers.Application.Interfaces.Repositories;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Infrastructure.Persistence;
using TreasuryTransfers.Infrastructure.Repositories;

namespace TreasuryTransfers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IRepository<Account>, AccountRepository>(); // Se usa singleton para mantener el mismo contexto de cuentas en memoria
        services.AddScoped<ILedgerTransactionRepository, LedgerTransactionRepository>();

        return services;
    }
}
