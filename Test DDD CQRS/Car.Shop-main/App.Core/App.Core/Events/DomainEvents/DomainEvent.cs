using System;
using App.Core.Commands;
using App.Core.Domain;
using Newtonsoft.Json;

namespace App.Core.Events.DomainEvents
{
    public abstract class DomainEvent<TA, TId> : IDomainEvent<TId> where TA: IAggregateRoot<TId> 
    where TId: IComparable, IEquatable<TId>
    {
        public Guid EventId { get; private set; }
        public string EventName { get; private set; }
        public string EventType { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public TId AggregateId { get; private set; }
        public long Version { get; private set; }
        public CommandMeta CommandMeta { get; private set; }
        
        [JsonIgnore]
        public string EventString => JsonConvert.SerializeObject(this);
        public void SetVersion(long version) => Version = version;
        protected DomainEvent() { }
        protected DomainEvent(TA aggregateRoot, ICommand command)
        {
            EventId = Guid.NewGuid();
            EventName = GetType().Name;
            EventType = GetType().AssemblyQualifiedName;
            TimeStamp = DateTime.UtcNow;
            CommandMeta = command.CommandMeta;
            if (AggregateId.Equals(default))
                AggregateId = aggregateRoot.Id;
        }
    }
}