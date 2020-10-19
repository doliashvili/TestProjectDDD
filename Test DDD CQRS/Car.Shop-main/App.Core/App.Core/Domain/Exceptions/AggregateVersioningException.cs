namespace App.Core.Domain.Exceptions
{
    public class AggregateVersioningException : BaseDomainException
    {
        public AggregateVersioningException(string message) : base(message)
        {
        }
    }
}