using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eveindustry.Core.Models;

namespace Eveindustry.Tests.Utils
{
    // TODO add helper for EveType and SdeType creation.
    public static class DataHelper
    {
        public static EveType NewType(long id, string nameEn, EveBlueprint bp = null)
        {
            return new() {Id = id, Name = nameEn};
        }

        public static EveBlueprint NewBp(
            int id, 
            EveType et, 
            int quantity,
            IEnumerable<(EveType Type, int Quantity)> materials)
        {
            var requirements = materials.Select(i => new EveMaterialRequirement()
            {
                MaterialId = i.Type.Id,
                Quantity = i.Quantity
            });

            return new EveBlueprint()
            {
                Id = id, Materials = requirements.ToList(), ItemsPerRun = quantity, ProducedTypeId = et.Id
            };
        }
    }
}