using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eveindustry.Models;
using Eveindustry.StaticDataModels;
using Eveindustry.Tests.Utils;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Eveindustry.Tests
{
    class ManufacturingBuilderTests
    {
        [Test]
        public void build_info_returns_non_empty_results()
        {
            var etmain = DataHelper.NewType(1, "Main"); //Root item
            var etSub2 = DataHelper.NewType(2, "Sub2"); //item in ignore list with dependencies
            var etsub3 = DataHelper.NewType(3, "Sub3"); //item in ignore list without dependencies
            var etsub4 = DataHelper.NewType(4, "Sub4"); //item with dependencies
            var etsub5 = DataHelper.NewType(5, "Sub5"); //item without dependencies
            var etsub6 = DataHelper.NewType(6, "Sub6"); //Dependency of 2
            var etsub7 = DataHelper.NewType(7, "Sub7"); //Dependency of 4
            var etsub8 = DataHelper.NewType(8, "Sub8"); //Dependency of 7, termination

            var bpMain = DataHelper.NewBp(100, etmain, 1, new[] {(etSub2, 10), (etsub3, 20), (etsub4, 30), (etsub5, 40)});
            var bpsub2 = DataHelper.NewBp(200, etSub2, 1, new[] {(etsub6, 50)});
            var bpSub4 = DataHelper.NewBp(300, etsub4, 1, new[] {(etsub7, 60)});
            var bpSub7 = DataHelper.NewBp(400, etsub7, 1, new[] {(etsub8, 70)});
            
            
            var evePricesMock = new Mock<IEvePricesRepository>();
            var esiPricesMock = new Mock<IEsiPricesRepository>();
            var tiRepoMock = new Mock<IEveTypeInfoRepository>();
            var bpRepoMock = new Mock<IBlueprintsInfoRepository>();
            
            tiRepoMock.Setup(x => x.GetById(1)).Returns(etmain);
            tiRepoMock.Setup(x => x.GetById(2)).Returns(etSub2);
            tiRepoMock.Setup(x => x.GetById(3)).Returns(etsub3);
            tiRepoMock.Setup(x => x.GetById(4)).Returns(etsub4);
            tiRepoMock.Setup(x => x.GetById(5)).Returns(etsub5);
            tiRepoMock.Setup(x => x.GetById(6)).Returns(etsub6);
            tiRepoMock.Setup(x => x.GetById(7)).Returns(etsub7);
            tiRepoMock.Setup(x => x.GetById(8)).Returns(etsub8);

            bpRepoMock.Setup(x => x.FindByProductId(1)).Returns(bpMain);
            bpRepoMock.Setup(x => x.FindByProductId(2)).Returns(bpsub2);
            bpRepoMock.Setup(x => x.FindByProductId(4)).Returns(bpSub4);
            bpRepoMock.Setup(x => x.FindByProductId(7)).Returns(bpSub7);

            evePricesMock.Setup(x => x.GetPriceInfo(It.IsAny<long>())).Returns(new EvePriceInfo(1, 0, 0));
            esiPricesMock.Setup(x => x.GetAdjustedPriceInfo(It.IsAny<long>())).Returns(new ESIPriceData()
                {AdjustedPrice = 0, AveragePrice = 0, TypeId = 0});

            var mb = new ManufacturingInfoBuilder(tiRepoMock.Object, bpRepoMock.Object, evePricesMock.Object,
                esiPricesMock.Object);

            var result = mb.BuildInfo(1);
            Assert.NotNull(result, "Expected 'BuildInfo' return non empty results");
        }

        [Test]
        public void build_info_returns_full_hierarchy()
        {
            
            var etmain = DataHelper.NewType(1, "Main"); //Root item
            var etSub2 = DataHelper.NewType(2, "Sub2"); //item in ignore list with dependencies
            var etsub3 = DataHelper.NewType(3, "Sub3"); //item in ignore list without dependencies
            var etsub4 = DataHelper.NewType(4, "Sub4"); //item with dependencies
            var etsub5 = DataHelper.NewType(5, "Sub5"); //item without dependencies
            var etsub6 = DataHelper.NewType(6, "Sub6"); //Dependency of 2
            var etsub7 = DataHelper.NewType(7, "Sub7"); //Dependency of 4
            var etsub8 = DataHelper.NewType(8, "Sub8"); //Dependency of 7, termination

            var bpMain = DataHelper.NewBp(100, etmain, 1, new[] {(etSub2, 10), (etsub3, 20), (etsub4, 30), (etsub5, 40)});
            var bpsub2 = DataHelper.NewBp(200, etSub2, 1, new[] {(etsub6, 50)});
            var bpSub4 = DataHelper.NewBp(300, etsub4, 1, new[] {(etsub7, 60)});
            var bpSub7 = DataHelper.NewBp(400, etsub7, 1, new[] {(etsub8, 70)});
            
            
            var evePricesMock = new Mock<IEvePricesRepository>();
            var esiPricesMock = new Mock<IEsiPricesRepository>();
            var tiRepoMock = new Mock<IEveTypeInfoRepository>();
            var bpRepoMock = new Mock<IBlueprintsInfoRepository>();
            
            tiRepoMock.Setup(x => x.GetById(1)).Returns(etmain);
            tiRepoMock.Setup(x => x.GetById(2)).Returns(etSub2);
            tiRepoMock.Setup(x => x.GetById(3)).Returns(etsub3);
            tiRepoMock.Setup(x => x.GetById(4)).Returns(etsub4);
            tiRepoMock.Setup(x => x.GetById(5)).Returns(etsub5);
            tiRepoMock.Setup(x => x.GetById(6)).Returns(etsub6);
            tiRepoMock.Setup(x => x.GetById(7)).Returns(etsub7);
            tiRepoMock.Setup(x => x.GetById(8)).Returns(etsub8);

            bpRepoMock.Setup(x => x.FindByProductId(1)).Returns(bpMain);
            bpRepoMock.Setup(x => x.FindByProductId(2)).Returns(bpsub2);
            bpRepoMock.Setup(x => x.FindByProductId(4)).Returns(bpSub4);
            bpRepoMock.Setup(x => x.FindByProductId(7)).Returns(bpSub7);

            evePricesMock.Setup(x => x.GetPriceInfo(It.IsAny<long>())).Returns(new EvePriceInfo(1, 0, 0));
            esiPricesMock.Setup(x => x.GetAdjustedPriceInfo(It.IsAny<long>())).Returns(new ESIPriceData()
                {AdjustedPrice = 0, AveragePrice = 0, TypeId = 0});

            var mb = new ManufacturingInfoBuilder(tiRepoMock.Object, bpRepoMock.Object, evePricesMock.Object,
                esiPricesMock.Object);

            var result = mb.BuildInfo(1);

            //ROOT
            result.TypeId.Should().Be(etmain.Id, "it's configured per eve type info");
            result.CanBeManufactured.Should().Be(true, "root item has blueprint");

            var rootRequirements = result.Requirements;
            rootRequirements.Should().NotBeNullOrEmpty("it's configured for blueprint information");
            rootRequirements.Should().HaveCount(4,"it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.TypeId == etSub2.Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.TypeId == etsub3.Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.TypeId == etsub4.Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.TypeId == etsub5.Id,
                "it's configured for blueprint information");
            
            //SUB2
            var sub2Result = result.Requirements.First(r => r.Material.TypeId == etSub2.Id);
            sub2Result.Quantity.Should().Be(10, 
                "it's configured for root blueprint");
            sub2Result.Material.CanBeManufactured.Should().Be(true,
                "there is blueprint for sub2");
            var sub2Requirements = sub2Result.Material.Requirements;
            
            sub2Requirements.Should().HaveCount(1, "it's configured for blueprint");
            sub2Requirements.Should().Contain(i => i.Material.TypeId == etsub6.Id,
                "it's configured per blueprint");
            
            //SUB3
            var sub3Result = result.Requirements.First(r => r.Material.TypeId == etsub3.Id);
            sub3Result.Quantity.Should().Be(20, "It's configured for blueprint");
            sub3Result.Material.CanBeManufactured.Should().Be(false, "There is no blueprint for sub3");
            var sub3Requirements = sub3Result.Material.Requirements;
            
            sub3Requirements.Should().BeNullOrEmpty("Expected sub2 object to contain 1 requirement as configured for blueprint");
        }
    }
}