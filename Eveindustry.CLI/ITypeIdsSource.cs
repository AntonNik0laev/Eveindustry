using System.Collections.Generic;

namespace Eveindustry.CLI
{
    /// <summary>
    /// Simple interface to consume type ids.
    /// Might be useful to limit type ids to specific items for some of external apis, like evepraisal.
    /// </summary>
    public interface ITypeIdsSource
    {
        /// <summary>
        /// Returns list of type ids.
        /// </summary>
        /// <returns>List of type ids. </returns>
        public IEnumerable<long> GetTypeIds();
    }
}