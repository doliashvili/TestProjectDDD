using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Events;
using App.Core.MessageBroker;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace App.Infrastructure.RabbitMq
{
    public class RabbitMqProducer : IEventProducer
    {
        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly IConnection _connection;

        public RabbitMqProducer(IRabbitConnectionWrapper rabbitConnectionWrapper, 
            RabbitMqConfig rabbitMqConfig)
        {
            _connection = rabbitConnectionWrapper.Connection;
            _rabbitMqConfig = rabbitMqConfig;
            
        }


        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
        {
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var routingKey = @event.GetType().Name;
            var queueName = $"{_rabbitMqConfig.Exchange}.{routingKey}";

            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(_rabbitMqConfig.Exchange,
                ExchangeType.Direct,
                true, 
                false,
                new Dictionary<string, object>()
                {
                    { "x-message-ttl", _rabbitMqConfig.Ttl }
                });

            channel.QueueDeclare(queueName, true, false, false);
            channel.BasicPublish(_rabbitMqConfig.Exchange, routingKey, true, null, body);
            return Task.CompletedTask;
        }
    }
}
