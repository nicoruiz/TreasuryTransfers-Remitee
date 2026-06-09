using TreasuryTransfers.Domain.ValueObjects;

namespace TreasuryTransfers.Tests.ValueObjects;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD", 2)]
    [InlineData("ARS", 2)]
    [InlineData("CLP", 0)]
    public void FromCode_SupportedCurrency_ReturnsCorrectDecimalPlaces(string code, int expectedDecimals)
    {
        var currency = Currency.FromCode(code);

        Assert.Equal(code, currency.Code);
        Assert.Equal(expectedDecimals, currency.DecimalPlaces);
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("Clp")]
    public void FromCode_CaseInsensitive_ReturnsCurrency(string code)
    {
        var currency = Currency.FromCode(code);

        Assert.Equal(code.ToUpperInvariant(), currency.Code);
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("")]
    public void FromCode_UnsupportedCurrency_ThrowsArgumentException(string code)
    {
        Assert.Throws<ArgumentException>(() => Currency.FromCode(code));
    }

    [Theory]
    [InlineData("USD", 100.50, true)]
    [InlineData("USD", 100.999, false)]
    [InlineData("CLP", 1500, true)]
    [InlineData("CLP", 1500.50, false)]
    [InlineData("ARS", 250.12, true)]
    [InlineData("ARS", 250.123, false)]
    public void IsValidAmount_ValidatesDecimalPlaces(string code, decimal amount, bool expected)
    {
        var currency = Currency.FromCode(code);

        Assert.Equal(expected, currency.IsValidAmount(amount));
    }

    [Theory]
    [InlineData("CLP", 1500.50, 1500)]
    [InlineData("CLP", 1500.99, 1501)]
    [InlineData("USD", 100.999, 101.00)]
    [InlineData("USD", 100.124, 100.12)]
    public void Round_RoundsToCorrectDecimalPlaces(string code, decimal amount, decimal expected)
    {
        var currency = Currency.FromCode(code);

        Assert.Equal(expected, currency.Round(amount));
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        var currency = Currency.FromCode("USD");

        Assert.Equal("USD", currency.ToString());
    }

    [Fact]
    public void FromCode_SameCode_ReturnsSameInstance()
    {
        var first = Currency.FromCode("USD");
        var second = Currency.FromCode("USD");

        Assert.Same(first, second);
    }
}
