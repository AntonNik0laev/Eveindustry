using Eveindustry.Core.Sde.StaticDataModels;

namespace Eveindustry.Core.Sde.Loaders
{
    /// <summary>
    /// Provides methods to search from eve blueprints static data export.
    /// </summary>
    internal interface ISdeBlueprintsInfoRepository
    {
        /// <summary>
        /// Get <see cref="SdeBlueprintInfo"/> by blueprint id.
        /// </summary>
        /// <param name="blueprintId">blueprint id. </param>
        /// <returns><see cref="SdeBlueprintInfo"/>. </returns>
        SdeBlueprintInfo GetByBluprintId(long blueprintId);

        /// <summary>
        /// Search blueprint info by blueprint product id.
        /// </summary>
        /// <param name="productId">blueprint product id. </param>
        /// <returns> <see cref="SdeBlueprintInfo"/>. </returns>
        SdeBlueprintInfo FindByProductId(long productId);
    }
}