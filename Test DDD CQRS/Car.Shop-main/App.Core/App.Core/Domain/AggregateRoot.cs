using System;
using System.Collections.Generic;
using App.Core.Domain.Exceptions;
using App.Core.Events.DomainEvents;
using App.Core.Helpers;

namespace App.Core.Domain
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
        where TId: IComparable, IEquatable<TId>
    {
        public override TId Id { get; protected set; }
        public long Version { get; private set; }
        
        private Queue<IDomainEvent<TId>> _events = new Queue<IDomainEvent<TId>>();
        
        protected AggregateRoot() { }

        protected AggregateRoot(TId id)
        {
            Id = id;
        }
        public void ApplyChange(IDomainEvent<TId> domainEvent, bool isFromHistory = false)
        {
            lock (_events)
            {
                InnerApply(domainEvent);
                if (!isFromHistory)
                {
                    _events.Enqueue(domainEvent);
                    domainEvent.SetVersion(Version + 1);
                }
                if(Version + 1 != domainEvent.Version)
                    throw new AggregateVersioningException(
                        $"DomainEvent({domainEvent.GetType().Name}) version {domainEvent.Version} is not valid, next version should be {Version + 1}");
                Version++;
            }
        }

        public IDomainEvent<TId>[] GetUncommittedEvents()
        {
            lock (_events)
            {
                return _events.ToArray();
            }
        }

        public IDomainEvent<TId>[] FlushUncommittedEvents()
        {
            lock (_events)
            {
                var events = _events.ToArray();
                _events.Clear();
                return events;
            }
        }

        public void LoadFromHistory(IEnumerable<IDomainEvent<TId>> eventStream)
        {
            lock (_events)
            {
                foreach (var @event in eventStream)
                    ApplyChange(@event, true);
            }
        }

        private void InnerApply(IDomainEvent<TId> domainEvent)
        {
            this.Invoke("Apply", domainEvent);
        }


        /*
        public override bool Equals(object obj)
        {
            return obj is AggregateRoot<TId> agr &&
                   GetType() == agr.GetType() &&
                   EqualityComparer<TId>.Default.Equals(Id, agr.Id);
        }
        
        public override int GetHashCode() 
            => HashCode.Combine(GetType(), Id);
        
        public static bool operator ==(AggregateRoot<TId> agr1, AggregateRoot<TId> agr2) 
            => EqualityComparer<AggregateRoot<TId>>.Default.Equals(agr1, agr2);

        public static bool operator !=(AggregateRoot<TId> agr1, AggregateRoot<TId> agr2) 
            => !(agr1 == agr2);
            */
    }
}