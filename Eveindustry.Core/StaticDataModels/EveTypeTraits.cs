using System.Collections.Generic;
using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core.StaticDataModels
{
    /// <summary>
    /// Eve type traits dto for SDE.
    /// </summary>
    [MessagePackObject]
    public class EveTypeTraits
    {
        /// <summary>
        /// Gets or sets MiscBonuses.
        /// </summary>
        [YamlMember(Alias = "miscBonuses")]
        [Key(0)]
        public virtual EveTypeTraitBonus[] MiscBonuses { get; set; }

        /// <summary>
        /// Gets or sets RoleBonuses.
        /// </summary>
        [YamlMember(Alias = "roleBonuses")]
        [Key(1)]
        public virtual EveTypeTraitBonus[] RoleBonuses { get; set; }

        /// <summary>
        /// Gets or sets Types bonuses.
        /// </summary>
        [YamlMember(Alias = "types")]
        [Key(2)]
        public virtual Dictionary<int, EveTypeTraitBonus[]> Types { get; set; }

        /// <summary>
        /// Gets or sets iconId.
        /// </summary>
        [YamlMember(Alias = "iconID")]
        [Key(3)]
        public virtual int? IconId { get; set; }
    }
}