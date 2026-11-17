using AwesomeAssertions;
using MiniBank.Accounts.Domain.Accounts;
using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Tests;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD", "USD")]
    [InlineData("usd", "USD")]
    [InlineData("  eur ", "EUR")]
    public void From_ValidCode_ReturnsNormalizedCurrency(string input, string expectedCode)
    {
        var currency = Currency.From(input);

        currency.Code.Should().Be(expectedCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("U1D")]
    [InlineData("ლარ")]
    public void From_InvalidCode_Throws(string input)
    {
        var act = () => Currency.From(input);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void WellKnownCurrency_EqualsParsedCurrency()
    {
        Currency.From("gel").Should().Be(Currency.GEL);
    }
}
