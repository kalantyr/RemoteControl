using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.RemoteControl.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.RemoteControl.Server.Controllers
{
    [ApiController]
    [Route("power")]
    public class PowerController: ControllerBase
    {
        private readonly PowerService _powerService;

        public PowerController(PowerService powerService)
        {
            _powerService = powerService ?? throw new ArgumentNullException(nameof(powerService));
        }

        [HttpGet]
        [Route("off")]
        public async Task<IActionResult> GetPowerOffAsync(CancellationToken cancellationToken)
        {
            return Ok(_powerService.GetPowerOff());
        }

        [HttpPost]
        [Route("off")]
        public async Task<IActionResult> PowerOffAsync([FromBody]TimeSpan delay, CancellationToken cancellationToken)
        {
            _powerService.PowerOff(delay);
            return Ok();
        }
    }
}
