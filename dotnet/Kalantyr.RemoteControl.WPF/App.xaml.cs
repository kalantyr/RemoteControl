using System;
using System.Net.Http;
using Kalantyr.RemoteControl.WPF.Client;
using Kalantyr.RemoteControl.WPF.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kalantyr.RemoteControl.WPF
{
    public partial class App
    {
        public IServiceProvider Services { get; }

        public App()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(ConfigureServices);
            var host = hostBuilder.Build();
            Services = host.Services;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services
                .AddHttpClient<RemoteControlClient>(cl =>
                {
                    cl.BaseAddress = new Uri(Settings.Default.ServerAddress);
                });

            services.AddSingleton<IRemoteControlClient>(sp => new RemoteControlClient(sp.GetService<IHttpClientFactory>()));
        }
    }
}
