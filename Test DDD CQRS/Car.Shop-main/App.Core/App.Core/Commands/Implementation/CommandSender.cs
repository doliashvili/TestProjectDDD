using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace App.Core.Commands.Implementation
{
    public class CommandSender : ICommandSender
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandSender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        //send command who implements ICommandHandler
        public Task SendAsync<T>(T command, CancellationToken cancellationToken = default) 
            where T : class, ICommand
        {
            //get all ICommandHandlers
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<T>>();
            //invoke handle method
            return handler.HandleAsync(command, cancellationToken);
        }
    }
}