using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core.StaticDataModels
{
    /// <summary>
    /// Eve blueprint activities info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    public partial class BlueprintActivities
    {
        /// <summary>
        /// Gets or sets Copying.
        /// </summary>
        [YamlMember(Alias = "copying")]
        [Key(0)]
        public virtual BlueprintActivity Copying { get; set; }

        /// <summary>
        /// Gets or sets Manufacturing.
        /// </summary>
        [YamlMember(Alias = "manufacturing")]
        [Key(1)]
        public virtual BlueprintActivity Manufacturing { get; set; }

        /// <summary>
        /// Gets or sets ResearchMaterial.
        /// </summary>
        [YamlMember(Alias = "research_material")]
        [Key(2)]
        public virtual BlueprintActivity ResearchMaterial { get; set; }

        /// <summary>
        /// Gets or sets ResearchTime.
        /// </summary>
        [YamlMember(Alias = "research_time")]
        [Key(3)]
        public virtual BlueprintActivity ResearchTime { get; set; }

        /// <summary>
        /// Gets or sets Reaction.
        /// </summary>
        [YamlMember(Alias = "reaction")]
        [Key(4)]
        public virtual BlueprintActivity Reaction { get; set; }

        /// <summary>
        /// Gets or sets Invention.
        /// </summary>
        [YamlMember(Alias = "invention")]
        [Key(5)]
        public virtual BlueprintActivity Invention { get; set; }
    }
}