namespace Eveindustry.CLI.Models.Config
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

    class EvePricesUdateConfiguration : IEvePricesUdateConfiguration
    {
        public long UpdateIntervalMinutes { get; } = 60;
    }
}