using System.Collections;
using System.Collections.Generic;
using Eveindustry.Shared.DTO.EveType;

namespace Eveindustry.Shared.DTO.EveTypeDependenciesRequest
{
    public class EveTypeDependenciesResponse
    {
        public IList<EveTypeDto> EveTypes { get; set; }
    }
}