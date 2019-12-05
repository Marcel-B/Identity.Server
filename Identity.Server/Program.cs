using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Prometheus;
using System;

namespace Identity.Servier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var pusher = new MetricPusher(new MetricPusherOptions
            {
                Endpoint = "https://push.qaybe.de/metrics",
                Job = "IdentityServer",
                Instance = "Identity"
            });

            pusher.Start();

            var file = "nlog.config";
            var logger = NLogBuilder.ConfigureNLog(file).GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog();
    }
}
