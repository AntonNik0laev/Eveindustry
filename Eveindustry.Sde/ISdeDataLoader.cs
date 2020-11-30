using System.Collections.Generic;
using Eveindustry.Sde.Loaders;
using Eveindustry.Sde.Models;

namespace Eveindustry.Sde
{
    public interface ISdeDataLoader : IDataLoader<SortedList<long, SdeType>>
    {
    }
}