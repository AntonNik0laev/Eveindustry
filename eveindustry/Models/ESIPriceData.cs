using Newtonsoft.Json;

namespace Eveindustry.Models
{
    /// <summary>
    /// ESI model for price data.
    /// </summary>
    public class ESIPriceData
    {
        /// <summary>
        /// Gets or sets esi adjusted price.
        /// </summary>
        [JsonProperty("adjusted_price")]
        public decimal AdjustedPrice { get; set; }

        /// <summary>
        /// Gets or sets esi average price.
        /// </summary>
        [JsonProperty("average_price")]
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Gets or sets type id.
        /// </summary>
        [JsonProperty("type_id")]
        public int TypeId { get; set; }
    }
}