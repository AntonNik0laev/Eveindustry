using System.Collections.Generic;
using System.Linq;
using Eveindustry.StaticDataModels;

namespace Eveindustry
{
    /// <inheritdoc />
    public class BlueprintsInfoRepository : IBlueprintsInfoRepository
    {
        private readonly Dictionary<string, BlueprintInfo> details;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlueprintsInfoRepository"/> class.
        /// </summary>
        /// <param name="loader">SDE loader for blueprints file. </param>
        public BlueprintsInfoRepository(IBlueprintsInfoLoader loader)
        {
            this.details = loader.Load();
        }

        /// <inheritdoc />
        public BlueprintInfo GetByBluprintId(int blueprintId)
        {
            return this.details[blueprintId.ToString()];
        }

        /// <inheritdoc />
        public BlueprintInfo FindByProductId(int productId)
        {
            var byManufacturing = this.details.Values.FirstOrDefault(v =>
                v.Activities.Manufacturing?.Products?.Any(p => p.TypeId == productId) ?? false);
            var byResearch = this.details.Values.FirstOrDefault(v =>
                v.Activities.Reaction?.Products?.Any(p => p.TypeId == productId) ?? false);
            return byManufacturing ?? byResearch;
        }
    }
}