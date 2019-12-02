using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Servier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var connectionString = "";
#if DEBUG
                    connectionString = "Server=localhost,1433;Database=Identity;User Id=sa;Password=foo123bar!";
#endif
                    var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
                    services.AddIdentityServer()
                        .AddConfigurationStore(options =>
                        {
                            options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                            // this enables automatic token cleanup. this is optional.
                            //options.EnableTokenCleanup = true;
                            //options.TokenCleanupInterval = 30; // interval in seconds
                        })
                        .AddOperationalStore(options =>
                        {
                            options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                        });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
