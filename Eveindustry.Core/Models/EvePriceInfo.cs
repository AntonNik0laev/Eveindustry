using MessagePack;

namespace Eveindustry.Core.Models
{
    /// <summary>
    /// Information about eve type jita max buy / min sell prices.
    /// </summary>
    [MessagePackObject]
    public sealed record EvePriceInfo(long TypeId, decimal JitaBuy, decimal JitaSell)
    {
        /// <summary>
        /// Gets or sets TypeId
        /// </summary>
        [Key(0)]
        public long TypeId { get; init; } = TypeId;
        /// <summary>
        /// Gets or sets Jita buy price
        /// </summary>
        [Key(1)]
        public decimal JitaBuy { get; init; } = JitaBuy;
        /// <summary>
        /// Gets or sets Jita sell price
        /// </summary>
        [Key(2)]
        public decimal JitaSell { get; init; } = JitaSell;
    }
}