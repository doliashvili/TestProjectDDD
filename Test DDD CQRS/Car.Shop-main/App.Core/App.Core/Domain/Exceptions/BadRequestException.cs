using System;

namespace App.Core.Domain.Exceptions
{
    public class BadRequestException : BaseDomainException
    {
        public BadRequestException(Type aggregate, string id) 
            : base($"BadRequest for aggregate {aggregate.Name}, Id: {id}")
        {
        }
        
        public BadRequestException(string message) : base(message)
        {
        }
    }
}