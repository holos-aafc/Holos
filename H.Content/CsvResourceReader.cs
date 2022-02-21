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
                    return SplitFileIntoLines(Resource.Table_5);

                case CsvResourceNames.FertilizerApplicationRates:
                    return SplitFileIntoLines(Resource.Table_6);

                case CsvResourceNames.CropFactors:
                    return SplitFileIntoLines(Resource.Table_7);

                case CsvResourceNames.TillageRatioFactosForDirectSoilNitrousOxideEmissions:
                    return SplitFileIntoLines(Resource.Table_8);

                case CsvResourceNames.TextureRatioFactorsForDirectSoilNitrousOxideEmissions:
                    return SplitFileIntoLines(Resource.Table_9);

                case CsvResourceNames.NitrogenApplicationRatesForSpringWheatStubbleAndFallowCrops:
                    return SplitFileIntoLines(Resource.Table_10);

                case CsvResourceNames.LumCMaxAndKValuesForTillagePracticeChange:
                    return SplitFileIntoLines(Resource.Table_1);

                case CsvResourceNames.LumCMaxAndKValuesForFallowPracticeChange:
                    return SplitFileIntoLines(Resource.Table_2);

                case CsvResourceNames.LumCMaxAndKValuesForPerennialCroppingChange:
                    return SplitFileIntoLines(Resource.Table_3);
                case CsvResourceNames.SoilCarbonEmissions:
                    return SplitFileIntoLines(Resource.Table_7_8_9);

                case CsvResourceNames.ActivityCoefficientsForDairyBeefSheep:
                    return SplitFileIntoLines(Resource.Table_11_16_26);

                case CsvResourceNames.DietCoefficientsForDairyBeefSheep:
                    return SplitFileIntoLines(Resource.Table_12_17_30);

                case CsvResourceNames.AdditiveReductionFactorsForAllCattle:
                    return SplitFileIntoLines(Resource.Table_13_18);

                case CsvResourceNames.MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine:
                    return SplitFileIntoLines(Resource.Table_14_19_23);

                case CsvResourceNames.SwineVolatileExcretion:
                    return SplitFileIntoLines(Resource.Table_20);

                case CsvResourceNames.SwineVolatileAndSoilNitrogenExcretionAdjustmentFactors:
                    return SplitFileIntoLines(Resource.Table_21);

                case CsvResourceNames.FeedIntakeAndCrudeProteinForSwine:
                    return SplitFileIntoLines(Resource.Table_22_24);

                case CsvResourceNames.SheepCoefficients:
                    return SplitFileIntoLines(Resource.Table_30);

                case CsvResourceNames.ShelterbeltCarbonAccumulationCoefficients:
                    return SplitFileIntoLines(Resource.Table_33);

                case CsvResourceNames.EnergyRequirementsForCrops:
                    return SplitFileIntoLines(Resource.Table_38);

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
                    return SplitFileIntoLines(Resource.residue_table);

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
                    return SplitFileIntoLines(Resource.IrrigationByMonth);

                case CsvResourceNames.GrowingDegreeCoefficients:
                    return SplitFileIntoLines(Resource.GrowingDegreeCoefficients);

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
                    return SplitFileIntoLinesUsingRegex(Resource.fertilizer_blends);

                case CsvResourceNames.SmallAreaYields:
                    return SplitFileIntoLines(Resource.small_area_yields);

                case CsvResourceNames.ElectricityConversionValues:
                    return SplitFileIntoLines(Resource.electricity_conversion_values);

                case CsvResourceNames.FuelEnergyEstimates:
                    return SplitFileIntoLines(Resource.fuel_energy_requirement_estimates);

                case CsvResourceNames.HerbicideEnergyEstimates:
                    return SplitFileIntoLines(Resource.herbicide_energy_requirement_estimates);

                case CsvResourceNames.GlobalRadiativeForcing:
                    return SplitFileIntoLines(Resource.global_radiative_forcing);

                case CsvResourceNames.GlobalWarmingPotential:
                    return SplitFileIntoLines(Resource.global_warming_potential_of_emissions);

                case CsvResourceNames.CalibratedModelParameters:
                    return SplitFileIntoLines(Resource.globally_calibrated_model_parameters);

                case CsvResourceNames.NitrogenLinginContentsInSteadyStateMethods:
                    return SplitFileIntoLines(Resource.nitrogen_lingin_contents_steadystate_methods);

                case CsvResourceNames.SilageYields:
                    return SplitFileIntoLines(Resource.silage_yields);

                case CsvResourceNames.ParametersBiogasMethaneProduction:
                    return SplitFileIntoLines(Resource.parameters_biogas_methane_production);

                case CsvResourceNames.EmissionFactorsForDigestateStorage:
                    return SplitFileIntoLines(Resource.emission_factor_digestate_storage);

                case CsvResourceNames.AverageMilkProductionDairyCows:
                    return SplitFileIntoLines(Resource.average_milk_production_dairy_cows);

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