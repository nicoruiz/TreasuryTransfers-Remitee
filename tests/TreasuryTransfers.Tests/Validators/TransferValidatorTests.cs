using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Validators;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Domain.Exceptions;
using TreasuryTransfers.Domain.ValueObjects;

namespace TreasuryTransfers.Tests.Validators;

public class TransferValidatorTests
{
    private static Account CreateAccount(
        string id = "ACC-USD-1",
        string currency = "USD",
        decimal balance = 10000m,
        AccountStatus status = AccountStatus.Active) => new()
    {
        Id = id,
        Currency = Currency.FromCode(currency),
        Balance = balance,
        Status = status
    };

    private static TransferRequest CreateRequest(
        string sourceId = "ACC-USD-1",
        string targetId = "ACC-ARS-1",
        decimal amount = 100m,
        decimal? fx = 1000m) => new(
        Guid.NewGuid(), sourceId, targetId, amount, "USD", fx);

    #region ValidateAmount

    [Fact]
    public void ValidateAmount_PositiveAmount_DoesNotThrow()
    {
        TransferValidator.ValidateAmount(100m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void ValidateAmount_ZeroOrNegative_ThrowsValidationException(decimal amount)
    {
        var ex = Assert.Throws<ValidationException>(() => TransferValidator.ValidateAmount(amount));
        Assert.Contains("greater than zero", ex.Message);
    }

    #endregion

    #region ValidateAmountDecimals

    [Fact]
    public void ValidateAmountDecimals_ValidDecimalsForUsd_DoesNotThrow()
    {
        TransferValidator.ValidateAmountDecimals(Currency.FromCode("USD"), 100.50m);
    }

    [Fact]
    public void ValidateAmountDecimals_TooManyDecimalsForUsd_Throws()
    {
        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateAmountDecimals(Currency.FromCode("USD"), 100.123m));
        Assert.Contains("invalid decimal places", ex.Message);
    }

    [Fact]
    public void ValidateAmountDecimals_IntegerAmountForClp_DoesNotThrow()
    {
        TransferValidator.ValidateAmountDecimals(Currency.FromCode("CLP"), 5000m);
    }

    [Fact]
    public void ValidateAmountDecimals_DecimalAmountForClp_Throws()
    {
        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateAmountDecimals(Currency.FromCode("CLP"), 5000.50m));
        Assert.Contains("invalid decimal places", ex.Message);
        Assert.Contains("CLP", ex.Message);
    }

    #endregion

    #region ValidateDifferentAccountIds

    [Fact]
    public void ValidateDifferentAccountIds_DifferentIds_DoesNotThrow()
    {
        TransferValidator.ValidateDifferentAccountIds("ACC-1", "ACC-2");
    }

    [Fact]
    public void ValidateDifferentAccountIds_SameId_Throws()
    {
        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateDifferentAccountIds("ACC-1", "ACC-1"));
        Assert.Contains("must be different", ex.Message);
    }

    #endregion

    #region ValidateSupportedCurrency

    [Fact]
    public void ValidateSupportedCurrency_ValidCurrency_DoesNotThrow()
    {
        TransferValidator.ValidateSupportedCurrency("USD");
    }

    [Fact]
    public void ValidateSupportedCurrency_InvalidCurrency_Throws()
    {
        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateSupportedCurrency("XYZ"));
        Assert.Contains("Unsupported currency", ex.Message);
    }

    #endregion

    #region ValidateAccountStatus

    [Fact]
    public void ValidateAccountStatus_ActiveAccount_DoesNotThrow()
    {
        var account = CreateAccount(status: AccountStatus.Active);
        TransferValidator.ValidateAccountStatus(account);
    }

    [Fact]
    public void ValidateAccountStatus_FrozenAccount_Throws()
    {
        var account = CreateAccount(id: "ACC-FROZEN", status: AccountStatus.Frozen);

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateAccountStatus(account));
        Assert.Contains("ACC-FROZEN", ex.Message);
        Assert.Contains("frozen", ex.Message);
    }

    #endregion

    #region ValidateSufficientBalance

    [Fact]
    public void ValidateSufficientBalance_EnoughBalance_DoesNotThrow()
    {
        var source = CreateAccount(balance: 1000m);
        TransferValidator.ValidateSufficientBalance(source, 500m);
    }

    [Fact]
    public void ValidateSufficientBalance_ExactBalance_DoesNotThrow()
    {
        var source = CreateAccount(balance: 1000m);
        TransferValidator.ValidateSufficientBalance(source, 1000m);
    }

    [Fact]
    public void ValidateSufficientBalance_InsufficientBalance_Throws()
    {
        var source = CreateAccount(id: "ACC-BROKE", balance: 100m);

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateSufficientBalance(source, 200m));
        Assert.Contains("insufficient balance", ex.Message);
        Assert.Contains("ACC-BROKE", ex.Message);
    }

    #endregion

    #region ValidateFx

    [Fact]
    public void ValidateFx_SameCurrencyNoFx_DoesNotThrow()
    {
        var source = CreateAccount("ACC-1", "USD");
        var target = CreateAccount("ACC-2", "USD");
        var request = CreateRequest(fx: null);

        TransferValidator.ValidateFx(request, source, target);
    }

    [Fact]
    public void ValidateFx_SameCurrencyWithFx_Throws()
    {
        var source = CreateAccount("ACC-1", "USD");
        var target = CreateAccount("ACC-2", "USD");
        var request = CreateRequest(fx: 1.5m);

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateFx(request, source, target));
        Assert.Contains("should not be provided", ex.Message);
    }

    [Fact]
    public void ValidateFx_DifferentCurrencyWithFx_DoesNotThrow()
    {
        var source = CreateAccount("ACC-1", "USD");
        var target = CreateAccount("ACC-2", "ARS");
        var request = CreateRequest(fx: 1000m);

        TransferValidator.ValidateFx(request, source, target);
    }

    [Fact]
    public void ValidateFx_DifferentCurrencyNoFx_Throws()
    {
        var source = CreateAccount("ACC-1", "USD");
        var target = CreateAccount("ACC-2", "ARS");
        var request = CreateRequest(fx: null);

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateFx(request, source, target));
        Assert.Contains("must be provided", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidateFx_ZeroOrNegativeFx_Throws(decimal fx)
    {
        var source = CreateAccount("ACC-1", "USD");
        var target = CreateAccount("ACC-2", "ARS");
        var request = CreateRequest(fx: fx);

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateFx(request, source, target));
        Assert.Contains("greater than zero", ex.Message);
    }

    #endregion

    #region ValidateCurrency

    [Fact]
    public void ValidateCurrency_MatchingCurrency_DoesNotThrow()
    {
        var source = CreateAccount("ACC-1", "USD");
        var request = CreateRequest(fx: null);

        TransferValidator.ValidateCurrency(request, source);
    }

    [Fact]
    public void ValidateCurrency_MismatchedCurrency_Throws()
    {
        var source = CreateAccount("ACC-1", "ARS");
        var request = CreateRequest(fx: null); // request.Currency = "USD"

        var ex = Assert.Throws<ValidationException>(
            () => TransferValidator.ValidateCurrency(request, source));
        Assert.Contains("does not match", ex.Message);
    }

    #endregion

    #region Validate (integration)

    [Fact]
    public void Validate_ValidRequest_DoesNotThrow()
    {
        var source = CreateAccount("ACC-1", "USD", 10000m);
        var target = CreateAccount("ACC-2", "ARS", 500m);
        var request = CreateRequest("ACC-1", "ACC-2", 100m, 1000m);

        TransferValidator.Validate(request, source, target);
    }

    [Fact]
    public void Validate_FrozenSource_Throws()
    {
        var source = CreateAccount("ACC-1", "USD", 10000m, AccountStatus.Frozen);
        var target = CreateAccount("ACC-2", "ARS", 500m);
        var request = CreateRequest("ACC-1", "ACC-2", 100m, 1000m);

        Assert.Throws<ValidationException>(
            () => TransferValidator.Validate(request, source, target));
    }

    [Fact]
    public void Validate_FrozenTarget_Throws()
    {
        var source = CreateAccount("ACC-1", "USD", 10000m);
        var target = CreateAccount("ACC-2", "ARS", 500m, AccountStatus.Frozen);
        var request = CreateRequest("ACC-1", "ACC-2", 100m, 1000m);

        Assert.Throws<ValidationException>(
            () => TransferValidator.Validate(request, source, target));
    }

    #endregion
}
