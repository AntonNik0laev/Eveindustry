using System.Threading.Tasks;
using Eveindustry.CLI.Models;

namespace Eveindustry.CLI
{
    /// <summary>
    /// Methods to get information for eve items current market prices.
    /// Might update it's data periodically.
    /// </summary>
    public interface IEvePricesRepository
    {
        /// <summary>
        /// Initial fetch eve item prices. may take some time.
        /// </summary>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation. </returns>
        Task Init();

        /// <summary>
        /// Return current type prices information.
        /// Data updates each 30 minutes.
        /// </summary>
        /// <param name="typeId">item type id to search price. </param>
        /// <returns>current jita prices for a type. </returns>
        EvePriceInfo GetPriceInfo(long typeId);
    }
}