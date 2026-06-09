using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Application.Interfaces.Services;

public interface IAccountService
{
    Task<Account> GetByIdOrThrowAsync(string accountId);
    void DebitCredit(Account source, Account target, decimal amount, decimal fx);
}