using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// http://sis.agr.gc.ca/cansis/nsdb/soil/v2/slt/index.html
    /// </summary>
    internal class SoilLayerTableData
    {
        public string SoilNameIdentifier { get; set; }
        public string ProvinceCode { get; set; }
        public string SoilCode { get; set; }
        public string SoilCodeModifier { get; set; }
        public string TypeOfSoilProfile { get; set; }
        public string LayerNumber { get; set; }
        public int UpperDepth { get; set; }
        public int LowerDepth { get; set; }
        public string HorizonLithologicalDiscontinuity { get; set; }
        public string HorizonMasterCode { get; set; }
        public string HorizonSuffix { get; set; }
        public string HorizonModifier { get; set; }
        public int CoarseFragments { get; set; }
        public string DominantSandFraction { get; set; }
        public int VeryFineSand { get; set; }
        public int TotalSand { get; set; }
        public int TotalSilt { get; set; }
        public int TotalClay { get; set; }
        public double OrganicCarbon { get; set; }
        public double PHInCalciumChloride { get; set; }
        public double PHAsPerProjectReport { get; set; }
        public int BaseSaturation { get; set; }
        public int CationExchangeCapacity { get; set; }
        public double SaturatedHydraulicConductivity { get; set; }
        public int WaterRetentionAt0kP { get; set; }
        public int WaterRetentionAt10kP { get; set; }
        public int WaterRetentionAt33kP { get; set; }
        public int WaterRetentionAt1500kP { get; set; }
        public double BulkDensity { get; set; }
        public int ElectricalConductivity { get; set; }
        public int CalciumCarbonateEquivalent { get; set; }
        public int VonPost { get; set; }
        public int WoodyMaterial { get; set; }
        public Province Province { get; set; }
    }
}