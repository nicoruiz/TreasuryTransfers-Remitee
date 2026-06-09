using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Application.Interfaces.Repositories;

public interface ILedgerTransactionRepository
{
    Task<LedgerTransaction?> GetByOperationIdAsync(Guid operationId);
    Task<IReadOnlyList<LedgerTransaction>> GetAllAsync();
    Task AddAsync(LedgerTransaction transaction);
    Task SaveChangesAsync();
}
