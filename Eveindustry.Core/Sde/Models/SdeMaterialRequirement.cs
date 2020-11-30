using MessagePack;

namespace Eveindustry.Core.Sde
{
    [MessagePackObject]
    internal class SdeMaterialRequirement
    {
        [Key(0)]
        public SdeType SdeType { get; set; }
        [Key(1)]
        public long Quantity { get; set; }
    }
}