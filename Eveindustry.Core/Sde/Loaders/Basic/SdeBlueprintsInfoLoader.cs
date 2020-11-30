using Eveindustry.Core.Sde.Models.Basic;

namespace Eveindustry.Core.Sde.Loaders.Basic
{
    /// <inheritdoc cref="ISdeBlueprintsInfoLoader"/>x
    internal class SdeBlueprintsInfoLoader : EveSdeLoaderBase<SdeBlueprintInfo>, ISdeBlueprintsInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdeBlueprintsInfoLoader"/> class.
        /// </summary>
        /// <param name="sdeRoot">path to Eve Static Data Export files (SDE). </param>
        public SdeBlueprintsInfoLoader(string sdeRoot)
            : base(sdeRoot)
        {
        }

        /// <inheritdoc/>
        protected override string CacheFilename => "blueprints.bin";

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\blueprints.yaml";
    }
}