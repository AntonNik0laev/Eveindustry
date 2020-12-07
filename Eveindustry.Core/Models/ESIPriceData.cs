using System.Text.Json.Serialization;

namespace Eveindustry.Core.Models
{
    /// <summary>
    /// ESI model for price data.
    /// </summary>
    public class ESIPriceData
    {
        /// <summary>
        /// Gets or sets esi adjusted price.
        /// </summary>
        [JsonPropertyName("adjusted_price")]
        public decimal AdjustedPrice { get; set; }

        /// <summary>
        /// Gets or sets esi average price.
        /// </summary>
        [JsonPropertyName("average_price")]
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Gets or sets type id.
        /// </summary>
        [JsonPropertyName("type_id")]
        public long TypeId { get; set; }
    }
}