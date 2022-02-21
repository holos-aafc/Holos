using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.Core.Enumerations;

namespace H.CLI.ComponentKeys
{
    /// <summary>
    /// The dictionary below takes in a string - the header and a nullable ImperialUnitsOfMeasurement?. We do not include
    /// the MetricUnitsOfMeasurement because the calculations performed later demand that the values be in Metric.
    /// Therefore, in our parser, we need to convert all Imperial units to Metric units and to do that, we only need
    /// to know what the Imperial units are (because if its metric, we don't need to do anything because the data is already
    /// in Metric).
    /// </summary>
    public class FieldKeys : IComponentKeys
    {
        /// <summary>
        /// When you modify the key, remember to add a new property corresponding to the new key that you have added 
        /// below to the FieldTemporaryInput class in the format: "Example Format",
        /// In this case, please add a new property in the format: ExampleFormat to the concrete FieldTemporaryInput class.
        /// The order of the keys below is the order in which they will be written when creating the template files for a Field.
        /// </summary>

        public FieldKeys()
        {
            this.MissingHeaders.Add(Properties.Resources.Key_NitrogenFixation, false);
        }
        public Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; } = new Dictionary<string, ImperialUnitsOfMeasurement?>()
        {
            // Note the casing must match in the resource files (i.e. it must be Phase Number in the resource file not Phase number). This is because reflection is
            // being used

            // When adding a string here, ensure property is added to FieldTemporaryInput class as well

            // Ordering matters here, the ordering of keys here must match the ordering of the columns in the input files (i.e. in FieldProcessor class)
            {Properties.Resources.Key_PhaseNumber, null },
            {Properties.Resources.Key_Name, null },
            {Properties.Resources.Key_Area, ImperialUnitsOfMeasurement.Acres },
            {Properties.Resources.Key_CurrentYear, null },
            {Properties.Resources.Key_CropYear, null },
            {Properties.Resources.Key_CropType, null },
            {Properties.Resources.Key_TillageType, null },
            {Properties.Resources.Key_YearInPerennialStand, null },
            {Properties.Resources.Key_PerennialStandID, null },
            {Properties.Resources.Key_PerennialStandLength, null },
            {Properties.Resources.Key_BiomassCoefficientProduct, null },
            {Properties.Resources.Key_BiomassCoefficientStraw, null },
            {Properties.Resources.Key_BiomassCoefficientRoots, null },
            {Properties.Resources.Key_BiomassCoefficientExtraroot, null },
            {Properties.Resources.Key_NitrogenContentInProduct, ImperialUnitsOfMeasurement.PoundsPerPound },
            {Properties.Resources.Key_NitrogenContentInStraw, ImperialUnitsOfMeasurement.PoundsPerPound },
            {Properties.Resources.Key_NitrogenContentInRoots, ImperialUnitsOfMeasurement.PoundsPerPound },
            {Properties.Resources.Key_NitrogenContentInExtraroot, ImperialUnitsOfMeasurement.PoundsPerPound },
            {Properties.Resources.Key_NitrogenFixation, ImperialUnitsOfMeasurement.PoundsNitrogenPerAcrePerYear },
            {Properties.Resources.Key_NitrogenDeposit, ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre },
            {Properties.Resources.Key_CarbonConcentration, ImperialUnitsOfMeasurement.PoundsPerPound },
            {Properties.Resources.Key_Yield, ImperialUnitsOfMeasurement.BushelsPerAcre },
            {Properties.Resources.Key_HarvestMethod, null },
            {Properties.Resources.Key_NitrogenFertilizerRate, ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre },
            {Properties.Resources.Key_PhosphorousFertilizerRate, ImperialUnitsOfMeasurement.PoundsPhosphorousPerAcre },
            {Properties.Resources.Key_IsIrrigated, null },
            {Properties.Resources.Key_IrrigationType, null },
            {Properties.Resources.Key_AmountOfIrrigation, ImperialUnitsOfMeasurement.InchesToMm },
            {Properties.Resources.Key_MoistureContentOfCrop,  ImperialUnitsOfMeasurement.Percentage},
            {Properties.Resources.Key_PercentageOfStrawReturnedToSoil,  ImperialUnitsOfMeasurement.Percentage },
            {Properties.Resources.Key_PercentageOfRootsReturnedToSoil,  ImperialUnitsOfMeasurement.Percentage },
            {Properties.Resources.Key_PercentageOfProductYieldReturnedToSoil,  ImperialUnitsOfMeasurement.Percentage },
            {Properties.Resources.Key_IsPesticideUsed, null },
            {Properties.Resources.Key_NumberOfPesticidePasses, null },
            {Properties.Resources.Key_ManureApplied, null },
            {Properties.Resources.Key_AmountOfManureApplied, ImperialUnitsOfMeasurement.PoundsPerAcre },
            {Properties.Resources.Key_ManureApplicationType, null },
            {Properties.Resources.Key_ManureAnimalSourceType, null },
            {Properties.Resources.Key_ManureStateType, null },
            {Properties.Resources.Key_ManureLocationSourceType, null },
            {Properties.Resources.Key_CoverCropsUsed, null },
            {Properties.Resources.Key_CoverCropType, null },
            {Properties.Resources.Key_CoverCropTerminationType, null },
            {Properties.Resources.Key_UnderSownCropsUsed, null },
            {Properties.Resources.Key_CropIsGrazed, null },
            {Properties.Resources.Key_FieldSystemComponentGuid, null },
            {Properties.Resources.Key_TimePeriodCategoryString, null },
            {Properties.Resources.Key_ClimateParameter, null },
            {Properties.Resources.Key_TillageFactor, null },
            {Properties.Resources.Key_ManagementFactor, null },
            {Properties.Resources.Key_PlantCarbonInAgriculturalProduct, null },
            {Properties.Resources.Key_CarbonInputFromProduct, null },
            {Properties.Resources.Key_CarbonInputFromStraw, null },
            {Properties.Resources.Key_CarbonInputFromRoots, null },
            {Properties.Resources.Key_CarbonInputFromExtraroots, null },
            {Properties.Resources.Key_SizeOfFirstRotationForField, null },
            {Properties.Resources.Key_AboveGroundCarbonInput, null },
            {Properties.Resources.Key_BelowGroundCarbonInput, null },
        };


        //currently only 2 optional headers in the fieldkeys
        public bool IsHeaderOptional(string s)
        {
            if (s == Properties.Resources.Key_NitrogenFixation) return true;
            else if (s == Properties.Resources.Key_NitrogenDeposit) return true;
            else return false;
        }
        //populate with all the keys that exist curently and tell if it is missing or not
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>(){
};
    }

}
