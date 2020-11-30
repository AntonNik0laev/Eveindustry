using Eveindustry.Core.Sde.Loaders.Basic;
using Eveindustry.Core.Sde.Models.Basic;
using Eveindustry.Core.Sde.StaticDataModels;

namespace Eveindustry.Core.Sde.Loaders
{
    /// <inheritdoc cref="ISdeBasicTypeInfoLoader"/>
    internal class SdeBasicTypeInfoLoader : EveSdeLoaderBase<SdeEveBasicType>, ISdeBasicTypeInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdeBasicTypeInfoLoader"/> class.
        /// </summary>
        /// <param name="options">options, containing base path to sde data. </param>
        public SdeBasicTypeInfoLoader(TypeInfoLoaderOptions options)
            : base(options.SdeBasePath)
        {
        }

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\typeIDs.yaml";

        /// <inheritdoc/>
        protected override string CacheFilename => "typeids.bin";
    }
}