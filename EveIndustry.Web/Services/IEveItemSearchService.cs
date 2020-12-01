using System.Collections.Generic;
using System.Threading.Tasks;
using Eveindustry.Shared.DTO.EveTypeSearch;

namespace EveIndustry.Web.Services
{
    public interface IEveItemSearchService
    {
        Task<IList<EveTypeSearchInfo>> Search(string searchText);
        string WTF { get; }
    }
}