using System.Threading.Tasks;
using Eveindustry.Core.Models;

namespace Eveindustry.Core
{
    /// <summary>
    /// Methods to collect ESI adjusted/average prices.
    /// </summary>
    public interface IEsiPricesRepository
    {
        /// <summary>
        /// Initial load esi prices data.
        /// </summary>
        Task Init();

        /// <summary>
        /// Get adjusted / average prices info for given type.
        /// </summary>
        /// <param name="typeId">type to get prices info. </param>
        /// <returns>prices info. </returns>
        ESIPriceData GetAdjustedPriceInfo(long typeId);
    }
}