using System;
using App.Core.Domain;
using Apple.Domain.Apple.Commands;
using Apple.Domain.Apple.Events;

namespace Apple.Domain.Apple.DomainObjects
{
    public partial class Apple : AggregateRoot<Guid>
    {
        public Apple()
        {
        }
        public Apple(CreateApple command) : base(command.Id)
        {
            Id = Guid.NewGuid();
            Color = command.Color;
            Weight = command.Weight;
            
            ApplyChange(
                new AppleCreated(
                    Color, Weight, 
                    this,
                    command)
                );
        }

        public void Apply(AppleCreated e)
        {
            Id = e.AggregateId;
            Color = e.Color;
            Weight = e.Weight;
        }
        
    }
}