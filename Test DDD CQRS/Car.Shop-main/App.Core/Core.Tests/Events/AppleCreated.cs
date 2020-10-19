using System;
using App.Core.Commands;
using App.Core.Events.DomainEvents;
using Core.Tests.Aggregates;

namespace Core.Tests.Events
{
    public class AppleCreated : DomainEvent<AppleAggregate, Guid>
    {
        public string Color { get; private set; }

        public AppleCreated()
        {
        }
        public AppleCreated(string color, AppleAggregate aggregateRoot, ICommand command) 
            : base(aggregateRoot, command)
        {
            Color = color;
        }
    }
}