using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Fields

        private const double DieselConversion = 70;
        private const double HerbicideConversion = 5.8;
        private const double NitrogenFertilizerConversion = 3.59;
        private const double PhosphorusFertilizerConversion = 0.5699;
        private const double PotassiumConversion = 1.0;  // We do not have data for potassium conversion value yet, so temporarily use 1.0 for now.
        private const double IrrigationConversion = 367;

        #endregion

        #region Public Methods

        public List<MonthlyManureSpreadingEmissions> GetManureSpreadingEmissions(
            CropViewItem viewItem,
            Farm farm)
        {
            var result = new List<MonthlyManureSpreadingEmissions>();

            // Get the months in which manure has been spread
            var monthlySpreadingData = _manureService.GetMonthlyManureSpreadingData(viewItem, farm);
            if (monthlySpreadingData.Any() == false)
            {
                var volume = _n2OEmissionFactorCalculator.CalculateVolumeFromLeftOverManureForField(farm, viewItem);
                for (int i = 0; i < 12; i++)
                {
                    var monthlyEmissions = new MonthlyManureSpreadingEmissions()
                    {
                        Month = i + 1,
                        Year = viewItem.Year,
                        TotalVolume = volume / 12.0,
                    };

                    var volumeForMonth = volume / 12.0;
                    var reducedVolume = volumeForMonth / 1000.0;
                    var emissionsForMonth = this.CalculateManureSpreadingEmissions(reducedVolume);

                    monthlyEmissions.TotalVolume = volumeForMonth;
                    monthlyEmissions.TotalEmissions = emissionsForMonth;

                    result.Add(monthlyEmissions);
                }

                return result;
            }

            foreach (var monthlyManureSpreadingData in monthlySpreadingData)
            {
                // In this month, there was manure spread onto the field. Use the volume used during that month to create a new emissions object
                var monthlyEmissions = new MonthlyManureSpreadingEmissions(monthlyManureSpreadingData);

                // This needs to be per 1000 kg/l since manure spreading energy emissions use factors that are in GJ / 1000 kg/l
                var totalVolume = monthlyManureSpreadingData.TotalVolume / 1000;

                // Calculate the total emissions based on the volume of manure spread.
                monthlyEmissions.TotalEmissions = this.CalculateManureSpreadingEmissions(volumeOfLandAppliedManure: totalVolume);

                result.Add(monthlyEmissions);
            }

            // Calculate the amount of manure left over for the entire year associated with this field
            var volumeOfManureLeftOver = _n2OEmissionFactorCalculator.CalculateVolumeFromLeftOverManureForField(farm, viewItem);

            // Assumption is that any remaining amounts will be added to the total volume of manure made in any given month
            var volumeAmountAttributedToEachMonth = volumeOfManureLeftOver / monthlySpreadingData.Count;

            // This needs to be per 1000 kg/l since manure spreading energy emissions use factors that are in GJ / 1000 kg/l
            volumeAmountAttributedToEachMonth /= 1000;

            // Calculate emissions from the application of this left over amount
            var emissionsAttributedToEachMonth = this.CalculateManureSpreadingEmissions(volumeAmountAttributedToEachMonth);

            // Assumption is that all remaining (non-used) amounts are considered for the year
            bool includeRemainingAMounts = true;
            if (includeRemainingAMounts)
            {
                foreach (var monthlyData in result)
                {
                    // Add to existing amounts from actual field applications
                    monthlyData.TotalEmissions += emissionsAttributedToEachMonth;
                }
            }
            return result;
        }

        public CropEnergyResults CalculateCropEnergyResults(CropViewItem viewItem, Farm farm)
        {
            var results = new CropEnergyResults();

            if (viewItem == null)
            {
                return results;
            }

            // No fuel is used on grasslands/pasture
            if (viewItem.CropType.IsNativeGrassland() == false)
            {
                results.EnergyCarbonDioxideFromFuelUse = this.CalculateCarbonDioxideEmissionsFromCroppingFuelUse(
                    energyFromFuelUse: viewItem.FuelEnergy,
                    area: viewItem.Area,
                    dieselConversion: DieselConversion);
            }

            if (viewItem.IsHerbicideUsed)
            {
                // Upstream emissions
                results.UpstreamEnergyCarbonDioxideFromHerbicideUse = this.CalculateCarbonDioxideEmissionsFromCroppingHerbicideProduction(
                    energyForHerbicideProduction: viewItem.HerbicideEnergy,
                    area: viewItem.Area,
                    herbicideConversion: HerbicideConversion);
            }

            // Upstream emissions
            results.UpstreamEnergyFromFertilizerProduction = this.CalculateUpstreamEnergyEmissionsFromFertilizer(viewItem, farm);

            // Application (on-farm) emissions
            results.EnergyCarbonDioxideFromFertilizerUse = this.CalculateOnFarmEnergyEmissionsFromFertilizerUse(viewItem, farm);
            results.EnergyCarbonDioxideFromLimeUse = this.CalculateEnergyEmissionsFromLimeFertilizer(viewItem);

            if (viewItem.AmountOfIrrigation > 0)
            {
                results.EnergyCarbonDioxideFromIrrigation = this.CalculateTotalCarbonDioxideEmissionsFromIrrigation(
                    areaOfCropIrrigated: viewItem.Area,
                    amountOfIrrigation: viewItem.AmountOfIrrigation,
                    pumpEmissionsFactor: farm.Defaults.PumpEmissionFactor);
            }

            var manureSpreadingResults = this.GetManureSpreadingEmissions(
                viewItem: viewItem,
                farm: farm);

            results.ManureSpreadingResults.AddRange(manureSpreadingResults);

            return results;
        }

        /// <summary>
        /// </summary>
        /// <param name="energyFromFuelUse">Energy from fuel use (GJ ha^1)</param>
        /// <param name="area">Area (ha)</param>
        /// <param name="dieselConversion">Conversion of GJ of diesel to kg CO2 (kg CO2 GJ^-1)</param>
        /// <returns>CO2 emissions from cropping fuel use (kg CO2 year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromCroppingFuelUse(
            double energyFromFuelUse, 
            double area, 
            double dieselConversion)
        {
            return energyFromFuelUse * area * dieselConversion;
        }

        /// <summary>
        /// Equation 6.1.2-1
        /// Equation 6.1.2-2
        /// </summary>
        /// <param name="energyForHerbicideProduction">Energy for herbicide production (GJ ha^1)</param>
        /// <param name="area">Area (ha)</param>
        /// <param name="herbicideConversion">Conversion of GJ for herbicide production to kg CO2 (kg CO2 GJ^-1)</param>
        /// <returns>CO2 emissions from cropping herbicide production (kg CO2 year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromCroppingHerbicideProduction(
            double energyForHerbicideProduction,
            double area,
            double herbicideConversion)
        {
            return energyForHerbicideProduction * area * herbicideConversion;
        }

        /// <summary>
        /// Equation 6.3.1-2
        /// </summary>
        public double CalculateManureSpreadingEmissions(
            double volumeOfLandAppliedManure)
        {
            return volumeOfLandAppliedManure * 0.0248 * 70;
        }

        /// <summary>
        /// Equation 6.1.3-1
        /// Equation 6.1.3-3
        /// Equation 6.1.3-4
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <param name="farm"></param>
        /// <returns>CO2 emissions from fertilizer production (kg CO2 year^-1)</returns>
        public double CalculateUpstreamEnergyEmissionsFromFertilizer(CropViewItem viewItem, Farm farm)
        {
            var upstreamEmissions = 0.0;

            // There are no upstream emissions associated with organic fertilizer applications
            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend != FertilizerBlends.CustomOrganic && x.FertilizerBlendData.FertilizerBlend != FertilizerBlends.Lime))
            {
                var blendEmissions = _carbonFootprintForFertilizerBlendsProvider.GetData(fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend);

                var amountOfProduct = fertilizerApplicationViewItem.AmountOfBlendedProductApplied;
                var area = viewItem.Area;
                var gateEmissions = blendEmissions.CarbonDioxideEmissionsAtTheGate;

                upstreamEmissions += amountOfProduct * area * gateEmissions;
            }

            return upstreamEmissions;
        }

        /// <summary>
        /// Equation 6.1.3-2
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <param name="farm"></param>
        /// <returns>CO2 emissions from fertilizer production (kg CO2 year^-1)</returns>
        public double CalculateOnFarmEnergyEmissionsFromFertilizerUse(
            CropViewItem viewItem, 
            Farm farm)
        {
            var onFarmEmissions = 0.0;

            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend != FertilizerBlends.CustomOrganic && x.FertilizerBlendData.FertilizerBlend != FertilizerBlends.Lime))
            {
                var blend = fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend;
                var amountOfNitrogenApplied = fertilizerApplicationViewItem.AmountOfBlendedProductApplied;
                var area = viewItem.Area;
                var blendEmissions = _carbonFootprintForFertilizerBlendsProvider.GetData(blend);

                var emissionFactor = blendEmissions.ApplicationEmissions;
                if (blend.IsNitrogenFertilizer() && farm.Defaults.UseCustomNitrogenFertilizerConversionFactor)
                {
                    emissionFactor = farm.Defaults.NitrogenFertilizerConversionFactor;
                }
                else if (blend.IsPhosphorusFertilizer() && farm.Defaults.UseCustomPhosphorusFertilizerConversionFactor)
                {
                    emissionFactor = farm.Defaults.PhosphorusFertilizerConversionFactor;
                }
                else if (blend.IsPotassiumFertilizer() && farm.Defaults.UseCustomPotassiumConversionFactor)
                {
                    emissionFactor = farm.Defaults.PotassiumConversionFactor;
                }

                onFarmEmissions += amountOfNitrogenApplied * area * emissionFactor;
            }

            return onFarmEmissions;
        }

        /// <summary>
        /// Equation 6.1.3.5
        /// Equation 6.1.3.6
        /// </summary>
        /// <returns>Total CO2 emissions from liming application (kg CO2 year^-1)</returns>
        private double CalculateEnergyEmissionsFromLimeFertilizer(CropViewItem viewItem)
        {
            var result = 0.0;

            const double emissionsFactor = 0.12;

            foreach (var viewItemFertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend == FertilizerBlends.Lime))
            {
                result += (viewItemFertilizerApplicationViewItem.AmountOfBlendedProductApplied * emissionsFactor) * CoreConstants.ConvertFromCToCO2;
            }

            return result;
        }

        /// <summary>
        /// Equation 6.1.4-1
        /// </summary>
        /// <param name="areaOfCropIrrigated">area of crop irrigated (ha)</param>
        /// <param name="amountOfIrrigation">Amount of irrigation (mm ha^-1 yr^-1)</param>
        /// <param name="pumpEmissionsFactor">For electric pump = 0.266 kg CO2 mm^-1, for natural gas pump = 1.145 kg CO2 mm^-1</param>
        /// <returns>Total CO2 emissions from irrigation (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromIrrigation(
               double areaOfCropIrrigated, 
               double amountOfIrrigation,
               double pumpEmissionsFactor)
        {
            return areaOfCropIrrigated * amountOfIrrigation * pumpEmissionsFactor;
        }

        /// <summary>
        /// Equation 4.1.5-1
        /// </summary>
        /// <param name="carbonDioxideEmissionsFromCroppingFuelUse">CO2emissions from cropping/fallow fuel use (kg CO2year^-1)</param>
        /// <param name="carbonDioxideEmissionsFromCroppingHerbicideProduction">CO2emissions from cropping/fallow herbicide production (kg CO2year^-1)</param>
        /// <param name="NFertilizerProduction">CO2emissions from N fertilizer production (kg CO2year^-1)</param>
        /// <param name="phosphorusPentoxideFertilizerProduction">CO2emissions from P2O5 fertilizer production (kg CO2year^-1)</param>
        /// <param name="potassiumProduction">CO2emissions from K2O fertilizer production (kg CO2year^-1)</param>
        /// <returns>CO2 emissions from cropping energy use (kg CO2year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromCroppingEnergyUse(
            double carbonDioxideEmissionsFromCroppingFuelUse,
            double carbonDioxideEmissionsFromCroppingHerbicideProduction,
            double NFertilizerProduction,
            double potassiumProduction,
            double phosphorusPentoxideFertilizerProduction)
        {
            return carbonDioxideEmissionsFromCroppingFuelUse +
                   carbonDioxideEmissionsFromCroppingHerbicideProduction +
                   NFertilizerProduction +
                   potassiumProduction +
                   phosphorusPentoxideFertilizerProduction;
        }

        /// <summary>
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromFallowingFuelUse"></param>
        /// <param name="totalCarbonDioxideEmissionsFromCroppingHerbicideProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromFallowHerbicideProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromP2O5FertilizerProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromPotassiumProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromIrrigation"></param>
        /// <returns></returns>
        public double CalculateTotalCarbonDioxideEmissionsFromCroppingEnergyUse(double totalCarbonDioxideEmissionsFromFallowingFuelUse,
            double totalCarbonDioxideEmissionsFromCroppingHerbicideProduction,
            double totalCarbonDioxideEmissionsFromFallowHerbicideProduction,
            double totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction,
            double totalCarbonDioxideEmissionsFromP2O5FertilizerProduction,
            double totalCarbonDioxideEmissionsFromPotassiumProduction,
            double totalCarbonDioxideEmissionsFromIrrigation)
        {
            return totalCarbonDioxideEmissionsFromFallowingFuelUse +
                   totalCarbonDioxideEmissionsFromCroppingHerbicideProduction +
                   totalCarbonDioxideEmissionsFromFallowHerbicideProduction +
                   totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction +
                   totalCarbonDioxideEmissionsFromP2O5FertilizerProduction +
                   totalCarbonDioxideEmissionsFromPotassiumProduction +
                   totalCarbonDioxideEmissionsFromIrrigation;
        } 

        #endregion
    }
}