using System.Collections.Generic;
using System.Linq;
using Eveindustry.Sde.Repositories;

namespace Eveindustry.Core
{
    /// <summary>
    /// Implementation of <see cref="ITypeIdsSource"/> which simply returns all existing type ids.
    /// </summary>
    public class AllTypeIdsSource : ITypeIdsSource
    {
        private readonly ISdeDataRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllTypeIdsSource"/> class.
        /// </summary>
        /// <param name="repository">eve types repository to get all information about existed types. </param>
        public AllTypeIdsSource(ISdeDataRepository repository)
        {
            this.repository = repository;
        }

        /// <inheritdoc/>
        public IEnumerable<long> GetTypeIds()
        {
            return this.repository.GetAll().Values.Where(i => i.MarketGroupId != 0).Select(i => i.Id);
        }
    }
}