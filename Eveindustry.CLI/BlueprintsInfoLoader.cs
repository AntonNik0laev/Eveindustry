using Eveindustry.CLI.StaticDataModels;

namespace Eveindustry.CLI
{
    /// <inheritdoc cref="IBlueprintsInfoLoader"/>x
    public class BlueprintsInfoLoader : EveSdeLoaderBase<BlueprintInfo>, IBlueprintsInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlueprintsInfoLoader"/> class.
        /// </summary>
        /// <param name="sdeRoot">path to Eve Static Data Export files (SDE). </param>
        public BlueprintsInfoLoader(string sdeRoot)
            : base(sdeRoot)
        {
        }

        /// <inheritdoc/>
        protected override string CacheFilename => "blueprints.bin";

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\blueprints.yaml";
    }
}