using System;
using App.Core.Commands;
using App.Core.Events.DomainEvents;
using Apple.Domain.Apple.DomainObjects;

namespace Apple.Domain.Apple.Events
{
    public class AppleCreated : DomainEvent<DomainObjects.Apple, Guid>
    {
        public string Color { get; private set; }
        public Weight Weight { get; private set; }

        public AppleCreated() { }
        public AppleCreated(string color, Weight weight,
            DomainObjects.Apple agr, ICommand command) : base(agr, command)
        {
            Color = color;
            Weight = weight;
        }
    }
}