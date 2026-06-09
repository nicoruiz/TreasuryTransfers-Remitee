using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Application.Interfaces.Services;

public interface IAccountService
{
    Task<Account> GetByIdOrThrowAsync(string accountId);
    Task<IReadOnlyList<AccountResponse>> GetAllAsync();
    void DebitCredit(Account source, Account target, decimal amount, decimal fx);
}