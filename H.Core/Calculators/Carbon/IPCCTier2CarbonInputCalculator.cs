using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Plants;
using H.Core.Services.Animals;

namespace H.Core.Calculators.Carbon
{
    public class IPCCTier2CarbonInputCalculator : CarbonInputCalculatorBase, IIPCCTier2CarbonInputCalculator
    {
        #region Fields

        private readonly Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _slopeProvider;
        private readonly IManureService _manureService;
        private readonly IDigestateService _digestateService;

        #endregion

        #region Constructors

        public IPCCTier2CarbonInputCalculator()
        {
            _slopeProvider = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider();
            _manureService = new ManureService();
            _digestateService = new DigestateService();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The IPCC Tier 2 approach can only estimate carbon inputs for a small set of crop types. If a crop type does not have values for intercept, slope, etc.
        /// from Table 12 then we cannot use the Tier 2 approach for calculating carbon inputs.
        /// </summary>
        public bool CanCalculateInputsForCrop(CropViewItem cropViewItem)
        {
            var slope = _slopeProvider.GetDataByCropType(cropViewItem.CropType);

            return slope.SlopeValue > 0;
        }

        public void CalculateInputsForCrop(CropViewItem viewItem, Farm farm)
        {
            var cropData = _slopeProvider.GetDataByCropType(viewItem.CropType);

            var slope = cropData.SlopeValue;
            var intercept = cropData.InterceptValue;

            // Note that the yield must be converted to tons here since the curve equation expects a yield in tons when multiplying by slope
            var harvestIndex = this.CalculateHarvestIndex(
                slope: slope,
                freshWeightOfYield: viewItem.Yield,
                intercept: intercept,
                moistureContentAsPercentage: viewItem.MoistureContentOfCropPercentage);

            if (viewItem.HarvestMethod == HarvestMethods.Swathing && farm.CropHasGrazingAnimals(viewItem))
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = (100 - viewItem.GetAverageUtilizationFromGrazingAnimals());
            }

            viewItem.AboveGroundResidueDryMatter = this.CalculateAboveGroundResidueDryMatter(harvestIndex: harvestIndex, viewItem: viewItem);

            viewItem.AboveGroundResidueDryMatterExported = this.CalculateAboveGroundResidueDryMatterExported(harvestRatio: harvestIndex, cropViewItem: viewItem);

            var fractionRenewed = viewItem.CropType.IsAnnual() ? 1 : 1 / viewItem.PerennialStandLength;

            var finalAboveGroundResidue = this.CalculateAnnualAboveGroundResidue(
                aboveGroundResidueDryMatterForCrop: viewItem.AboveGroundResidueDryMatter,
                area: viewItem.Area,
                fractionRenewed: fractionRenewed,
                fractionBurned: 0,
                fractionRemoved: 0,
                combustionFactor: 0);

            const double AboveGroundCarbonContent = 0.42;

            // Note that eq. 2.2.3-3 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.AboveGroundCarbonInput = (finalAboveGroundResidue * AboveGroundCarbonContent) / viewItem.Area;

            var supplementalFeedingAmount = base.CalculateInputsFromSupplementalHayFedToGrazingAnimals(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItems: null,
                farm: farm);

            viewItem.AboveGroundCarbonInput += supplementalFeedingAmount;

            viewItem.BelowGroundResidueDryMatter = this.CalculateBelowGroundResidueDryMatter(shootToRootRatio: cropData.RSTRatio,
                fractionRenewed: fractionRenewed,
                harvestIndex: harvestIndex,
                cropViewItem: viewItem);

            const double BelowGroundCarbonContent = 0.42;

            // Note that eq. 2.2.3-4 is the residue for the entire field, we report per ha on the details screen so we divide by the area here
            viewItem.BelowGroundCarbonInput = (viewItem.BelowGroundResidueDryMatter * BelowGroundCarbonContent) / viewItem.Area;


            if (farm.IsCommandLineMode == false)
            {
                viewItem.ManureCarbonInputsPerHectare = _manureService.GetTotalManureCarbonInputsForField(farm, viewItem.Year, viewItem);
            }

            viewItem.ManureCarbonInputsPerHectare += viewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture;

            viewItem.DigestateCarbonInputsPerHectare = _digestateService.GetTotalDigestateCarbonInputsForField(farm, viewItem.Year, viewItem);

            /*
             * Equation 2.2.2-12 (kg C will be used in pool calculations instead of tons C). Algorithm document converts to tons before inputs are used in pool calculations but inputs are kept in kg C
             * here. We report results in kg C on graphs so the conversion to tons is not performed here.
             *
             * Since we report ICBM in kg C (not tons), we do not convert to tons here so output of pool calculations on chart can be compared to ICBM chart on same scale (i.e. kg C and not T C).
             */

            viewItem.TotalCarbonInputs = viewItem.AboveGroundCarbonInput + viewItem.BelowGroundCarbonInput + viewItem.ManureCarbonInputsPerHectare + viewItem.DigestateCarbonInputsPerHectare;
        }

        /// <summary>
        /// Equation 2.2.2-1
        /// </summary>
        /// <param name="slope">(unitless)</param>
        /// <param name="freshWeightOfYield">The yield of the harvest (wet/fresh weight) (kg ha^-1)</param>
        /// <param name="intercept">(unitless)</param>
        /// <param name="moistureContentAsPercentage">The moisture content of the yield (%)</param>
        /// <returns>The harvest index</returns>
        public double CalculateHarvestIndex(
            double slope,
            double freshWeightOfYield,
            double intercept,
            double moistureContentAsPercentage)
        {
            return slope * ((freshWeightOfYield / 1000) * (1 - (moistureContentAsPercentage / 100.0))) + intercept;
        }

        /// <summary>
        /// Equation 2.2.2-2
        /// </summary>
        /// <param name="harvestIndex">The harvest index (kg DM ha^-1)</param>
        /// <param name="viewItem"></param>
        /// <returns>Above ground residue dry matter for crop (kg ha^-1)</returns>
        public double CalculateAboveGroundResidueDryMatter(
            double harvestIndex,
            CropViewItem viewItem)
        {
            if (harvestIndex <= 0)
            {
                return 0;
            }

            var freshWeightOfYield = viewItem.Yield;
            var moistureContentOfCropAsPercentage = viewItem.MoistureContentOfCropPercentage;

            if (viewItem.HasGrazingViewItems)
            {
                moistureContentOfCropAsPercentage = viewItem.GrazingViewItems.Average(x => x.MoistureContentAsPercentage);
            }

            var moistureContentFraction = moistureContentOfCropAsPercentage / 100.0;
            var moistureFractionDifference = 1 - moistureContentFraction;

            var strawReturnedToSoilFraction = viewItem.PercentageOfStrawReturnedToSoil / 100.0;
            var productReturnedToSoilFraction = viewItem.PercentageOfProductYieldReturnedToSoil / 100.0;

            var leftFirstTerm = (freshWeightOfYield * moistureFractionDifference) / harvestIndex;
            var leftSecondTerm = freshWeightOfYield * moistureFractionDifference;
            var leftInnerDifference = leftFirstTerm - leftSecondTerm;
            var leftResult = leftInnerDifference * strawReturnedToSoilFraction;

            var rightFirstTerm = freshWeightOfYield * moistureFractionDifference;
            var rightResult = rightFirstTerm * productReturnedToSoilFraction;

            var finalResult = leftResult + rightResult;

            return finalResult;
        }

        /// <summary>
        /// Equation 2.2.2-4
        /// </summary>
        /// <param name="aboveGroundResidueDryMatterForCrop">Above ground residue dry matter for crop (kg ha^-1)</param>
        /// <param name="area">Area of field (ha)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <param name="fractionBurned">(unitless)</param>
        /// <param name="fractionRemoved">(unitless)</param>
        /// <param name="combustionFactor">(unitless)</param>
        /// <returns>Annual total amount of above-ground residue (kg year^-1)</returns>
        public double CalculateAnnualAboveGroundResidue(
            double aboveGroundResidueDryMatterForCrop,
            double area,
            double fractionRenewed,
            double fractionBurned,
            double fractionRemoved,
            double combustionFactor)
        {
            // Not considering burned residues right now
            return aboveGroundResidueDryMatterForCrop * area * fractionRenewed * (1 - fractionRemoved - (fractionBurned * combustionFactor));
        }

        /// <summary>
        /// Equation 2.2.2-3
        /// </summary>
        /// <param name="harvestRatio">The harvest index (kg DM ha^-1)</param>
        /// <param name="cropViewItem"></param>
        /// <returns>Above ground residue dry matter for crop (kg ha^-1)</returns>
        public double CalculateAboveGroundResidueDryMatterExported(double harvestRatio,
            CropViewItem cropViewItem)
        {
            if (harvestRatio <= 0)
            {
                return 0;
            }

            var moistureContentFraction = cropViewItem.MoistureContentOfCropPercentage / 100.0;
            var percentageOfStrawFraction = cropViewItem.PercentageOfStrawReturnedToSoil / 100.0;
            var freshWeightOfYield = cropViewItem.Yield;

            var percentageOfProductFraction = 0d;
            if (cropViewItem.HasGrazingViewItems)
            {
                percentageOfProductFraction = 1.0 - ((cropViewItem.GrazingViewItems.Average(x => x.Utilization) / 100.0));
            }
            else
            {
                percentageOfProductFraction = cropViewItem.PercentageOfProductYieldReturnedToSoil / 100.0;
            }

            var firstTerm = (freshWeightOfYield * (1 - moistureContentFraction)) / harvestRatio;
            var secondTerm = (freshWeightOfYield * (1 - moistureContentFraction));
            var thirdTerm = 1 - percentageOfStrawFraction;
            var fourthTerm = (freshWeightOfYield * (1 - moistureContentFraction));
            var fifthTerm = 1 - percentageOfProductFraction;

            var result = ((firstTerm - secondTerm) * (thirdTerm)) + ((fourthTerm * fifthTerm));

            return result;
        }

        /// <summary>
        /// Equation 2.2.2-5
        /// Equation 2.2.2-6
        /// </summary>
        /// <param name="shootToRootRatio">Ratio of below-ground root biomass to above-ground shoot biomass (RS(T)) (kg dm ha^-1 (kg dm ha^-1)^-1)</param>
        /// <param name="fractionRenewed">(unitless)</param>
        /// <param name="harvestIndex">Harvest ratio/index (R_AG(T))</param>
        /// <param name="cropViewItem"></param>
        /// <returns>Annual total amount of below-ground residue (kg year^-1)</returns>
        public double CalculateBelowGroundResidueDryMatter(
            double shootToRootRatio,
            double fractionRenewed,
            double harvestIndex,
            CropViewItem cropViewItem)
        {
            var cropArea = cropViewItem.Area;
            var freshWeight = cropViewItem.Yield;
            var moisturePercentage = cropViewItem.MoistureContentOfCropPercentage;
            var harvestMethod = cropViewItem.HarvestMethod;
            var moistureContentFraction = moisturePercentage / 100.0;
            var moistureContentDifference = 1 - moistureContentFraction;

            if (harvestIndex <= 0)
            {
                harvestIndex = 1;
            }

            var result = 0d;
            if (harvestMethod == HarvestMethods.CashCrop)
            {
                var firstTerm = (freshWeight * moistureContentDifference) / harvestIndex;

                result = firstTerm * shootToRootRatio * cropArea * fractionRenewed;
            }
            else
            {
                // Swathing, silage, green manure harvests
                var innerResult = (freshWeight * moistureContentDifference);

                result = innerResult * shootToRootRatio * cropArea * fractionRenewed;
            }

            return result;
        }

        #endregion
    }
}