using Microsoft.EntityFrameworkCore;
using TreasuryTransfers.Application.Interfaces.Repositories;
using TreasuryTransfers.Domain.Entities;
using TreasuryTransfers.Infrastructure.Persistence;

namespace TreasuryTransfers.Infrastructure.Repositories;

public class LedgerTransactionRepository : ILedgerTransactionRepository
{
    private readonly AppDbContext _dbContext;

    public LedgerTransactionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LedgerTransaction transaction)
    {
        await _dbContext.LedgerTransactions.AddAsync(transaction);
    }

    public async Task<LedgerTransaction?> GetByOperationIdAsync(Guid operationId)
    {
        return await _dbContext.LedgerTransactions
            .FirstOrDefaultAsync(t => t.OperationId == operationId);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
