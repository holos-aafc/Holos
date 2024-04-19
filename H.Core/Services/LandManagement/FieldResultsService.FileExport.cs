using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        public void ExportAllResultsToFile(
            string path, 
            MeasurementSystemType measurementSystemType, 
            CultureInfo cultureInfo, 
            IEnumerable<CropViewItem> viewItems, 
            Farm farm)
        {
            var stringBuilder = new StringBuilder();

            this.BuildResultsFileOutputHeader(stringBuilder, measurementSystemType);

            foreach (var cropViewItem in viewItems)
            {
                this.BuildResultsFileOutputRow(stringBuilder, cropViewItem, cultureInfo, measurementSystemType, farm, false);
            }

            try
            {
                File.WriteAllText(path, stringBuilder.ToString(), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(this.ExportAllResultsToFile)}: error writing data to csv file: '{exception.Message}'.");

                return;
            }

            Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(this.ExportAllResultsToFile)}: successfully exported data to csv file: '{path}'.");
        }

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

            this.BuildResultsFileOutputHeader(stringBuilder, measurementSystemType);
            string filePath;
            if (exportedFromGui)
            {
                filePath = path.ToString();
            }
            else
            {
                filePath = $"{path}{farm.Name}_{Properties.Resources.Output}{languageAddOn}.csv";
            }

            foreach (var viewItem in results)
            {
                this.BuildResultsFileOutputRow(stringBuilder, viewItem, cultureInfo, measurementSystemType, farm, exportedFromGui);
            }

            try
            {
                if (cultureInfo.Name == Infrastructure.InfrastructureConstants.FrenchCultureInfo.Name)
                {
                    //display the accented chars correctly
                    File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.GetEncoding("iso-8859-1"));
                }
                else
                {
                    File.WriteAllText(filePath, stringBuilder.ToString());
                }
            }
            catch (IOException exception)
            {
                Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(this.ExportResultsToFile)}: error writing data to csv file: '{exception.Message}'.");

                return false;
            }

            Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(this.ExportResultsToFile)}: successfully exported data to csv file: '{filePath}'.");
            return true;
        }

        private void BuildResultsFileOutputHeader(
            StringBuilder stringBuilder,
            MeasurementSystemType measurementSystem)
        {
            stringBuilder.AppendLine("sep =,");
            stringBuilder.AppendLine(

                Properties.Resources.LabelFarm + ", " +
                Properties.Resources.LabelField + ", " +
                Properties.Resources.LabelTimePeriod + ", " +
                Properties.Resources.LabelYear + ", " +
                Properties.Resources.LabelNotes + ", " +
                Properties.Resources.LabelCrop + ", " +

                Properties.Resources.LabelYield + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsPerHectareCrop) + "," +
                Properties.Resources.LabelMoistureContent + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) + "," +
                Properties.Resources.LabelHarvestMethod + "," +
                Properties.Resources.LabelTillageType + "," +
                Properties.Resources.LabelAmountOfProductReturnedToSoil + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) + "," +
                Properties.Resources.LabelAmountOfStrawReturnedToSoil + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) + "," +
                Properties.Resources.LabelAmountOfRootsReturnedToSoil + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Percentage) + "," +

                Properties.Resources.LabelRelativeCAllocationCoefficientProduct + "," +
                Properties.Resources.LabelRelativeCAllocationCoefficientStraw + "," +
                Properties.Resources.LabelRelativeCAllocationCoefficientRoots + "," +
                Properties.Resources.LabelRelativeCAllocationCoefficientExtraroots + "," +

                Properties.Resources.LabelClimateFactor + "," +
                Properties.Resources.LabelTillageFactor + "," +
                Properties.Resources.LabelManagementFactor + "," +
                Properties.Resources.LabelGrowingSeasonPrecipitation + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear) + "," +
                Properties.Resources.LabelGrowingSeasonEvapotranspiration + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear) + "," +
                Properties.Resources.LabelFractionOfLandOccupied + "," +
                Properties.Resources.LabelWeightedModifierBasedOnTexture + "," +

                Properties.Resources.LabelPlantCarbonInProduct + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelCarbonInputFromProduct + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelCarbonInputFromStraw + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelCarbonInputFromRoots + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelCarbonInputFromExtraroots + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelAverageSoilCarbonAllFieldsInFarm + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelMeasuredSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelSoilCarbonRMSE + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelSoilCarbonMAE + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelCalculatedCarbonDifferenceFromStartYear + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelMeasuredCarbonDifferenceFromStartYear + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelChangeInSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelAboveGroundCarbonInput + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelBelowGroundCarbonInput + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelManureCarbonInput + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsPerYear) + "," +
                Properties.Resources.LabelDigestateCarbonInput + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsPerYear) + "," +
                Properties.Resources.LabelTotalCarbonInput + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelTFac + "," +
                Properties.Resources.LabelWFac + "," +

                Properties.Resources.January + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.February + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.March + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.April + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.May + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.June + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.July + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.August + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.September + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.October + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.November + " " + Properties.Resources.LabelTemperatureEffect + "," +
                Properties.Resources.December + " " + Properties.Resources.LabelTemperatureEffect + "," +

                Properties.Resources.January + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.February + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.March + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.April + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.May + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.June + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.July + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.August + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.September + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.October + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.November + " " + Properties.Resources.LabelWaterEffect + "," +
                Properties.Resources.December + " " + Properties.Resources.LabelWaterEffect + "," +

                Properties.Resources.LabelActivePool + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelPassivePool + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelSlowPool + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelYoungPoolAboveGround + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelYoungPoolBelowGround + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelOldPoolSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelSyntheticInputsBeforeAdjustment + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                //Properties.Resources.LabelAboveGroundNitrogenResidueForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                //Properties.Resources.LabelBelowGroundResidueNitrogenForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                //Properties.Resources.LabelAboveGroundResiduePool_AGresidueN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                //Properties.Resources.LabelBelowGroundResiduePool_BGresidueN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                //Properties.Resources.LabelCropResiduesBeforeAdjustment + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Properties.Resources.LabelManureResiduePool_ManureN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelCropNitrogenDemand + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelN_min_FromDecompositionOfOldCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelOldPoolNitrogenRequirement + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobePoolAfterCloseOfBudget + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobePoolAfterOldPoolDemandReduction + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobePoolAfterCropDemandReduction + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMineralNitrogenPool_N_mineralN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobeNitrogenPool_N_microbeN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +

                Properties.Resources.LabelMineralNitrogenBalance + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobialNitrogenBalance + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelTotalNitrogenInputs + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelTotalNitrogenEmissions + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelTotalUptake + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelTotalNitrogenOutputs + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelDifferenceBetweenInputsAndOutputs + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelMicrobeDeath + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelSumOfMineralAndMicrobialPools + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelOverflow + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelRatio + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +

                Properties.Resources.LabelTotalDirectN2O + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Properties.Resources.LabelTotalDirectN2OExcludingRemainingAmounts + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromSyntheticN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromCropResidues + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromNMinealization + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromOrganicNitrogen + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromOrganicNitrogenExcludingRemainingAmounts + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +

                Properties.Resources.LabelTotalIndirectN2O + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Properties.Resources.LabelTotalIndirectN2OExcludingRemainingAmounts + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromSyntheticNitrogenLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                //Properties.Resources.LabelIndirectNitrousOxideFromCropResidueLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromMineralLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromOrganicNLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromOrganicNLeachingExcludingRemaining + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromSyntheticNVolatilization + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromOrganicNVolatilization + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromOrganicNVolatilizationNotIncludingRemainingAmounts + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField) + "," +

                Properties.Resources.LabelEnergyCO2 + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Kilograms) + "," +
                Properties.Resources.LabelUpstreamCarbonDioxide + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.Kilograms) + ","
            );
        }

        /// <summary>
        /// Ensure ordering of output matches ordering of headers
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
            const string DefaultDecimalOutputFormat = "F3";

            // Old farm files used the FieldName property which is deprecated
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                fieldName = viewItem.FieldName;
            }

            #region stringBuilder

            stringBuilder.Append(String.Format("\"{0}\",", farm.Name?.ToString(culture)));
            stringBuilder.Append(String.Format("\"{0}\",", fieldName?.ToString(culture)));
            stringBuilder.Append(String.Format("\"{0}\",", viewItem.TimePeriodCategoryString.ToString(culture)));
            stringBuilder.Append(String.Format("\"{0}\",", viewItem.Year.ToString(culture)));
            stringBuilder.Append(String.Format("\"{0}\",", viewItem.Description?.ToString(culture)));
            stringBuilder.Append(String.Format("\"{0}\",", viewItem.CropTypeString.ToString(culture)));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.ConvertCropTypeValue(measurementSystem, viewItem.CropType, viewItem.Yield).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MoistureContentOfCropPercentage.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.HarvestMethodString}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.TillageTypeString}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.PercentageOfProductYieldReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.PercentageOfStrawReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.PercentageOfRootsReturnedToSoil.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientProduct.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientStraw.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientRoots.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientExtraroot.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.ClimateParameter.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.TillageFactor.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.ManagementFactor.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, farm.ClimateData.GetGrowingSeasonPrecipitation(viewItem.Year), exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.MillimetersPerYear, farm.ClimateData.GetGrowingSeasonEvapotranspiration(viewItem.Year), exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.FractionOfLandOccupiedByLowerPortionsOfLandscape.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.WeightedModifierBasedOnTexture.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.PlantCarbonInAgriculturalProduct, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromProduct, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromStraw, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromRoots, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CarbonInputFromExtraroots, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.AverageSoilCarbonAcrossAllFieldsInFarm, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ActualMeasuredSoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SoilCarbonRootMeanSquareError, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.MeanAbsoluteError, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.DifferenceOfCurrentYearCalculatedCarbonAndStartYearCalculatedCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.DifferenceOfCurrentYearMeasuredCarbonAndStartYearMeasuredCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ChangeInCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedAboveGroundInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedBelowGroundInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedManureInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.CombinedDigestateInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.TotalCarbonInputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.TFac, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.WFac, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.January.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.February.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.March.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.April.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.May.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.June.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.July.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.August.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.September.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.October.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.November.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2TemperatureFactors.December.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.January.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.February.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.March.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.April.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.May.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.June.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.July.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.August.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.September.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.October.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.November.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.MonthlyIpccTier2WaterFactors.December.ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.ActivePoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.PassivePoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.SlowPoolCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonAboveGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonBelowGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.OldPoolSoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.SyntheticInputsBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundNitrogenResidueForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResidueNitrogenForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundResiduePool_AGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResiduePool_BGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.CropResiduesBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.ManureResiduePool_ManureN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.CropNitrogenDemand, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.N_min_FromDecompositionOfOldCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.OldPoolNitrogenRequirement, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterCloseOfBudget, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterOldPoolDemandAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialPoolAfterCropDemandAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MineralNitrogenPool_N_mineralN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobeNitrogenPool_N_microbeN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MineralNitrogenBalance, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobialNitrogenBalance, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenInputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalUptake, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.TotalNitrogenOutputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.DifferenceBetweenInputsAndOutputs, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.MicrobeDeath, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.SumOfMineralAndMicrobialPools, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.Overflow, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.Ratio, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalDirectNitrousOxidePerHectare, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalDirectNitrousOxidePerHectareExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalIndirectNitrousOxidePerHectare, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2OPerHectare, viewItem.TotalIndirectNitrousOxidePerHectareExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            //stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideLeachingEmissionsFromOrganicNitrogenForAreaExcludingRemainingManure, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerField, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfOrganicNitrogenForAreaExcludingRemainingAmounts, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.Kilograms, viewItem.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.Kilograms, viewItem.CropEnergyResults.TotalUpstreamCroppingEnergyEmissions, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.AppendLine();
            #endregion
        }
    }
}