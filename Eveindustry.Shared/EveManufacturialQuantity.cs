namespace Eveindustry.Shared
{
    /// <summary>
    /// Information related to materials required for manufacturing job
    /// </summary>
    public sealed class EveManufacturialQuantity
    {
        /// <summary>
        /// Gets or sets eve material.
        /// </summary>
        public EveItemManufacturingInfo Material { get; set; }

        /// <summary>
        /// Gets or sets material quantity to build.
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// Gets or sets material remaining quantity after build.
        /// </summary>
        public long RemainingQuantity { get; set; }

        /// <summary>
        /// Gets total adjusted price for materials multiplied by quantity.
        /// </summary>
        public decimal MaterialsAdjustedPrice => this.Material.MaterialsAdjustedPricePerItem * this.Quantity;

        /// <summary>
        /// Gets total jita buy price multiplied by quantity.
        /// </summary>
        public decimal TotalJitaBuyPrice => this.Material.MarketBuy * this.Quantity;

        /// <summary>
        /// Gets total jita sell price multiplied by quantity.
        /// </summary>
        public decimal TotalJitaSellPrice => this.Material.MarketSell * this.Quantity;

        /// <summary>
        /// Gets jita buy for remaining items
        /// </summary>
        public decimal RemainingJitaBuyPrice => this.Material.MarketBuy * this.RemainingQuantity;

        /// <summary>
        /// Gets jita sell for remaining items
        /// </summary>
        public decimal RemainingJitaSellPrice => this.Material.MarketSell * this.RemainingQuantity;

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