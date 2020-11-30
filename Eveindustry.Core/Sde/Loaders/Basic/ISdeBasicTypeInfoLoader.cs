using System.Collections.Generic;
using Eveindustry.Core.Sde.Models;
using Eveindustry.Core.Sde.Models.Basic;

namespace Eveindustry.Core.Sde.Loaders.Basic
{
    /// <summary>
    /// Load eve type info from SDE.
    /// </summary>
    internal interface ISdeBasicTypeInfoLoader : IDataLoader<SortedList<long, SdeType>>
    {
    }
}