using System.Collections.Generic;
using System.Threading.Tasks;
using Eveindustry.Shared.DTO.EveType;
using Eveindustry.Shared.DTO.EveTypeSearch;

namespace EveIndustry.Web.Services
{
    public interface IEveItemSearchService
    {
        Task<IList<EveTypeSearchInfo>> Search(string searchText);

        Task<IList<EveTypeDto>> GetAllDependent(long id);
    }
}