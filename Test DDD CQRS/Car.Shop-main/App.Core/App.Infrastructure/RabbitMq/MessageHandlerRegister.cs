using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using App.Core.MessageBroker;

namespace App.Infrastructure.RabbitMq
{
    public static class MessageHandlerRegister
    {

        public static List<Type> GetMessageHandlers(IEnumerable<Assembly> assemblies)
        {
            var handlers = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var messageHandlers =
                    assembly.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract
                                              && t.GetInterfaces().Any(i=>i == typeof(IMessageConsumer))
                                              && t.GetCustomAttributes(true).Any(a => 
                                                  a.GetType() == typeof(MessageHandlerAttribute)));
                messageHandlers.ToList().ForEach(x =>
                {
                    handlers.Add(x);
                });
            }

            return handlers;
        } 

    }
}
