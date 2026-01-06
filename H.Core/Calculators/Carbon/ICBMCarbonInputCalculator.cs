using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Services.LandManagement;

namespace H.Core.Calculators.Carbon
{
    public class ICBMCarbonInputCalculator : CarbonInputCalculatorBase, IICBMCarbonInputCalculator
    {
        #region Constructors

        public ICBMCarbonInputCalculator()
        {
        }

        #endregion

        #region Public Methods

        public CropViewItem AssignInputs(CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm, List<AnimalComponentEmissionsResults> animalResults)
        {
            manureService.Initialize(farm, animalResults);
            digestateService.Initialize(farm, animalResults);

            var isNonSwathingGrazingScenario = farm.IsNonSwathingGrazingScenario(currentYearViewItem);

            currentYearViewItem.PlantCarbonInAgriculturalProduct = this.CalculatePlantCarbonInAgriculturalProduct(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm: farm);

            currentYearViewItem.CarbonInputFromProduct = this.CalculateCarbonInputFromProduct(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm);

            if (isNonSwathingGrazingScenario)
            {
                // Total C losses from grazing animals is calculated in Equation 11.3.2-4

                // Equation 11.3.2-6
                currentYearViewItem.PlantCarbonInAgriculturalProduct = currentYearViewItem.TotalCarbonLossesByGrazingAnimals / currentYearViewItem.Area;

                // Equation 11.3.2-7
                currentYearViewItem.CarbonInputFromProduct = (currentYearViewItem.TotalCarbonLossesByGrazingAnimals - currentYearViewItem.TotalCarbonUptakeByAnimals) / currentYearViewItem.Area;

                // Equation 11.3.2-9
                var moistureContent = currentYearViewItem.GrazingViewItems.Any() ? currentYearViewItem.GrazingViewItems.Average(x => x.MoistureContentAsPercentage) : 1;
                var totalYieldForArea = (currentYearViewItem.TotalCarbonLossesByGrazingAnimals / farm.Defaults.CarbonConcentration) / (1 - (moistureContent / 100.0));

                // Convert to per hectare
                currentYearViewItem.Yield = totalYieldForArea / currentYearViewItem.Area;
            }

            currentYearViewItem.CarbonInputFromStraw = this.CalculateCarbonInputFromStraw(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm: farm);

            currentYearViewItem.CarbonInputFromRoots = this.CalculateCarbonInputFromRoots(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm: farm);

            currentYearViewItem.CarbonInputFromExtraroots = this.CalculateCarbonInputFromExtraroot(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm: farm);

            currentYearViewItem.AboveGroundCarbonInput = this.CalculateTotalAboveGroundCarbonInput(
                cropViewItem: currentYearViewItem,
                farm: farm);

            var supplementalFeedingAmount = this.CalculateInputsFromSupplementalHayFedToGrazingAnimals(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItems: nextYearViewItem,
                farm: farm);

            // Add in any supplemental feeding amounts that were given to grazing animals
            currentYearViewItem.AboveGroundCarbonInput += supplementalFeedingAmount;

            currentYearViewItem.BelowGroundCarbonInput = this.CalculateTotalBelowGroundCarbonInput(
                cropViewItem: currentYearViewItem,
                farm: farm);

            this.AssignManureInputs(previousYearViewItem, currentYearViewItem, nextYearViewItem, farm, animalResults);

            currentYearViewItem.DigestateCarbonInputsPerHectare = digestateService.GetTotalDigestateCarbonInputsForField(farm, currentYearViewItem.Year, currentYearViewItem);
            currentYearViewItem.DigestateCarbonInputsPerHectareFromApplicationsOnly = currentYearViewItem.GetTotalCarbonFromAppliedDigestate(ManureLocationSourceType.Livestock) / currentYearViewItem.Area;

            currentYearViewItem.TotalCarbonInputs = currentYearViewItem.AboveGroundCarbonInput + currentYearViewItem.BelowGroundCarbonInput + currentYearViewItem.ManureCarbonInputsPerHectare + currentYearViewItem.DigestateCarbonInputsPerHectare;

            return currentYearViewItem;
        }

        public void AssignManureInputs(CropViewItem previousYearViewItem, CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem, Farm farm, List<AnimalComponentEmissionsResults> animalResults)
        {
            base.AssignManureCarbonInputs(currentYearViewItem, farm, animalResults);
        }

        public double CalculatePlantCarbonInAgriculturalProduct(
            CropViewItem previousYearViewItem, 
            CropViewItem currentYearViewItem, 
            Farm farm)
        {
            if (currentYearViewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct)
            {
                return currentYearViewItem.PlantCarbonInAgriculturalProduct;
            }

            if (currentYearViewItem.CropType.IsFallow() || currentYearViewItem.CropType == CropType.NotSelected)
            {
                return 0;
            }

            // Old farms fix
            if (currentYearViewItem.MoistureContentOfCrop > 1)
            {
                currentYearViewItem.MoistureContentOfCrop /= 100;
            }

            var result = 0d;
            var moistureContentFraction = currentYearViewItem.MoistureContentOfCrop;
            var isGrazed = currentYearViewItem.GrazingViewItems.Any();
            if (isGrazed)
            {
                moistureContentFraction = (currentYearViewItem.GrazingViewItems.Average(x => x.MoistureContentAsPercentage) / 100.0);
            }

            var isCustomYieldAssignmentMethod = farm.YieldAssignmentMethod == YieldAssignmentMethod.Custom;
            var isAllProductReturned = Math.Abs(currentYearViewItem.PercentageOfProductYieldReturnedToSoil - 100) < double.Epsilon;
            var isSwathing = currentYearViewItem.HarvestMethod == HarvestMethods.Swathing;
            var isGreenManure = currentYearViewItem.HarvestMethod == HarvestMethods.GreenManure;
            var isCustomYieldAndIsGrazed = isCustomYieldAssignmentMethod && isGrazed;
            var hasHarvest = currentYearViewItem.GetHayHarvests().Any();
            var isCustomYieldAndNoHarvestAndNoGrazing = isCustomYieldAssignmentMethod && (hasHarvest == false) && (isGrazed == false);

            var moistureContentAdjustment = (1.0 - moistureContentFraction);
            var carbonConcentration = currentYearViewItem.CarbonConcentration;
            var yield = currentYearViewItem.Yield;

            if (isAllProductReturned || isSwathing || isGreenManure || isCustomYieldAndIsGrazed || isCustomYieldAndNoHarvestAndNoGrazing)
            {
                result = yield * moistureContentAdjustment * carbonConcentration;
            }
            else
            {
                var firstTerm = yield / (1 - (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100.0));

                result = firstTerm * moistureContentAdjustment * carbonConcentration;
            }

            return result;
        }

        public double EstimatePlantCarbonInAgriculturalProductForNextYear(
            CropViewItem nextYearViewItem,
            Farm farm)
        {
            if (nextYearViewItem == null)
            {
                // If there is no data for the next year, there can be no estimated value
                return 0;
            }

            var year = nextYearViewItem.Year;
            var moistureContentAsPercentage = nextYearViewItem.MoistureContentOfCropPercentage;
            var carbonConcentration = nextYearViewItem.CarbonConcentration;
            var totalPrecipitationForTheYear = farm.ClimateData.GetTotalPrecipitationForYear(year);
            var totalEvapotranspirationForTheYear = farm.ClimateData.GetTotalEvapotranspirationForYear(year);
            var proportionOfPrecipitationMayThroughSeptember = farm.ClimateData.ProportionOfPrecipitationFallingInMayThroughSeptember(year);

            var result = this.CalculateProductivity(
                annualPrecipitation: totalPrecipitationForTheYear,
                annualPotentialEvapotranspiration: totalEvapotranspirationForTheYear,
                proportionOfPrecipitationMayThroughSeptember: proportionOfPrecipitationMayThroughSeptember,
                carbonConcentration: carbonConcentration);

            return result;
        }

        public double CalculateProductivity(double annualPrecipitation,
            double annualPotentialEvapotranspiration,
            double proportionOfPrecipitationMayThroughSeptember,
            double carbonConcentration)
        {
            var production = (2.973 + (0.00453 * annualPrecipitation) + (-0.00259 * annualPotentialEvapotranspiration) + (6.187 * proportionOfPrecipitationMayThroughSeptember));
            var dryMatter = Math.Pow(Math.E, production);

            var result = dryMatter * carbonConcentration;

            return result;
        }

        public double CalculateAboveGroundCarbonInputFromPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm)
        {
            // Estimate the value using the productivity calculation
            var estimatedPlantCarbonInAgriculturalProductInNextYear = this.EstimatePlantCarbonInAgriculturalProductForNextYear(
                nextYearViewItem: currentYearViewItem,
                farm: farm);

            // C_ptoSoil
            var carbonInputFromProduct = 0.0;
            if (currentYearViewItem.YearInPerennialStand == 1)
            {
                /*
                 * Consider the first year
                 */

                if (currentYearViewItem.PlantCarbonInAgriculturalProduct > 0)
                {
                    /* Equation 2.1.2-20
                     *
                     * Situation when C_p for current year (first year in this condition) is known
                     */

                    carbonInputFromProduct = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    var isCustomYieldAssignmentMethod = farm.YieldAssignmentMethod == YieldAssignmentMethod.Custom;
                    var isGrazed = currentYearViewItem.HasGrazingViewItems;

                    if (isGrazed && isCustomYieldAssignmentMethod)
                    {
                        var returned = 1.0 - (currentYearViewItem.GetAverageUtilizationFromGrazingAnimals() / 100.0);
                        carbonInputFromProduct = currentYearViewItem.PlantCarbonInAgriculturalProduct * returned;
                    }
                }
                else if (currentYearViewItem.PlantCarbonInAgriculturalProduct == 0 && nextYearViewItem != null && (nextYearViewItem.PlantCarbonInAgriculturalProduct > 0 || nextYearViewItem.Yield > 0))
                {
                    /* Equation 2.1.2-21
                     *
                     * Situation when C_p for the current year (first year in this condition) is not known, but yield or C_p for subsequent year (second year in this condition)
                     * is known.
                     *
                     * If the next years C_p has been set already we use that, otherwise we need to calculate it here.
                     */

                    var plantCarbonInAgriculturalProductForNextYear = 0.0;
                    if (nextYearViewItem.PlantCarbonInAgriculturalProduct > 0)
                    {
                        // We will already have a calculated C_p value for the next year when the previous year set it for us (down below)
                        plantCarbonInAgriculturalProductForNextYear = nextYearViewItem.PlantCarbonInAgriculturalProduct;
                    }
                    else
                    {
                        plantCarbonInAgriculturalProductForNextYear = this.CalculatePlantCarbonInAgriculturalProduct(
                            previousYearViewItem: null,
                            currentYearViewItem: nextYearViewItem,
                            farm: farm);
                    }

                    // This year's C_p will be a fraction of next year's C_p

                    // Equation 2.1.2-23
                    var thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears = plantCarbonInAgriculturalProductForNextYear * farm.Defaults.EstablishmentGrowthFactorFractionForPerennials;

                    // Equation 2.1.2-24
                    carbonInputFromProduct = thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    // Since this year doesn't have any C_p, we assign the value now so that when we calculate C_r and C_e for this year, we will have a C_p value to work with
                    currentYearViewItem.PlantCarbonInAgriculturalProduct = thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears;

                }
                else
                {
                    /*
                     * Situation when the C_p is not known for the current year (first year in this condition) or the subsequent year (second year in this condition). Have to use estimated value
                     */

                    // This year's C_p will be a fraction of the estimated value for next year's C_p 
                    var thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears = (estimatedPlantCarbonInAgriculturalProductInNextYear * farm.Defaults.EstablishmentGrowthFactorFractionForPerennials);

                    carbonInputFromProduct = thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    // Since this year doesn't have any C_p, we assign the value now so that when we calculate C_r and C_e for this year, we will have a C_p value to work with
                    currentYearViewItem.PlantCarbonInAgriculturalProduct = thisYearsPlantCarbonInAgriculturalProductAsAFractionOfNextYears;

                    if (nextYearViewItem != null)
                    {
                        /* Equation 2.1.2-25 
                         *
                         * Set the the C_p for next year to be the calculated value
                         */
                        nextYearViewItem.PlantCarbonInAgriculturalProduct = estimatedPlantCarbonInAgriculturalProductInNextYear;

                        // Since we are assigning a calculated value to the next year's C_p value, we need to set a flag so that it doesn't get overwritten when we calculate C_p on the next iteration
                        nextYearViewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;
                    }
                }
            }
            else
            {
                /* Equation 2.1.2-27
                 *  
                 * Consider any year other than the first
                 */

                if (currentYearViewItem.PlantCarbonInAgriculturalProduct > 0)
                {
                    /*
                     * Situation when C_p for the current year is available
                     */

                    carbonInputFromProduct = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    var isCustomYieldAssignmentMethod = farm.YieldAssignmentMethod == YieldAssignmentMethod.Custom;
                    var isGrazed = currentYearViewItem.HasGrazingViewItems;

                    if (isGrazed && isCustomYieldAssignmentMethod)
                    {
                        var returned = 1.0 - (currentYearViewItem.GetAverageUtilizationFromGrazingAnimals() / 100.0);
                        carbonInputFromProduct = currentYearViewItem.PlantCarbonInAgriculturalProduct * returned;
                    }
                }
                else
                {
                    /*
                     * Situation when C_p for the current year is not known. Have to use estimated value
                     */

                    var estimatedPlantC = estimatedPlantCarbonInAgriculturalProductInNextYear * farm.Defaults.EstablishmentGrowthFactorFractionForPerennials;

                    carbonInputFromProduct = (estimatedPlantC) * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    // Since this year doesn't have any C_p, we assign the value now so that when we calculate C_r and C_e for this year, we will have a C_p value to work with
                    currentYearViewItem.PlantCarbonInAgriculturalProduct = estimatedPlantC;
                }
            }

            // Note that the straw inputs are not considered when calculating above ground inputs from perennials - just the product (where the 'straw' is considered to be the product)
            return carbonInputFromProduct;
        }

        public double CalculateCarbonInputFromProduct(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm)
        {
            if (currentYearViewItem.CropType.IsPerennial())
            {
                return this.CalculateAboveGroundCarbonInputFromPerennials(
                    previousYearViewItem: previousYearViewItem,
                    currentYearViewItem: currentYearViewItem,
                    nextYearViewItem: nextYearViewItem,
                    farm: farm);
            }

            var result = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100.0);

            return result;
        }

        public double CalculateCarbonInputFromStraw(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            if (currentYearViewItem.CropType.IsFallow() ||                              // No inputs from fallow fields
                currentYearViewItem.CropType == CropType.NotSelected ||                 // Need a crop type to calculate input
                currentYearViewItem.HarvestMethod == HarvestMethods.GreenManure ||      // In these two harvest method cases, the residue fractions for product and straw are combined and so the inputs from straw are omitted
                currentYearViewItem.HarvestMethod == HarvestMethods.Swathing ||
                currentYearViewItem.CropType.IsPerennial())                             // All above ground inputs from perennials are from the product and not the straw
            {
                return 0;
            }

            // Some crop types do not have values for biomass of product, since dividing by 0 will cause NaN (in the next calculation below) return 0
            if (Math.Abs(currentYearViewItem.BiomassCoefficientProduct) < double.Epsilon)
            {
                return 0;
            }

            var result = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientStraw / currentYearViewItem.BiomassCoefficientProduct) * (currentYearViewItem.PercentageOfStrawReturnedToSoil / 100);

            return result;
        }

        public double CalculateCarbonInputFromRootsForPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            // Next line divides by biomass of product so it can't be zero
            if (currentYearViewItem.BiomassCoefficientProduct == 0)
            {
                return 0;
            }

            var carbonInput = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientRoots / currentYearViewItem.BiomassCoefficientProduct);
            if (carbonInput < 450)
            {
                carbonInput = 450;
            }

            // Equation 2.1.2-28
            var carbonInputFromRoots = carbonInput * (currentYearViewItem.PercentageOfRootsReturnedToSoil / 100.0);
            if (currentYearViewItem.YearInPerennialStand == 1)
            {
                return carbonInputFromRoots;
            }

            // We only consider the previous year if that year was growing the same perennial. It is possible the previous year was not a year in the same perennial (i.e. previous year could have been Barley)
            if (previousYearViewItem != null && (previousYearViewItem.PerennialStandGroupId.Equals(currentYearViewItem.PerennialStandGroupId)))
            {
                // Equation 2.1.2-30
                carbonInputFromRoots = previousYearViewItem.CarbonInputFromRoots + (previousYearViewItem.CarbonInputFromRoots * (19.35 / 100.0));

                if (currentYearViewItem.YearInPerennialStand > 5)
                {
                    carbonInputFromRoots = previousYearViewItem.CarbonInputFromRoots;
                }
            }

            return carbonInputFromRoots;
        }

        public double CalculateCarbonInputFromRoots(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            if (currentYearViewItem.CropType.IsFallow() ||                              // No inputs from fallow fields
                currentYearViewItem.CropType == CropType.NotSelected)                   // Need a crop type to calculate input
            {
                return 0;
            }

            if (currentYearViewItem.CropType.IsPerennial() || currentYearViewItem.CropType == CropType.GrassSilage)
            {
                return this.CalculateCarbonInputFromRootsForPerennials(
                    previousYearViewItem: previousYearViewItem,
                    currentYearViewItem: currentYearViewItem,
                    farm: farm);
            }

            // This is a special case when using an annual crop as green manure or swathed (see note under annual crop C input section)
            if (currentYearViewItem.HarvestMethod == HarvestMethods.GreenManure || currentYearViewItem.HarvestMethod == HarvestMethods.Swathing)
            {
                return this.CalculateCarbonInputFromRootsForGreenManureOrSwathing(
                    previousYearViewItem,
                    currentYearViewItem,
                    farm);
            }

            // Some crop types do not have values for biomass of product, since dividing by 0 will cause NaN (in the next calculation below) return 0 instead.
            if (Math.Abs(currentYearViewItem.BiomassCoefficientProduct) < double.Epsilon)
            {
                return 0;
            }

            var result = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientRoots / currentYearViewItem.BiomassCoefficientProduct) * (currentYearViewItem.PercentageOfRootsReturnedToSoil / 100);

            return result;
        }

        public double CalculateCarbonInputFromRootsForGreenManureOrSwathing(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            var combinedBiomassCoefficientOfProduct = currentYearViewItem.BiomassCoefficientProduct + currentYearViewItem.BiomassCoefficientStraw;
            if (combinedBiomassCoefficientOfProduct == 0)
            {
                return 0;
            }

            var greenManureResult = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientRoots / combinedBiomassCoefficientOfProduct) * (currentYearViewItem.PercentageOfRootsReturnedToSoil / 100);

            return greenManureResult;
        }

        public double CalculateCarbonInputFromExtraroot(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            if (currentYearViewItem.CropType.IsFallow() ||                              // No inputs from fallow fields
                currentYearViewItem.CropType == CropType.NotSelected)                   // Need a crop type to calculate input
            {
                return 0;
            }

            if (currentYearViewItem.CropType.IsPerennial() || currentYearViewItem.CropType == CropType.GrassSilage)
            {
                return this.CalculateCarbonInputFromExtrarootsForPerennials(
                    previousYearViewItem: previousYearViewItem,
                    currentYearViewItem: currentYearViewItem,
                    farm: farm);
            }

            // This is a special case when using an annual crop as green manure (see note under annual crop C input section)
            if (currentYearViewItem.HarvestMethod == HarvestMethods.GreenManure || currentYearViewItem.HarvestMethod == HarvestMethods.Swathing)
            {
                return this.CalculateCarbonInputFromExtrarootsForGreenManureOrSwathing(
                    previousYearViewItem,
                    currentYearViewItem,
                    farm);
            }

            // Some crop types do not have values for biomass of product, since dividing by 0 will cause NaN (in the next calculation below) return 0 instead.
            if (Math.Abs(currentYearViewItem.BiomassCoefficientProduct) < double.Epsilon)
            {
                return 0;
            }

            var result = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientExtraroot / currentYearViewItem.BiomassCoefficientProduct);

            return result;
        }

        public double CalculateCarbonInputFromExtrarootsForPerennials(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            // Next line divides by biomass of product so it can't be zero
            if (currentYearViewItem.BiomassCoefficientProduct == 0)
            {
                return 0;
            }

            // Equation 2.1.2-29
            // Equation 2.1.2-31
            var carbonInputFromExtraroots = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientExtraroot / currentYearViewItem.BiomassCoefficientProduct) * (currentYearViewItem.PercentageOfExtraRootsReturnedToSoil / 100.0);

            // Taken out now as the extra roots only ever depend on the C_p and not the extraroots from the previous year
            //// We only consider the previous year if that year was growing the same perennial. It is possible the previous year was not a year in the same perennial (i.e. previous year could have been Barley)
            //if (previousYearViewItem != null && (previousYearViewItem.PerennialStandGroupId.Equals(currentYearViewItem.PerennialStandGroupId)) && carbonInputFromExtraroots < previousYearViewItem.CarbonInputFromExtraroots)
            //{
            //    // Equation 2.1.2-33
            //    carbonInputFromExtraroots = previousYearViewItem.CarbonInputFromExtraroots;
            //}

            return carbonInputFromExtraroots;
        }

        public double CalculateCarbonInputFromExtrarootsForGreenManureOrSwathing(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            Farm farm)
        {
            var combinedBiomassCoefficientOfProduct = currentYearViewItem.BiomassCoefficientProduct + currentYearViewItem.BiomassCoefficientStraw;
            if (combinedBiomassCoefficientOfProduct == 0)
            {
                return 0;
            }

            var greenManureResult = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientExtraroot / combinedBiomassCoefficientOfProduct);

            return greenManureResult;
        }

        public double CalculateTotalAboveGroundCarbonInput(
            CropViewItem cropViewItem,
            Farm farm)
        {
            // There are no inputs from straw when the harvest method is green manure or swathing
            if (cropViewItem.HarvestMethod == HarvestMethods.GreenManure || cropViewItem.HarvestMethod == HarvestMethods.Swathing)
            {
                return cropViewItem.CarbonInputFromProduct;
            }

            if (cropViewItem.IsSelectedCropTypeRootCrop == false)
            {
                return cropViewItem.CarbonInputFromProduct + cropViewItem.CarbonInputFromStraw;
            }
            else
            {
                return cropViewItem.CarbonInputFromStraw;
            }
        }

        public double CalculateTotalBelowGroundCarbonInput(
            CropViewItem cropViewItem,
            Farm farm)
        {
            if (cropViewItem.IsSelectedCropTypeRootCrop == false)
            {
                return cropViewItem.CarbonInputFromRoots + cropViewItem.CarbonInputFromExtraroots;
            }
            else
            {
                return cropViewItem.CarbonInputFromProduct + cropViewItem.CarbonInputFromExtraroots;
            }
        }

        #endregion
    }
}