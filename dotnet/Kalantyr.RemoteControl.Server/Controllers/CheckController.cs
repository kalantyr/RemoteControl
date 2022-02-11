using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.RemoteControl.Server.Controllers
{
    [ApiController]
    [Route("check")]
    public class CheckController: ControllerBase
    {
        [HttpGet]
        [Route("version")]
        public async Task<IActionResult> SearchSuggestsAsync(CancellationToken cancellationToken)
        {
            var version = typeof(CheckController).Assembly.GetName().Version;
            return Ok(version);
        }
    }
}
