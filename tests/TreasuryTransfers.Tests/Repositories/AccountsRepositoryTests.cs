using TreasuryTransfers.Infrastructure.Repositories;

namespace TreasuryTransfers.Tests.Repositories;

public class AccountsRepositoryTests
{
    private readonly AccountsRepository _sut = new();

    [Theory]
    [InlineData("ACC-USD-1")]
    [InlineData("ACC-USD-2")]
    [InlineData("ACC-ARS-1")]
    [InlineData("ACC-CLP-1")]
    [InlineData("ACC-FROZEN")]
    public async Task GetByIdAsync_ExistingId_ReturnsAccount(string accountId)
    {
        var result = await _sut.GetByIdAsync(accountId);

        Assert.NotNull(result);
        Assert.Equal(accountId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync("ACC-INEXISTENTE");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_AccountHasExpectedCurrency()
    {
        var usd = await _sut.GetByIdAsync("ACC-USD-1");
        var ars = await _sut.GetByIdAsync("ACC-ARS-1");
        var clp = await _sut.GetByIdAsync("ACC-CLP-1");

        Assert.Equal("USD", usd!.Currency.Code);
        Assert.Equal("ARS", ars!.Currency.Code);
        Assert.Equal("CLP", clp!.Currency.Code);
    }

    [Fact]
    public async Task GetByIdAsync_FrozenAccount_HasFrozenStatus()
    {
        var frozen = await _sut.GetByIdAsync("ACC-FROZEN");

        Assert.Equal(Domain.Enums.AccountStatus.Frozen, frozen!.Status);
    }
}
