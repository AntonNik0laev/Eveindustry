using System.Collections.Generic;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    internal interface ISdeBasicCategoriesLoader: IDataLoader<SortedList<long, SdeBasicCategory>> {}
}