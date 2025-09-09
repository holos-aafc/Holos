using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        public bool ExportResultsToFile(
            IEnumerable<CropViewItem> results,
            object path,
            CultureInfo cultureInfo,
            MeasurementSystemType measurementSystemType,
            string languageAddOn,
            bool exportedFromGui,
            Farm farm)
        {
            var stringBuilder = new StringBuilder();

            BuildResultsFileOutputHeader(stringBuilder, measurementSystemType);
            string filePath;
            if (exportedFromGui)
                filePath = path.ToString();
            else
                filePath = $"{path}{farm.Name}_{Resources.Output}{languageAddOn}.csv";

            foreach (var viewItem in results)
                BuildResultsFileOutputRow(stringBuilder, viewItem, cultureInfo, measurementSystemType, farm,
                    exportedFromGui);

            try
            {
                if (cultureInfo.Name == InfrastructureConstants.FrenchCultureInfo.Name)
                    //display the accented chars correctly
                    File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.GetEncoding("iso-8859-1"));
                else
                    File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (IOException exception)
            {
                Trace.TraceInformation(
                    $"{nameof(FieldResultsService)}.{nameof(ExportResultsToFile)}: error writing data to csv file: '{exception.Message}'.");

                return false;
            }

            Trace.TraceInformation(
                $"{nameof(FieldResultsService)}.{nameof(ExportResultsToFile)}: successfully exported data to csv file: '{filePath}'.");
            return true;
        }

        public void ExportAllResultsToFile(
            string path,
            MeasurementSystemType measurementSystemType,
            CultureInfo cultureInfo,
            IEnumerable<CropViewItem> viewItems,
            Farm farm)
        {
            var stringBuilder = new StringBuilder();

            BuildResultsFileOutputHeader(stringBuilder, measurementSystemType);

            foreach (var cropViewItem in viewItems)
                BuildResultsFileOutputRow(stringBuilder, cropViewItem, cultureInfo, measurementSystemType, farm, false);

            try
            {
                File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Trace.TraceInformation(
                    $"{nameof(FieldResultsService)}.{nameof(ExportAllResultsToFile)}: error writing data to csv file: '{exception.Message}'.");

                return;
            }

            Trace.TraceInformation(
                $"{nameof(FieldResultsService)}.{nameof(ExportAllResultsToFile)}: successfully exported data to csv file: '{path}'.");
        }

        private void BuildResultsFileOutputHeader(
            StringBuilder stringBuilder,
            MeasurementSystemType measurementSystem)
        {
            stringBuilder.AppendLine("sep =,");
            stringBuilder.AppendLine(
                Resources.LabelFarm + ", " +
                Resources.LabelField + ", " +
                Resources.LabelTimePeriod + ", " +
                Resources.LabelYear + ", " +
                Resources.LabelNotes + ", " +
                Resources.LabelCrop + ", " +
                Resources.LabelYield + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsPerHectareCropWetWeight) + "," +
                Resources.LabelMoistureContent +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) +
                "," +
                Resources.LabelHarvestMethod + "," +
                Resources.LabelTillageType + "," +
                Resources.LabelAmountOfProductReturnedToSoil +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) +
                "," +
                Resources.LabelAmountOfStrawReturnedToSoil +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) +
                "," +
                Resources.LabelAmountOfRootsReturnedToSoil +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) +
                "," +
                Resources.LabelRelativeCAllocationCoefficientProduct + "," +
                Resources.LabelRelativeCAllocationCoefficientStraw + "," +
                Resources.LabelRelativeCAllocationCoefficientRoots + "," +
                Resources.LabelRelativeCAllocationCoefficientExtraroots + "," +
                Resources.LabelClimateFactor + "," +
                Resources.LabelTillageFactor + "," +
                Resources.LabelManagementFactor + "," +
                Resources.LabelGrowingSeasonPrecipitation +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.MillimetersPerYear) + "," +
                Resources.LabelGrowingSeasonEvapotranspiration +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.MillimetersPerYear) + "," +
                Resources.LabelFractionOfLandOccupied + "," +
                Resources.LabelWeightedModifierBasedOnTexture + "," +
                Resources.LabelPlantCarbonInProduct +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelCarbonInputFromProduct +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelCarbonInputFromStraw +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelCarbonInputFromRoots +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelCarbonInputFromExtraroots +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelAverageSoilCarbonAllFieldsInFarm +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelMeasuredSoilCarbon +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelSoilCarbonRMSE +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelSoilCarbonMAE +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelCalculatedCarbonDifferenceFromStartYear +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelMeasuredCarbonDifferenceFromStartYear +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelChangeInSoilCarbon +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelAboveGroundCarbonInput +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelBelowGroundCarbonInput +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelManureCarbonInput +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelDigestateCarbonInput +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelTotalCarbonInput +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelTFac + "," +
                Resources.LabelWFac + "," +
                Resources.January + " " + Resources.LabelTemperatureEffect + "," +
                Resources.February + " " + Resources.LabelTemperatureEffect + "," +
                Resources.March + " " + Resources.LabelTemperatureEffect + "," +
                Resources.April + " " + Resources.LabelTemperatureEffect + "," +
                Resources.May + " " + Resources.LabelTemperatureEffect + "," +
                Resources.June + " " + Resources.LabelTemperatureEffect + "," +
                Resources.July + " " + Resources.LabelTemperatureEffect + "," +
                Resources.August + " " + Resources.LabelTemperatureEffect + "," +
                Resources.September + " " + Resources.LabelTemperatureEffect + "," +
                Resources.October + " " + Resources.LabelTemperatureEffect + "," +
                Resources.November + " " + Resources.LabelTemperatureEffect + "," +
                Resources.December + " " + Resources.LabelTemperatureEffect + "," +
                Resources.January + " " + Resources.LabelWaterEffect + "," +
                Resources.February + " " + Resources.LabelWaterEffect + "," +
                Resources.March + " " + Resources.LabelWaterEffect + "," +
                Resources.April + " " + Resources.LabelWaterEffect + "," +
                Resources.May + " " + Resources.LabelWaterEffect + "," +
                Resources.June + " " + Resources.LabelWaterEffect + "," +
                Resources.July + " " + Resources.LabelWaterEffect + "," +
                Resources.August + " " + Resources.LabelWaterEffect + "," +
                Resources.September + " " + Resources.LabelWaterEffect + "," +
                Resources.October + " " + Resources.LabelWaterEffect + "," +
                Resources.November + " " + Resources.LabelWaterEffect + "," +
                Resources.December + " " + Resources.LabelWaterEffect + "," +
                "FracNLeach" + "," +
                "N2O_EF_SN" + "," +
                "N2O_EF_CR" + "," +
                "N2O_EF_ON" + "," +
                Resources.LabelAboveGroundNitrogenResidueForCrop + "," +
                Resources.LabelBelowGroundResidueNitrogenForCrop + "," +
                Resources.Beta_B_N + "," +
                Resources.Alpha_a_N + "," +
                Resources.TitleActivePoolDecayRate + "," +
                Resources.TitleSlowPoolDecayRate + "," +
                Resources.TitlePassivePoolDecayRate + "," +
                Resources.TitleActivePoolSteadyState + "," +
                Resources.TitleActivePoolN + "," +
                Resources.TitleSlowPoolSteadyState + "," +
                Resources.TitleSlowPoolN + "," +
                Resources.TitlePassivePoolSteadyState + "," +
                Resources.TitlePassivePoolN + "," +
                Resources.N_min + "," +
                Resources.SocNRequirement + "," +
                Resources.LabelActivePool + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelPassivePool +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelSlowPool + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelYoungPoolAboveGround +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelYoungPoolBelowGround +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelOldPoolSoilCarbon +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Resources.LabelSyntheticInputsBeforeAdjustment +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                //Properties.Resources.LabelAboveGroundNitrogenResidueForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                //Properties.Resources.LabelBelowGroundResidueNitrogenForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                "Young Pool Aboveground Residue" + "," +
                "Young Pool Belowground Residue" + "," +
                Resources.LabelAboveGroundResiduePool_AGresidueN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Resources.LabelBelowGroundResiduePool_BGresidueN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Resources.LabelCropResiduesBeforeAdjustment +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Resources.LabelManureResiduePool_ManureN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelCropNitrogenDemand +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelN_min_FromDecompositionOfOldCarbon +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelOldPoolNitrogenRequirement +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobePoolAfterCloseOfBudget +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobePoolAfterOldPoolDemandReduction +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobePoolAfterCropDemandReduction +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMineralNitrogenPool_N_mineralN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobeNitrogenPool_N_microbeN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMineralNitrogenBalance +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobialNitrogenBalance +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelTotalNitrogenInputs +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelTotalNitrogenEmissions +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelTotalUptake +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelTotalNitrogenOutputs +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelDifferenceBetweenInputsAndOutputs +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelMicrobeDeath +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelSumOfMineralAndMicrobialPools +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelOverflow +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelRatio +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Resources.LabelTotalDirectN2O +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Resources.LabelTotalDirectN2OExcludingRemainingAmounts +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Resources.LabelDirectNitrousOxideEmissionsFromSyntheticN +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelDirectNitrousOxideEmissionsFromCropResidues +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelDirectNitrousOxideEmissionsFromNMinealization +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelDirectNitrousOxideEmissionsFromOrganicNitrogen +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelDirectNitrousOxideEmissionsFromOrganicNitrogenExcludingRemainingAmounts +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelTotalIndirectN2O +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Resources.LabelTotalIndirectN2OExcludingRemainingAmounts +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Resources.LabelIndirectNitrousOxideFromSyntheticNitrogenLeaching +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                //Properties.Resources.LabelIndirectNitrousOxideFromCropResidueLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromMineralLeaching +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromOrganicNLeaching +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromOrganicNLeachingExcludingRemaining +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromSyntheticNVolatilization +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromOrganicNVolatilization +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelIndirectNitrousOxideFromOrganicNVolatilizationNotIncludingRemainingAmounts +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem,
                    MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Resources.LabelEnergyCO2 +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Kilograms) +
                "," +
                Resources.LabelUpstreamCarbonDioxide +
                _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Kilograms) +
                ","
            );
        }

        /// <summary>
        ///     Ensure ordering of output matches ordering of headers
        /// </summary>
        private void BuildResultsFileOutputRow(StringBuilder stringBuilder,
            CropViewItem viewItem,
            CultureInfo culture,
            MeasurementSystemType measurementSystem,
            Farm farm,
            bool exportedFromGui)
        {
            var fieldName = string.Empty;
            fieldName = viewItem.Name;
            const string DefaultDecimalOutputFormat = "F4";

            // Old farm files used the FieldName property which is deprecated
            if (string.IsNullOrWhiteSpace(fieldName)) fieldName = viewItem.FieldName;

            #region stringBuilder

            stringBuilder.Append(string.Format("\"{0}\",", farm.Name?.ToString(culture)));
            stringBuilder.Append(string.Format("\"{0}\",", fieldName?.ToString(culture)));
            stringBuilder.Append(string.Format("\"{0}\",", viewItem.TimePeriodCategoryString.ToString(culture)));
            stringBuilder.Append(string.Format("\"{0}\",", viewItem.Year.ToString(culture)));
            stringBuilder.Append(string.Format("\"{0}\",", viewItem.Description?.ToString(culture)));
            stringBuilder.Append(string.Format("\"{0}\",", viewItem.CropTypeString.ToString(culture)));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.ConvertCropTypeValue(measurementSystem, viewItem.CropType, viewItem.Yield).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MoistureContentOfCropPercentage.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",", $"{viewItem.HarvestMethodString}"));
            stringBuilder.Append(string.Format("\"{0}\",", $"{viewItem.TillageTypeString}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.PercentageOfProductYieldReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.PercentageOfStrawReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.PercentageOfRootsReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.BiomassCoefficientProduct.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.BiomassCoefficientStraw.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.BiomassCoefficientRoots.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.BiomassCoefficientExtraroot.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.ClimateParameter.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.TillageFactor.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.ManagementFactor.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, farm.ClimateData.GetGrowingSeasonPrecipitation(viewItem.Year), exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, farm.ClimateData.GetGrowingSeasonEvapotranspiration(viewItem.Year), exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.FractionOfLandOccupiedByLowerPortionsOfLandscape.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.WeightedModifierBasedOnTexture.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.PlantCarbonInAgriculturalProduct, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromProduct, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromStraw, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromRoots, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromExtraroots, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.AverageSoilCarbonAcrossAllFieldsInFarm, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ActualMeasuredSoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SoilCarbonRootMeanSquareError, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.MeanAbsoluteError, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.DifferenceOfCurrentYearCalculatedCarbonAndStartYearCalculatedCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.DifferenceOfCurrentYearMeasuredCarbonAndStartYearMeasuredCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ChangeInCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.AboveGroundCarbonInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.BelowGroundCarbonInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedManureInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedDigestateInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.TotalCarbonInputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.TFac, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.WFac, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.January.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.February.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.March.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.April.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.May.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.June.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.July.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.August.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.September.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.October.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.November.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2TemperatureFactors.December.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.January.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.February.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.March.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.April.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.May.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.June.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.July.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.August.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.September.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.October.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.November.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MonthlyIpccTier2WaterFactors.December.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.FractionOfNitrogenLostByLeachingAndRunoff.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",", $"{viewItem.EF_SN.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",", $"{viewItem.EF_CRN.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",", $"{viewItem.EF_ON.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.CombinedAboveGroundResidueNitrogen.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.CombinedBelowGroundResidueNitrogen.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.Beta.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.Alpha.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.ActivePoolDecayRate.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.SlowPoolDecayRate.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.PassivePoolDecayRate.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.ActivePoolSteadyState.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.ActivePool.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.SlowPoolSteadyState.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.SlowPool.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.PassivePoolSteadyState.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.IpccTier2NitrogenResults.PassivePool.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.MineralPool.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.SocNRequirement.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ActivePoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.PassivePoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SlowPoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonAboveGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonBelowGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.OldPoolSoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.SyntheticInputsBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundNitrogenResidueForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResidueNitrogenForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.YoungPoolAboveGroundResidueN.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{viewItem.YoungPoolBelowGroundResidueN.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundResiduePool_AGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResiduePool_BGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.CropResiduesBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.ManureResiduePool_ManureN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.CropNitrogenDemand, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.N_min_FromDecompositionOfOldCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.OldPoolNitrogenRequirement, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterCloseOfBudget, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterOldPoolDemandAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterCropDemandAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MineralNitrogenPool_N_mineralN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobeNitrogenPool_N_microbeN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MineralNitrogenBalance, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialNitrogenBalance, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenInputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalUptake, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenOutputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.DifferenceBetweenInputsAndOutputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobeDeath, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.SumOfMineralAndMicrobialPools, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.Overflow, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.Ratio, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalDirectNitrousOxidePerHectare, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalIndirectNitrousOxidePerHectare, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideLeachingEmissionsFromOrganicNitrogenForAreaExcludingRemainingManure, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForAreaExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.Kilograms, viewItem.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(string.Format("\"{0}\",",
                $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.Kilograms, viewItem.CropEnergyResults.TotalUpstreamCroppingEnergyEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.AppendLine();

            #endregion
        }
    }
}