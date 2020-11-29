namespace Eveindustry.Core.Models.Config
{
    /// <summary>
    /// Eve prices update configuration.
    /// </summary>
    public interface IEvePricesUdateConfiguration
    {
        /// <summary>
        /// Eve prices update interval.
        /// </summary>
        long UpdateIntervalMinutes { get; }
    }

    /// <inheritdoc />
    public class EvePricesUdateConfiguration : IEvePricesUdateConfiguration
    {
        /// <inheritdoc />
        public long UpdateIntervalMinutes { get; } = 60;
    }
}