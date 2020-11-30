using System.Collections.Generic;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    /// <summary>
    /// Data Loader for Eve SDE Blueprints file.
    /// </summary>
    internal interface ISdeBlueprintsInfoLoader : IDataLoader<SortedList<long, SdeBlueprintInfo>>
    {
    }
}