using System.Threading.Tasks;

namespace App.Core.MessageBroker
{
    public interface IMessageConsumer
    {
        Task<HandleResult> ConsumeAsync(string message);
    }
}
