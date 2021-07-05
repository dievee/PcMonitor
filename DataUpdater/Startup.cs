using DataUpdater;
using DataUpdater.Infrastructure;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DataUpdater
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddLinqToDbContext<AppDataConnection>((provider, options) =>
            {
                options
                    .UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString"))
                    .UseDefaultLogging(provider);
            });
        }
    }
}
