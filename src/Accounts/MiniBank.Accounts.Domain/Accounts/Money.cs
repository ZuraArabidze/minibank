using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Accounts;

public readonly record struct Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Zero(Currency currency) => new(0m, currency);

    public bool IsPositive => Amount > 0;
    public bool IsZero => Amount == 0;

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static bool operator >(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount < b.Amount;
    }

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException($"Cannot operate on {a.Currency} and {b.Currency} together.");
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
