using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Eveindustry.Core.Models;
using Eveindustry.Sde;
using Eveindustry.Sde.Models;
using Eveindustry.Sde.Repositories;

namespace Eveindustry.Core
{
    /// <inheritdoc />
    public class EveTypeRepository : IEveTypeRepository
    {
        private readonly IMapper mapper;
        private readonly ISdeDataRepository sdeDataRepository;
        private readonly IEsiPricesRepository esiPrices;
        private readonly Lazy<IEvePricesRepository> evePrices;

        private SortedList<long, EveType> allTypes;

        /// <summary>
        /// Create new instance of <see cref="EveTypeRepository"/>
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="sdeDataRepository"></param>
        /// <param name="esiPrices"></param>
        /// <param name="evePrices"></param>
        public EveTypeRepository(IMapper mapper, ISdeDataRepository sdeDataRepository, IEsiPricesRepository esiPrices, Lazy<IEvePricesRepository> evePrices)
        {
            this.mapper = mapper;
            this.sdeDataRepository = sdeDataRepository;
            this.esiPrices = esiPrices;
            this.evePrices = evePrices;
        }

        /// <summary>
        /// Load all required data.
        /// </summary>
        public Task Init()
        {
            var allSde = this.sdeDataRepository.GetAll();
            this.allTypes = new SortedList<long, EveType>(allSde.Count);
            foreach (var sdeType in allSde)
            {
                var newType = mapper.Map<SdeType, EveType>(sdeType.Value);
                var marketPrices = this.evePrices.Value.GetPriceInfo(newType.Id);
                var adjustedPrices = this.esiPrices.GetAdjustedPriceInfo(newType.Id);
                mapper.Map(marketPrices, newType);
                mapper.Map(adjustedPrices, newType);
                this.allTypes.Add(sdeType.Key, newType);
                
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public EveType GetById(long id)
        {
            return this.allTypes[id];
        }

        /// <inheritdoc />
        public IList<EveType> GetAll()
        {
            return this.allTypes.Values;
        }

        /// <inheritdoc />
        public IList<EveType> GetAllDependencies(long rootTypeId)
        {
            SortedList<long, EveType> allResults = new();

            void GetAllDependentIdsRecursive(long currentTypeId)
            {
                if(allResults.ContainsKey(currentTypeId)) return;
                var item = this.GetById(currentTypeId);
                allResults.Add(currentTypeId, item);
                
                foreach (var material in item.Blueprint?.Materials ?? new List<EveMaterialRequirement>())
                {
                    GetAllDependentIdsRecursive(material.MaterialId);
                }
            }
            GetAllDependentIdsRecursive(rootTypeId);
            return allResults.Values;
        }

        /// <inheritdoc />
        public EveType GetByExactName(string name)
        {
            return this.FindByPartialName(name, FindByPartialNameOptions.ExactMatch).FirstOrDefault();
        }
        
        /// <inheritdoc />
        public List<EveType> FindByPartialName(string partialName, FindByPartialNameOptions options)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;
            Func<EveType, bool> predicate = options switch
            {
                FindByPartialNameOptions.ExactMatch => type =>
                    string.Equals(type.Name, partialName, comparison),
                FindByPartialNameOptions.Contains => type =>
                    type.Name.Contains(partialName, comparison),
                FindByPartialNameOptions.StartingWith => type => type.Name.StartsWith(partialName, comparison),
                _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
            };
            return this.allTypes.Values.Where(t => t?.Name != null && t?.Blueprint != null && predicate(t)).ToList();
        }
    }
}