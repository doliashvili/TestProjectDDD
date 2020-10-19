using System;

namespace App.Core.Domain.Exceptions
{
    public class AggregateNotFoundException : BaseDomainException
    {

        public AggregateNotFoundException(Type aggregate) 
            : base($"Aggregate: {aggregate.Name} not found")
        {
        }        
        
        public AggregateNotFoundException(Type aggregate, string id) 
            : base($"Aggregate {aggregate.Name} with Id:{id} not found")
        {
        }
        
        public AggregateNotFoundException(string message) : base(message)
        {
        }
    }
}