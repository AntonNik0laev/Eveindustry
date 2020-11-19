using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.StaticDataModels
{
    /// <summary>
    /// Blueprint activity info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    public partial class BlueprintActivity
    {
        /// <summary>
        /// Gets or sets Materials.
        /// </summary>
        [YamlMember(Alias = "materials")]
        [Key(0)]
        public virtual Material[] Materials { get; set; }

        /// <summary>
        /// Gets or sets Skills.
        /// </summary>
        [YamlMember(Alias = "skills")]
        [Key(1)]
        public virtual Skill[] Skills { get; set; }

        /// <summary>
        /// Gets or sets Time.
        /// </summary>
        [YamlMember(Alias = "time")]
        [Key(2)]
        public virtual long Time { get; set; }

        /// <summary>
        /// Gets or sets Products.
        /// </summary>
        [YamlMember(Alias = "products")]
        [Key(3)]
        public virtual Material[] Products { get; set; }
    }
}