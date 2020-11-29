using System.Collections.Generic;

namespace Eveindustry.Core.StaticDataModels
{
    /// <summary>
    /// Load eve type info from SDE.
    /// </summary>
    public interface ITypeInfoLoader : IDataLoader<Dictionary<string, EveType>>
    {
    }
}