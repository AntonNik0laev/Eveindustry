using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Shared.DTO.EveType;
using Eveindustry.Shared.DTO.EveTypeDependenciesRequest;
using Eveindustry.Shared.DTO.EveTypeSearch;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Authenticators.OAuth;

namespace Eveindustry.API.Controllers
{
    [ApiController]
    [Route("/types")]
    public class EveTypesController : Controller
    {
        private readonly IMapper mapper;
        private readonly IEveTypeRepository repository;

        public EveTypesController(IMapper mapper, IEveTypeRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }
        
        // GET
        [HttpGet("search")]
        [ProducesResponseType(typeof(EveTypeSearchResponse), 200)]
        public IActionResult Search([FromQuery]EveTypeSearchRequest request)
        {

            var searchResults = this.repository.FindByPartialName(request.PartialName,
                mapper.Map<FindByPartialNameOptions>(request.Options));

            return Ok(new EveTypeSearchResponse()
            {
                SearchResults = mapper.Map<IList<EveTypeSearchInfo>>(searchResults)
            });
        }

        [HttpGet("dependencies")]
        [ProducesResponseType(typeof(EveTypeDependenciesResponse), 200)]
        public IActionResult GetAllDependencies([FromQuery] EveTypeDependenciesRequest request)
        {
            var results = this.repository.GetAllDependencies(request.TypeId);
            return Ok(new EveTypeDependenciesResponse()
            {
                EveTypes = mapper.Map<IList<EveTypeDto>>(results)
            });
        }
    }
}