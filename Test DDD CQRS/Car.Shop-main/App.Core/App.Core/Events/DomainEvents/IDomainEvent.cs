using System;
using App.Core.Commands;

namespace App.Core.Events.DomainEvents
{
    public interface IDomainEvent<TId> : IEvent
    {
        Guid EventId { get; }
        string EventName { get; }
        string EventType { get; }
        DateTime TimeStamp { get; }
        TId AggregateId { get; }
        long Version { get; }
        CommandMeta CommandMeta { get; }
        string EventString { get; }
        void SetVersion(long version);
    }
}