#region Imports

#endregion

using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// http://sis.agr.gc.ca/cansis/nsdb/soil/v2/snt/index.html
    /// </summary>
    internal class SoilNameTableData
    {
        public string SoilNameIdentifier { get; set; }
        public string ProvinceCode { get; set; }
        public string SoilCode { get; set; }
        public string SoilCodeModifier { get; set; }
        public string TypeOfSoilProfile { get; set; }
        public string SoilName { get; set; }
        public string KindOfSurfaceMaterial { get; set; }
        public string WaterTableCharacteristics { get; set; }
        public string SoilLayerThatRestrictsRootsGrowth { get; set; }
        public string TypeOfRootRestrictingLayer { get; set; }
        public string SoilDrainageClass { get; set; }
        public string FirstParentMaterialTexture { get; set; }
        public string SecondParentMaterialTexture { get; set; }
        public string ThirdParentMaterialTexture { get; set; }
        public string FirstParentMaterialChemicalProperty { get; set; }
        public string SecondParentMaterialChemicalProperty { get; set; }
        public string ThirdParentMaterialChemicalProperty { get; set; }
        public string FirstModeOfDeposition { get; set; }
        public string SecondModeOfDeposition { get; set; }
        public string ThirdModeOfDeposition { get; set; }
        public string SoilOrderSecondEdition { get; set; }
        public string SoilGreatGroupSecondEdition { get; set; }
        public string SoilSubgroupSecondEdition { get; set; }
        public string SoilOrderThirdEdition { get; set; }
        public string SoilGreatGroupThirdEdition { get; set; }
        public string SoilSubgroupThirdEdition { get; set; }
        public Province Province { get; set; }
    }
}