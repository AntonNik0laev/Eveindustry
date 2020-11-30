using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Sde.Models.Internal
{
    [MessagePackObject]
    internal class SdeBasicGroup
    {
        [Key(0)]
        [YamlMember(Alias = "anchorable")]
        public bool Anchorable { get; set; }
        [Key(1)]
        [YamlMember(Alias = "anchored")]
        public bool Anchored { get; set; }
        [Key(2)]
        [YamlMember(Alias = "categoryID")]
        public long CategoryId { get; set; }
        [Key(3)]
        [YamlMember(Alias = "fittableNonSingleton")]
        public bool FittableNonSingleton { get; set; }
        [Key(4)]
        [YamlMember(Alias = "name")]
        public LocalizedText Name { get; set; }
        [Key(5)]
        [YamlMember(Alias = "published")]
        public bool Published { get; set; }
        [Key(6)]
        [YamlMember(Alias = "useBasePrice")]
        public bool UseBasePrice { get; set; }
        [Key(7)]
        [YamlMember(Alias = "iconID")]
        public long IconId { get; set; }
    }
}