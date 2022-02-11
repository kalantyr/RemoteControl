using System;

namespace Kalantyr.RemoteControl.Server.Services
{
    public class PowerService
    {
        private readonly TimerService _timerService;

        public PowerService(TimerService timerService)
        {
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        }

        public void PowerOff(TimeSpan delay)
        {
            _timerService.PowerOffTime = delay.Equals(TimeSpan.Zero)
                ? default(DateTime?)
                : DateTime.Now + delay;
        }

        public TimeSpan? GetPowerOff()
        {
            if (_timerService.PowerOffTime == null)
                return null;

            return _timerService.PowerOffTime.Value - DateTime.Now;
        }
    }
}
