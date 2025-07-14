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

        /// <summary>
        /// No equation. See section 4.9.2
        /// </summary>
        /// <param name="amountOfNitrogen"></param>
        /// <returns></returns>
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
        /// Equation 4.6.1-1
        /// 
        /// Calculates direct emissions from the digestate specifically applied to the field
        ///
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var fieldSpecificOrganicNitrogenEmissionFactor = this.CalculateOrganicNitrogenEmissionFactor(
                viewItem: viewItem,
                farm: farm);

            var totalLocalAndImportedNitrogenApplied = this.GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(viewItem, farm);

            var result = totalLocalAndImportedNitrogenApplied * fieldSpecificOrganicNitrogenEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-2
        /// 
        /// (kg N)
        /// </summary>
        public double GetTotalDigestateNitrogenAppliedFromLivestockAndImportsInYear(CropViewItem viewItem, Farm farm)
        {
            var year = viewItem.Year;

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            var totalNitrogen = 0d;

            var livestockApplications = field.GetLivestockDigestateApplicationsInYear(year);
            var importedApplications = field.GetImportedDigestateApplicationsInYear(year);
            var allApplications = livestockApplications.Concat(importedApplications);

            foreach (var digestateApplicationViewItem in allApplications)
            {
                totalNitrogen += digestateApplicationViewItem.AmountOfNitrogenAppliedPerHectare * viewItem.Area;
            }

            return totalNitrogen;
        }

        public double GetTotalDigestateVolumeAppliedFromLivestockAndImportsInYear(CropViewItem viewItem, Farm farm)
        {
            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null || (field.HasLivestockDigestateApplicationsInYear(viewItem.Year) == false && field.HasImportedDigestateApplicationsInYear(viewItem.Year) == false))
            {
                return 0;
            }

            var totalVolume = 0d;

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems.Where(manureViewItem => manureViewItem.DateCreated.Year == viewItem.Year))
            {
                totalVolume += digestateApplicationViewItem.AmountAppliedPerHectare * viewItem.Area;
            }

            return totalVolume;
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
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var year = viewItem.Year;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            var weightedEmissionFactorForOrganicNitrogen = this.CalculateWeightedOrganicNitrogenEmissionFactor(itemsByYear, farm);

            var component = farm.GetAnaerobicDigestionComponent();
            if (component == null)
            {
                return 0;
            }

            var nitrogenRemainingAtEndOfYear = _digestateService.GetTotalNitrogenRemainingAtEndOfYearAfterFieldApplications(year, farm);

            // The total N2O-N from the remaining N
            var emissionsFromNitrogenRemaining = this.CalculateTotalEmissionsFromRemainingDigestateThatIsAppliedToAllFields(
                    weightedEmissionFactor: weightedEmissionFactorForOrganicNitrogen,
                    totalNitrogenFromRemainingDigestate: nitrogenRemainingAtEndOfYear);

            var totalAreaOfAllFields = farm.GetTotalAreaOfFarm(false, year);
            if (totalAreaOfAllFields == 0)
            {
                totalAreaOfAllFields = 1;
            }

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            var areaOfThisField = field.FieldArea;

            // The total N2O-N that is left over and must be associated with this field so that all digestate is applied to the fields in the same year (nothing is remaining to be applied)
            var result = emissionsFromNitrogenRemaining * (areaOfThisField / totalAreaOfAllFields);

            return result;
        }

        /// <summary>
        /// Remaining nitrogen is spread evenly across all fields
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double GetDigestateNitrogenRemainingForField(CropViewItem viewItem, Farm farm)
        {
            var fractionUsed = viewItem.Area / farm.GetTotalAreaOfFarm(includeNativeGrasslands: false, viewItem.Year);
            var digestateNitrogenRemaining = _digestateService.GetTotalNitrogenRemainingAtEndOfYearAfterFieldApplications(viewItem.Year, farm);

            return fractionUsed * digestateNitrogenRemaining;
        }

        /// <summary>
        /// Equation 4.6.1-10
        ///
        /// Includes direct emissions from applied digestate and optionally direct emissions from remaining digestate for the field.
        /// 
        /// (kg N2O-N (kg N)^-1)
        /// </summary>
        public double CalculateDirectN2ONFromFieldAppliedDigestate(
            Farm farm,
            CropViewItem viewItem, 
            bool includeRemainingAmounts)
        {
            var result = 0d;

            var applied = this.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(viewItem, farm);
            var leftOver = this.CalculateDirectN2ONFromLeftOverDigestateForField(viewItem, farm);

            result = applied;

            if (includeRemainingAmounts)
            {
                result += leftOver;
            }

            return result;
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
            var nitrogenRemainingAtEndOfYear = _digestateService.GetTotalNitrogenRemainingAtEndOfYearAfterFieldApplications(year, farm);
            var totalNitrogenExported = _digestateService.GetTotalNitrogenExported(year, farm);

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
            var field = farm.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (field == null || field.HasLivestockDigestateApplicationsInYear(cropViewItem.Year) == false)
            {
                return 0;
            }

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
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDigestateN2ONVolatilizationForField(
            CropViewItem cropViewItem,
            Farm farm,
            int year, 
            bool includeRemainingAmounts)
        {
            if (cropViewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            /*
             * Note: currently the user imports manure into the AD system and the outptut is identical to the outputs as if the AD was using on-farm manure
             * and so we don't add in an 'extra' amount for the imported manure (which is then transformed into digestate). This different than the approach
             * taken when considering manure applications to a field
             */

            var volatilizationFromLandAppliedDigestate = this.CalculateN2ONFromVolatilizationOfFarmSourcedLandAppliedDigestateForField(year, farm, cropViewItem);
            var volatilizationFromLeftOverDigestate = this.CalculateN2ONFromVolatilizationOfLeftOverDigestateForField(year, farm, cropViewItem);
            var volatilizationFromImportedDigestate = 0;

            result = volatilizationFromLandAppliedDigestate + volatilizationFromImportedDigestate;
            if (includeRemainingAmounts)
            {
                result += volatilizationFromLeftOverDigestate;
            }

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
                result += this.CalculateTotalDigestateN2ONVolatilizationForField(cropViewItem, farm, year, true);
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
            if (cropViewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var result = 0d;

            var adjustedAmmoniaFromLandApplications = CalculateAdjustedDigestateNH3NEmissionsForField(farm, cropViewItem, year);
            var adjustedAmmoniaFromImports = 0;
            var adjustedAmmoniaFromLeftOverDigestate = this.CalculateTotalAdjustedAmmoniaEmissionsFromLeftOverDigestateForField(farm, cropViewItem, year);

            result = adjustedAmmoniaFromLandApplications + adjustedAmmoniaFromImports + adjustedAmmoniaFromLeftOverDigestate;

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-1
        ///
        /// Calculate leaching for single application to a field
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
        /// Calculate leaching for all applications on field
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalN2ONFromDigestateLeachingForField(Farm farm, CropViewItem viewItem)
        {
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            if (field == null)
            {
                return 0;
            }

            var result = 0d;

            var year = viewItem.Year;
            var livestockApplications = field.GetLivestockDigestateApplicationsInYear(year);
            var importedApplications = field.GetImportedDigestateApplicationsInYear(year);
            var allApplications = livestockApplications.Concat(importedApplications);

            foreach (var fieldApplication in allApplications)
            {
                result += this.CalculateTotalN2ONFromDigestateLeachingFromDigestateApplication(farm, viewItem, fieldApplication);
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
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

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
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

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
            if (viewItem.CropType.IsNativeGrassland())
            {
                return 0;
            }

            var totalNitrogenRemainingForField = this.GetDigestateNitrogenRemainingForField(viewItem, farm);

            var leachingEmissionFactorForLandApplication = farm.Defaults.EmissionFactorForLeachingAndRunoff;
            var leachingFraction = this.GetLeachingFraction(farm, viewItem.Year);

            var result = totalNitrogenRemainingForField * leachingFraction * (1 - leachingEmissionFactorForLandApplication);

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-8
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalNitrateLeachedFromExportedDigestateForFarmAndYear(Farm farm, int year)
        {
            // Export of digestate not supported yet
            return 0;
        }

        /// <summary>
        /// Equation 4.6.4-9
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateAllDigestateNitrateLeached(Farm farm, CropViewItem cropViewItem)
        {
            var result = 0d;

            var leachingFromApplications = this.CalculateTotalDigestateNitrateLeached(farm, cropViewItem);
            var leachingFromRemaining = CalculateTotalNitrateLeachedFromLeftOverDigestateForField(farm, cropViewItem);

            result = leachingFromApplications + leachingFromRemaining; 

            return result;
        }

        /// <summary>
        /// Equation 4.6.4-10
        ///
        /// (kg NO3-N)
        /// </summary>
        public double CalculateTotalDigestateNitrateLeachedFromForFarmAndYear(Farm farm, int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateAllDigestateNitrateLeached(farm, cropViewItem);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.6-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalDirectEmissionsFromDigestate(int year, Farm farm)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);
            foreach (var cropViewItem in itemsByYear)
            {
                result += this.CalculateDirectN2ONEmissionsFromFieldSpecificDigestateSpreadingForField(cropViewItem, farm);
                result += CalculateDirectN2ONFromLeftOverDigestateForField(cropViewItem, farm);
            }

            // Exports not considered yet
            result += 0;

            return result;
        }

        /// <summary>
        /// Equation 4.9.5-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalIndirectEmissionsFromDigestateForFarm(
            Farm farm,
            int year)
        {
            var result = 0d;

            var itemsByYear = farm.GetCropDetailViewItemsByYear(year, false);

            foreach (var cropViewItem in itemsByYear)
            {
                var nitrateLeached = CalculateTotalN2ONFromDigestateLeachingForField(farm, cropViewItem);
                var totalVolatilization = CalculateTotalDigestateN2ONVolatilizationForField(cropViewItem, farm, year, true);

                result += (nitrateLeached + totalVolatilization);
            }

            return result;
        }

        /// <summary>
        /// Equation 4.9.6-1
        ///
        /// (kg N2O-N)
        /// </summary>
        public double CalculateTotalEmissionsFromDigestate(
            Farm farm,
            int year)
        {
            var direct = this.CalculateTotalDirectEmissionsFromDigestate(year, farm);
            var indirect = CalculateTotalIndirectEmissionsFromDigestateForFarm(farm, year);

            return indirect + direct;
        }

        #endregion   
    }
}