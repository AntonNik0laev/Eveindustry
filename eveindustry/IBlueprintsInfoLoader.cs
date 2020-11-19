using System.Collections.Generic;
using Eveindustry.StaticDataModels;

namespace Eveindustry
{
    /// <summary>
    /// Data Loader for Eve SDE Blueprints file.
    /// </summary>
    public interface IBlueprintsInfoLoader : IDataLoader<Dictionary<string, BlueprintInfo>>
    {
    }
}