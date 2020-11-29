using System.Collections.Generic;
using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.Core
{
    /// <summary>
    /// Data Loader for Eve SDE Blueprints file.
    /// </summary>
    public interface IBlueprintsInfoLoader : IDataLoader<Dictionary<string, BlueprintInfo>>
    {
    }
}