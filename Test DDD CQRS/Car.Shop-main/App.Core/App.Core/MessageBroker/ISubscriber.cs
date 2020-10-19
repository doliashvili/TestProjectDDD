namespace App.Core.MessageBroker
{
    public interface ISubscriber 
    {
        void Subscribe(IMessageConsumer consumer);
    }
}
