using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Interfaces.Repositories;
using TreasuryTransfers.Application.Interfaces.Services;
using TreasuryTransfers.Application.Mappings;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Exceptions;

namespace TreasuryTransfers.Application.Services;

public class AccountService : IAccountService
{
    private readonly IRepository<Account> _accountRepository;

    public AccountService(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> GetByIdOrThrowAsync(string accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account is null)
            throw new NotFoundException("Account", accountId);

        return account;
    }

    public async Task<IReadOnlyList<AccountResponse>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(a => a.ToResponse()).ToList().AsReadOnly();
    }

    public void DebitCredit(Account source, Account target, decimal amount, decimal fx)
    {
        var credited = target.Currency.Round(amount * fx);
        source.Balance -= amount;
        target.Balance += credited;
    }
}