using System.Collections.Generic;

namespace Eveindustry.Core.Models
{
    /// <summary>
    /// generic type containing id and name. 
    /// </summary>
    public abstract class EveIdNameBase
    {
        /// <summary>
        /// Eve type item id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Eve type item nam. 
        /// </summary>
        public string Name { get; set; }
    }
    
    /// <summary>
    /// All information available for eve item type. 
    /// </summary>
    public class EveType: EveIdNameBase
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

    /// <summary>
    /// Information about type group.
    /// </summary>
    public class EveGroup : EveIdNameBase
    {
        
    }
    /// <summary>
    /// information about type category.
    /// </summary>
    public class EveCategory: EveIdNameBase
    {
    }

    /// <summary>
    /// Information about market group
    /// </summary>
    public class EveMarketGroup : EveIdNameBase
    {
        /// <summary>
        /// Parent market group id.
        /// </summary>
        public long ParentMarketGroupId { get; set; }
    }

    /// <summary>
    /// Information about type blueprint.
    /// </summary>
    public class EveBlueprint : EveIdNameBase
    {
        /// <summary>
        /// type Ids and quantities for type manufacturing or reaction
        /// </summary>
        public List<EveMaterialRequirement> Materials { get; set; }
        
        /// <summary>
        /// manufacturing or reaction result type id.
        /// </summary>
        public long ProducedTypeId { get; set; }

        /// <summary>
        /// Manufacturing or reaction result type items quantity per job run.  
        /// </summary>
        public long ItemsPerRun { get; set; }
    }

    /// <summary>
    /// Information about quantity and material required for blueprint activity.
    /// </summary>
    public class EveMaterialRequirement 
    {
        /// <summary>
        /// Quantity of material required for blueprint activity
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// Type of material required for blueprint activity. 
        /// </summary>
        public long MaterialId { get; set; }
    }
}