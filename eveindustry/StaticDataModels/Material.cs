using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.StaticDataModels
{
    /// <summary>
    /// Eve blueprint material info.
    /// </summary>
    [MessagePackObject]
    public partial class Material
    {
        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [YamlMember(Alias = "quantity")]
        [Key(0)]
        public virtual int Quantity { get; set; }

        /// <summary>
        /// Gets or sets TypeId.
        /// </summary>
        [YamlMember(Alias = "typeID")]
        [Key(1)]
        public virtual int TypeId { get; set; }

        /// <summary>
        /// Gets or sets Probability.
        /// </summary>
        [YamlMember(Alias = "probability")]
        [Key(2)]
        public virtual double Probability { get; set; }
    }
}