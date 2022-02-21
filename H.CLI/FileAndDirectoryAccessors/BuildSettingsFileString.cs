using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class BuildSettingsFileString
    {
        #region Fields

        //private GeographicData geographicData = new GeographicData();
        private KeyConverter.KeyConverter _keyConverter = new KeyConverter.KeyConverter();
        private UnitsOfMeasurementCalculator uCalc = new UnitsOfMeasurementCalculator();
        public List<string> keys = new List<string>();
        private const int dataRoundingDecimalPoints = 4;

        #endregion

        #region Constructors

        /// <summary>
        /// A list of global settings, if you would like to add a new global setting, add another 
        /// key and value below in the format: "KeyFormat = _defaults.Value
        /// Every time you add a new Global Settings Key remember to also add a line to parse the new setting(s) in the SettingsHandler
        /// ApplySettingsFromUserFile method (where we populate the default and geographic data settings for the Farm).
        /// </summary>
        /// <param name="geographicData"></param>
        /// <param name="defaults">The default to build the settings file. This can come from and imported (exported from GUI) farm so it must be passed as an argument.</param>
        public BuildSettingsFileString(Farm farm)
        {
            var defaults = farm.Defaults;
            var geographicData = farm.GeographicData;
            var climateData = farm.ClimateData;
            const string KeyValuePairSeparator = " = ";
            const string UnitsAndKeySeparator = " ";

            keys = new List<string>()
            {
                string.Format(Properties.Resources.SettingsFileHeader,farm.PolygonId),
            Properties.Resources.Settings_Latitude + KeyValuePairSeparator + farm.Latitude.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_Longitude + KeyValuePairSeparator + farm.Longitude.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_CarbonConcentration + UnitsAndKeySeparator +
                                                                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerKilogram) +
                                                                KeyValuePairSeparator +
                                                                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramPerKilogram,defaults.CarbonConcentration, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_EmergenceDay +  KeyValuePairSeparator +  defaults.EmergenceDay.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_RipeningDay +  KeyValuePairSeparator +  defaults.RipeningDay.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_Variance +  KeyValuePairSeparator +  defaults.Variance.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_Alfa +  KeyValuePairSeparator +  defaults.Alfa.ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_DecompositionMinimumTemperature +
                UnitsAndKeySeparator +
                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius) +
                KeyValuePairSeparator +
                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,defaults.DecompositionMinimumTemperature, false).ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_DecompositionMaximumTemperature +
                UnitsAndKeySeparator +
                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius) +
                KeyValuePairSeparator +
                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, defaults.DecompositionMaximumTemperature, false).ToString(CLILanguageConstants.culture),


            Properties.Resources.Settings_MoistureResponseFunctionAtSaturation + KeyValuePairSeparator + defaults.MoistureResponseFunctionAtSaturation.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MoistureResponseFunctionAtWiltingPoint + KeyValuePairSeparator + defaults.MoistureResponseFunctionAtWiltingPoint.ToString(CLILanguageConstants.culture),

            Properties.Resources.AnnualCrops,
            Properties.Resources.Settings_PercentageOfProductReturnedToSoilForAnnuals + KeyValuePairSeparator + defaults.PercentageOfProductReturnedToSoilForAnnuals.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForAnnuals + KeyValuePairSeparator + defaults.PercentageOfStrawReturnedToSoilForAnnuals.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForAnnuals + KeyValuePairSeparator + defaults.PercentageOfRootsReturnedToSoilForAnnuals.ToString(CLILanguageConstants.culture),

            Properties.Resources.SilageCrops,
            Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForSilageCrops+ KeyValuePairSeparator + defaults.PercentageOfProductYieldReturnedToSoilForSilageCrops.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForSilageCrops + KeyValuePairSeparator + defaults.PercentageOfRootsReturnedToSoilForSilageCrops.ToString(CLILanguageConstants.culture),

            Properties.Resources.CoverCrops,
            Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCrops + KeyValuePairSeparator + defaults.PercentageOfProductYieldReturnedToSoilForCoverCrops.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCropsForage + KeyValuePairSeparator + defaults.PercentageOfProductYieldReturnedToSoilForCoverCropsForage.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfProductYieldReturnedToSoilForCoverCropsProduce + KeyValuePairSeparator + defaults.PercentageOfProductYieldReturnedToSoilForCoverCropsProduce.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForCoverCrops + KeyValuePairSeparator + defaults.PercentageOfStrawReturnedToSoilForCoverCrops.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForCoverCrops + KeyValuePairSeparator + defaults.PercetageOfRootsReturnedToSoilForCoverCrops.ToString(CLILanguageConstants.culture),

            Properties.Resources.RootCrops,
            Properties.Resources.Settings_PercentageOfProductReturnedToSoilForRootCrops + KeyValuePairSeparator + defaults.PercentageOfProductReturnedToSoilForRootCrops.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfStrawReturnedToSoilForRootCrops + KeyValuePairSeparator + defaults.PercentageOfStrawReturnedToSoilForRootCrops.ToString(CLILanguageConstants.culture),

            Properties.Resources.PerennialCrops,
            Properties.Resources.Settings_PercentageOfProductReturnedToSoilForPerennials + KeyValuePairSeparator + defaults.PercentageOfProductReturnedToSoilForPerennials.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForPerennials + KeyValuePairSeparator + defaults.PercentageOfRootsReturnedToSoilForPerennials.ToString(CLILanguageConstants.culture),

            Properties.Resources.Rangeland,
            Properties.Resources.Settings_PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss + KeyValuePairSeparator + defaults.PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForRangeland + KeyValuePairSeparator + defaults.PercentageOfRootsReturnedToSoilForRangeland.ToString(CLILanguageConstants.culture),

            Properties.Resources.FodderCorn,
            Properties.Resources.Settings_PercentageOfProductReturnedToSoilForFodderCorn + KeyValuePairSeparator + defaults.PercentageOfProductReturnedToSoilForFodderCorn.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_PercentageOfRootsReturnedToSoilForFodderCorn + KeyValuePairSeparator + defaults.PercentageOfRootsReturnedToSoilForFodderCorn.ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_DecompositionRateConstantYoungPool + KeyValuePairSeparator + defaults.DecompositionRateConstantYoungPool.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_DecompositionRateConstantOldPool + KeyValuePairSeparator + defaults.DecompositionRateConstantOldPool.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_OldPoolCarbonN + KeyValuePairSeparator + defaults.OldPoolCarbonN.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_NORatio + KeyValuePairSeparator +  defaults.NORatio.ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_EmissionFactorForLeachingAndRunOff +
                UnitsAndKeySeparator +
                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN) +
                KeyValuePairSeparator +
                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN, defaults.EmissionFactorForLeachingAndRunoff, false).ToString(CLILanguageConstants.culture),


            Properties.Resources.Settings_EmissionFactorForVolatilization +
                UnitsAndKeySeparator +
                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN) +
                KeyValuePairSeparator +
                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerKilogramN, defaults.EmissionFactorForVolatilization, false).ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_FractionOfNLostByVolatilization + KeyValuePairSeparator + defaults.FractionOfNLostByVolatilization.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MicrobeDeath + KeyValuePairSeparator + defaults.MicrobeDeath.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_Denitrification + KeyValuePairSeparator + defaults.Denitrification.ToString(CLILanguageConstants.culture),

            // ICBM
            Properties.Resources.ICBMSettingsHeader,

            Properties.Resources.Settings_HumificationCoefficientAboveGround + KeyValuePairSeparator + defaults.HumificationCoefficientAboveGround.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_HumificationCoefficientBelowGround + KeyValuePairSeparator + defaults.HumificationCoefficientBelowGround.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_HumificationCoefficientManure + KeyValuePairSeparator + defaults.HumificationCoefficientManure.ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_ClimateFilename + KeyValuePairSeparator + "climate.csv",
            Properties.Resources.Settings_ClimateDataAcquisition + KeyValuePairSeparator + farm.ClimateAcquisition.ToString(),
            Properties.Resources.Settings_UseClimateParameterInsteadOfManagementFactor + KeyValuePairSeparator + defaults.UseClimateParameterInsteadOfManagementFactor.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_EnableCarbonModelling + KeyValuePairSeparator + farm.EnableCarbonModelling.ToString(CLILanguageConstants.culture),

            string.Format(Properties.Resources.PrecipitationData, uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters)),
            Properties.Resources.Settings_JanuaryPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, climateData.PrecipitationData.January, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_FebruaryPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, climateData.PrecipitationData.February, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MarchPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, climateData.PrecipitationData.March, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AprilPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.April, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MayPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.May, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JunePrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.June, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JulyPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.July, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AugustPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.August, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_SeptemberPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.September, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_OctoberPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.October, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_NovemberPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.November, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_DecemberPrecipitation + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters,climateData.PrecipitationData.December, false).ToString(CLILanguageConstants.culture),

            string.Format(Properties.Resources.EvapotranspirationData, uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear)),
            Properties.Resources.Settings_JanuaryPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, climateData.EvapotranspirationData.January, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_FebruaryPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.February, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MarchPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.March, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AprilPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.April, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MayPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.May, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JunePotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.June, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JulyPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.July, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AugustPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.August, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_SeptemberPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.September, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_OctoberPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.October, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_NovemberPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.November, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_DecemberPotentialEvapotranspiration+ KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear,climateData.EvapotranspirationData.December, false).ToString(CLILanguageConstants.culture),

            string.Format(Properties.Resources.TemperatureData, uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius)),
            Properties.Resources.Settings_JanuaryMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius, climateData.TemperatureData.January, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_FebruaryMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.February, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MarchMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.March, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AprilMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.April, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_MayMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.May, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JuneMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.June, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_JulyMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.July, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_AugustMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.August, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_SeptemberMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.September, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_OctoberMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.October, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_NovemberMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.November, false).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_DecemberMeanTemperature + KeyValuePairSeparator + uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.DegreesCelsius,climateData.TemperatureData.December, false).ToString(CLILanguageConstants.culture),

            Properties.Resources.SoilData,
            Properties.Resources.Settings_YearOfObservation + KeyValuePairSeparator +  geographicData.DefaultSoilData.YearOfObservation.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_EcodistrictID + KeyValuePairSeparator +  geographicData.DefaultSoilData.EcodistrictId.ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_SoilGreatGroup + KeyValuePairSeparator + geographicData.DefaultSoilData.SoilGreatGroup,

            Properties.Resources.Settings_SoilFunctionalCategory + KeyValuePairSeparator + geographicData.DefaultSoilData.SoilFunctionalCategory,

            Properties.Resources.Settings_BulkDensity + KeyValuePairSeparator +  Math.Round(geographicData.DefaultSoilData.BulkDensity, dataRoundingDecimalPoints).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_SoilTexture + KeyValuePairSeparator +  geographicData.DefaultSoilData.SoilTexture.ToString(),
            Properties.Resources.Settings_SoilPh + KeyValuePairSeparator +  Math.Round(geographicData.DefaultSoilData.SoilPh, dataRoundingDecimalPoints).ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_TopLayerThickness +
                UnitsAndKeySeparator +
                uCalc.GetUnitsOfMeasurementString(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters) +
                KeyValuePairSeparator +
                uCalc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Millimeters, geographicData.DefaultSoilData.TopLayerThickness, false).ToString(CLILanguageConstants.culture),

            Properties.Resources.Settings_ProportionOfSandInSoil  + KeyValuePairSeparator +  Math.Round(geographicData.DefaultSoilData.ProportionOfSandInSoil, dataRoundingDecimalPoints).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_ProportionOfClayInSoil  + KeyValuePairSeparator +  Math.Round(geographicData.DefaultSoilData.ProportionOfClayInSoil, dataRoundingDecimalPoints).ToString(CLILanguageConstants.culture),
            Properties.Resources.Settings_ProportionOfSoilOrganicCarbon  + KeyValuePairSeparator +  Math.Round(geographicData.DefaultSoilData.ProportionOfSoilOrganicCarbon, dataRoundingDecimalPoints).ToString(CLILanguageConstants.culture),
            };
        }

        #endregion
    }
}
