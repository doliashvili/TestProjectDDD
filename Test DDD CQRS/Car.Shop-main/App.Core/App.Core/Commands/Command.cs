namespace App.Core.Commands
{
    public abstract class Command<TId> : ICommand
    {
        public abstract TId Id { get; protected set; }
        public long? ExpectedVersion { get; private set; }
        public CommandMeta CommandMeta { get; private set; }

        protected Command(){ }
        protected Command(CommandMeta commandMeta, long? expectedVersion = null)
        {
            ExpectedVersion = expectedVersion;
            CommandMeta = commandMeta;
        }
    }
}