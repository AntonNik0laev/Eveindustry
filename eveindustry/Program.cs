using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Eveindustry.Models;
using Eveindustry.StaticDataModels;
using RestSharp;

namespace Eveindustry
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

        private static List<ESIPriceData> EsiPriceData { get; set; } = new();

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">program command line args. </param>
        public static void Main(string[] args)
        {
            Program.bpRepository = new BlueprintsInfoRepository(new BlueprintsInfoLoader(SdeGlobalPath));
            Program.typeRepository = new EveTypeInfoRepository(new TypeInfoLoader(SdeGlobalPath));

            var allTypes = typeRepository.GetAll();
            Console.WriteLine("Starting get all prices.. ");
            var sw = Stopwatch.StartNew();
            var prices = GetEveMarketerJitaPrices(allTypes.Select(t => t.Id)).ToList();
            sw.Stop();
            Console.WriteLine($"done get all prices for {allTypes.Count} types. took: {sw.ElapsedMilliseconds} ms");
            Console.ReadKey();
            var hulk = typeRepository.FindByName(args[0]);
            var results = new List<EveBasicManufacturingInfo>();
            TraverseManufacturingTree(hulk.Id, int.Parse(args[1]), new List<int> {4051, 4246, 4247, 4312, 17476},
                ref results);

            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL LIST");
            Console.WriteLine();

            PrintStagesDetails(GroupByStages(BuildDetails(results)));

            Console.ReadKey();
        }

        private static void TraverseManufacturingTree(
            long typeId,
            int quantity,
            IEnumerable<int> terminationTypes,
            ref List<EveBasicManufacturingInfo> aggregateList)
        {
            var bp = bpRepository.FindByProductId(typeId);
            var typeInfo = typeRepository.GetById(typeId);

            var activity = bp?.Activities?.Manufacturing ?? bp?.Activities?.Reaction;

            var requiredMaterials = activity?.Materials ?? new Material[0];
            var (numberOfRuns, correctedQuantity) = GetNumbetOfRunsAndQuantity(activity, quantity);
            if (correctedQuantity == 0)
            {
                correctedQuantity = quantity;
            }

            var existedItem = aggregateList.FirstOrDefault(l => l.TypeId == typeInfo.Id);

            if (existedItem == null)
            {
                existedItem = new EveBasicManufacturingInfo()
                {
                    Quantity = 0,
                    Requirements = new(),
                    TypeId = typeId,
                };
                aggregateList.Add(existedItem);
            }

            existedItem.Quantity += correctedQuantity;

            if (bp == null || terminationTypes.Any(i => i == typeId))
            {
                return;
            }

            existedItem.Requirements = requiredMaterials.Select(r => new EveBasicManufacturingInfo
                {Quantity = r.Quantity, TypeId = r.TypeId}).ToList();
            foreach (var material in requiredMaterials)
            {
                var matInfo = typeRepository.GetById(material.TypeId);
                var requiredQuantity = material.Quantity * numberOfRuns;
                TraverseManufacturingTree(matInfo.Id, requiredQuantity, terminationTypes, ref aggregateList);
            }
        }

        private static List<EveManufacturingUnit> BuildDetails(List<EveBasicManufacturingInfo> basicData)
        {
            var typeIds = basicData.Select(d => d.TypeId).ToList();
            var adjustedPrices = GetAdjustedPrice(typeIds).ToList();
            var jitaPrices = GetEveMarketerJitaPrices(typeIds).ToList();
            var totalList = new List<EveManufacturingUnit>();

            foreach (var item in basicData)
            {
                var details = typeRepository.GetById(item.TypeId);
                var bpDetails = bpRepository.FindByProductId(item.TypeId);
                var activity = bpDetails?.Activities?.Manufacturing ?? bpDetails?.Activities?.Reaction;
                var itemsPerRun = activity?.Products?.First()?.Quantity ?? 1;
                var adjPrice = adjustedPrices.First(p => p.typeId == item.TypeId);
                var jitaPrice = jitaPrices.First(j => j.typeId == item.TypeId);
                totalList.Add(new EveManufacturingUnit()
                {
                    Quantity = item.Quantity,
                    Material = new EveItemManufacturingInfo()
                    {
                        Name = details.Name.En,
                        AdjustedPrice = adjPrice.adjustedPrice,
                        PriceBuy = jitaPrice.priceBuy,
                        PriceSell = jitaPrice.priceSell,
                        TypeId = item.TypeId,
                        ItemsPerRun = itemsPerRun,
                        Requirements = new(),
                    },
                });
            }

            // Fill item requirements lists so that it will be same objects filled with details on previous loop
            foreach (var eveItemQuantity in totalList)
            {
                var item = eveItemQuantity.Material;
                var requirements = basicData.First(b => b.TypeId == item.TypeId).Requirements;
                foreach (var requirement in requirements)
                {
                    item.Requirements.Add(new EveManufacturingUnit()
                    {
                        Quantity = requirement.Quantity,
                        Material = totalList.First(t => t.Material.TypeId == requirement.TypeId).Material,
                    });
                }
            }

            return totalList;
        }

        private static IEnumerable<(long typeId, decimal priceSell, decimal priceBuy)> GetEveMarketerJitaPrices(
            IEnumerable<long> typeIds)
        {
            var pageSize = 200;
            var typeIdsList = typeIds as long[] ?? typeIds.ToArray();
            int totalPages = (int) Math.Ceiling((double) typeIdsList.Count() / pageSize);
            var typeIDS = typeIdsList;

            var jitaId = "30000142";

            var baseUrl = "https://api.evemarketer.com/";
            var client = new RestClient(baseUrl);

            for (int pageNum = 0; pageNum < totalPages; pageNum++)
            {
                var request = new RestRequest("ec/marketstat");
                var length = (pageNum * pageSize) + pageSize > typeIdsList.Length
                    ? (typeIdsList.Length - (pageNum * pageSize))
                    : pageSize;
                var span = new ReadOnlySpan<long>(typeIdsList, pageNum * pageSize, length);
                for (int i = 0; i < span.Length; i++)
                {
                    var typeId = span[i];
                    request.AddQueryParameter("typeid", typeId.ToString());
                }

                request.AddQueryParameter("usesystem", jitaId);

                var response = client.Execute(request);
                var rawText = response.Content;

                var doc = new XmlDocument();
                doc.Load(new StringReader(rawText));
                var items = doc["exec_api"]["marketstat"];

                foreach (XmlNode childNode in items.ChildNodes)
                {
                    var id = long.Parse(childNode.Attributes["id"].Value);
                    var minSell = Decimal.Parse(childNode["sell"]["min"].InnerText);
                    var maxBuy = Decimal.Parse(childNode["buy"]["max"].InnerText);

                    yield return (id, minSell, maxBuy);
                }
            }
        }

        private static List<(long typeId, decimal adjustedPrice)> GetAdjustedPrice(IEnumerable<long> typeIds)
        {
            // https://esi.evetech.net/latest/markets/prices/
            var client = new RestClient("https://esi.evetech.net/latest");
            var pricesRequest = new RestRequest("/markets/prices/");
            var prices = client.Get<List<ESIPriceData>>(pricesRequest).Data;
            return typeIds.Select(t => (t, prices.FirstOrDefault(p => p.TypeId == t).AdjustedPrice)).ToList();
        }

        private static List<List<EveManufacturingUnit>> GroupByStages(List<EveManufacturingUnit> manufacturingList)
        {
            var builtList = new List<EveItemManufacturingInfo>();

            var stages = new List<List<EveManufacturingUnit>>();
            while (builtList.Count < manufacturingList.Count)
            {
                var stageList = new List<EveManufacturingUnit>();
                stages.Add(stageList);
                foreach (var item in manufacturingList)
                {
                    if (builtList.Contains(item.Material))
                    {
                        continue; // is that already built?
                    }

                    var requirementTypes = item.Material.Requirements;

                    if (!requirementTypes.All(i => builtList.Contains(i.Material)))
                    {
                        continue; // Is all prerequisites built?
                    }

                    stageList.Add(item);
                }

                foreach (var item in stageList)
                {
                    builtList.Add(item.Material);
                }
            }

            return stages;
        }

        private static void PrintStagesDetails(List<List<EveManufacturingUnit>> items)
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
                    "Mat sell"); // 6));
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
                    unit.MaterialsJitaSellPrice);
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

            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"--STAGE {i}--");
                PrintHeader();
                var stageList = items[i];
                foreach (var stageItem in items[i].OrderByDescending(x => x.TotalJitaSellPrice))
                {
                    PrintDetails(stageItem);
                }

                PrintTotal(stageList);
                Console.WriteLine();
            }
        }

        private static (int numberOfRuns, int quantity) GetNumbetOfRunsAndQuantity(
            BlueprintActivity activity,
            int requiredQuantity)
        {
            if (activity == null)
            {
                return (0, 0);
            }

            var builtQuantity = activity.Products.FirstOrDefault().Quantity;
            int numberOfRuns = requiredQuantity / builtQuantity;
            if (requiredQuantity % builtQuantity != 0)
            {
                numberOfRuns += 1;
            }

            var correctedQuantity = numberOfRuns * builtQuantity;
            return (numberOfRuns, correctedQuantity);
        }
    }
}