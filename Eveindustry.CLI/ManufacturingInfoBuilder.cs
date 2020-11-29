using System;
using System.Collections.Generic;
using System.Linq;
using Eveindustry.CLI.Models;
using Eveindustry.CLI.StaticDataModels;

namespace Eveindustry.CLI
{
    /// <inheritdoc />
    public class ManufacturingInfoBuilder : IManufacturingInfoBuilder
    {
        private readonly IEveTypeInfoRepository typeInfoRepository;
        private readonly IBlueprintsInfoRepository bpRepository;
        private readonly IEvePricesRepository pricesRepository;
        private readonly IEsiPricesRepository esiPricesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturingInfoBuilder"/> class.
        /// </summary>
        /// <param name="typeInfoRepository">type info repository. </param>
        /// <param name="bpRepository">blueprints repository. </param>
        /// <param name="pricesRepository">prices repository. </param>
        /// <param name="esiPricesRepository">ESI adjusted/average prices repository. </param>
        public ManufacturingInfoBuilder(
            IEveTypeInfoRepository typeInfoRepository,
            IBlueprintsInfoRepository bpRepository,
            IEvePricesRepository pricesRepository,
            IEsiPricesRepository esiPricesRepository)
        {
            this.typeInfoRepository = typeInfoRepository;
            this.bpRepository = bpRepository;
            this.pricesRepository = pricesRepository;
            this.esiPricesRepository = esiPricesRepository;
        }

        /// <inheritdoc />
        public EveItemManufacturingInfo BuildInfo(long id)
        {
            return this.BuildInfoRecursive(id, new SortedList<long, EveItemManufacturingInfo>());
        }

        /// <inheritdoc />
        public SortedList<long, EveItemManufacturingInfo> BuildInfo(IEnumerable<long> typeIds)
        {
            var result = new SortedList<long, EveItemManufacturingInfo>();
            foreach (var typeId in typeIds)
            {
                // Method will add result item to list, so discarding return.
                _ = this.BuildInfoRecursive(typeId, result);
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EveManufacturingUnit> GetFlatManufacturingList(
            EveItemManufacturingInfo info,
            long quantity,
            IEnumerable<long> ignoreTypeIds)
        {
            var totalList = new SortedList<long, EveManufacturingUnit>();
            var typeIds = ignoreTypeIds.ToList();

            void BuildFlatListRecursive(EveManufacturingUnit currentItem)
            {
                var currentMaterial = currentItem.Material;

                if (!totalList.ContainsKey(currentItem.Material.TypeId))
                {
                    totalList.Add(currentItem.Material.TypeId, new EveManufacturingUnit()
                    {
                        Material = currentMaterial,
                        Quantity = 0,
                    });
                }

                var (numberOfRuns, correctedQuantity, remainingQuantity) =
                    this.GetNumbetOfRunsAndQuantity(
                        currentMaterial.ItemsPerRun,
                        currentItem.Quantity,
                        totalList[currentMaterial.TypeId].RemainingQuantity);
                if (!currentMaterial.CanBeManufactured)
                {
                    correctedQuantity = currentItem.Quantity;
                }

                totalList[currentMaterial.TypeId].Quantity += correctedQuantity;
                totalList[currentMaterial.TypeId].RemainingQuantity = remainingQuantity;
                if (typeIds.Any(i => i == currentItem.Material.TypeId))
                {
                    return;
                }

                if (correctedQuantity == 0)
                {
                    return;
                }

                foreach (var material in currentMaterial.Requirements)
                {
                    var requiredQuantity = material.Quantity * numberOfRuns;
                    BuildFlatListRecursive(new EveManufacturingUnit()
                    {
                        Material = material.Material,
                        Quantity = requiredQuantity,
                    });
                }
            }

            BuildFlatListRecursive(new EveManufacturingUnit {Material = info, Quantity = quantity });
            return totalList.Values;
        }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<EveManufacturingUnit>> GroupIntoStages(
            IEnumerable<EveManufacturingUnit> flatList, IEnumerable<long> ignoredTypeIds)
        {
            var builtList = new List<long>();
            
            var stages = new List<List<EveManufacturingUnit>>();
            var eveManufacturingUnits = flatList.ToList();

            while (builtList.Count < eveManufacturingUnits.Count)
            {
                var stageList = new List<EveManufacturingUnit>();
                stages.Add(stageList);
                foreach (var item in eveManufacturingUnits)
                {
                    if (builtList.Contains(item.Material.TypeId))
                    {
                        continue; // is that already built?
                    }

                    var requirementTypes = item.Material.Requirements;
                    if (ignoredTypeIds.Contains(item.Material.TypeId))
                    {
                        stageList.Add(item);
                        continue;
                    }

                    if (!requirementTypes.All(i => builtList.Contains(i.Material.TypeId)))
                    {
                        continue; // Is all prerequisites built?
                    }

                    stageList.Add(item);
                }

                foreach (var item in stageList)
                {
                    builtList.Add(item.Material.TypeId);
                }
            }

            return stages;
        }

        private EveItemManufacturingInfo BuildInfoRecursive(
            long id,
            SortedList<long, EveItemManufacturingInfo> allResults)
        {
            var adjustedPrice = this.esiPricesRepository.GetAdjustedPriceInfo(id);
            var jitaPrices = this.pricesRepository.GetPriceInfo(id);
            var eveTypeInfo = this.typeInfoRepository.GetById(id);
            var bpItem = this.bpRepository.FindByProductId(id);

            var activity = bpItem?.Activities?.Manufacturing ?? bpItem?.Activities?.Reaction;
            var dependencies = activity?.Materials ?? new Material[0];
            var itemsPerRun = activity?.Products?[0]?.Quantity ?? 1;

            var manufacturingInfo = new EveItemManufacturingInfo()
            {
                Name = eveTypeInfo.Name.En,
                Requirements = new List<EveManufacturingUnit>(),
                AdjustedPrice = adjustedPrice.AdjustedPrice,
                PriceBuy = jitaPrices.JitaBuy,
                PriceSell = jitaPrices.JitaSell,
                TypeId = id,
                ItemsPerRun = itemsPerRun,
            };

            if (!allResults.ContainsKey(id))
            {
                allResults.Add(id, manufacturingInfo);
            }

            foreach (var dependency in dependencies)
            {
                var dependencyInfo = allResults.ContainsKey(dependency.TypeId)
                    ? allResults[dependency.TypeId]
                    : this.BuildInfoRecursive(dependency.TypeId, allResults);
                manufacturingInfo.Requirements.Add(new EveManufacturingUnit()
                {
                    Quantity = dependency.Quantity,
                    Material = dependencyInfo,
                });
            }

            return manufacturingInfo;
        }

        private (long NumberOfRuns, long Quantity, long RemainingQuantity) GetNumbetOfRunsAndQuantity(int itemsPerRun,
            long requiredQuantity, long remainingQuantity)
        {
            var discountedRequiredQuantity = requiredQuantity - remainingQuantity;
            var discountedRemainingQuantity = Math.Max(0, remainingQuantity - requiredQuantity );
            if (discountedRequiredQuantity <= 0)
            {
                return (0, 0, discountedRemainingQuantity);
            }

            long numberOfRuns = (long) Math.Ceiling(discountedRequiredQuantity / (double) itemsPerRun);

            var correctedQuantity = numberOfRuns * itemsPerRun;
            var totalBuilt = correctedQuantity + remainingQuantity;
            var addedRemainingQuantity = totalBuilt - requiredQuantity;
            return (numberOfRuns, correctedQuantity, addedRemainingQuantity);
        }
    }
}