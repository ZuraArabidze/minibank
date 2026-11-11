using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Accounts;

public readonly record struct Currency
{
    public static readonly Currency GEL = new("GEL");
    public static readonly Currency USD = new("USD");
    public static readonly Currency EUR = new("EUR");
    public static readonly Currency GBP = new("GBP");

    private static readonly HashSet<string> Supported = ["GEL", "USD", "EUR", "GBP"];

    public string Code { get; }

    private Currency(string code) => Code = code;

    public static Currency From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Currency code is required.");

        var normalized = code.Trim().ToUpperInvariant();
        if (!Supported.Contains(normalized))
            throw new DomainException($"Currency '{code}' is not supported.");

        return new Currency(normalized);
    }

    public override string ToString() => Code;
}

