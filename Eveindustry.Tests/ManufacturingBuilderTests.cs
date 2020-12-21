using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Eveindustry.API;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Shared;
using Eveindustry.Shared.DTO;
using Eveindustry.Shared.DTO.EveType;
using Eveindustry.Shared.Profiles;
using Eveindustry.Tests.Utils;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Eveindustry.Tests
{
    // TODO refactor to updated api with EveType / SdeType
    class ManufacturingBuilderTests
    {
        private EveType[] types;
        private ManufacturingInfoBuilder sut;
        private SortedList<long, EveTypeManufacturingParameters> sorted;
        
        [SetUp]
        public void Setup()
        {
            //               1
            //            / / | \
            //           2 3  4  5
            //          /    /\
            //         6    7  8
            //             /    \
            //             8     9
            //            /
            //            9






            var etsub9 = DataHelper.NewType(9, "Sub9"); //Dependency of 8
            var etsub8 = DataHelper.NewType(8, "Sub8"); //Dependency of 7
            var etsub7 = DataHelper.NewType(7, "Sub7"); //Dependency of 4
            var etsub6 = DataHelper.NewType(6, "Sub6"); //Dependency of 2
            var etsub5 = DataHelper.NewType(5, "Sub5"); //item without dependencies
            var etsub4 = DataHelper.NewType(4, "Sub4"); //item with dependencies
            var etsub3 = DataHelper.NewType(3, "Sub3"); //item in ignore list without dependencies
            var etsub2 = DataHelper.NewType(2, "Sub2"); //item in ignore list with dependencies
            var etmain = DataHelper.NewType(1, "Main"); //Root item

            etmain.Blueprint = DataHelper.NewBp(100, etmain, 1, new[] {(etsub2, 10), (etsub3, 20), (etsub4, 30), (etsub5, 40)});
            etsub2.Blueprint = DataHelper.NewBp(200, etsub2, 1, new[] {(etsub6, 50)});
            etsub4.Blueprint = DataHelper.NewBp(300, etsub4, 1, new[] {(etsub7, 60), (etsub8, 60)});
            etsub7.Blueprint = DataHelper.NewBp(400, etsub7, 1, new[] {(etsub8, 70)});
            etsub8.Blueprint =  DataHelper.NewBp(500, etsub8, 150000, new[] {(etsub9, 70)});

            var etMock = new Mock<IEveTypeRepository>();

            etMock.Setup(x => x.GetById(1)).Returns(etmain);
            etMock.Setup(x => x.GetById(2)).Returns(etsub2);
            etMock.Setup(x => x.GetById(3)).Returns(etsub3);
            etMock.Setup(x => x.GetById(4)).Returns(etsub4);
            etMock.Setup(x => x.GetById(5)).Returns(etsub5);
            etMock.Setup(x => x.GetById(6)).Returns(etsub6);
            etMock.Setup(x => x.GetById(7)).Returns(etsub7);
            etMock.Setup(x => x.GetById(8)).Returns(etsub8);
            etMock.Setup(x => x.GetById(9)).Returns(etsub9);
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile(new EveItemManufacturingInfoMappingProfile());
                c.AddProfile(new DtoMappingProfile());
            });

            var flatList = new List<EveType> {etmain, etsub2, etsub3, etsub4, etsub5, etsub6, etsub7, etsub8, etsub9};

            var mapper = mapperConfig.CreateMapper();
            var mapped = mapper.Map<IList<EveTypeDto>>(flatList);
            sorted = new SortedList<long, EveTypeManufacturingParameters>(mapped.ToDictionary(i => i.Id, i => new EveTypeManufacturingParameters
            {
                EveType = i,
                Parameters = new ManufacturingParameters()
            }));
            var mb = new ManufacturingInfoBuilder(mapper);
            sut = mb;
            types = new[]
            {
                etmain, // 0
                etsub2, // 1
                etsub3, // 2
                etsub4, // 3 
                etsub5, // 4
                etsub6, // 5
                etsub7, // 6
                etsub8, // 7
                etsub9, // 8
            };
        }

        [Test, Description("When getting build information and requested items exist, result should not be null or empty")]
        public void build_info_returns_non_empty_results()
        {

            var result = sut.BuildInfo(1, sorted);
            Assert.NotNull(result, "Expected 'BuildInfo' return non empty results");
        }

        [Test, Description("When getting build information and request item exist, " +
                           "result should contain hierarchical data representing item, " +
                           "it's dependencies and all dependency dependencies down to buttom level")]
        public void build_info_returns_full_hierarchy()
        {
            var result = sut.BuildInfo(1, sorted);

            //ROOT
            result.Id.Should().Be(types[0].Id, "it's configured per eve type info");
            result.CanBeManufactured.Should().Be(true, "root item has blueprint");

            var rootRequirements = result.Requirements;
            rootRequirements.Should().NotBeNullOrEmpty("it's configured for blueprint information");
            rootRequirements.Should().HaveCount(4,"it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.Id == types[1].Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.Id == types[2].Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.Id == types[3].Id,
                "it's configured for blueprint information");
            rootRequirements.Should().Contain(i => i.Material.Id == types[4].Id,
                "it's configured for blueprint information");
            
            //SUB2
            var sub2Result = result.Requirements.First(r => r.Material.Id == types[1].Id);
            sub2Result.Quantity.Should().Be(10, 
                "it's configured for root blueprint");
            sub2Result.Material.CanBeManufactured.Should().Be(true,
                "there is blueprint for sub2");
            var sub2Requirements = sub2Result.Material.Requirements;
            
            sub2Requirements.Should().HaveCount(1, "it's configured for blueprint");
            sub2Requirements.Should().Contain(i => i.Material.Id == types[5].Id,
                "it's configured per blueprint");
            
            //SUB3
            var sub3Result = result.Requirements.First(r => r.Material.Id == types[2].Id);
            sub3Result.Quantity.Should().Be(20, "It's configured for blueprint");
            sub3Result.Material.CanBeManufactured.Should().Be(false, "There is no blueprint for sub3");
            var sub3Requirements = sub3Result.Material.Requirements;
            
            sub3Requirements.Should().BeNullOrEmpty("no requirements configured for blueprint");
            
            //SUB4
            var sub4Result = result.Requirements.First(r => r.Material.Id == types[3].Id);
            sub4Result.Quantity.Should().Be(30, 
                "it's configured for root blueprint");
            sub4Result.Material.CanBeManufactured.Should().Be(true,
                "there is blueprint for sub4");
            var sub4Requirements = sub4Result.Material.Requirements;
            
            sub4Requirements.Should().HaveCount(2, "it's configured for blueprint");
            sub4Requirements.Should().Contain(i => i.Material.Id == types[6].Id,
                "it's configured per blueprint");
            sub4Requirements.Should().Contain(i => i.Material.Id == types[7].Id,
                "it's configured per blueprint");
            
            //SUB5
            var sub5Result = result.Requirements.First(r => r.Material.Id == types[4].Id);
            sub5Result.Quantity.Should().Be(40, "It's configured for blueprint");
            sub5Result.Material.CanBeManufactured.Should().Be(false, "There is no blueprint for sub5");
            var sub5Requirements = sub3Result.Material.Requirements;
            
            sub5Requirements.Should().BeNullOrEmpty("no requirements configured for blueprint");
            
            //SUB6
            var sub6Result = sub2Result.Material.Requirements.First(r => r.Material.Id == types[5].Id);
            sub6Result.Quantity.Should().Be(50, "It's configured for blueprint");
            sub6Result.Material.CanBeManufactured.Should().Be(false, "There is no blueprint for sub6");
            var sub6Requirements = sub3Result.Material.Requirements;
            
            sub6Requirements.Should().BeNullOrEmpty("no requirements configured for blueprint");
            
            //SUB7
            var sub7Result = sub4Result.Material.Requirements.First(r => r.Material.Id == types[6].Id);
            sub7Result.Quantity.Should().Be(60, "It's configured for blueprint");
            sub4Result.Material.CanBeManufactured.Should().Be(true,
                "there is blueprint for sub7");
            var sub7Requirements = sub7Result.Material.Requirements;
            
            sub7Requirements.Should().HaveCount(1, "it's configured for blueprint");
            sub7Requirements.Should().Contain(i => i.Material.Id == types[7].Id,
                "it's configured per blueprint");
            
            //SUB8
            var sub8Result = sub7Result.Material.Requirements.First(r => r.Material.Id == types[7].Id);
            sub8Result.Quantity.Should().Be(70, "It's configured for blueprint");
            sub8Result.Material.CanBeManufactured.Should().Be(true, "there is blueprint for sub8");
            var sub8Requirements = sub8Result.Material.Requirements;
            
            sub8Requirements.Should().HaveCount(1, "it's configured per blueprint");
            sub8Requirements.Should().Contain(i => i.Material.Id == types[8].Id,
                "it's configured per blueprint");
        }

        [Test, Description("when getting flat items list it should contain all required items from hierarchy with all total quantities" +
                           " aggregated for each item")]
        public void get_flat_list_should_return_aggregated_list_with_total_quantities()
        {
            var result = sut.GetFlatManufacturingList(sut.BuildInfo(1, sorted), 2).Values;

            result.Count().Should().Be(types.Count(), "Aggregated list should contain no duplicates");
            var main = result.First(i => i.Material.Id == types[0].Id);

            var sub2 = result.First(i => i.Material.Id == types[1].Id);
            var sub3 = result.First(i => i.Material.Id == types[2].Id);
            var sub4 = result.First(i => i.Material.Id == types[3].Id);
            var sub5 = result.First(i => i.Material.Id == types[4].Id);
            var sub6 = result.First(i => i.Material.Id == types[5].Id);
            var sub7 = result.First(i => i.Material.Id == types[6].Id);
            var sub8 = result.First(i => i.Material.Id == types[7].Id);
            var sub9 = result.First(i => i.Material.Id == types[8].Id);
            main.Quantity.Should().Be(2, "it's configured as method parameter");
            main.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub2.Quantity.Should().Be(10 * 2, "requested 2 items of main, each requires 10 items of sub2");
            sub2.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub3.Quantity.Should().Be(20 * 2, "requested 2 items of main, each requires 20 items of sub3");
            sub3.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub4.Quantity.Should().Be(30 * 2, "requested 2 items of main, each requires 30 items of sub4");
            sub4.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub5.Quantity.Should().Be(40 * 2, "requested 2 items of main, each requires 40 items of sub5");
            sub5.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub6.Quantity.Should().Be(50 * 10 * 2, "requested 2 items of main, each requires 10 items of sub2, each requires 50 items of sub6, e.g 50*10*2");
            sub6.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub7.Quantity.Should().Be(60 * 30 * 2, "requested 2 items of main, each requires 30 items of sub4, each requires 60 items of sub7, e.g. 60*30*2");
            sub7.RemainingQuantity.Should().Be(0, "no items should remain after manufacturing");
            sub8.Quantity.Should().Be(300000, "material required for two items - (60 * 30 * 2) + (70 * 60 * 30 * 2) = 255600, " +
                                                                          "but it's only possible to build 150000 per run, so 300000 is minimum with remaining 44000");
            sub8.RemainingQuantity.Should().Be(44400,
                "material required for two items - (60 * 30 * 2) + (70 * 60 * 30 * 2) = 255600, " +
                "but it's only possible to build 150000 per run, so 300000 is minimum with remaining 44000");
            sub9.Quantity.Should().Be(70 * 2, "sub9 is needed to build 300000 items of sub8 for 2 runs. each run require 70 items, so 70 *2 ");
        }

        [Test, Description("when list of ignored ids passed to GetFlatManufacturingList, it should not add childrens of that items to flat list, " +
                           "but ignored items itself should be added")]
        public void get_flat_list_with_ignored_ids_parameter_should_not_contain_dependencies_for_ignored_ids()
        {
            var ignoreList = new[] {2L, 3L};
            
            foreach (var i in ignoreList)
            {
                sorted[i].Parameters.ForceBuy = true;
            }
            
            var results = sut.GetFlatManufacturingList(sut.BuildInfo(1, sorted), 1).Values;
            results.Should().HaveCount(8, "Ignore list contains one dependency which should be excluded");
            results.Should().NotContain(i => i.Material.Id == 6,
                "Excluded item id is 6, so it should not be in flat list");
        }

        [Test]
        public void stages_should_not_contain_dependencies_of_items_from_ignore_list()
        {
            var ignoreList = new[] {2L, 3L, 8L};
            foreach (var i in ignoreList)
            {
                sorted[i].Parameters.ForceBuy = true;
            }
            var results = sut.GroupIntoStages(sut.GetFlatManufacturingList(sut.BuildInfo(1L,sorted), 1).Values);
            results.Should().HaveCount(4, "manufacturing should contain 4 stages");
            var stage0 = results.First();
            stage0.Should().HaveCount(4);
            stage0.Should().Contain(i => i.Material.Id == 2, "stage 0 should contain ignored items and items without dependencies. 2 is ignored");
            stage0.Should().Contain(i => i.Material.Id == 3, "stage 0 should contain ignored items and items without dependencies. 3 is ignored");
            stage0.Should().Contain(i => i.Material.Id == 5, "stage 0 should contain ignored items and items without dependencies. 5 is ignored");
            stage0.Should().Contain(i => i.Material.Id == 8, "stage 0 should contain ignored items and items without dependencies. 8 is ignored");
        }
    }
}