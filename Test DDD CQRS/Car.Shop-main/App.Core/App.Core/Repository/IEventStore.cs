using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Domain;
using App.Core.Events.DomainEvents;

namespace App.Core.Repository
{
    public interface IEventStore<TA, TId> where TId : IComparable, IEquatable<TId>
        where TA : IAggregateRoot<TId>
    {
        Task AppendAsync(IEnumerable<IDomainEvent<TId>> domainEvents,
            CancellationToken cancellationToken = default);
        
        Task<TA> RestoreAsync(TId id, CancellationToken cancellationToken = default);
    }
}