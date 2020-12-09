using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Eveindustry.Shared.DTO.EveType;

namespace Eveindustry.Shared
{
    /// <inheritdoc />
    public class ManufacturingInfoBuilder : IManufacturingInfoBuilder
    {
        private readonly IDictionary<long, EveTypeDto> types;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturingInfoBuilder"/> class.
        /// </summary>
        public ManufacturingInfoBuilder(IMapper mapper)
        {
            this.types = types;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public EveItemManufacturingInfo BuildInfo(long id, IDictionary<long, EveTypeDto> types)
        {
            return this.BuildInfoRecursive(id, types, new SortedList<long, EveItemManufacturingInfo>());
        }

        /// <inheritdoc />
        public SortedList<long, EveItemManufacturingInfo> BuildInfo(IEnumerable<long> typeIds, IDictionary<long, EveTypeDto> types)
        {
            var result = new SortedList<long, EveItemManufacturingInfo>();
            foreach (var typeId in typeIds)
            {
                // Method will add result item to list, so discarding return.
                _ = this.BuildInfoRecursive(typeId, types, result);
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EveManufacturialQuantity> GetFlatManufacturingList(
            EveItemManufacturingInfo info,
            long quantity,
            IEnumerable<long> ignoreTypeIds)
        {
            var totalList = new SortedList<long, EveManufacturialQuantity>();
            var typeIds = ignoreTypeIds?.ToList() ?? new List<long>();

            void BuildFlatListRecursive(EveManufacturialQuantity currentItem)
            {
                var currentMaterial = currentItem.Material;

                if (!totalList.ContainsKey(currentItem.Material.Id))
                {
                    totalList.Add(currentItem.Material.Id, new EveManufacturialQuantity()
                    {
                        Material = currentMaterial,
                        Quantity = 0,
                    });
                }

                var (numberOfRuns, correctedQuantity, remainingQuantity) =
                    this.GetNumbetOfRunsAndQuantity(
                        currentMaterial.ItemsPerRun,
                        currentItem.Quantity,
                        totalList[currentMaterial.Id].RemainingQuantity);
                if (!currentMaterial.CanBeManufactured)
                {
                    correctedQuantity = currentItem.Quantity;
                }

                totalList[currentMaterial.Id].Quantity += correctedQuantity;
                totalList[currentMaterial.Id].RemainingQuantity = remainingQuantity;
                if (typeIds.Any(i => i == currentItem.Material.Id))
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
                    BuildFlatListRecursive(new EveManufacturialQuantity()
                    {
                        Material = material.Material,
                        Quantity = requiredQuantity,
                    });
                }
            }

            BuildFlatListRecursive(new EveManufacturialQuantity {Material = info, Quantity = quantity });
            return totalList.Values;
        }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<EveManufacturialQuantity>> GroupIntoStages(
            IEnumerable<EveManufacturialQuantity> flatList, IEnumerable<long> ignoredTypeIds)
        {
            var builtList = new List<long>();
            
            var stages = new List<List<EveManufacturialQuantity>>();
            var eveManufacturingUnits = flatList.ToList();

            while (builtList.Count < eveManufacturingUnits.Count)
            {
                var stageList = new List<EveManufacturialQuantity>();
                stages.Add(stageList);
                foreach (var item in eveManufacturingUnits)
                {
                    if (builtList.Contains(item.Material.Id))
                    {
                        continue; // is that already built?
                    }

                    var requirementTypes = item.Material.Requirements;
                    if (ignoredTypeIds.Contains(item.Material.Id))
                    {
                        stageList.Add(item);
                        continue;
                    }

                    if (!requirementTypes.All(i => builtList.Contains(i.Material.Id)))
                    {
                        continue; // Is all prerequisites built?
                    }

                    stageList.Add(item);
                }

                foreach (var item in stageList)
                {
                    builtList.Add(item.Material.Id);
                }
            }

            return stages;
        }

        private EveItemManufacturingInfo BuildInfoRecursive(
            long id,
            IDictionary<long, EveTypeDto> types,
            SortedList<long, EveItemManufacturingInfo> allResults)
        {
            var eveTypeInfo = types[id];

            var dependencies = eveTypeInfo.Blueprint?.Materials ?? new List<EveMaterialRequirement>();
            var itemsPerRun = eveTypeInfo.Blueprint?.ItemsPerRun;

            var manufacturingInfo = mapper.Map<EveTypeDto, EveItemManufacturingInfo>(eveTypeInfo);
            if (!allResults.ContainsKey(id))
            {
                allResults.Add(id, manufacturingInfo);
            }

            manufacturingInfo.Requirements = new List<EveManufacturialQuantity>();
            foreach (var dependency in dependencies)
            {
                var dependencyInfo = allResults.ContainsKey(dependency.MaterialId)
                    ? allResults[dependency.MaterialId]
                    : this.BuildInfoRecursive(dependency.MaterialId, types, allResults);
                manufacturingInfo.Requirements.Add(new EveManufacturialQuantity()
                {
                    Quantity = dependency.Quantity,
                    Material = dependencyInfo,
                });
            }

            return manufacturingInfo;
        }

        private (long NumberOfRuns, long Quantity, long RemainingQuantity) GetNumbetOfRunsAndQuantity(long itemsPerRun,
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