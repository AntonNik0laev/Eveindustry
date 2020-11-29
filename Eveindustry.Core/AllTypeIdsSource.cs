using System.Collections.Generic;
using System.Linq;

namespace Eveindustry.Core
{
    /// <summary>
    /// Implementation of <see cref="ITypeIdsSource"/> which simply returns all existing type ids.
    /// </summary>
    public class AllTypeIdsSource : ITypeIdsSource
    {
        private readonly IEveTypeInfoRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllTypeIdsSource"/> class.
        /// </summary>
        /// <param name="repository">eve types repository to get all information about existed types. </param>
        public AllTypeIdsSource(IEveTypeInfoRepository repository)
        {
            this.repository = repository;
        }

        /// <inheritdoc/>
        public IEnumerable<long> GetTypeIds()
        {
            return this.repository.GetAll().Select(i => i.Id);
        }
    }
}