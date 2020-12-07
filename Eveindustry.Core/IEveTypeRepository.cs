using System.Collections.Generic;
using System.Threading.Tasks;
using Eveindustry.Core.Models;

namespace Eveindustry.Core
{
    /// <summary>
    /// Provides methods to access eve types data
    /// </summary>
    public interface IEveTypeRepository
    {
        /// <summary>
        /// Load all required data.
        /// </summary>
        Task Init();

        /// <summary>
        /// Get eve type by it's type id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EveType GetById(long id);
        /// <summary>
        /// Get all existed eve types. 
        /// </summary>
        /// <returns></returns>
        IList<EveType> GetAll();
        /// <summary>
        /// Get eve type by exact name, ignoring case. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        EveType GetByExactName(string name);

        /// <summary>
        /// Find eve types by partial name, starting with, containing all exact matching parameter.
        /// </summary>
        /// <param name="partialName">partial name </param>
        /// <param name="options">search configuration. </param>
        /// <returns>list of matched records. </returns>
        List<EveType> FindByPartialName(string partialName, FindByPartialNameOptions options);

        public IList<EveType> GetAllDependencies(long rootTypeId);
    }
}