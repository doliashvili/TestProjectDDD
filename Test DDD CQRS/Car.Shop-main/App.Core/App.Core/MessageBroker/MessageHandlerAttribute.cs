using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.MessageBroker
{
    public class MessageHandlerAttribute : Attribute
    {
        public string Exchange { get; }
        public string RoutingKey { get; }

        public MessageHandlerAttribute(string exchange, string routingKey)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
        }
    }
}
