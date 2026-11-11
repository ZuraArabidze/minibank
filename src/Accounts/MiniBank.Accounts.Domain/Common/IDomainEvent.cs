using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get;  }
    DateTimeOffset OccurredAt { get; }
}
