using System.Collections.Generic;

namespace Eveindustry.Core
{
    /// <summary>
    /// Simple implementation of <see cref="ITypeIdsSource"/> to return specific list.
    /// </summary>
    public class ListTypeIdsSource : ITypeIdsSource
    {
        private readonly IEnumerable<long> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTypeIdsSource"/> class.
        /// </summary>
        /// <param name="items">type ids to return. </param>
        public ListTypeIdsSource(IEnumerable<long> items)
        {
            this.items = items;
        }

        /// <inheritdoc />
        public IEnumerable<long> GetTypeIds()
        {
            return this.items;
        }
    }
}