using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.Results;
using H.Core.Providers.Animals;
using H.Core.Providers.Economics;
using H.Core.Services;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Calculators.Economics
{
    public class EconomicsCalculator : EconomicsHelper
    {
        #region Fields

        private readonly CropEconomicsProvider _provider = new CropEconomicsProvider();
        private readonly UnitsOfMeasurementCalculator _unitsCalculator = new UnitsOfMeasurementCalculator();
        private readonly IFieldResultsService _fieldResultsService;

        private const double MetricFixedHerbicideCost = 3.52;
        private const double MetricVariableHerbicideCost = 3.06;
        private const double MetricLabourCostOfHerbicide = 0.57;

        #endregion

        #region Constructors

        public EconomicsCalculator()
        {
        }

        public EconomicsCalculator(IFieldResultsService fieldResultsService)
        {
            if (fieldResultsService != null)
            {
                _fieldResultsService = fieldResultsService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldResultsService));
            }
        }

        #endregion

        #region Properties

        public IEnumerable<EconomicsResultsViewItem> EconomicViewItems { get; set; }
        public MeasurementSystemType MeasurementSystem { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write economic data to a file
        /// </summary>
        /// <param name="farm">the farm with the economics data</param>
        /// <param name="path">path of the file to write to</param>
        /// <param name="exportFromGui"></param>
        /// <param name="applicationData"></param>
        /// <param name="languageAddon"></param>
        public bool ExportEconomicsDataToFile(Farm farm, string path, bool exportFromGui,
                                              ApplicationData applicationData,
                                              string languageAddon = null)
        {
            this.MeasurementSystem = farm.MeasurementSystemType;

            var strBuilder = new StringBuilder();
            this.BuildFileContents(farm, strBuilder, applicationData);

            if (!exportFromGui)
            {
                //need  a special path for the CLI outputs
                path = $"{path}{farm.Name}_{Properties.Resources.Economics}_{Properties.Resources.Output}{languageAddon}.csv";
            }
            try
            {
                File.WriteAllText(path, strBuilder.ToString(), Encoding.UTF8);
            }
            catch (IOException e)
            {
                Trace.TraceError($"{nameof(EconomicsCalculator)}.{nameof(ExportEconomicsDataToFile)}: error occurred {e.Message}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the  total profit of all the profits in the list of <see cref="EconomicsResultsViewItem"/>.
        /// </summary>
        /// <param name="economicsResultsViewItems">List of  all the <see cref="EconomicsResultsViewItem"/> to be displayed in the results.</param>
        /// <returns>total profit for all items</returns>
        public double GetTotalProfit(IEnumerable<EconomicsResultsViewItem> economicsResultsViewItems)
        {
            return economicsResultsViewItems.Sum(item => item.Profit);
        }

        /// <summary>
        /// Get the sum total of all the revenues in the list of <see cref="EconomicsResultsViewItem"/>
        /// </summary>
        /// <param name="economicsResultsViewItems">the list of view items to display in the results</param>
        /// <returns>total sum of all the revenues in the list of view items</returns>
        public double GetTotalRevenues(IEnumerable<EconomicsResultsViewItem> economicsResultsViewItems)
        {
            return economicsResultsViewItems.Sum(item => item.Revenues);
        }

        /// <summary>
        /// Calculate the profit for a single <see cref="EconomicsResultsViewItem"/>
        /// </summary>
        /// <param name="economicResultsViewItem">the EconomicsResultsViewItem in question</param>
        /// <param name="measurementType">the farm's units of measurement</param>
        /// <returns>the profit for the current EconomicsResultsViewItem</returns>
        public void CalculateFieldComponentsProfit(EconomicsResultsViewItem economicResultsViewItem,
            MeasurementSystemType measurementType)
        {
            economicResultsViewItem.TotalFixedCost = this.CalculateTotalFixedCost(economicResultsViewItem.CropEconomicData.TotalFixedCostPerUnit, economicResultsViewItem.Area);
            economicResultsViewItem.TotalVariableCost = this.CalculateTotalVariableCost(economicResultsViewItem.CropEconomicData.TotalVariableCostPerUnit, economicResultsViewItem.Area);

            //units have been converted in the previous calcs so no need to convert here
            economicResultsViewItem.TotalCost = economicResultsViewItem.TotalFixedCost + economicResultsViewItem.TotalVariableCost;

            economicResultsViewItem.Profit = economicResultsViewItem.Revenues - economicResultsViewItem.TotalCost;
        }

        /// <summary>
        /// Calculate the results to display in the economics view
        /// </summary>
        /// <param name="fieldResultsService">the <see cref="IFieldResultsService"/> to get the emission results</param>
        /// <param name="farm">the current farm</param>
        /// <returns>list of <see cref="EconomicsResultsViewItem"/></returns>
        public List<EconomicsResultsViewItem> CalculateCropResults(IFieldResultsService fieldResultsService, Farm farm)
        {
            var result = new List<EconomicsResultsViewItem>();

            var fieldComponentEmissionResults = fieldResultsService.CalculateResultsForFieldComponent(farm);

            foreach (var emissionResults in fieldComponentEmissionResults)
            {
                var singleYearViewItem = emissionResults.FieldSystemComponent.GetSingleYearViewItem();
                if (singleYearViewItem == null) // Will be null when component has no view items (e.g. user removed all items)
                {
                    continue;
                }

                if (singleYearViewItem.CropEconomicData == null)
                {
                    Trace.TraceError($"{nameof(EconomicsCalculator)}.{nameof(CalculateCropResults)}: {nameof(CropEconomicData)} is null for {singleYearViewItem.CropType.GetDescription()}");
                    continue;
                }

                if (singleYearViewItem.CropEconomicData.IsUserDefined)
                {
                    singleYearViewItem.CropEconomicData.SetUserDefinedVariableCostPerUnit();

                    //set only once
                    if (!singleYearViewItem.CropEconomicData.FixedCostHandled)
                    {
                        singleYearViewItem.CropEconomicData.SoilFunctionalCategory = farm.DefaultSoilData
                            .SoilFunctionalCategory.GetBaseSoilFunctionalCategory();
                        singleYearViewItem.CropEconomicData.SetUserDefinedFixedCostPerUnit(farm.MeasurementSystemType);
                        singleYearViewItem.CropEconomicData.FixedCostHandled = true;
                    }
                }

                var resultsViewItem = new EconomicsResultsViewItem();
                resultsViewItem.CropEconomicData = singleYearViewItem.CropEconomicData;
                resultsViewItem.Farm = farm;
                resultsViewItem.CropViewItem = singleYearViewItem;
                resultsViewItem.Name = emissionResults.FieldSystemComponent.Name + " (" + singleYearViewItem.CropTypeString + ")";
                resultsViewItem.Component = emissionResults.FieldSystemComponent;
                resultsViewItem.GroupingString = Properties.Resources.TitleCrops;
                resultsViewItem.Harvest = emissionResults.SingleYearHarvest;
                resultsViewItem.Area = emissionResults.FieldSystemComponent.FieldArea;

                this.CalculateRevenues(resultsViewItem);
                this.CalculateFieldComponentsProfit(resultsViewItem, farm.MeasurementSystemType);

                result.Add(resultsViewItem);

                resultsViewItem.PropertyChanged -= ResultsViewItemOnPropertyChanged;
                resultsViewItem.PropertyChanged += ResultsViewItemOnPropertyChanged;
            }

            this.EconomicViewItems = result;
            return result;
        }

        /// <summary>
        /// Check the CropEconomicProvider cache for data on a province.
        /// </summary>
        /// <param name="province">the province in question</param>
        /// <returns>true if the province exists, false otherwise.</returns>
        public bool HasEconDataForProvince(Province province)
        {
            return _provider.HasDataForProvince(province);
        }

        /// <summary>
        /// Check the CropEconomicProvider's cache a specific crop.
        /// </summary>
        /// <param name="cropType">the crop in question</param>
        /// <returns>true if crop exists in the cache, false otherwise.</returns>
        public bool HasEconDataForCropType(CropType cropType)
        {
            return _provider.HasDataForCropType(cropType);
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 1.0 
        /// </summary>
        /// <param name="expectedMarketPrice">price in $/kg or $/bu</param>
        /// <param name="harvest">harvest of the crop in kg or bu</param>
        /// <returns>revenue</returns>
        public double CalculateRevenue(double expectedMarketPrice, double harvest)
        {
            var revenue = Math.Round(expectedMarketPrice, 2) * harvest;
            return revenue;
        }

        /// <summary>
        /// Equation 1.1
        /// </summary>
        /// <param name="fixedCostsPerUnitArea">fixed costs assumed to be in $/acre coming from the 'crop_economics.csv'</param>
        /// <param name="area">area of the field in ac or ha</param>
        /// <returns>fixed costs</returns>
        public double CalculateTotalFixedCost(double fixedCostsPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(fixedCostsPerUnitArea, area);
        }

        /// <summary>
        /// Equation. 1.2
        /// </summary>
        /// <param name="irrigatedFixedCostsPerUnitArea">fixed costs assumed to be in $/acre coming from the 'crop_economics.csv'</param>
        /// <param name="area">area of the field in matching units</param>
        /// <returns>fixed costs for irrigated land</returns>
        public double CalculateIrrigatedFixedCosts(double irrigatedFixedCostsPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(irrigatedFixedCostsPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.3
        /// </summary>
        /// <param name="variabelCostsPerUnitArea">variable costs assumed to be in $/acre coming from the 'crop_economics.csv'</param>
        /// <param name="area">area of field in ac or ha</param>
        /// <returns>variable cost</returns>
        public double CalculateTotalVariableCost(double variabelCostsPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(variabelCostsPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.4
        /// </summary>
        /// <param name="variableIrragatedCostsPerUnitArea">variable costs assumed to be in $/acre coming from the 'crop_economics.csv'</param>
        /// <param name="area">area in matching units</param>
        /// <returns>variable cost for irrigation</returns>
        public double CalculateIrrigatedVariableCost(double variableIrragatedCostsPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(variableIrragatedCostsPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.5
        /// </summary>
        /// <param name="variableCostsNitrogenPerTonne">variable costs for nitrogen in $/tonne found in 'crop_economics.csv'</param>
        /// <param name="nitrogenFertilizerRate">fertilizer rate in kg/ha or lb/ac</param>
        /// <param name="area">area in ha or ac</param>
        /// <param name="measurementType">units of measurement matching the area</param>
        /// <returns>nitrogen variable cost</returns>
        public double CalculateNitrogenVariableCost(double variableCostsNitrogenPerTonne,
            double nitrogenFertilizerRate, double area, MeasurementSystemType measurementType)
        {
            if (measurementType == MeasurementSystemType.Metric)
            {
                return (variableCostsNitrogenPerTonne / 1000) * nitrogenFertilizerRate * area;
            }

            // working with imperial units we need to convert them to metric and use the same equation above
            var convertedMetricRate =
                 _unitsCalculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsPerAcre,
                     nitrogenFertilizerRate);
            var convertedMetricArea =
                _unitsCalculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Acres, area);

            return variableCostsNitrogenPerTonne / 1000 * convertedMetricRate * convertedMetricArea;
        }

        /// <summary>
        /// Equation 1.6
        /// </summary>
        /// <param name="variableCostsPhophorusPerTonne">variable costs for phosphorus in $/tonne found in 'crop_economics.csv'</param>
        /// <param name="phosphorusFertilizerRate">fertilizer rate in kg/ha or lb/ac</param>
        /// <param name="area">area of field in ha or ac</param>
        /// <param name="measurementType">units of measurement matching the area</param>
        /// <returns>phosphorus variable cost</returns>
        public double CalculatePhosphorusVariableCost(double variableCostsPhophorusPerTonne,
            double phosphorusFertilizerRate, double area, MeasurementSystemType measurementType)
        {
            if (measurementType == MeasurementSystemType.Metric)
            {
                return variableCostsPhophorusPerTonne / 1000 * phosphorusFertilizerRate * area;
            }

            // working with imperial units we need to convert them to metric and use the same equation above
            var convertedMetricRate =
                 _unitsCalculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.PoundsPerAcre,
                     phosphorusFertilizerRate);
            var convertedMetricArea =
                _unitsCalculator.ConvertValueToMetricFromImperial(ImperialUnitsOfMeasurement.Acres, area);

            return variableCostsPhophorusPerTonne / 1000 * convertedMetricRate * convertedMetricArea;
        }

        /// <summary>
        /// Equation 1.7
        /// </summary>
        /// <param name="variableHerbicideCostPerUnitArea">variable cost for herbicide in $/acre of $/ha</param>
        /// <param name="area">area in matching units</param>
        /// <returns>variable cost for herbicide</returns>
        public double CalculateHerbicideVariableCost(double variableHerbicideCostPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(variableHerbicideCostPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.8
        /// </summary>
        /// <param name="labourCostsPerUnitArea">labour costs in $/acre or $/ha</param>
        /// <param name="area">area in matching units</param>
        /// <returns>labour cost</returns>
        public double CalculateFixedLabourCost(double labourCostsPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(labourCostsPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.9
        /// </summary>
        /// <param name="labourCostsIrrigationPerUnitArea">labour cost for irrigation in $/unit found in 'crop_economics.csv'</param>
        /// <param name="area">area in matching units</param>
        /// <returns>labour cost for irrigation</returns>
        public double CalculateFixedLabourCostIrrigation(double labourCostsIrrigationPerUnitArea, double area)
        {
            return MultiplyCostPerUnitAreaByArea(labourCostsIrrigationPerUnitArea, area);
        }

        /// <summary>
        /// Equation 1.10
        /// </summary>
        /// <param name="numberOfPasses">Number of passes over field</param>
        /// <param name="fixedCostPerUnitArea">fixed cost in $/acre or $/ha</param>
        /// <param name="area">area of field in matching units</param>
        /// <param name="measurementType">units of measurement matching the area</param>
        /// <returns>Fixed cost of NO herbicide</returns>
        public double CalculateNOHerbicideFixedCost(int numberOfPasses, double fixedCostPerUnitArea, double area, MeasurementSystemType measurementType)
        {
            var fixedCost = this.CalculateTotalFixedCost(fixedCostPerUnitArea, area);

            if (measurementType == MeasurementSystemType.Metric)
            {
                return fixedCost - MetricFixedHerbicideCost * numberOfPasses * area;
            }

            //working in imperial
            var imperialFixedHerbicedCost = MetricFixedHerbicideCost / AcresPerHectare;

            return fixedCost - imperialFixedHerbicedCost * numberOfPasses * area;
        }


        /// <summary>
        /// Equation 1.11
        /// </summary>
        /// <param name="variableCostPerUnitArea">variable cost in $/acre should be part of 'crop_economics.csv'</param>
        /// <param name="numberOfPasses">number of passes over field</param>
        /// <param name="area">area of field in matching units</param>
        /// <param name="measurementType">units of measurement matching the area</param>
        /// <returns>variable cost of NO Herbicide</returns>
        public double CalculateNOHerbicideVariableCost(double variableCostPerUnitArea, int numberOfPasses, double area, MeasurementSystemType measurementType)
        {
            var variableCost = this.CalculateTotalVariableCost(variableCostPerUnitArea, area);
            //area is supposed to be in hectares for this.
            if (measurementType == MeasurementSystemType.Metric)
            {
                return variableCost - (MetricVariableHerbicideCost * numberOfPasses * area);
            }

            //working in imperial
            var imperialVariableHerbicideCost = MetricVariableHerbicideCost / AcresPerHectare;

            return variableCost - imperialVariableHerbicideCost * numberOfPasses * area;
        }

        /// <summary>
        /// Equation 1.12
        /// </summary>
        /// <param name="labourCostsPerUnitArea">labour costs in $/acre should be part of 'crop_economics.csv'</param>
        /// <param name="numberOfPasses">number of passes over field</param>
        /// <param name="area">area in matching units</param>
        /// <param name="measurementType">units of measurement system matching the area</param>
        /// <returns>labour costs for NO herbicide</returns>
        public double CalculateNOHerbicideLabourCost(double labourCostsPerUnitArea, int numberOfPasses, double area,
            MeasurementSystemType measurementType)
        {
            var labourCost = this.CalculateFixedLabourCost(labourCostsPerUnitArea, area);

            if (measurementType == MeasurementSystemType.Metric)
            {
                return labourCost - MetricLabourCostOfHerbicide * numberOfPasses * area;
            }

            // working in imperial
            var imperialLabourCostOfHerbicide = MetricLabourCostOfHerbicide / AcresPerHectare;

            return labourCost - imperialLabourCostOfHerbicide * numberOfPasses * area;
        }
        #endregion


        #region Private Methods

        private void BuildFileContents(Farm farm,
                                       StringBuilder strBuilder,
                                       ApplicationData applicationData)
        {
            this.BuildHeaderRow(strBuilder, farm.MeasurementSystemType, applicationData);
            if (!this.EconomicDataExistsForProvinceOrCrop(farm))
            {
                return;
            }

            var resultViewItems = this.CalculateCropResults(_fieldResultsService, farm);
            foreach (var resultViewItem in resultViewItems)
            {
                strBuilder.AppendLine($"{farm.Name}, " +
                    $"{resultViewItem.Name}, " +
                    $"{resultViewItem.CropViewItem.CropTypeString}, " +
                    $"{resultViewItem.Harvest}, " +
                    $"{resultViewItem.CropEconomicData.ExpectedMarketPrice}, " +
                    $"{resultViewItem.Revenues}");
            }
            strBuilder.AppendLine();
            strBuilder.AppendLine($"{Properties.Resources.TotalRevenue}, {GetTotalRevenues(resultViewItems)}");
        }

        private bool EconomicDataExistsForProvinceOrCrop(Farm farm)
        {
            if (!HasEconDataForProvince(farm.DefaultSoilData.Province))
            {
                return false;
            }
            foreach (var component in farm.FieldSystemComponents)
            {
                foreach (var item in component.CropViewItems)
                {
                    if (!HasEconDataForCropType(item.CropType))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CalculateRevenues(EconomicsResultsViewItem resultsViewItem)
        {
            resultsViewItem.Revenues = this.CalculateRevenue(resultsViewItem.CropEconomicData.ExpectedMarketPrice, resultsViewItem.Harvest);
        }

        private void ResultsViewItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is EconomicsResultsViewItem resultsViewItem)
            {
                if (e.PropertyName.Equals(nameof(CropEconomicData.ExpectedMarketPrice)))
                {
                    this.CalculateRevenues(resultsViewItem);
                    this.CalculateFieldComponentsProfit(resultsViewItem, resultsViewItem.Farm.MeasurementSystemType);
                }
            }
        }

        private void BuildHeaderRow(StringBuilder stringbuilder, MeasurementSystemType measurementSystemType, ApplicationData applicationData)
        {
            stringbuilder.AppendLine(
                $"{Properties.Resources.LabelFarm}, " +
                $"{Properties.Resources.LabelField}, " +
                $"{Properties.Resources.LabelCrop }, " +
                $"{Properties.Resources.TitleHarvest} {_unitsCalculator.GetUnitsOfMeasurementString(measurementSystemType, MetricUnitsOfMeasurement.Kilograms)}, " +
                $"{Properties.Resources.TitleExpectedMarketPrice} {applicationData.DisplayUnitStrings.DollarsPerKilogramString}, " +
                $"{Properties.Resources.TitleRevenue} {applicationData.DisplayUnitStrings.DollarsPerHectare}"
                );
        }

        private double MultiplyCostPerUnitAreaByArea(double costPerUnitArea, double area)
        {
            //costPerUnitArea should be converted to the appropriate units by this point
            return costPerUnitArea * area;
        }
        #endregion
    }
}