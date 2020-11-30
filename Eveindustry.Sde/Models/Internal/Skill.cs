using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Sde.Models.Internal
{
    /// <summary>
    /// Blueprint skill information DTO for SDE.
    /// </summary>
    [MessagePackObject]
    internal partial class Skill
    {
        /// <summary>
        /// Gets or sets skill level.
        /// </summary>
        [YamlMember(Alias = "level")]
        [Key(0)]
        public virtual long Level { get; set; }

        /// <summary>
        /// Gets or sets skill type id.
        /// </summary>
        [YamlMember(Alias = "typeID")]
        [Key(1)]
        public virtual long TypeId { get; set; }
    }
}