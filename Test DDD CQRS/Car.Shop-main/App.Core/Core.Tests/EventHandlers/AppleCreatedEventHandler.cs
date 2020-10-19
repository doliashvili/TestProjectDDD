using System.Threading;
using System.Threading.Tasks;
using App.Core.Events.InternalEvents;
using Core.Tests.Events;

namespace Core.Tests.EventHandlers
{
    public class AppleCreatedEventHandler : IInternalEventHandler<AppleCreated>
    {
        public static bool IsHandled = false;
        
        public AppleCreatedEventHandler()
        {
            IsHandled = false;
        }
        
        public Task HandleAsync(AppleCreated @event, CancellationToken cancellationToken = default)
        {
            IsHandled = true;
            
            return Task.CompletedTask;
        }
    }
}