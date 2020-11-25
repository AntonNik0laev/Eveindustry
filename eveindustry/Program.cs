using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Eveindustry.StaticDataModels;
using Newtonsoft.Json;
using RestSharp;

namespace Eveindustry
{
    public class ReqirementsInfo
    {
        public int TotalQuantity { get; set; }

        public List<Material> Requirements { get; set; }

        public EveType EveType { get; set; }
    }

    public class ESIPriceData
    {
        [JsonProperty("adjusted_price")]
        public decimal AdjustedPrice { get; set; }

        [JsonProperty("average_price")]
        public decimal AveragePrice { get; set; }

        [JsonProperty("type_id")]
        public int TypeId { get; set; }
    }

    /// <summary>
    /// Entry point class.
    /// </summary>
    internal static class Program
    {
        private static BlueprintsInfoRepository bpRepository;
        private static EveTypeInfoRepository typeRepository;
        private const string SdeGlobalPath = "D:\\data\\sde\\";

        private static Dictionary<EveType, ReqirementsInfo> TotalList =
            new Dictionary<EveType, ReqirementsInfo>();

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">program command line args. </param>
        public static void Main(string[] args)
        {
            Program.bpRepository = new BlueprintsInfoRepository(new BlueprintsInfoLoader(SdeGlobalPath));
            Program.typeRepository = new EveTypeInfoRepository(new TypeInfoLoader(SdeGlobalPath));
            var hulk = typeRepository.FindByName(args[0]);
            DumpManufacturingTree(0, hulk.Id, int.Parse(args[1]), new List<int> {4051, 4246, 4247, 4312, 17476}, null);

            UpdatePriceWithEveMarketer();
            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL LIST");
            Console.WriteLine();

            DumpManufacturingStages();

            Console.ReadKey();
        }

        private static void DumpManufacturingTree(int indent, int typeId, int quantity, List<int> terminationTypes,
            int? parentTypeId)
        {
            var bp = bpRepository.FindByProductId(typeId);
            var typeInfo = typeRepository.GetById(typeId);
            var strIndent = new string(' ', indent * 2);

            var activity = bp?.Activities?.Manufacturing ?? bp?.Activities?.Reaction;

            var requiredMaterials = activity?.Materials;
            var (numberOfRuns, correctedQuantity) = GetNumbetOfRunsAndQuantity(activity, quantity);
            if (correctedQuantity == 0)
            {
                correctedQuantity = quantity;
            }

            if (TotalList.ContainsKey(typeInfo))
            {
                TotalList[typeInfo].TotalQuantity += correctedQuantity;
            }
            else
            {
                TotalList[typeInfo] = new ReqirementsInfo()
                {
                    TotalQuantity = correctedQuantity,
                    Requirements = new List<Material>(),
                    EveType = typeInfo,
                };
            }

            if (bp == null || terminationTypes.Any(i => i == typeId))
            {
                return;
            }

            TotalList[typeInfo].Requirements = requiredMaterials.ToList();
            foreach (var material in requiredMaterials)
            {
                var matInfo = typeRepository.GetById(material.TypeId);
                var requiredQuantity = material.Quantity * numberOfRuns;
                DumpManufacturingTree(indent + 1, matInfo.Id, requiredQuantity, terminationTypes, typeId);
            }
        }

        private static void SearchType(string name)
        {
            var types = Program.typeRepository.Search(name);
            foreach (var type in types)
            {
                Console.WriteLine($"{type.Id} - {type.Name.En}");
            }
        }

        private static void UpdatePriceWithEveMarketer()
        {
            var typeIDS = TotalList.Keys.Select(k => k.Id).ToList();

            var jitaId = "30000142";

            var baseUrl = "https://api.evemarketer.com/";
            var client = new RestClient(baseUrl);

            var request = new RestRequest("ec/marketstat");
            foreach (var typeId in typeIDS)
            {
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
                var id = childNode.Attributes["id"].Value;
                var minSell = Decimal.Parse(childNode["sell"]["min"].InnerText);
                var maxBuy = Decimal.Parse(childNode["buy"]["max"].InnerText);

                var item = TotalList.FirstOrDefault(i => i.Key.Id == int.Parse(id));

                item.Key.PriceBuy = maxBuy;
                item.Key.PriceSell = minSell;
            }
        }

        private static void DumpManufacturingStages()
        {
            var builtList = new List<EveType>();
            int stageNum = 0;
            while (builtList.Count < TotalList.Keys.Count)
            {
                var stageList = new List<ReqirementsInfo>();
                decimal stageSell = 0;
                decimal stageBuy = 0;
                foreach (var item in TotalList)
                {
                    if (builtList.Contains(item.Key))
                    {
                        continue; // is that already built?
                    }

                    var requirementTypes =
                        item.Value.Requirements.Select(r => Program.typeRepository.GetById(r.TypeId));

                    if (!requirementTypes.All(i => builtList.Contains(i)))
                    {
                        continue; // Is all prerequisites built?
                    }

                    stageList.Add(new ReqirementsInfo()
                    {
                        TotalQuantity = item.Value.TotalQuantity,
                        EveType = item.Value.EveType
                    });
                }

                Console.WriteLine($"STAGE {stageNum}");
                foreach (var stageItem in stageList)
                {
                    builtList.Add(stageItem.EveType);
                    var priceSell = stageItem.EveType.PriceSell * stageItem.TotalQuantity;
                    var priceBuy = stageItem.EveType.PriceBuy * stageItem.TotalQuantity;

                    stageSell += priceSell;
                    stageBuy += priceBuy;
                    Console.WriteLine(
                        $"{stageItem.EveType.Name} : {stageItem.TotalQuantity}. $ {priceSell:N} / {priceBuy:N}");
                }

                Console.WriteLine($"Stage TOTAL $: {stageSell:N} / {stageBuy:N}");
                Console.WriteLine();

                stageNum++;
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