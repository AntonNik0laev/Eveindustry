using Eveindustry.Core.Models.Config;
using Eveindustry.Sde.Models.Config;

namespace Eveindustry.API.Config
{
    public class AppConfiguration
    {
        public EvePricesUdateConfiguration EvePricesUdateConfiguration { get; set; }
        public TypeInfoLoaderOptions TypeInfoLoaderOptions { get; set; }
    }
}