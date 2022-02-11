using System;
using Kalantyr.RemoteControl.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kalantyr.RemoteControl.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            //app.UseCors("AllowEverything"); // TODO: это нужно?

            app.UseRouting();

            app.UseEndpoints(q => q.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowEverything", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });

            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                //jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Always;
            });

            var timerService = new TimerService();
            services.AddHostedService(sp => timerService);
            services.AddSingleton(new PowerService(timerService));
        }
    }
}
