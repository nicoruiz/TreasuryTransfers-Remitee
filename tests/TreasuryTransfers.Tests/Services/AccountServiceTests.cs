using TreasuryTransfers.Application.Services;
using TreasuryTransfers.Domain.Exceptions;
using TreasuryTransfers.Infrastructure.Repositories;

namespace TreasuryTransfers.Tests.Services;

public class AccountServiceTests
{
    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _sut = new AccountService(new AccountsRepository());
    }

    #region GetByIdOrThrowAsync

    [Fact]
    public async Task GetByIdOrThrowAsync_AccountExists_ReturnsAccount()
    {
        var result = await _sut.GetByIdOrThrowAsync("ACC-USD-1");

        Assert.Equal("ACC-USD-1", result.Id);
        Assert.Equal(10000.00m, result.Balance);
    }

    [Fact]
    public async Task GetByIdOrThrowAsync_AccountDoesNotExist_ThrowsNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.GetByIdOrThrowAsync("ACC-INEXISTENTE"));

        Assert.Contains("ACC-INEXISTENTE", ex.Message);
    }

    #endregion

    #region DebitCredit

    [Fact]
    public async Task DebitCredit_WithFxOne_DebitsSourceAndCreditsTarget()
    {
        var source = await _sut.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _sut.GetByIdOrThrowAsync("ACC-USD-2");

        _sut.DebitCredit(source, target, 200m, 1m);

        Assert.Equal(9800m, source.Balance);
        Assert.Equal(700m, target.Balance);
    }

    [Fact]
    public async Task DebitCredit_WithFxGreaterThanOne_AppliesFxToTarget()
    {
        var source = await _sut.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _sut.GetByIdOrThrowAsync("ACC-ARS-1");

        _sut.DebitCredit(source, target, 100m, 1000m);

        Assert.Equal(9900m, source.Balance);
        Assert.Equal(1100000m, target.Balance);
    }

    [Fact]
    public async Task DebitCredit_ZeroAmount_BalancesUnchanged()
    {
        var source = await _sut.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _sut.GetByIdOrThrowAsync("ACC-USD-2");
        var originalSource = source.Balance;
        var originalTarget = target.Balance;

        _sut.DebitCredit(source, target, 0m, 1m);

        Assert.Equal(originalSource, source.Balance);
        Assert.Equal(originalTarget, target.Balance);
    }

    [Fact]
    public async Task DebitCredit_WithClpTarget_ResultHasNoDecimals()
    {
        var source = await _sut.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _sut.GetByIdOrThrowAsync("ACC-CLP-1");

        _sut.DebitCredit(source, target, 50m, 900m);

        Assert.Equal(9950m, source.Balance);
        Assert.Equal(45000m, target.Balance);
        Assert.Equal(0, target.Currency.DecimalPlaces);
    }

    [Fact]
    public async Task DebitCredit_WithClpTarget_RoundsToZeroDecimals()
    {
        var source = await _sut.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _sut.GetByIdOrThrowAsync("ACC-CLP-1");

        // 33.33 * 950 = 31663.50 → CLP rounds to 31664 (banker's rounding)
        _sut.DebitCredit(source, target, 33.33m, 950m);

        Assert.Equal(31664m, target.Balance);
    }

    #endregion
}
