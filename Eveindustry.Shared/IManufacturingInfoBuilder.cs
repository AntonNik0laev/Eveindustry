using System.Collections.Generic;
using Eveindustry.Shared.DTO.EveType;

namespace Eveindustry.Shared
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
        /// <param name="types"></param>
        /// <returns>manufacturing details information. </returns>
        EveItemManufacturingInfo BuildInfo(long id, IDictionary<long, EveTypeDto> types);

        /// <summary>
        /// Build manufacturing info details for list of ids. See <see cref="BuildInfo(long,System.Collections.Generic.IDictionary{long,Eveindustry.Shared.DTO.EveType.EveTypeDto})"/>
        /// </summary>
        /// <param name="typeIds">list of type ids to get detailed information. </param>
        /// <param name="types"></param>
        /// <returns>manufacturing details information. </returns>
        SortedList<long, EveItemManufacturingInfo> BuildInfo(IEnumerable<long> typeIds,
            IDictionary<long, EveTypeDto> types);

        /// <summary>
        /// Given item manufacturing info with all it's dependencies,
        /// build flat total list of items required at all stages.
        /// </summary>
        /// <param name="info">root item. </param>
        /// <param name="quantity">root item quantity required. </param>
        /// <param name="ignoreTypeIds">list of ids to ignore. </param>
        /// <returns>flat list with all required items to build given item. </returns>
        IEnumerable<EveManufacturialQuantity> GetFlatManufacturingList(
            EveItemManufacturingInfo info,
            long quantity,
            IEnumerable<long> ignoreTypeIds = null);

        /// <summary>
        /// Group flat list of items so that each 'level' contains elements produced on previous level.
        /// </summary>
        /// <param name="flatList">flat list of items required. </param>
        /// <param name="ignoredTypeIds">list of ids considered as 'raw' types, so it's dependencies will not be included in stages. </param>
        /// <returns>grouped list. </returns>
        IEnumerable<IEnumerable<EveManufacturialQuantity>> GroupIntoStages(
            IEnumerable<EveManufacturialQuantity> flatList, IEnumerable<long> ignoredTypeIds);
    }
}