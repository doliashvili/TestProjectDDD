using System;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Commands;
using App.Core.Events.InternalEvents;
using Core.Tests.Aggregates;
using Core.Tests.CommandHandlers;
using Core.Tests.Commands;
using Core.Tests.EventHandlers;
using Core.Tests.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Tests
{
    public class HandlersTests
    {
        private readonly IServiceProvider _provider;
        private readonly ICommandSender _commandPublisher;
        private readonly IInternalEventPublisher _internalEventPublisher;

        public HandlersTests()
        {
            _provider = DependencyInjection.GetServiceProvider();
            _commandPublisher = _provider.GetRequiredService<ICommandSender>();
            _internalEventPublisher = _provider.GetRequiredService<IInternalEventPublisher>();
        }


        [Fact]
        public async Task HandleCreateAppleCommand()
        {
            var command = new CreateApple(Guid.NewGuid(), "Green", 
                new CommandMeta(Guid.NewGuid(), Guid.NewGuid()));
            await _commandPublisher.SendAsync(command);

            Assert.True(AppleCommandHandlers.IsHandled);
        }

        [Fact]
        public async Task Publish_and_handle_InternalEvent()
        {
            var agr = CreateAppleAgr("Green");
            var @event = agr.GetUncommittedEvents().First();

            var s = _provider.GetRequiredService<IInternalEventHandler<AppleCreated>>();
            var ss = _provider.GetServices<IInternalEventHandler<AppleCreated>>();
            
            await _internalEventPublisher.PublishAsync(@event);
            
            
            Assert.True(AppleCreatedEventHandler.IsHandled);
        }
        
        
        private AppleAggregate CreateAppleAgr(string color) => 
            new AppleAggregate(
                new CreateApple(Guid.NewGuid(), 
                    color, 
                    new CommandMeta(Guid.NewGuid(), Guid.NewGuid())
                ));
    }
}