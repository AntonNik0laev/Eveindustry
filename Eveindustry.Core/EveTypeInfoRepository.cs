using System;
using System.Collections.Generic;
using System.Linq;
using Eveindustry.Core.Models;
using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.Core
{
    /// <inheritdoc />
    public class EveTypeInfoRepository : IEveTypeInfoRepository
    {
        private Dictionary<string, EveType> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveTypeInfoRepository"/> class.
        /// </summary>
        /// <param name="loader">eve type info loader. </param>
        public EveTypeInfoRepository(ITypeInfoLoader loader)
        {
            this.data = loader.Load();
            this.SetIds();
        }

        /// <inheritdoc />
        public EveType GetById(long id)
        {
            var result = this.data[id.ToString()];

            return result;
        }

        /// <inheritdoc/>
        public EveType FindByExactName(string name)
        {
            var (key, value) = this.data.FirstOrDefault(d => d.Value.Name.En == name);

            return value;
        }

        /// <inheritdoc />
        public List<EveType> FindByPartialName(string partialName, FindByPartialNameOptions options)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;
            Func<EveType, bool> predicate = options switch
            {
                FindByPartialNameOptions.ExactMatch => type =>
                    string.Equals(type.Name.En, partialName, comparison),
                FindByPartialNameOptions.Contains => type =>
                    type.Name.En.Contains(partialName, comparison),
                FindByPartialNameOptions.StartingWith => type => type.Name.En.StartsWith(partialName, comparison),
                _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
            };
            return this.data.Values.Where(t => t?.Name?.En != null && predicate(t)).ToList();
        }

        /// <inheritdoc />
        public List<EveType> Search(string partName)
        {
            var matched = this.data.Where(d =>
                d.Value.Name.En.Contains(partName, StringComparison.InvariantCultureIgnoreCase));
            foreach (var match in matched)
            {
                match.Value.Id = int.Parse(match.Key);
            }

            return matched.Select(m => m.Value).ToList();
        }

        /// <inheritdoc />
        public List<EveType> GetAll()
        {
            return this.data.Values.ToList();
        }

        private void SetIds()
        {
            foreach (var kvp in this.data)
            {
                kvp.Value.Id = long.Parse(kvp.Key);
            }
        }
    }
}