using AutoMapper;
using Eveindustry.API.DTO.EveTypeSearch;
using Eveindustry.Core.Models;
using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.API
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<EveType, EveTypeSearchInfo>()
                .ForMember(i => i.EveTypeId, expression => expression.MapFrom(s => s.Id))
                .ForMember(i => i.EveTypeName, expression => expression.MapFrom(i => i.Name.En));
            CreateMap<EveTypeSearchOptions, FindByPartialNameOptions>();
        }
    }
}