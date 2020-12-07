using System.Collections.Generic;

namespace Eveindustry.Shared.DTO.EveType
{
    /// <summary>
    /// Information about type blueprint.
    /// </summary>
    public class EveBlueprint : EveIdNameBase
    {
        /// <summary>
        /// type Ids and quantities for type manufacturing or reaction
        /// </summary>
        public List<EveMaterialRequirement> Materials { get; set; }
        
        /// <summary>
        /// manufacturing or reaction result type id.
        /// </summary>
        public long ProducedTypeId { get; set; }

        /// <summary>
        /// Manufacturing or reaction result type items quantity per job run.  
        /// </summary>
        public long ItemsPerRun { get; set; }
    }
}