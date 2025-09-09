#region Imports

using H.Core.Enumerations;
using H.Core.Properties;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Soil
{
    /// <summary>
    ///     A class to hold soil properties for a field or farm location
    /// </summary>
    public class SoilData : ModelBase
    {
        #region Constructors

        public SoilData()
        {
            SoilName = Resources.EnumDefault;
            BulkDensity = 0;
            ProportionOfClayInSoil = 0;
            ProportionOfSandInSoil = 0;
            ProportionOfSoilOrganicCarbon = 0;
            SoilGreatGroup = SoilGreatGroupType.Unknown;
            SoilSubGroup = Resources.Unknown;
            TopLayerThickness = 0;
            SoilPh = 0;
            SoilCec = 0;
            SoilFunctionalCategory = SoilFunctionalCategory.Unknown;
            SoilTexture = SoilTexture.Unknown;
            EcodistrictName = Resources.Unknown;
            EcodistrictId = 0;
            Ecozone = Ecozone.Unknown;
            DrainageClass = SoilDrainageClasses.Unknown;
        }

        #endregion

        #region Fields

        private int _polygonId;
        private int _componentId;
        private int _ecodistrictId;

        private double _soilCec;
        private double _soilPh;
        private double _proportionOfSandInSoil;
        private double _proportionOfClayInSoil;
        private double _topLayerThickness;
        private double _proportionOfSoilOrganicCarbon;
        private double _bulkDensity;

        private string _soilName;
        private string _ecodistrictName;
        private string _soilSubGroup;

        private SoilDrainageClasses _drainageClass;
        private SoilTexture _soilTexture;
        private SoilGreatGroupType _soilGreatGroup;
        private SoilFunctionalCategory _soilFunctionalCategory;
        private Province _province;
        private ParentMaterialTextureType _parentMaterialTextureType;
        private Ecozone _ecozone;
        private bool _isDefaultSoilData;

        #endregion

        #region Properties

        public int PolygonId
        {
            get => _polygonId;
            set => SetProperty(ref _polygonId, value);
        }

        public int ComponentId
        {
            get => _componentId;
            set => SetProperty(ref _componentId, value);
        }

        public int EcodistrictId
        {
            get => _ecodistrictId;
            set => SetProperty(ref _ecodistrictId, value);
        }

        /// <summary>
        ///     (unitless)
        /// </summary>
        public double ProportionOfClayInSoil
        {
            get => _proportionOfClayInSoil;
            set
            {
                SetProperty(ref _proportionOfClayInSoil, value,
                    () => { RaisePropertyChanged(nameof(ProportionOfClayInSoilAsPercentage)); });
            }
        }

        /// <summary>
        ///     (%)
        /// </summary>
        public double ProportionOfClayInSoilAsPercentage => ProportionOfClayInSoil * 100;

        public double ClayInSoilInGramsPerKilogram => ProportionOfClayInSoil * 1000;

        /// <summary>
        ///     Top layer thickness
        ///     (mm)
        /// </summary>
        public double TopLayerThickness
        {
            get => _topLayerThickness;
            set => SetProperty(ref _topLayerThickness, value);
        }

        /// <summary>
        ///     (unitless)
        /// </summary>
        public double ProportionOfSandInSoil
        {
            get => _proportionOfSandInSoil;
            set
            {
                SetProperty(ref _proportionOfSandInSoil, value,
                    () => { RaisePropertyChanged(nameof(ProportionOfSandInSoilAsPercentage)); });
            }
        }

        /// <summary>
        ///     (%)
        /// </summary>
        public double ProportionOfSandInSoilAsPercentage => ProportionOfSandInSoil * 100;

        public double ProportionOfSoilOrganicCarbon
        {
            get => _proportionOfSoilOrganicCarbon;
            set => SetProperty(ref _proportionOfSoilOrganicCarbon, value);
        }

        public double SoilOrganicCarbonInGramsPerKilogram => ProportionOfSoilOrganicCarbon * 1000;

        public double BulkDensity
        {
            get => _bulkDensity;
            set => SetProperty(ref _bulkDensity, value);
        }

        public SoilGreatGroupType SoilGreatGroup
        {
            get => _soilGreatGroup;
            set => SetProperty(ref _soilGreatGroup, value);
        }

        public string SoilGreatGroupString => SoilGreatGroup.GetDescription();

        public double SoilPh
        {
            get => _soilPh;
            set => SetProperty(ref _soilPh, value);
        }

        /// <summary>
        ///     Cation exchange capacity.
        ///     NOTE: CanSIS db returns '-9' if value is not available
        ///     [milliequivalents per 100g (mEq/100g)]
        /// </summary>
        public double SoilCec
        {
            get => _soilCec;
            set => SetProperty(ref _soilCec, value);
        }

        public SoilFunctionalCategory SoilFunctionalCategory
        {
            get => _soilFunctionalCategory;
            set => SetProperty(ref _soilFunctionalCategory, value);
        }

        public string SoilFunctionalCategoryString => SoilFunctionalCategory.GetDescription();

        public string SoilNameAndGreatGroup => CombinedSoilNameDisplayString + " - " + SoilGreatGroupString;

        public Province Province
        {
            get => _province;
            set { SetProperty(ref _province, value, () => { RaisePropertyChanged(nameof(ProvinceString)); }); }
        }

        public string ProvinceString => Province.GetDescription();

        public ParentMaterialTextureType ParentMaterialTextureString
        {
            get => _parentMaterialTextureType;
            set => SetProperty(ref _parentMaterialTextureType, value);
        }

        public string ParentMaterialString => ParentMaterialTextureString.GetDescription();

        /// <summary>
        ///     The soil texture associated with the polygon. Can be reset by user in the SoilDetailsView
        /// </summary>
        public SoilTexture SoilTexture
        {
            get => _soilTexture;
            set { SetProperty(ref _soilTexture, value, () => { RaisePropertyChanged(nameof(SoilTextureString)); }); }
        }

        public string UserFriendlySoilTextureDisplayString
        {
            get
            {
                if (SoilTexture == SoilTexture.Fine) return UserFriendlySoilTextures.Clay.GetDescription();

                if (SoilTexture == SoilTexture.Medium) return UserFriendlySoilTextures.Loam.GetDescription();

                return UserFriendlySoilTextures.Sand.GetDescription();
            }
        }

        /// <summary>
        ///     Soil names must be displayed as black, brown, etc. for AB, MB, SK - for all other provinces, soil names must be of
        ///     the form "LOCATION-LOAM, LOCATION_2-SAND, etc."
        /// </summary>
        public string CombinedSoilNameDisplayString
        {
            get
            {
                switch (Province)
                {
                    case Province.Manitoba:
                    case Province.Alberta:
                    case Province.Saskatchewan:
                    {
                        return SoilFunctionalCategoryString;
                    }

                    default:
                    {
                        return $"{SoilName} - {UserFriendlySoilTextureDisplayString}";
                    }
                }
            }
        }

        public string SoilTextureString => SoilTexture.GetDescription();

        public string EcodistrictName
        {
            get => _ecodistrictName;
            set => SetProperty(ref _ecodistrictName, value);
        }

        public SoilDrainageClasses DrainageClass
        {
            get => _drainageClass;
            set
            {
                SetProperty(ref _drainageClass, value,
                    () => { RaisePropertyChanged(nameof(SoilDrainageClassString)); });
            }
        }

        public string SoilDrainageClassString => DrainageClass.GetDescription();

        public string SoilName
        {
            get => _soilName;
            set => SetProperty(ref _soilName, value);
        }

        public string SoilSubGroup
        {
            get => _soilSubGroup;
            set => SetProperty(ref _soilSubGroup, value);
        }

        public Ecozone Ecozone
        {
            get => _ecozone;
            set { SetProperty(ref _ecozone, value, () => { RaisePropertyChanged(nameof(EcozoneString)); }); }
        }

        public string EcozoneString => Ecozone.GetDescription();

        public bool IsDefaultSoilData
        {
            get => _isDefaultSoilData;
            set => SetProperty(ref _isDefaultSoilData, value);
        }

        #endregion

        #region Public Methods

        public bool IsOrganic()
        {
            return SoilFunctionalCategory == SoilFunctionalCategory.Organic;
        }

        public override string ToString()
        {
            return
                $"{nameof(PolygonId)}: {PolygonId}, {nameof(Province)}: {Province}, {nameof(EcodistrictName)}: {EcodistrictName}, {nameof(ProportionOfClayInSoil)}: {ProportionOfClayInSoil}, {nameof(TopLayerThickness)}: {TopLayerThickness}, {nameof(ProportionOfSandInSoil)}: {ProportionOfSandInSoil}, {nameof(ProportionOfSoilOrganicCarbon)}: {ProportionOfSoilOrganicCarbon}, {nameof(BulkDensity)}: {BulkDensity}, {nameof(SoilGreatGroup)}: {SoilGreatGroup}, {nameof(SoilPh)}: {SoilPh}, {nameof(SoilFunctionalCategory)}: {SoilFunctionalCategory}, {nameof(ParentMaterialTextureString)}: {ParentMaterialTextureString}, {nameof(SoilTexture)}: {SoilTexture}";
        }

        #endregion
    }
}