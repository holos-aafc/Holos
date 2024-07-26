using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
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
            _icbmCarbonInputCalculator.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _icbmNitrogenInputCalculator.CalculateGrainNitrogenTotal(
                carbonInputFromAgriculturalProduct: viewItem.PlantCarbonInAgriculturalProduct,
                nitrogenConcentrationInProduct: viewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _icbmNitrogenInputCalculator.CalculateNitrogenContentStrawReturnedToSoil(
                carbonInputFromStraw: viewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: viewItem.NitrogenContentInStraw);

            var nitrogenContentOfRootReturnedToSoil = _icbmNitrogenInputCalculator.CalculateNitrogenContentRootReturnedToSoil(
                carbonInputFromRoots: viewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: viewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _icbmNitrogenInputCalculator.CalculateNitrogenContentExaduatesReturnedToSoil(
                carbonInputFromExtraroots: viewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: viewItem.NitrogenContentInExtraroot);

            var isLeguminousCrop = viewItem.CropType.IsLeguminousCoverCrop() || viewItem.CropType.IsPulseCrop();

            var syntheticFertilizerApplied = _icbmNitrogenInputCalculator.CalculateSyntheticFertilizerApplied(
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

        public void InitializeBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var data = _carbonFootprintForFertilizerBlendsProvider.GetData(fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend);
            if (data != null)
            {
                /*
                 * Don't reassign the FertilizerBlendData property to the object returned from the provider since the view model will have attached event handlers
                 * that will get lost of the object is assigned, instead copy individual properties
                 */

                fertilizerApplicationViewItem.FertilizerBlendData.PercentageNitrogen = data.PercentageNitrogen;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePhosphorus = data.PercentagePhosphorus;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentagePotassium = data.PercentagePotassium;
                fertilizerApplicationViewItem.FertilizerBlendData.PercentageSulphur = data.PercentageSulphur;
                fertilizerApplicationViewItem.FertilizerBlendData.ApplicationEmissions = data.ApplicationEmissions;
                fertilizerApplicationViewItem.FertilizerBlendData.CarbonDioxideEmissionsAtTheGate = data.CarbonDioxideEmissionsAtTheGate;
            }
        }

        /// <summary>
        /// When not using a blended P fertilizer approach, use this to assign a P rate directly to the crop
        /// </summary>
        public void InitializePhosphorusFertilizerRate(CropViewItem viewItem, Farm farm)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);
            var province = soilData.Province;

            // Start with a default then get lookup value if one is available
            viewItem.PhosphorusFertilizerRate = 25;

            var soilFunctionCategory = farm.GetPreferredSoilData(viewItem).SoilFunctionalCategory;
            var residueData = _relativeBiomassInformationProvider.GetResidueData(viewItem.IrrigationType, viewItem.AmountOfIrrigation, viewItem.CropType, soilFunctionCategory, farm.Province);
            if (residueData != null)
            {
                if (residueData.PhosphorusFertilizerRateTable.ContainsKey(province))
                {
                    var phosphorusFertilizerTable = residueData.PhosphorusFertilizerRateTable[province];
                    if (phosphorusFertilizerTable.ContainsKey(soilData.SoilFunctionalCategory))
                    {
                        var rate = phosphorusFertilizerTable[soilData.SoilFunctionalCategory];
                        viewItem.PhosphorusFertilizerRate = rate;
                    }
                }
            }
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
            var requiredNitrogen = this.CalculateRequiredNitrogenFertilizer(
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

        #endregion
    }
}