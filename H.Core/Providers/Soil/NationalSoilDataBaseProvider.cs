#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// http://sis.agr.gc.ca/cansis/nsdb/index.html
    /// </summary>
    public class NationalSoilDataBaseProvider : GeographicDataProviderBase, ISoilDataProvider
    {
        #region Fields       

        private const string AgriculturalTypeSoilProfile = "A";
        private const string NativeTypeSoilProfile = "N";

        private List<ComponentTableData> _componentTableDataList;
        private List<SoilLayerTableData> _soilLayerTableDataList;
        private List<SoilNameTableData> _soilNameTableDataList;
        private List<SoilGreatGroupData> _soilGreatGroupDataList;
        private Dictionary<int, PolygonAttributeTableData> _polygonAttributeTableDataList;
        private Dictionary<int, EcodistrictNamesTableData> _ecodistrictNamesTableDataList;

        private readonly Dictionary<string, List<SoilLayerTableData>> _soilLayerTableBySoilIdentifierDictionary;
        private readonly Dictionary<string, SoilLayerTableData> _firstNonLitterLayerCache;

        private readonly ProvinceStringConverter _provinceStringConverter;

        #endregion

        #region Constructors

        public NationalSoilDataBaseProvider()
        {
            HTraceListener.AddTraceListener();
            _provinceStringConverter = new ProvinceStringConverter();
            _firstNonLitterLayerCache = new Dictionary<string, SoilLayerTableData>();
            _soilLayerTableBySoilIdentifierDictionary = new Dictionary<string, List<SoilLayerTableData>>();
            _componentTableDataList = new List<ComponentTableData>();
            _soilLayerTableDataList = new List<SoilLayerTableData>();
            _soilNameTableDataList = new List<SoilNameTableData>();
            _soilGreatGroupDataList = new List<SoilGreatGroupData>();
            _polygonAttributeTableDataList = new Dictionary<int, PolygonAttributeTableData>();
            _ecodistrictNamesTableDataList = new Dictionary<int, EcodistrictNamesTableData>();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void Initialize()
        {
            if (this.IsInitialized)
            {
                return;
            }

            _componentTableDataList = this.GetComponentDataTable();
            _soilLayerTableDataList = this.GetSoilLayerTable();
            _soilNameTableDataList = this.GetSoilNameTable();
            _soilGreatGroupDataList = this.GetSoilGreatGroupTable();
            _polygonAttributeTableDataList = this.GetPolygonAttributeTable();
            _ecodistrictNamesTableDataList = this.GetEcodistrictNamesTable();

            this.IsInitialized = true;

            Trace.TraceInformation($"{nameof(NationalSoilDataBaseProvider)} has been initialized.");
        }


        public bool DataExistsForPolygon(int polygonId)
        {
            // Check if polygon exists.
            if (this.GetPolygonIdList().Contains(polygonId) == false)
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(DataExistsForPolygon)} polygon '{polygonId}' not found in polygon attribute table.");

                return false;
            }

            // Check if we have component data entry for polygon.
            var componentExists = _componentTableDataList.Any(x => x.PolygonId == polygonId);
            if (componentExists == false)
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(DataExistsForPolygon)} no soil component entry found for polygon '{polygonId}'.");

                return false;
            }

            // Check if we have soil data
            var soilData = this.GetAllSoilDataForAllComponentsWithinPolygon(polygonId);
            if (soilData.Any() == false)
            {
                return false;
            }

            // Don't add organic types or any other type for which we have no methodology
            var knownMethodologyType = soilData.Where(x => x.SoilGreatGroup != SoilGreatGroupType.NotApplicable && x.SoilFunctionalCategory != SoilFunctionalCategory.Organic);
            if (knownMethodologyType.Any() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the polygon attribute table (pat.csv) to see if table has an entry for the polygon id.
        /// </summary>
        public IEnumerable<int> GetPolygonIdList()
        {
            if (_polygonAttributeTableDataList.Any())
            {
                return _polygonAttributeTableDataList.Select(x => x.Key);
            }
            else
            {
                return this.GetPolygonAttributeTable().Select(x => x.Key);
            }
        }

        public SoilData GetPredominantSoilDataByPolygonId(int polygonId)
        {
            if (this.GetFirstNonLitterLayer(polygonId) == null)
            {
                return null;
            }

            var soilData = new SoilData
            {
                PolygonId = polygonId,
                BulkDensity = this.GetBulkDensity(polygonId),
                ProportionOfClayInSoil = this.GetProportionOfClayInSoil(polygonId),
                ProportionOfSandInSoil = this.GetProportionOfSandInSoil(polygonId),
                ProportionOfSoilOrganicCarbon = this.GetPercentageSoilOrganicCarbon(polygonId),
                SoilGreatGroup = this.GetSoilGreatGroup(polygonId),
                SoilSubGroup = this.GetSubGroup(polygonId),
                TopLayerThickness = this.GetTopLayerThickness(polygonId),
                SoilPh = this.GetSoilPh(polygonId),
                SoilCec = this.GetSoilCec(polygonId),
                SoilFunctionalCategory = this.GetSoilFunctionalCategory(polygonId),
                Province = this.GetProvince(polygonId),
                ParentMaterialTextureString = this.GetParentMaterialTextureType(polygonId),
                SoilTexture = this.GetSoilTexture(polygonId),
                EcodistrictName = this.GetEcodistrictName(polygonId),
                SoilName = this.GetSoilName(polygonId),
                EcodistrictId = this.GetEcodistrictId(polygonId),
                Ecozone = this.GetEcozone(polygonId),
                DrainageClass = this.GetDrainage(polygonId),
            };

            return soilData;
        }

        private Ecozone GetEcozone(int polygonId)
        {
            var ecodistrictId = this.GetEcodistrictId(polygonId);

            return base.ecodistrictDefaultsProvider.GetEcozone(ecodistrictId);
        }

        /// <summary>
        /// A polygon contains many components. Get a list of all components within the polygon.
        /// </summary>
        /// <param name="polygonId"></param>
        /// <returns></returns>
        public IEnumerable<SoilData> GetAllSoilDataForAllComponentsWithinPolygon(int polygonId)
        {
            var componentsWithinPolygon = _componentTableDataList.Where(x => x.PolygonId == polygonId).ToList();

            Trace.TraceInformation($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetAllSoilDataForAllComponentsWithinPolygon)} found {componentsWithinPolygon.Count} total components in polygon '{polygonId}. Getting first non-litter layers for these components.");

            var result = new List<SoilData>();

            // There are many components within a polygon
            for (var index = 0; index < componentsWithinPolygon.Count; index++)
            {
                var componentTableData = componentsWithinPolygon[index];

                Trace.TraceInformation($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetAllSoilDataForAllComponentsWithinPolygon)} getting soil data for component #{index + 1}.");

                // If there is no soil dat for this component, move on to next.
                if (this.GetFirstNonLitterLayer(componentTableData) == null)
                {
                    continue;
                }

                var soilData = new SoilData
                {
                    PolygonId = polygonId,
                    ComponentId = componentTableData.PolygonComponentId,
                    BulkDensity = this.GetBulkDensityByComponent(componentTableData),
                    ProportionOfClayInSoil = this.GetProportionOfClayInSoilByComponent(componentTableData),
                    ProportionOfSandInSoil = this.GetProportionOfSandInSoilByComponent(componentTableData),
                    ProportionOfSoilOrganicCarbon = this.GetPercentageSoilOrganicCarbonByComponent(componentTableData),
                    SoilGreatGroup = this.GetSoilGreatGroupByComponent(componentTableData),
                    SoilSubGroup = this.GetSoilSubGroup(componentTableData),
                    TopLayerThickness = this.GetTopLayerThicknessByComponent(componentTableData),
                    SoilPh = this.GetSoilPhByComponent(componentTableData),
                    SoilCec = this.GetSoilCecByComponent(componentTableData),
                    SoilFunctionalCategory = this.GetSoilFunctionalCategoryByComponent(componentTableData),
                    Province = this.GetProvinceByComponent(componentTableData),
                    ParentMaterialTextureString = this.GetParentMaterialTextureTypeByComponent(componentTableData),
                    SoilTexture = this.GetSoilTextureByComponent(componentTableData),
                    SoilName = this.GetSoilName(polygonId),
                    EcodistrictName = this.GetEcodistrictName(polygonId),
                    EcodistrictId = this.GetEcodistrictId(polygonId),
                    Ecozone = this.GetEcozone(polygonId),
                    DrainageClass = this.GetDrainage(polygonId),
                };

                result.Add(soilData);

                Trace.TraceInformation($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetAllSoilDataForAllComponentsWithinPolygon)} found soil data for component #{index + 1}.");
            }

            return result;
        }

        #endregion

        #region Private Methods

        public string GetEcodistrictName(int polygonId)
        {
            if (!_polygonAttributeTableDataList.ContainsKey(polygonId))
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(NationalSoilDataBaseProvider.GetEcodistrictName)}: unable to find ecodistrict name for polygon id of {polygonId}. Returning empty string.");

                return string.Empty;
            }

            var polygonAttributeTableData = _polygonAttributeTableDataList[polygonId];
            if (_ecodistrictNamesTableDataList.ContainsKey(polygonAttributeTableData.EcodistrictId) == false)
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(NationalSoilDataBaseProvider.GetEcodistrictName)}: unable to find ecodistrict name for polygon id of {polygonId}. Returning empty string.");

                return string.Empty;
            }

            var ecodistrictNamesTableData = _ecodistrictNamesTableDataList[polygonAttributeTableData.EcodistrictId];

            return ecodistrictNamesTableData.EcodistrictName;
        }

        private int GetEcodistrictId(int polygonId)
        {
            if (!_polygonAttributeTableDataList.ContainsKey(polygonId))
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(NationalSoilDataBaseProvider.GetEcodistrictId)}: unable to find ecodistrict id for polygon id of {polygonId}. Returning default value of 0.");

                return 0;
            }

            return _polygonAttributeTableDataList[polygonId].EcodistrictId;
        }

        private ParentMaterialTextureType GetParentMaterialTextureType(int polygonId)
        {
            var parentMaterialTextureCode = this.GetParentMaterialTexture(polygonId);
            var parentMaterialTexture = this.ConvertParentMaterialTexture(parentMaterialTextureCode);

            return parentMaterialTexture;
        }

        private ParentMaterialTextureType GetParentMaterialTextureTypeByComponent(ComponentTableData componentTableData)
        {
            var parentMaterialTextureCode = this.GetParentMaterialTextureByComponent(componentTableData);
            var parentMaterialTexture = this.ConvertParentMaterialTexture(parentMaterialTextureCode);

            return parentMaterialTexture;
        }

        private SoilTexture GetSoilTexture(int polygonId)
        {
            var parentMaterialTexture = this.GetParentMaterialTextureType(polygonId);
            var soilTexture = this.ConvertParentMaterialTextureToSoilTexture(parentMaterialTexture);

            return soilTexture;
        }

        private SoilTexture GetSoilTextureByComponent(ComponentTableData componentTableData)
        {
            var parentMaterialTexture = this.GetParentMaterialTextureTypeByComponent(componentTableData);
            var soilTexture = this.ConvertParentMaterialTextureToSoilTexture(parentMaterialTexture);

            return soilTexture;
        }

        private SoilTexture ConvertParentMaterialTextureToSoilTexture(ParentMaterialTextureType parentMaterialTexture)
        {
            switch (parentMaterialTexture)
            {
                case ParentMaterialTextureType.VeryCoarse:
                case ParentMaterialTextureType.ModeratelyCoarse:
                case ParentMaterialTextureType.Coarse:
                    return SoilTexture.Coarse;

                case ParentMaterialTextureType.Medium:
                case ParentMaterialTextureType.MediumSkeletal:
                    return SoilTexture.Medium;

                case ParentMaterialTextureType.ModeratelyFine:
                case ParentMaterialTextureType.Fine:
                case ParentMaterialTextureType.VeryFine:
                case ParentMaterialTextureType.FineSkeletal:
                    return SoilTexture.Fine;

                // BUG 246

                // Return Medium for all parent material textures until a solution is found for BUG 246
                default:
                    return SoilTexture.Medium;
            }
        }

        private ParentMaterialTextureType ConvertParentMaterialTexture(string parentMaterialTextureCode)
        {
            switch (parentMaterialTextureCode.ToLowerInvariant())
            {
                case "vc":
                    return ParentMaterialTextureType.VeryCoarse;

                case "c":
                    return ParentMaterialTextureType.Coarse;

                case "mc":
                    return ParentMaterialTextureType.ModeratelyCoarse;

                case "m":
                    return ParentMaterialTextureType.Medium;

                case "mf":
                    return ParentMaterialTextureType.ModeratelyFine;

                case "f":
                    return ParentMaterialTextureType.Fine;

                case "vf":
                    return ParentMaterialTextureType.VeryFine;

                case "cs":
                    return ParentMaterialTextureType.CoarseSkeletal;

                case "ms":
                    return ParentMaterialTextureType.MediumSkeletal;

                case "fs":
                    return ParentMaterialTextureType.FineSkeletal;

                case "fr":
                    return ParentMaterialTextureType.Fragmental;

                case "sm":
                    return ParentMaterialTextureType.StratifiedMineral;

                case "su":
                    return ParentMaterialTextureType.StratifiedMineralAndOrganic;

                case "fi":
                    return ParentMaterialTextureType.Fibric;

                case "me":
                    return ParentMaterialTextureType.Mesic;

                case "hu":
                    return ParentMaterialTextureType.Humic;

                case "ud":
                    return ParentMaterialTextureType.Undifferentiated;

                case "-":
                    return ParentMaterialTextureType.NotApplicable;

                case "i":
                case "p":
                case "vp":
                case "":
                case "w":
                case "1":
                    return ParentMaterialTextureType.Unknown;

                default:
                    throw new Exception($"{nameof(NationalSoilDataBaseProvider)}.{nameof(this.ConvertParentMaterialTexture)} value not found for parent material texture code '{parentMaterialTextureCode.ToLowerInvariant()}'.");
            }
        }

        private SoilFunctionalCategory GetSoilFunctionalCategory(int polygonId)
        {
            var soilGreatGroup = this.GetSoilGreatGroup(polygonId);
            var province = this.GetProvince(polygonId);
            var region = province.GetRegion();

            if (soilGreatGroup == SoilGreatGroupType.Unknown && region == Region.EasternCanada)
            {
                return SoilFunctionalCategory.EasternCanada;
            }

            var result = _soilGreatGroupDataList.SingleOrDefault(x => x.SoilGreatGroup == soilGreatGroup && x.Region == region);
            if (result != null)
            {
                return result.SoilFunctionalCategory;
            }

            return SoilFunctionalCategory.Black;
        }

        private SoilFunctionalCategory GetSoilFunctionalCategoryByComponent(ComponentTableData componentTableData)
        {
            var soilGreatGroup = this.GetSoilGreatGroupByComponent(componentTableData);
            var province = this.GetProvinceByComponent(componentTableData);
            var region = province.GetRegion();

            if (soilGreatGroup == SoilGreatGroupType.Unknown && region == Region.EasternCanada)
            {
                return SoilFunctionalCategory.EasternCanada;
            }

            var result = _soilGreatGroupDataList.SingleOrDefault(x => x.SoilGreatGroup == soilGreatGroup && x.Region == region);
            if (result != null)
            {
                return result.SoilFunctionalCategory;
            }

            return SoilFunctionalCategory.Black;
        }

        private Province GetProvince(int polygonId)
        {
            var provinceCode = this.GetProvinceCode(polygonId);

            var result = this.GetProvinceFromProvinceCode(provinceCode);

            return result;
        }

        private Province GetProvinceByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);
            var provinceCode = this.GetProvinceCodeByComponent(componentTableData);

            var result = this.GetProvinceFromProvinceCode(provinceCode);

            return result;
        }

        private string GetProvinceCode(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.ProvinceCode;
        }

        private string GetProvinceCodeByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.ProvinceCode;
        }

        private double GetProportionOfClayInSoil(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.TotalClay / 100.0;
        }

        private double GetProportionOfClayInSoilByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.TotalClay / 100.0;
        }

        /// <summary>
        /// Returns the top layer thickenss in mm.
        /// </summary>
        private int GetTopLayerThickness(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            // Values from table are in cm - convert to mm since climate parameter calculations need input to be in mm.
            var result = (layer.LowerDepth - layer.UpperDepth) * 10;

            return result;
        }

        /// <summary>
        /// Returns the top layer thickenss in mm.
        /// </summary>
        private double GetTopLayerThicknessByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            // Values from table are in cm - convert to mm since climate parameter calculations need input to be in mm.
            var result = (layer.LowerDepth - layer.UpperDepth) * 10;

            return result;
        }

        private double GetPercentageSoilOrganicCarbon(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.OrganicCarbon;
        }

        private double GetPercentageSoilOrganicCarbonByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.OrganicCarbon;
        }

        private double GetBulkDensity(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.BulkDensity;
        }

        private double GetBulkDensityByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.BulkDensity;
        }

        private string GetParentMaterialTexture(int polygonId)
        {
            var soilNameTableData = this.GetSoilNameTableData(polygonId);

            return soilNameTableData.FirstParentMaterialTexture;
        }

        private string GetParentMaterialTextureByComponent(ComponentTableData componentTableData)
        {
            var soilNameTableData = this.GetSoilNameTableData(componentTableData);

            return soilNameTableData.FirstParentMaterialTexture;
        }

        private SoilDrainageClasses GetDrainage(int polygonId)
        {
            var soilNameTableData = this.GetSoilNameTableData(polygonId);
            var drainageString = soilNameTableData.SoilDrainageClass;

            if (string.IsNullOrWhiteSpace(drainageString) == false)
            {
                if (drainageString.Equals("VR", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.VeryRapidlyDrained;
                }

                if (drainageString.Equals("R", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.RapidlyDrained;
                }

                if (drainageString.Equals("W", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.WellDrained;
                }

                if (drainageString.Equals("MW", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.ModeratelyWellDrained;
                }

                if (drainageString.Equals("I", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.ImperfectlyDrained;
                }

                if (drainageString.Equals("P", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.PoorlyDrained;
                }

                if (drainageString.Equals("VP", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.VeryPoorlyDrained;
                }

                if (drainageString.Equals("-", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SoilDrainageClasses.NotApplicable;
                }
            }

            return SoilDrainageClasses.NotApplicable;
        }

        private double GetSoilPh(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.PHAsPerProjectReport;
        }

        private double GetSoilCec(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.CationExchangeCapacity;
        }

        private double GetSoilPhByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.PHAsPerProjectReport;
        }

        private double GetSoilCecByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.CationExchangeCapacity;
        }

        private SoilGreatGroupType GetSoilGreatGroup(int polygonId)
        {
            var soilNameTableData = this.GetSoilNameTableData(polygonId);
            var soilGreatGroupCode = soilNameTableData.SoilGreatGroupThirdEdition;
            var result = this.ConvertSoilGreatGroupCode(soilGreatGroupCode);

            return result;
        }

        private string GetSubGroup(int polygonId)
        {
            var soilNameTableData = this.GetSoilNameTableData(polygonId);
            var result = soilNameTableData.SoilSubgroupThirdEdition;

            return result;
        }

        private SoilGreatGroupType GetSoilGreatGroupByComponent(ComponentTableData componentTableData)
        {
            var soilNameTableData = this.GetSoilNameTableData(componentTableData);
            var soilGreatGroupCode = soilNameTableData.SoilGreatGroupThirdEdition;
            var result = this.ConvertSoilGreatGroupCode(soilGreatGroupCode);

            return result;
        }

        private string GetSoilSubGroup(ComponentTableData componentTableData)
        {
            var soilNameTableData = this.GetSoilNameTableData(componentTableData);
            var result = soilNameTableData.SoilSubgroupThirdEdition;

            return result;
        }

        private string GetSoilName(int polygonId)
        {
            var soilNameTableData = this.GetSoilNameTableData(polygonId);
            var soilName = soilNameTableData.SoilName;

            return soilName;
        }

        private double GetProportionOfSandInSoil(int polygonId)
        {
            var layer = this.GetFirstNonLitterLayer(polygonId);

            return layer.TotalSand / 100.0;
        }

        private double GetProportionOfSandInSoilByComponent(ComponentTableData componentTableData)
        {
            var layer = this.GetFirstNonLitterLayer(componentTableData);

            return layer.TotalSand / 100.0;
        }

        private List<SoilGreatGroupData> GetSoilGreatGroupTable()
        {
            var result = new List<SoilGreatGroupData>
            {
                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.BrownChernozem,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.BrownChernozem,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DarkBrownChernozem,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.DarkBrown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DarkBrownChernozem,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.BlackChernozem,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Black
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.BlackChernozem,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DarkGrayChernozem,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Black
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DarkGrayChernozem,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Solonetz,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Solonetz,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.SolodizedSolonetz,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.SolodizedSolonetz,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Solod,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Solod,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.VerticSolonetz,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.VerticSolonetz,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.GrayBrownLuvisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.GrayBrownLuvisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.GrayLuvisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Black
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.GrayLuvisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.FerroHumicPodzol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.FerroHumicPodzol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumicPodzol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumicPodzol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumoFerricPodzol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumoFerricPodzol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.MelanicBrunisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.MelanicBrunisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.EutricBrunisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.EutricBrunisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.SombricBrunisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.SombricBrunisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DystricBrunisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.DystricBrunisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumicGleysol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.HumicGleysol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Gleysol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Brown
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Gleysol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.LuvicGleysol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Black
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.LuvicGleysol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Fibrisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Fibrisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Mesisol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.Mesisol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.OrganicCryosol,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.OrganicCryosol,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.Organic
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.NotApplicable,
                    Region = Region.WesternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                },

                new SoilGreatGroupData
                {
                    SoilGreatGroup = SoilGreatGroupType.NotApplicable,
                    Region = Region.EasternCanada,
                    SoilFunctionalCategory = SoilFunctionalCategory.NotApplicable
                }
            };

            return result;
        }

        private Dictionary<int, EcodistrictNamesTableData> GetEcodistrictNamesTable()
        {
            var results = new Dictionary<int, EcodistrictNamesTableData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CanSisEcodistrictNamesTable)
                                             .Skip(1)
                                             .ToList();
            var result = from line in fileLines
                         select new EcodistrictNamesTableData
                         {
                             EcodistrictId = int.Parse(line.ElementAt(0), cultureInfo),
                             EcodistrictName = line.ElementAt(1)
                         };

            foreach (var data in result)
            {
                results.Add(data.EcodistrictId, data);
            }


            return results;
        }

        private Dictionary<int, PolygonAttributeTableData> GetPolygonAttributeTable()
        {
            var results = new Dictionary<int, PolygonAttributeTableData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CanSisPolygonAttributeTable)
                                             .Skip(1)
                                             .ToList();
            var result = from line in fileLines
                         select new PolygonAttributeTableData
                         {
                             PolygonId = int.Parse(line.ElementAt(2), cultureInfo),
                             EcodistrictId = int.Parse(line.ElementAt(3), cultureInfo)
                         };

            foreach (var data in result)
            {
                results.Add(data.PolygonId, data);
            }

            return results;
        }

        private List<SoilNameTableData> GetSoilNameTable()
        {
            var csvResourceNames = new List<CsvResourceNames>
            {
                CsvResourceNames.CanSisSoilNameTableAlberta,
                CsvResourceNames.CanSisSoilNameTableBritishColumbia,
                CsvResourceNames.CanSisSoilNameTableManitoba,
                CsvResourceNames.CanSisSoilNameTableNewBrunswick,
                CsvResourceNames.CanSisSoilNameTableNewfoundland,
                CsvResourceNames.CanSisSoilNameTableNovaScotia,
                CsvResourceNames.CanSisSoilNameTableOntario,
                CsvResourceNames.CanSisSoilNameTablePrinceEdwardIsland,
                CsvResourceNames.CanSisSoilNameTableQuebec,
                CsvResourceNames.CanSisSoilNameTableSaskatchewan
            };

            var results = new List<SoilNameTableData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            foreach (var csvResourceName in csvResourceNames)
            {
                var fileLines = CsvResourceReader.GetFileLines(csvResourceName)
                                                 .Skip(1)
                                                 .ToList();

                foreach (var line in fileLines)
                {
                    var entry = new SoilNameTableData()
                    {
                        SoilNameIdentifier = line.ElementAt(0),
                        ProvinceCode = line.ElementAt(1),
                        SoilCode = line.ElementAt(2),
                        SoilCodeModifier = line.ElementAt(3),
                        TypeOfSoilProfile = line.ElementAt(4),
                        SoilName = line.ElementAt(5),
                        KindOfSurfaceMaterial = line.ElementAt(6),
                        WaterTableCharacteristics = line.ElementAt(7),
                        SoilLayerThatRestrictsRootsGrowth = line.ElementAt(8),
                        TypeOfRootRestrictingLayer = line.ElementAt(9),
                        SoilDrainageClass = line.ElementAt(10),
                        FirstParentMaterialTexture = line.ElementAt(11),
                        SecondParentMaterialTexture = line.ElementAt(12),
                        ThirdParentMaterialTexture = line.ElementAt(13),
                        FirstParentMaterialChemicalProperty = line.ElementAt(14),
                        SecondParentMaterialChemicalProperty = line.ElementAt(15),
                        ThirdParentMaterialChemicalProperty = line.ElementAt(16),
                        FirstModeOfDeposition = line.ElementAt(17),
                        SecondModeOfDeposition = line.ElementAt(18),
                        ThirdModeOfDeposition = line.ElementAt(19),
                        SoilOrderSecondEdition = line.ElementAt(20),
                        SoilGreatGroupSecondEdition = line.ElementAt(21),
                        SoilSubgroupSecondEdition = line.ElementAt(22),
                        SoilOrderThirdEdition = line.ElementAt(23),
                        SoilGreatGroupThirdEdition = line.ElementAt(24),
                        SoilSubgroupThirdEdition = line.ElementAt(25)
                    };

                    entry.Province = _provinceStringConverter.Convert(entry.ProvinceCode);

                    results.Add(entry);
                }
            }

            return results;
        }

        private List<ComponentTableData> GetComponentDataTable()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.CanSisComponentTable)
                                             .Skip(1)
                                             .ToList();
            var result = from line in fileLines
                         select new ComponentTableData
                         {
                             PolygonId = int.Parse(line.ElementAt(0), cultureInfo),
                             ComponentNumber = int.Parse(line.ElementAt(1), cultureInfo),
                             PercentageOfPolygonOccupiedByComponent = int.Parse(line.ElementAt(2), cultureInfo),
                             SlopeGradient = line.ElementAt(3),
                             Stone = line.ElementAt(4),
                             LocalSurfaceForm = line.ElementAt(5),
                             ProvinceCode = line.ElementAt(6),
                             SoilCode = line.ElementAt(7),
                             SoilCodeModifier = line.ElementAt(8),
                             TypeOfSoilProfile = line.ElementAt(9),
                             SoilNameIdentifier = line.ElementAt(10),
                             PolygonComponentId = int.Parse(line.ElementAt(11), cultureInfo)
                         };

            return result.ToList();
        }

        private List<SoilLayerTableData> GetSoilLayerTable()
        {
            var csvResourceNames = new List<CsvResourceNames>
            {
                CsvResourceNames.CanSisSoilLayerTableAlberta,
                CsvResourceNames.CanSisSoilLayerTableBritishColumbia,
                CsvResourceNames.CanSisSoilLayerTableManitoba,
                CsvResourceNames.CanSisSoilLayerTableNewBrunswick,
                CsvResourceNames.CanSisSoilLayerTableNewfoundland,
                CsvResourceNames.CanSisSoilLayerTableNovaScotia,
                CsvResourceNames.CanSisSoilLayerTableOntario,
                CsvResourceNames.CanSisSoilLayerTablePrinceEdwardIsland,
                CsvResourceNames.CanSisSoilLayerTableQuebec,
                CsvResourceNames.CanSisSoilLayerTableSaskatchewan
            };

            var results = new List<SoilLayerTableData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            foreach (var csvResourceName in csvResourceNames)
            {
                var fileLines = CsvResourceReader.GetFileLines(csvResourceName)
                                                 .Skip(1)
                                                 .ToList();

                foreach (var line in fileLines)
                {
                    var entry = new SoilLayerTableData()
                    {
                        SoilNameIdentifier = line.ElementAt(0),
                        ProvinceCode = line.ElementAt(1),
                        SoilCode = line.ElementAt(2),
                        SoilCodeModifier = line.ElementAt(3),
                        TypeOfSoilProfile = line.ElementAt(4),
                        LayerNumber = line.ElementAt(5),
                        UpperDepth = int.Parse(line.ElementAt(6), cultureInfo),
                        LowerDepth = int.Parse(line.ElementAt(7), cultureInfo),
                        HorizonLithologicalDiscontinuity = line.ElementAt(8),
                        HorizonMasterCode = line.ElementAt(9),
                        HorizonSuffix = line.ElementAt(10),
                        HorizonModifier = line.ElementAt(11),
                        CoarseFragments = int.Parse(line.ElementAt(12), cultureInfo),
                        DominantSandFraction = line.ElementAt(13),
                        VeryFineSand = int.Parse(line.ElementAt(14)),
                        TotalSand = int.Parse(line.ElementAt(15), cultureInfo),
                        TotalSilt = int.Parse(line.ElementAt(16), cultureInfo),
                        TotalClay = int.Parse(line.ElementAt(17), cultureInfo),
                        OrganicCarbon = double.Parse(line.ElementAt(18), cultureInfo),
                        PHInCalciumChloride = double.Parse(line.ElementAt(19), cultureInfo),
                        PHAsPerProjectReport = double.Parse(line.ElementAt(20), cultureInfo),
                        BaseSaturation = int.Parse(line.ElementAt(21), cultureInfo),
                        CationExchangeCapacity = int.Parse(line.ElementAt(22), cultureInfo),
                        SaturatedHydraulicConductivity = double.Parse(line.ElementAt(23), cultureInfo),
                        WaterRetentionAt0kP = int.Parse(line.ElementAt(24), cultureInfo),
                        WaterRetentionAt10kP = int.Parse(line.ElementAt(25), cultureInfo),
                        WaterRetentionAt33kP = int.Parse(line.ElementAt(26), cultureInfo),
                        WaterRetentionAt1500kP = int.Parse(line.ElementAt(27), cultureInfo),
                        BulkDensity = double.Parse(line.ElementAt(28), cultureInfo),
                        ElectricalConductivity = int.Parse(line.ElementAt(29), cultureInfo),
                        CalciumCarbonateEquivalent = int.Parse(line.ElementAt(30), cultureInfo),
                        VonPost = int.Parse(line.ElementAt(31), cultureInfo),
                        WoodyMaterial = int.Parse(line.ElementAt(32), cultureInfo)
                    };

                    if (string.IsNullOrWhiteSpace(entry.ProvinceCode) || entry.ProvinceCode.Equals("PR", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Some rows have an empty province code, in these cases get the province from the filename. The string "PR" is found in some rows and is probable an error when creating the CanSIS database
                        switch (csvResourceName)
                        {
                            case CsvResourceNames.CanSisSoilLayerTableAlberta:
                                {
                                    entry.Province = Province.Alberta;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableBritishColumbia:
                                {
                                    entry.Province = Province.BritishColumbia;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableManitoba:
                                {
                                    entry.Province = Province.Manitoba;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableNewBrunswick:
                                {
                                    entry.Province = Province.NewBrunswick;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableNewfoundland:
                                {
                                    entry.Province = Province.Newfoundland;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableNovaScotia:
                                {
                                    entry.Province = Province.NovaScotia;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableOntario:
                                {
                                    entry.Province = Province.Ontario;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTablePrinceEdwardIsland:
                                {
                                    entry.Province = Province.PrinceEdwardIsland;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableQuebec:
                                {
                                    entry.Province = Province.Quebec;
                                    break;
                                }
                            case CsvResourceNames.CanSisSoilLayerTableSaskatchewan:
                                {
                                    entry.Province = Province.Saskatchewan;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        entry.Province = _provinceStringConverter.Convert(entry.ProvinceCode);
                    }

                    results.Add(entry);

                    if (_soilLayerTableBySoilIdentifierDictionary.ContainsKey(entry.SoilNameIdentifier))
                    {
                        _soilLayerTableBySoilIdentifierDictionary[entry.SoilNameIdentifier].Add(entry);
                    }
                    else
                    {
                        _soilLayerTableBySoilIdentifierDictionary.Add(entry.SoilNameIdentifier, new List<SoilLayerTableData>() { entry });
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Get first non-litter layer using the soil name identifier from the soil component.
        /// </summary>
        private SoilLayerTableData GetFirstNonLitterLayer(ComponentTableData componentTableData)
        {
            var soilNameIdentifier = componentTableData.SoilNameIdentifier;

            var firstNonLitterLayer = this.GetFirstNonLitterLayerBySoilNameIdentifier(soilNameIdentifier);
            if (firstNonLitterLayer != null)
            {
                return firstNonLitterLayer;
            }

            Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} first non-litter layer not found component id '{componentTableData.PolygonComponentId}'. Returning null.");

            return null;
        }

        /// <summary>
        /// Get first non-litter layer using a polygon as a lookup key.
        /// </summary>
        private SoilLayerTableData GetFirstNonLitterLayer(int polygonId)
        {
            var largestComponentWithinPolygon = this.GetLargestComponentWithinPolygon(polygonId);
            if (largestComponentWithinPolygon == null)
            {
                return null;
            }

            var soilNameIdentifier = largestComponentWithinPolygon.SoilNameIdentifier;

            var firstNonLitterLayer = this.GetFirstNonLitterLayerBySoilNameIdentifier(soilNameIdentifier);
            if (firstNonLitterLayer != null)
            {
                return firstNonLitterLayer;
            }

            Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} first non-litter layer not found for polygon '{polygonId}'. Returning null.");

            return null;
        }

        private SoilLayerTableData GetAppropriateLayer(IEnumerable<SoilLayerTableData> entries)
        {
            // Select first item since there may be more than one match.
            var levelOneSoilData = entries.FirstOrDefault(x => x.LayerNumber.Equals("1") && x.UpperDepth >= 0);
            if (levelOneSoilData != null)
            {
                return levelOneSoilData;
            }

            // The first layer item has a negative upper depth value indicating that it is a litter layer. Return the second layer in this case.
            var levelTwoSoilData = entries.FirstOrDefault(x => x.LayerNumber.Equals("2"));
            if (levelTwoSoilData != null)
            {
                return levelTwoSoilData;
            }
            else
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(this.GetAppropriateLayer)} no layers found with layer numbers equal to 1 or 2.");

                return null;
            }
        }

        /// <summary>
        /// Gets the first non-litter layer by using the soil name identifier as a lookup key into the soil layer table.
        /// </summary>
        private SoilLayerTableData GetFirstNonLitterLayerBySoilNameIdentifier(string soilNameIdentifier)
        {
            if (_firstNonLitterLayerCache.ContainsKey(soilNameIdentifier))
            {
                return _firstNonLitterLayerCache[soilNameIdentifier];
            }

            if (_soilLayerTableBySoilIdentifierDictionary.ContainsKey(soilNameIdentifier))
            {
                var entriesBySoilNameIdentifier = _soilLayerTableBySoilIdentifierDictionary[soilNameIdentifier];
                var agriculturalProfileEntries = entriesBySoilNameIdentifier.Where(x => x.TypeOfSoilProfile.Equals(AgriculturalTypeSoilProfile));
                if (agriculturalProfileEntries.Any())
                {
                    var layer = this.GetAppropriateLayer(agriculturalProfileEntries);

                    if (_firstNonLitterLayerCache.ContainsKey(soilNameIdentifier) == false)
                    {
                        _firstNonLitterLayerCache.Add(soilNameIdentifier, layer);
                    }

                    return layer;
                }
                else
                {
                    Trace.TraceWarning($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} no soil layer table entries found for soil name id '{soilNameIdentifier}' with agricultural soil profile. Searching for native soil profiles.");
                }

                var nativeProfileEntries = entriesBySoilNameIdentifier.Where(x => x.TypeOfSoilProfile.Equals(NativeTypeSoilProfile));
                if (nativeProfileEntries.Any())
                {
                    var layer = this.GetAppropriateLayer(nativeProfileEntries);
                    Trace.TraceWarning($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} native soil profile was found.");

                    if (_firstNonLitterLayerCache.ContainsKey(soilNameIdentifier) == false)
                    {
                        _firstNonLitterLayerCache.Add(soilNameIdentifier, layer);
                    }

                    return layer;
                }
                else
                {
                    Trace.TraceWarning($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} no soil layer table entries found for soil name id '{soilNameIdentifier}' with native soil profile.");
                }
            }

            Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetFirstNonLitterLayer)} no soil layer table entry found for soil name id '{soilNameIdentifier}'. Returning null.");

            return null;
        }

        private ComponentTableData GetLargestComponentWithinPolygon(int polygonId)
        {
            var componentTableDataWithCommonPolygonId = _componentTableDataList.Where(x => x.PolygonId == polygonId).ToList();
            if (componentTableDataWithCommonPolygonId.Any())
            {
                var largestComponent = componentTableDataWithCommonPolygonId.OrderByDescending(x => x.PercentageOfPolygonOccupiedByComponent).FirstOrDefault();

                return largestComponent;
            }
            else
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(this.GetLargestComponentWithinPolygon)} value not found for polygon '{polygonId}'. Returning null.");

                return null;
            }
        }

        private SoilNameTableData GetSoilNameTableData(ComponentTableData componentTableData)
        {
            var soilNameIdentifier = componentTableData.SoilNameIdentifier;

            return this.GetSoilNameTableDataBySoilNameIdentifier(soilNameIdentifier);
        }

        private SoilNameTableData GetSoilNameTableData(int polygonId)
        {
            var largestComponentWithinPolygon = this.GetLargestComponentWithinPolygon(polygonId);
            var soilNameIdentifier = largestComponentWithinPolygon.SoilNameIdentifier;

            return this.GetSoilNameTableDataBySoilNameIdentifier(soilNameIdentifier);
        }

        private SoilNameTableData GetSoilNameTableDataBySoilNameIdentifier(string soilNameIdentifier)
        {
            var provinceCode = soilNameIdentifier.Substring(0, 2);
            var province = _provinceStringConverter.Convert(provinceCode);
            var filterByProvince = _soilNameTableDataList.Where(x => x.Province == province);

            if (province == Province.NewBrunswick)
            {
                // The soil id column in the cmp.csv file has incorrect soil ids for polygons in NB, these are in the format:
                // NBHOU~60~~N
                // The modifier part, '~60~~' is incorrect, it should have been '60~~~', i.e. no leading '~' characters
                // This was noticed because there is no 'NBHOU~60~~N' key in the soil name table for NB that matches. There is however a 'NBHOU60~~~N' key. So, since this
                // seems to be an issue with NB only, compare the soil ids *without* the '~' characters.

                var matches = new List<SoilNameTableData>();
                foreach (var tableData in filterByProvince)
                {
                    var tableDataNoTilde = tableData.SoilNameIdentifier.Replace("~", "");
                    var inputSoilNameIdentifierNoTilde = soilNameIdentifier.Replace("~", "");

                    if (tableDataNoTilde.Equals(inputSoilNameIdentifierNoTilde))
                    {
                        matches.Add(tableData);
                    }
                }

                // Get ag. profile first
                var agProfile = matches.FirstOrDefault(x => x.SoilNameIdentifier.EndsWith(AgriculturalTypeSoilProfile));
                if (agProfile != null)
                {
                    return agProfile;
                }

                // Get native profile next
                var nativeProfile = matches.FirstOrDefault(x => soilNameIdentifier.EndsWith(NativeTypeSoilProfile));
                if (nativeProfile != null)
                {
                    return nativeProfile;
                }

                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetSoilNameTableData)} value not found for soil name identifier '{soilNameIdentifier}'. Returning null.");

                return null;
            }

            // Select first item since there may be more than one match.
            var soilNameTableData = filterByProvince.FirstOrDefault(x => x.SoilNameIdentifier.Equals(soilNameIdentifier) && x.TypeOfSoilProfile.Equals(AgriculturalTypeSoilProfile));
            if (soilNameTableData != null)
            {
                return soilNameTableData;
            }

            soilNameTableData = filterByProvince.FirstOrDefault(x => x.SoilNameIdentifier.Equals(soilNameIdentifier) && x.TypeOfSoilProfile.Equals(NativeTypeSoilProfile));
            if (soilNameTableData != null)
            {
                return soilNameTableData;
            }
            else
            {
                Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(GetSoilNameTableData)} value not found for soil name identifier '{soilNameIdentifier}'. Returning null.");

                return null;
            }
        }

        private Province GetProvinceFromProvinceCode(string province)
        {
            return _provinceStringConverter.Convert(province);
        }

        private SoilGreatGroupType ConvertSoilGreatGroupCode(string soilGreatGroup)
        {
            switch (soilGreatGroup.ToLower())
            {
                case "mb":
                    return SoilGreatGroupType.MelanicBrunisol;

                case "eb":
                    return SoilGreatGroupType.EutricBrunisol;

                case "sb":
                    return SoilGreatGroupType.SombricBrunisol;

                case "dyb":
                    return SoilGreatGroupType.DystricBrunisol;

                case "bc":
                    return SoilGreatGroupType.BrownChernozem;

                case "dbc":
                    return SoilGreatGroupType.DarkBrownChernozem;

                case "blc":
                    return SoilGreatGroupType.BlackChernozem;

                case "dgc":
                    return SoilGreatGroupType.DarkGrayChernozem;

                case "tc":
                    return SoilGreatGroupType.TurbicCryosol;

                case "sc":
                    return SoilGreatGroupType.StaticCryosol;

                case "oc":
                    return SoilGreatGroupType.OrganicCryosol;

                case "hg":
                    return SoilGreatGroupType.HumicGleysol;

                case "g":
                    return SoilGreatGroupType.Gleysol;

                case "lg":
                    return SoilGreatGroupType.LuvicGleysol;

                case "gbl":
                    return SoilGreatGroupType.GrayBrownLuvisol;

                case "gl":
                    return SoilGreatGroupType.GrayLuvisol;

                case "f":
                    return SoilGreatGroupType.Fibrisol;

                case "m":
                    return SoilGreatGroupType.Mesisol;

                case "h":
                    return SoilGreatGroupType.Humisol;

                case "fo":
                    return SoilGreatGroupType.Folisol;

                case "hp":
                    return SoilGreatGroupType.HumicPodzol;

                case "fhp":
                    return SoilGreatGroupType.FerroHumicPodzol;

                case "hfp":
                    return SoilGreatGroupType.HumoFerricPodzol;

                case "r":
                    return SoilGreatGroupType.Regosol;

                case "rg":
                    return SoilGreatGroupType.Regosol;

                case "hr":
                    return SoilGreatGroupType.HumicRegosol;

                case "sz":
                    return SoilGreatGroupType.Solonetz;

                case "ss":
                    return SoilGreatGroupType.SolodizedSolonetz;

                case "so":
                    return SoilGreatGroupType.Solod;

                case "vsz":
                    return SoilGreatGroupType.VerticSolonetz;

                case "v":
                    return SoilGreatGroupType.Vertisol;

                case "hv":
                    return SoilGreatGroupType.HumicVertisol;

                default:
                    {
                        Trace.TraceError($"{nameof(NationalSoilDataBaseProvider)}.{nameof(this.ConvertSoilGreatGroupCode)} value not found for soil great group code '{soilGreatGroup.ToLowerInvariant()}'.");

                        return SoilGreatGroupType.Unknown;
                    }
            }
        }

        private SoilFunctionalCategory ConvertSoilGreatGroupCodeToSoilFunctionalCategory(
            SoilGreatGroupType soilGreatGroup)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}