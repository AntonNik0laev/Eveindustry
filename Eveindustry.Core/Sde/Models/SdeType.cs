using Eveindustry.Core.Sde.Models;
using MessagePack;

namespace Eveindustry.Core.Sde
{
    [MessagePackObject]
    internal class SdeType : SdeNameIdBase
    {
        [Key(2)]
        private SdeGroup Group { get; set; }
        [Key(3)]
        public SdeCategory Category { get; set; }
        [Key(4)]
        public SdeBlueprint Blueprint { get; set; }
    }
}