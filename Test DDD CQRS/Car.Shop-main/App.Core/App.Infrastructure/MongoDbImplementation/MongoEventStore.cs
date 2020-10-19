using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Domain;
using App.Core.Events.DomainEvents;
using App.Core.Repository;
using App.Infrastructure.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace App.Infrastructure.MongoDbImplementation
{
    public class MongoEventStore<TA, TId> : IEventStore<TA, TId>
        where TId: IComparable, IEquatable<TId>
        where TA: IAggregateRoot<TId>, new()
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Event> _collection;
        private readonly IMongoClient _mongoClient;
        private readonly EventStoreConfig _config;

        public MongoEventStore(IMongoClient mongoClient, EventStoreConfig eventStoreConfig)
        {
            _mongoClient = mongoClient;
            _db = _mongoClient.GetDatabase("DomainEvents");
            _collection = _db.GetCollection<Event>("Events");
            _config = eventStoreConfig;
        }


        public async Task AppendAsync(IEnumerable<IDomainEvent<TId>> domainEvents, 
            CancellationToken cancellationToken = default) 
        {
            //using var session = await _mongoClient.StartSessionAsync(null, cancellationToken);
            //session.StartTransaction();

            try
            {
                foreach (var domainEvent in domainEvents)
                {
                    var e = MapToMongoEvent(domainEvent);
                    await _collection.InsertOneAsync(e, 
                        null,
                        cancellationToken);
                }
                    
                //await session.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                //await session.AbortTransactionAsync(cancellationToken);
                throw;
            }
        }
        
        public async Task<TA> RestoreAsync(TId id, 
            CancellationToken cancellationToken = default)
        {
            var agr = new TA();
            var query = _collection.Aggregate()
                .Match(x => x.AggregateId.Equals(id)).SortBy(x => x.Version);
            query.Options.BatchSize = _config.BatchSize;
            
            using var cursor = await query.ToCursorAsync(cancellationToken);
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var @event in cursor.Current)
                {
                    agr.ApplyChange(MapToDomainEvent(@event), true);
                }   
            }

            return agr.Version == 0 ? default : agr;
        }

        
        #region Private methods
        private Event MapToMongoEvent(IDomainEvent<TId> domainEvent)
        {
            return new Event()
            {
                Version = domainEvent.Version,
                EventId = domainEvent.EventId,
                AggregateId = domainEvent.AggregateId,
                TimeStamp = domainEvent.TimeStamp,
                EventName = domainEvent.EventName,
                EventType = domainEvent.EventType,
                EventString = domainEvent.EventString,
                CommandMeta = new CommandMetaInternal()
                {
                    CommandId = domainEvent.CommandMeta.CommandId,
                    CorrelationId = domainEvent.CommandMeta.CorrelationId,
                    UserId = domainEvent.CommandMeta.UserId,
                    UserIp = domainEvent.CommandMeta.UserIp
                }
            };
        }
        
        
        private IDomainEvent<TId> MapToDomainEvent(Event e)
        {
            var eventType = Type.GetType(e.EventType); //TODO cache
            var desObj = JsonConvert.DeserializeObject(e.EventString, eventType, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateSetterContractResolver()
            });
            return (IDomainEvent<TId>) desObj;
        }

        #endregion
        
        #region Internal classes
        internal class Event
        {
            [BsonId]
            [BsonRepresentation(BsonType.String)]
            public Guid EventId { get; set; }
            public string EventName { get; set; }
            public string EventType { get; set; }
            public string EventString { get; set; }
            public DateTime TimeStamp { get; set; }
            
            [BsonRepresentation(BsonType.String)]
            public TId AggregateId { get; set; }
            public long Version { get; set; }
            public CommandMetaInternal CommandMeta { get; set; }
        }
        
        internal class CommandMetaInternal
        {
            [BsonRepresentation(BsonType.String)]
            public Guid CommandId { get; set; }
            [BsonRepresentation(BsonType.String)]
            public Guid CorrelationId { get; set; }
            public string UserId { get; set; }
            public string UserIp { get; set; }
        }
        
        #endregion
    }
}