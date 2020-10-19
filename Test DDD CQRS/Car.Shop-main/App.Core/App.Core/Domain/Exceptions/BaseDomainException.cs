using System;

namespace App.Core.Domain.Exceptions
{
    public abstract class BaseDomainException : Exception
    {
        protected BaseDomainException(string message) : base(message)
        {
        }
    }
}