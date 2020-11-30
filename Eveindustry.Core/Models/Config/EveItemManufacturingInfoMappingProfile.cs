using AutoMapper;

namespace Eveindustry.Core.Models.Config
{
    /// <summary>
    /// Mapping profile to map from <see cref="EveType"/> to <see cref="EveItemManufacturingInfo"/>
    /// </summary>
    public class EveItemManufacturingInfoMappingProfile: Profile
    {
        /// <summary>
        /// Creates mapping profile instance
        /// </summary>
        public EveItemManufacturingInfoMappingProfile()
        {
            /*
             *                 Name = eveTypeInfo.Name,
                Requirements = new List<EveManufacturialQuantity>(),
                AdjustedPrice = eveTypeInfo.AdjustedPrice,
                MarketBuy = eveTypeInfo.MarketBuy,
                MarketSell = eveTypeInfo.MarketSell,
                TypeId = id,
                ItemsPerRun = itemsPerRun ?? 0L
             */
            CreateMap<EveType, EveItemManufacturingInfo>()
                .ForMember(i => i.ItemsPerRun, c => c.MapFrom((src,_) => src.Blueprint?.ItemsPerRun ?? 1L ))
                .ForMember(t => t.Requirements, c => c.Ignore());
        }
    }
}