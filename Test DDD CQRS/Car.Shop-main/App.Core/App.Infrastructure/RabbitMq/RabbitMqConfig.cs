using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure.RabbitMq
{
    public class RabbitMqConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public int Ttl { get; set; }
    }
}
