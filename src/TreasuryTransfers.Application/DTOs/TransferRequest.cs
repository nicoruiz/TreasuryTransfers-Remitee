namespace TreasuryTransfers.Application.DTOs;

public record TransferRequest
(
    Guid OperationId,
    string SourceAccountId,
    string TargetAccountId,
    decimal Amount,
    string Currency,
    decimal? Fx
);