using AutoMapper;
using Eveindustry.Core.Models;
using Eveindustry.Shared.DTO.EveTypeSearch;

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
        }
    }
}