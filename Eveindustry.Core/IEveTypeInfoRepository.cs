using System.Collections.Generic;
using Eveindustry.Core.Models;
using Eveindustry.Core.StaticDataModels;

namespace Eveindustry.Core
{
    /// <summary>
    /// Provides search functions for eve SDE data types file.
    /// </summary>
    public interface IEveTypeInfoRepository
    {
        /// <summary>
        /// Get eve type info by type id.
        /// </summary>
        /// <param name="id">type id.</param>
        /// <returns><see cref="EveType"/>. </returns>
        EveType GetById(long id);

        /// <summary>
        /// Find eve type info by exact type name.
        /// </summary>
        /// <param name="name">eve type name. </param>
        /// <returns><see cref="EveType"/>. </returns>
        EveType FindByExactName(string name);

        /// <summary>
        /// Performs search across all records to find partially matched list.
        /// </summary>
        /// <param name="partialName">partial name. </param>
        /// <param name="options">search options. </param>
        /// <returns>list of matches. </returns>
        List<EveType> FindByPartialName(string partialName, FindByPartialNameOptions options);
        /// <summary>
        /// Find one or more eve types by partial name.
        /// </summary>
        /// <param name="partName">partial name. </param>
        /// <returns>matched types. </returns>
        List<EveType> Search(string partName);

        /// <summary>
        /// Get all eve types
        /// </summary>
        /// <returns>all eve types. </returns>
        List<EveType> GetAll();
    }
}