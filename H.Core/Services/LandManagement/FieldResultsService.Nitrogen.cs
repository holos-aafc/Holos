using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods

        /// <summary>
        /// Determines the amount of N fertilizer required for the specified crop type and yield
        /// </summary>
        public double CalculateRequiredNitrogenFertilizer(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _n2OEmissionFactorCalculator.CalculateGrainNitrogenTotal(
                carbonInputFromAgriculturalProduct: viewItem.PlantCarbonInAgriculturalProduct,
                nitrogenConcentrationInProduct: viewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _n2OEmissionFactorCalculator.CalculateNitrogenContentStrawReturnedToSoil(
                carbonInputFromStraw: viewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: viewItem.NitrogenContentInStraw);

            var nitrogenContentOfRootReturnedToSoil = _n2OEmissionFactorCalculator.CalculateNitrogenContentRootReturnedToSoil(
                carbonInputFromRoots: viewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: viewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _n2OEmissionFactorCalculator.CalculateNitrogenContentExaduatesReturnedToSoil(
                carbonInputFromExtraroots: viewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: viewItem.NitrogenContentInExtraroot);

            var isLeguminousCrop = viewItem.CropType.IsLeguminousCoverCrop() || viewItem.CropType.IsPulseCrop();

            var syntheticFertilizerApplied = _n2OEmissionFactorCalculator.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: nitrogenContentOfGrainReturnedToSoil,
                nitrogenContentOfStrawReturnedToSoil: nitrogenContentOfStrawReturnedToSoil,
                nitrogenContentOfRootReturnedToSoil: nitrogenContentOfRootReturnedToSoil,
                nitrogenContentOfExtrarootReturnedToSoil: nitrogenContentOfExtrarootReturnedToSoil,
                fertilizerEfficiencyFraction: fertilizerApplicationViewItem.FertilizerEfficiencyFraction,
                soilTestN: viewItem.SoilTestNitrogen,
                isNitrogenFixingCrop: isLeguminousCrop,
                nitrogenFixationAmount: viewItem.NitrogenFixation,
                atmosphericNitrogenDeposition: viewItem.NitrogenDepositionAmount);

            return syntheticFertilizerApplied;
        }

        /// <summary>
        /// Equation 2.5.5-7
        ///
        /// Calculates the amount of the fertilizer blend needed to support the yield that was input.This considers the amount of nitrogen uptake by the plant and then
        /// converts that value into an amount of fertilizer blend/product
        /// </summary>
        /// <returns>The amount of product required (kg product ha^-1)</returns>
        public double CalculateAmountOfProductRequired(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var requiredNitrogen = CalculateRequiredNitrogenFertilizer(
                farm: farm,
                viewItem: viewItem,
                fertilizerApplicationViewItem: fertilizerApplicationViewItem);

            // If blend is custom, default N value will be zero and so we can't calculate the amount of product required
            if (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen == 0)
            {
                return 0;
            }

            // Need to convert to amount of fertilizer product from required nitrogen
            var requiredAmountOfProduct = (requiredNitrogen / (fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen / 100));

            return requiredAmountOfProduct;
        }

        /// <summary>
        /// Calculates how much nitrogen added from manure of animals grazing on the field.
        /// </summary>
        public void CalculateManureNitrogenInputsByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            List<CropViewItem> cropViewItems)
        {
            this.CalculateManureNitrogenInputByGrazingAnimals(
                fieldSystemComponent: fieldSystemComponent,
                results: this.AnimalResults,
                cropViewItems);
        }

        /// <summary>
        /// Equation 5.6.2-1
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double CalculateManureNitrogenInputsFromGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results)
        {
            var totalNitrogenExcretedByAnimals = 0d;
            var totalAmmoniaEmissions = 0d;
            var totalLeaching = 0d;
            var totalN2ON = 0d;


            var grazingViewItems = fieldSystemComponent.CropViewItems.Where(y => y.CropType == cropViewItem.CropType).SelectMany(x => x.GrazingViewItems).ToList();

            var grazingItems = grazingViewItems.Where(x => x.Start.Year == cropViewItem.Year).ToList();

            foreach (var grazingViewItem in grazingItems)
            {
                var emissionsFromGrazingAnimals = this.GetGroupEmissionsFromGrazingAnimals(results, grazingViewItem);
                foreach (var groupEmissionsByMonth in emissionsFromGrazingAnimals)
                {
                    totalNitrogenExcretedByAnimals += groupEmissionsByMonth.MonthlyAmountOfNitrogenExcreted;
                    totalAmmoniaEmissions += groupEmissionsByMonth.MonthlyNH3FromGrazingAnimals;
                    totalLeaching += groupEmissionsByMonth.MonthlyManureLeachingN2ONEmission;
                    totalN2ON += (groupEmissionsByMonth.MonthlyManureDirectN2ONEmission + groupEmissionsByMonth.MonthlyManureIndirectN2ONEmission);
                }
            }

            var result = (totalNitrogenExcretedByAnimals - (totalN2ON + (CoreConstants.ConvertToNH3N(totalAmmoniaEmissions)) + totalLeaching)) / cropViewItem.Area;

            return result < 0 ? 0 : result;
        }

        public void CalculateManureNitrogenInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results, List<CropViewItem> cropViewItems)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                cropViewItem.TotalNitrogenInputFromManureFromAnimalsGrazingOnPasture = this.CalculateManureNitrogenInputsFromGrazingAnimals(fieldSystemComponent, cropViewItem, results.ToList());
            }
        }

        #endregion

        #region Private Methods

        private double CalculateAboveGroundResidueNitrogen(CropViewItem cropViewItem)
        {
            if (_tier2SoilCarbonCalculator.CanCalculateInputsForCrop(cropViewItem))
            {
                return _n2OEmissionFactorCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
                    aboveGroundResidueDryMatter: cropViewItem.AboveGroundResidueDryMatter,
                    cropViewItem.CarbonConcentration,
                    nitrogenContentInStraw: cropViewItem.NitrogenContentInStraw);
            }
            else
            {
                return _n2OEmissionFactorCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIcbm(
                    cropViewItem: cropViewItem);
            }
        }

        private double CalculateBelowGroundResidueNitrogen(CropViewItem cropViewItem)
        {
            if (_tier2SoilCarbonCalculator.CanCalculateInputsForCrop(cropViewItem))
            {
                return _n2OEmissionFactorCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(
                    viewItem: cropViewItem);
            }
            else
            {
                return _n2OEmissionFactorCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIcbm(
                    cropViewItem: cropViewItem);
            }
        }

        #endregion
    }
}