using System;
using Eveindustry.StaticDataModels;

namespace Eveindustry
{
    public class EveTypeManufacturingInfo
    {
        public EveType EveType { get; set; }
        public BlueprintInfo BlueprintInfo { get; set; }
        public decimal PriceSell { get; set; }
        public decimal PriceBuy { get; set; }
        public int RequiredQuantity { get; set; }
        public decimal AdjustedPrice { get; set; }
        public decimal TotalManufacturingJobPrice { get; set; }
    }
}