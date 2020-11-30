using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal.Basic
{
    internal class SdeBasicGroupsLoader: EveSdeLoaderBase<SdeBasicGroup>, ISdeBasicGroupsLoader
    {
        public SdeBasicGroupsLoader(TypeInfoLoaderOptions options) : base(options.SdeBasePath) {}

        protected override string SdeFileRelativePath => "fsd/groupIDs.yaml";
        protected override string CacheFilename => "groupids.bin";
    }
}