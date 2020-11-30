using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Sde.Models.Internal
{
    /// <summary>
    /// Eve blueprint material info.
    /// </summary>
    [MessagePackObject]
    internal partial class Material
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
        public virtual long TypeId { get; set; }

        /// <summary>
        /// Gets or sets Probability.
        /// </summary>
        [YamlMember(Alias = "probability")]
        [Key(2)]
        public virtual double Probability { get; set; }
    }
}