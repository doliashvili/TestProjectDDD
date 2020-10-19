using System;
using App.Core.Commands;

namespace Core.Tests.Commands
{
    public class CreateApple : Command<Guid>
    {
        public string Color { get; private set; }
        public override Guid Id { get; protected set; }

        public CreateApple(Guid id, string color, CommandMeta commandMeta) : base(commandMeta)
        {
            Color = color;
            Id = id;
        }

    }
}