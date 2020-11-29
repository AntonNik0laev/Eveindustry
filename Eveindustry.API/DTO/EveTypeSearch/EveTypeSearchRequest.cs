namespace Eveindustry.API.DTO.EveTypeSearch
{
    public class EveTypeSearchRequest
    {
        public string PartialName { get; set; }
        public EveTypeSearchOptions Options { get; set; }
    }
}