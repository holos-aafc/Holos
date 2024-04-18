#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Services.LandManagement;

#endregion

namespace H.Core.Calculators.Carbon
{
    /// <summary>
    /// </summary>
    public partial class ICBMSoilCarbonCalculator : CarbonCalculatorBase, IICBMSoilCarbonCalculator
    {
        #region Fields


        #endregion

        #region Constructors

        public ICBMSoilCarbonCalculator(IClimateProvider climateProvider, N2OEmissionFactorCalculator n2OEmissionFactorCalculator)
        {
            if (climateProvider != null)
            {
                _climateProvider = climateProvider;
            }
            else
            {
                throw new ArgumentNullException(nameof(climateProvider));
            }

            if (n2OEmissionFactorCalculator != null)
            {
                base.N2OEmissionFactorCalculator = n2OEmissionFactorCalculator;
            }
            else
            {
                throw new ArgumentNullException(nameof(n2OEmissionFactorCalculator));
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public CropViewItem SetCarbonInputs(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItem,
            Farm farm)
        {
            var isNonSwathingGrazingScenario = currentYearViewItem.TotalCarbonLossesByGrazingAnimals > 0 &&
                                    farm.CropHasGrazingAnimals(currentYearViewItem) &&
                                    farm.YieldAssignmentMethod != YieldAssignmentMethod.Custom &&
                                    currentYearViewItem.HarvestMethod != HarvestMethods.StubbleGrazing &&
                                    currentYearViewItem.HarvestMethod != HarvestMethods.Swathing;

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
                var totalYieldForArea = currentYearViewItem.TotalCarbonLossesByGrazingAnimals / farm.Defaults.CarbonConcentration;

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

            currentYearViewItem.ManureCarbonInputsPerHectare = this.N2OEmissionFactorCalculator.ManureService.GetTotalManureCarbonInputsForField(farm, currentYearViewItem.Year, currentYearViewItem);

            if (farm.CropHasGrazingAnimals(currentYearViewItem))
            {
                currentYearViewItem.ManureCarbonInputsPerHectare += currentYearViewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture;
            }

            currentYearViewItem.DigestateCarbonInputsPerHectare = this.CalculateDigestateCarbonInputPerHectare(currentYearViewItem, farm);

            currentYearViewItem.TotalCarbonInputs = currentYearViewItem.AboveGroundCarbonInput + currentYearViewItem.BelowGroundCarbonInput + currentYearViewItem.ManureCarbonInputsPerHectare + currentYearViewItem.DigestateCarbonInputsPerHectare;            

            return currentYearViewItem;
        }

        private double RecalculateYield(CropViewItem cropViewItem)
        {
            var result = (cropViewItem.PlantCarbonInAgriculturalProduct / cropViewItem.CarbonConcentration);


            return result;
        }

        /// <summary>
        /// Calculates the plant carbon in the agricultural product for the given species grown in the given year.
        /// 
        /// C_p
        ///
        /// Equation 2.1.2-1
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The total above ground carbon input</returns>
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
            if (Math.Abs(currentYearViewItem.PercentageOfProductYieldReturnedToSoil - 100) < double.Epsilon)
            {
                // If all product is returned to soil, use this calculation otherwise when 100% of product is returned, a doubling of yield will be used as in below calculation. 
                // 100 % of product will be returned when crops are being used as 'green manure' e.g. lentils
                result = (currentYearViewItem.Yield) * (1 - currentYearViewItem.MoistureContentOfCrop) * currentYearViewItem.CarbonConcentration;                
            }
            else
            {
                if (currentYearViewItem.HarvestMethod == HarvestMethods.Swathing || currentYearViewItem.HarvestMethod == HarvestMethods.GreenManure)
                {
                    result = ((currentYearViewItem.Yield) * (1 - currentYearViewItem.MoistureContentOfCrop)) * currentYearViewItem.CarbonConcentration;
                }
                else
                {
                    result = ((currentYearViewItem.Yield + currentYearViewItem.Yield * (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100)) * (1 - currentYearViewItem.MoistureContentOfCrop)) * currentYearViewItem.CarbonConcentration;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the total above ground carbon input for the given species grown in the given year.
        /// 
        /// C_ag
        /// 
        /// Equation 2.1.2-2
        /// Equation 2.1.2-4
        /// </summary>
        /// <param name="cropViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The total above ground carbon input</returns>
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

        /// <summary>
        /// Calculates the total below ground carbon input for the given species grown in the given year.
        /// 
        /// C_bg
        ///
        /// Equation 2.1.2-3
        /// Equation 2.1.2-5
        /// </summary>
        /// <param name="cropViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The total below ground carbon input</returns>
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

        /// <summary>
        /// Calculates the carbon input from the product for the given species grown in the given year.
        /// 
        /// C_ptoSoil
        /// 
        /// Equation 2.1.2-6
        /// Equation 2.1.2-10
        /// Equation 2.1.2-14
        /// Equation 2.1.2-17
        /// Equation 2.1.2-20
        /// Equation 2.1.2-23
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the subsequent year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from the product</returns>
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

        /// <summary>
        /// Calculates the carbon input from straw for the given species grown in the given year.
        /// 
        /// C_s
        ///
        /// Equation 2.1.2-7
        /// Equation 2.1.2-18
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from straw</returns>
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

        /// <summary>
        /// Calculates the carbon input from roots for the given species grown in the given year.
        ///
        /// C_r
        /// 
        /// Equation 2.1.2-8
        /// Equation 2.1.2-11
        /// Equation 2.1.2-15
        /// Equation 2.1.2-21
        /// Equation 2.1.2-24
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from roots</returns>
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

            if (currentYearViewItem.CropType.IsPerennial())
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

        /// <summary>
        /// Calculates the carbon input from extraroots for the given species grown in the given year.
        /// 
        /// C_e
        /// 
        /// Equation 2.1.2-9
        /// Equation 2.1.2-12
        /// Equation 2.1.2-16
        /// Equation 2.1.2-19
        /// Equation 2.1.2-25
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
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

            if (currentYearViewItem.CropType.IsPerennial())
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

        /// <summary>
        /// Calculates the total above ground carbon input for a year in which a perennial crop is grown. Note that for perennials, all above ground inputs are
        /// contributed by the C_ptoSoil as there is no C_s calculation for perennials.
        ///
        /// Side effect: if this year doesn't have any C_p, a value will be assigned
        /// 
        /// C_ag (which is equivalent to the C_ptoSoil when considering a perennial)
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the subsequent year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The total above ground carbon input</returns>
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
                }
                else
                {
                    /*
                     * Situation when C_p for the current year is not known. Have to use estimated value
                     */

                    var estimatedPlantC = estimatedPlantCarbonInAgriculturalProductInNextYear * farm.Defaults.EstablishmentGrowthFactorFractionForPerennials;

                    carbonInputFromProduct = (estimatedPlantC)* (currentYearViewItem.PercentageOfProductYieldReturnedToSoil / 100);

                    // Since this year doesn't have any C_p, we assign the value now so that when we calculate C_r and C_e for this year, we will have a C_p value to work with
                    currentYearViewItem.PlantCarbonInAgriculturalProduct = estimatedPlantC;
                }
            }

            // Note that the straw inputs are not considered when calculating above ground inputs from perennials - just the product (where the 'straw' is considered to be the product)
            return carbonInputFromProduct;
        }

        /// <summary>
        /// Calculates the carbon input from roots for a year in which a perennial crop is grown.
        /// 
        /// C_r
        ///
        /// Note: the <see cref="FieldResultsService.AssignDefaultBiomassCoefficients"/> sets the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.BiomassCoefficientProduct"/> to the value of the biomass coefficient of straw returned by the provider for perennials.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from roots</returns>
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

            // Equation 2.1.2-28
            var carbonInputFromRoots = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientRoots / currentYearViewItem.BiomassCoefficientProduct) * (currentYearViewItem.PercentageOfRootsReturnedToSoil / 100.0);
            if (carbonInputFromRoots < 450)
            {
                carbonInputFromRoots = 450;
            }

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

        /// <summary>
        /// Calculates the carbon input from extraroots for a year in which a perennial crop is grown.
        /// 
        /// C_e
        ///
        /// Note: the <see cref="FieldResultsService.AssignDefaultBiomassCoefficients"/> sets the <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.BiomassCoefficientProduct"/> to the value of the biomass coefficient of straw returned by the provider for perennials.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
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
            var carbonInputFromExtraroots = currentYearViewItem.PlantCarbonInAgriculturalProduct * (currentYearViewItem.BiomassCoefficientExtraroot / currentYearViewItem.BiomassCoefficientProduct);

            // Taken out now as the extra roots only ever depend on the C_p and not the extraroots from the previous year
            //// We only consider the previous year if that year was growing the same perennial. It is possible the previous year was not a year in the same perennial (i.e. previous year could have been Barley)
            //if (previousYearViewItem != null && (previousYearViewItem.PerennialStandGroupId.Equals(currentYearViewItem.PerennialStandGroupId)) && carbonInputFromExtraroots < previousYearViewItem.CarbonInputFromExtraroots)
            //{
            //    // Equation 2.1.2-33
            //    carbonInputFromExtraroots = previousYearViewItem.CarbonInputFromExtraroots;
            //}

            return carbonInputFromExtraroots;
        }

        /// <summary>
        /// Estimates the plant carbon in agricultural product for the subsequent year (t + 1) when considering perennial stands with missing yield data.
        /// 
        /// C_p
        /// </summary>
        /// <param name="nextYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the subsequent (t + 1) year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The estimated plant carbon in agricultural product</returns>
        private double EstimatePlantCarbonInAgriculturalProductForNextYear(
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

        /// <summary>
        /// Equation 2.1.2-22
        /// Equation 2.1.2-26
        /// </summary>
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

        /// <summary>
        /// Calculates the carbon input from roots for the given species grown in the given year when the harvest method is green manure.
        /// 
        /// C_r
        ///
        /// When green manure is chosen as a harvest option for a main crop (meaning the user is entering an above ground biomass value instead of a grain yield), the residue fractions for product
        /// and straw are combined to form a single coefficient, Rp, and Cs is omitted.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from roots</returns>
        private double CalculateCarbonInputFromRootsForGreenManureOrSwathing(
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

        /// <summary>
        /// Calculates the carbon input from extraroots for the given species grown in the given year when the harvest method is green manure.
        /// 
        /// C_e
        ///
        /// When green manure is chosen as a harvest option for a main crop (meaning the user is entering an above ground biomass value instead of a grain yield), the residue fractions for product
        /// and straw are combined to form a single coefficient, Rp, and Cs is omitted.
        /// </summary>
        /// <param name="previousYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the previous year</param>
        /// <param name="currentYearViewItem">The details of the <see cref="FieldSystemComponent"/> in the current year</param>
        /// <param name="farm">The <see cref="Farm"/> being considered</param>
        /// <returns>The carbon input from extraroots</returns>
        private double CalculateCarbonInputFromExtrarootsForGreenManureOrSwathing(
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

        /// <summary>
        /// Equation 2.2.2-27
        /// </summary>
        public double CalculateAmountOfNitrogenAppliedFromManure(
            double manureAmount, 
            double fractionOfNitrogenInAppliedManure)
        {
            return manureAmount * fractionOfNitrogenInAppliedManure;
        }

        /// <summary>
        /// Equation 2.2.2-28
        /// </summary>
        public double CalculateAmountOfPhosphorusAppliedFromManure(
            double manureAmount, 
            double fractionOfPhosphorusInAppliedManure)
        {
            return manureAmount * fractionOfPhosphorusInAppliedManure;
        }

        /// <summary>
        /// Equation 2.2.2-29
        /// </summary>
        public double CalculateMoistureOfManure(
            double manureAmount, 
            double waterFraction)
        {
            return manureAmount * waterFraction / 10000;
        }

        /// <summary>
        /// Equation 2.1.3-1
        /// </summary>
        public double CalculateAverageAboveGroundResidueCarbonInput(
            double carbonInputFromProductOfEachRotationPhase, 
            double carbonInputFromStrawOfEachRotationPhase)
        {
            return carbonInputFromProductOfEachRotationPhase + carbonInputFromStrawOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-2
        /// </summary>
        public double CalculateAverageBelowGroundResidueCarbonInput(
            double carbonInputFromRootsOfEachRotationPhase, 
            double carbonInputFromExtrarootOfEachRotationPhase)
        {
            return carbonInputFromExtrarootOfEachRotationPhase + carbonInputFromRootsOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-3
        /// </summary>
        public double CalculateAverageManureCarbonInput(double carbonInputsFromManureInputsOfEachRotationPhase)
        {
            return carbonInputsFromManureInputsOfEachRotationPhase;
        }

        /// <summary>
        /// Equation 2.1.3-4
        /// </summary>
        public double CalculateYoungPoolSteadyStateAboveGround(
            double averageAboveGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageAboveGroundCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-5
        /// </summary>
        public double CalculateYoungPoolSteadyStateBelowGround(
            double averageBelowGroundCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageBelowGroundCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-6
        /// </summary>
        public double CalculateYoungPoolSteadyStateManure(
            double averageManureCarbonInput, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var numerator = averageManureCarbonInput * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var denominator = 1 - Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = numerator / denominator;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-7
        /// </summary>
        public double CalculateOldPoolSteadyState(
            double youngPoolDecompositionRate, 
            double oldPoolDecompositionRate, 
            double climateParameter,
            double aboveGroundHumificationCoefficient, 
            double belowGroundHumificationCoefficient, 
            double averageAboveGroundCarbonInputOfRotation, 
            double averageBelowGroundCarbonInputOfRotation, 
            double aboveGroundYoungPoolSteadyState, 
            double belowGroundYoungPoolSteadyState, 
            double manureYoungPoolSteadyState,
            double averageManureCarbonInputOfRotation, 
            double manureHumificationCoefficient)
        {
            var firstFactorNumerator = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter) - Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);
            var firstFactorDenominator = 1 - Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);

            var secondFactorNumeratorFactorOne = aboveGroundHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorTwo = aboveGroundYoungPoolSteadyState + averageAboveGroundCarbonInputOfRotation;

            var secondFactorNumeratorFactorThree = belowGroundHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorFour = belowGroundYoungPoolSteadyState + averageBelowGroundCarbonInputOfRotation;

            var secondFactorNumeratorFactorFive = manureHumificationCoefficient * youngPoolDecompositionRate;
            var secondFactorNumeratorFactorSix = manureYoungPoolSteadyState + averageManureCarbonInputOfRotation;

            var secondFactorNumerator = secondFactorNumeratorFactorOne * secondFactorNumeratorFactorTwo +
                                        secondFactorNumeratorFactorThree * secondFactorNumeratorFactorFour +
                                        secondFactorNumeratorFactorFive * secondFactorNumeratorFactorSix;

            var secondFactorDenominator = oldPoolDecompositionRate - youngPoolDecompositionRate;

            var result = (firstFactorNumerator / firstFactorDenominator) * (secondFactorNumerator / secondFactorDenominator);

            return result;
        }


        /// <summary>
        /// Equation 2.1.3-11
        /// </summary>
        public double CalculateYoungPoolAboveGroundCarbonAtInterval(
            double youngPoolAboveGroundCarbonAtPreviousInterval, 
            double aboveGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolAboveGroundCarbonAtPreviousInterval + aboveGroundCarbonAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-12
        /// </summary>
        public double CalculateYoungPoolBelowGroundCarbonAtInterval(
            double youngPoolBelowGroundCarbonAtPreviousInterval, 
            double belowGroundCarbonAtPreviousInterval, 
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolBelowGroundCarbonAtPreviousInterval + belowGroundCarbonAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-13
        /// </summary>
        public double CalculateYoungPoolManureCarbonAtInterval(
            double youngPoolManureCarbonAtPreviousInterval, 
            double manureCarbonInputAtPreviousInterval,
            double youngPoolDecompositionRate, 
            double climateParameter)
        {
            var firstFactor = youngPoolManureCarbonAtPreviousInterval + manureCarbonInputAtPreviousInterval;
            var secondFactor = Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstFactor * secondFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-14
        /// </summary>
        /// <returns></returns>
        public double CalculateOldPoolSoilCarbonAtInterval(
            double oldPoolSoilCarbonAtPreviousInterval, 
            double aboveGroundHumificationCoefficient, 
            double belowGroundHumificationCoefficient, 
            double youngPoolDecompositionRate, 
            double oldPoolDecompositionRate, 
            double youngPoolAboveGroundOrganicCarbonAtPreviousInterval, 
            double youngPoolBelowGroundOrganicCarbonAtPreviousInterval, 
            double aboveGroundCarbonResidueAtPreviousInterval, 
            double belowGroundCarbonResidueAtPreviousInterval, 
            double climateParameter, 
            double youngPoolManureAtPreviousInterval, 
            double manureHumificationCoefficient, 
            double manureCarbonInputAtPreviousInterval)
        {
            var decompositionRateDifference = oldPoolDecompositionRate - youngPoolDecompositionRate;

            var aboveGroundDivisionTermNumerator = youngPoolDecompositionRate *
                                                   (youngPoolAboveGroundOrganicCarbonAtPreviousInterval +
                                                    aboveGroundCarbonResidueAtPreviousInterval); 
            var aboveGroundDivisionTerm = aboveGroundHumificationCoefficient *
                                          (aboveGroundDivisionTermNumerator / decompositionRateDifference);

            var belowGroundDivisionTermNumerator = youngPoolDecompositionRate *
                                                   (youngPoolBelowGroundOrganicCarbonAtPreviousInterval +
                                                    belowGroundCarbonResidueAtPreviousInterval);
            var belowGroundDivisionTerm = belowGroundHumificationCoefficient *
                                          (belowGroundDivisionTermNumerator / decompositionRateDifference);

            var manureDivisionTermNumerator = youngPoolDecompositionRate *
                                              (youngPoolManureAtPreviousInterval + manureCarbonInputAtPreviousInterval);
            var manureDivisionTerm = manureHumificationCoefficient *
                                     (manureDivisionTermNumerator / decompositionRateDifference);

            var firstTerm =
                (oldPoolSoilCarbonAtPreviousInterval - aboveGroundDivisionTerm - belowGroundDivisionTerm -
                 manureDivisionTerm) * Math.Exp(-1 * oldPoolDecompositionRate * climateParameter);
            var secondTerm = aboveGroundDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var thirdTerm = belowGroundDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);
            var fourthTerm = manureDivisionTerm * Math.Exp(-1 * youngPoolDecompositionRate * climateParameter);

            var result = firstTerm + secondTerm + thirdTerm + fourthTerm;

            return result;
        }

        /// <summary>
        /// Equation 2.1.3-15
        /// </summary>
        public double CalculateSoilCarbonAtInterval(
            double youngPoolSoilCarbonAboveGroundAtInterval, 
            double youngPoolSoilCarbonBelowGroundAtInterval, 
            double oldPoolSoilCarbonAtInterval, 
            double youngPoolManureAtInterval)
        {
            return youngPoolSoilCarbonAboveGroundAtInterval + youngPoolSoilCarbonBelowGroundAtInterval + oldPoolSoilCarbonAtInterval + youngPoolManureAtInterval;
        }

        /// <summary>
        /// Equation 2.1.3-16
        /// </summary>
        public double CalculateChangeInSoilCarbonAtInterval(
            double soilOrganicCarbonAtInterval, 
            double soilOrganicCarbonAtPreviousInterval)
        {
            return soilOrganicCarbonAtInterval - soilOrganicCarbonAtPreviousInterval;
        }

        /// <summary>
        /// Equation 2.1.3-17
        /// </summary>
        public double CalculateChangeInSoilOrganicCarbonForFieldAtInterval(
            double changeInSoilOrganicCarbonAtInterval, 
            double fieldArea)
        {
            return changeInSoilOrganicCarbonAtInterval * fieldArea;
        }

        /// <summary>
        /// Equation 2.1.3-18
        /// </summary>
        public double CalculateChangeInSoilOrganicCarbonForFarmAtInterval(
            IEnumerable<double> changeInSoilOrganicCarbonForFields)
        {
            return changeInSoilOrganicCarbonForFields.Sum();
        }

        /// <summary>
        /// Equation 2.1.4-1
        /// </summary>
        public double CalculateCarbonDioxideEquivalentsForSoil(double soilOrganicCarbonAtInterval)
        {
            var carbonDioxideEquivalentForSoil = soilOrganicCarbonAtInterval * (44.0 / 12.0);
            return carbonDioxideEquivalentForSoil;
        }

        /// <summary>
        /// Equation 2.1.4-2
        /// </summary>
        public double CalculateChangeInCarbonDioxideEquivalentsForSoil(double changeInSoilOrganicCarbonAtInterval)
        {
            var changeInCarbonDioxideEquivalentsForSoil = changeInSoilOrganicCarbonAtInterval * (44.0 / 12.0);
            return changeInCarbonDioxideEquivalentsForSoil;
        }

        /// <summary>
        /// Equation 2.1.4-3
        /// </summary>
        public double CalculateCarbonDioxideChangeForSoilsByMonth(double changeInCarbonDioxideEquivalentsForSoil)
        {
            var carbonDioxideChangeForSoilByMonth = changeInCarbonDioxideEquivalentsForSoil / 12.0;
            return carbonDioxideChangeForSoilByMonth;
        }

        /// <summary>
        /// Equation 11.3.2-4
        /// </summary>
        /// <returns>Total carbon losses from grazing animals (kg C)</returns>
        public double RecalculatePlantCarbonForGrazingScenario(
            CropViewItem viewItem)
        {
            var lossesFromGrazing = viewItem.TotalCarbonLossesByGrazingAnimals;

            var averageUtilizationRate = viewItem.GrazingViewItems.Any() ? viewItem.GrazingViewItems.Average(x => x.Utilization) : 0;
            var denominator = 1 - (averageUtilizationRate / 100.0);
            if (denominator < 0)
            {
                denominator = 1;
            }

            var uptake = lossesFromGrazing / denominator;

            return uptake;
        }

        public double RecalculateCarbonInputForGrazingScenario(
            CropViewItem viewItem)
        {
            // Check for negative values here

            var result = viewItem.PlantCarbonInAgriculturalProduct - (viewItem.TotalCarbonLossesByGrazingAnimals / viewItem.Area);
            if (result < 0)
            {
                return viewItem.PlantCarbonInAgriculturalProduct;
            }

            return result;
        }

        #endregion
    }
}