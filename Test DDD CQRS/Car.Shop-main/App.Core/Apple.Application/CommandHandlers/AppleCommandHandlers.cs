using System;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Commands;
using App.Core.Repository;
using Apple.Domain.Apple.Commands;

namespace Apple.Application.CommandHandlers
{
    public class AppleCommandHandlers : ICommandHandler<CreateApple>
    {
        private readonly IAggregateRepository<Domain.Apple.DomainObjects.Apple, Guid> _appleRepo;

        public AppleCommandHandlers(IAggregateRepository<Domain.Apple.DomainObjects.Apple, Guid> appleRepo)
        {
            _appleRepo = appleRepo;
        }
        
        public async Task HandleAsync(CreateApple command, CancellationToken cancellationToken = default)
        {
            var agr = new Domain.Apple.DomainObjects.Apple(command);
            await _appleRepo.SaveAsync(agr, cancellationToken);
        }
    }
}