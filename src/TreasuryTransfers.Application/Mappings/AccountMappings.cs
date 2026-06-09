using TreasuryTransfers.Application.DTOs;
using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Application.Mappings;

public static class AccountMappings
{
    public static AccountResponse ToResponse(this Account entity)
    {
        return new AccountResponse(
            entity.Id,
            entity.Currency.Code,
            entity.Balance,
            entity.Status.ToString()
        );
    }
}
