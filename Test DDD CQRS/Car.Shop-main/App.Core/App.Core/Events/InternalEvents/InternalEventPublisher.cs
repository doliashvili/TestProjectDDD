using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace App.Core.Events.InternalEvents
{
    public class InternalEventPublisher : IInternalEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public InternalEventPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
        {
            if (null == @event)
                return Task.CompletedTask;
            
            var handlerType = typeof(IInternalEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = 
                _serviceProvider.GetServices(handlerType).ToList();

            var taskList = handlers
                .Select(handler => (Task) handler.Invoke(
                    "HandleAsync",
                    @event, 
                    cancellationToken))
                .ToList();

            return !taskList.Any() ? Task.CompletedTask : Task.WhenAll(taskList);
        }
    }
}