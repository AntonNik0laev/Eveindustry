using System.Collections.Generic;
using System.Linq;
using Eveindustry.Core.Models;

namespace Eveindustry.Core
{
    /// <summary>
    /// Build manufacturing info relative details, using multiple data sources.
    /// </summary>
    public interface IManufacturingInfoBuilder
    {
        /// <summary>
        /// Build manufacturing info relative details.
        /// Details contains -
        ///     1. item basic info (name, id etc) data from Eve SDE files.
        ///     2. item blueprint information data from Eve SDE files.
        ///     3. item current Jita prices imported from evemarketer.
        ///     4. Item adjusted/average prices imported from eve ESI.
        /// </summary>
        /// <param name="id">item id. </param>
        /// <returns>manufacturing details information. </returns>
        EveItemManufacturingInfo BuildInfo(long id);

        /// <summary>
        /// Build manufacturing info details for list of ids. See <see cref="BuildInfo(long)"/>
        /// </summary>
        /// <param name="typeIds">list of type ids to get detailed information. </param>
        /// <returns>manufacturing details information. </returns>
        SortedList<long, EveItemManufacturingInfo> BuildInfo(IEnumerable<long> typeIds);

        /// <summary>
        /// Given item manufacturing info with all it's dependencies,
        /// build flat total list of items required at all stages.
        /// </summary>
        /// <param name="info">root item. </param>
        /// <param name="quantity">root item quantity required. </param>
        /// <param name="ignoreTypeIds">list of ids to ignore. </param>
        /// <returns>flat list with all required items to build given item. </returns>
        IEnumerable<EveManufacturingUnit> GetFlatManufacturingList(
            EveItemManufacturingInfo info,
            long quantity,
            IEnumerable<long> ignoreTypeIds = null);

        /// <summary>
        /// Group flat list of items so that each 'level' contains elements produced on previous level.
        /// </summary>
        /// <param name="flatList">flat list of items required. </param>
        /// <returns>grouped list. </returns>
        public IEnumerable<IEnumerable<EveManufacturingUnit>> GroupIntoStages(
            IEnumerable<EveManufacturingUnit> flatList)
        {
            var builtList = new List<EveItemManufacturingInfo>();

            var stages = new List<List<EveManufacturingUnit>>();
            var manufacturingList = flatList.ToList();
            while (builtList.Count < manufacturingList.Count())
            {
                var stageList = new List<EveManufacturingUnit>();
                stages.Add(stageList);
                foreach (var item in manufacturingList)
                {
                    if (builtList.Contains(item.Material))
                    {
                        continue; // is that already built?
                    }

                    var requirementTypes = item.Material.Requirements;

                    if (!requirementTypes.All(i => builtList.Contains(i.Material)))
                    {
                        continue; // Is all prerequisites built?
                    }

                    stageList.Add(item);
                }

                foreach (var item in stageList)
                {
                    builtList.Add(item.Material);
                }
            }

            return stages;
        }
    }
}