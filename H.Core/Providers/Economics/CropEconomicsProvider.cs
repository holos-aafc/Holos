using System;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Animation;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;
using AutoMapper;

namespace H.Core.Providers.Economics
{
    public class CropEconomicsProvider
    {
        #region Fields

        private readonly CropTypeStringConverter _cropTypeStringConverter = new CropTypeStringConverter();
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly EconomicsMeasurementStringConverter _economicsMeasurementStringConverter = new EconomicsMeasurementStringConverter();

        private readonly List<CropEconomicData> _cache;

        //private const string AlbertaCroppingGuideLink = "https://open.alberta.ca/dataset/64f607fb-d73c-407d-aa60-1f112be29777/resource/573633a4-69ed-49f4-bf34-354e2df3447c/download/af-cropping-alternatives-2020.pdf";
        //private const string SaskCroppingGuideLink = "https://pubsaskdev.blob.core.windows.net/pubsask-prod/125043/Crop%252BPlanning%252BGuide%252B2021.pdf";
        //private const string ManitobaCroppingGuideLink = "https://www.gov.mb.ca/agriculture/farm-management/production-economics/pubs/cop-crop-production.pdf";
        //private const string OntarioCroppingGuideLink = "http://www.omafra.gov.on.ca/english/busdev/bear2000/Budgets/budgettools.htm#crops";

        private const string AlbertaCroppingGuideLink = "https://bit.ly/3tZU1xe";
        private const string SaskCroppingGuideLink = "https://bit.ly/3hHzxXC";
        private const string ManitobaCroppingGuideLink = "https://bit.ly/3v80uaO";
        private const string OntarioCroppingGuideLink = "https://bit.ly/3wnvofC";
        #endregion

        #region Constructors

        public CropEconomicsProvider()
        {
            _cache = this.ReadFile();
        }

        #endregion

        #region Public Methods

        public string GetDataSourceUrlForProvince(Province province)
        {
            switch (province)
            {
                case Province.Alberta:
                    return AlbertaCroppingGuideLink;
                case Province.Saskatchewan:
                    return SaskCroppingGuideLink;
                case Province.Manitoba:
                    return ManitobaCroppingGuideLink;
                case Province.Ontario:
                    return OntarioCroppingGuideLink;
                default:
                    return string.Empty;
            }
        }

        private void FinalizeCropEconomicData(CropEconomicData result, CropType cropType)
        {
            result.ViewItemCropType = cropType;
            result.DataSourceUrl = GetDataSourceUrlForProvince(result.Province);
        }

        /// <summary>
        /// Get economic data for a crop in a given soil region in a given province.
        /// </summary>
        /// <param name="cropType">the crop to get economics for</param>
        /// <param name="soilFunctionalCategory">the soil zone to search</param>
        /// <param name="province">the province to search</param>
        /// <returns><see cref="CropEconomicData"/> for the given crop</returns>
        public CropEconomicData Get(CropType cropType, SoilFunctionalCategory soilFunctionalCategory, Province province)
        {
            //we need a deep copy of the econ data
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CropEconomicData, CropEconomicData>());
            var mapper = config.CreateMapper();

            var soilCategory = soilFunctionalCategory.GetBaseSoilFunctionalCategory();

            var econCropType = this.FarmCropTypeToEconCropType(cropType, soilFunctionalCategory, province);

            //We look for a direct match of croptype, soil, and province
            var result = this.GetDirectMatch(cropType, province, soilCategory, econCropType, mapper);
            if (result != null)
            {
                this.FinalizeCropEconomicData(result, cropType);
                return result;
            }

            //if no direct match then we should look at other soilcategories in the same province
            result = this.GetMatchAcrossTheProvince(province, econCropType, soilCategory, mapper);
            if (result != null)
            {
                this.FinalizeCropEconomicData(result, cropType);
                return result;
            }

            //if no match in other soil regions in the province then look to neighbouring provinces
            result = this.GetMatchInNeighbouringProvince(province, cropType, soilCategory, mapper);
            if (result != null)
            {
                this.FinalizeCropEconomicData(result, cropType);
                return result;
            }

            Trace.TraceError($"{nameof(CropEconomicsProvider)}.{nameof(Get)}: no economic data found for '{cropType.GetDescription()}','{soilFunctionalCategory.GetDescription()}, and '{province.GetDescription()}''. Returning default value");

            return new CropEconomicData() { Province = province };
        }


        /// <summary>
        /// Map a farm crop to the appropriate economics crop. There isn't a 1-to-1 match so there will still be holes for a lot of farm crops
        /// </summary>
        /// <param name="cropType">the economic data from the original CropViewItem</param>
        /// <param name="soilCategory">the soil functional category for the region</param>
        /// <param name="province">the province of the farm</param>
        /// <returns>CropEconomicData</returns>
        public CropType FarmCropTypeToEconCropType(CropType cropType, SoilFunctionalCategory soilCategory, Province province)
        {
            var cropsTypesForProvinceAndSoilCategory = this.GetCropEconomicDataByProvinceAndSoilZone(province, soilCategory).Select(x => x.CropType).ToList();

            if (cropsTypesForProvinceAndSoilCategory.Contains(cropType))
            {
                return cropType;
            }

            //if they don't match we need to find a suitable replacement for the farm crop for the given province
            switch (province)
            {
                case Province.Alberta:
                    return this.MapAlbertaFarmCropToEconomiCropType(cropType);
                case Province.Saskatchewan:
                    return this.MapSaskFarmCropToEconomicCropType(cropType);
                case Province.Manitoba:
                    return this.MapManitobaFarmCropToEconomicCropType(cropType);
                case Province.Ontario:
                    return this.MapOntarioFarmCropToEconomicCropType(cropType);
                default:
                    Trace.TraceError($"{nameof(CropEconomicsProvider)}.{nameof(this.FarmCropTypeToEconCropType)}: {province} is invalid and something wasn't handled correctly");
                    return CropType.NotSelected;
            }
        }

        /// <summary>
        /// Check the cache for the crop.
        /// </summary>
        /// <param name="cropType">The crop to look for.</param>
        /// <returns>True if crop is in the cache, false otherwise.</returns>
        public bool HasDataForCropType(CropType cropType)
        {
            bool result = _cache.Any(x => x.CropType == cropType);
            return result;
        }
        /// <summary>
        /// Check the cache for the province
        /// </summary>
        /// <param name="province">The province to look for.</param>
        /// <returns>True if province exists in cache, false otherwise.</returns>
        public bool HasDataForProvince(Province province)
        {
            bool result = _cache.Any(x => x.Province == province);
            return result;
        }

        /// <summary>
        /// Get all the economic data for a given <see cref="SoilFunctionalCategory"/> in a given <see cref="Province"/>.
        /// </summary>
        /// <param name="province">The province to search for.</param>
        /// <param name="soilCategory">The soil category to search for.</param>
        /// <returns>List of <see cref="CropEconomicData"/> in that soil zone and province.</returns>
        public IEnumerable<CropEconomicData> GetCropEconomicDataByProvinceAndSoilZone(Province province,
            SoilFunctionalCategory soilCategory)
        {
            var result = _cache.Where(econData =>
                econData.Province == province && econData.SoilFunctionalCategory == soilCategory);
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Search for crop in another province
        /// </summary>
        /// <param name="province">the original searched province</param>
        /// <param name="originalCropType">the ORIGINAL crop. NOT economic type</param>
        /// <param name="soilCategory">the soil category</param>
        /// <param name="mapper">the mapper</param>
        /// <returns>CropEconomicData matching the search but from another province</returns>
        private CropEconomicData GetMatchInNeighbouringProvince(Province province, CropType originalCropType,
            SoilFunctionalCategory soilCategory, IMapper mapper)
        {
            const int searchLimit = 2;

            var provinceNeighbour = province.GetNeigbouringProvince();

            //we have a province with no economic neighbour
            if (provinceNeighbour == province) return null;

            CropEconomicData econData = null;
            for (var i = 0; i < searchLimit; i++)
            {
                //we need to get the econ croptype everytime we look a new province
                var econCropType = this.FarmCropTypeToEconCropType(originalCropType, soilCategory, provinceNeighbour);

                if (econCropType != CropType.NotSelected)
                {
                    econData = _cache.SingleOrDefault(entry =>
                        entry.SoilFunctionalCategory == soilCategory &&
                        entry.CropType == econCropType &&
                        entry.Province == provinceNeighbour);
                    if (econData != null) break;

                    //need to search through all the province's soil zones also if I strike out with the given soil zone in a new province
                    econData = this.GetMatchAcrossTheProvince(provinceNeighbour, econCropType, soilCategory, mapper);

                    if (econData != null) break;
                }

                provinceNeighbour = provinceNeighbour.GetNeigbouringProvince();

                //this way we actually loop through all the provinces and don't check something we have already checked
                if (provinceNeighbour == province)
                {
                    provinceNeighbour = Province.Alberta;
                }
            }

            var result = mapper.Map<CropEconomicData>(econData);
            return result;
        }

        /// <summary>
        /// Cycle through all of the soil zones in the province and try and find economic data for the croptype
        /// </summary>
        private CropEconomicData GetMatchAcrossTheProvince(Province province, CropType econCropType,
            SoilFunctionalCategory soilFunctionalCategory, IMapper mapper)
        {
            if (soilFunctionalCategory == SoilFunctionalCategory.NotApplicable) return new CropEconomicData();
            CropEconomicData econData = null;
            var soilNeighbour = soilFunctionalCategory.GetNeighbouringCategory();

            while (soilNeighbour != soilFunctionalCategory && soilNeighbour != SoilFunctionalCategory.NotApplicable)
            {
                econData = _cache.SingleOrDefault(entry =>
                    entry.SoilFunctionalCategory == soilNeighbour &&
                    entry.CropType == econCropType &&
                    entry.Province == province);

                if (econData != null) break;

                soilNeighbour = soilNeighbour.GetNeighbouringCategory();
            }

            var result = mapper.Map<CropEconomicData>(econData);
            return result;

        }

        /// <summary>
        /// Get economic data that matches directly to province, soil zone, and croptype
        /// </summary>
        private CropEconomicData GetDirectMatch(CropType cropType, Province province,
            SoilFunctionalCategory soilCategory, CropType econCropType, IMapper mapper)
        {
            CropEconomicData result;
            if (soilCategory == SoilFunctionalCategory.Brown && province == Province.Alberta &&
                (cropType == CropType.SummerFallow || cropType == CropType.Fallow))
            {
                //alberta has a summerfallow for brown soil only
                var econData = _cache.SingleOrDefault(entry =>
                    entry.CropType == CropType.SummerFallow &&
                    entry.SoilFunctionalCategory == soilCategory &&
                    entry.Province == province);
                result = mapper.Map<CropEconomicData>(econData);
            }
            else if (cropType == CropType.Fallow || cropType == CropType.SummerFallow)
            {
                //any other attempt at getting fallow should use this
                var econData = _cache.SingleOrDefault(entry =>
                    entry.CropType == econCropType &&
                    entry.SoilFunctionalCategory == SoilFunctionalCategory.NotApplicable &&
                    entry.Province == province);
                result = mapper.Map<CropEconomicData>(econData);
            }
            else
            {
                //typical search for crop economic data
                var econData = _cache.SingleOrDefault(entry =>
                    entry.CropType == econCropType &&
                    entry.SoilFunctionalCategory == soilCategory &&
                    entry.Province == province);
                result = mapper.Map<CropEconomicData>(econData);
            }

            return result;
        }
        private List<CropEconomicData> ReadFile()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CropEconomics);
            var result = new List<CropEconomicData>();
            double parseResult;
            foreach (var line in fileLines.Skip(1))
            {
                var entry = new CropEconomicData();

                var cropType = _cropTypeStringConverter.Convert(line[0]);
                var province = _provinceStringConverter.Convert(line[1]);

                var soilFunctionalCategory = _soilFunctionalCategoryStringConverter.Convert(line[2]);

                //only fallow crops should be 'NotApplicable'
                if (soilFunctionalCategory == SoilFunctionalCategory.NotApplicable)
                {
                    var fallowCrops = cropType == CropType.Fallow || cropType == CropType.SummerFallow;
                    if (!fallowCrops) continue;

                }

                var unit = _economicsMeasurementStringConverter.Convert(line[3]);
                var expectedYieldPerAcre = double.Parse(line[4], cultureInfo);
                var expectedMarketPrice = double.Parse(line[5], cultureInfo);
                var cropSalesPerAcre = double.Parse(line[6], cultureInfo);
                var seedCleaningAndTreatment = double.Parse(line[7], cultureInfo);
                var fertilizer = double.Parse(line[8], cultureInfo);
                var chemical = double.Parse(line[10], cultureInfo);
                var hailCropInsurance = double.TryParse(line[11], NumberStyles.Float, cultureInfo, out parseResult) ? parseResult : 0;
                var truckingMarketing = double.Parse(line[12], cultureInfo);
                var fuelOilLube = double.Parse(line[13], cultureInfo);
                var machineryRepairs = double.Parse(line[14], cultureInfo);
                var buildingRepairs = double.Parse(line[15], cultureInfo);
                var pumpingCosts = double.Parse(line[16], cultureInfo);
                var customWork = double.Parse(line[17], cultureInfo);
                var labor = double.Parse(line[18], cultureInfo);
                var utilities = double.Parse(line[19], cultureInfo);
                var operatingInterest = double.Parse(line[20], cultureInfo);
                var totalCost = double.Parse(line[21], cultureInfo);
                var contributionMargin = double.Parse(line[22], cultureInfo);
                var totalCostPerUnit = double.TryParse(line[23], NumberStyles.Float, cultureInfo, out parseResult) ? parseResult : 0;
                var breakEvenYield = double.TryParse(line[24], NumberStyles.Float, cultureInfo, out parseResult) ? parseResult : 0;
                var herbicideCost = double.Parse(line[25], cultureInfo);
                var nitrogenCost = double.Parse(line[26], cultureInfo);
                var phosphorusCost = double.Parse(line[27], cultureInfo);
                var variableCost = double.Parse(line[28], cultureInfo);
                var fixedCost = double.Parse(line[29], cultureInfo);

                entry.CropType = cropType;
                entry.Province = province;
                entry.SoilFunctionalCategory = soilFunctionalCategory;
                entry.Unit = unit;
                entry.ExpectedMarketPrice = expectedMarketPrice;
                entry.ExpectedYieldPerAcre = expectedYieldPerAcre;
                entry.SeedCleaningAndTreatment = seedCleaningAndTreatment;
                entry.Fertilizer = fertilizer;
                entry.Chemical = chemical;
                entry.HailCropInsurance = hailCropInsurance;
                entry.TruckingMarketing = truckingMarketing;
                entry.FuelOilLube = fuelOilLube;
                entry.MachineryRepairs = machineryRepairs;
                entry.BuildingRepairs = buildingRepairs;
                entry.CustomWork = customWork;
                entry.Labour = labor;
                entry.Utilities = utilities;
                entry.OperatingInterest = operatingInterest;
                entry.TotalCost = totalCost;
                entry.ContributionMargin = contributionMargin;
                entry.TotalCostPerUnit = totalCostPerUnit;
                entry.BreakEvenYield = breakEvenYield;
                entry.CropSalesPerAcre = cropSalesPerAcre;
                entry.PumpingCosts = pumpingCosts;
                entry.HerbicideCost = herbicideCost;
                entry.NitrogenCostPerTonne = nitrogenCost;
                entry.PhosphorusCostPerTonne = phosphorusCost;
                entry.TotalVariableCostPerUnit = variableCost;
                entry.TotalFixedCostPerUnit = fixedCost;

                result.Add(entry);
            }

            return result;
        }
        private CropType MapAlbertaFarmCropToEconomiCropType(CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Barley:
                case CropType.UndersownBarley:
                    return CropType.FeedBarley;
                case CropType.OatSilage:
                case CropType.BarleySilage:
                case CropType.TriticaleSilage:
                case CropType.WheatSilage:
                    return CropType.CerealSilage;
                case CropType.BeansDryField:
                    return CropType.DryBean;
                case CropType.Canola:
                    return CropType.ArgentineHTCanola;
                case CropType.Chickpeas:
                    return CropType.KabuliChickpea;
                case CropType.FieldPeas:
                    return CropType.FieldPeas;
                case CropType.Flax:
                    return CropType.Flax;
                case CropType.TameMixed:
                case CropType.TameGrass:
                case CropType.TameLegume:
                    return CropType.TameMixed;
                case CropType.Lentils:
                    return CropType.RedLentils;
                case CropType.Mustard:
                    return CropType.YellowMustard;
                case CropType.Oats:
                    return CropType.MillingOats;
                case CropType.Wheat:
                    return CropType.SpringWheat;
                case CropType.Fallow:
                    return CropType.Fallow;
                case CropType.SummerFallow:
                    return CropType.SummerFallow;

                default:
                    return CropType.NotSelected;
            }
        }
        private CropType MapSaskFarmCropToEconomicCropType(CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Barley:
                case CropType.UndersownBarley:
                    return CropType.FeedBarley;
                case CropType.CanarySeed:
                    return CropType.CanarySeed;
                case CropType.Canola:
                    return CropType.Canola;
                case CropType.Chickpeas:
                    return CropType.DesiChickpeas;
                case CropType.FieldPeas:
                    return CropType.EdibleGreenPeas;
                case CropType.Flax:
                case CropType.Oilseeds:
                    return CropType.Flax;
                case CropType.GrainCorn:
                case CropType.SilageCorn:
                    return CropType.Corn;
                case CropType.Lentils:
                    return CropType.RedLentils;
                case CropType.Oats:
                    return CropType.Oats;
                case CropType.Soybeans:
                    return CropType.Soybeans;
                case CropType.SunflowerSeed:
                    return CropType.SunflowerOilseedEMSS;
                case CropType.Wheat:
                    return CropType.SpringWheat;
                case CropType.Mustard:
                    return CropType.YellowMustard;
                case CropType.FallRye:
                    return CropType.HybridFallRye;
                case CropType.Fallow:
                    return CropType.Fallow;
                case CropType.SummerFallow:
                    return CropType.SummerFallow;
                default:
                    return CropType.NotSelected;
            }
        }
        private CropType MapManitobaFarmCropToEconomicCropType(CropType cropType)
        {
            switch (cropType)
            {
                case CropType.Barley:
                    return CropType.Barley;
                case CropType.Canola:
                    return CropType.Canola;
                case CropType.GrainCorn:
                case CropType.SilageCorn:
                    return CropType.Corn;
                case CropType.DryPeas:
                case CropType.FieldPeas:
                    return CropType.Peas;
                case CropType.Flax:
                case CropType.Oilseeds:
                    return CropType.FlaxSeed;
                case CropType.Oats:
                    return CropType.Oats;
                case CropType.Soybeans:
                    return CropType.Soybeans;
                case CropType.SunflowerSeed:
                    return CropType.SunflowerConfection;
                case CropType.Wheat:
                    return CropType.WheatPrairieSpring;
                case CropType.Fallow:
                    return CropType.Fallow;
                case CropType.SummerFallow:
                    return CropType.SummerFallow;
                default:
                    return CropType.NotSelected;
            }
        }
        private CropType MapOntarioFarmCropToEconomicCropType(CropType cropType)
        {
            //not a lot of the ontario crops map nicely to the 'ValidCropTypes'
            switch (cropType)
            {
                case CropType.Barley:
                    return CropType.SouthernOntarioBarley;
                case CropType.SilageCorn:
                    return CropType.CornSilage;
                case CropType.GrainCorn:
                    return CropType.GrainCorn;
                case CropType.Wheat:
                    return CropType.HardRedSpringWheat;
                case CropType.Canola:
                    return CropType.SpringCanolaHt;
                case CropType.Oats:
                    return CropType.SouthernOntarioOats;
                case CropType.Soybeans:
                    return CropType.Soybeans;
                case CropType.Flax:
                    return CropType.Flax;
                default:
                    return CropType.NotSelected;
            }
        }
        #endregion
    }
}