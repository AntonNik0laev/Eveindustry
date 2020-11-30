using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using Eveindustry.Sde;
using Eveindustry.Sde.Models;
using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eveindustry.CLI
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    internal class Program
    {
        private const string SdeGlobalPath = "D:\\data\\sde\\";

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">program command line args. </param>
        public static void Main(string[] args)
        {
            var terminationTypes = new List<long> {4051, 4246, 4247, 4312, 17476};
            var name = args[0];
            var quantity = int.Parse(args[1]);
            Console.WriteLine();
            
            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL MANUFACTURING LIST");

            var container = RegisterServices();
            var manufacturingBuilder = container.Resolve<IManufacturingInfoBuilder>();
            var etRepository = container.Resolve<IEveTypeRepository>();
            var itemToBuild = etRepository.GetByExactName(name);
            var infoTree = manufacturingBuilder.BuildInfo(itemToBuild.Id);
            var flat = manufacturingBuilder.GetFlatManufacturingList(infoTree, quantity, terminationTypes);
            var grouped = manufacturingBuilder.GroupIntoStages(flat, terminationTypes);

            PrintStagesDetails(grouped);

            Console.ReadKey();
        }

        private static ILifetimeScope RegisterServices()
        {
            
            var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()
            {
                {"TypeInfoLoaderOptions:SdeBasePath", "d:/data/sde"},
                {"EvePricesUdateConfiguration:UpdateIntervalMinutes", "1440"}
            });
            var spFactory = new AutofacServiceProviderFactory();
            var Configuration = configBuilder.Build();

            var services = new ServiceCollection();
            services.AddOptions<TypeInfoLoaderOptions>().Bind(Configuration.GetSection(nameof(TypeInfoLoaderOptions)));
            services.AddOptions<EvePricesUdateConfiguration>().Bind(Configuration.GetSection(nameof(EvePricesUdateConfiguration)));

            services.AddSingleton<ISdeDataRepository, SdeDataRepository>();
            services.AddSingleton<ISdeDataLoader, SdeDataLoader>();
            services.AddSingleton<IEsiPricesRepository, EsiPricesRepository>();
            services.AddSingleton<IEvePricesRepository, EvePricesRepository>();
            services.AddSingleton<ITypeIdsSource, AllTypeIdsSource>();

            services.AddSingleton<IEveTypeRepository, EveTypeRepository>();
            services.AddScoped<IManufacturingInfoBuilder, ManufacturingInfoBuilder>();
            services.AddAutoMapper(typeof(EveType).Assembly);
            services.AddHttpClient();
            services.AddLogging(c => c.AddConsole());
            
            var sp = spFactory.CreateServiceProvider(spFactory.CreateBuilder(services));
            var container = sp.GetAutofacRoot();
            
            var logger = container.Resolve<ILogger<Program>>();
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
            return container;
        }

        private static void PrintStagesDetails(IEnumerable<IEnumerable<EveManufacturialQuantity>> items)
        {
            decimal systemCost = 0.05M;

            var formatString = "{0,35}|{1,15:N0}|{2, 15:N0}|{3, 15:N0}|{4, 15:N0}|{5,15:N0}|{6,15:N0}";

            void PrintHeader()
            {
                Console.WriteLine(
                    formatString,
                    "Name", // 0
                    "Amount", // 1
                    "PriceBuy", // 2
                    "PriceSell", // 3
                    "Job cost", // 4
                    "Mat Buy", // 5
                    "Mat sell", // 6
                    "Remain"); // 7
            }

            void PrintDetails(EveManufacturialQuantity unit)
            {
                var details = string.Format(
                    formatString,
                    unit.Material.Name, // 0
                    unit.Quantity, // 1
                    unit.TotalJitaBuyPrice, // 2
                    unit.TotalJitaSellPrice, // 3
                    unit.MaterialsAdjustedPrice * systemCost, // 4
                    unit.MaterialsJitaBuyPrice,
                    unit.MaterialsJitaSellPrice,
                    unit.RemainingQuantity);
                Console.WriteLine(details);
            }

            void PrintTotal(IEnumerable<EveManufacturialQuantity> stage)
            {
                var details = string.Format(
                    formatString,
                    "TOTAL",
                    stage.Sum(i => i.Quantity),
                    stage.Sum(i => i.TotalJitaBuyPrice),
                    stage.Sum(i => i.TotalJitaSellPrice),
                    stage.Sum(i => i.MaterialsAdjustedPrice * systemCost),
                    stage.Sum(i => i.MaterialsJitaBuyPrice),
                    stage.Sum(i => i.MaterialsJitaSellPrice));
                Console.WriteLine(details);
            }

            var itemsList = items.ToList();
            for (int i = 0; i < itemsList.Count(); i++)
            {
                Console.WriteLine($"--STAGE {i}--");
                PrintHeader();
                var stageList = itemsList[i];
                foreach (var stageItem in itemsList[i].OrderByDescending(x => x.TotalJitaSellPrice))
                {
                    PrintDetails(stageItem);
                }

                PrintTotal(stageList);
                Console.WriteLine();
            }

            Console.WriteLine("Remaining items: ");
            var formatStringRemaining = "{0,35}|{1,15:N0}|{2, 15:N0}|{3, 15:N0}";
            var itemsFlat = items.SelectMany(i => i).OrderByDescending(i => i.RemainingJitaSellPrice).ToList();
            foreach (var item in itemsFlat)
            {
                if (item.RemainingQuantity == 0)
                {
                    continue;
                }

                var formatted = string.Format(
                    formatStringRemaining,
                    item.Material.Name,
                    item.RemainingQuantity,
                    item.RemainingJitaBuyPrice,
                    item.RemainingJitaSellPrice);
                Console.WriteLine(formatted);
            }

            var totalFormatted = string.Format(
                formatStringRemaining,
                "TOTAL",
                itemsFlat.Sum(i => i.RemainingQuantity),
                itemsFlat.Sum(i => i.RemainingJitaBuyPrice),
                itemsFlat.Sum(i => i.RemainingJitaSellPrice));
            Console.WriteLine(totalFormatted);
        }
    }
}