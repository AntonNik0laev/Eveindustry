namespace Eveindustry.Shared
{
    /// <summary>
    /// Type of region, Null, High or Low
    /// </summary>
    public enum RegionKinds
    {
        /// <summary>
        /// High Security region (0.5-1.0)
        /// </summary>
        HighSec,
        /// <summary>
        /// Low security region (0.1-0.4)
        /// </summary>
        LowSec,
        /// <summary>
        /// Null security region (-1 - 0.0)
        /// </summary>
        NullSec
    }
}