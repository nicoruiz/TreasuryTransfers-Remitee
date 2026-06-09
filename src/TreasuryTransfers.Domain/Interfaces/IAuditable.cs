namespace TreasuryTransfers.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
}