using AutoMapper;
using Eveindustry.API.DTO.EveTypeSearch;
using Eveindustry.Core.Models;

namespace Eveindustry.API
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<EveType, EveTypeSearchInfo>();
            CreateMap<EveTypeSearchOptions, FindByPartialNameOptions>();
        }
    }
}