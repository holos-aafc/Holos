using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Soil;
using H.Infrastructure;

namespace H.Core.Calculators.Nitrogen
{
    public class SingleYearNitrousOxideCalculator
    {
        #region Fields

        private readonly Table_15_16_Soil_N2O_Emission_Factors_Provider _soilN2OEmissionFactorsProvider = new Table_15_16_Soil_N2O_Emission_Factors_Provider();

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.5.1-1
        /// Equation 2.5.1-2
        /// </summary>
        /// <param name="precipitation">Growing season precipitation, by ecodistrict (May – October)</param>
        /// <param name="potentialEvapotranspiration">Growing season potential evapotranspiration, by ecodistrict (May – October)</param>
        /// <returns>Ecodistrict emission factor [kg N2O-N (kg N)-1]</returns>
        public double CalculateEcodistrictEmissionFactor(double precipitation,
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
        /// Equation 2.5.1-3
        /// Equation 2.5.1-4
        /// Equation 2.5.1-5
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
        /// Equation 2.5.1-6
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
        /// Equation 2.5.1-7
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
        /// Equation 2.5.1-8
        /// </summary>
        /// <param name="baseEcodistictEmissionFactor">A function of the three factors that create a base ecodistrict specific value that accounts for the climatic, topographic and edaphic characteristics of the spatial unit for lands</param>
        /// <param name="croppingSystemModifier">Cropping system modifier (Ann = Annual, Per = Perennial)</param>
        /// <param name="tillageModifier">tillage modifier RF_Till (Conservation or Conventional Tillage)</param>
        /// <param name="nitrogenSourceModifier">N source modifier RF_NSk (SN = Synthetic Nitrogen; ON = Organic Nitrogen; CRN = Crop Residue Nitrogen)</param>
        /// <returns>The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N sourcve type, tillage, cropping sytem and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</returns>
        public double CalculateEmissionFactor(double baseEcodistictEmissionFactor,
                                      double croppingSystemModifier,
                                      double tillageModifier,
                                      double nitrogenSourceModifier)
        {
            var result = baseEcodistictEmissionFactor * croppingSystemModifier * tillageModifier * nitrogenSourceModifier;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-5
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
                result = (totalNitrogenContent * (1 - nitrogenFixationAmount) - soilTestN - atmosphericNitrogenDeposition) / fertilizerEfficiencyFraction;
            }
            else
            {
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
        /// Equation 2.5.2-2
        /// </summary>
        /// <param name="fertilizerApplied">N fertilizer applied (kg ha^-1)</param>
        /// <param name="area">Area of crop (ha)</param>
        /// <returns>N inputs from synthetic fertilizer (kg N)</returns>
        public double CalculateNitrogenInputsFromSyntheticFertilizer(double fertilizerApplied, 
                                                                     double area)
        {
            var result = fertilizerApplied * area;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-11
        /// </summary>
        /// <param name="inputsFromSyntheticFertilizer">N inputs from synthetic fertilizer (kg N)</param>
        /// <param name="factor">The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N source type, tillage, cropping sytem and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</param>
        /// <returns>N2O emissions (kg N2O-N kg-1 N) resulting from fertilizer application</returns>
        public double CalculateEmissionsFromSyntheticFetilizer(
            double inputsFromSyntheticFertilizer, 
            double factor)
        {
            var result = inputsFromSyntheticFertilizer * factor;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-12
        /// </summary>
        /// <param name="inputsFromOrganicFertilizer">N inputs from organic fertilizer (kg N)</param>
        /// <param name="factor">The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N source type, tillage, cropping sytem and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</param>
        /// <returns>N2O emissions (kg N2O-N kg-1 N) resulting from organic fertilizer application</returns>
        public double CalculateEmissionsFromOrganicFetilizer(
            double inputsFromOrganicFertilizer,
            double factor)
        {
            var result = inputsFromOrganicFertilizer * factor;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-13
        /// </summary>
        public double CalculateGrainNitrogenTotal(
            double carbonInputFromAgriculturalProduct,
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromAgriculturalProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-10
        /// </summary>
        /// <param name="carbonInputFromProduct">Carbon input from product (kg ha^-1) </param>
        /// <param name="nitrogenConcentrationInProduct">N concentration in the product (kg kg-1) </param>
        public double CalculateGrainNitrogen(
            double carbonInputFromProduct, 
            double nitrogenConcentrationInProduct)
        {
            var result = (carbonInputFromProduct / 0.45) * nitrogenConcentrationInProduct;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-11
        /// </summary>
        /// <param name="carbonInputFromStraw">Carbon input from straw (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInStraw"></param>
        public double CalculateStrawNitrogen(
            double carbonInputFromStraw,
            double nitrogenConcentrationInStraw)
        {
            var result = (carbonInputFromStraw / 0.45) * nitrogenConcentrationInStraw;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-12
        /// </summary>
        /// <param name="carbonInputFromRoots">Carbon input from roots (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInRoots">N concentration in the roots (kg kg-1) </param>
        public double CalculateRootNitrogen(
            double carbonInputFromRoots,
            double nitrogenConcentrationInRoots)
        {
            var result = (carbonInputFromRoots / 0.45) * nitrogenConcentrationInRoots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-13
        /// </summary>
        /// <param name="carbonInputFromExtraroots">Carbon input from extra-root material (kg ha^-1)</param>
        /// <param name="nitrogenConcentrationInExtraroots">N concentration in the extra root (kg kg-1) (until known from literature, the same N concentration used for roots will be utilized)</param>
        public double CalculateExtrarootNitrogen(
            double carbonInputFromExtraroots,
            double nitrogenConcentrationInExtraroots)
        {
            var result = (carbonInputFromExtraroots / 0.45) * nitrogenConcentrationInExtraroots;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-14
        /// Equation 2.6.2-2
        /// </summary>
        /// <param name="nitrogenContentOfGrainReturned">Nitrogen content of the grain returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfStrawReturned">Nitrogen content of the straw returned to the soil (kg N ha^-1)</param>
        /// <returns>Above ground residue N (kg N ha^-1)</returns>
        public double CalculateAboveGroundResidueNitrogen(
            double nitrogenContentOfGrainReturned,
            double nitrogenContentOfStrawReturned)
        {
            var result = nitrogenContentOfGrainReturned + nitrogenContentOfStrawReturned;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-15
        /// Equation 2.6.2-2
        /// </summary>
        /// <param name="nitrogenContentOfRootReturned">Nitrogen content of the root returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfExtrarootReturned">Nitrogen content of the exudates returned to the soil (kg N ha^-1)</param>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateBelowGroundResidueNitrogen(
            double nitrogenContentOfRootReturned,
            double nitrogenContentOfExtrarootReturned)
        {
            var result = nitrogenContentOfRootReturned + nitrogenContentOfExtrarootReturned;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-16
        /// </summary>
        /// <param name="nitrogenContentOfRootReturned">Nitrogen content of the root returned to the soil (kg N ha^-1)</param>
        /// <param name="nitrogenContentOfExtrarootReturned">Nitrogen content of the exudates returned to the soil (kg N ha^-1)</param>
        /// <param name="perennialStandLength">Length of perennial stand (years)</param>
        /// <returns>Below ground residue N (kg N ha^-1)</returns>
        public double CalculateBelowGroundResidueNitrogenForPerennialForage(
            double nitrogenContentOfRootReturned,
            double nitrogenContentOfExtrarootReturned,
            int perennialStandLength)
        {
            if (perennialStandLength <= 0)
            {
                perennialStandLength = 1;
            }

            var result = (1.0 / perennialStandLength) * (nitrogenContentOfRootReturned + nitrogenContentOfExtrarootReturned);

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-17
        /// </summary>
        /// <param name="aboveGroundResidue">Above ground residue N (kg N ha^-1)</param>
        /// <param name="belowGroundResidue">Below ground residue N (kg N ha^-1)</param>
        /// <param name="area">Area of crop (ha)</param>
        /// <returns>N inputs from crop residue returned to soil (kg N)</returns>
        public double CalculateInputsFromResidueReturned(double aboveGroundResidue,
                                                         double belowGroundResidue,
                                                         double area)
        {
            var result = (aboveGroundResidue + belowGroundResidue) * area;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-18
        /// </summary>
        /// <param name="inputFromResidueReturnedToSoil">N inputs from crop residue returned to soil (kg N)</param>
        /// <param name="emissionFactor">The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N sourcve type, tillage, cropping sytem and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</param>
        /// <returns>N2O emissions (kg N2O-N kg-1 N) resulting from crop residues and N mineralization</returns>
        public double CalculateEmissionsFromResidues(
            double inputFromResidueReturnedToSoil, 
            double emissionFactor)
        {
            var result = inputFromResidueReturnedToSoil * emissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 2.5.2-19
        ///
        /// N_min
        /// </summary>
        /// <param name="carbonChangeInSoil">C change (kg)</param>
        /// <returns>N inputs from mineralization of native soil organic matter (kg N). This value can only be positive. </returns>
        public double CalculateNitrogenInputsFromMineralizationOfNativeSoilOrganicMatter(double carbonChangeInSoil)
        {
            var nitrogenMineralization = carbonChangeInSoil / 10;
            if (nitrogenMineralization < 0)
            {
                return 0;
            }

            return nitrogenMineralization;
        }

        /// <summary>
        /// Equation 2.5.2-15
        /// </summary>
        /// <param name="inputsFromMineralization">N inputs from mineralization of native soil organic matter (kg N). This value can only be positive.</param>
        /// <param name="weightedEcodistrictEmissionFactorForMineralizedNitrogen">The EF considering the impact of the N source on the cropping system and site dependent factors associated with rainfall, topography, soil texture, N sourcve type, tillage, cropping sytem and moisture managment (kg N2O-N kg-1 N) for ecodistrict ‘‘i’’.</param>
        /// <returns>N2O emissions (kg N2O-N kg-1 N) resulting from crop residues and N mineralization</returns>
        public double CalculateEmissionsFromMineralizedNitrogen(double inputsFromMineralization, 
                                                                double weightedEcodistrictEmissionFactorForMineralizedNitrogen)
        {
            var result = inputsFromMineralization * weightedEcodistrictEmissionFactorForMineralizedNitrogen;

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
        /// Equation 2.5.3-2
        /// </summary>
        /// <param name="inputsFromSyntheticFertilizer">N inputs from synthetic fertilizer (kg N)</param>
        /// <param name="inputsFromResidueReturned">N inputs from crop residue (kg N)</param>
        /// <param name="factionLeach">Fraction of N lost by leaching and runoff  (kg N (kg N)-1)</param>
        /// <param name="emissionFactorLeachingRunoff">Emission factor for leaching and runoff [kg N2O-N (kg N)-1]</param>
        /// <returns>N emissions due to leaching and runoff from cropland (kg N2O-N)</returns>
        public double CalculateCropLeach(double inputsFromSyntheticFertilizer, 
                                         double inputsFromResidueReturned,
                                         double factionLeach, 
                                         double emissionFactorLeachingRunoff)
        {
            var result = (inputsFromSyntheticFertilizer + inputsFromResidueReturned) * factionLeach * emissionFactorLeachingRunoff;

            return result;
        }

        /// <summary>
        /// Equation 2.5.3-3
        ///
        /// N2O_CRNminleach
        /// </summary>
        /// <param name="nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter">N inputs from mineralization of native soil organic matter (kg N). This value can only be positive. If the result is negative, then N_min is equal to zero.</param>
        /// <param name="fractionOfNitrogenLostByLeachingAndRunoff">Fraction of N lost by leaching and runoff  (kg N (kg N)-1)</param>
        /// <param name="emissionsFactorForLeachingAndRunoff">Emission factor for leaching and runoff [kg N2O-N (kg N)-1]</param>
        /// <returns>N emissions due to leaching and runoff from mineralized N (kg N2O-N)</returns>
        public double CalculateNitrogenEmissionsDueToLeachingAndRunoffFromMineralizedNitrogen(
            double nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter, 
            double fractionOfNitrogenLostByLeachingAndRunoff, 
            double emissionsFactorForLeachingAndRunoff)
        {
            return nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter *
                   fractionOfNitrogenLostByLeachingAndRunoff *
                   emissionsFactorForLeachingAndRunoff;
        }

        /// <summary>
        /// Equation 2.5.3-4
        /// </summary>
        /// <param name="totalNitrogenInputsFromLandAppliedManureNitrogen">Total N inputs from all land applied manure N (kg - includes on farm produced manure from all livestock scenarios)</param>
        /// <param name="fractionOfNitrogenLostByLeachingAndRunoff">Fraction of N lost by leaching and runoff  (kg N (kg N)-1)</param>
        /// <param name="emissionsFactorForLeachingAndRunoff">Emission factor for leaching and runoff [kg N2O-N (kg N)-1]</param>
        /// <returns>N emissions due to leaching and runoff from all land applied manure N (kg N2O-N)</returns>
        public double CalculateNitrogenEmissionsDueToLeachingAndRunoffFromAllLandAppliedManure(double totalNitrogenInputsFromLandAppliedManureNitrogen,
                                                                                               double fractionOfNitrogenLostByLeachingAndRunoff,
                                                                                               double emissionsFactorForLeachingAndRunoff)
        {
            return totalNitrogenInputsFromLandAppliedManureNitrogen *
                   fractionOfNitrogenLostByLeachingAndRunoff *
                   emissionsFactorForLeachingAndRunoff;
        }

        /// <summary>
        /// Equation 2.5.3-5
        /// </summary>
        /// <param name="nitrogenInputsFromSyntheticFertilizer">N inputs from synthetic fertilizer (kg N)</param>
        /// <param name="fractionOfNitrogenLostByVolatilization">Fraction of N lost by volatilization</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)-1]</param>
        /// <returns>N emissions due to volatilization from cropland (kg N2O-N)</returns>
        public double CalculateNitrogenEmissionsDueToVolatizationFromCropland(double nitrogenInputsFromSyntheticFertilizer,
                                                                              double fractionOfNitrogenLostByVolatilization,
                                                                              double emissionFactorForVolatilization)
        {
            return nitrogenInputsFromSyntheticFertilizer *
                   fractionOfNitrogenLostByVolatilization *
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 2.5.3-7
        /// </summary>
        /// <param name="totalNitrogenInputsFromManure">Total N inputs from all land applied manure N (kg - includes on farm produced manure from all livestock scenarios)</param>
        /// <param name="fractionOfNitrogenLostByVolatilization">Fraction of N lost by volatilization</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)-1]</param>
        /// <returns>N emissions due to volatilization from all land applied manure N (kg N2O-N)</returns>
        public double CalculateNitrogenEmissionsDueToVolatilizationOfAllLandAppliedManure(double totalNitrogenInputsFromManure,
                                                                                          double fractionOfNitrogenLostByVolatilization,
                                                                                          double emissionFactorForVolatilization)
        {
            return totalNitrogenInputsFromManure *
                   fractionOfNitrogenLostByVolatilization *
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 2.5.4-1
        /// </summary>
        /// <param name="emissionsFromSyntheticFertilizer">N emissions from cropland due to synthetic fertilizer inputs (kg N2O-N)</param>
        /// <param name="emissionsFromResidues">N emissions from cropland due to crop residues (kg N2O-N)</param>
        /// <param name="emissionsFromOrganicFertilizer">N emissions from cropland due to crop residues (kg N2O-N)</param>
        /// <param name="emissionsFromLandAppliedManure">N emissions from cropland due to crop residues (kg N2O-N)</param>
        /// <returns>Total direct N emissions from cropland (kg N2O-N year-1)</returns>
        public double CalculateTotalDirectEmissionsForCrop(double emissionsFromSyntheticFertilizer,
            double emissionsFromResidues,
            double emissionsFromOrganicFertilizer, 
            double emissionsFromLandAppliedManure)
        {
            return emissionsFromSyntheticFertilizer + emissionsFromResidues + emissionsFromOrganicFertilizer + emissionsFromLandAppliedManure;
        }

        /// <summary>
        /// Equation 2.5.4-4
        /// </summary>
        /// <param name="emissionsDueToLeachingAndRunoff">N emissions due to leaching and runoff (kg N2O-N)</param>
        /// <param name="emissionsDueToVolatilization">N emissions due to volatilization (kg N2O-N)</param>
        /// <returns>Total indirect N emissions – for each N2O-Ncropindirect, N2O-NminNindirect, N2O-NmanureNindirect (kg N2O-N year-1)</returns>
        public double CalculateTotalIndirectNitrogenEmissions(double emissionsDueToLeachingAndRunoff,
                                                              double emissionsDueToVolatilization)
        {
            return emissionsDueToLeachingAndRunoff + emissionsDueToVolatilization;
        }        

        /// <summary>
        /// Equation 2.5.4-5
        /// </summary>
        /// <param name="totalDirectNitrogenEmissions"></param>
        /// <param name="totalIndirectNitrogenEmissions"></param>
        /// <returns>Total N emissions – for each N2O-Ncropsoils, N2O-NminNsoils, N2O-NmanureNsoils (kg N2O-N year-1)</returns>
        public double CalculateTotalNitrogenEmissions(
            double totalDirectNitrogenEmissions, 
            double totalIndirectNitrogenEmissions)
        {
            return totalDirectNitrogenEmissions + totalIndirectNitrogenEmissions;
        }

        /// <summary>
        /// Equation 2.5.5-1
        /// Equation 2.5.5-2
        /// Equation 2.5.5-3
        /// 
        /// Converts N2O-N to N2O
        /// </summary>
        /// <param name="n2ONEmissions">Total N2O emissions (kg N2O-N year-1)</param>
        /// <returns>Direct N2O emissions from soils – for each N2Ocropdirect, N2O minNdirect, N2OmanureNdirect (kg N2O year-1)</returns>
        public double ConvertN2ONToN2O(double n2ONEmissions)
        {
            return n2ONEmissions * CoreConstants.ConvertN2ONToN2O;
        }

        /// <summary>
        /// Equation 2.5.6-1
        /// </summary>
        /// <param name="directNitrousOxideEmissionsFromSoils">Direct N2O emissions from soils (kg N2O year-1) </param>
        /// <param name="percentageOfAnnualEmissionsAllocatedToMonth">Percentage of annual emissions allocated to month (defaults in Table 12)</param>
        /// <returns>Direct N2O emissions from soils (kg N2O month-1) – by month – for each N2Ocropdirect, N2O minNdirect, N2OmanureNdirect</returns>
        public double CalculateDirectNitrousOxideEmissionsFromSoilsByMonth(double directNitrousOxideEmissionsFromSoils,
                                                                           double percentageOfAnnualEmissionsAllocatedToMonth)
        {
            return directNitrousOxideEmissionsFromSoils * percentageOfAnnualEmissionsAllocatedToMonth / 100.0;
        }

        /// <summary>
        /// Equation 2.5.6-2
        /// </summary>
        /// <param name="indirectNitrousOxideEmissionsFromSoils">Indirect N2O emissions from soils (kg N2O year-1) </param>
        /// <param name="percentageOfAnnualEmissionsAllocatedToMonth">Percentage of annual emissions allocated to month (defaults in Table 12)</param>
        /// <returns>Indirect N2O emissions from soils (kg N2O month-1) – by month – for each N2Ocropindirect, N2O minNindirect, N2OmanureNindirct</returns>
        public double CalculateIndirectNitrousOxideEmissionsFromSoilsByMonth(double indirectNitrousOxideEmissionsFromSoils,
                                                                             double percentageOfAnnualEmissionsAllocatedToMonth)
        {
            return indirectNitrousOxideEmissionsFromSoils * percentageOfAnnualEmissionsAllocatedToMonth / 100.0;
        }

        /// <summary>
        /// Equation 2.5.6-3
        /// </summary>
        /// <param name="totalNitrousOxideEmissionsFromSoils">Total N2O emissions from soils (kg N2O year-1) </param>
        /// <param name="percentageOfAnnualEmissionsAllocatedToMonth">Percentage of annual emissions allocated to month (defaults in Table 12)</param>
        /// <returns>Total N2O emissions from soils (kg N2O month-1) – by month – for each N2Ocropsoils, N2OminNsoils, N2OmanureNsoils</returns>
        public double CalculateTotalNitrousOxideEmissionsFromSoilsByMonth(double totalNitrousOxideEmissionsFromSoils,
                                                                          double percentageOfAnnualEmissionsAllocatedToMonth)
        {
            return totalNitrousOxideEmissionsFromSoils * percentageOfAnnualEmissionsAllocatedToMonth / 100.0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Equation 2.5.1-1
        ///
        /// Note that we do NOT multiple this result by 100 as was previously thought (4/20/2021)
        /// </summary>
        private double CalculateEmissionFactorUsingPrecipitation(double precipitation)
        {
            return Math.Exp((0.00558 * precipitation) - 7.7);
        }

        /// <summary>
        /// Equation 2.5.1-2
        ///
        /// Note that we do NOT multiple this result by 100 as was previously thought (4/20/2021)
        /// </summary>
        private double CalculateEmissionFactorUsingPotentialEvapotranspiration(double potentialEvapotranspiration)
        {
            return Math.Exp((0.00558 * potentialEvapotranspiration) - 7.7);
        }

        #endregion
    }
}