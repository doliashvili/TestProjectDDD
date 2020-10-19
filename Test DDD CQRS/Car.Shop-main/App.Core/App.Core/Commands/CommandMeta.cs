using System;

namespace App.Core.Commands
{
    public class CommandMeta
    {
        public Guid CommandId { get; set; }
        public Guid CorrelationId { get; set; }
        public string UserId { get; set; }
        public string UserIp { get; set; }

        public CommandMeta() { }
        public CommandMeta(Guid commandId, Guid correlationId, 
            string userId= "-1", string userIp = "0.0.0.0")
        {
            CommandId = commandId;
            CorrelationId = correlationId;
            UserId = userId;
            UserIp = userIp;
        }
    }
}