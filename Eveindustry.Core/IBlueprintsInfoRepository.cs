using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.Core
{
    /// <summary>
    /// Provides methods to search from eve blueprints static data export.
    /// </summary>
    public interface IBlueprintsInfoRepository
    {
        /// <summary>
        /// Get <see cref="BlueprintInfo"/> by blueprint id.
        /// </summary>
        /// <param name="blueprintId">blueprint id. </param>
        /// <returns><see cref="BlueprintInfo"/>. </returns>
        BlueprintInfo GetByBluprintId(long blueprintId);

        /// <summary>
        /// Search blueprint info by blueprint product id.
        /// </summary>
        /// <param name="productId">blueprint product id. </param>
        /// <returns> <see cref="BlueprintInfo"/>. </returns>
        BlueprintInfo FindByProductId(long productId);
    }
}