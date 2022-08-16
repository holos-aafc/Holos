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

                Properties.Resources.LabelYoungPoolAboveGround + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +
                Properties.Resources.LabelYoungPoolBelowGround + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelOldPoolSoilCarbon + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare) + "," +

                Properties.Resources.LabelSyntheticInputsBeforeAdjustment + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen) + "," +
                Properties.Resources.LabelAboveGroundNitrogenResidueForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Properties.Resources.LabelBelowGroundResidueNitrogenForCrop + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Properties.Resources.LabelAboveGroundResiduePool_AGresidueN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Properties.Resources.LabelBelowGroundResiduePool_BGresidueN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
                Properties.Resources.LabelCropResiduesBeforeAdjustment + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare) + "," +
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

                Properties.Resources.LabelTotalDirectNitrousOxide + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromSyntheticN + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromCropResidues + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromNMinealization + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelDirectNitrousOxideEmissionsFromOrganicNitrogen + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +

                Properties.Resources.LabelTotalIndirectNitrousOxide + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromSyntheticNitrogenLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromCropResidueLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromMineralLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromOrganicNLeaching + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +
                Properties.Resources.LabelIndirectNitrousOxideFromSyntheticNVolatilization + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare) + "," +

                Properties.Resources.LabelTotalNitricOxide + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNONPerHectare) + "," +
                Properties.Resources.LabelTotalNitrateLeachingForArea + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNO3NPerHectare) + "," +
                Properties.Resources.LabelTotalAmmoniaForArea + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsNH4NPerHectare) + "," +

                Properties.Resources.LabelDenitrificationForArea + _unitsCalculator.GetUnitsOfMeasurementString(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2NPerHectare)
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
            const string DefaultDecimalOutputFormat = "F2";

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

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientProduct.ToString("F3")}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientStraw.ToString("F3")}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientRoots.ToString("F3")}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.BiomassCoefficientExtraroot.ToString("F3")}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.ClimateParameter.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.TillageFactor.ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{viewItem.ManagementFactor.ToString(DefaultDecimalOutputFormat)}"));

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

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.AboveGroundCarbonInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.BelowGroundCarbonInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsPerYear, viewItem.ManureCarbonInput, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonAboveGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.YoungPoolSoilCarbonBelowGround, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsCarbonPerHectare, viewItem.OldPoolSoilCarbon, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, viewItem.SyntheticInputsBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundNitrogenResidueForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResidueNitrogenForCrop, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.AboveGroundResiduePool_AGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.BelowGroundResiduePool_BGresidueN, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare, viewItem.CropResiduesBeforeAdjustment, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
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

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.TotalDirectNitrousOxideForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.DirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.TotalIndirectNitrousOxideForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.IndirectNitrousOxideEmissionsFromCropResiduesForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2ONPerHectare, viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNONPerHectare, viewItem.TotalNitricOxideForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNO3NPerHectare, viewItem.TotalNitrateLeachingForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));
            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsNH4NPerHectare, viewItem.TotalAmmoniaForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.Append(String.Format("\"{0}\",", $"{_unitsCalculator.GetUnitsOfMeasurementValue(measurementSystem, MetricUnitsOfMeasurement.KilogramsN2NPerHectare, viewItem.DenitrificationForArea, exportedFromGui).ToString(DefaultDecimalOutputFormat)}"));

            stringBuilder.AppendLine();
            #endregion
        }
    }
}