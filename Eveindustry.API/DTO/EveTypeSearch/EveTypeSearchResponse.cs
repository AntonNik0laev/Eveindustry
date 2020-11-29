using System.Collections.Generic;

namespace Eveindustry.API.DTO.EveTypeSearch
{
    public class EveTypeSearchResponse
    {
        public IEnumerable<EveTypeSearchInfo> SearchResults { get; set; }
    }
}