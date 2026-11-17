using AwesomeAssertions;
using MiniBank.Accounts.Domain.Accounts;
using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Add_SameCurrency_ReturnSUm()
    {
        //Arrange
        var a = new Money(10.10m, Currency.GEL);
        var b = new Money(2.20m, Currency.GEL);

        //Act
        var result = a + b;

        //Assert
        result.Should().Be(new Money(12.30m, Currency.GEL));
    }

    [Fact]
    public void Add_DifferentCurrencies_ThrowsException()
    {
        //Arrange
        var gel = new Money(10.10m, Currency.GEL);
        var usd = new Money(2.20m, Currency.USD);

        //Act
        var act = () => gel + usd;

        //Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Substract_SameCurrency_ReturnDifference()
    {
        //Arrange
        var a = new Money(10.10m, Currency.GEL);
        var b = new Money(2.20m, Currency.GEL);

        //Act
        var result = a - b;

        //Assert
        result.Should().Be(new Money(7.90m, Currency.GEL));
    }

    [Fact]
    public void GreaterThan_ComparesAmounts()
    {
        //Arrange
        var bigger = new Money(10, Currency.GEL);
        var smaller = new Money(5, Currency.GEL);

        //Act
        var result1 = bigger > smaller;
        var result2 = smaller > bigger;

        //Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Fact]
    public void Constructor_RoundsToTwoDecimals_UsingBankersRounding()
    {
        //Arrange
        var amount1 = 10.125m;
        var amount2 = 1.23m;
        var amount3 = 1.00m;

        //Act
        var money1 = new Money(amount1, Currency.GEL);
        var money2 = new Money(amount2, Currency.GEL);
        var money3 = new Money(amount3, Currency.GEL);

        //Assert
        money1.Amount.Should().Be(10.12m);
        money2.Amount.Should().Be(1.23m);
        money3.Amount.Should().Be(1.00m);
    }

    [Fact]
    public void Equality_IsByValue()
    {
        //Arrange
        var money1 = new Money(5, Currency.USD);
        var money2 = new Money(5.00m, Currency.USD);
        var money3 = new Money(5, Currency.GEL);

        //Act
        var result1 = money1 == money2;
        var result2 = money1 == money3;

        //Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
}
