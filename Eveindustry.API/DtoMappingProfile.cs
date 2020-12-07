using AutoMapper;
using Eveindustry.Core.Models;
using Eveindustry.Shared.DTO.EveType;
using Eveindustry.Shared.DTO.EveTypeSearch;
using EveBlueprint = Eveindustry.Core.Models.EveBlueprint;
using EveCategory = Eveindustry.Core.Models.EveCategory;
using EveGroup = Eveindustry.Core.Models.EveGroup;
using EveMarketGroup = Eveindustry.Core.Models.EveMarketGroup;
using EveMaterialRequirement = Eveindustry.Core.Models.EveMaterialRequirement;

namespace Eveindustry.API
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<EveType, EveTypeSearchInfo>()
                .ForMember(d => d.Category, c => c.MapFrom(s => s.Category.Name))
                .ForMember(d => d.Group, c => c.MapFrom(s => s.Group.Name));
            
            CreateMap<EveTypeSearchOptions, FindByPartialNameOptions>();

            CreateMap<EveType, EveTypeDto>();
            CreateMap<EveBlueprint, Eveindustry.Shared.DTO.EveType.EveBlueprint>();
            CreateMap<EveCategory, Eveindustry.Shared.DTO.EveType.EveCategory>();
            CreateMap<EveGroup, Eveindustry.Shared.DTO.EveType.EveGroup>();
            CreateMap<EveMarketGroup, Eveindustry.Shared.DTO.EveType.EveMarketGroup>();
            CreateMap<EveMaterialRequirement, Eveindustry.Shared.DTO.EveType.EveMaterialRequirement>();
        }
    }
}