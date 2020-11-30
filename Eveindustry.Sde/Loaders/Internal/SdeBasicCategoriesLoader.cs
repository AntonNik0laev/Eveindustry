using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Models.Internal;

namespace Eveindustry.Sde.Loaders.Internal
{
    internal class SdeBasicCategoriesLoader : EveSdeLoaderBase<SdeBasicCategory>, ISdeBasicCategoriesLoader
    {
        public SdeBasicCategoriesLoader(TypeInfoLoaderOptions options) : base(options.SdeBasePath)
        {
            
        }

        protected override string SdeFileRelativePath => "fsd/categoryIDs.yaml";
        protected override string CacheFilename => "categories.bin";
    }
}