using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using App.Core.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace App.Infrastructure.RabbitMq
{
    public class RabbitMqSubscriber : ISubscriber, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Type _consumerType;


        public RabbitMqSubscriber(IRabbitConnectionWrapper connectionWrapper, 
            ILogger logger, IServiceScopeFactory serviceScopeFactory)
        {
            _connection = connectionWrapper.Connection;
            _logger = logger;
            _channel = _connection.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
        }


        public void Subscribe(IMessageConsumer consumer)
        {
            _consumerType = consumer.GetType();
            var attr = _consumerType.GetCustomAttribute<MessageHandlerAttribute>(inherit: true);
            var exchange = attr.Exchange;
            var routingKey = attr.RoutingKey;
            var queueName = $"{exchange}.{routingKey}";

            _channel.ExchangeDeclare(exchange,
                ExchangeType.Direct,
                true,
                false,
                new Dictionary<string, object>()
                {
                    { "x-message-ttl", 0 }
                });
            _channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueBind(queueName, exchange, routingKey);
            _channel.BasicQos(0, 10, false);

            var basicConsumer = new EventingBasicConsumer(_channel);
            basicConsumer.Received += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var consumers = scope.ServiceProvider.GetServices<IMessageConsumer>()
                        .Where(x => x.GetType() == _consumerType).ToList();

                    consumers.ForEach(async consumer => {
                        var handleResult = await consumer.ConsumeAsync(message);
                    });

                    _channel.BasicAck(e.DeliveryTag, true);
                }
                catch (Exception exception)
                {
                    _logger.Fatal(nameof(Subscribe), exception);
                    throw;
                }
            };

            _channel.BasicConsume(queueName, false, basicConsumer);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
