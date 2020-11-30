using System.Collections.Generic;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    /// <summary>
    /// Load eve type info from SDE.
    /// </summary>
    internal interface ISdeBasicTypeInfoLoader : IDataLoader<SortedList<long, SdeEveBasicType>>
    {
    }
}