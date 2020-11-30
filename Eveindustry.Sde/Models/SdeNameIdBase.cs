using MessagePack;

namespace Eveindustry.Sde.Models
{
    [MessagePackObject]
    public abstract class SdeNameIdBase
    {
        [Key(0)]
        public long Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
    }
}