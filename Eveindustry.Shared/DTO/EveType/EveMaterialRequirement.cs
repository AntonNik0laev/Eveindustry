namespace Eveindustry.Shared.DTO.EveType
{
    /// <summary>
    /// Information about quantity and material required for blueprint activity.
    /// </summary>
    public class EveMaterialRequirement 
    {
        /// <summary>
        /// Quantity of material required for blueprint activity
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// Type of material required for blueprint activity. 
        /// </summary>
        public long MaterialId { get; set; }
    }
}