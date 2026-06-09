using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Domain.Exceptions;
using TreasuryTransfers.Domain.ValueObjects;

namespace TreasuryTransfers.Application.Validators;

public static class TransferValidator
{
    public static void Validate(TransferRequest request, Account sourceAccount, Account targetAccount)
    {
        ValidateAmount(request.Amount);
        ValidateSupportedCurrency(request.Currency);
        ValidateDifferentAccountIds(request.SourceAccountId, request.TargetAccountId);
        ValidateCurrency(request, sourceAccount);
        ValidateAmountDecimals(sourceAccount.Currency, request.Amount);
        ValidateAccountStatus(sourceAccount);
        ValidateAccountStatus(targetAccount);
        ValidateSufficientBalance(sourceAccount, request.Amount);
        ValidateFx(request, sourceAccount, targetAccount);
    }

    public static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ValidationException("Transfer amount must be greater than zero.");
    }

    public static void ValidateCurrency(TransferRequest request, Account sourceAccount)
    {
        if (!string.Equals(request.Currency, sourceAccount.Currency.Code, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException($"Request currency '{request.Currency}' does not match source account currency '{sourceAccount.Currency.Code}'.");
    }

    public static void ValidateSupportedCurrency(string currency)
    {
        try
        {
            Currency.FromCode(currency);
        }
        catch (ArgumentException)
        {
            throw new ValidationException($"Unsupported currency: '{currency}'.");
        }
    }

    public static void ValidateDifferentAccountIds(string sourceAccountId, string targetAccountId)
    {
        if (sourceAccountId == targetAccountId)
            throw new ValidationException("Source and target accounts must be different.");
    }

    public static void ValidateAccountStatus(Account account)
    {
        if (account.Status == AccountStatus.Frozen)
            throw new ValidationException($"Account '{account.Id}' is frozen and cannot be used for transfers.");
    }

    public static void ValidateSufficientBalance(Account source, decimal amount)
    {
        if (source.Balance < amount)
            throw new ValidationException($"Account '{source.Id}' has insufficient balance.");
    }

    public static void ValidateAmountDecimals(Currency currency, decimal amount)
    {
        if (!currency.IsValidAmount(amount))
            throw new ValidationException($"Amount {amount} has invalid decimal places for currency {currency.Code}.");
    }

    public static void ValidateFx(TransferRequest request, Account sourceAccount, Account targetAccount)
    {
        if (sourceAccount.Currency == targetAccount.Currency && request.Fx.HasValue)
            throw new ValidationException("FX rate should not be provided when source and target accounts have the same currency.");

        if (sourceAccount.Currency != targetAccount.Currency && !request.Fx.HasValue)
            throw new ValidationException("FX rate must be provided when source and target accounts have different currencies.");

        if (request.Fx.HasValue && request.Fx <= 0)
            throw new ValidationException("FX rate must be greater than zero when provided.");
    }
}
