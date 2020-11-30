using MessagePack;

namespace Eveindustry.Sde.Models
{
    [MessagePackObject]
    public class SdeMaterialRequirement
    {
        [Key(0)]
        public long MaterialId { get; set; }
        [Key(1)]
        public long Quantity { get; set; }
    }
}