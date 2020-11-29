namespace Eveindustry.CLI.StaticDataModels
{
    /// <inheritdoc />
    public class TypeInfoLoader : EveSdeLoaderBase<EveType>, ITypeInfoLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLoader"/> class.
        /// </summary>
        /// <param name="sdeBasePath">base path to sde data. </param>
        public TypeInfoLoader(string sdeBasePath)
            : base(sdeBasePath)
        {
        }

        /// <inheritdoc/>
        protected override string SdeFileRelativePath => "fsd\\typeIDs.yaml";

        /// <inheritdoc/>
        protected override string CacheFilename => "typeids.bin";
    }
}