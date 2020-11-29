using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core.StaticDataModels
{
    /// <summary>
    /// Eve type trait bonus info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    public class EveTypeTraitBonus
    {
        /// <summary>
        /// Gets or sets Bonus.
        /// </summary>
        [YamlMember(Alias = "bonus")]
        [Key(0)]
        public virtual double Bonus { get; set; }

        /// <summary>
        /// Gets or sets BonusText.
        /// </summary>
        [YamlMember(Alias = "bonusText")]
        [Key(1)]
        public virtual LocalizedText BonusText { get; set; }

        /// <summary>
        /// Gets or sets UnitId.
        /// </summary>
        [YamlMember(Alias = "unitID")]
        [Key(2)]
        public virtual int UnitId { get; set; }

        /// <summary>
        /// Gets or sets Importance.
        /// </summary>
        [YamlMember(Alias = "importance")]
        [Key(3)]
        public virtual double Importance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bonus is positive or not.
        /// </summary>
        [YamlMember(Alias = "isPositive")]
        [Key(4)]
        public virtual bool? IsPositive { get; set; }
    }
}