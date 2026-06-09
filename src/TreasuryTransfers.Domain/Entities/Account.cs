using TreasuryTransfers.Domain.Enums;
using TreasuryTransfers.Domain.ValueObjects;

namespace TreasuryTransfers.Domain.Entities;

public class Account
{
    public required string Id { get; set; }
    public required Currency Currency { get; set; }
    public decimal Balance { get; set; }
    public required AccountStatus Status { get; set; }
}