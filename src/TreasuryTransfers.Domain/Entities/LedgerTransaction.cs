using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Domain.Interfaces;

namespace TreasuryTransfers.Domain.Entities;

public class LedgerTransaction : IAuditable
{
    public string Id { get; set; } = $"tx-{Guid.NewGuid()}";
    public Guid OperationId { get; set; }
    public required TransferStatus Status { get; set; }
    public required string SourceAccountId { get; set; }
    public required string TargetAccountId { get; set; }
    public decimal AmountDebited { get; set; }
    public decimal AmountCredited { get; set; }
    public DateTime CreatedAt { get; set; }
}