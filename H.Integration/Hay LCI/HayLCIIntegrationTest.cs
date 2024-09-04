using H.Core.Calculators.Infrastructure;
using H.Core.Models.Animals.Beef;
using H.Core.Models;
using H.Core.Providers.Feed;
using H.Core.Providers;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using H.Core.Services;
using H.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using H.Infrastructure;
using System.Reflection;
using H.Content;
using H.Core.Calculators.Climate;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using System.Globalization;
using System.Diagnostics;
using CsvHelper;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Services.Initialization.Climate;
using H.Core.Services.Initialization.Geography;
using H.Core.Test;

namespace H.Integration.Hay_LCI
{
    [TestClass]
    [Ignore]
    public class HayLCIIntegrationTest : UnitTestBase
    {
        #region Internal Classes

        class Table1Item
        {
            public FertilizerBlends FertilizerBlend { get; set; }
            public Dictionary<Province, double> RatesByProvince { get; set; } = new Dictionary<Province, double>();
        }

        class Table2Item
        {
            public Months Month { get; set; }
            public ComponentCategory ComponentCategory { get; set; }
            public ManureStateType ManureStateType { get; set; }
            public Dictionary<Province, double> Rates { get; set; } = new Dictionary<Province, double>();
        }

        class Table3Item
        {
            public int Polygon { get; set; }
            public int Ecodistrict { get; set; }
            public double Area { get; set; }
            public double EcodistrictArea { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Yield { get; set; }
        }

        #endregion

        #region Fields

        private static Defaults _defaults;
        private static ClimateNormalCalculator _climateNormalCalculator;
        private static NasaClimateProvider _nasaClimateProvider;
        private static GeographicDataProvider _geograhicDataProvider;
        private static FertilizerBlendConverter _fertilizerBlendConverter;
        private static FieldResultsService _fieldResultsService;
        private static GlobalSettings _globalSettings;
        private static int SimulationStartYear = 2009;
        private static int SimulationEndYear = 2018;
        private List<Table1Item> _fertilizerRates;
        private List<Table2Item> _manureRates;
        private static List<DefaultManureCompositionData> _manureCompositionTypes;
        private Dictionary<Farm, List<CropViewItem>> _nonIrrigatedResultsByFarm = new Dictionary<Farm, List<CropViewItem>>();
        private Dictionary<Farm, List<CropViewItem>> _irrigatedResultsByFarm = new Dictionary<Farm, List<CropViewItem>>();
        private string _baseOutputDirectory;
        private bool _usingIrrigation;
        private List<Table3Item> _slcList;
        private IClimateInitializationService _climateInitializationService;
        private IGeographyInitializationService _geographyInitializationService;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
           
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _defaults = new Defaults();
            _defaults.DefaultRunInPeriod = 1;

            _defaults.CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2;

            _globalSettings = new GlobalSettings();

            _climateNormalCalculator = new ClimateNormalCalculator();
            _nasaClimateProvider = new NasaClimateProvider();
            _nasaClimateProvider.Expiry = TimeSpan.MaxValue;
            _nasaClimateProvider.StartDate = new DateTime(SimulationStartYear, 1, 1);
            _nasaClimateProvider.EndDate = new DateTime(SimulationEndYear, 12, 31);

            _climateProvider = new ClimateProvider(new SlcClimateDataProvider());

            _geograhicDataProvider = new GeographicDataProvider();
            _geograhicDataProvider.Initialize();

            _fertilizerBlendConverter = new FertilizerBlendConverter();


            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            var ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);

            var fieldResultsService = new FieldResultsService(iCBMSoilCarbonCalculator, ipcc, n2oEmissionFactorCalculator, _initializationService);

            _fieldResultsService = fieldResultsService;

            var manureCompositionProvider = new Table_6_Manure_Types_Default_Composition_Provider();
            _manureCompositionTypes = manureCompositionProvider.ManureCompositionData;

            _climateInitializationService = new ClimateInitializationService();
            _geographyInitializationService = new GeographyInitializationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        
        public void HayLCI()
        {
            // Read in data from files
            _fertilizerRates = this.GetFertilizerRates();
            _manureRates = this.GetManureRates();
            _slcList = this.ReadSlcs().ToList();

            this.RunSimulations();
        }

        #endregion

        #region Private Methods

        private void RunSimulations()
        {
            var useIrrigationValues = new List<bool>() {true, false};
            foreach (var useIrrigation in useIrrigationValues)
            {
                var farms = this.BuldFarms(_slcList).ToList();

                _usingIrrigation = useIrrigation;
                var farmNameSuffix = useIrrigation ? "_with_irrigation" : "_no_irrigation";

                foreach (var farm in farms)
                {
                    farm.Name += farmNameSuffix;
                }

                this.ProcessFarms(farms);
            }
        }

        private void ProcessFarms(List<Farm> farms)
        {
            this.AssignClimateData(farms);
            this.AssignGeographicData(farms);
            this.AssignComponents(farms);
            this.CreateHistory(farms);
            this.CalculateResults(farms);
            this.WritePerPolygonOutputs();
            this.WriteEcodistrictOutputs();
        }

        private void WriteEcodistrictOutputs()
        {
            var outputDirectory = _baseOutputDirectory + Path.DirectorySeparatorChar + "PerEcodistrict";
            Directory.CreateDirectory(outputDirectory);
            var suffix = _usingIrrigation ? "_with_Irrigation" : "_no_irrigation";
            var path = outputDirectory + Path.DirectorySeparatorChar + $"PerEcodistrictResults{suffix}.csv";

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var results = _usingIrrigation ? _irrigatedResultsByFarm : _nonIrrigatedResultsByFarm;

                var groupedByEcodistrict = results.GroupBy(x => x.Key.DefaultSoilData.EcodistrictId);
                foreach (var grouping in groupedByEcodistrict)
                {
                    var ecodistrict = grouping.Key;

                    var totalDirectNitrousOxidePerHectare = 0d;
                    var directNitrousOxideEmissionsFromSyntheticNitrogenForArea = 0d;
                    var directNitrousOxideEmissionsFromCropResiduesForArea = 0d;
                    var directNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = 0d;
                    var directNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = 0d;
                    var totalIndirectNitrousOxidePerHectareKgN2OHa = 0d;
                    var totalIndirectNitrousOxideForAreaKgN2ONField = 0d;
                    var indirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField = 0d;
                    var indirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField = 0d;
                    var indirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = 0d;
                    var indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = 0d;
                    var indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForAreaKgN2ONField = 0d;
                    var totalNitricOxideForAreaKgNONField = 0d;
                    var totalNitrateLeachingForAreaKgNO3NField = 0d;
                    var totalAmmoniaForAreaKgNH4NField = 0d;
                    var totalOnFarmCroppingEnergyEmissionsKgCO2 = 0d;
                    var totalUpstreamCroppingEnergyEmissionsKgCO2 = 0d;
                    var energyCarbonDioxideFromManureSpreadingKgCO2 = 0d;
                    var adjustedAmmoniacalLossFromLandAppliedManure = 0d;

                    var nO3NFromSyntheticFertilizerLeaching = 0d;
                    var nO3NFromResiduesLeaching = 0d;
                    var nO3NFromMineralizationLeaching = 0d;
                    var nO3NFromManureAndDigestateLeaching = 0d;
                    var totalN2ONFromManureAndDigestateLeachingN2ONField = 0d;

                    var soilCarbonKgCHa = 0d;
                    var changeInCarbonKgCHa = 0d;

                    var nH4FromSyntheticNitogenVolatilized = 0d;

                    foreach (var keyValuePair in grouping)
                    {
                        var farm = keyValuePair.Key;
                        var farmEmissionsFor2018 = keyValuePair.Value.Single(x => x.Year == SimulationEndYear);

                        totalDirectNitrousOxidePerHectare += farmEmissionsFor2018.TotalDirectNitrousOxidePerHectare;
                        directNitrousOxideEmissionsFromSyntheticNitrogenForArea += farmEmissionsFor2018.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea;
                        directNitrousOxideEmissionsFromCropResiduesForArea += farmEmissionsFor2018.DirectNitrousOxideEmissionsFromCropResiduesForArea;
                        directNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField += farmEmissionsFor2018.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea;
                        directNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField += farmEmissionsFor2018.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea;
                        totalIndirectNitrousOxidePerHectareKgN2OHa += farmEmissionsFor2018.TotalIndirectNitrousOxidePerHectare;
                        totalIndirectNitrousOxideForAreaKgN2ONField += farmEmissionsFor2018.TotalIndirectNitrousOxideForArea;
                        indirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField += farmEmissionsFor2018.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea;
                        indirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField += farmEmissionsFor2018.IndirectNitrousOxideEmissionsFromCropResiduesForArea;
                        indirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField += farmEmissionsFor2018.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea;
                        indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField += farmEmissionsFor2018.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea;
                        indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForAreaKgN2ONField += farmEmissionsFor2018.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea;
                        totalNitricOxideForAreaKgNONField += farmEmissionsFor2018.TotalNitricOxideForArea;
                        totalNitrateLeachingForAreaKgNO3NField += farmEmissionsFor2018.TotalNitrateLeachingForArea;
                        totalAmmoniaForAreaKgNH4NField += farmEmissionsFor2018.TotalAmmoniaForArea;
                        adjustedAmmoniacalLossFromLandAppliedManure += farmEmissionsFor2018.AdjustedAmmoniacalLossFromLandAppliedManurePerHectare;

                        totalOnFarmCroppingEnergyEmissionsKgCO2 += farmEmissionsFor2018.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions;
                        totalUpstreamCroppingEnergyEmissionsKgCO2 += farmEmissionsFor2018.CropEnergyResults.TotalUpstreamCroppingEnergyEmissions;
                        energyCarbonDioxideFromManureSpreadingKgCO2 += farmEmissionsFor2018.CropEnergyResults.EnergyCarbonDioxideFromManureSpreading;

                        nO3NFromSyntheticFertilizerLeaching += farmEmissionsFor2018.NO3NFromSyntheticFertilizerLeaching;
                        nO3NFromResiduesLeaching += farmEmissionsFor2018.NO3NFromResiduesLeaching;
                        nO3NFromMineralizationLeaching += farmEmissionsFor2018.NO3NFromMineralizationLeaching;
                        nO3NFromManureAndDigestateLeaching += farmEmissionsFor2018.NO3NFromManureAndDigestateLeaching;
                        totalN2ONFromManureAndDigestateLeachingN2ONField += farmEmissionsFor2018.TotalN2ONFromManureAndDigestateLeaching;

                        nH4FromSyntheticNitogenVolatilized += farmEmissionsFor2018.NH4FromSyntheticNitogenVolatilized;

                        soilCarbonKgCHa += farmEmissionsFor2018.SoilCarbon;
                        changeInCarbonKgCHa += farmEmissionsFor2018.ChangeInCarbon;
                    }

                    var farmCount = grouping.Count();
                    var ecodistrictName = grouping.First().Key.DefaultSoilData.EcodistrictName;
                    var province = grouping.First().Key.DefaultSoilData.ProvinceString;
                    var ecozoneId = grouping.First().Key.DefaultSoilData.EcodistrictId;
                    var ecozoneZone = grouping.First().Key.DefaultSoilData.EcozoneString;

                    var record = new
                    {
                        Ecodistrict = ecodistrict,
                        EcodistrictName = ecodistrictName,
                        EcozoneId = ecozoneId,
                        Ecozone = ecozoneZone,
                        Province = province,
                        FarmCount = farmCount,
                        AvgTotalDirectNitrousOxidePerHectareKgN2O = totalDirectNitrousOxidePerHectare /farmCount,
                        AvgDirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField = directNitrousOxideEmissionsFromSyntheticNitrogenForArea /farmCount,
                        AvgDirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField = directNitrousOxideEmissionsFromCropResiduesForArea / farmCount,

                        AvgDirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = directNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField / farmCount,
                        AvgDirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = directNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField /farmCount,
                        AvgTotalIndirectNitrousOxidePerHectareKgN2OHa = totalIndirectNitrousOxidePerHectareKgN2OHa / farmCount,
                        AvgTotalIndirectNitrousOxideForAreaKgN2ONField = totalIndirectNitrousOxideForAreaKgN2ONField / farmCount,
                        AvgIndirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField = indirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField / farmCount,
                        AvgIndirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField = indirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField / farmCount,
                        AvgIndirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = indirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField /farmCount,
                        AvgIndirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = indirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField / farmCount,
                        AvgIndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForAreaKgN2ONField = indirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForAreaKgN2ONField /farmCount,
                        AvgTotalNitricOxideForAreaKgNONField = totalNitricOxideForAreaKgNONField / farmCount,
                        AvgTotalNitrateLeachingForAreaKgNO3NField = totalNitrateLeachingForAreaKgNO3NField /farmCount,
                        AvgTotalAmmoniaForAreaKgNH4NField = totalAmmoniaForAreaKgNH4NField /farmCount,
                        AvgAdjustedAmmoniacalLossFromLandAppliedManureKgNH3NHa = adjustedAmmoniacalLossFromLandAppliedManure / farmCount,

                    AvgTotalOnFarmCroppingEnergyEmissionsKgCO2 = totalOnFarmCroppingEnergyEmissionsKgCO2 /farmCount,
                        AvgTotalUpstreamCroppingEnergyEmissionsKgCO2 = totalUpstreamCroppingEnergyEmissionsKgCO2 / farmCount,
                        AvgEnergyCarbonDioxideFromManureSpreadingKgCO2 = energyCarbonDioxideFromManureSpreadingKgCO2 / farmCount,

                        AvgNO3NFromSyntheticFertilizerLeaching = nO3NFromSyntheticFertilizerLeaching / farmCount,
                        AvgNO3NFromResiduesLeaching = nO3NFromResiduesLeaching / farmCount,
                        AvgNO3NFromMineralizationLeaching = nO3NFromMineralizationLeaching /farmCount,
                        AvgNO3NFromManureAndDigestateLeaching = nO3NFromManureAndDigestateLeaching / farmCount,
                        AvgTotalN2ONFromManureAndDigestateLeachingN2ONField = totalN2ONFromManureAndDigestateLeachingN2ONField / farmCount,

                        AvgTotalNH4FromSyntheticNitogenVolatilizedKgNH3N = nH4FromSyntheticNitogenVolatilized / farmCount,

                        AvgSoilCarbonKgCHa = soilCarbonKgCHa / farmCount,
                        AvgChangeInCarbonKgCHa = changeInCarbonKgCHa / farmCount,
                };

                    csv.WriteRecords(new[] {record});
                }
            }
        }

        private void WritePerPolygonOutputs()
        {
            var outputDirectory = _baseOutputDirectory + Path.DirectorySeparatorChar + "PerPolygon";
            Directory.CreateDirectory(outputDirectory);
            var path =outputDirectory + Path.DirectorySeparatorChar + "PerPolygonResults.csv";

            

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var farmResults in _nonIrrigatedResultsByFarm.Concat(_irrigatedResultsByFarm))
                {
                    var farm = farmResults.Key;
                    var viewItem = farmResults.Value.Single(x => x.Year == 2018);

                    var record = new
                    {
                        FarmName = farm.Name,
                        Polygon = farm.PolygonId,
                        Year = viewItem.Year,
                        Province = farm.DefaultSoilData.ProvinceString,
                        EcodistrictId = farm.DefaultSoilData.EcodistrictId,
                        Ecodistrict = farm.DefaultSoilData.EcodistrictName,
                        EcozoneId = farm.DefaultSoilData.EcodistrictId,
                        EcozoneZone = farm.DefaultSoilData.EcozoneString,
                        Yield = viewItem.Yield,
                        Latitude = farm.Latitude,
                        Longitude = farm.Longitude,
                        GrowingSeasonPrecipitationMm = farm.ClimateData.PrecipitationData.GrowingSeasonPrecipitation,
                        GrowingSeasonEvapotranspirationMm = farm.ClimateData.EvapotranspirationData.GrowingSeasonEvapotranspiration,
                        AmountOfIrrigation = viewItem.AmountOfIrrigation,

                        TotalOnFarmCroppingEnergyEmissionsKgCO2 = viewItem.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions,
                        TotalUpstreamCroppingEnergyEmissionsKgCO2 = viewItem.CropEnergyResults.TotalUpstreamCroppingEnergyEmissions,
                        EnergyCarbonDioxideFromManureSpreadingKgCO2 = viewItem.CropEnergyResults.EnergyCarbonDioxideFromManureSpreading,

                        TotalNitrogenInputsKgNHa = viewItem.TotalNitrogenInputs,
                        TotalDirectNitrousOxidePerHectareKgN2OHa = viewItem.TotalDirectNitrousOxidePerHectare,
                        DirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField = viewItem.DirectNitrousOxideEmissionsFromSyntheticNitrogenForArea,
                        DirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField = viewItem.DirectNitrousOxideEmissionsFromCropResiduesForArea,
                        DirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = viewItem.DirectNitrousOxideEmissionsFromMineralizedNitrogenForArea,
                        DirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = viewItem.DirectNitrousOxideEmissionsFromOrganicNitrogenForArea,
                        TotalIndirectNitrousOxidePerHectareKgN2OHa = viewItem.TotalIndirectNitrousOxidePerHectare,
                        TotalIndirectNitrousOxideForAreaKgN2ONField = viewItem.TotalIndirectNitrousOxideForArea,
                        IndirectNitrousOxideEmissionsFromSyntheticNitrogenForAreaKgN2ONField = viewItem.IndirectNitrousOxideEmissionsFromSyntheticNitrogenForArea,
                        IndirectNitrousOxideEmissionsFromCropResiduesForAreaKgN2ONField = viewItem.IndirectNitrousOxideEmissionsFromCropResiduesForArea,
                        IndirectNitrousOxideEmissionsFromMineralizedNitrogenForAreaKgN2ONField = viewItem.IndirectNitrousOxideEmissionsFromMineralizedNitrogenForArea,
                        IndirectNitrousOxideEmissionsFromOrganicNitrogenForAreaKgN2ONField = viewItem.IndirectNitrousOxideEmissionsFromOrganicNitrogenForArea,
                        IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForAreaKgN2ONField = viewItem.IndirectNitrousOxideEmissionsFromVolatilizationOfSyntheticNitrogenForArea,
                        TotalNitricOxideForAreaKgNONField = viewItem.TotalNitricOxideForArea,
                        TotalNitrateLeachingForAreaKgNO3NField = viewItem.TotalNitrateLeachingForArea,
                        TotalAmmoniaForAreaKgNH4NField = viewItem.TotalAmmoniaForArea,
                        AdjustedAmmoniacalLossFromLandAppliedManure = viewItem.AdjustedAmmoniacalLossFromLandAppliedManurePerHectare,
                        NO3NFromSyntheticFertilizerLeaching = viewItem.NO3NFromSyntheticFertilizerLeaching,
                        NO3NFromResiduesLeaching = viewItem.NO3NFromResiduesLeaching,
                        NO3NFromMineralizationLeaching = viewItem.NO3NFromMineralizationLeaching,
                        NO3NFromManureAndDigestateLeaching = viewItem.NO3NFromManureAndDigestateLeaching,
                        TotalN2ONFromManureAndDigestateLeachingN2ONField = viewItem.TotalN2ONFromManureAndDigestateLeaching,
                        NH4FromSyntheticNitogenVolatilized = viewItem.NH4FromSyntheticNitogenVolatilized,
                        SoilCarbonKgCHa = viewItem.SoilCarbon,
                        ChangeInCarbonKgCHa = viewItem.ChangeInCarbon,
                    };

                    var results = new List<object>() { record };

                    csv.WriteRecords(results);

                }
            }
        }

        private void CalculateResults(List<Farm> farms)
        {
            _baseOutputDirectory = "HayLCI_Output";
            Directory.CreateDirectory(_baseOutputDirectory);

            foreach (var farm in farms)
            {
                var finalResults = _fieldResultsService.CalculateFinalResults(farm);
                if (_usingIrrigation)
                {
                    _irrigatedResultsByFarm.Add(farm, finalResults);

                    //_fieldResultsService.ExportResultsToFile(finalResults, _baseOutputDirectory + Path.DirectorySeparatorChar, InfrastructureConstants.EnglishCultureInfo, MeasurementSystemType.Metric, string.Empty, false, farm);
                }
                else
                {
                    _nonIrrigatedResultsByFarm.Add(farm, finalResults);

                    //_fieldResultsService.ExportResultsToFile(finalResults, _baseOutputDirectory + Path.DirectorySeparatorChar, InfrastructureConstants.EnglishCultureInfo, MeasurementSystemType.Metric, string.Empty, false, farm);
                }
            }
        }

        private void CreateHistory(List<Farm> farms)
        {
            foreach (var farm in farms)
            {
                var fieldComponent = farm.Components.OfType<FieldSystemComponent>().Single();
                farm.StageStates.Add(new FieldSystemDetailsStageState() {DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>(fieldComponent.CropViewItems) {}});
            }
        }

        private void AssignComponents(List<Farm> farms)
        {
            var farmCount = farms.Count;
            var currentCount = 0;

            foreach (var farm in farms)
            {
                var field = new FieldSystemComponent();
                field.StartYear = SimulationEndYear - 1;
                field.EndYear = SimulationEndYear;

                var yield = _slcList.Single(x => x.Polygon == farm.PolygonId).Yield;

                for (int i = SimulationEndYear - 1; i <= SimulationEndYear; i++)
                {
                    var crop = new CropViewItem();
                    crop.CropType = CropType.TameMixed;
                    crop.Area = 1;
                    crop.FieldSystemComponentGuid = field.Guid;
                    crop.Year = i;

                    field.CropViewItems.Add(crop);
                }

                farm.Components.Add(field);

                foreach (var cropViewItem in field.CropViewItems)
                {
                    _initializationService.InitializeCrop(cropViewItem, farm, _globalSettings);

                    this.AssignFertilizerApplications(cropViewItem, farm);
                    this.AssignManureApplications(cropViewItem, farm);

                    if (_usingIrrigation == false)
                    {
                        cropViewItem.AmountOfIrrigation = 0;
                    }

                    cropViewItem.Yield = yield;
                    cropViewItem.PercentageOfRootsReturnedToSoil = 30;
                }

                _fieldResultsService.AssignCarbonInputs(new List<CropViewItem>(field.CropViewItems) {}, farm, field);

                currentCount++;

                Trace.TraceInformation($"Assigned default data for farm #{farm.Name}, {farmCount - currentCount} remaining.");
            }
        }

        private void AssignManureApplications(CropViewItem viewItem, Farm farm)
        {
            foreach (var manureRate in _manureRates)
            {
                var month = manureRate.Month;
                var state = manureRate.ManureStateType;
                var componentCategory = manureRate.ComponentCategory;
                var animalType = componentCategory.GetAnimalTypeFromComponentCategory();

                // Polygons from NL are in inputs - they will be excluded so any value can be assigned to these farms
                var lookup = farm.DefaultSoilData.Province;
                if (manureRate.Rates.ContainsKey(lookup) == false)
                {
                    lookup = Province.Alberta;
                }

                var rate = manureRate.Rates[lookup];

                var manureApplication = new ManureApplicationViewItem();
                manureApplication.DateOfApplication = new DateTime(SimulationEndYear, (int) month, 1);
                manureApplication.ManureStateType = state;
                manureApplication.AnimalType = animalType;
                manureApplication.AmountOfNitrogenAppliedPerHectare = rate;
                manureApplication.ManureLocationSourceType = ManureLocationSourceType.Imported;

                var manureComposition = farm.GetManureCompositionData(
                    manureStateType: manureApplication.ManureStateType,
                    animalType: manureApplication.AnimalType);

                manureApplication.DefaultManureCompositionData = manureComposition;
                

                viewItem.ManureApplicationViewItems.Add(manureApplication);
            }
        }

        private void AssignFertilizerApplications(CropViewItem viewItem, Farm farm)
        {
            foreach (var fertilizerRate in _fertilizerRates)
            {
                var blend = fertilizerRate.FertilizerBlend;
                var province = farm.DefaultSoilData.Province;

                // Polygons from NL are in inputs - they will be excluded so any value can be assigned to these farms
                var lookup = province;
                if (fertilizerRate.RatesByProvince.ContainsKey(lookup) == false)
                {
                    lookup = Province.Alberta;
                }

                var rate = fertilizerRate.RatesByProvince[lookup];

                var fertilizerApplication = new FertilizerApplicationViewItem();
                fertilizerApplication.DateCreated = new DateTime(SimulationEndYear, 5, 1);
                fertilizerApplication.FertilizerBlendData.FertilizerBlend = blend;
                fertilizerApplication.AmountOfBlendedProductApplied = rate;

                _initializationService.InitializeFertilizerBlendData(fertilizerApplication);

                viewItem.FertilizerApplicationViewItems.Add(fertilizerApplication);

                viewItem.UpdateApplicationRateTotals();
            }
        }

        private List<Table3Item> ReadSlcs()
        {
            var result = new List<Table3Item>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream("H.Integration.Resources.table_3_polygons.csv");

            var reader = CsvResourceReader.SplitFileIntoLinesUsingRegex(resourceStream);
            foreach (var row in reader.Skip(2))
            {
                var item = new Table3Item();
                if (string.IsNullOrWhiteSpace(row[0]))
                {
                    continue;
                }

                item.Polygon = int.Parse(row[0]);
                item.Ecodistrict = int.Parse(row[1]);
                item.Area = double.Parse(row[2]);
                item.EcodistrictArea = double.Parse(row[3]);
                item.Latitude = double.Parse(row[4]);
                item.Longitude = double.Parse(row[5]);
                item.Yield= double.Parse(row[6]);

                result.Add(item);
            }

            return result;
        }

        private List<Farm> BuldFarms(List<Table3Item> items)
        {
            var result = new List<Farm>();

            foreach (var table3Item in items)
            {
                var farm = new Farm();
                farm.PolygonId = table3Item.Polygon;
                farm.Latitude = table3Item.Latitude;
                farm.CarbonModellingEquilibriumYear = SimulationStartYear;
                farm.Longitude = table3Item.Longitude;
                farm.Defaults = _defaults;
                farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

                farm.DefaultManureCompositionData.AddRange(_manureCompositionTypes);

                farm.Name = $"HayLCI_Results_{farm.PolygonId}_lat_{farm.Latitude}_long_{farm.Longitude}";

                result.Add(farm);
            }

            return result;
        }

        private void AssignClimateData(List<Farm> farms)
        {
            var startYear = 2009;
            var endYear = 2018;
            var farmCount = farms.Count;
            var currentCount = 0;

            foreach (var farm in farms)
            {
                _climateInitializationService.InitializeClimate(farm, startYear, endYear);

                currentCount++;

                Trace.TraceInformation($"Downloaded climate data for farm #{currentCount}, {farmCount - currentCount} remaining.");
            }
        }

        private void AssignGeographicData(List<Farm> farms)
        {
            var farmCount = farms.Count;
            var currentCount = 0;


            foreach (var farm in farms)
            {
                var geographicData = _geograhicDataProvider.GetGeographicalData(farm.PolygonId);
                if (geographicData.DefaultSoilData == null)
                {
                    geographicData.DefaultSoilData = new SoilData();
                }
                
                farm.GeographicData = geographicData;

                currentCount++;

                Trace.TraceInformation($"Assigned soil data for farm #{currentCount}, {farmCount - currentCount} remaining.");
            }
        }

        private List<Table1Item> GetFertilizerRates()
        {
            var result = new List<Table1Item>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream("H.Integration.Resources.table_1_fertilizer.csv");

            var reader = CsvResourceReader.SplitFileIntoLinesUsingRegex(resourceStream);
            foreach (var line in reader.Skip(3))
            {
                var tableItem = new Table1Item();

                tableItem.FertilizerBlend = _fertilizerBlendConverter.Convert(line[0]);

                var bc = double.Parse(line[2]);
                var ab = double.Parse(line[3]);
                var sk = double.Parse(line[4]);
                var mb = double.Parse(line[5]);
                var on = double.Parse(line[6]);
                var qc = double.Parse(line[7]);

                tableItem.RatesByProvince.Add(Province.BritishColumbia, bc);
                tableItem.RatesByProvince.Add(Province.Alberta, ab);
                tableItem.RatesByProvince.Add(Province.Saskatchewan, sk);
                tableItem.RatesByProvince.Add(Province.Manitoba, mb);
                tableItem.RatesByProvince.Add(Province.Ontario, on);
                tableItem.RatesByProvince.Add(Province.Quebec, qc);

                result.Add(tableItem);
            }

            return result;
        }

        private List<Table2Item> GetManureRates()
        {
            var result = new List<Table2Item>();

            var lines = this.GetLines("H.Integration.Resources.table_2_manure_rates.csv");

            //result.AddRange(this.ParseMonthlyManureRates(lines.Skip(2).Take(12), ComponentCategory.BeefProduction, ManureStateType.LiquidNoCrust));
            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(15).Take(12), ComponentCategory.BeefProduction, ManureStateType.SolidStorage));

            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(29).Take(12), ComponentCategory.Dairy, ManureStateType.LiquidNoCrust));
            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(42).Take(12), ComponentCategory.Dairy, ManureStateType.SolidStorage));

            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(56).Take(12), ComponentCategory.Swine, ManureStateType.LiquidNoCrust));
            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(69).Take(12), ComponentCategory.Swine, ManureStateType.CompostedInVessel));

            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(83).Take(12), ComponentCategory.Sheep, ManureStateType.SolidStorage));

            //result.AddRange(this.ParseMonthlyManureRates(lines.Skip(97).Take(12), ComponentCategory.Poultry, ManureStateType.LiquidNoCrust));
            result.AddRange(this.ParseMonthlyManureRates(lines.Skip(110).Take(12), ComponentCategory.Poultry, ManureStateType.SolidStorageWithOrWithoutLitter));

            return result;
        }

        private IEnumerable<string[]> GetLines(string resourceName)
        {
            return CsvResourceReader.SplitFileIntoLinesUsingRegex(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
        }

        private List<Table2Item> ParseMonthlyManureRates(IEnumerable<string[]> lines, ComponentCategory componentCategory, ManureStateType manureStateType)
        {
            var result = new List<Table2Item>();

            foreach (var line in lines)
            {
                var item = new Table2Item();

                item.ManureStateType = manureStateType;
                item.ComponentCategory = componentCategory;
                item.Month = (Months)DateTime.ParseExact(line[1], "MMM", CultureInfo.CurrentCulture).Month;

                var bc = double.Parse(line[2]);
                var ab = double.Parse(line[3]);
                var sk = double.Parse(line[4]);
                var mb = double.Parse(line[5]);
                var on = double.Parse(line[6]);
                var qc = double.Parse(line[7]);

                item.Rates = new Dictionary<Province, double>()
                {
                    {Province.BritishColumbia, bc},
                    {Province.Alberta, ab},
                    {Province.Saskatchewan, sk},
                    {Province.Manitoba, mb},
                    {Province.Ontario, on},
                    {Province.Quebec, qc},
                };

                result.Add(item);
            }

            return result;
        }

        #endregion
    }
}
