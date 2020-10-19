using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Domain;
using App.Core.Domain.Exceptions;
using App.Core.Events;
using App.Core.MessageBroker;
using App.Core.Repository;

namespace App.Infrastructure
{
    public class AggregateRepository<TA, TId> : IAggregateRepository<TA, TId>
    where TId: IComparable, IEquatable<TId>
    where TA: IAggregateRoot<TId>
    {
        private readonly IEventStore<TA, TId> _eventStore;
        private readonly IEventProducer _eventPublisher;

        public AggregateRepository(IEventStore<TA, TId> eventStore, 
            IEventProducer eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }
        
        
        public async Task SaveAsync(TA aggregateRoot, CancellationToken cancellationToken = default)
        {
            var events = aggregateRoot.FlushUncommittedEvents();
            if (!events.Any())
                return;
            
            await _eventStore.AppendAsync(events, cancellationToken);

            try
            {
                foreach (var @event in events)
                {
                    await _eventPublisher.PublishAsync(@event, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<TA> GetAsync(TId id, long? expectedVersion = null, CancellationToken cancellationToken = default)
        {
            var agr = await _eventStore.RestoreAsync(id, cancellationToken);
            if (agr.Equals(default))
                return agr;

            if (null != expectedVersion)
                CheckExpectedVersion(expectedVersion.Value, agr.Version);
            return agr;
        }

        private void CheckExpectedVersion(long expectedVersion, long aggregateVersion)
        {
            if (aggregateVersion != expectedVersion)
                throw new AggregateConcurrencyException(typeof(TA));
        }
    }
}