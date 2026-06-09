namespace TreasuryTransfers.Application.DTOs;

public record AccountResponse(
    string Id,
    string Currency,
    decimal Balance,
    string Status
);
