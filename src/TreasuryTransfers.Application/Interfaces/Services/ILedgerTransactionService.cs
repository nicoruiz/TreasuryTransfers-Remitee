using TreasuryTransfers.Application.DTOs;

namespace TreasuryTransfers.Application.Interfaces.Services;

public interface ILedgerTransactionService
{
    Task<TransferResult> TransferAsync(TransferRequest request);
    Task<IReadOnlyList<TransferResponse>> GetAllAsync();
}

public record TransferResult(TransferResponse Response, bool IsNew);