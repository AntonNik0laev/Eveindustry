using System.Collections.Generic;
using Eveindustry.CLI.StaticDataModels;

namespace Eveindustry.CLI
{
    /// <summary>
    /// Data Loader for Eve SDE Blueprints file.
    /// </summary>
    public interface IBlueprintsInfoLoader : IDataLoader<Dictionary<string, BlueprintInfo>>
    {
    }
}