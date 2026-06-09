namespace TreasuryTransfers.Domain.ValueObjects;

public class Currency
{
    public string Code { get; }
    public int DecimalPlaces { get; }

    private static readonly Dictionary<string, Currency> _currencies = new()
    {
        ["USD"] = new Currency("USD", 2),
        ["ARS"] = new Currency("ARS", 2),
        ["CLP"] = new Currency("CLP", 0)
    };

    private Currency(string code, int decimalPlaces)
    {
        Code = code;
        DecimalPlaces = decimalPlaces;
    }

    public static Currency FromCode(string code)
    {
        if (!_currencies.TryGetValue(code.ToUpperInvariant(), out var currency))
            throw new ArgumentException($"Unsupported currency: {code}");

        return currency;
    }

    public bool IsValidAmount(decimal amount) =>
        amount == Math.Round(amount, DecimalPlaces);

    public decimal Round(decimal amount) =>
        Math.Round(amount, DecimalPlaces);

    public override string ToString() => Code;
}
