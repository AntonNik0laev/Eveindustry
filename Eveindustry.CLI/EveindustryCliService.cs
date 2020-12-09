using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Shared;
using Eveindustry.Shared.DTO.EveType;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Eveindustry.CLI
{
    /// <inheritdoc />
    internal class EveindustryCliService: IHostedService
    {
        private readonly IConfiguration config;
        private readonly IManufacturingInfoBuilder manufacturingBuilder;
        private readonly IEveTypeRepository etRepository;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IMapper mapper;

        public EveindustryCliService(IConfiguration config, IManufacturingInfoBuilder manufacturingBuilder, IEveTypeRepository etRepository, IHostApplicationLifetime applicationLifetime, IMapper mapper)
        {
            this.config = config;
            this.manufacturingBuilder = manufacturingBuilder;
            this.etRepository = etRepository;
            this.applicationLifetime = applicationLifetime;
            this.mapper = mapper;
        }
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            var terminationTypes = new List<long> {4051, 4246, 4247, 4312, 17476};
            var name = config.GetValue<string>("EveItemName");
            var quantity = config.GetValue<long>("EveItemQuantity");
            Console.WriteLine();
            
            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL MANUFACTURING LIST");

            var itemToBuild = this.etRepository.GetByExactName(name);
            var allDependencies = this.etRepository.GetAllDependencies(itemToBuild.Id).ToList();
            
            var mapped = this.mapper.Map<IList<EveTypeDto>>(allDependencies);
            var sorted = new SortedList<long, EveTypeDto>(mapped.ToDictionary(i => i.Id, i => i));
            var infoTree = this.manufacturingBuilder.BuildInfo(itemToBuild.Id,sorted);
            var flat = this.manufacturingBuilder.GetFlatManufacturingList(infoTree, quantity, terminationTypes);
            var grouped = this.manufacturingBuilder.GroupIntoStages(flat, terminationTypes);

            PrintStagesDetails(grouped);
            this.applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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