using System.Collections.Generic;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    internal interface ISdeBasicGroupsLoader: IDataLoader<SortedList<long, SdeBasicGroup>> { }
}