using System;

namespace App.Core.Domain.Exceptions
{
    public class AggregateConcurrencyException : BaseDomainException
    {
        public AggregateConcurrencyException(Type aggregate) 
            : base($"Detected concurrency issue for aggregate: {aggregate.Name}")
        {
        }
    }
}