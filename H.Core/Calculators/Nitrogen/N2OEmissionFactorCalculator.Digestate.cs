using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator
    {
        #region Public Methods

        public double CalculateNH3LossFromLandAppliedDigestateForField(double amountOfNitrogen)
        {
            return amountOfNitrogen * 0.1705;
        }

        public double CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
            double weightedEmissionFactor,
            double totalNitrogenFromRemainingDigestate)
        {
            var result = totalNitrogenFromRemainingDigestate * weightedEmissionFactor;

            return result;
        }

        /// <summary>
        /// Calculates direct emissions from the digestate specifically applied to the field (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreading(
            CropViewItem viewItem,
            Farm farm)
        {
            var totalNitrogenApplied = 0d;

            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                totalNitrogenApplied += digestateApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            }

            var result = totalNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-6
        /// 
        /// (kg N2O-N)
        /// </summary>
        public double CalculateDirectN2ONFromLeftOverDigestateForField(
            CropViewItem viewItem,
            Farm farm)
        {
            var itemsByYear = farm.GetCropDetailViewItemsByYear(viewItem.Year, false);
            var weightedEmissionFactorForOrganicNitrogen = this.CalculateWeightedOrganicNitrogenEmissionFactor(itemsByYear, farm);

            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                return 0;
            }

            // Get all fields that exist in the same year
            var itemsInYear = farm.GetCropDetailViewItemsByYear(viewItem.Year, false);

            // This is the total N remaining after all field applications have been considered
            var nitrogenRemainingAtEndOfYear = viewItem.GetRemainingNitrogenFromDigestateAtEndOfYear();

            // The total N2O-N from the remaining N
            var emissionsFromNitrogenRemaining = this.CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
                    weightedEmissionFactor: weightedEmissionFactorForOrganicNitrogen,
                    totalNitrogenFromRemainingDigestate: nitrogenRemainingAtEndOfYear);

            var totalAreaOfAllFields = itemsInYear.Sum(x => x.Area);
            var areaOfThisField = viewItem.Area;

            // The total N2O-N that is left over and must be associated with this field so that all digestate is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        /// <summary>
        /// Remaining nitrogen is spread evenly across all fields
        /// </summary>
        public double GetDigestateNitrogenRemainingForField(CropViewItem viewItem, Farm farm)
        {
            var fractionUsed = viewItem.Area / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, viewItem.Year);
            var manureNitrogenRemaining = this.DigestateService.GetTotalNitrogenRemainingAtEndOfYear(viewItem.Year, farm);

            return fractionUsed * manureNitrogenRemaining;
        }

        /// <summary>
        /// Equation 4.6.2-12
        /// </summary>
        public double CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in cropViewItem.DigestateApplicationViewItems)
            {
                result += this.CalculateNH3LossFromLandAppliedDigestateForField(manureApplicationViewItem.AmountOfNitrogenAppliedPerHectare * cropViewItem.Area);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-14
        /// </summary>
        public double CalculateTotalDigestateNitrogenRemaining(
            Farm farm,
            int year)
        {
            var result = 0d;

            // This will return total amount of N created minus amounts from land applications
            var nitrogenRemainingAtEndOfYear = this.DigestateService.GetTotalNitrogenRemainingAtEndOfYear(year, farm);
            var totalNitrogenExported = this.DigestateService.GetTotalNitrogenExported(year, farm);

            result = nitrogenRemainingAtEndOfYear - totalNitrogenExported;
            if (result < 0)
            {
                return 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.2-16
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateNH3NEmissionsFromLeftOverDigestateForField(
            CropViewItem cropViewItem,
            int year,
            Farm farm)
        {
            var result = 0d;

            var remainingNitrogen = this.GetDigestateNitrogenRemainingForField(cropViewItem, farm);
            result = this.CalculateNH3LossFromLandAppliedDigestateForField(remainingNitrogen);

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-1
        /// </summary>
        public double CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var ammoniaEmissionsFromLandAppliedManure = this.CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, cropViewItem, year);
            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);

            var result = ammoniaEmissionsFromLandAppliedManure * emissionFactorForVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-1
        /// </summary>
        public double CalculateN2OFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var ammoniaEmissionsFromLandAppliedManure = CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(year, farm, cropViewItem); var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);

            var result = CoreConstants.ConvertToN2O((ammoniaEmissionsFromLandAppliedManure));

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-2
        /// </summary>
        public double CalculateN2ONFromVolatilizationOfLeftOverDigestateForField(int year, Farm farm, CropViewItem cropViewItem)
        {
            var emissionFactorForVolatilization = this.GetEmissionFactorForVolatilization(farm, year);
            var leftOverAmmonia = this.CalculateNH3NEmissionsFromLeftOverDigestateForField(cropViewItem, year, farm);

            var result = leftOverAmmonia * emissionFactorForVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-5
        /// </summary>
        public double CalculateTotalDigestateN2ONVolatilizationForField(
            CropViewItem cropViewItem,
            Farm farm,
            int year)
        {
            var result = 0d;

            var volatilizationFromLandAppliedDigestate = this.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(year, farm, cropViewItem);
            var volatilizationFromLeftOverDigestate = this.CalculateN2ONFromVolatilizationOfLeftOverDigestateForField(year, farm, cropViewItem);
            var volatilizationFromImportedDigestate = 0;

            result = volatilizationFromLandAppliedDigestate + volatilizationFromLeftOverDigestate + volatilizationFromImportedDigestate;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-6
        /// </summary>
        public double CalculateTotalDigestateN2ONVolatilizationForFarmAndYear(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateTotalDigestateN2ONVolatilizationForField(cropViewItem, farm, year);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-7
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateAdjustedDigestateNH3NEmissionsForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var ammoniaFromLandApplications = CalculateNH3NLossFromFarmSourcedLandAppliedDigestateForField(farm, cropViewItem, year);
            var volatilizationFromLandApplications = this.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(year, farm, cropViewItem);

            result = ammoniaFromLandApplications - volatilizationFromLandApplications;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-7
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateAdjustedDigestateNH3EmissionsForField(
            Farm farm,
            CropViewItem cropViewItem)
        {
            var result = 0d;

            var ammoniaFromLandApplications = CalculateAdjustedDigestateNH3NEmissionsForField(farm, cropViewItem, cropViewItem.Year);

            result = CoreConstants.ConvertToNH3(ammoniaFromLandApplications);

            return result;
        }

        /// <summary>
        /// Equation 4.6.5-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalIndirectEmissionsFromDigestateForField(
            Farm farm,
            CropViewItem viewItem,
            int year)
        {
            var nitrateLeached = CalculateTotalDigestateNitrateLeached(farm, viewItem);
            var totalVolatilization = CalculateTotalDigestateN2ONVolatilizationForField(viewItem, farm, year);

            return nitrateLeached + totalVolatilization;
        }

        /// <summary>
        /// Equation 4.6.3-8
        /// </summary>
        public double CalculateTotalAdjustedAmmoniaEmissionsFromLeftOverDigestateForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var leftOverAmmonia = this.CalculateNH3NEmissionsFromLeftOverDigestateForField(cropViewItem, year, farm);
            var leftOverVolatilization = this.CalculateN2ONFromVolatilizationOfLeftOverDigestateForField(year, farm, cropViewItem);

            result = leftOverAmmonia - leftOverVolatilization;

            return result;
        }

        /// <summary>
        /// Equation 4.6.3-11
        ///
        /// (kg NH3-N)
        /// </summary>
        public double CalculateTotalDigestateAmmoniaEmissionsForField(
            Farm farm,
            CropViewItem cropViewItem,
            int year)
        {
            var result = 0d;

            var adjustedAmmoniaFromLandApplications = CalculateAdjustedDigestateNH3NEmissionsForField(farm, cropViewItem, year);
            var adjustedAmmoniaFromImports = 0;
            var adjustedAmmoniaFromLeftOverManure = this.CalculateTotalAdjustedAmmoniaEmissionsFromLeftOverDigestateForField(farm, cropViewItem, year);

            result = adjustedAmmoniaFromLandApplications + adjustedAmmoniaFromImports + adjustedAmmoniaFromLeftOverManure;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONFromDigestateLeachingFromDigestateApplication(Farm farm, CropViewItem viewItem, ManureItemBase manureItemBase)
        {
            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var nitrogenUsed = manureItemBase.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = nitrogenUsed * leachingFraction * leachingEmissionFactorForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONFromDigestateLeachingForField(Farm farm, CropViewItem viewItem)
        {
            var result = 0d;

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                result += this.CalculateTotalN2ONFromDigestateLeachingFromDigestateApplication(farm, viewItem, digestateApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-2
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONLeachingFromLeftOverDigestateLeachingForField(Farm farm, CropViewItem viewItem)
        {
            var digestateNitrogenRemainingForField = GetDigestateNitrogenRemainingForField(viewItem, farm);

            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);
            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;

            var result = digestateNitrogenRemainingForField * leachingFraction * leachingEmissionFactorForLandApplication;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-6
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalDigestateNitrateLeached(Farm farm, CropViewItem viewItem)
        {
            var result = 0d;

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                result += this.CalculateTotalNitrateLeached(farm, viewItem, digestateApplicationViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-7
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromLeftOverDigestateForField(Farm farm, CropViewItem viewItem)
        {
            var totalNitrogenRemainingForField = this.GetDigestateNitrogenRemainingForField(viewItem, farm);

            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = totalNitrogenRemainingForField * leachingFraction * (1 - leachingEmissionFactorForLandApplication);

            return result;
        }

        #endregion   
    }
}