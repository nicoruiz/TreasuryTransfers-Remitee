using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Interfaces.Services;
using TreasuryTransfers.Application.Interfaces.Repositories;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Application.Mappings;
using TreasuryTransfers.Application.Validators;

namespace TreasuryTransfers.Application.Services;

public class LedgerTransactionService : ILedgerTransactionService
{
    private readonly ILedgerTransactionRepository _ledgerTransactionRepository;
    private readonly IAccountService _accountService;

    public LedgerTransactionService(
        ILedgerTransactionRepository ledgerTransactionRepository,
        IAccountService accountService)
    {
        _ledgerTransactionRepository = ledgerTransactionRepository;
        _accountService = accountService;
    }

    public async Task<TransferResult> TransferAsync(TransferRequest request)
    {
        // Step 1: Get accounts
        var sourceAccount = await _accountService.GetByIdOrThrowAsync(request.SourceAccountId);
        var targetAccount = await _accountService.GetByIdOrThrowAsync(request.TargetAccountId);

        // Step 2: Validate
        TransferValidator.Validate(request, sourceAccount, targetAccount);

        // Step 3: Check for existing transaction (idempotency)
        var existingTransaction = await _ledgerTransactionRepository.GetByOperationIdAsync(request.OperationId);
        if (existingTransaction != null)
            return new TransferResult(existingTransaction.ToResponse(), IsNew: false);

        // Step 4: Debit/Credit with rollback on failure
        var fx = request.Fx ?? 1m;
        var originalSourceBalance = sourceAccount.Balance;
        var originalTargetBalance = targetAccount.Balance;

        try
        {
            _accountService.DebitCredit(sourceAccount, targetAccount, request.Amount, fx);

            // Step 5: Create a new LedgerTransaction
            var amountCredited = targetAccount.Currency.Round(request.Amount * fx);
            var newTransaction = new LedgerTransaction
            {
                OperationId = request.OperationId,
                Status = TransferStatus.Completed,
                SourceAccountId = request.SourceAccountId,
                TargetAccountId = request.TargetAccountId,
                AmountDebited = request.Amount,
                AmountCredited = amountCredited,
                CreatedAt = DateTime.UtcNow
            };

            await _ledgerTransactionRepository.AddAsync(newTransaction);
            await _ledgerTransactionRepository.SaveChangesAsync();

            return new TransferResult(newTransaction.ToResponse(), IsNew: true);
        }
        catch // If something fails, we rollback the account balances to maintain consistency
        {
            sourceAccount.Balance = originalSourceBalance;
            targetAccount.Balance = originalTargetBalance;
            throw;
        }
    }

    public async Task<IReadOnlyList<TransferResponse>> GetAllAsync()
    {
        var transactions = await _ledgerTransactionRepository.GetAllAsync();
        return transactions.Select(t => t.ToResponse()).ToList().AsReadOnly();
    }
}