using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Application.Mappings;

public static class LedgerTransactionMappings
{
    public static TransferResponse ToResponse(this LedgerTransaction entity)
    {
        return new TransferResponse(
            entity.Id,
            entity.OperationId,
            entity.Status.ToString(),
            entity.SourceAccountId,
            entity.TargetAccountId,
            entity.AmountDebited,
            entity.AmountCredited,
            entity.CreatedAt
        );
    }
}
