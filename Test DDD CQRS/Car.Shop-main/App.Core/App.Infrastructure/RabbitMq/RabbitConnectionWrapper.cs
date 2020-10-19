using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace App.Infrastructure.RabbitMq
{
    public interface IRabbitConnectionWrapper
    {
        IConnection Connection { get; }
    }


    public class RabbitConnectionWrapper : IRabbitConnectionWrapper
    {
        private readonly RabbitMqConfig _config;
        public IConnection Connection { get; }

        public RabbitConnectionWrapper(RabbitMqConfig config)
        {
            _config = config;
            var connectionFactory = new ConnectionFactory()
            {
                UserName = _config.UserName,
                Password = _config.Password,
                HostName = _config.Host,
                Port = _config.Port,
                VirtualHost = _config.VirtualHost,
            };

            Connection = connectionFactory.CreateConnection();
        }
    }
}
