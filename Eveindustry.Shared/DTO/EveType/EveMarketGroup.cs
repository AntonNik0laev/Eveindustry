namespace Eveindustry.Shared.DTO.EveType
{
    /// <summary>
    /// Information about market group
    /// </summary>
    public class EveMarketGroup : EveIdNameBase
    {
        /// <summary>
        /// Parent market group id.
        /// </summary>
        public long ParentMarketGroupId { get; set; }
    }
}