using System.Collections.Generic;
using Eveindustry.Core.Sde.Models.Basic;

namespace Eveindustry.Core.Sde.Loaders.Basic
{
    /// <summary>
    /// Data Loader for Eve SDE Blueprints file.
    /// </summary>
    internal interface ISdeBlueprintsInfoLoader : IDataLoader<SortedList<long, SdeBlueprintInfo>>
    {
    }
}