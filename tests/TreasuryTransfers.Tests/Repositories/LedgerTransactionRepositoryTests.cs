using Microsoft.EntityFrameworkCore;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Infrastructure.Persistence;
using TreasuryTransfers.Infrastructure.Repositories;

namespace TreasuryTransfers.Tests.Repositories;

public class LedgerTransactionRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly LedgerTransactionRepository _sut;

    public LedgerTransactionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _sut = new LedgerTransactionRepository(_dbContext);
    }

    private static LedgerTransaction CreateTransaction(Guid? operationId = null) => new()
    {
        OperationId = operationId ?? Guid.NewGuid(),
        Status = TransferStatus.Completed,
        SourceAccountId = "ACC-USD-1",
        TargetAccountId = "ACC-ARS-1",
        AmountDebited = 100m,
        AmountCredited = 100000m,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task AddAsync_AndSave_PersistsTransaction()
    {
        var tx = CreateTransaction();

        await _sut.AddAsync(tx);
        await _sut.SaveChangesAsync();

        Assert.Single(_dbContext.LedgerTransactions);
    }

    [Fact]
    public async Task GetByOperationIdAsync_ExistingOperation_ReturnsTransaction()
    {
        var operationId = Guid.NewGuid();
        var tx = CreateTransaction(operationId);
        await _sut.AddAsync(tx);
        await _sut.SaveChangesAsync();

        var result = await _sut.GetByOperationIdAsync(operationId);

        Assert.NotNull(result);
        Assert.Equal(operationId, result.OperationId);
        Assert.Equal("ACC-USD-1", result.SourceAccountId);
    }

    [Fact]
    public async Task GetByOperationIdAsync_NonExistingOperation_ReturnsNull()
    {
        var result = await _sut.GetByOperationIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_MultipleTransactions_AllPersisted()
    {
        await _sut.AddAsync(CreateTransaction());
        await _sut.AddAsync(CreateTransaction());
        await _sut.SaveChangesAsync();

        Assert.Equal(2, await _dbContext.LedgerTransactions.CountAsync());
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
