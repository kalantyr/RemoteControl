using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kalantyr.RemoteControl.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramRunner.LogIfException(() =>
            {
                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var fileInfo = new FileInfo(typeof(Program).Assembly.Location);
                        config.SetBasePath(fileInfo.Directory.FullName);
                    })
                    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                    .UseWindowsService()
                    .Build()
                    .Run();
            });
        }
    }
}
