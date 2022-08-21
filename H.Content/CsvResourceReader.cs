#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using H.Content.Properties;
using H.Infrastructure;

#endregion

namespace H.Content
{
    /// <summary>
    /// </summary>
    public static class CsvResourceReader
    {
        #region Public Methods        

        public static IEnumerable<string[]> GetFileLines(CsvResourceNames csvResourceName)
        {
            switch (csvResourceName)
            {
                case CsvResourceNames.Kapuskasing:
                    return SplitFileIntoLines(Resource.Kapuskasing);

                case CsvResourceNames.ClimateNormalsByPolygon:
                    return SplitFileIntoLines(Resource.climate_norms_by_poly);

                case CsvResourceNames.CanSisComponentTable:
                    return SplitFileIntoLines(Resource.cmp);

                case CsvResourceNames.CanSisPolygonAttributeTable:
                    return SplitFileIntoLines(Resource.pat);

                case CsvResourceNames.CanSisEcodistrictNamesTable:
                    return SplitFileIntoLines(Resource.ednames);

                case CsvResourceNames.CanSisSoilLayerTableAlberta:
                    return SplitFileIntoLines(Resource.sl_ab);

                case CsvResourceNames.CanSisSoilLayerTableBritishColumbia:
                    return SplitFileIntoLines(Resource.sl_bc);

                case CsvResourceNames.CanSisSoilLayerTableManitoba:
                    return SplitFileIntoLines(Resource.sl_mb);

                case CsvResourceNames.CanSisSoilLayerTableNewfoundland:
                    return SplitFileIntoLines(Resource.sl_nl);

                case CsvResourceNames.CanSisSoilLayerTableNovaScotia:
                    return SplitFileIntoLines(Resource.sl_ns);

                case CsvResourceNames.CanSisSoilLayerTableNewBrunswick:
                    return SplitFileIntoLines(Resource.sl_nb);

                case CsvResourceNames.CanSisSoilLayerTableOntario:
                    return SplitFileIntoLines(Resource.sl_on);

                case CsvResourceNames.CanSisSoilLayerTablePrinceEdwardIsland:
                    return SplitFileIntoLines(Resource.sl_pe);

                case CsvResourceNames.CanSisSoilLayerTableQuebec:
                    return SplitFileIntoLines(Resource.sl_qc);

                case CsvResourceNames.CanSisSoilLayerTableSaskatchewan:
                    return SplitFileIntoLines(Resource.sl_sk);

                case CsvResourceNames.CanSisSoilNameTableAlberta:
                    return SplitFileIntoLines(Resource.sn_ab);

                case CsvResourceNames.CanSisSoilNameTableBritishColumbia:
                    return SplitFileIntoLines(Resource.sn_bc);

                case CsvResourceNames.CanSisSoilNameTableManitoba:
                    return SplitFileIntoLines(Resource.sn_mb);

                case CsvResourceNames.CanSisSoilNameTableNewfoundland:
                    return SplitFileIntoLines(Resource.sn_nl);

                case CsvResourceNames.CanSisSoilNameTableNovaScotia:
                    return SplitFileIntoLines(Resource.sn_ns);

                case CsvResourceNames.CanSisSoilNameTableNewBrunswick:
                    return SplitFileIntoLines(Resource.sn_nb);

                case CsvResourceNames.CanSisSoilNameTableOntario:
                    return SplitFileIntoLines(Resource.sn_on);

                case CsvResourceNames.CanSisSoilNameTablePrinceEdwardIsland:
                    return SplitFileIntoLines(Resource.sn_pe);

                case CsvResourceNames.CanSisSoilNameTableQuebec:
                    return SplitFileIntoLines(Resource.sn_qc);

                case CsvResourceNames.CanSisSoilNameTableSaskatchewan:
                    return SplitFileIntoLines(Resource.sn_sk);

                case CsvResourceNames.DairyFeedComposition:
                    return SplitFileIntoLinesUsingRegex(Resource.dairy_feed_composition);

                case CsvResourceNames.CoefficientsForAboveGroundBiomassEstimationForShelterbeltTreeSpecies:
                    return SplitFileIntoLines(Resource.Table_14_Coefficients_For_AGB_Estimation_For_Shelterbelt_Trees);

                case CsvResourceNames.FertilizerApplicationRates:
                    return SplitFileIntoLines(Resource.FertilizerApplicationRates);

                case CsvResourceNames.CropFactors:
                    return SplitFileIntoLines(Resource.CropFactors);
                    
                case CsvResourceNames.SoilN2OEmissionFactorsInfluencedByTillPractice:
                    return SplitFileIntoLines(Resource.Table_15_Soil_N2O_Emission_Factors_Influenced_By_Tillage_Practice);

                case CsvResourceNames.SoilN2OEmissionFactorsInfluencedBySoilTexture:
                    return SplitFileIntoLines(Resource.Table_15_Soil_N2O_Emission_Factors_Influenced_By_Soil_Texture);

                case CsvResourceNames.NitrogenApplicationRatesForSpringWheatStubbleAndFallowCrops:
                    return SplitFileIntoLines(Resource.NitrogenApplicationRatesForSpringWheatStubbleAndFallowCrops);

                case CsvResourceNames.LumCMaxAndKValuesForTillagePracticeChange:
                    return SplitFileIntoLines(Resource.Table_3_LumCMax_And_KValues_For_Tillage_Practice_Change);

                case CsvResourceNames.LumCMaxAndKValuesForFallowPracticeChange:
                    return SplitFileIntoLines(Resource.Table_4_LumCMax_And_KValues_For_Fallow_Practice_Change);

                case CsvResourceNames.LumCMaxAndKValuesForPerennialCroppingChange:
                    return SplitFileIntoLines(Resource.Table_5_LumCMax_And_KValues_For_Perennial_Cropping_Change);

                case CsvResourceNames.SoilCarbonEmissions:
                    return SplitFileIntoLines(Resource.SoilCarbonEmissions);

                // Provider does not exist
                case CsvResourceNames.ActivityCoefficientsForDairyBeefSheep:
                    return SplitFileIntoLines(Resource.Table_20_Beef_And_Dairy_Cattle_Feeding_Activity_Coefficients);

                case CsvResourceNames.DietCoefficientsForDairyBeefSheep:
                    return SplitFileIntoLines(Resource.Table_21_29_Diet_Coefficients_For_Beef_Dairy_Sheep);

                case CsvResourceNames.AdditiveReductionFactorsForAllCattle:
                    return SplitFileIntoLines(Resource.Table_22_Additive_Reduction_Factors_For_Beef_Dairy_Cattle);

                // Provider not being used. Duplicate
                case CsvResourceNames.MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine:
                    return SplitFileIntoLines(Resource.MCFAndNitrogenOxideEmissionsForBeefDairySwine);

                case CsvResourceNames.SwineVolatileExcretion:
                    return SplitFileIntoLines(Resource.SwineVolatileExcretion_34);
                
                // Provider not being used. Duplicate
                case CsvResourceNames.SwineVolatileAndSoilNitrogenExcretionAdjustmentFactors:
                    return SplitFileIntoLines(Resource.Table_35_VS_Excretion_For_Performance_Standard_Diets_For_Swine_Group);

                case CsvResourceNames.FeedIntakeAndCrudeProteinForSwine:
                    return SplitFileIntoLines(Resource.Table_36_Daily_Feed_Intake_For_Swine_Groups);

                case CsvResourceNames.SheepCoefficients:
                    return SplitFileIntoLines(Resource.Table_25_Livestock_Coefficients_For_Sheep);

                case CsvResourceNames.ShelterbeltCarbonAccumulationCoefficients:
                    return SplitFileIntoLines(Resource.ShelterbeltCarbonAccumulationCoefficients);

                case CsvResourceNames.EnergyRequirementsForCrops:
                    return SplitFileIntoLines(Resource.EnergyRequirementsForCrops);

                case CsvResourceNames.ShelterbeltAllometricTable:
                    return SplitFileIntoLines(Resource.shelterbelt_allometric_table);

                case CsvResourceNames.ShelterbeltEcodistrictLookupTable:
                    return SplitFileIntoLines(Resource.shelterbelt_ecodistrict_lookup_table);

                case CsvResourceNames.ShelterbeltHardinessZoneLookup:
                    return SplitFileIntoLines(Resource.shelterbelt_hardinesszone_lookup);

                case CsvResourceNames.SlcToHardinessZone:
                    return SplitFileIntoLines(Resource.slc_to_hardiness_zone);

                case CsvResourceNames.LookupEcodistrictClusters:
                    return SplitFileIntoLines(Resource.lookup_ecodistricts_clusters);

                case CsvResourceNames.CaraganaAllClustersFuture:
                    return SplitFileIntoLines(Resource.caragana__31_allclusters_future);

                case CsvResourceNames.GreenAshAllClustersFuture:
                    return SplitFileIntoLines(Resource.green_ash__31_allclusters_future);

                case CsvResourceNames.HybridPoplarAllClustersFuture:
                    return SplitFileIntoLines(Resource.hybrid_poplar__31_allclusters_future);

                case CsvResourceNames.ManitobaMapleAllClustersFuture:
                    return SplitFileIntoLines(Resource.manitoba_maple__31_allclusters_future);

                case CsvResourceNames.ScotsPineAllClustersFuture:
                    return SplitFileIntoLines(Resource.scots_pine__31_allclusters_future);

                case CsvResourceNames.WhiteSpruceAllClustersFuture:
                    return SplitFileIntoLines(Resource.white_spruce__31_allclusters_future);

                case CsvResourceNames.FeedNames:
                    return SplitFileIntoLinesUsingRegex(Resource.feeds);

                case CsvResourceNames.AboveGroundBiomassTotalTreeBiomassRatios:
                    return SplitFileIntoLinesUsingRegex(Resource.Agt_Ratios);

                case CsvResourceNames.FusionTableAlberta:
                    return SplitFileIntoLines(Resource.ft_AB);

                case CsvResourceNames.ResidueDataFile:
                    return SplitFileIntoLines(Resource.Table_10_A_B_Relative_Biomass_Information);

                case CsvResourceNames.EcodistrictToEcozoneMapping:
                    return SplitFileIntoLines(Resource.ecodistrict_to_ecozone_mapping);

                case CsvResourceNames.AB_CAR_to_SLC:
                    return SplitFileIntoLines(Resource.ab_car_to_slc);

                case CsvResourceNames.AB_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.ab_default_yields);

                case CsvResourceNames.SK_CAR_to_SLC:
                    return SplitFileIntoLines(Resource.sk_car_to_slc);

                case CsvResourceNames.SK_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.sk_default_yields);

                case CsvResourceNames.QC_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.qc_default_yields);

                case CsvResourceNames.BC_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.bc_default_yields);

                case CsvResourceNames.MB_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.mb_default_yields);

                case CsvResourceNames.ON_Default_Yields:
                    return SplitFileIntoLinesUsingRegex(Resource.on_default_yields);
                
                case CsvResourceNames.SwineFeedIngredientList:
                    return SplitFileIntoLinesUsingRegex(Resource.Swine_Feed_Ingredient_List);

                case CsvResourceNames.IrrigationByMonth:
                    return SplitFileIntoLines(Resource.Table_7_Percentage_Total_Annual_Irrigation_Water_Applied);

                case CsvResourceNames.GrowingDegreeCoefficients:
                    return SplitFileIntoLines(Resource.Table_1_Growing_Degree_Crop_Coefficients);

                case CsvResourceNames.CondensedLethbridgeSOCData:
                    return SplitFileIntoLines(Resource.Condensed_LethbridgeSOCData_AllRotations);

                case CsvResourceNames.ClimateNormalsByPolygon1950_1980:
                    return SplitFileIntoLines(Resource.climateNorms_by_poly_1950_1980);

                case CsvResourceNames.ClimateNormalsByPolygon1960_1990:
                    return SplitFileIntoLines(Resource.climateNorms_by_poly_1960_1990);

                case CsvResourceNames.ClimateNormalsByPolygon1970_2000:
                    return SplitFileIntoLines(Resource.climateNorms_by_poly_1970_2000);

                case CsvResourceNames.ClimateNormalsByPolygon1980_2010:
                    return SplitFileIntoLines(Resource.climateNorms_by_poly_1980_2010);

                case CsvResourceNames.ClimateNormalsByPolygon1990_2017:
                    return SplitFileIntoLines(Resource.climateNorms_by_poly_1990_2017);

                case CsvResourceNames.CropEconomics:
                    return SplitFileIntoLinesUsingRegex(Resource.crop_economics);

                case CsvResourceNames.FertilizerBlends:
                    return SplitFileIntoLinesUsingRegex(Resource.Table_51_Carbon_Footprint_At_Plant_Gate_For_Direct_Fertilizer_Blends);

                case CsvResourceNames.SmallAreaYields:
                    return SplitFileIntoLines(Resource.small_area_yields);

                case CsvResourceNames.ElectricityConversionValues:
                    return SplitFileIntoLines(Resource.Table_52_Electricity_Conversion_Values_By_Province);

                case CsvResourceNames.FuelEnergyEstimates:
                    return SplitFileIntoLines(Resource.Table_53_Fuel_Energy_Requirement_Estimates_By_Region);

                case CsvResourceNames.HerbicideEnergyEstimates:
                    return SplitFileIntoLines(Resource.Table_54_Herbicide_Energy_Requirement_Estimates_By_Region);

                case CsvResourceNames.GlobalRadiativeForcing:
                    return SplitFileIntoLines(Resource.Table_66_Global_Radiative_Forcing);

                case CsvResourceNames.GlobalWarmingPotential:
                    return SplitFileIntoLines(Resource.Table_65_Global_Warming_Potential_Of_Emissions);

                case CsvResourceNames.CalibratedModelParameters:
                    return SplitFileIntoLines(Resource.Table_11_Globally_Calibrated_Model_Paramters_To_Estimate_SOC_Changes);

                case CsvResourceNames.NitrogenLinginContentsInSteadyStateMethods:
                    return SplitFileIntoLines(Resource.Table_12_Default_Values_For_Nitrogen_Lignin_In_Crops);

                case CsvResourceNames.SilageYields:
                    return SplitFileIntoLines(Resource.silage_yields);

                case CsvResourceNames.ParametersBiogasMethaneProduction:
                    return SplitFileIntoLines(Resource.Table_49_Parameters_For_Calculating_Biogas_Methane_Production_In_AD_System);

                case CsvResourceNames.EmissionFactorsForDigestateStorage:
                    return SplitFileIntoLines(Resource.emission_factor_digestate_storage);

                case CsvResourceNames.AverageMilkProductionDairyCows:
                    return SplitFileIntoLines(Resource.Table_24_Average_Milk_Production_For_Dairy_Cows_By_Province);

                case CsvResourceNames.CaraganaCarbonDataFuture:
                    return SplitFileIntoLines(Resource.CG__31_allclusters_future_DOM);

                case CsvResourceNames.GreenAshCarbonDataFuture:
                    return SplitFileIntoLines(Resource.GA__31_allclusters_future_DOM);

                case CsvResourceNames.HybridPoplarCarbonDataFuture:
                    return SplitFileIntoLines(Resource.HP__31_allclusters_future_DOM);

                case CsvResourceNames.ManitobaMapleCarbonDataFuture:
                    return SplitFileIntoLines(Resource.MM__31_allclusters_future_DOM);

                case CsvResourceNames.ScotsPineCarbonDataFuture:
                    return SplitFileIntoLines(Resource.SP__31_allclusters_future_DOM);

                case CsvResourceNames.WhiteSpruceCarbonDataFuture:
                    return SplitFileIntoLines(Resource.WS__31_allclusters_future_DOM);

                case CsvResourceNames.CaraganaCarbonDataPast:
                    return SplitFileIntoLines(Resource.CG_31_allclusters_past_CBM);

                case CsvResourceNames.GreenAshCarbonDataPast:
                    return SplitFileIntoLines(Resource.GA_31_allclusters_past_CBM);

                case CsvResourceNames.HybridPoplarCarbonDataPast:
                    return SplitFileIntoLines(Resource.HP_31_allclusters_past_CBM);

                case CsvResourceNames.ManitobaMapleCarbonDataPast:
                    return SplitFileIntoLines(Resource.MM_31_allclusters_past_CBM);

                case CsvResourceNames.ScotsPineCarbonDataPast:
                    return SplitFileIntoLines(Resource.SP_31_allclusters_past_CBM);

                case CsvResourceNames.WhiteSpruceCarbonDataPast:
                    return SplitFileIntoLines(Resource.WS_31_allclusters_past_CBM);

                case CsvResourceNames.SteadyStateMethodDefaultNValues:
                    return SplitFileIntoLines(Resource.Table_13_Default_Values_For_Steady_State_Method);

                case CsvResourceNames.BeefCattleFedWinterFeedCost:
                    return SplitFileIntoLines(Resource.Table_61_Beef_Cattle_Fed_Winter_Feed_Cost);

                case CsvResourceNames.BeefFeedCost:
                    return SplitFileIntoLines(Resource.Table_62_Feed_Costs_For_Beef);

                case CsvResourceNames.BeefCattlePastureSummerFeedCost:
                    return SplitFileIntoLines(Resource.Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost);

                case CsvResourceNames.ManureTypesDefaultComposition:
                    return SplitFileIntoLines(Resource.Table_9_Manure_Types_And_Default_Composition);

                case CsvResourceNames.PoultryNExcretionParameterValues:
                    return SplitFileIntoLines(Resource.Table_44_Poultry_N_Excretion_Rate_Parameter_Values);
                default:
                    return null;
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        /// <summary>
        /// Use this method when CSV has no nested ',' characters. This method is faster than regex method.
        /// </summary>
        public static IEnumerable<string[]> SplitFileIntoLines(string fileContents)
        {
            return SplitFileIntoLines(fileContents, ',');
        }

        public static IEnumerable<string[]> SplitFileIntoLines(string fileContents, char delimiter)
        {
            var lines = fileContents.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var line in lines)
            {
                var a = line.Split(delimiter);

                yield return a;
            }
        }

        /// <summary>
        /// Use this method when CSV has nested ',' characters. This method is slower than non-regex method.
        /// </summary>
        public static IEnumerable<string[]> SplitFileIntoLinesUsingRegex(string fileContents)
        {
            // CsvReader uses a regex internally.
            var reader = new CsvReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContents)));
            foreach (var row in reader.RowEnumerator)
            {
                yield return (string[]) row;
            }
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}