using System;
using System.Linq;
using System.Reflection;
using App.Core.Events;
using App.Core.MessageBroker;
using App.Core.Repository;
using App.Infrastructure.Helpers;
using App.Infrastructure.MongoDbImplementation;
using App.Infrastructure.RabbitMq;
using App.Infrastructure.RavenDbImplementation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace App.Infrastructure
{
    public static class DependencyInjection
    {
        internal class CustomSerializationProvider : IBsonSerializationProvider
        {
            private static readonly IBsonSerializer<decimal> DecimalSerializer =
                new DecimalSerializer(BsonType.Decimal128);

            private static readonly IBsonSerializer NullableSerializer =
                new NullableSerializer<decimal>(DecimalSerializer);

            private static readonly IBsonSerializer GuidSerializer = new GuidSerializer(GuidRepresentation.Standard);

            public IBsonSerializer GetSerializer(Type type)
            {
                if (type == typeof(decimal)) return DecimalSerializer;
                if (type == typeof(decimal?)) return NullableSerializer;
                if (type == typeof(Guid)) return GuidSerializer;

                return null; // falls back to Mongo defaults
            }
        }

        public static IServiceCollection AddRavenDb(this IServiceCollection services, RavenSettings settings)
        {
            services.AddSingleton(services);
            services.AddSingleton<IRavenConnectionWrapper, RavenDbConnectionWrapper>();
            return services;
        }


        public static IServiceCollection AddRabbitMq(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddTransient<IEventProducer, RabbitMqProducer>();
			services.AddTransient<ISubscriber, RabbitMqSubscriber>();

            var consumers = MessageHandlerRegister.GetMessageHandlers(assemblies);
            consumers.ForEach(consumer =>
            {
                var handlerInterface = typeof(IMessageConsumer);
                services.AddTransient(handlerInterface, consumer);
            });

            return services;
        }


        public static IServiceCollection AddMongoDb(this IServiceCollection services, 
            MongoConfig mongoConfig)
        {
            services.AddSingleton(mongoConfig);
            BsonSerializer.RegisterSerializationProvider(new CustomSerializationProvider());

            var mongoClient = new MongoClient(mongoConfig.ConnectionString);
            services.AddSingleton<IMongoClient>(mongoClient);
            
            return services;
        }



        public static IServiceCollection ConfigureRepositories(this IServiceCollection services,
            bool excludeTests = true)
        {
            var assemblies = 
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic).ToArray();

            if (excludeTests)
                assemblies = assemblies
                    .Where(x => 
                        !x.FullName.Contains("Test", StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

            var aggregates = AssemblyScanner.FindAggregates(assemblies);


            var implementedEventStore = typeof(MongoEventStore<,>);
            
            foreach (var kv in aggregates)
            {
                var eventStoreInterface = typeof(IEventStore<,>).MakeGenericType(kv.Key, kv.Value);
                var repositoryInterface = typeof(IAggregateRepository<,>).MakeGenericType(kv.Key, kv.Value);
                
                var concreteRepository = typeof(AggregateRepository<,>).MakeGenericType(kv.Key, kv.Value);
                var concreteEventStore = typeof(MongoEventStore<,>).MakeGenericType(kv.Key, kv.Value);
   
                services.AddSingleton(eventStoreInterface, concreteEventStore);
                services.AddSingleton(repositoryInterface, concreteRepository);
            }
            
            return services;
        }

        public static IServiceCollection AddEventStoreConfig(this IServiceCollection services, EventStoreConfig cfg)
        {
            services.AddSingleton(cfg);
            return services;
        }
        
        public static IServiceCollection ConfigureMongoEventStore(this IServiceCollection services)
        {
            //TODO check if exists other eStore
            services.AddSingleton(typeof(IEventStore<,>), typeof(MongoEventStore<,>));

            return services;
        }
    }
}