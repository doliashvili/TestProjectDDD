using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App.Core.Commands;
using Apple.Domain.Apple.Commands;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Apple.Api.Controllers
{
    [Route("v1/Apple")]
    public class AppleCommandController : ControllerBase
    {
        private readonly ICommandSender _commandSender;

        public AppleCommandController(ICommandSender commandSender)
        {
            _commandSender = commandSender;
        }

       
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Create([FromBody] CreateApple command)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            command.CommandMeta.UserIp = ip;
            await _commandSender.SendAsync(command);
            return Ok();
        }
    }
}