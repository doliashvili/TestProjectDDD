using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using App.Core.Commands;
using App.Core.Commands.Implementation;
using App.Core.Events;
using App.Core.Events.InternalEvents;
using App.Core.Queries;
using App.Core.Queries.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace App.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInternalEventPublisher(this IServiceCollection services)
        {
            services.AddTransient<IInternalEventPublisher, InternalEventPublisher>();
            return services;
        }

        
        public static IServiceCollection AddCommandSender(this IServiceCollection services)
        {
            services.AddTransient<ICommandSender, CommandSender>();
            return services;
        }
        
        /// <summary>
        /// Registers command handlers as a transient services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            if (!assemblies.Any())
                throw new ArgumentNullException(nameof(assemblies));
            
            //Find handlers and generic parameters
            var handlersWithParameters
                = GetHandlersWithGenericParameterTypes(typeof(ICommandHandler<>), assemblies);
            
            ThrowIfDuplicates(handlersWithParameters);
            
            //Register as transient service
            foreach (var (handler, parameter) in handlersWithParameters)
            {
                var interfaceType = typeof(ICommandHandler<>).MakeGenericType(parameter);
                services.AddTransient(interfaceType, handler);
            }

            return services;
        }
        
        
        /// <summary>
        /// Registers event handlers as a transient services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddInternalEventHandlers(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            if (!assemblies.Any())
                throw new ArgumentNullException(nameof(assemblies));
            
            //Find handlers and generic parameters
            var handlersWithParameters
                = GetHandlersWithGenericParameterTypes(typeof(IInternalEventHandler<>), assemblies).ToList();
            
            //Register as transient service
            foreach (var (handler, parameter) in handlersWithParameters)
            {
                var interfaceType = typeof(IInternalEventHandler<>).MakeGenericType(parameter);
                services.AddTransient(interfaceType, handler);
            }

            return services;
        }

        
        /// <summary>
        /// Registers query handlers as a transient services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddQueryHandlers(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            if (!assemblies.Any())
                throw new ArgumentNullException(nameof(assemblies));

            var registeredServices = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes()
                    .Where(x => x.IsClass && !x.IsAbstract
                                          && x.GetInterfaces()
                                              .Any(i => i.IsGenericType
                                                        && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                    .ToList();

                foreach (var handlerType in handlerTypes)
                {
                    var genericInterface = handlerType.GetInterfaces()
                        .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                    if(registeredServices.Any(x=>x == genericInterface))
                        throw new InvalidOperationException($"Duplicate QueryHandler {genericInterface.Name}");
                    registeredServices.Add(genericInterface);
                    services.AddTransient(genericInterface, handlerType);
                }
            }
            
            return services;
        }


        public static IServiceCollection AddQueryProcessor(this IServiceCollection services)
        {
            services.AddTransient<IQueryProcessor, QueryProcessor>();
            return services;
        }

        
        #region Private methods


        // Checks duplicates and throws if exist
        private static void ThrowIfDuplicates(IDictionary<Type, Type> handlersWithTypes)
        {
            if(null == handlersWithTypes) return;
            var argumentTypes = handlersWithTypes.Select(x => x.Value);

            var duplicates = argumentTypes.GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x=>x.Key.FullName).Distinct().ToArray();
            if (!duplicates.Any()) return;
            var joinString = string.Join(',', duplicates);
            throw new InvalidOperationException($"Duplicate handlers detected for types: {joinString}");
        }
        
        
        
        // Scan assemblies and find handler types
        private static Dictionary<Type, Type> GetHandlersWithGenericParameterTypes
            (Type genericInterfaceType, params Assembly[] assemblies)
        {
            //  Key=Concrete class, Value=Generic parameter                     
            var dictionary = new Dictionary<Type, Type>();

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => t.IsClass
                                && !t.IsAbstract
                                && t.GetInterfaces()
                                    .Any(x=>x.IsGenericType 
                                            && x.GetGenericTypeDefinition() == genericInterfaceType))
                                    .ToList();

                   var f = handlers  
                       .Select(x => new
                    {
                        HandlerClassType = x,
                        GenericParameterType = x.GetInterfaces()
                            .First(ii => ii.IsGenericType &&
                                               ii.GetGenericTypeDefinition() == genericInterfaceType)
                            .GenericTypeArguments[0],
                    }).ToList();
                
                f.ForEach(h =>
                {
                    dictionary.Add(h.HandlerClassType, h.GenericParameterType);
                });
            }

            return dictionary;
        }
        
        #endregion
    }
}