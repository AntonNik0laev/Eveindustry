using System.ComponentModel.DataAnnotations;
using Eveindustry.Shared;

namespace EveIndustry.Web.Shared.ConfigureMaterialsForm
{
    public class ConfigureMaterialsModel
    {
        public int BlueprintMe { get; set; }
        public string FacilityKind { get; set; }
        public string FacilityRigKind { get; set; }
    }
}