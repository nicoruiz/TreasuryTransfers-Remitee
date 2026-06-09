using Microsoft.EntityFrameworkCore;
using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Application.Services;
using TreasuryTransfers.Domain.Exceptions;
using TreasuryTransfers.Infrastructure.Persistence;
using TreasuryTransfers.Infrastructure.Repositories;

namespace TreasuryTransfers.Tests.Services;

public class LedgerTransactionServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly LedgerTransactionService _sut;
    private readonly AccountService _accountService;

    public LedgerTransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        var ledgerRepo = new LedgerTransactionRepository(_dbContext);
        _accountService = new AccountService(new AccountRepository());
        _sut = new LedgerTransactionService(ledgerRepo, _accountService);
    }

    private static TransferRequest CreateRequest(
        string sourceId = "ACC-USD-1",
        string targetId = "ACC-USD-2",
        decimal amount = 100m,
        decimal? fx = null,
        Guid? operationId = null) => new(
        operationId ?? Guid.NewGuid(), sourceId, targetId, amount, "USD", fx);

    [Fact]
    public async Task TransferAsync_ValidRequest_DebitsAndCredits()
    {
        var request = CreateRequest(amount: 200m);

        var result = await _sut.TransferAsync(request);

        Assert.True(result.IsNew);
        Assert.Equal("COMPLETED", result.Response.Status);
        Assert.Equal(200m, result.Response.AmountDebited);
        Assert.Equal(200m, result.Response.AmountCredited);

        var source = await _accountService.GetByIdOrThrowAsync("ACC-USD-1");
        var target = await _accountService.GetByIdOrThrowAsync("ACC-USD-2");
        Assert.Equal(9800m, source.Balance);
        Assert.Equal(700m, target.Balance);
    }

    [Fact]
    public async Task TransferAsync_DuplicateOperationId_ReturnsExistingResponse()
    {
        var operationId = Guid.NewGuid();
        var request = CreateRequest(amount: 100m, operationId: operationId);

        var first = await _sut.TransferAsync(request);
        var second = await _sut.TransferAsync(request);

        Assert.True(first.IsNew);
        Assert.False(second.IsNew);
        Assert.Equal(first.Response.Id, second.Response.Id);
        Assert.Equal(first.Response.OperationId, second.Response.OperationId);

        var source = await _accountService.GetByIdOrThrowAsync("ACC-USD-1");
        Assert.Equal(9900m, source.Balance);
    }

    [Fact]
    public async Task TransferAsync_NonExistentAccount_ThrowsNotFoundException()
    {
        var request = CreateRequest(sourceId: "ACC-FAKE");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.TransferAsync(request));
    }

    [Fact]
    public async Task TransferAsync_InsufficientBalance_ThrowsAndRollsBack()
    {
        var request = CreateRequest(sourceId: "ACC-USD-2", targetId: "ACC-USD-1", amount: 9999m);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.TransferAsync(request));

        var source = await _accountService.GetByIdOrThrowAsync("ACC-USD-2");
        Assert.Equal(500m, source.Balance);
    }

    [Fact]
    public async Task TransferAsync_CrossCurrencyWithFx_CalculatesCorrectly()
    {
        var request = CreateRequest(
            sourceId: "ACC-USD-1", targetId: "ACC-ARS-1", amount: 10m, fx: 1000m);

        var result = await _sut.TransferAsync(request);

        Assert.Equal(10m, result.Response.AmountDebited);
        Assert.Equal(10000m, result.Response.AmountCredited);
    }

    [Fact]
    public async Task TransferAsync_CrossCurrencyToClp_RoundsAmountCredited()
    {
        // 33.33 USD * 950 = 31663.50 → CLP has 0 decimals → rounds to 31664
        var request = CreateRequest(
            sourceId: "ACC-USD-1", targetId: "ACC-CLP-1", amount: 33.33m, fx: 950m);

        var result = await _sut.TransferAsync(request);

        Assert.Equal(33.33m, result.Response.AmountDebited);
        Assert.Equal(31664m, result.Response.AmountCredited);
    }

    [Fact]
    public async Task TransferAsync_FrozenAccount_ThrowsValidationException()
    {
        var request = CreateRequest(
            sourceId: "ACC-FROZEN", targetId: "ACC-USD-1", amount: 100m);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.TransferAsync(request));
        Assert.Contains("frozen", ex.Message);
    }

    [Fact]
    public async Task TransferAsync_SameCurrencyWithFx_ThrowsValidationException()
    {
        var request = CreateRequest(
            sourceId: "ACC-USD-1", targetId: "ACC-USD-2", amount: 100m, fx: 1.5m);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.TransferAsync(request));
    }

    [Fact]
    public async Task TransferAsync_DifferentCurrencyWithoutFx_ThrowsValidationException()
    {
        var request = CreateRequest(
            sourceId: "ACC-USD-1", targetId: "ACC-ARS-1", amount: 100m, fx: null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.TransferAsync(request));
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
