using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.CLI.StaticDataModels
{
    /// <summary>
    /// Eve localized text info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    public class LocalizedText
    {
        /// <summary>
        /// Gets or sets text in german.
        /// </summary>
        [YamlMember(Alias = "de")]
        [Key(0)]
        public virtual string De { get; set; }

        /// <summary>
        /// Gets or sets text in english.
        /// </summary>
        [YamlMember(Alias = "en")]
        [Key(1)]
        public virtual string En { get; set; }

        /// <summary>
        /// Gets or sets text in french.
        /// </summary>
        [YamlMember(Alias = "fr")]
        [Key(2)]
        public virtual string Fr { get; set; }

        /// <summary>
        /// Gets or sets text in japanese.
        /// </summary>
        [YamlMember(Alias = "ja")]
        [Key(3)]
        public virtual string Ya { get; set; }

        /// <summary>
        /// Gets or sets text in russian.
        /// </summary>
        [YamlMember(Alias = "ru")]
        [Key(4)]
        public virtual string Ru { get; set; }

        /// <summary>
        /// Gets or sets text in zh?.
        /// </summary>
        [YamlMember(Alias = "zh")]
        [Key(5)]
        public virtual string Zh { get; set; }

        /// <summary>
        /// Gets or sets text in spain.
        /// </summary>
        [YamlMember(Alias = "es")]
        [Key(6)]
        public virtual string Es { get; set; }

        /// <summary>
        /// Gets or sets text in italian.
        /// </summary>
        [YamlMember(Alias = "it")]
        [Key(7)]
        public virtual string It { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.En.ToString();
        }
    }
}