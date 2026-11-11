using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Accounts.Events;

public abstract record AccountEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public required AccountId AccountId { get; init; }
}

public sealed record AccountOpened : AccountEvent
{
    public required CustomerId OwnerId { get; init; }
    public required Currency Currency { get; init; }
}

public sealed record MoneyDeposited : AccountEvent
{
    public required Money Amount { get; init; }
    public string? Description { get; init; }
}

public sealed record MoneyWithdrawn : AccountEvent
{
    public required Money Amount { get; init; }
    public string? Description { get; init; }
}

public sealed record AccountClosed : AccountEvent
{
    public string? Reason { get; init; }
}