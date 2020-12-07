using AutoMapper;
using Eveindustry.API;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using Eveindustry.Shared.DTO.EveType;
using NUnit.Framework;

namespace Eveindustry.Tests
{
    public class MappingConfigurationTests
    {
        [Test]
        public void assert_mapping_configuration_is_valid()
        {
            var autoMapperConfig = new MapperConfiguration(c =>
            {
                c.AddMaps(typeof(EveTypeDto).Assembly, typeof(EveType).Assembly);
            });
            autoMapperConfig.AssertConfigurationIsValid();
        }
    }
}