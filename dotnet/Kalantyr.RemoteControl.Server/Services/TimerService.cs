using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kalantyr.RemoteControl.Server.Services
{
    public class TimerService: IHostedService
    {
        private Timer _timer;

        public DateTime? PowerOffTime { get; set; }

        private void OnTimer(object? state)
        {
            var service = (TimerService) state;

            if (service.PowerOffTime != null)
                if (service.PowerOffTime.Value <= DateTime.Now)
                    Shutdown();
        }

        private static void Shutdown()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /f /t 0",
                UseShellExecute = true,
                Verb = "runas"
            };
            using var process = new Process { StartInfo = startInfo };
            process.Start();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(OnTimer, this, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }
    }
}
