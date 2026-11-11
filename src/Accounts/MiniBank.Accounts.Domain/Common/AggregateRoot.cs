using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Common;

public abstract class AggregateRoot<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _uncommittedEvents = [];

    public TId Id { get; protected set; } = default!;
    public int Version { get; private set; } = -1;
    public IReadOnlyList<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    public void Load(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            foreach (var e in history) Mutate(e);
        }
    }

    public void Raise(IDomainEvent @event)
    {
        Mutate(@event);
        _uncommittedEvents.Add(@event);
    }

    protected abstract void When(IDomainEvent @event);

    private void Mutate(IDomainEvent @event)
    {
        When(@event);
        Version++;
    }
}
