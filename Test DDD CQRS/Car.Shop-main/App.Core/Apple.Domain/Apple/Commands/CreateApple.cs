using System;
using App.Core.Commands;
using Apple.Domain.Apple.DomainObjects;
using Newtonsoft.Json;

namespace Apple.Domain.Apple.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateApple : Command<Guid>
    {
        public override Guid Id { get; protected set; }
        public string Color { get; protected set; }
        public Weight Weight { get; protected set; }

        public CreateApple(string color, Weight weight, 
            CommandMeta commandMeta, long? expectedVersion = null) : base(commandMeta, expectedVersion)
        {
            Color = color;
            Weight = weight;
        }
    }
}