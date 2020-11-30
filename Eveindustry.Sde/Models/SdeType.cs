using MessagePack;

namespace Eveindustry.Sde.Models
{
    [MessagePackObject]
    public class SdeType : SdeNameIdBase
    {
        [Key(2)]
        public SdeGroup Group { get; set; }
        [Key(3)]
        public SdeCategory Category { get; set; }
        [Key(4)]
        public SdeBlueprint Blueprint { get; set; }
        [Key(5)]
        public long MarketGroupId { get; set; }
    }
}