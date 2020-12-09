using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Eveindustry.API;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using Eveindustry.Sde;
using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Repositories;
using Eveindustry.Shared;
using Eveindustry.Shared.DTO.EveType;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Eveindustry.CLI
{
    internal static class HostExtensions
    {
        public static IHost InitializeEveData(this IHost host)
        {
            var container = host.Services.GetAutofacRoot();
            
            var logger = container.Resolve<ILogger<IHost>>();
            logger.LogInformation("Starting to initialize repositories");
            logger.LogInformation("Begin initialize ISdeDataRepository");
            container.Resolve<ISdeDataRepository>().Init().Wait();
            logger.LogInformation("End initialize ISdeDataRepository");
            logger.LogInformation("Begin initialize IEsiPricesRepository");
            container.Resolve<IEsiPricesRepository>().Init().Wait();
            logger.LogInformation("End initialize IEsiPricesRepository");
            logger.LogInformation("Begin initialize IEvePricesRepository");
            container.Resolve<IEvePricesRepository>().Init().Wait();
            logger.LogInformation("End initialize IEvePricesRepository");
            logger.LogInformation("Begin initialize IEveTypeRepository");
            container.Resolve<IEveTypeRepository>().Init().Wait();
            logger.LogInformation("End initialize IEveTypeRepository");
            return host;
        }
    } 
    
    /// <summary>
    /// Entry point class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">program command line args. </param>
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(c => c.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    {"TypeInfoLoaderOptions:SdeBasePath", "d:/data/sde"},
                    {"EvePricesUdateConfiguration:UpdateIntervalMinutes", "14400"},
                    {"EveItemName", args[0]},
                    {"EveItemQuantity", args[1]}
                }))
                .ConfigureServices(AddEveServices)
                .ConfigureServices(sc => sc.AddHostedService<EveindustryCliService>())
                .UseConsoleLifetime()
                .Build()
                .InitializeEveData()
                .RunAsync();

            Console.ReadKey();
        }

        private static void AddEveServices(HostBuilderContext context, IServiceCollection sc)
        {
            var configuration = context.Configuration;
            var services = sc;
            services.AddOptions<TypeInfoLoaderOptions>().Bind(configuration.GetSection(nameof(TypeInfoLoaderOptions)));
            services.AddOptions<EvePricesUdateConfiguration>().Bind(configuration.GetSection(nameof(EvePricesUdateConfiguration)));

            services.TryAddSingleton<ISdeDataRepository, SdeDataRepository>();
            services.TryAddSingleton<ISdeDataLoader, SdeDataLoader>();
            services.TryAddSingleton<IEsiPricesRepository, EsiPricesRepository>();
            services.TryAddSingleton<IEvePricesRepository, EvePricesRepository>();
            services.TryAddSingleton<ITypeIdsSource, AllTypeIdsSource>();

            services.TryAddSingleton<IEveTypeRepository, EveTypeRepository>();
            services.TryAddScoped<IManufacturingInfoBuilder, ManufacturingInfoBuilder>();
            services.AddAutoMapper(typeof(EveType).Assembly, typeof(EveTypeDto).Assembly, typeof(Startup).Assembly);
            services.AddHttpClient();
            services.AddLogging(c =>
            {
                c.ClearProviders().AddSerilog().AddFile(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "eveindustry.log"));
            });
        }
    }
}