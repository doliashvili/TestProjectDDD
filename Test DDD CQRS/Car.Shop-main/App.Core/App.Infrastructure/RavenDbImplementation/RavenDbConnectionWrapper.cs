using System;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Serilog;

namespace App.Infrastructure.RavenDbImplementation
{
    public interface IRavenConnectionWrapper
    {
        IDocumentStore Store { get; }
    }
    
    public class RavenDbConnectionWrapper : IRavenConnectionWrapper
    {
        private readonly RavenSettings _options;
        private readonly ILogger _logger;
        public IDocumentStore Store { get; }

        public RavenDbConnectionWrapper(RavenSettings ravenSettings, ILogger logger)
        {
            _logger = logger;
            _options = ravenSettings;
            Store = new DocumentStore()
            {
                Urls = _options.Urls ?? throw new ArgumentNullException(nameof(_options.Urls)),
                Database = _options.Database ?? "RavenDb"
            };
            
            Store.Conventions.SaveEnumsAsIntegers = false;
            Store.Conventions.UseOptimisticConcurrency = true;
            Store.Conventions.MaxNumberOfRequestsPerSession = 2;
            Store.Initialize();
            _logger.Information($"ravenDb: database {_options.Database} initialized successfully");
            //CreateDatabaseIfNotExists();
        }
        
        private void CreateDatabaseIfNotExists()
        {
            var database = Store.Database;
            var dbRecords = 
                Store.Maintenance.Server.Send(new GetDatabaseRecordOperation(database));
            
            if (null == dbRecords)
            {
                Store.Maintenance.Server.Send(
                    new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
        }
    }
}