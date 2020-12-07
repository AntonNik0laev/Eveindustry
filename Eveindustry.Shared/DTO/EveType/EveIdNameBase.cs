namespace Eveindustry.Shared.DTO.EveType
{
    /// <summary>
    /// generic type containing id and name. 
    /// </summary>
    public abstract class EveIdNameBase
    {
        /// <summary>
        /// Eve type item id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Eve type item nam. 
        /// </summary>
        public string Name { get; set; }
    }
}