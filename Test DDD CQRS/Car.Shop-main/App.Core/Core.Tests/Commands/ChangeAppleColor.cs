using System;
using App.Core.Commands;

namespace Core.Tests.Commands
{
    public class ChangeAppleColor : Command<Guid>
    {
        public override Guid Id { get; protected set; }
        public string Color { get; private set; }

        public ChangeAppleColor(Guid agrId, string color, CommandMeta commandMeta) : base(commandMeta)
        {
            Id = agrId;
            Color = color;
        }
    }
}