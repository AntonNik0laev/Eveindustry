using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Eveindustry.Sde.Utils;

namespace Eveindustry.Sde.Loaders.Internal
{
    /// <summary>
    /// Base class to load eve Static data (SDE).
    /// We assume that all static data is organized as dictionaries with key as item ID and value is item details.
    /// </summary>
    /// <typeparam name="TData">type of item details. </typeparam>
    internal abstract class EveSdeLoaderBase<TData> : IDataLoader<SortedList<long, TData>>
    {
        private readonly string options;
        private SortedList<long, TData> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveSdeLoaderBase{TData}"/> class.
        /// </summary>
        /// <param name="options">path to eve static data export (SDE) root directory. </param>
        protected EveSdeLoaderBase(string options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets path to sde file relative to sde root.
        /// </summary>
        protected abstract string SdeFileRelativePath { get; }

        /// <summary>
        /// Gets filename to use for binary serialization cache.
        /// Cache file is stored at main binary folder.
        /// </summary>
        protected abstract string CacheFilename { get; }

        private string SdeFileFullPath => Path.Join(this.options, this.SdeFileRelativePath);

        /// <inheritdoc/>
        public async Task<SortedList<long, TData>> Load()
        {
            return this.items ??= await SerializationUtils
                .ReadAndCacheBinaryAsync<SortedList<long, TData>>(this.SdeFileFullPath, this.CacheFilename);
        }
    }
}