using System.Collections.Generic;
using System.Linq;

namespace Eveindustry.Shared
{
    /// <summary>
    /// Information required for manufacturing eve item.
    /// </summary>
    public partial class EveItemManufacturingInfo
    {
        /// <summary>
        /// Gets or sets Eve type id.
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// Gets or sets type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets number of items per manufacturing job run.
        /// </summary>
        public long ItemsPerRun { get; set; }

        /// <summary>
        /// Gets or sets item manufacturing job material requirements.
        /// </summary>
        public List<EveManufacturialQuantity> Requirements { get; set; }

        /// <summary>
        /// Gets or sets market sell price per item.
        /// </summary>
        public decimal MarketSell { get; set; }

        /// <summary>
        /// Gets or sets market buy price per item.
        /// </summary>
        public decimal MarketBuy { get; set; }

        /// <summary>
        /// Gets or sets item adjusted price
        /// </summary>
        public decimal AdjustedPrice { get; set; }

        /// <summary>
        /// Gets adjusted price of  materials per item.
        /// </summary>
        public decimal MaterialsAdjustedPricePerItem =>
            this.Requirements.Sum(item => item.Material.AdjustedPrice * item.Quantity) /
            this.ItemsPerRun;

        /// <summary>
        /// Gets jita buy price for all required materials per single item.
        /// </summary>
        public decimal MaterialsJitaBuyPricePerItem =>
            this.Requirements.Sum(item => item.TotalJitaBuyPrice) / this.ItemsPerRun;

        /// <summary>
        /// Gets jita sell price for all required materials per single item.
        /// </summary>
        public decimal MaterialsJitaSellPricePerItem =>
            this.Requirements.Sum((item) => item.TotalJitaSellPrice) / this.ItemsPerRun;

        /// <summary>
        /// Gets a value indicating whether item can be manufactured.
        /// </summary>
        public bool CanBeManufactured => this.Requirements.Count > 0;
        
        /// <summary>
        /// Do not compute given item for manufacturing plan, assuming it's bought, not built
        /// </summary>
        public bool ForceBuy { get; set; }
        
        /// <summary>
        /// Gets or sets material efficiency value
        /// </summary>
        public int BlueprintME { get; set; }
        
        /// <summary>
        /// Gets or sets time efficiency value
        /// </summary>
        public int BlueprintTE { get; set; }
        
        /// <summary>
        /// Gets or sets production system name
        /// </summary>
        public string SystemName { get; set; }
        
        /// <summary>
        /// Gets or sets type of region
        /// </summary>
        public RegionKinds RegionKind { get; set; }
        public FacilityKinds FacilityKind { get; set; }
        public FacilityRigKinds FacilityRigKind { get; set; }

        // Equality members

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is EveItemManufacturingInfo other && this.Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        private bool Equals(EveItemManufacturingInfo other)
        {
            return this.Id == other.Id;
        }
    }
}