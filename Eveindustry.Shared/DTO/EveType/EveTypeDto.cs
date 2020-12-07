namespace Eveindustry.Shared.DTO.EveType
{
    /// <summary>
    /// All information available for eve item type. 
    /// </summary>
    public class EveTypeDto: EveIdNameBase
    {
        /// <summary>
        /// Item group.
        /// </summary>
        public EveGroup Group { get; set; }
        /// <summary>
        /// Item group category.
        /// </summary>
        public EveCategory Category { get; set; }
        /// <summary>
        /// Item market group id.
        /// </summary>
        public long MarketGroupId { get; set; }
        /// <summary>
        /// Market buy price in selected location.
        /// </summary>
        public decimal MarketBuy { get; set; }
        /// <summary>
        /// Market sell price in selected location. 
        /// </summary>
        public decimal MarketSell { get; set; }
        /// <summary>
        /// Adjusted price according to eve api.
        /// </summary>
        public decimal AdjustedPrice { get; set; }
        /// <summary>
        /// Eve type blueprint information. Null if there is no blueprint.
        /// </summary>
        public EveBlueprint Blueprint { get; set; }
    }
}