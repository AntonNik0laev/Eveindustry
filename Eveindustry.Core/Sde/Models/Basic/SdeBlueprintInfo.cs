using Eveindustry.Core.Sde.Models.Basic;
using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core.Sde.StaticDataModels
{
    /// <summary>
    /// Blueprint info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    internal partial class SdeBlueprintInfo
    {
        /// <summary>
        /// Gets or sets blueprint id.
        /// </summary>
        [IgnoreMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Activities.
        /// </summary>
        [YamlMember(Alias = "activities")]
        [Key(0)]
        public virtual BlueprintActivities Activities { get; set; }

        /// <summary>
        /// Gets or sets BlueprintTypeId.
        /// </summary>
        [YamlMember(Alias = "blueprintTypeID")]
        [Key(1)]
        public virtual long BlueprintTypeId { get; set; }

        /// <summary>
        /// Gets or sets MaxProductionLimit.
        /// </summary>
        [YamlMember(Alias = "maxProductionLimit")]
        [Key(2)]
        public virtual long MaxProductionLimit { get; set; }
    }
}