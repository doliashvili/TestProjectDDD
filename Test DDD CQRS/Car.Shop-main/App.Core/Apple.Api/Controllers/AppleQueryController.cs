using System.Collections.Generic;
using System.Threading.Tasks;
using App.Core.Queries;
using Apple.ReadModels.Models;
using Apple.ReadModels.Read.Queries;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Apple.Api.Controllers
{
    [Route("v1/Apple")]
    public class AppleQueryController : ControllerBase
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger _logger;

        public AppleQueryController(IQueryProcessor queryProcessor, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IReadOnlyList<AppleReadModel>> Get([FromQuery] GetAllApples query)
        {
            return await _queryProcessor.QueryAsync(query);
        }
    }
}