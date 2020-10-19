using System;
using App.Core.Commands;
using App.Core.Events.DomainEvents;
using Core.Tests.Aggregates;

namespace Core.Tests.Events
{
    public class AppleColorChanged : DomainEvent<AppleAggregate, Guid>
    {
        public string OldColor { get; private set; }
        public string NewColor { get; private set; }

        public AppleColorChanged()
        {
        }
        public AppleColorChanged(string oldColor, string newColor,
            AppleAggregate aggregateRoot, ICommand command) : base(aggregateRoot, command)
        {
            OldColor = oldColor;
            NewColor = newColor;
        }
    }
}