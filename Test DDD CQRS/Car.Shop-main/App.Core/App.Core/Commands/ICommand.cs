using App.Core.Messages;

namespace App.Core.Commands
{
    //IMessage for rabbitmq
    public interface ICommand : IMessage
    {
        CommandMeta CommandMeta { get; }
    }
}