using System;
using System.Collections.Generic;
using System.Linq;

namespace Eveindustry.Core.Models
{
    /// <summary>
    /// Information required for manufacturing eve item.
    /// </summary>
    public sealed class EveItemManufacturingInfo
    {
        /// <summary>
        /// Manufacturing activity kinds - Manufacturing or Reaction.
        /// </summary>
        public enum ActivityKinds
        {
            /// <summary>
            /// Manufacturing activity
            /// </summary>
            Manufacturing,

            /// <summary>
            /// Reaction activity
            /// </summary>
            Reaction,
        }

        /// <summary>
        /// Gets or sets Eve type id.
        /// </summary>
        public long TypeId { get; init; }

        /// <summary>
        /// Gets or sets type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets number of items per manufacturing job run.
        /// </summary>
        public int ItemsPerRun { get; set; }

        /// <summary>
        /// Gets or sets item manufacturing job material requirements.
        /// </summary>
        public List<EveManufacturingUnit> Requirements { get; set; }

        /// <summary>
        /// Gets or sets market sell price per item.
        /// </summary>
        public decimal PriceSell { get; set; }

        /// <summary>
        /// Gets or sets market buy price per item.
        /// </summary>
        public decimal PriceBuy { get; set; }

        /// <summary>
        /// Gets or sets item adjusted price
        /// </summary>
        public decimal AdjustedPrice { get; set; }

        /// <summary>
        /// Gets adjusted price of  materials per item.
        /// </summary>
        public decimal MaterialsAdjustedPricePerItem =>
            this.AggregateRequirements((sum, item) => sum += item.Material.AdjustedPrice * item.Quantity) /
            this.ItemsPerRun;

        /// <summary>
        /// Gets jita buy price for all required materials per single item.
        /// </summary>
        public decimal MaterialsJitaBuyPricePerItem =>
            this.AggregateRequirements((sum, item) => sum += item.TotalJitaBuyPrice) / this.ItemsPerRun;

        /// <summary>
        /// Gets jita sell price for all required materials per single item.
        /// </summary>
        public decimal MaterialsJitaSellPricePerItem =>
            this.AggregateRequirements((sum, item) => sum += item.TotalJitaSellPrice) / this.ItemsPerRun;

        /// <summary>
        /// Gets a value indicating whether item can be manufactured.
        /// </summary>
        public bool CanBeManufactured => this.Requirements.Count > 0;

        // Equality members

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is EveItemManufacturingInfo other && this.Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.TypeId.GetHashCode();
        }

        private bool Equals(EveItemManufacturingInfo other)
        {
            return this.TypeId == other.TypeId;
        }

        private decimal AggregateRequirements(Func<decimal, EveManufacturingUnit, decimal> aggregate) =>
            this.Requirements.Aggregate(0, aggregate);
    }
}