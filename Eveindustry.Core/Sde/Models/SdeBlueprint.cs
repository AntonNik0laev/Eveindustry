using Eveindustry.Core.Sde.Models;
using MessagePack;

namespace Eveindustry.Core.Sde
{
    [MessagePackObject]
    internal  class SdeBlueprint : SdeNameIdBase
    {
        [Key(2)]
        public SdeMaterialRequirement MaterialRequirements { get; set; }
        [Key(3)]
        public SdeType ProducedType { get; set; }
        [Key(4)]
        public long ItemsPerRun { get; set; }
    }
}