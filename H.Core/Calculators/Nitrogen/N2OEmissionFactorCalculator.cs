
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public partial class N2OEmissionFactorCalculator : IN2OEmissionFactorCalculator
    {
        #region Fields

        private readonly Table_13_Soil_N2O_Emission_Factors_Provider _soilN2OEmissionFactorsProvider = new Table_13_Soil_N2O_Emission_Factors_Provider();
        private readonly EcodistrictDefaultsProvider _ecodistrictDefaultsProvider = new EcodistrictDefaultsProvider();

        #endregion

        #region Properties
        
        public IManureService ManureService { get; set; }
        public IDigestateService DigestateService { get; set; }

        public IClimateProvider ClimateProvider { get; set; }

        public IAnimalEmissionFactorsProvider LivestockEmissionConversionFactorsProvider { get; set; }
        public IAnimalAmmoniaEmissionFactorProvider AnimalAmmoniaEmissionFactorProvider { get; set; }

        #endregion

        #region Constructors

        public N2OEmissionFactorCalculator(IClimateProvider climateProvider)
        {
            this.ManureService = new ManureService();
            this.DigestateService = new DigestateService();

            if (climateProvider != null)
            {
                this.ClimateProvider = climateProvider;
            }

            this.LivestockEmissionConversionFactorsProvider = new Table_36_Livestock_Emission_Conversion_Factors_Provider();
            this.AnimalAmmoniaEmissionFactorProvider = new Table_43_Beef_Dairy_Default_Emission_Factors_Provider();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.6.6-1
        /// Equation 2.6.6-2
        /// </summary>
        private double CalculateLeachingFraction(
            double precipitation,
            double potentialEvapotranspiration)
        {
            var result = 0.3247 * (precipitation / potentialEvapotranspiration) - 0.0247;
            if (result <= 0.05)
            {
                return 0.05;
            }
            else if (result >= 0.3)
            {
                return 0.3;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Equation 4.6.2-1
        /// </summary>
        /// <param name="averageDailyTemperature">The temperature (degrees C) when manure is applied.</param>
        /// <returns>Ambient temperature-based adjustment</returns>
        public double CalculateAmbientTemperatureAdjustmentForLandApplication(double averageDailyTemperature)
        {
            var result = 1 - (0.058 * (15 - averageDailyTemperature));

            if (result > 1)
            {
                return 1;
            }
            else if (result < 0)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        public double CalculateBaseEcodistrictFactor(
            Farm farm, 
            CropViewItem viewItem)
        {
            var soilData = farm.GetPreferredSoilData(viewItem);

            var fractionOfLandOccupiedByLowerPortionsOfLandscape = _ecodistrictDefaultsProvider.GetFractionOfLandOccupiedByPortionsOfLandscape(
                ecodistrictId: soilData.EcodistrictId,
                province: soilData.Province);

            viewItem.FractionOfLandOccupiedByLowerPortionsOfLandscape = fractionOfLandOccupiedByLowerPortionsOfLandscape;

            var emissionsDueToLandscapeAndTopography = this.CalculateTopographyEmissions(
                fractionOfLandOccupiedByLowerPortionsOfLandscape: fractionOfLandOccupiedByLowerPortionsOfLandscape,
                growingSeasonPrecipitation: farm.GetGrowingSeasonPrecipitation(viewItem.Year),
                growingSeasonEvapotranspiration: farm.GetGrowingSeasonEvapotranspiration(viewItem.Year));

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictValue(
                topographyEmission: emissionsDueToLandscapeAndTopography,
                soilTexture: soilData.SoilTexture,
                region: soilData.Province.GetRegion());

            viewItem.WeightedModifierBasedOnTexture = this.CalculateModifierBasedOnTexture(soilData.SoilTexture, soilData.Province.GetRegion(), 1);

            return baseEcodistrictFactor;
        }

        public double CalculateSyntheticNitrogenEmissionFactor(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem == null)
            {
                return 0;
            }

            if (farm.Defaults.UseCustomN2OEmissionFactor)
            {
                return farm.Defaults.CustomN2OEmissionFactor;
            }

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm, viewItem);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var soilData = farm.GetPreferredSoilData(viewItem);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: soilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var nitrogenSourceModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.SyntheticNitrogen, 
                cropViewItem: viewItem);

            var soilReductionFactor = _soilN2OEmissionFactorsProvider.GetReductionFactorBasedOnApplicationMethod(viewItem.SoilReductionFactor);
            if (viewItem.SoilReductionFactor == SoilReductionFactors.Custom)
            {
                soilReductionFactor = viewItem.CustomReductionFactor;
            }

            var syntheticNitrogenEmissionFactor = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier, 
                applicationMethodReductionFactor: soilReductionFactor);

            return syntheticNitrogenEmissionFactor;
        }

        public double CalculateOrganicNitrogenEmissionFactor(
            CropViewItem viewItem,
            Farm farm)
        {
            if (viewItem == null)
            {
                return 0;
            }

            if (farm.Defaults.UseCustomN2OEmissionFactor)
            {
                return farm.Defaults.CustomN2OEmissionFactor;
            }

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm, viewItem);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var soilData = farm.GetPreferredSoilData(viewItem);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: soilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var nitrogenSourceModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.OrganicNitrogen, 
                cropViewItem: viewItem);

            var ecodistrictManureEmissionFactor = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: nitrogenSourceModifier);

            return ecodistrictManureEmissionFactor;
        }

        public double GetEmissionFactorForCropResidues(
            CropViewItem viewItem, 
            Farm farm)
        {
            if (viewItem == null)
            {
                return 0;
            }

            if (farm.Defaults.UseCustomN2OEmissionFactor)
            {
                return farm.Defaults.CustomN2OEmissionFactor;
            }

            var baseEcodistrictFactor = this.CalculateBaseEcodistrictFactor(farm, viewItem);

            var croppingSystemModifier = _soilN2OEmissionFactorsProvider.GetFactorForCroppingSystem(
                cropType: viewItem.CropType);

            var soilData = farm.GetPreferredSoilData(viewItem);

            var tillageModifier = _soilN2OEmissionFactorsProvider.GetFactorForTillagePractice(
                region: soilData.Province.GetRegion(),
                cropViewItem: viewItem);

            var cropResidueModifier = _soilN2OEmissionFactorsProvider.GetFactorForNitrogenSource(
                nitrogenSourceType: Table_13_Soil_N2O_Emission_Factors_Provider.NitrogenSourceTypes.CropResidueNitrogen, 
                cropViewItem: viewItem);

            var emissionFactorForCropResidues = this.CalculateEmissionFactor(
                baseEcodistictEmissionFactor: baseEcodistrictFactor,
                croppingSystemModifier: croppingSystemModifier,
                tillageModifier: tillageModifier,
                nitrogenSourceModifier: cropResidueModifier);

            return emissionFactorForCropResidues;
        }

        /// <summary>
        /// Equation 2.6.6-13
        /// Equation 2.7.5-13
        /// 
        /// Frac_volatilizationSoil
        ///
        /// <para>This value used to be a constant (0.1) but is now calculated according to crop type, fertilizer type, etc.</para>
        ///
        /// <para>Implements: Table 14. Coefficients used for the Bouwman et al. (2002) equation, which was of the form: emission factor (%) = 100 x exp (sum of relevant coefficients)</para>
        /// </summary>
        /// <returns>Fraction of N lost by volatilization (kg N (kg N)-1)</returns>
        public double CalculateFractionOfNitrogenLostByVolatilization(
            CropViewItem cropViewItem,
            Farm farm)
        {
            var cropTypeFactor = 0.0;
            if (cropViewItem.CropType.IsPerennial())
            {
                cropTypeFactor = -0.158;
            }
            else
            {
                // Annuals
                cropTypeFactor = -0.045;
            }

            // Take average of coefficients if there is more than one application
            var fertilizerTypeFactor = 0.0;
            var fertlizerTypeFactors = new List<double>();
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
            {
                if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.Urea)
                {
                    fertilizerTypeFactor = 0.666;
                }
                else if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.UreaAmmoniumNitrate)
                {
                    fertilizerTypeFactor = 0.282;
                }
                else if (fertilizerApplicationViewItem.FertilizerBlendData.FertilizerBlend == FertilizerBlends.AmmoniumSulphate)
                {
                    fertilizerTypeFactor = -1.151;
                }
                else
                {
                    // Other
                    fertilizerTypeFactor = -0.238;
                }

                fertlizerTypeFactors.Add(fertilizerTypeFactor);
            }

            fertilizerTypeFactor = fertlizerTypeFactors.Any() ? fertlizerTypeFactors.Average() : 0;

            var methodOfApplicationFactor = 0.0;
            var methodOfApplicationFactors = new List<double>();
            foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
            {
                // Footnote 1: Broadcast application of fertilizer is assumed for perennials
                if (fertilizerApplicationViewItem.FertilizerApplicationMethodology == FertilizerApplicationMethodologies.Broadcast)
                {
                    methodOfApplicationFactor = -1.305;
                }
                else
                {
                    methodOfApplicationFactor = -1.895;
                }

                methodOfApplicationFactors.Add(methodOfApplicationFactor);
            }

            methodOfApplicationFactor = methodOfApplicationFactors.Any() ? methodOfApplicationFactors.Average() : 0;

            var soilData = farm.GetPreferredSoilData(cropViewItem);

            var soilPhFactor = 0.0;
            if (soilData.SoilPh < 7.25)
            {
                soilPhFactor = -1;
            }
            else
            {
                soilPhFactor = -0.608;
            }

            var soilCecFactor = 0.0;
            if (soilData.SoilCec < 250)
            {
                soilCecFactor = 0.0507;
            }
            else
            {
                soilCecFactor = 0.0848;
            }

            const double temperatureFactor = -0.402;

            var result = Math.Exp(cropTypeFactor + fertilizerTypeFactor + methodOfApplicationFactor + soilPhFactor + soilCecFactor + temperatureFactor);

            return result;
        }

        /// <summary>
        /// Equation 2.5.1-1
        /// Equation 2.5.1-2
        /// </summary>
        /// <param name="precipitation">Growing season precipitation, by ecodistrict (May – October)</param>
        /// <param name="potentialEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>Ecodistrict emission factor [kg N2O-N (kg N)-1]</returns>
        public double CalculateEcodistrictEmissionFactor(
            double precipitation, 
            double potentialEvapotranspiration)
        {
            if (precipitation > potentialEvapotranspiration)
            {
                var result = this.CalculateEmissionFactorUsingPrecipitation(precipitation);

                return result;
            }
            else
            {
                var result = this.CalculateEmissionFactorUsingPotentialEvapotranspiration(potentialEvapotranspiration);

                return result;
            }
        }

        /// <summary>
        /// Equation 2.5.2-1
        /// Equation 2.5.2-2
        /// Equation 2.5.2-3
        /// </summary>
        /// <param name="fractionOfLandOccupiedByLowerPortionsOfLandscape">Fraction of land occupied by lower portions of landscape (from Rochette et al. 2008)</param>
        /// <param name="growingSeasonPrecipitation">Annual growing season precipitation (May – October)</param>
        /// <param name="growingSeasonEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>N2O emission factor adjusted due to position in landscape and moisture regime (kg N2O-N)</returns>
        public double CalculateTopographyEmissions(
            double fractionOfLandOccupiedByLowerPortionsOfLandscape,
            double growingSeasonPrecipitation,
            double growingSeasonEvapotranspiration)
        {
            if (Math.Abs(growingSeasonEvapotranspiration) < double.Epsilon)
            {
                return 0;
            }

            var emissionFactorUsingPotentialEvapotranspiration = this.CalculateEmissionFactorUsingPotentialEvapotranspiration(growingSeasonEvapotranspiration);
            var emissionFactorUsingPrecipitation = this.CalculateEmissionFactorUsingPrecipitation(growingSeasonPrecipitation);

            var emissionFactorForIrrigatedSites = emissionFactorUsingPotentialEvapotranspiration;
            var emissionFactorForHumidEnvironments = this.CalculateEcodistrictEmissionFactor(growingSeasonPrecipitation, growingSeasonEvapotranspiration);
            var emissionFactorForDryEnvironments = emissionFactorUsingPotentialEvapotranspiration * fractionOfLandOccupiedByLowerPortionsOfLandscape + (emissionFactorUsingPrecipitation * (1 - fractionOfLandOccupiedByLowerPortionsOfLandscape));

            var result = 0.0;

            /*
             * Note the base Ef is not multiplied here by 100 as it is in the algorithm document since the FTopo value is a percentage in the lookup table and subsequently divided by 100. This negates the need to multiply
             * by 100 as in the algorithm document.
             */

            // For irrigated sites
            if (Math.Abs(growingSeasonPrecipitation - growingSeasonEvapotranspiration) < double.Epsilon)
            {
                result = emissionFactorForIrrigatedSites;
            }
            
            // For humid environments
            if ((growingSeasonPrecipitation / growingSeasonEvapotranspiration) > 1)
            {
                result = emissionFactorForHumidEnvironments;
            }

            // For dry environments
            if ((growingSeasonPrecipitation / growingSeasonEvapotranspiration) <= 1)
            {
                result = emissionFactorForDryEnvironments;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.5.3-1
        /// </summary>
        /// <param name="soilTexture">The soil texture of the ecodistrict</param>
        /// <param name="region">The region of the ecodistrict</param>
        /// <param name="fractionOfThisTexture">The fraction of the ecodistrict that is comprised of this soil texture type (100% for now)</param>
        /// <returns>A weighted modifier which provides a correction of the EF_Topo in ecodistrict ‘‘i’’ based on the soil texture </returns>
        public double CalculateModifierBasedOnTexture(
            SoilTexture soilTexture, 
            Region region, 
            double fractionOfThisTexture)
        {
            var textureFactor = _soilN2OEmissionFactorsProvider.GetFactorForSoilTexture(
                soilTexture: soilTexture,
                region: region);

            var result = textureFactor * fractionOfThisTexture;

            return result;
        }

        /// <summary>
        /// Equation 2.5.3-2
        /// </summary>
        /// <param name="topographyEmission">N2O emission factor adjusted due to position in landscape and moisture regime (kg N2O-N)</param>
        /// <param name="soilTexture">The soil texture of the ecodistrict</param>
        /// <param name="region">The region of the ecodistrict</param>
        /// <returns>A function of the three factors that create a base ecodistrict specific value that accounts for the climatic, topographic and edaphic characteristics of the spatial unit for lands</returns>
        public double CalculateBaseEcodistrictValue(
            double topographyEmission, 
            SoilTexture soilTexture, 
            Region region)
        {
            var textureModifier = this.CalculateModifierBasedOnTexture(
                    soilTexture: soilTexture,
                    region: region,

                    /*
                     * This is 1 for now since we allow the user to use a single texture in calculations only (i.e. this texture comprises 100% of the area). This
                     * might change in the future
                     */

                    fractionOfThisTexture: 1
                );

            const double winterCorrection = (1.0 / 0.645);

            var result = topographyEmission * textureModifier * winterCorrection;

            return result;
        }

        /// <summary>
        /// Equation 2.5.4-1
        /// </summary>
        /// <param name="baseEcodistictEmissionFactor">A function of the three factors that create a base ecodistrict specific value that accounts for the climatic, topographic and edaphic characteristics of the spatial unit for lands</param>
        /// <param name="croppingSystemModifier">Cropping system modifier (Ann = Annual, Per = Perennial)</param>
        /// <param name="tillageModifier">tillage modifier RF_Till (Conservation or Conventional Tillage)</param>
        /// <param name="nitrogenSourceModifier">N source modifier RF_NSk (SN = Synthetic Nitrogen; ON = Organic Nitrogen; CRN = Crop Residue Nitrogen)</param>
        /// <param name="applicationMethodReductionFactor">Reduction factor based on application method, only applicable to calculations of EF specific for SN</param>
        /// <returns>The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N source type, tillage, cropping system and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</returns>
        public double CalculateEmissionFactor(double baseEcodistictEmissionFactor,
            double croppingSystemModifier,
            double tillageModifier,
            double nitrogenSourceModifier, 
            double applicationMethodReductionFactor = 1.0)
        {
            var result = baseEcodistictEmissionFactor * croppingSystemModifier * tillageModifier * nitrogenSourceModifier * applicationMethodReductionFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.5.5-5
        /// Equation 2.5.5-6
        /// </summary>
        /// <param name="nitrogenContentOfGrainReturnedToSoil">Nitrogen content of the grain returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfStrawReturnedToSoil">Nitrogen content of the straw returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfRootReturnedToSoil">Nitrogen content of the root returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfExtrarootReturnedToSoil">Nitrogen content of the extraroot returned to the soil (kg N ha^-1)</param>
        /// <param name="fertilizerEfficiencyFraction">Fertilizer use efficiency (fraction)</param>
        /// <param name="soilTestN">User defined value for existing Soil N supply for which fertilization rate was adapted</param>
        /// <param name="isNitrogenFixingCrop">Indicates if the type of crop is nitrogen fixing.</param>
        /// <param name="nitrogenFixationAmount">The amount of nitrogen fixation by the crop (fraction)</param>
        /// <param name="atmosphericNitrogenDeposition">N deposition on a specific field n (kg ha^-1) </param>
        /// <returns>N fertilizer applied (kg ha^-1)</returns>
        public double CalculateSyntheticFertilizerApplied(double nitrogenContentOfGrainReturnedToSoil,
            double nitrogenContentOfStrawReturnedToSoil,
            double nitrogenContentOfRootReturnedToSoil,
            double nitrogenContentOfExtrarootReturnedToSoil,
            double fertilizerEfficiencyFraction,
            double soilTestN,
            bool isNitrogenFixingCrop,
            double nitrogenFixationAmount, 
            double atmosphericNitrogenDeposition)
        {
            var totalNitrogenContent = (nitrogenContentOfGrainReturnedToSoil + nitrogenContentOfStrawReturnedToSoil + nitrogenContentOfRootReturnedToSoil + nitrogenContentOfExtrarootReturnedToSoil);

            var result = 0d;
            if (isNitrogenFixingCrop)
            {
                // Equation 2.5.5-6
                result = (totalNitrogenContent * (1 - nitrogenFixationAmount) - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }
            else
            {
                // Equation 2.5.5-5
                result = (totalNitrogenContent - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }

            // Suggested amount can never be less than zero
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-1
        /// </summary>
        public double CalculateGrainNitrogenTotal(
            double carbonInputFromAgriculturalProduct,
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromAgriculturalProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-2
        /// </summary>
        /// <param name="carbonInputFromProduct">Carbon input from product (kg ha^-1) </param>
        /// <param name="nitrogenConcentrationInProduct">N concentration in the product (kg kg-1) </param>
        public double CalculateNitrogenContentGrainReturnedToSoil(
            double carbonInputFromProduct, 
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-3
        /// </summary>
        /// <param name="carbonInputFromStraw">Carbon input from straw (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInStraw"></param>
        public double CalculateNitrogenContentStrawReturnedToSoil(
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw)
        {
            var result = (carbonInputFromStraw / 0.45) * nitrogenConcentrationInStraw;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-4
        /// </summary>
        /// <param name="carbonInputFromRoots">Carbon input from roots (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInRoots">N concentration in the roots (kg kg-1) </param>
        public double CalculateNitrogenContentRootReturnedToSoil(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots)
        {
            var result = (carbonInputFromRoots / 0.45) * nitrogenConcentrationInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-5
        /// </summary>
        /// <param name="carbonInputFromExtraroots">Carbon input from extra-root material (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInExtraroots">N concentration in the extra root (kg kg-1) (until known from literature, the same N concentration used for roots will be utilized)</param>
        public double CalculateNitrogenContentExaduatesReturnedToSoil(
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots)
        {
            var result = (carbonInputFromExtraroots / 0.45) * nitrogenConcentrationInExtraroots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.6-6
        /// Equation 2.6.2-2
        /// Equation 2.7.2-3
        /// Equation 2.7.2-5
        /// Equation 2.7.2-7
        /// Equation 2.7.2-9
        /// </summary>
        /// <returns>Above ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var nitrogenContentOfGrainReturned = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var nitrogenContentOfStrawReturned = this.CalculateNitrogenContentStrawReturnedToSoil(
                carbonInputFromStraw: cropViewItem.CarbonInputFromStraw,
                nitrogenConcentrationInStraw: cropViewItem.NitrogenContentInStraw);

            if (cropViewItem.CropType.IsAnnual() || cropViewItem.CropType.IsPerennial())
            {
                return nitrogenContentOfGrainReturned + nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return nitrogenContentOfStrawReturned;
            }

            if (cropViewItem.CropType.IsCoverCrop() || cropViewItem.CropType.IsSilageCrop())
            {
                return nitrogenContentOfGrainReturned;
            }

            // Fallow
            return 0;
        }

        /// <summary>
        /// Equation 2.5.6-7
        /// Equation 2.5.6-8
        /// Equation 2.6.2-5
        /// Equation 2.7.2-4
        /// Equation 2.7.2-6
        /// Equation 2.7.2-8
        /// Equation 2.7.2-10
        /// </summary>
        /// <param name="cropViewItem"></param>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIcbm(CropViewItem cropViewItem)
        {
            var grainNitrogen = this.CalculateNitrogenContentGrainReturnedToSoil(
                carbonInputFromProduct: cropViewItem.CarbonInputFromProduct,
                nitrogenConcentrationInProduct: cropViewItem.NitrogenContentInProduct);

            var rootNitrogen = this.CalculateNitrogenContentRootReturnedToSoil(
                carbonInputFromRoots: cropViewItem.CarbonInputFromRoots,
                nitrogenConcentrationInRoots: cropViewItem.NitrogenContentInRoots);

            var exudateNitrogen = this.CalculateNitrogenContentExaduatesReturnedToSoil(
                carbonInputFromExtraroots: cropViewItem.CarbonInputFromExtraroots,
                nitrogenConcentrationInExtraroots: cropViewItem.NitrogenContentInExtraroot);

            // Equation 2.5.6-7
            if (cropViewItem.CropType.IsAnnual())
            {
                return rootNitrogen + exudateNitrogen;
            }

            // Equation 2.5.6-8
            if (cropViewItem.CropType.IsPerennial())
            {
                return (rootNitrogen * (cropViewItem.PercentageOfRootsReturnedToSoil / 100.0)) + exudateNitrogen;
            }

            if (cropViewItem.CropType.IsRootCrop())
            {
                return grainNitrogen + exudateNitrogen;
            }

            if (cropViewItem.CropType.IsSilageCrop() || cropViewItem.CropType.IsCoverCrop())
            {
                return rootNitrogen + exudateNitrogen;
            }

            return 0;
        }

        /// <summary>
        /// Equation 2.7.2-1
        /// </summary>
        /// <returns></returns>
        public double CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
            double aboveGroundResidueDryMatter,
            double carbonConcentration,
            double nitrogenContentInStraw)
        {
            return aboveGroundResidueDryMatter * nitrogenContentInStraw;
        }

        /// <summary>
        /// Equation 2.7.2-2
        /// Equation 2.7.2-4
        /// Equation 2.6.2-6
        /// </summary>
        public double CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(CropViewItem viewItem)
        {
            if (viewItem.CropType.IsPerennial())
            {
                var perennialNitrogen = (viewItem.BelowGroundResidueDryMatter / viewItem.Area) * viewItem.NitrogenContentInRoots * (viewItem.PercentageOfRootsReturnedToSoil / 100.0);

                return perennialNitrogen;
            }
            // Equation 2.7.2-2
            var result = (viewItem.BelowGroundResidueDryMatter / viewItem.Area) * viewItem.NitrogenContentInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-20
        /// </summary>
        public double CalculateWeightedEmissionFactor(IEnumerable<WeightedAverageInput> areasAndEmissionFactors)
        {
            return areasAndEmissionFactors.WeightedAverage(record => record.Value, record => record.Weight);
        }

        /// <summary>
        /// Equation 2.5.3-1
        /// Equation 2.7.5-1
        /// Equation 2.7.5-2
        /// </summary>
        /// <param name="growingSeasonPrecipitation">Growing season precipitation, by ecodistrict (May – October)</param>
        /// <param name="growingSeasonEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>Fraction of N lost by leaching and runoff  (kg N (kg N)^-1)</returns>
        public double CalculateFractionOfNitrogenLostByLeachingAndRunoff(
            double growingSeasonPrecipitation, 
            double growingSeasonEvapotranspiration)
        {
            var fractionOfNitrogenLostByLeachingAndRunoff = 0.3247 * (growingSeasonPrecipitation / growingSeasonEvapotranspiration) - 0.0247;
            if (fractionOfNitrogenLostByLeachingAndRunoff < 0.05)
            {
                return 0.05;
            }

            if (fractionOfNitrogenLostByLeachingAndRunoff > 0.3)
            {
                return 0.3;
            }

            return fractionOfNitrogenLostByLeachingAndRunoff;
        }


        /// <summary>
        /// Combines total emissions for entire area of a field from each manure/digestate application into per hectare emissions
        /// </summary>
        /// <param name="results">The emissions for each field</param>
        /// <param name="viewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> that will be used to calculate per hectare emissions</param>
        /// <returns></returns>
        public LandApplicationEmissionResult ConvertPerFieldEmissionsToPerHectare(
            List<LandApplicationEmissionResult> results,
            CropViewItem viewItem)
        {
            var totalAmountsPerHectareFromManureApplications = new LandApplicationEmissionResult();

            foreach (var landApplicationEmissionResult in results)
            {
                /*
                 * Totals are for the entire field. Convert to per hectare below.
                 */

                totalAmountsPerHectareFromManureApplications.TotalN2ONFromManureLeaching += landApplicationEmissionResult.TotalN2ONFromManureLeaching > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureLeaching / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalIndirectN2ONEmissions += landApplicationEmissionResult.TotalIndirectN2ONEmissions > 0
                    ? landApplicationEmissionResult.TotalIndirectN2ONEmissions / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalNitrateLeached += landApplicationEmissionResult.TotalNitrateLeached > 0
                    ? landApplicationEmissionResult.TotalNitrateLeached / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalIndirectN2OEmissions += landApplicationEmissionResult.TotalIndirectN2OEmissions > 0
                    ? landApplicationEmissionResult.TotalIndirectN2OEmissions / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalN2OFromManureVolatilized += landApplicationEmissionResult.TotalN2OFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2OFromManureVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalN2ONFromManureVolatilized += landApplicationEmissionResult.TotalN2ONFromManureVolatilized > 0
                    ? landApplicationEmissionResult.TotalN2ONFromManureVolatilized / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.TotalVolumeOfManureUsedDuringApplication += landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication > 0
                    ? landApplicationEmissionResult.TotalVolumeOfManureUsedDuringApplication / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AmmoniacalLoss += landApplicationEmissionResult.AmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AmmoniacalLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AdjustedAmmoniaLoss += landApplicationEmissionResult.AdjustedAmmoniaLoss > 0
                    ? landApplicationEmissionResult.AdjustedAmmoniaLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AdjustedAmmoniacalLoss += landApplicationEmissionResult.AdjustedAmmoniacalLoss > 0
                    ? landApplicationEmissionResult.AdjustedAmmoniacalLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.AmmoniaLoss += landApplicationEmissionResult.AmmoniaLoss > 0
                    ? landApplicationEmissionResult.AmmoniaLoss / viewItem.Area
                    : 0;

                totalAmountsPerHectareFromManureApplications.ActualAmountOfNitrogenAppliedFromLandApplication += landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication > 0
                    ? landApplicationEmissionResult.ActualAmountOfNitrogenAppliedFromLandApplication / viewItem.Area
                    : 0;

                // Equation 4.6.2-4
                totalAmountsPerHectareFromManureApplications.TotalTANApplied += landApplicationEmissionResult.TotalTANApplied > 0
                    ? landApplicationEmissionResult.TotalTANApplied / viewItem.Area
                    : 0;
            }

            return totalAmountsPerHectareFromManureApplications;
        }


        public double GetAmountOfNitrogenUsed(CropViewItem viewItem, ManureItemBase manureItemBase)
        {
            return viewItem.Area * manureItemBase.AmountOfNitrogenAppliedPerHectare;
        }

        public double GetAmountOfDigestateNitrogenUsed(CropViewItem viewItem)
        {
            var result = 0d;

            foreach (var viewItemDigestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                result += GetAmountOfNitrogenUsed(viewItem, viewItemDigestateApplicationViewItem);
            }

            return result;
        }

        public double GetAmountOfManureNitrogenUsed(CropViewItem viewItem)
        {
            var result = 0d;

            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                result += GetAmountOfNitrogenUsed(viewItem, manureApplicationViewItem);
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Equation 2.5.1-1
        /// </summary>
        private double CalculateEmissionFactorUsingPrecipitation(double precipitation)
        {
            return Math.Exp((0.00558 * precipitation) - 7.7);
        }

        /// <summary>
        /// Equation 2.5.1-2
        /// </summary>
        private double CalculateEmissionFactorUsingPotentialEvapotranspiration(double potentialEvapotranspiration)
        {
            return Math.Exp((0.00558 * potentialEvapotranspiration) - 7.7);
        }

        #endregion


    }
}