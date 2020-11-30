using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Sde.Models.Internal
{
    [MessagePackObject]
    internal class SdeBasicCategory
    {
        [YamlMember(Alias = "name")]
        [Key(0)]
        public LocalizedText Name { get; set; }
        [YamlMember(Alias = "published")]
        [Key(1)]
        public bool Published { get; set; }
        [YamlMember(Alias = "iconID")]
        [Key(2)]
        public long IconId { get; set; }
    }
}