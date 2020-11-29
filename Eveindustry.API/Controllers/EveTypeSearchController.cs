using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Eveindustry.API.DTO.EveTypeSearch;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Authenticators.OAuth;

namespace Eveindustry.API.Controllers
{
    [ApiController]
    [Route("/search")]
    public class EveTypeSearchController : Controller
    {
        private readonly IMapper mapper;
        private readonly IEveTypeInfoRepository repository;

        public EveTypeSearchController(IMapper mapper, IEveTypeInfoRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }
        
        // GET
        [HttpGet]
        [ProducesResponseType(typeof(EveTypeSearchResponse), 200)]
        public IActionResult Get([FromQuery]EveTypeSearchRequest request)
        {

            var searchResults = this.repository.FindByPartialName(request.PartialName,
                mapper.Map<FindByPartialNameOptions>(request.Options));

            return Ok(new EveTypeSearchResponse()
            {
                SearchResults = mapper.Map<IEnumerable<EveTypeSearchInfo>>(searchResults)
            });
        }
    }
}