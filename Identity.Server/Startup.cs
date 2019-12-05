using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using com.b_velop.Identity.Server;
using IdentityServer4.EntityFramework.Mappers;
using com.b_velop.Identity.Server.Infrastructure;
using Prometheus;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;

namespace Identity.Servier
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "";
            var user = Environment.GetEnvironmentVariable("USER");
            var db = Environment.GetEnvironmentVariable("DB");
            var server = Environment.GetEnvironmentVariable("SERVER");

            var secretProvider = new SecretProvider();
            var pw = secretProvider.GetSecret("sqlserver");
            var key = secretProvider.GetSecret("key");

            var cert = new X509Certificate2("/app/Keys/identity_rsa", key);

            connectionString = $"Server={server},1433;Database={db};User Id={user};Password={pw}";

#if DEBUG
            connectionString = "Server=localhost,1433;Database=Identity;User Id=sa;Password=foo123bar!";
#endif

            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var builder = services.AddIdentityServer(options =>
            {
                options.PublicOrigin = "https://identity.qaybe.de";
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddSigningCredential(cert);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            app.UseHttpMetrics(options =>
            {
                options.RequestCount.Enabled = false;

                options.RequestDuration.Histogram = Metrics.CreateHistogram("identity_http_request_duration_seconds", "",
                    new HistogramConfiguration
                    {
                        Buckets = Histogram.LinearBuckets(start: 1, width: 1, count: 64),
                        LabelNames = new[] { "code", "method" }
                    });
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                foreach (var client in Config.Clients)
                {
                    if (context.Clients.FirstOrDefault(_ => _.ClientId == client.ClientId) == null)
                        context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
