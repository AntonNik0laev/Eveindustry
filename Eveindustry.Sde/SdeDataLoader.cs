using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Eveindustry.Sde.Loaders.Internal;
using Eveindustry.Sde.Models;
using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Models.Internal;
using Eveindustry.Sde.Utils;
using MessagePack;
using Microsoft.Extensions.Options;

namespace Eveindustry.Sde
{
    public class SdeDataLoader: ISdeDataLoader
    {
        private readonly ISdeBasicTypeInfoLoader typeInfoLoader;
        private readonly ISdeBlueprintsInfoLoader bpLoader;
        private readonly ISdeBasicCategoriesLoader categoriesLoader;
        private readonly ISdeBasicGroupsLoader groupsLoader;
        private string cacheFileName = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "sdedata.bin");

        public SdeDataLoader(IOptions<TypeInfoLoaderOptions> options) : this(
            new SdeBasicTypeInfoLoader(options.Value),
            new SdeBlueprintsInfoLoader(options.Value),
            new SdeBasicCategoriesLoader(options.Value),
            new SdeBasicGroupsLoader(options.Value))
        {
            
        }

        internal SdeDataLoader(
            ISdeBasicTypeInfoLoader typeInfoLoader, 
            ISdeBlueprintsInfoLoader bpLoader,
            ISdeBasicCategoriesLoader categoriesLoader,
            ISdeBasicGroupsLoader groupsLoader)
        {
            this.typeInfoLoader = typeInfoLoader;
            this.bpLoader = bpLoader;
            this.categoriesLoader = categoriesLoader;
            this.groupsLoader = groupsLoader;
        }
        
        public async Task<SortedList<long, SdeType>> Load()
        {
            if (!File.Exists(cacheFileName)) return await Build();
            
            return await SerializationUtils.ReadFromBinary<SortedList<long, SdeType>>(cacheFileName);
        }

        private async Task<SortedList<long, SdeType>> Build()
        {
            var allTypes = await this.typeInfoLoader.Load();
            var allBp = await this.bpLoader.Load();

            var allCategories = await this.categoriesLoader.Load();
            var allGroups = await this.groupsLoader.Load();
            
            var targetList = new SortedList<long, SdeType>();

            foreach (var (typeId, value) in allTypes)
            {
                var basicGroup = allGroups[value.GroupId];
                var basicCategory = allCategories[basicGroup.CategoryId];

                var type = new SdeType()
                {
                    Id = typeId,
                    Name = value.Name.En,
                    Group = new SdeGroup()
                    {
                        Id = value.GroupId,
                        Name = basicGroup.Name.En
                    },
                    Category = new SdeCategory()
                    {
                        Id = basicGroup.CategoryId,
                        Name = basicCategory.Name.En
                    },
                    Blueprint = GetBlueprintByProductId(allBp, allTypes, typeId),
                    MarketGroupId = value.MarketGroupID
                    
                };
                targetList.Add(type.Id, type);
            }

            await SerializationUtils.DumpBinary(this.cacheFileName, targetList);

            return targetList;
        }

        private SdeBlueprint GetBlueprintByProductId(SortedList<long, SdeBlueprintInfo> allBp, IReadOnlyDictionary<long, SdeEveBasicType> allTypes, long productId)
        {
            foreach (var (key, value) in allBp)
            {
                var activity = value.Activities?.Manufacturing ?? value.Activities?.Reaction;
                var bpProduct = activity?.Products?[0];
                var bpProductId = bpProduct?.TypeId;
                if (bpProductId != productId) continue;
                
                var bpType = allTypes[key];

                return new SdeBlueprint()
                {
                    Id = key,
                    MaterialRequirements = activity?.Materials?.Select(i => new SdeMaterialRequirement()
                    {
                        Quantity = i.Quantity,
                        MaterialId = i.TypeId
                    }).ToList(),
                    ItemsPerRun = bpProduct.Quantity,
                    ProducedTypeId = bpProduct.TypeId,
                    Name = bpType.Name.En
                };
            }

            return null;
        }
    }
}