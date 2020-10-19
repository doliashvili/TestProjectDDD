using System.Threading;
using System.Threading.Tasks;
using App.Core.Events.InternalEvents;
using App.Core.Repository;
using App.Infrastructure.RavenDbImplementation;
using Apple.Domain.Apple.Events;
using Apple.ReadModels.Models;
using Raven.Client.Documents;

namespace Apple.ReadModels.Write
{
    public class AppleDomainEventProcessor : IInternalEventHandler<AppleCreated>
    {
        private readonly IReadModelRepository<AppleReadModel, string> _repo;

        public AppleDomainEventProcessor(IReadModelRepository<AppleReadModel, string> repo)
        {
            _repo = repo;
        }
        
        
        public async Task HandleAsync(AppleCreated @event, CancellationToken cancellationToken = default)
        {
            var appleModel = new AppleReadModel()
            {
                Color = @event.Color,
                Id = @event.AggregateId.ToString(),
                Version = @event.Version,
                Weight = @event.Weight
            };
            await _repo.WriteAsync(appleModel, cancellationToken);
        }
    }
}