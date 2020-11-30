using System.Collections.Generic;
using System.IO;
using Eveindustry.Core.Sde.Utils;

namespace Eveindustry.Core.Sde.Loaders.Basic
{
    /// <summary>
    /// Base class to load eve Static data (SDE).
    /// We assume that all static data is organized as dictionaries with key as item ID and value is item details.
    /// </summary>
    /// <typeparam name="TData">type of item details. </typeparam>
    internal abstract class EveSdeLoaderBase<TData> : IDataLoader<SortedList<long, TData>>
    {
        private readonly string sdeBasePath;
        private SortedList<long, TData> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveSdeLoaderBase{TData}"/> class.
        /// </summary>
        /// <param name="sdeBasePath">path to eve static data export (SDE) root directory. </param>
        protected EveSdeLoaderBase(string sdeBasePath)
        {
            this.sdeBasePath = sdeBasePath;
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

        private string SdeFileFullPath => Path.Join(this.sdeBasePath, this.SdeFileRelativePath);

        /// <inheritdoc/>
        public SortedList<long, TData> Load()
        {
            if (this.data != null)
            {
                return this.data;
            }

            this.data = SerializationUtils
                .ReadAndCacheBinary<SortedList<long, TData>>(this.SdeFileFullPath, this.CacheFilename);

            return this.data;
        }
    }
}