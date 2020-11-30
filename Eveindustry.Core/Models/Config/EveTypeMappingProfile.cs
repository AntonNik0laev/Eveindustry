using AutoMapper;
using Eveindustry.Sde.Models;

namespace Eveindustry.Core.Models.Config
{
    
    /// <summary>
    /// Mapping profile configuration to map from sde, esi prices and evemarketer prices to evetype.
    /// </summary>
    public class EveTypeMappingProfile: Profile
    {
        /// <summary>
        /// Creates new instance of EveTypeMappingProfile
        /// </summary>
        public EveTypeMappingProfile()
        {
            CreateMap<SdeCategory, EveCategory>();
            CreateMap<SdeGroup, EveGroup>();
            CreateMap<SdeMaterialRequirement, EveMaterialRequirement>();
            CreateMap<SdeBlueprint, EveBlueprint>().ForMember(m => m.Materials, c => c.MapFrom(s => s.MaterialRequirements));

            CreateMap<SdeType, EveType>();
            
            CreateMap<ESIPriceData, EveType>()
                .ForMember(m => m.AdjustedPrice, c => c.MapFrom(s => s.AdjustedPrice))
                .ForAllOtherMembers(c => c.Ignore());
            CreateMap<EvePriceInfo, EveType>()
                .ForMember(m => m.MarketBuy, c => c.MapFrom(s => s.JitaBuy))
                .ForMember(m => m.MarketSell, c => c.MapFrom(s => s.JitaSell));
            

        }
    }
}