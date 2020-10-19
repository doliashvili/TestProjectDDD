using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using App.Core.Domain;

namespace App.Infrastructure.Helpers
{
    public static class AssemblyScanner
    {
        public static Dictionary<Type, Type> FindAggregates(params Assembly[] assemblies)
        {
            var aggregateDictionary = new Dictionary<Type, Type>();
            
            foreach (var assembly in assemblies)
            {
                var types =
                    assembly.GetTypes()
                        .Where(t => t.IsClass 
                                    && !t.IsAbstract 
                                    && t.GetInterfaces().Any(
                                        x => x.IsGenericType
                                              && x.GetGenericTypeDefinition() == typeof(IAggregateRoot<>)));
                                    
                foreach (var type in types)
                {
                    var interfaceType = type.GetInterfaces()
                        .First(x => x.IsGenericType
                                    && x.GetGenericTypeDefinition() == typeof(IAggregateRoot<>));
                    
                    aggregateDictionary.Add(type, interfaceType.GetGenericArguments()[0]);
                }
            }

            return aggregateDictionary;
        }
    }
}