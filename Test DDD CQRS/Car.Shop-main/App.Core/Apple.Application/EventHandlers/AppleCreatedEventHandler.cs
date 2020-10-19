using System.Threading.Tasks;
using App.Core.MessageBroker;
using Serilog;

namespace Apple.Application.EventHandlers
{
    [MessageHandler("test", "AppleCreated")]
    public class AppleCreatedEventHandler : IMessageConsumer
    {
        private readonly ILogger _logger;
        private int inc = 0;

        public AppleCreatedEventHandler(ILogger logger)
        {
            _logger = logger;
            _logger.Information("Created handler!!!!");
        }


        public Task<HandleResult> ConsumeAsync(string message)
        {
            inc ++;
            _logger.Error(inc.ToString());
            return Task.FromResult(HandleResult.Success);
        }
    }
}
