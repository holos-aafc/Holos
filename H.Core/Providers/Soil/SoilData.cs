#region Imports

using H.Core.Enumerations;
using H.Infrastructure;

#endregion

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// A class to hold soil properties for a field or farm location
    /// </summary>
    public class SoilData : ModelBase
    {
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

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public int PolygonId
        {
            get
            {
                return _polygonId;
            }
            set
            {
                SetProperty(ref _polygonId, value);
            }
        }

        public int ComponentId
        {
            get
            {
                return _componentId;
            }
            set
            {
                SetProperty(ref _componentId, value);
            }
        }

        public int EcodistrictId
        {
            get
            {
                return _ecodistrictId;
            }
            set
            {
                SetProperty(ref _ecodistrictId, value);
            }
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double ProportionOfClayInSoil
        {
            get { return _proportionOfClayInSoil; }
            set
            {
                this.SetProperty(ref _proportionOfClayInSoil, value, () => { this.RaisePropertyChanged(nameof(this.ProportionOfClayInSoilAsPercentage)); });
            }
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double ProportionOfClayInSoilAsPercentage
        {
            get { return this.ProportionOfClayInSoil * 100; }
        }

        public double ClayInSoilInGramsPerKilogram
        {
            get { return this.ProportionOfClayInSoil * 1000; }
        }

        /// <summary>
        /// Top layer thickness
        ///
        /// (mm)
        /// </summary>
        public double TopLayerThickness
        {
            get { return _topLayerThickness; }
            set { SetProperty(ref _topLayerThickness, value); }
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double ProportionOfSandInSoil
        {
            get { return _proportionOfSandInSoil; }
            set
            {
                this.SetProperty(ref _proportionOfSandInSoil, value, () => { this.RaisePropertyChanged(nameof(this.ProportionOfSandInSoilAsPercentage)); });
            }
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double ProportionOfSandInSoilAsPercentage
        {
            get { return this.ProportionOfSandInSoil * 100; }
        }

        public double ProportionOfSoilOrganicCarbon
        {
            get
            {
                return _proportionOfSoilOrganicCarbon;
            }
            set
            {
                SetProperty(ref _proportionOfSoilOrganicCarbon, value);
            }
        }

        public double SoilOrganicCarbonInGramsPerKilogram
        {
            get { return this.ProportionOfSoilOrganicCarbon * 1000; }
        }

        public double BulkDensity
        {
            get
            {
                return _bulkDensity;
            }
            set
            {
                SetProperty(ref _bulkDensity, value);
            }
        }

        public SoilGreatGroupType SoilGreatGroup
        {
            get
            {
                return _soilGreatGroup;
            }
            set
            {
                SetProperty(ref _soilGreatGroup, value);
            }
        }

        public string SoilGreatGroupString
        {
            get { return this.SoilGreatGroup.GetDescription(); }
        }

        public double SoilPh
        {
            get
            {
                return _soilPh;
            }
            set
            {
                SetProperty(ref _soilPh, value);
            }
        }

        /// <summary>
        /// Cation exchange capacity.
        /// 
        /// NOTE: CanSIS db returns '-9' if value is not available
        /// 
        /// [milliequivalents per 100g (mEq/100g)]
        /// </summary>
        public double SoilCec
        {
            get
            {
                return _soilCec;
            }
            set
            {
                SetProperty(ref _soilCec, value);
            }
        }

        public SoilFunctionalCategory SoilFunctionalCategory
        {
            get
            {
                return _soilFunctionalCategory;
            }
            set
            {
                SetProperty(ref _soilFunctionalCategory, value);
            }
        }

        public string SoilFunctionalCategoryString
        {
            get { return this.SoilFunctionalCategory.GetDescription(); }
        }

        public Province Province
        {
            get
            {
                return _province;
            }
            set
            {
                SetProperty(ref _province, value, () => { RaisePropertyChanged(nameof(this.ProvinceString)); });
            }
        }

        public string ProvinceString
        {
            get { return this.Province.GetDescription(); }
        }

        public ParentMaterialTextureType ParentMaterialTextureString
        {
            get
            {
                return _parentMaterialTextureType;
            }
            set
            {
                SetProperty(ref _parentMaterialTextureType, value);
            }
        }

        public string ParentMaterialString
        {
            get { return this.ParentMaterialTextureString.GetDescription(); }
        }

        /// <summary>
        /// The soil texture associated with the polygon. Can be reset by user in the SoilDetailsView
        /// </summary>
        public SoilTexture SoilTexture
        {
            get
            {
                return _soilTexture;
            }
            set
            {
                SetProperty(ref _soilTexture, value, () => { RaisePropertyChanged(nameof(this.SoilTextureString)); });
            }
        }

        public string UserFriendlySoilTextureDisplayString
        {
            get
            {
                if (this.SoilTexture == SoilTexture.Fine)
                {
                    return UserFriendlySoilTextures.Clay.GetDescription();
                }

                if (this.SoilTexture == SoilTexture.Medium)
                {
                    return UserFriendlySoilTextures.Loam.GetDescription();
                }

                return UserFriendlySoilTextures.Sand.GetDescription();
            }
        }

        /// <summary>
        /// Soil names must be displayed as black, brown, etc. for AB, MB, SK - for all other provinces, soil names must be of the form "LOCATION-LOAM, LOCATION_2-SAND, etc."
        /// </summary>
        public string CombinedSoilNameDisplayString
        {
            get
            {
                switch (this.Province)
                {
                    case Province.Manitoba:
                    case Province.Alberta:
                    case Province.Saskatchewan:
                        {
                            return this.SoilFunctionalCategoryString;
                        }

                    default:
                        {
                            return $"{this.SoilName} - {this.UserFriendlySoilTextureDisplayString}";
                        }
                }
            }
        }

        public string SoilTextureString
        {
            get { return this.SoilTexture.GetDescription(); }
        }

        public string EcodistrictName
        {
            get
            {
                return _ecodistrictName;
            }
            set
            {
                SetProperty(ref _ecodistrictName, value);
            }
        }

        public SoilDrainageClasses DrainageClass
        {
            get { return _drainageClass; }
            set { SetProperty(ref _drainageClass, value, () => { base.RaisePropertyChanged(nameof(this.SoilDrainageClassString)); }); }
        }

        public string SoilDrainageClassString
        {
            get { return this.DrainageClass.GetDescription(); }
        }

        public string SoilName
        {
            get { return _soilName; }
            set { SetProperty(ref _soilName, value); }
        }

        public string SoilSubGroup
        {
            get
            {
                return _soilSubGroup;
            }
            set
            {
                SetProperty(ref _soilSubGroup, value);
            }
        }

        public Ecozone Ecozone
        {
            get
            {
                return _ecozone;
            }
            set
            {
                SetProperty(ref _ecozone, value, () => { RaisePropertyChanged(nameof(this.EcozoneString)); });
            }
        }

        public string EcozoneString
        {
            get { return this.Ecozone.GetDescription(); }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return
                $"{nameof(this.PolygonId)}: {this.PolygonId}, {nameof(this.Province)}: {this.Province}, {nameof(this.EcodistrictName)}: {this.EcodistrictName}, {nameof(this.ProportionOfClayInSoil)}: {this.ProportionOfClayInSoil}, {nameof(this.TopLayerThickness)}: {this.TopLayerThickness}, {nameof(this.ProportionOfSandInSoil)}: {this.ProportionOfSandInSoil}, {nameof(this.ProportionOfSoilOrganicCarbon)}: {this.ProportionOfSoilOrganicCarbon}, {nameof(this.BulkDensity)}: {this.BulkDensity}, {nameof(this.SoilGreatGroup)}: {this.SoilGreatGroup}, {nameof(this.SoilPh)}: {this.SoilPh}, {nameof(this.SoilFunctionalCategory)}: {this.SoilFunctionalCategory}, {nameof(this.ParentMaterialTextureString)}: {this.ParentMaterialTextureString}, {nameof(this.SoilTexture)}: {this.SoilTexture}";
        }

        #endregion
    }
}