using System.Collections.Generic;
using MessagePack;

namespace Eveindustry.Sde.Models
{
    [MessagePackObject]
    public  class SdeBlueprint : SdeNameIdBase
    {
        [Key(2)]
        public List<SdeMaterialRequirement> MaterialRequirements { get; set; }
        [Key(3)]
        public long ProducedTypeId { get; set; }
        [Key(4)]
        public long ItemsPerRun { get; set; }
    }
}