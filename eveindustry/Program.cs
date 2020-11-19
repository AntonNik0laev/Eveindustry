using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Eveindustry.StaticDataModels;

namespace Eveindustry
{
    public class ReqirementsInfo
    {
        public uint TotalQuantity { get; set; }

        public List<Material> Requirements { get; set; }

        public EveType EveType { get; set; }
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
            var quantity =  uint.Parse(args[1]);
            DumpManufacturingTree(0, hulk.Id, quantity, new List<int> {4051, 4246, 4247, 4312}, null);

            Console.WriteLine("========================================================");
            Console.WriteLine("TOTAL LIST");
            Console.WriteLine();

            DumpManufacturingStages();

            Console.ReadKey();
        }

        private static void DumpManufacturingTree(int indent, int typeId, uint quantity, List<int> terminationTypes,
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
                var requiredQuantity = (uint)material.Quantity * numberOfRuns;
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

        private static void DumpManufacturingStages()
        {
            var builtList = new List<EveType>();
            int stageNum = 0;
            while (builtList.Count < TotalList.Keys.Count)
            {
                var stageList = new List<ReqirementsInfo>();
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
                    Console.WriteLine($"{stageItem.EveType.Name} : {stageItem.TotalQuantity}");
                }

                Console.WriteLine();

                stageNum++;
            }
        }

        private static (uint numberOfRuns, uint quantity) GetNumbetOfRunsAndQuantity(
            BlueprintActivity activity,
            uint requiredQuantity)
        {
            if (activity == null)
            {
                return (0, 0);
            }

            uint builtQuantity = (uint)activity.Products.FirstOrDefault().Quantity;
            uint numberOfRuns = requiredQuantity / builtQuantity;
            if (requiredQuantity % builtQuantity != 0)
            {
                numberOfRuns += 1;
            }

            var correctedQuantity = numberOfRuns * builtQuantity;
            return (numberOfRuns, correctedQuantity);
        }
    }
}