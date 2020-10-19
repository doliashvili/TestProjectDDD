namespace App.Core.Events.InternalEvents
{
    public interface IInternalEventHandler<in T> : IEventHandler<T> 
        where T: IEvent 
    {
    }
}