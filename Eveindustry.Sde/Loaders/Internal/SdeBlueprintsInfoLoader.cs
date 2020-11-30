using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    /// <inheritdoc cref="ISdeBlueprintsInfoLoader"/>x
    internal class SdeBlueprintsInfoLoader : EveSdeLoaderBase<SdeBlueprintInfo>, ISdeBlueprintsInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdeBlueprintsInfoLoader"/> class.
        /// </summary>
        /// <param name="sdeRoot">path to Eve Static Data Export files (SDE). </param>
        public SdeBlueprintsInfoLoader(TypeInfoLoaderOptions options)
            : base(options.SdeBasePath)
        {
        }

        /// <inheritdoc/>
        protected override string CacheFilename => "blueprints.bin";

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\blueprints.yaml";
    }
}