namespace Eveindustry.Models
{
    /// <summary>
    /// Information about eve type jita max buy / min sell prices.
    /// </summary>
    public sealed record EvePriceInfo(long TypeId, decimal JitaBuy, decimal JitaSell);
}