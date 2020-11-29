namespace Eveindustry.Core.StaticDataModels
{
    /// <inheritdoc cref="ITypeInfoLoader"/>
    public class TypeInfoLoader : EveSdeLoaderBase<EveType>, ITypeInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLoader"/> class.
        /// </summary>
        /// <param name="options">options, containing base path to sde data. </param>
        public TypeInfoLoader(TypeInfoLoaderOptions options)
            : base(options.SdeBasePath)
        {
        }

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\typeIDs.yaml";

        /// <inheritdoc/>
        protected override string CacheFilename => "typeids.bin";
    }
}