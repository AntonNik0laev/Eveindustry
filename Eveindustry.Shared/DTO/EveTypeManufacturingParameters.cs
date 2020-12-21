using Eveindustry.Shared.DTO.EveType;

namespace Eveindustry.Shared.DTO
{
    public class ManufacturingParameters
    {
        public int BlueprintME { get; set; }
        public FacilityKinds FacilityKind { get; set; }
        public FacilityRigKinds FacilityRigKind { get; set; }
        public bool ForceBuy { get; set; }
    }
    
    public class EveTypeManufacturingParameters
    {
        public EveTypeDto EveType { get; set; }
        public ManufacturingParameters Parameters { get; set; }
    }
}