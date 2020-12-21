using AutoMapper;
using Eveindustry.Shared.DTO;
using Eveindustry.Shared.DTO.EveType;

namespace Eveindustry.Shared.Profiles
{
    /// <summary>
    /// Mapping profile to map from <see cref="EveTypeDto"/> to <see cref="Eveindustry.Shared.EveItemManufacturingInfo"/>
    /// </summary>
    public class EveItemManufacturingInfoMappingProfile: Profile
    {
        /// <summary>
        /// Creates mapping profile instance
        /// </summary>
        public EveItemManufacturingInfoMappingProfile()
        {
            CreateMap<EveTypeDto, EveItemManufacturingInfo>()
                .ForMember(i => i.ItemsPerRun, c => c.MapFrom((src,_) => src.Blueprint?.ItemsPerRun ?? 1L ))
                .ForMember(t => t.Requirements, c => c.Ignore())
                .ForMember(d => d.FacilityKind, c => c.Ignore())
                .ForMember(d => d.FacilityRigKind, c => c.Ignore())
                .ForMember(d => d.BlueprintME, c => c.Ignore())
                .ForMember(d => d.BlueprintTE, c=> c.Ignore())
                .ForMember(d => d.SystemName, c=> c.Ignore())
                .ForMember(d => d.RegionKind, c => c.Ignore())
                .ForMember(d => d.ForceBuy, c => c.Ignore());
            CreateMap<ManufacturingParameters, EveItemManufacturingInfo>()
                .ForMember(d => d.FacilityKind, c => c.MapFrom(s => s.FacilityKind))
                .ForMember(d => d.FacilityRigKind, c => c.MapFrom(s => s.FacilityRigKind))
                .ForMember(d => d.BlueprintME, c => c.MapFrom(s => s.BlueprintME))
                .ForMember(d => d.ForceBuy, c => c.MapFrom(s => s.ForceBuy))
                .ForAllOtherMembers(c => c.Ignore());
        }
    }
}