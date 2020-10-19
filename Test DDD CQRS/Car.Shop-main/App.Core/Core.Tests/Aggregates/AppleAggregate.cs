using System;
using App.Core.Domain;
using Core.Tests.Commands;
using Core.Tests.Events;

namespace Core.Tests.Aggregates
{
    public class AppleAggregate : AggregateRoot<Guid>
    {
        public string Color { get; protected set; }

        public AppleAggregate() { }
        public AppleAggregate(CreateApple command) : base(command.Id)
        {
            ApplyChange(
                new AppleCreated(command.Color, 
                    this,
                    command));            
        }

        public void Apply(AppleCreated e)
        {
            Id = e.AggregateId;
            Color = e.Color;
        }

        public void ChangeAppleColor(ChangeAppleColor command)
        {
            if(string.IsNullOrWhiteSpace(command.Color))
                throw new Exception("Color error");
            
            ApplyChange(
                new AppleColorChanged(
                    Color, 
                    command.Color, 
                    this, 
                    command));
        }

        /*
        public void Apply(AppleColorChanged e)
        {
            Color = e.NewColor;
        } */
    }
}