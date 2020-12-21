using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Eveindustry.Shared.DTO;
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
        public EveItemManufacturingInfo BuildInfo(long id, IDictionary<long, EveTypeManufacturingParameters> types)
        {
            return this.BuildInfoRecursive(id, types, new SortedList<long, EveItemManufacturingInfo>());
        }

        /// <inheritdoc />
        public SortedList<long, EveItemManufacturingInfo> BuildInfo(IEnumerable<long> typeIds, IDictionary<long, EveTypeManufacturingParameters> types)
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
        public IDictionary<long, EveManufacturialQuantity> GetFlatManufacturingList(
            EveItemManufacturingInfo info,
            long quantity,
            IDictionary<long, EveManufacturialQuantity> existedItems = null)
        {
            var totalList = existedItems ?? new SortedList<long, EveManufacturialQuantity>();
            foreach (var eveManufacturialQuantity in totalList)
            {
                eveManufacturialQuantity.Value.Quantity = 0;
                eveManufacturialQuantity.Value.RemainingQuantity = 0;
            }

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
                if (currentItem.Material.ForceBuy)
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
                    Console.WriteLine($"BUILDER: applying material reduction. {currentMaterial.Name}: ME: {currentMaterial.BlueprintME}; rig: {currentMaterial.FacilityRigKind}; " +
                                      $"facility: {currentMaterial.FacilityKind}; quantity: {material.Quantity}");
                    
                    var reducedQuantity = ApplyMaterialReduction(requiredQuantity, currentMaterial.BlueprintME, currentMaterial.FacilityKind, currentMaterial.FacilityRigKind, material.Quantity);
                    
                    BuildFlatListRecursive(new EveManufacturialQuantity()
                    {
                        Material = material.Material,
                        Quantity = reducedQuantity,
                    });
                }
            }

            BuildFlatListRecursive(new EveManufacturialQuantity {Material = info, Quantity = quantity });

            List<long> idsToRemove = new List<long>();

            foreach (var dependency in totalList.Values)
            {
                var id = dependency.Material.Id;
                var required = false;
                foreach (var dependant in totalList.Values)
                {
                    if(dependant.Material.ForceBuy) continue;
                    if (id == info.Id)
                    {
                        required = true;
                        break;
                    }
                    if (dependant.Material.Requirements.Select(r => r.Material).Contains(dependency.Material))
                    {
                        required = true;
                        break;
                    };
                }

                if (!required)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (var cleanupId in idsToRemove)
            {
                totalList.Remove(cleanupId);
            }

            return totalList;
        }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<EveManufacturialQuantity>> GroupIntoStages(IEnumerable<EveManufacturialQuantity> flatList)
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
                    if (item.Material.ForceBuy)
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
            IDictionary<long, EveTypeManufacturingParameters> types,
            SortedList<long, EveItemManufacturingInfo> allResults)
        {
            var eveTypeInfo = types[id].EveType;

            var dependencies = eveTypeInfo.Blueprint?.Materials ?? new List<EveMaterialRequirement>();
            var itemsPerRun = eveTypeInfo.Blueprint?.ItemsPerRun;

            var manufacturingInfo = mapper.Map<EveTypeDto, EveItemManufacturingInfo>(eveTypeInfo);
            mapper.Map(types[id].Parameters, manufacturingInfo);
            
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
                    Material = dependencyInfo
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

        private long ApplyMaterialReduction(long originalQuantity, int blueprintMe,
            FacilityKinds facilityKind, FacilityRigKinds rigKind, long itemsPerRun)
        {
            var meReduction = (double)blueprintMe / 100;
            var facilityKindReduction = facilityKind switch
            {
                FacilityKinds.EngeneeringComplex => 0.01,
                FacilityKinds.Other or _ => 0
            };
            var facilityRigReduction = rigKind switch
            {
                FacilityRigKinds.T1 => 0.042,
                FacilityRigKinds.T2 => 0.0504,
                FacilityRigKinds.None or _ => 0

            };

            if (itemsPerRun <= 1)
            {
                return originalQuantity;
            }

            var reduced = originalQuantity * (1 - meReduction) * (1 - facilityRigReduction) *
                          (1 - facilityKindReduction);
            Console.WriteLine($"Original: {originalQuantity}; reduced: {reduced}");
            return (long) Math.Round(reduced);
        }
    }
}