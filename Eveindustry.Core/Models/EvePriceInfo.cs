namespace Eveindustry.Core.Models
{
    /// <summary>
    /// Information about eve type jita max buy / min sell prices.
    /// </summary>
    public sealed record EvePriceInfo(long TypeId, decimal JitaBuy, decimal JitaSell)
    {
        /// <summary>
        /// Gets or sets TypeId
        /// </summary>
        public long TypeId { get; init; } = TypeId;
        /// <summary>
        /// Gets or sets Jita buy price
        /// </summary>
        public decimal JitaBuy { get; init; } = JitaBuy;
        /// <summary>
        /// Gets or sets Jita sell price
        /// </summary>
        public decimal JitaSell { get; init; } = JitaSell;
    }
}