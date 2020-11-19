using System;
using System.Collections.Generic;
using System.Linq;
using Eveindustry.StaticDataModels;

namespace Eveindustry
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
        }

        /// <inheritdoc />
        public EveType GetById(int id)
        {
            var result = this.data[id.ToString()];
            if (result != null)
            {
                result.Id = id;
            }

            return result;
        }

        /// <inheritdoc/>
        public EveType FindByName(string name)
        {
            var (key, value) = this.data.FirstOrDefault(d => d.Value.Name.En == name);
            if (key != null)
            {
                value.Id = int.Parse(key);
            }

            return value;
        }

        /// <inheritdoc />
        public List<EveType> Search(string partName)
        {
            var matched = this.data.Where(d => d.Value.Name.En.Contains(partName, StringComparison.InvariantCultureIgnoreCase));
            foreach (var match in matched)
            {
                match.Value.Id = int.Parse(match.Key);
            }

            return matched.Select(m => m.Value).ToList();
        }
    }
}