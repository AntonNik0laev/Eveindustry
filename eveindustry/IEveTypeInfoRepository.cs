using System.Collections.Generic;
using Eveindustry.StaticDataModels;

namespace Eveindustry
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
        EveType FindByName(string name);

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