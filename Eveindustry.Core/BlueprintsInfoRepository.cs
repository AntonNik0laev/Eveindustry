﻿using System.Collections.Generic;
using System.Linq;
using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.Core
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
        public BlueprintInfo GetByBluprintId(long blueprintId)
        {
            return this.details[blueprintId.ToString()];
        }

        /// <inheritdoc />
        public BlueprintInfo FindByProductId(long productId)
        {
            var byManufacturing = this.details.Values.FirstOrDefault(v =>
                v.Activities.Manufacturing?.Products?.Any(p => p.TypeId == productId) ?? false);
            var byResearch = this.details.Values.FirstOrDefault(v =>
                v.Activities.Reaction?.Products?.Any(p => p.TypeId == productId) ?? false);
            return byManufacturing ?? byResearch;
        }
    }
}