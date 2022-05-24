using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const double IrrigationConversion = 367;

        #endregion

        #region Public Methods

        public List<MonthlyManureSpreadingResults> GetManureSpreadingResults(
            FieldSystemComponent fieldSystemComponent,
            Farm farm)
        {
            var result = new List<MonthlyManureSpreadingResults>();

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                var totalVolume = manureApplicationViewItem.AmountOfManureAppliedPerHectare * viewItem.Area;
                var month = manureApplicationViewItem.DateOfApplication.Month;
                var year = manureApplicationViewItem.DateOfApplication.Year;

                var totalEnergyEmissions = this.CalculateManureSpreadingEmissions(
                    volumeOfLandAppliedManure: totalVolume);

                var resultItem = new MonthlyManureSpreadingResults();
                resultItem.Year = year;
                resultItem.Month = month;
                resultItem.TotalEmissions = totalEnergyEmissions;

                result.Add(resultItem);
            }

            return result;
        }

        /// <summary>
        /// Calculates emissions from energy usage for fuel use, herbicide, N and P fertilizer, and irrigation.
        ///
        /// <remarks>Both upstream and on-farm emissions are calculated here.</remarks>
        /// </summary>
        public CropEnergyResults CalculateCropEnergyResults(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var results = new CropEnergyResults();
            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null)
            {
                return results;
            }

            // No fuel is used on grasslands/pasture
            if (viewItem.CropType == CropType.SeededGrassland == false)
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
            results.UpstreamEnergyFromNitrogenFertilizer = this.CalculateUpstreamEnergyEmissionsFromSyntheticNitrogenFertilizer(viewItem);

            // Application (on-farm) emissions
            results.EnergyCarbonDioxideFromNitrogenFertilizer = this.CalculateOnFarmEnergyEmissionsFromNitrogenFertilizer(viewItem);

            results.EnergyCarbonDioxideFromPhosphorusFertilizer = this.CalculateEnergyEmissionsFromPhosphorusFertilizer(viewItem);
            results.EnergyCarbonDioxideFromPotassiumFertilizer = this.CalculateEnergyEmissionsFromPotassiumFertilizer(viewItem);
            results.EnergyCarbonDioxideFromSulphurFertilizer = 0;// No methodology yet but we have amounts from currently available fertilizer blends
            results.EnergyCarbonDioxideFromLimeUse = this.CalculateEnergyEmissionsFromLimeFertilizer(viewItem);

            if (viewItem.AmountOfIrrigation > 0)
            {
                results.EnergyCarbonDioxideFromIrrigation = this.CalculateTotalCarbonDioxideEmissionsFromIrrigation(
                    areaOfCropIrrigated: fieldSystemComponent.FieldArea,
                    irrigationConversion: IrrigationConversion,
                    pumpEmissionsFactor: farm.Defaults.PumpEmissionFactor);
            }

            var manureSpreadingResults = this.GetManureSpreadingResults(
                fieldSystemComponent: fieldSystemComponent,
                farm: farm);

            results.ManureSpreadingResults.AddRange(manureSpreadingResults);

            return results;
        }

        /// <summary>
        /// Equation 4.1.1-1
        /// Equation 4.1.1-2
        /// </summary>
        /// <param name="energyFromFuelUse">Energy from fuel use (GJ ha^1)</param>
        /// <param name="area">Area (ha))</param>
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
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <returns>CO2 emissions from N fertilizer production (kg CO2 year^-1)</returns>
        public double CalculateUpstreamEnergyEmissionsFromSyntheticNitrogenFertilizer(CropViewItem viewItem)
        {
            var upstreamEmissions = 0.0;

            // There are no upstream emissions associated with organic fertilizer applications
            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend != FertilizerBlends.CustomOrganic))
            {
                var amountOfNitrogenApplied = fertilizerApplicationViewItem.AmountOfNitrogenApplied;
                var area = viewItem.Area;
                var gateEmissions = fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate;

                upstreamEmissions += amountOfNitrogenApplied * area * gateEmissions;
            }

            return upstreamEmissions;
        }

        /// <summary>
        /// Equation 6.1.3-2
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <returns>CO2 emissions from N fertilizer production (kg CO2 year^-1)</returns>
        public double CalculateOnFarmEnergyEmissionsFromNitrogenFertilizer(CropViewItem viewItem)
        {
            var onFarmEmissions = 0.0;

            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems)
            {
                var amountOfNitrogenApplied = fertilizerApplicationViewItem.AmountOfNitrogenApplied;
                var area = viewItem.Area;
                var applicationEmissions = fertilizerApplicationViewItem.FertilizerBlendData.ApplicationEmissions;

                onFarmEmissions += amountOfNitrogenApplied * area * applicationEmissions;
            }

            return onFarmEmissions;
        }

        /// <summary>
        /// Equation 6.1.3-3
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <returns>CO2 emissions from P fertilizer production (kg CO2 year^-1)</returns>
        public double CalculateEnergyEmissionsFromPhosphorusFertilizer(CropViewItem viewItem)
        {
            var result = 0.0;

            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems)
            {
                var amountOfPhosphorusApplied = fertilizerApplicationViewItem.AmountOfPhosphorusApplied;
                var area = viewItem.Area;
                var gateEmissions = fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate;

                result += amountOfPhosphorusApplied * area * gateEmissions;
            }

            return result;
        }

        /// <summary>
        /// Equation 6.1.3-4
        /// </summary>
        /// <param name="viewItem">The crop details for the year</param>
        /// <returns>CO2 emissions from K fertilizer production (kg CO2 year^-1)</returns>
        private double CalculateEnergyEmissionsFromPotassiumFertilizer(CropViewItem viewItem)
        {
            var result = 0.0;

            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems)
            {
                var amountOfPotassiumApplied = fertilizerApplicationViewItem.AmountOfPotassiumApplied;
                var area = viewItem.Area;
                var gateEmissions = fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate;

                result += amountOfPotassiumApplied * area * gateEmissions;
            }

            return result;
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
        /// <param name="irrigationConversion">Conversion of area irrigated to kg CO2 (kg CO2 ha^1)</param>
        /// <returns>Total CO2 emissions from irrigation (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromIrrigation(
               double areaOfCropIrrigated, 
               double irrigationConversion,
               double pumpEmissionsFactor)
        {
            return areaOfCropIrrigated * irrigationConversion * pumpEmissionsFactor;
        }

        /// <summary>
        /// Equation 4.1.5-1
        /// </summary>
        /// <param name="carbonDioxideEmissionsFromCroppingFuelUse">CO2emissions from cropping/fallow fuel use (kg CO2year^-1)</param>
        /// <param name="carbonDioxideEmissionsFromCroppingHerbicideProduction">CO2emissions from cropping/fallow herbicide production (kg CO2year^-1)</param>
        /// <param name="NFertilizerProduction">CO2emissions from N fertilizer production (kg CO2year^-1)</param>
        /// <param name="phosphorusPentoxideFertilizerProduction">CO2emissions from P2O5 fertilizer production (kg CO2year^-1)</param>
        /// <returns>CO2 emissions from cropping energy use (kg CO2year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromCroppingEnergyUse(
            double carbonDioxideEmissionsFromCroppingFuelUse,
            double carbonDioxideEmissionsFromCroppingHerbicideProduction,
            double NFertilizerProduction,
            double phosphorusPentoxideFertilizerProduction)
        {
            return carbonDioxideEmissionsFromCroppingFuelUse +
                   carbonDioxideEmissionsFromCroppingHerbicideProduction +
                   NFertilizerProduction +
                   phosphorusPentoxideFertilizerProduction;
        }

        /// <summary>
        /// Equation 4.4.1-1
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromFallowingFuelUse"></param>
        /// <param name="totalCarbonDioxideEmissionsFromCroppingHerbicideProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromFallowHerbicideProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromP2O5FertilizerProduction"></param>
        /// <param name="totalCarbonDioxideEmissionsFromIrrigation"></param>
        /// <returns></returns>
        public double CalculateTotalCarbonDioxideEmissionsFromCroppingEnergyUse(double totalCarbonDioxideEmissionsFromFallowingFuelUse,
            double totalCarbonDioxideEmissionsFromCroppingHerbicideProduction,
            double totalCarbonDioxideEmissionsFromFallowHerbicideProduction,
            double totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction,
            double totalCarbonDioxideEmissionsFromP2O5FertilizerProduction,
            double totalCarbonDioxideEmissionsFromIrrigation)
        {
            return totalCarbonDioxideEmissionsFromFallowingFuelUse +
                   totalCarbonDioxideEmissionsFromCroppingHerbicideProduction +
                   totalCarbonDioxideEmissionsFromFallowHerbicideProduction +
                   totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction +
                   totalCarbonDioxideEmissionsFromP2O5FertilizerProduction +
                   totalCarbonDioxideEmissionsFromIrrigation;
        } 

        #endregion
    }
}