using H.Core.Models;
using System;
using System.Linq;
using H.Core.Models.LandManagement.Fields;
using H.Core.Enumerations;
using System.Collections.Generic;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        /// <returns>Returns the total direct N2O emissions from exported manure (kg N2O-N ha^-1)</returns>
        public double CalculateTotalDirectN2ONFromExportedManureByYear(Farm farm, int year)
        {
            var result = 0d;

            foreach (var manureExportViewItem in farm.ManureExportViewItems.Where(x => x.DateOfExport.Year == year))
            {
                result += this.CalculateTotalDirectN2ONFromExportedManure(farm, manureExportViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 2.6.5-6
        /// Equation 2.7.4-6
        ///
        /// Note: These equations slightly differ from what is in the algorithm document. In the algorithm document there is a placeholder for
        /// non-manure organic fertilizer that will be included in this calculated amount in a next version.
        /// 
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var totalNitrogen = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            
            var organicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(farm, manureExportViewItem.DateOfExport.Year);
            var emissionsForFarm = this.CalculateTotalDirectN2ONFromExportedManure(totalNitrogen, organicNitrogenEmissionFactor);
            var result = emissionsForFarm / farm.GetTotalAreaOfFarm(false, manureExportViewItem.DateOfExport.Year);

            return result;
        }

        /// <summary>
        /// Equation 2.6.9-24
        /// Equation 2.7.8-24
        ///
        /// Note: Can't attribute indirect N2O emissions from exported manure to any one particular field. The following amounts are not reported on the
        /// multi year carbon modelling report. For this reason, only the amount from 2.7.8-24 will be shown and only on the detailed emissions from (export emissions section)
        ///
        /// (kg N2O-N year^-1)
        /// </summary>
        public double CalculateTotalIndirectN2ONFromExportedManure(Farm farm, int year)
        {
            // Need this to allow farm results service to create manure export result view items (indirect)
            var totalLeachingEmissionsFromExportedManure = this.CalculateTotalLeachingN2ONFromExportedManure(farm, year);
            var totalVolatilizationEmissionsFromExportedManure = this.CalculateVolatilizationEmissionsFromExportedManureForFarmAndYear(farm, year);

            return totalLeachingEmissionsFromExportedManure + totalVolatilizationEmissionsFromExportedManure;
        }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double CalculateTotalIndirectN2ONFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var year = manureExportViewItem.DateOfExport.Year;
            var totalNitrogenExported = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            var leachingEmissions = this.CalculateLeachingEmissionsFromManure(farm, totalNitrogenExported, year);
            var volatilizationEmissions = this.CalculateVolatilizationEmissionsFromExportedManure(farm, totalNitrogenExported, manureExportViewItem.AnimalType, manureExportViewItem.DateOfExport);
            var totalEmissions = leachingEmissions + volatilizationEmissions;

            var result = totalEmissions / farm.GetTotalAreaOfFarm(false, year);

            return result;
        }
        
        /// <summary>
        /// Equation 4.6.1-9
        ///
        /// Returns the total N2O-N from exported manure for entire farm
        /// 
        /// (kg N2O-N year^-1 farm^-1)
        /// </summary>
        public double CalculateTotalDirectN2ONFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var result = 0d;

            result = this.CalculateTotalDirectN2ONFromExportedManureByYear(farm, year) * farm.GetTotalAreaOfFarm(false, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-3
        ///
        /// Note: Emissions can't be associated with any particular field and so results are reported for the entire farm
        /// 
        /// (kg N2O-N year^-1)
        /// </summary>
        public double CalculateTotalLeachingN2ONFromExportedManure(Farm farm, int year)
        {
            var nitrogenFromExportedManure = _manureService.GetTotalNitrogenFromExportedManure(year, farm);

            var result = this.CalculateLeachingEmissionsFromManure(farm, nitrogenFromExportedManure, year);

            return result;
        }


        /// <summary>
        /// Equation 4.6.2-19
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateNH3NLossFromManureExports(Farm farm, int year, ManureExportViewItem manureExportViewItem)
        {
            var nitrogenFromExportedManure = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            var result = this.CalculateAmmoniaLossFromManureExports(farm, manureExportViewItem.AnimalType, year, nitrogenFromExportedManure);

            return result;
        }

        /// <summary>
        /// (kg NH3-N)
        /// </summary>
        public double CalculateAdjustedNH3NLossFromManureExports(Farm farm, int year, ManureExportViewItem manureExportViewItem)
        {
            var result = 0d;

            var ammonia = this.CalculateNH3NLossFromManureExports(farm, year, manureExportViewItem);
            var totalNitrogenExported = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            var volatilization = this.CalculateVolatilizationEmissionsFromExportedManure(farm, totalNitrogenExported, manureExportViewItem.AnimalType, manureExportViewItem.DateOfExport);

            result = ammonia - volatilization;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// (kg NH3-N ha^-1)
        /// </summary>
        public double CalculateAdjustedNH3NLossFromManureExports(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var result = 0d;
            var year = manureExportViewItem.DateOfExport.Year;

            var emissions = this.CalculateAdjustedNH3NLossFromManureExports(farm, year, manureExportViewItem);

            result = emissions / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-19
        /// 
        /// Calculates total ammonia emissions for a given amount of N from exported manure
        /// 
        /// Note: this is only for exported manure - call <see cref="CalculateNH3NLossFromLandAppliedManure"/> otherwise
        /// 
        /// (kg NH3-N)
        /// </summary>
        private double CalculateAmmoniaLossFromManureExports(Farm farm, AnimalType animalType, int year, double totalNitrogen)
        {
            var factors = this.GetLandApplicationFactors(farm, animalType, year);
            var result = totalNitrogen * factors.VolatilizationFraction;

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-19
        ///
        /// (kg NH3-N year^-1)
        /// </summary>
        public Dictionary<AnimalType, double> CalculateAmmoniaEmissionsFromExportedManureForFarmAndYear(
            Farm farm,
            int year)
        {
            var dictionary = new Dictionary<AnimalType, double>();

            var totalNitrogenFromExports = this.CalculateTotalNitrogenFromExportedManure(year, farm);
            foreach (var totalNitrogenFromExport in totalNitrogenFromExports)
            {
                var manureType = totalNitrogenFromExport.Key;
                var nitrogenExported = totalNitrogenFromExport.Value;

                var amount = CalculateAmmoniaLossFromManureExports(farm, manureType, year, nitrogenExported);

                dictionary[manureType] = amount;
            }

            return dictionary;
        }

        /// <summary>
        /// Note: this is only for exported manure - call <see cref="CalculateNH3NLossFromLandAppliedManure"/> otherwise
        /// 
        /// (kg N2O-N)
        /// </summary>
        private double CalculateVolatilizationEmissionsFromExportedManure(Farm farm,
            double totalNitrogen,
            AnimalType animalType,
            DateTime dateTime)
        {
            return this.CalculateVolatilizationEmissionsFromExportedManure(farm, totalNitrogen, animalType, dateTime.Year);
        }

        /// <summary>
        ///  Note: this is only for exported manure - call <see cref="CalculateNH3NLossFromLandAppliedManure"/> otherwise
        /// 
        /// (kg N2O-N)
        /// </summary>
        private double CalculateVolatilizationEmissionsFromExportedManure(Farm farm,
            double totalNitrogen,
            AnimalType animalType,
            int year)
        {
            var factors = this.GetLandApplicationFactors(farm, animalType, year);

            // Note: this ammonia calculation is only for exported manure
            var ammonia = this.CalculateAmmoniaLossFromManureExports(farm, animalType, year, totalNitrogen);
            var emissionFactorVolatilization = factors.EmissionFactorVolatilization;
            var result = ammonia * emissionFactorVolatilization;

            return result;
        }

        /// <summary>
        ///  Note: this is only for exported manure - call <see cref="CalculateNH3NLossFromLandAppliedManure"/> otherwise
        /// 
        /// (kg N2O-N ha^-1)
        /// </summary>
        public double CalculateVolatilizationEmissionsFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var animalType = manureExportViewItem.AnimalType;
            var year = manureExportViewItem.DateOfExport.Year;
            var factors = this.GetLandApplicationFactors(farm, animalType, manureExportViewItem.DateOfExport.Year);
            var totalNitrogen = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);

            // Note: this ammonia calculation is only for exported manure
            var ammonia = this.CalculateAmmoniaLossFromManureExports(farm, animalType, year, totalNitrogen);
            var emissionFactorVolatilization = factors.EmissionFactorVolatilization;
            var emissions = ammonia * emissionFactorVolatilization;
            
            var result = emissions / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, year);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-8
        /// Equation 2.6.6-12
        /// Equation 2.7.5-12
        ///
        /// (kg NO3-N year^-1)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromExportedManureForFarmAndYear(Farm farm, int year)
        {
            var exportedManureNitrogen = _manureService.GetTotalNitrogenFromExportedManure(year, farm);

            var result = this.CalculateTotalNitrateLeachingEmissions(farm, exportedManureNitrogen, year);

            return result;
        }

        /// <summary>
        /// (kg NO3-N ha^-1)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromExportedManure(Farm farm, ManureExportViewItem manureExportViewItem)
        {
            var exportedManureNitrogen = _manureService.GetTotalNitrogenFromExportedManure(manureExportViewItem);
            var year = manureExportViewItem.DateOfExport.Year;

            var emissions = this.CalculateTotalNitrateLeachingEmissions(farm, exportedManureNitrogen, year);
            var result = emissions / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, year);

            return result;
        }

        #endregion
    }
}