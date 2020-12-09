using AutoMapper;
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
                .ForMember(t => t.Requirements, c => c.Ignore());
        }
    }
}