using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Accounts;

public readonly record struct AccountId(Guid Value)
{
    public static AccountId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct CustomerId(string Value)
{
    public override string ToString() => Value;
}
