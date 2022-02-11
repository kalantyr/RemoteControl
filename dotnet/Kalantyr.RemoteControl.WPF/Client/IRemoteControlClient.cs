using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kalantyr.RemoteControl.WPF.Client
{
    public interface IRemoteControlClient
    {
        Task<TimeSpan?> GetPowerOffAsync(CancellationToken cancellationToken);
 
        Task PowerOffAsync(TimeSpan delay, CancellationToken cancellationToken);
 
        Task CancelPowerOffAsync(CancellationToken cancellationToken);
    }
}