using System;
using App.Core;
using App.Core.Repository;
using App.Infrastructure;
using App.Infrastructure.MongoDbImplementation;
using Core.Tests.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests
{
    public static class DependencyInjection
    {
        private static IServiceCollection _services;

        public static IServiceProvider GetServiceProvider()
        {
            _services ??= new ServiceCollection();

            RegisterServices();

            return _services.BuildServiceProvider();
        }

        private static void RegisterServices()
        {
            _services.AddCommandHandlers(typeof(DependencyInjection).Assembly);
            _services.AddInternalEventHandlers(typeof(DependencyInjection).Assembly);
            _services.AddInternalEventPublisher();
            _services.AddCommandSender();

            _services.AddEventStoreConfig(new EventStoreConfig() {BatchSize = 200});
            _services.AddMongoDb(new MongoConfig()
            {
                ConnectionString =
                    "mongodb+srv://avtandilm:e5115135124w@cluster0.fcryz.azure.mongodb.net/BookDB?retryWrites=true&w=majority"
            });
            
            _services.ConfigureMongoEventStore();
            _services.ConfigureRepositories(false);

        }
    }
}