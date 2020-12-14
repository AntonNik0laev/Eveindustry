using System;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using Eveindustry.Sde;
using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveIndustry.UpdateData
{
    class Program
    {
        static void Main(string[] args)
        {
            var hb = new HostBuilder();

            hb
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(c => c.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"TypeInfoLoaderOptions:SdeBasePath", "d:/data/sde"},
                }))
                .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;
                services.AddOptions<TypeInfoLoaderOptions>().Bind(config.GetSection(nameof(TypeInfoLoaderOptions)));
                services.TryAddSingleton<ISdeDataLoader, SdeDataLoader>();
                services.AddHostedService<UpdateDataHostedService>();
                services.AddLogging(c => c.AddConsole());
            }).RunConsoleAsync();
        }
    }
}