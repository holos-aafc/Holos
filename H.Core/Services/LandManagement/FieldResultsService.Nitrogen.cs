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

        #region Land Applied Manure

        /// <summary>
        /// Equation 4.6.1-2
        ///
        /// (kg N)
        /// </summary>
        public double CalculateTotalManureNitrogenAppliedToAllFields(FarmEmissionResults farmEmissionResults)
        {
            var result = 0d;

            foreach (var fieldComponentEmissionResult in farmEmissionResults.FieldComponentEmissionResults)
            {
                var viewItem = fieldComponentEmissionResult.FieldSystemComponent.GetSingleYearViewItem();
                var appliedNitrogenFromManure = viewItem.GetTotalManureNitrogenAppliedFromLivestockInYear();

                result += appliedNitrogenFromManure;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.6.1-4
        /// </summary>
        public double CalculateTotalNitrogenFromLandManureRemaining(
            double totalManureAvailableForLandApplication,
            double totalManureAlreadyAppliedToFields,
            double totalManureExported)
        {
            return totalManureAvailableForLandApplication - totalManureAlreadyAppliedToFields - totalManureExported;
        }

        /// <summary>
        /// Equation 4.6.1-5
        /// </summary>
        public double CalculateTotalEmissionsFromRemainingManureThatIsAppliedToAllFields(
            double weightedEmissionFactor,
            double totalNitrogenFromLandManureRemaining)
        {

            var result = totalNitrogenFromLandManureRemaining * weightedEmissionFactor;

            return result;
        }

        #endregion

        /// <summary>
        /// Calculate amount of nitrogen input from all manure applications in a year
        /// </summary>
        /// <returns>The amount of nitrogen input during the year (kg N)</returns>
        public double CalculateManureNitrogenInput(
            CropViewItem cropViewItem,
            Farm farm)
        {
            var result = 0.0;

            foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
            {
                var manureCompositionData = manureApplicationViewItem.DefaultManureCompositionData;

                var fractionOfNitrogen = manureCompositionData.NitrogenContent;

                var amountOfNitrogen = _icbmSoilCarbonCalculator.CalculateAmountOfNitrogenAppliedFromManure(
                    manureAmount: manureApplicationViewItem.AmountOfManureAppliedPerHectare,
                    fractionOfNitrogen);

                result += amountOfNitrogen;
            }

            return Math.Round(result, DefaultNumberOfDecimalPlaces);
        }

        /// <summary>
        /// Determines the amount of N fertilizer required for the specified crop type and yield
        /// </summary>
        public double CalculateRequiredNitrogenFertilizer(
            Farm farm,
            CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            var defaults = farm.Defaults;

            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogenTotal(
                carbonInputFromAgriculturalProduct: viewItem.PlantCarbonInAgriculturalProduct,
                nitrogenConcentrationInProduct: viewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: viewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: viewItem.NitrogenContentInStraw);

            var nitrogenContentOfRootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: viewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: viewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: viewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: viewItem.NitrogenContentInExtraroot);

            var isLeguminousCrop = viewItem.CropType.IsLeguminousCoverCrop() || viewItem.CropType.IsPulseCrop();

            var syntheticFertilizerApplied = _singleYearNitrogenEmissionsCalculator.CalculateSyntheticFertilizerApplied(
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
        /// Equation 2.5.2-6
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
        /// Calculates emissions from all manure that remains after all field applications have been considered.
        /// </summary>
        public SoilN2OEmissionsResults CalculateManureN2OEmissionsForFarm(FarmEmissionResults farmEmissionResults)
        {
            var result = new SoilN2OEmissionsResults()
            {
                LandEmissionSource = LandEmissionSourceType.Manure,
                DirectN2OEmissions = 0,
                IndirectN2OEmissions = 0,
                Name = LandEmissionSourceType.Manure.GetDescription(),
            };

            // Equation 4.6.1-3
            var weightedEmissionFactor = _singleYearNitrogenEmissionsCalculator.CalculateWeightedOrganicNitrogenEmissionFactor(farmEmissionResults);

            /*
             * Direct emissions from manure nitrogen inputs from all fields on the farm.
             */

            var totalManureNitrogenAppliedToFields = this.CalculateTotalManureNitrogenAppliedToAllFields(farmEmissionResults);

            /*
             * All manure nitrogen applied to fields will need to be subtracted from the total amount of manure produced on farm. Any remaining amount is still associated with
             * the farm and must be accounted for in the 'left over' manure.
             */

            var remainingManure = this.CalculateTotalNitrogenFromLandManureRemaining(
                totalManureAvailableForLandApplication: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                totalManureAlreadyAppliedToFields: totalManureNitrogenAppliedToFields,
                totalManureExported: 0);

            // Equation 4.6.1-5
            var directEmissionsFromManureApplication = this.CalculateTotalEmissionsFromRemainingManureThatIsAppliedToAllFields(
                weightedEmissionFactor: weightedEmissionFactor,
                totalNitrogenFromLandManureRemaining: remainingManure);

            // Equation 2.5.3-1
            var fractionLeach = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farmEmissionResults.Farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farmEmissionResults.Farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.5.3-4
            var emissionsFromLeachingAndRunoff = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToLeachingAndRunoffFromAllLandAppliedManure(
                totalNitrogenInputsFromLandAppliedManureNitrogen: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                fractionOfNitrogenLostByLeachingAndRunoff: fractionLeach,
                emissionsFactorForLeachingAndRunoff: farmEmissionResults.Farm.Defaults.EmissionFactorForLeachingAndRunoff);

            // Equation 2.5.3-7
            var emissionsFromVolatilization = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToVolatilizationOfAllLandAppliedManure(
                totalNitrogenInputsFromManure: farmEmissionResults.TotalAvailableManureNitrogenInStoredManureAvailableForLandApplication,
                fractionOfNitrogenLostByVolatilization: farmEmissionResults.Farm.Defaults.FractionOfNLostByVolatilization,
                emissionFactorForVolatilization: farmEmissionResults.Farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.5.4-4
            var indirectEmissionsFromManureApplication = _singleYearNitrogenEmissionsCalculator.CalculateTotalIndirectNitrogenEmissions(
                emissionsDueToLeachingAndRunoff: emissionsFromLeachingAndRunoff,
                emissionsDueToVolatilization: emissionsFromVolatilization);

            // Equation 2.5.4-5
            var totalNEmissions = _singleYearNitrogenEmissionsCalculator.CalculateTotalNitrogenEmissions(
                totalDirectNitrogenEmissions: directEmissionsFromManureApplication,
                totalIndirectNitrogenEmissions: indirectEmissionsFromManureApplication);

            var convertedDirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: directEmissionsFromManureApplication);

            var convertedIndirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: indirectEmissionsFromManureApplication);

            result.DirectN2OEmissions = convertedDirectEmissions;
            result.IndirectN2OEmissions = convertedIndirectEmissions;

            return result;
        }

        /// <summary>
        /// Calculate N2O from crop residues and synthetic fertilizer only. Mineralized nitrogen emissions and land applied manure emissions are not calculated here.
        /// </summary>
        public SoilN2OEmissionsResults CalculateCropN2OEmissions(
            FieldSystemComponent fieldSystemComponent,
            Farm farm)
        {
            var result = new SoilN2OEmissionsResults()
            {
                LandEmissionSource = LandEmissionSourceType.Crop,
                Name = fieldSystemComponent.Name + " - " + fieldSystemComponent.CropString,
                FieldSystemComponent = fieldSystemComponent,
            };

            var viewItem = fieldSystemComponent.GetSingleYearViewItem();
            if (viewItem == null)
            {
                return result;
            }

            // From v3 GrasslandForm.vb: If Grassland (native or non native) has fertilizer or is irrigated then it is improved grassland and should be calculated, else it is unimproved and should not be calculated
            if (viewItem.CropType.IsPerennial() && viewItem.IsImprovedGrassland == false)
            {
                return result;
            }

            // Fallow emissions are not considered for N20
            if (viewItem.CropType.IsFallow())
            {
                return result;
            }

            var emissionFactorForSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateSyntheticNitrogenEmissionFactor(viewItem, farm);
            var emissionFactorForOrganicFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateOrganicNitrogenEmissionFactor(viewItem, farm);
            var emissionFactorForCropResidues = _singleYearNitrogenEmissionsCalculator.GetEmissionFactorForCropResidues(viewItem, farm);

            var inputsFromSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenInputsFromSyntheticFertilizer(
                    fertilizerApplied: viewItem.NitrogenFertilizerRate,
                    area: viewItem.Area);

            result.EmissionsFromSyntheticFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromSyntheticFetilizer(
                inputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                factor: emissionFactorForSyntheticFertilizer);

            /*
             * Note that the next calculations are for organic fertilizer applications made and not manure applications that were made.
             *
             * One being applied from the fertilizer application interface and the other being applied from the manure application interface
             */

            // Equation 2.5.2-10
            var totalOrganicNitrogenInputs = viewItem.GetTotalOrganicNitrogenInYear() / viewItem.Area;

            result.EmissionsFromOrganicFertilizer = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromOrganicFetilizer(
                inputsFromOrganicFertilizer: totalOrganicNitrogenInputs,
                factor: emissionFactorForOrganicFertilizer);

            // Equation 2.6.6-2
            var aboveGroundResidueNitrogen = this.CalculateAboveGroundResidueNitrogen(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                farm: farm);

            result.AboveGroundResidueNitrogen = aboveGroundResidueNitrogen;

            var belowGroundResidueNitrogen = this.CalculateBelowGroundResidueNitrogen(
                previousYearViewItem: null,
                currentYearViewItem: viewItem,
                farm: farm);

            result.BelowGroundResidueNitrogen = belowGroundResidueNitrogen;

            result.InputFromResiduesReturned = _singleYearNitrogenEmissionsCalculator.CalculateInputsFromResidueReturned(
                aboveGroundResidue: aboveGroundResidueNitrogen,
                belowGroundResidue: belowGroundResidueNitrogen,
                area: viewItem.Area);

            result.EmissionsFromInputsFromResidues = _singleYearNitrogenEmissionsCalculator.CalculateEmissionsFromResidues(
                inputFromResidueReturnedToSoil: result.InputFromResiduesReturned,
                emissionFactor: emissionFactorForCropResidues);

            // Equation 2.5.3-1
            var fractionLeach = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration);

            // Equation 2.5.3-2
            var emissionsFromLeaching = _singleYearNitrogenEmissionsCalculator.CalculateCropLeach(
                inputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                inputsFromResidueReturned: result.InputFromResiduesReturned,
                factionLeach: fractionLeach,
                emissionFactorLeachingRunoff: farm.Defaults.EmissionFactorForLeachingAndRunoff);

            var fractionOfNitrogenLostByVolatilization = _singleYearNitrogenEmissionsCalculator.CalculateFractionOfNitrogenLostByVolatilization(
                cropViewItem: viewItem,
                farm);

            // Equation 2.5.3-5
            var emissionsFromVolatilization = _singleYearNitrogenEmissionsCalculator.CalculateNitrogenEmissionsDueToVolatizationFromCropland(
                nitrogenInputsFromSyntheticFertilizer: inputsFromSyntheticFertilizer,
                fractionOfNitrogenLostByVolatilization: fractionOfNitrogenLostByVolatilization,
                emissionFactorForVolatilization: farm.Defaults.EmissionFactorForVolatilization);

            // Equation 2.5.4-1
            var directEmissionsFromCrops = _singleYearNitrogenEmissionsCalculator.CalculateTotalDirectEmissionsForCrop(
                emissionsFromSyntheticFertilizer: result.EmissionsFromSyntheticFertilizer,
                emissionsFromResidues: result.EmissionsFromInputsFromResidues,
                emissionsFromOrganicFertilizer: result.EmissionsFromOrganicFertilizer);

            // Equation 2.5.4-4
            var indirectEmissionsFromCrops = _singleYearNitrogenEmissionsCalculator.CalculateTotalIndirectNitrogenEmissions(
                emissionsDueToLeachingAndRunoff: emissionsFromLeaching,
                emissionsDueToVolatilization: emissionsFromVolatilization);

            // Equation 2.5.4-5
            var totalNEmissions = _singleYearNitrogenEmissionsCalculator.CalculateTotalNitrogenEmissions(
                totalDirectNitrogenEmissions: directEmissionsFromCrops,
                totalIndirectNitrogenEmissions: indirectEmissionsFromCrops);

            var convertedDirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: directEmissionsFromCrops);

            var convertedIndirectEmissions = _singleYearNitrogenEmissionsCalculator.ConvertN2ONToN2O(
                n2ONEmissions: indirectEmissionsFromCrops);

            result.DirectN2OEmissions = convertedDirectEmissions;
            result.IndirectN2OEmissions = convertedIndirectEmissions;

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Equation 2.6.2-2
        /// </summary>
        private double CalculateAboveGroundResidueNitrogen(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfGrainReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateGrainNitrogen(
                carbonInputFromProduct: currentYearViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: currentYearViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateStrawNitrogen(
                carbonInputFromStraw: currentYearViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: currentYearViewItem.NitrogenContentInStraw);

            var aboveGroundNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateAboveGroundResidueNitrogen(
                nitrogenContentOfGrainReturned: nitrogenContentOfGrainReturnedToSoil,
                nitrogenContentOfStrawReturned: nitrogenContentOfStrawReturnedToSoil);

            return aboveGroundNitrogen;
        }

        /// <summary>
        /// Equation 2.6.2-5
        /// </summary>
        private double CalculateBelowGroundResidueNitrogen(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            _icbmSoilCarbonCalculator.SetCarbonInputs(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            var nitrogenContentOfRootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateRootNitrogen(
                carbonInputFromRoots: currentYearViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: currentYearViewItem.NitrogenContentInRoots);

            var nitrogenContentOfExtrarootReturnedToSoil = _singleYearNitrogenEmissionsCalculator.CalculateExtrarootNitrogen(
                carbonInputFromExtraroots: currentYearViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: currentYearViewItem.NitrogenContentInExtraroot);

            var belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator.CalculateBelowGroundResidueNitrogen(
                nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil);

            if (currentYearViewItem.CropType.IsPerennial())
            {
                if (farm.EnableCarbonModelling)
                {
                    // Use the stand length as determined by the sequence of perennial crops entered by the user (Hay-Hay-Hay-Wheat = 3 year stand)
                    belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator
                        .CalculateBelowGroundResidueNitrogenForPerennialForage(
                            nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                            nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil,
                            perennialStandLength: currentYearViewItem.PerennialStandLength);
                }
                else
                {
                    // Use the input that specifies when the perennial was first seeded
                    belowGroundNitrogen = _singleYearNitrogenEmissionsCalculator
                        .CalculateBelowGroundResidueNitrogenForPerennialForage(
                            nitrogenContentOfRootReturned: nitrogenContentOfRootReturnedToSoil,
                            nitrogenContentOfExtrarootReturned: nitrogenContentOfExtrarootReturnedToSoil,
                            perennialStandLength: currentYearViewItem.YearsSinceYearOfConversion);
                };
            }

            return belowGroundNitrogen;
        }



        #endregion
    }
}