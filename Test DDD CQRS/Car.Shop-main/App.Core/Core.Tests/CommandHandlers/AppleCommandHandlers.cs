using System.Threading;
using System.Threading.Tasks;
using App.Core.Commands;
using Core.Tests.Commands;

namespace Core.Tests.CommandHandlers
{
    public class AppleCommandHandlers : ICommandHandler<CreateApple>
    {
        public static bool IsHandled = false;

        public AppleCommandHandlers()
        {
            IsHandled = false;
        }
        
        public Task HandleAsync(CreateApple command, CancellationToken cancellationToken = default)
        {
            IsHandled = true;
            return Task.CompletedTask;
        }
    }
}