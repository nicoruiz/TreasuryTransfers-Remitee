namespace TreasuryTransfers.Application.DTOs;

public record TransferResponse(
    string Id,
    Guid OperationId,
    string Status,
    string SourceAccountId,
    string TargetAccountId,
    decimal AmountDebited,
    decimal AmountCredited,
    DateTime CreatedAt
);