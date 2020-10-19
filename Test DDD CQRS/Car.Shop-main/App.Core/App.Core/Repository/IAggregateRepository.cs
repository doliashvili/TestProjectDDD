using System;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Domain;

namespace App.Core.Repository
{
    public interface IAggregateRepository<TA, TId> 
        where TId: IComparable, IEquatable<TId>
        where TA: IAggregateRoot<TId>
    {
        Task SaveAsync(TA aggregateRoot, CancellationToken cancellationToken = default);
        Task<TA> GetAsync(TId id, long? expectedVersion = null, CancellationToken cancellationToken = default);
    }
}