using TreasuryTransfers.Application.DTOs;

namespace TreasuryTransfers.Application.Interfaces.Services;

public interface ILedgerTransactionService
{
    Task<TransferResponse> TransferAsync(TransferRequest request);
}