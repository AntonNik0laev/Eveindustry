using System;
using System.Collections.Generic;
using Eveindustry.CLI;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Core.StaticDataModels;
using Eveindustry.Tests.Utils;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;

namespace Eveindustry.Tests
{
    public class BlueprintsRepositoryTests
    {
        [Test,Description("FindByProductId returns first type which can be produced with manufacturing")]
        public void find_by_product_id_returns_first_manufactured_product()
        {
            var type = DataHelper.NewType(1, "Test Type");
            var bpData1 = DataHelper.NewBp(100, type, 1, Array.Empty<(EveType Type, int Quantity)>());


            var loaderMock = new Mock<IBlueprintsInfoLoader>();
            loaderMock.Setup(x => x.Load()).Returns(new Dictionary<string, BlueprintInfo>() {{"100", bpData1}});
            var bpRepository = new BlueprintsInfoRepository(loaderMock.Object);

            var bp = bpRepository.FindByProductId(1);
            bp.Should().NotBeNull("Returned blueprint information should exist and cannot be null");
            bp.Should().BeEquivalentTo(bpData1, "Returned blueprintObjectShould be same as loaded");
        }
        
        [Test,Description("FindByProductId returns first type which can be produced with manufacturing")]
        public void find_by_product_id_returns_first_reaction_product()
        {
            var type = DataHelper.NewType(1, "Test Type");
            var bpData1 = DataHelper.NewBp(100, type, 1, Array.Empty<(EveType Type, int Quantity)>(), EveItemManufacturingInfo.ActivityKinds.Reaction);


            var loaderMock = new Mock<IBlueprintsInfoLoader>();
            loaderMock.Setup(x => x.Load()).Returns(new Dictionary<string, BlueprintInfo>() {{"100", bpData1}});
            var bpRepository = new BlueprintsInfoRepository(loaderMock.Object);

            var bp = bpRepository.FindByProductId(1);
            bp.Should().NotBeNull("Returned blueprint information should exist and cannot be null");
            bp.Should().BeEquivalentTo(bpData1, "Returned blueprintObjectShould be same as loaded");
        }
        
        [Test,Description("FindByProductId returns first type which can be produced with manufacturing")]
        public void get_by_id_returns_blueprint_when_exists()
        {
            var type = DataHelper.NewType(1, "Test Type");
            var bpData1 = DataHelper.NewBp(100, type, 1, Array.Empty<(EveType Type, int Quantity)>());


            var loaderMock = new Mock<IBlueprintsInfoLoader>();
            loaderMock.Setup(x => x.Load()).Returns(new Dictionary<string, BlueprintInfo>() {{"100", bpData1}});
            var bpRepository = new BlueprintsInfoRepository(loaderMock.Object);

            var bp = bpRepository.GetByBluprintId(100);
            bp.Should().NotBeNull("Returned blueprint information should exist and cannot be null");
            bp.Should().BeEquivalentTo(bpData1, "Returned blueprintObjectShould be same as loaded");
        }
    }
}