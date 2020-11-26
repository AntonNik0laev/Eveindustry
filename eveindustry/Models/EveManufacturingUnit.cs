using System.Linq;

namespace Eveindustry.Models
{
    /// <summary>
    /// Information related to materials required for manufacturing job
    /// </summary>
    public sealed class EveManufacturingUnit
    {
        /// <summary>
        /// Gets or sets eve material.
        /// </summary>
        public EveItemManufacturingInfo Material { get; set; }

        /// <summary>
        /// Gets or sets material quantity.
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// Gets total adjusted price for materials multiplied by quantity.
        /// </summary>
        public decimal MaterialsAdjustedPrice => this.Material.MaterialsAdjustedPricePerItem * this.Quantity;

        /// <summary>
        /// Gets total jita buy price multiplied by quantity.
        /// </summary>
        public decimal TotalJitaBuyPrice => this.Material.PriceBuy * this.Quantity;

        /// <summary>
        /// Gets total jita sell price multiplied by quantity.
        /// </summary>
        public decimal TotalJitaSellPrice => this.Material.PriceSell * this.Quantity;

        /// <summary>
        /// Gets total jita buy price for required materials
        /// </summary>
        public decimal MaterialsJitaBuyPrice => this.Material.MaterialsJitaBuyPricePerItem * this.Quantity;

        /// <summary>
        /// Gets total jita sell price for required materials
        /// </summary>
        public decimal MaterialsJitaSellPrice => this.Material.MaterialsJitaSellPricePerItem * this.Quantity;
    }
}