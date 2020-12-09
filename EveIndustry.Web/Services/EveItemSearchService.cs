using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Eveindustry.Shared.DTO.EveType;
using Eveindustry.Shared.DTO.EveTypeDependenciesRequest;
using Eveindustry.Shared.DTO.EveTypeSearch;
using Microsoft.AspNetCore.WebUtilities;

namespace EveIndustry.Web.Services
{
    public class EveItemSearchService : IEveItemSearchService
    {
        private readonly HttpClient client;

        public EveItemSearchService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IList<EveTypeSearchInfo>> Search(string searchText)
        {

            var searchOptions =
                searchText.Length > 3 ? EveTypeSearchOptions.Contains : EveTypeSearchOptions.StartingWith;
            Console.WriteLine("Requesting results from server.. ");
            var url = QueryHelpers.AddQueryString("types/search", new Dictionary<string, string>()
             {
                 {nameof(EveTypeSearchRequest.PartialName), searchText},
                 {nameof(EveTypeSearchRequest.Options), searchOptions.ToString()}
             });
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response =await this.client.SendAsync(request);
            Console.WriteLine($"Response: {response.StatusCode}");
            var content = await response.Content.ReadFromJsonAsync<EveTypeSearchResponse>();
            return content?.SearchResults ?? Array.Empty<EveTypeSearchInfo>();
        }

        public async Task<IList<EveTypeDto>> GetAllDependent(long id)
        {
            var url = $"types/dependencies/{id}";
            var response = await this.client.GetAsync(url);
            var content = await response.Content.ReadFromJsonAsync<EveTypeDependenciesResponse>();
            return content?.EveTypes ?? Array.Empty<EveTypeDto>();
        }
    }
}