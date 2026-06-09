namespace TreasuryTransfers.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, string id)
        : base($"{entityName} '{id}' not found.") { }
}
