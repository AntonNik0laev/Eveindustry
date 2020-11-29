using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Eveindustry.CLI.Models;
using Eveindustry.CLI.Models.Config;
using Eveindustry.CLI.StaticDataModels;

namespace Eveindustry.CLI
{
    public class EveBasicManufacturingInfo
    {
        public long TypeId { get; set; }

        public List<EveBasicManufacturingInfo> Requirements { get; set; }

        public long Quantity { get; set; }
    }

    /// <summary>
    /// Entry point class.
    /// </summary>
    internal static class Program
    {
        private const string SdeGlobalPath = "D:\\data\\sde\\";
        private static BlueprintsInfoRepository bpRepository;
        private static EveTypeInfoRepository typeRepository;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">program command line args. </param>
        public static void Main(string[] args)
        {
            Program.bpRepository = new BlueprintsInfoRepository(new BlueprintsInfoLoader(SdeGlobalPath));
            Program.typeRepository = new EveTypeInfoRepository(new TypeInfoLoader(SdeGlobalPath));
            var hulk = typeRepository.FindByName(args[0]);

            var terminationTypes = new List<long> {4051, 4246, 4247, 4312, 17476};
            var quantity = int.Parse(args[1]);
            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL LIST");
            Console.WriteLine();

            var totalTypesList = TraverseManufacturingTree(hulk.Id);
            var evePriceRepository =
                new EvePricesRepository(new ListTypeIdsSource(totalTypesList), new EvePricesUdateConfiguration());
            var esiPricesRepository = new EsiPricesRepository();

            evePriceRepository.Init().Wait();
            esiPricesRepository.Init().Wait();

            var manufacturingBuilder =
                new ManufacturingInfoBuilder(typeRepository, bpRepository, evePriceRepository, esiPricesRepository);
            var infoTree = manufacturingBuilder.BuildInfo(hulk.Id);
            var flat = manufacturingBuilder.GetFlatManufacturingList(infoTree, quantity, terminationTypes);
            var grouped = manufacturingBuilder.GroupIntoStages(flat, terminationTypes);

            PrintStagesDetails(grouped);

            Console.ReadKey();
        }

        private static List<long> TraverseManufacturingTree(
            long typeId)
        {
            var totalList = new List<long>();

            void AddToListRecursive(long typeId)
            {
                if (!totalList.Contains(typeId))
                {
                    totalList.Add(typeId);
                }

                var bp = bpRepository.FindByProductId(typeId);

                var activity = bp?.Activities?.Manufacturing ?? bp?.Activities?.Reaction;

                var requiredMaterials = activity?.Materials ?? new Material[0];

                foreach (var material in requiredMaterials)
                {
                    var matInfo = typeRepository.GetById(material.TypeId);
                    AddToListRecursive(matInfo.Id);
                }
            }

            AddToListRecursive(typeId);

            return totalList;
        }

        private static void PrintStagesDetails(IEnumerable<IEnumerable<EveManufacturingUnit>> items)
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

            void PrintDetails(EveManufacturingUnit unit)
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

            void PrintTotal(IEnumerable<EveManufacturingUnit> stage)
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
            var itemsFlat = items.
                SelectMany(i => i).
                OrderByDescending(i => i.RemainingJitaSellPrice).
                ToList();
            foreach (var item in itemsFlat)
            {
                if(item.RemainingQuantity == 0) {continue;}
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