using TreasuryTransfers.Application.Interfaces.Repositories;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Domain.ValueObjects;

namespace TreasuryTransfers.Infrastructure.Repositories;

public class AccountsRepository : IRepository<Account>
{
    private readonly IList<Account> _accounts = new List<Account>
    {
        new Account { Id = "ACC-USD-1", Currency = Currency.FromCode("USD"), Balance = 10000.00m, Status = AccountStatus.Active },
        new Account { Id = "ACC-USD-2", Currency = Currency.FromCode("USD"), Balance = 500.00m, Status = AccountStatus.Active },
        new Account { Id = "ACC-ARS-1", Currency = Currency.FromCode("ARS"), Balance = 1000000.00m, Status = AccountStatus.Active },
        new Account { Id = "ACC-CLP-1", Currency = Currency.FromCode("CLP"), Balance = 0, Status = AccountStatus.Active },
        new Account { Id = "ACC-FROZEN", Currency = Currency.FromCode("USD"), Balance = 9999.00m, Status = AccountStatus.Frozen }
    };

    public Task<Account?> GetByIdAsync(string id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);

        return Task.FromResult(account);
    }

    public Task<IReadOnlyList<Account>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Account>>(_accounts.ToList().AsReadOnly());
    }
}