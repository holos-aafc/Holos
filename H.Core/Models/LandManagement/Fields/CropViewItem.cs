#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using H.Core.Enumerations;
using H.Core.Models.Results;
using H.Core.Providers.Animals;
using H.Core.Providers.Economics;
using H.Infrastructure;

#endregion

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem : ModelBase
    {
        #region Fields

        private int _phaseNumber;
        private int _yearInPerennialStand;
        private int _perennialStandLength;
        private int _year;
        private int _yearOfSeeding;
        private int _yearOfConversion;
        private int _yearOfFallowChange;
        private int _yearOfTillageChange;
        private int _numberOfPesticidePasses;

        private string _fieldName;

        private bool _manureApplied;
        private bool _underSownCropsUsed;
        private bool _cropIsGrazed;
        private bool _isNativeGrassland;
        private bool _isBrokenGrassland;
        private bool _itemCanBeMovedUp;
        private bool _itemCanBeMovedDown;
        private bool _enableCustomUserDefaultsForThisCrop;
        private bool _cropEconomicDataApplied;
        private bool _hasHarvestViewItems;
        private bool _hasHayImportViewItems;
        private bool _hasGrazingViewItems;
        private bool _hasManureApplicationViewItems;
        private bool _isSecondaryCrop;

        private double _area;
        private double _pastFallowArea;
        private double _pastPerennialArea;
        private double _yield;
        private double _dryYield;
        private double _defaultYield;
        private double _totalHarvestBiomass;
        private double _soilTestNitrogen;
        private double _moistureContentOfCrop;
        private double _moistureContentOfCropPercentage;
        private double _amountOfIrrigation;
        private double _amountOfManureApplied;
        
        private double _nitrogenFertilizerEfficiencyPercentage;
        private double _nitrogenFertilizerRateOnStubble;
        private double _nitrogenFertilizerRateOnFallow;
        private double _nitrogenDepositionAmount;
        
        private double _percentageOfStrawReturnedToSoil;
        private double _percentageOfRootsReturnedToSoil;
        private double _percentageOfProductYieldReturnedToSoil;
        private double _fuelEnergy;
        private double _herbicideEnergy;

        /*
         * Main crop residues
         */

        private double _biomassCoefficientProduct;
        private double _biomassCoefficientStraw;
        private double _biomassCoefficientRoots;
        private double _biomassCoefficientExtraroot;

        private double _nitrogenContentInProduct;
        private double _nitrogenContentInStraw;
        private double _nitrogenContentInRoots;
        private double _nitrogenContentInExtraroot;

        private double _carbonConcentration;
        private double _carbonInputFromProduct;
        private double _carbonInputFromStraw;
        private double _carbonInputFromRoots;
        private double _carbonInputFromExtraroots;
        private double _lumCMax;
        private double _kValue;
        private double _nitrogenFixation;
        private double _forageUtilizationRate;

        private Guid _perennialStandGroupId;
        private Guid _detailViewItemToComponentSelectionViewItemMap;

        private Response _isIrrigated;
        private Response _isPesticideUsed;

        private ManureLocationSourceType _manureLocationSourceType;
        private ManureStateType _manureStateType;
        private ManureApplicationTypes _manureApplicationType;
        private ManureAnimalSourceTypes _manureAnimalSourceType;
        private FallowTypes _selectedFallowType;
        private IrrigationType _irrigationType;
        private CropType _cropType;
        private CropType _grasslandType;
        private CropEconomicData _cropEconomicData;
        private NitrogenFertilizerType _nitrogenFertilizerType;
        private FertilizerApplicationMethodologies _fertilizerApplicationMethodology;
        private Seasons _seasonOfFertilizerApplication;
        private CoverCropTerminationType _coverCoverCropTerminationType;

        private HarvestMethods _harvestMethod;

        private TillageType _tillageType;
        private TillageType _currentTillageType;
        private TillageType _pastTillageType;

        private double _aboveGroundCarbonInput;
        private double _belowGroundCarbonInput;
        private double _plantCarbonInAgriculturalProduct;
        private double _climateParameter;
        private double _tillageFactor;
        private double _managementFactor;
        private double _manureCarbonInput;
        private double _manureCarbonPerHectare;
        private double _ligninContent;

        private string _timePeriodCategoryString;
        private string _managementPeriodName;
        private string _cropTypeString;
        private string _harvestMethodString;
        private string _tillageTypeString;
        private string _cropTypeStringWithYear;

        #endregion

        #region Constructors

        public CropViewItem()
        {
            this.PropertyChanged += OnPropertyChanged;

            this.Area = 1;
            this.PastFallowArea = this.Area;                // Assume past areas are the same as current areas when starting out
            this.PastPerennialArea = this.Area;             // Assume past areas are the same as current areas when starting out

            this.Yield = 1000;                              // Start all crops with this yield, assign a CAR yield when user adds crop to field
            this.IsIrrigated = Response.No;
            this.IrrigationType = IrrigationType.RainFed;

            this.TillageType = TillageType.Reduced;
            this.PastTillageType = TillageType.Reduced;

            this.HarvestMethod = HarvestMethods.CashCrop;
            this.MoistureContentOfCropPercentage = 12;
            this.SoilReductionFactor = SoilReductionFactors.None;

            this.CoverCropTerminationType = CoverCropTerminationType.Natural;

            this.NitrogenDepositionAmount = 5;
            this.NitrogenFertilizerRateOnFallow = 14;       // From v3 GUI
            this.NitrogenFertilizerRateOnStubble = 47;      // From v3 GUI

            this.YearOfFallowChange = CoreConstants.IcbmEquilibriumYear;
            this.YearOfTillageChange = CoreConstants.IcbmEquilibriumYear;
            this.YearOfConversion = CoreConstants.IcbmEquilibriumYear;

            this.CarbonConcentration = CoreConstants.CarbonConcentration;

            this.CropEconomicData = new CropEconomicData();
            this.IpccTier2NitrogenResults = new IPCCTier2Results();
            this.IpccTier2CarbonResults = new IPCCTier2Results();

            this.IsPesticideUsed = Response.No;
            this.PerennialStandLength = DefaultPerennialStandLength;

            this.DoNotRecalculatePlantCarbonInAgriculturalProduct = false;

            this.GrazingViewItems = new ObservableCollection<GrazingViewItem>();


            this.FertilizerApplicationViewItems = new ObservableCollection<FertilizerApplicationViewItem>();
            this.FertilizerApplicationViewItems.CollectionChanged += FertilizerApplicationViewItemsOnCollectionChanged;

            this.HarvestViewItems.CollectionChanged += HarvestViewItemsOnCollectionChanged;
            this.HayImportViewItems.CollectionChanged += HayImportViewItemsOnCollectionChanged;
            this.GrazingViewItems.CollectionChanged += GrazingViewItemsOnCollectionChanged;
            this.ManureApplicationViewItems.CollectionChanged += ManureApplicationViewItemsOnCollectionChanged;
        }

        #endregion

        #region Properties

        public bool CropEconomicDataApplied
        {
            get => _cropEconomicDataApplied;
            set => SetProperty(ref _cropEconomicDataApplied, value);
        }

        /// <summary>
        /// A collection of hay import events that occurs when animals grazing on pasture need additional forage for consumption.
        /// </summary>
        public ObservableCollection<HayImportViewItem> HayImportViewItems { get; set; } = new ObservableCollection<HayImportViewItem>();
        public ObservableCollection<HarvestViewItem> HarvestViewItems { get; set; } = new ObservableCollection<HarvestViewItem>();

        /// <summary>
        /// A collection of all manure applications made to the crop in the year.
        /// </summary>
        public ObservableCollection<ManureApplicationViewItem> ManureApplicationViewItems { get; set; } = new ObservableCollection<ManureApplicationViewItem>();

        /// <summary>
        /// Returns true if the user has entered a value for the number of pesticide passes
        /// </summary>
        public bool IsHerbicideUsed
        {
            get
            {
                return this.NumberOfPesticidePasses > 0;
            }
        }

        public bool ItemCanBeMovedUp
        {
            get { return _itemCanBeMovedUp; }
            set { SetProperty(ref _itemCanBeMovedUp, value); }
        }

        public bool ItemCanBeMovedDown
        {
            get { return _itemCanBeMovedDown; }
            set { SetProperty(ref _itemCanBeMovedDown, value); }
        }

        public static int DefaultPerennialStandLength
        {
            get { return 1; }
        }

        public bool IsSelectedCropTypeRootCrop
        {
            get { return this.CropType.IsRootCrop(); }
        }

        public bool IsSelectedCropTypeBrokenGrassland
        {
            get { return this.IsBrokenGrassland; }
        }

        public int PhaseNumber
        {
            get { return _phaseNumber; }
            set { this.SetProperty(ref _phaseNumber, value); }
        }

        public CropType CropType
        {
            get { return _cropType; }
            set { this.SetProperty(ref _cropType, value, this.OnCropTypeChanged); }
        }

        /// <summary>
        /// Calling GetDescription (because of reflection) here is a huge bottleneck, return ToString only for now until a better solution is found.
        /// </summary>
        public string CropTypeString
        {
            get
            {
                return _cropTypeString;
            }
            set
            {
                SetProperty(ref _cropTypeString, value);
            }
        }

        /// <summary>
        /// The current tillage type (there is a separate property for past tillage type)
        /// </summary>
        public TillageType TillageType
        {
            get { return _tillageType; }
            set { this.SetProperty(ref _tillageType, value, OnTillageTypeChanged); }
        }

        /// <summary>
        /// Total area of crop, fallow area, grassland, etc.
        ///
        /// (ha)
        /// </summary>
        public double Area
        {
            get { return _area; }
            set { this.SetProperty(ref _area, value); }
        }

        public bool ManureApplied
        {
            get { return _manureApplied; }
            set { this.SetProperty(ref _manureApplied, value); }
        }

        public ManureLocationSourceType ManureLocationSourceType
        {
            get { return _manureLocationSourceType; }
            set { this.SetProperty(ref _manureLocationSourceType, value); }
        }

        public ManureAnimalSourceTypes ManureAnimalSourceType
        {
            get { return _manureAnimalSourceType; }
            set { this.SetProperty(ref _manureAnimalSourceType, value); }
        }

        /// <summary>
        /// The yield of the crop (wet weight). Holos uses this property (not the optional user entered dry weight) in soil carbon change calculations.
        /// 
        /// (kg ha^-1)
        /// </summary>
        public double Yield
        {
            get { return _yield; }
            set { this.SetProperty(ref _yield, value); }
        }

        /// <summary>
        /// The yield of the crop (dry weight not fresh weight - i.e. moisture weight is subtracted)
        ///
        /// (kg ha^-1)
        /// </summary>
        public double DryYield
        {
            get => _dryYield;
            set => SetProperty(ref _dryYield, value);
        }

        /// <summary>
        /// The total biomass value from the harvest tab of a perennial crop. The value is calculated based on
        /// (number of bales * weight per bale). This is set by the user when adding harvest information.
        /// 
        /// This property is set to default yield value when crop is initially loaded. The value is reset to default
        /// yield again when the user removes all harvest information for their component.
        /// </summary>
        public double TotalBiomassHarvest
        {
            get => _totalHarvestBiomass;
            set
            {
                if (!this.HasHarvestViewItems)
                {
                    value = this.DefaultYield;
                }

                SetProperty(ref _totalHarvestBiomass, value);
            }
        }

        /// <summary>
        /// The default yield as read from the data files. This is set when the yield value is set for the first
        /// time based on lookup table defaults.
        /// 
        /// (kg ha^-1)
        /// </summary>
        public double DefaultYield
        {
            get => _defaultYield;
            set => SetProperty(ref _defaultYield, value);
        }

        public Response IsIrrigated
        {
            get { return _isIrrigated; }
            set { this.SetProperty(ref _isIrrigated, value, OnIsIrrigatedChanged); }
        }

        public Response IsPesticideUsed
        {
            get { return _isPesticideUsed; }
            set { this.SetProperty(ref _isPesticideUsed, value); }
        }

        public int PerennialStandLength
        {
            get { return _perennialStandLength; }
            set { this.SetProperty(ref _perennialStandLength, value); }
        }

        public double AmountOfIrrigation
        {
            get { return _amountOfIrrigation; }
            set { this.SetProperty(ref _amountOfIrrigation, value); }
        }

        /// <summary>
        /// Fraction
        ///
        /// (unitless)
        /// </summary>
        public double MoistureContentOfCrop
        {
            get { return _moistureContentOfCrop; }
            set
            {
                if (value >= 1)
                {
                    // Old farms had this improperly set
                    value /= 100;
                }

                if (value >= 1000)
                {
                    // Old farms had this improperly set
                    value /= 100;
                    value /= 100;
                }

                this.SetProperty(ref _moistureContentOfCrop, value);
            }
        }

        /// <summary>
        /// Moisture content of crop expressed as a percentage.
        /// 
        /// %
        /// </summary>
        public double MoistureContentOfCropPercentage
        {
            get { return _moistureContentOfCropPercentage; }
            set { SetProperty(ref _moistureContentOfCropPercentage, value, () => { this.MoistureContentOfCrop = value / 100; }); }
        }

        /// <summary>
        /// Set to true when this view item is a perennial crop and the previous year is an annual crop and the user wants to indicate
        /// that this year's crop (the perennial) is undersown into the previous year's crop (the annual).
        /// </summary>
        public bool UnderSownCropsUsed
        {
            get { return _underSownCropsUsed; }
            set { this.SetProperty(ref _underSownCropsUsed, value); }
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { this.SetProperty(ref _fieldName, value); }
        }

        public int Year
        {
            get { return _year; }
            set { this.SetProperty(ref _year, value, () => { _cropTypeStringWithYear = $"[{_year}] - {_cropTypeString}"; }); }
        }

        public int YearInPerennialStand
        {
            get { return _yearInPerennialStand; }
            set { this.SetProperty(ref _yearInPerennialStand, value, this.OnYearInPerennialStandChanged); }
        }

        private void OnYearInPerennialStandChanged()
        {
        }

        public Guid PerennialStandGroupId
        {
            get { return _perennialStandGroupId; }
            set { this.SetProperty(ref _perennialStandGroupId, value); }
        }

        public bool CropIsGrazed
        {
            get { return _cropIsGrazed; }
            set { this.SetProperty(ref _cropIsGrazed, value); }
        }

        public int NumberOfPesticidePasses
        {
            get { return _numberOfPesticidePasses; }
            set { this.SetProperty(ref _numberOfPesticidePasses, value); }
        }

        public ManureApplicationTypes ManureApplicationType
        {
            get { return _manureApplicationType; }
            set { this.SetProperty(ref _manureApplicationType, value); }
        }

        public HarvestMethods HarvestMethod
        {
            get { return _harvestMethod; }
            set { this.SetProperty(ref _harvestMethod, value, OnHarvestMethodChanged); }
        }

        public string HarvestMethodString
        {
            get => _harvestMethodString;
            set => SetProperty(ref _harvestMethodString, value);
        }

        /// <summary>
        /// S_p
        /// 
        /// %
        /// </summary>
        public double PercentageOfProductYieldReturnedToSoil
        {
            get { return _percentageOfProductYieldReturnedToSoil; }
            set { SetProperty(ref _percentageOfProductYieldReturnedToSoil, value); }
        }

        /// <summary>
        /// %
        /// </summary>
        public double PercentageOfRootsReturnedToSoil
        {
            get { return _percentageOfRootsReturnedToSoil; }
            set { SetProperty(ref _percentageOfRootsReturnedToSoil, value); }
        }

        /// <summary>
        /// %
        /// </summary>
        public double PercentageOfStrawReturnedToSoil
        {
            get { return _percentageOfStrawReturnedToSoil; }
            set { this.SetProperty(ref _percentageOfStrawReturnedToSoil, value); }
        }

        /// <summary>
        /// (Fraction)
        /// </summary>
        public double CarbonConcentration
        {
            get { return _carbonConcentration; }
            set { SetProperty(ref _carbonConcentration, value); }
        }

        public IrrigationType IrrigationType
        {
            get { return _irrigationType; }
            set { SetProperty(ref _irrigationType, value); }
        }

        /// <summary>
        /// R_p
        /// </summary>
        public double BiomassCoefficientProduct
        {
            get { return _biomassCoefficientProduct; }
            set { SetProperty(ref _biomassCoefficientProduct, value); }
        }

        /// <summary>
        /// R_s
        /// </summary>
        public double BiomassCoefficientStraw
        {
            get { return _biomassCoefficientStraw; }
            set { SetProperty(ref _biomassCoefficientStraw, value); }
        }

        /// <summary>
        /// R_r
        /// </summary>
        public double BiomassCoefficientRoots
        {
            get { return _biomassCoefficientRoots; }
            set { SetProperty(ref _biomassCoefficientRoots, value); }
        }

        /// <summary>
        /// R_e
        /// </summary>
        public double BiomassCoefficientExtraroot
        {
            get { return _biomassCoefficientExtraroot; }
            set { SetProperty(ref _biomassCoefficientExtraroot, value); }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenContentInProduct
        {
            get { return _nitrogenContentInProduct; }
            set { SetProperty(ref _nitrogenContentInProduct, value); }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenContentInStraw
        {
            get { return _nitrogenContentInStraw; }
            set { SetProperty(ref _nitrogenContentInStraw, value); }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenContentInRoots
        {
            get { return _nitrogenContentInRoots; }
            set { SetProperty(ref _nitrogenContentInRoots, value); }
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double NitrogenContentInExtraroot
        {
            get { return _nitrogenContentInExtraroot; }
            set { SetProperty(ref _nitrogenContentInExtraroot, value); }
        }

        /// <summary>
        /// The amount of manure applied to the crop.
        /// 
        /// kg
        /// </summary>
        public double AmountOfManureApplied
        {
            get { return _amountOfManureApplied; }
            set { SetProperty(ref _amountOfManureApplied, value); }
        }

        public ManureStateType ManureStateType
        {
            get { return _manureStateType; }
            set { SetProperty(ref _manureStateType, value); }
        }

        public DefaultManureCompositionData ManureCompositionData { get; set; }

        public FallowTypes SelectedFallowType
        {
            get { return _selectedFallowType; }
            set { SetProperty(ref _selectedFallowType, value); }
        }

        private TillageType CurrentTillageType
        {
            get { return _currentTillageType; }
            set { SetProperty(ref _currentTillageType, value); }
        }

        public TillageType PastTillageType
        {
            get { return _pastTillageType; }
            set { SetProperty(ref _pastTillageType, value); }
        }

        /// <summary>
        /// Used for both perennials and grassland
        /// </summary>
        [Obsolete("Not used anymore, use YearOfConversion to capture the year of initial seeding since that is what is used in single year C calculations")]
        public int YearOfSeeding
        {
            get { return _yearOfSeeding; }
            set { SetProperty(ref _yearOfSeeding, value); }
        }

        public bool IsNativeGrassland
        {
            get { return _isNativeGrassland; }
            set { SetProperty(ref _isNativeGrassland, value); }
        }

        /// <summary>
        /// Indicates if the growing area was changed from a grassland to a cropping system (e.g. wheat)
        /// </summary>
        public bool IsBrokenGrassland
        {
            get => _isBrokenGrassland;
            set => SetProperty(ref _isBrokenGrassland, value);
        }

        /// <summary>
        /// Atmospheric nitrogen deposition
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double NitrogenDepositionAmount
        {
            get
            {
                return _nitrogenDepositionAmount;
            }

            set
            {
                SetProperty(ref _nitrogenDepositionAmount, value);
            }
        }

        /// <summary>
        /// C_ptoSoil (kg ha^-1)
        /// </summary>
        public double CarbonInputFromProduct
        {
            get { return _carbonInputFromProduct; }
            set { SetProperty(ref _carbonInputFromProduct, value); }
        }

        /// <summary>
        /// C_p (kg ha^-1)
        /// </summary>
        public double PlantCarbonInAgriculturalProduct
        {
            get { return _plantCarbonInAgriculturalProduct; }
            set { SetProperty(ref _plantCarbonInAgriculturalProduct, value); }
        }

        /// <summary>
        /// Used to prevent a custom C_p value from being overwritten from the usual method of calculation for C_p (used with perennials only)
        /// </summary>
        public bool DoNotRecalculatePlantCarbonInAgriculturalProduct { get; set; }

        /// <summary>
        /// C_s (kg ha^-1)
        /// </summary>
        public double CarbonInputFromStraw
        {
            get { return _carbonInputFromStraw; }
            set { SetProperty(ref _carbonInputFromStraw, value); }
        }

        /// <summary>
        /// C_r (kg ha^-1)
        /// </summary>
        public double CarbonInputFromRoots
        {
            get { return _carbonInputFromRoots; }
            set { SetProperty(ref _carbonInputFromRoots, value); }
        }

        /// <summary>
        /// C_e (kg ha^-1)
        /// </summary>
        public double CarbonInputFromExtraroots
        {
            get { return _carbonInputFromExtraroots; }
            set { SetProperty(ref _carbonInputFromExtraroots, value); }
        }

        /// <summary>
        /// Used to determine how much N fertilizer is required for a given user specified yield
        /// 
        /// (Kg N ha⁻¹)
        /// </summary>
        public double SoilTestNitrogen
        {
            get { return _soilTestNitrogen; }
            set { SetProperty(ref _soilTestNitrogen, value); }
        }

        public double NitrogenFertilizerRateOnStubble
        {
            get { return _nitrogenFertilizerRateOnStubble; }
            set { SetProperty(ref _nitrogenFertilizerRateOnStubble, value); }
        }

        public double NitrogenFertilizerRateOnFallow
        {
            get { return _nitrogenFertilizerRateOnFallow; }
            set { SetProperty(ref _nitrogenFertilizerRateOnFallow, value); }
        }

        /// <summary>
        /// Improved grassland/pasture is pasture that is fertilized and/or irrigated.
        /// </summary>
        public bool IsImprovedGrassland
        {
            get
            {
                return this.CropType.IsPerennial() && (this.AmountOfIrrigation > 0 || this.NitrogenFertilizerRate > 0 || this.PhosphorusFertilizerRate > 0);
            }
        }

        /// <summary>
        /// Expressed as a ratio (fraction)
        /// </summary>
        public double NitrogenFertilizerEfficiency
        {
            get { return this.NitrogenFertilizerEfficiencyPercentage / 100; }
        }

        /// <summary>
        /// Expressed as a percentage (%)
        /// </summary>
        public double NitrogenFertilizerEfficiencyPercentage
        {
            get { return _nitrogenFertilizerEfficiencyPercentage; }
            set { SetProperty(ref _nitrogenFertilizerEfficiencyPercentage, value); }
        }

        /// <summary>
        /// Maximum C produced by seeding grassland (g m^-2)
        /// </summary>
        public double LumCMax
        {
            get { return _lumCMax; }
            set { SetProperty(ref _lumCMax, value); }
        }

        /// <summary>
        /// Rate constant
        /// </summary>
        public double KValue
        {
            get { return _kValue; }
            set { SetProperty(ref _kValue, value); }
        }

        /// <summary>
        /// Used to determine how much N fertilizer is required for a given user specified yield
        /// 
        /// (fraction)
        /// </summary>
        public double NitrogenFixation
        {
            get { return _nitrogenFixation; }
            set { SetProperty(ref _nitrogenFixation, value); }
        }

        /// <summary>
        /// (GJ ha^-1)
        /// </summary>
        public double FuelEnergy
        {
            get { return _fuelEnergy; }
            set { SetProperty(ref _fuelEnergy, value); }
        }

        /// <summary>
        /// (GJ ha^-1)
        /// </summary>        
        public double HerbicideEnergy
        {
            get { return _herbicideEnergy; }
            set { SetProperty(ref _herbicideEnergy, value); }
        }

        /// <summary>
        /// When enabled, user provided defaults will be used when adding a new crop to a field (i.e. system defaults will not be used). 
        /// </summary>
        public bool EnableCustomUserDefaultsForThisCrop
        {
            get => _enableCustomUserDefaultsForThisCrop;
            set => SetProperty(ref _enableCustomUserDefaultsForThisCrop, value);
        }

        public CropEconomicData CropEconomicData
        {
            get => _cropEconomicData;
            set => SetProperty(ref _cropEconomicData, value);
        }

        /// <summary>
        /// Used to capture the year when either a perennial was changed to an annual (broken grassland) or an annual was changed to a perennial (seeded)
        /// </summary>
        public int YearOfConversion
        {
            get => _yearOfConversion;
            set => SetProperty(ref _yearOfConversion, value);
        }

        public int YearsSinceYearOfConversion
        {
            get
            {
                return this.Year - this.YearOfConversion;
            }
        }

        public double PastFallowArea
        {
            get => _pastFallowArea;
            set => SetProperty(ref _pastFallowArea, value);
        }

        public double PastPerennialArea
        {
            get => _pastPerennialArea;
            set => SetProperty(ref _pastPerennialArea, value);
        }

        public int YearOfFallowChange
        {
            get => _yearOfFallowChange;
            set => SetProperty(ref _yearOfFallowChange, value);
        }

        public int YearOfTillageChange
        {
            get => _yearOfTillageChange;
            set => SetProperty(ref _yearOfTillageChange, value);
        }

        /// <summary>
        /// Total carbon inputs
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double TotalCarbonInputs { get; set; }

        /// <summary>
        /// C_ag 
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double AboveGroundCarbonInput
        {
            get { return _aboveGroundCarbonInput; }
            set { SetProperty(ref _aboveGroundCarbonInput, value); }
        }

        /// <summary>
        /// C_bg 
        /// 
        /// (kg C ha^-1)
        /// </summary>
        public double BelowGroundCarbonInput
        {
            get { return _belowGroundCarbonInput; }
            set { SetProperty(ref _belowGroundCarbonInput, value); }
        }

        public int SizeOfFirstRotationForField { get; set; }

        /// <summary>
        /// The <see cref="Guid"/> of the <see cref="FieldSystemComponent"/> this view item belongs to.
        /// </summary>
        public Guid FieldSystemComponentGuid { get; set; }

        public string TimePeriodCategoryString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_timePeriodCategoryString) == false)
                {
                    return _timePeriodCategoryString;
                }
                else
                {
                    return TimePeriodCategory.Current.GetDescription();
                }
            }
            set
            {
                SetProperty(ref _timePeriodCategoryString, value);
            }
        }

        public double ClimateParameter
        {
            get { return _climateParameter; }
            set { SetProperty(ref _climateParameter, value); }
        }

        public double ManagementFactor
        {
            get { return _managementFactor; }
            set { SetProperty(ref _managementFactor, value); }
        }

        /// <summary>
        /// (kg C year^-1)
        ///
        /// Total manure C for entire year and for entire field
        /// </summary>
        public double ManureCarbonInput
        {
            get { return _manureCarbonInput; }
            set { SetProperty(ref _manureCarbonInput, value); }
        }

        /// <summary>
        /// (kg C ha^-1)
        ///
        /// Total manure C from all manure applications
        /// </summary>
        public double ManureCarbonInputsPerHectare
        {
            get { return _manureCarbonPerHectare; }
            set { SetProperty(ref _manureCarbonPerHectare, value); }
        }

        public double TillageFactor
        {
            get { return _tillageFactor; }
            set { SetProperty(ref _tillageFactor, value); }
        }

        public string ManagementPeriodName
        {
            get => _managementPeriodName;
            set => SetProperty(ref _managementPeriodName, value);
        }

        public NitrogenFertilizerType NitrogenFertilizerType
        {
            get => _nitrogenFertilizerType;
            set => SetProperty(ref _nitrogenFertilizerType, value);
        }

        public FertilizerApplicationMethodologies FertilizerApplicationMethodology
        {
            get => _fertilizerApplicationMethodology;
            set => SetProperty(ref _fertilizerApplicationMethodology, value);
        }

        public Seasons SeasonOfFertilizerApplication
        {
            get => _seasonOfFertilizerApplication;
            set => SetProperty(ref _seasonOfFertilizerApplication, value);
        }

        public string CropTypeStringWithYear
        {
            get => _cropTypeStringWithYear;
            set => SetProperty(ref _cropTypeStringWithYear, value);
        }

        public string TillageTypeString
        {
            get => _tillageTypeString;
            set => SetProperty(ref _tillageTypeString, value);
        }

        public bool HasHarvestViewItems
        {
            get => _hasHarvestViewItems;
            set => SetProperty(ref _hasHarvestViewItems, value);
        }

        public bool HasHayImportViewItems
        {
            get => _hasHayImportViewItems;
            set => SetProperty(ref _hasHayImportViewItems, value);
        }

        public bool HasGrazingViewItems
        {
            get => _hasGrazingViewItems;
            set => SetProperty(ref _hasGrazingViewItems, value);
        }

        public CoverCropTerminationType CoverCropTerminationType
        {
            get => _coverCoverCropTerminationType;
            set => SetProperty(ref _coverCoverCropTerminationType, value);
        }

        public bool HasManureApplicationViewItems
        {
            get => _hasManureApplicationViewItems;
            set => SetProperty(ref _hasManureApplicationViewItems, value);
        }

        /// <summary>
        /// Used to determine which component selection view item was used to create a detail view item
        /// </summary>
        public Guid DetailViewItemToComponentSelectionViewItemMap
        {
            get
            {
                return _detailViewItemToComponentSelectionViewItemMap;

            }
            set
            {
                SetProperty(ref _detailViewItemToComponentSelectionViewItemMap, value);
            }
        }

        /// <summary>
        /// Indicates if the view item represents a cover/winter or undersown crop
        /// </summary>
        public bool IsSecondaryCrop
        {
            get => _isSecondaryCrop;
            set => SetProperty(ref _isSecondaryCrop, value);
        }

        /// <summary>
        /// Lignin content of carbon input 
        /// 
        /// (unitless)
        /// </summary>
        public double LigninContent
        {
            get => _ligninContent;
            set => SetProperty(ref _ligninContent, value);
        }

        /// <summary>
        /// Grazing utilization rate depends on the type of forage rather than the type of grazing system or number of animals
        ///
        /// (%)
        /// </summary>
        public double ForageUtilizationRate
        {
            get => _forageUtilizationRate;
            set => SetProperty(ref _forageUtilizationRate, value);
        }

        public double CombinedAboveGroundInput { get; set; }
        public double CombinedBelowGroundInput { get; set; }
        public double CombinedManureInput { get; set; }

        #endregion

        #region Public Methods

        public void CalculateDryYield()
        {
            this.DryYield = this.Yield * (1 - this.MoistureContentOfCrop);
        }

        public void CalculateWetWeightYield()
        {
            this.Yield = this.DryYield / (1 - this.MoistureContentOfCrop);
        }

        /// <summary>
        /// This is the total amount of both organic fertilizer nitrogen and manure nitrogen.
        /// </summary>
        public double GetTotalOrganicAndManureNitrogenInYear()
        {
            var result = this.GetTotalOrganicNitrogenInYear() + this.GetTotalManureNitrogenAppliedFromLivestockInYear();

            return result;
        }

        /// <summary>
        /// This is not manure, but organic fertilizers.
        ///
        /// (kg N)
        /// </summary>
        /// <returns></returns>
        public double GetTotalOrganicNitrogenInYear()
        {
            var totalNitrogen = 0d;

            foreach (var fertilizerApplicationViewItem in this.FertilizerApplicationViewItems.Where(x => x.FertilizerBlendData.FertilizerBlend == FertilizerBlends.CustomOrganic))
            {
                totalNitrogen += fertilizerApplicationViewItem.AmountOfNitrogenApplied * this.Area;
            }

            return totalNitrogen;
        }

        /// <summary>
        /// (kg N)
        /// </summary>
        public double GetTotalManureNitrogenAppliedFromLivestockInYear()
        {
            var totalNitrogen = 0d;

            foreach (var manureApplication in this.ManureApplicationViewItems.Where(manureViewItem => manureViewItem.DateOfApplication.Year == this.Year &&
                         manureViewItem.ManureLocationSourceType == ManureLocationSourceType.Livestock))
            {
                totalNitrogen += manureApplication.AmountOfNitrogenAppliedPerHectare * this.Area;
            }

            return totalNitrogen;
        }

        /// <summary>
        /// Get all manure applications made on all fields on this date using the specified type of manure
        /// </summary>
        public IEnumerable<ManureApplicationViewItem> GetManureApplicationsFromLivestock(AnimalType animalType, DateTime dateOfManureApplication)
        {
            var result = new List<ManureApplicationViewItem>();

            foreach (var manureApplicationViewItem in this.ManureApplicationViewItems)
            {
                if (manureApplicationViewItem.AnimalType == animalType &&
                    manureApplicationViewItem.DateOfApplication.Equals(dateOfManureApplication) &&
                    manureApplicationViewItem.ManureLocationSourceType == ManureLocationSourceType.Livestock)
                {
                    result.Add(manureApplicationViewItem);
                }
            }

            return result;
        }

        /// <summary>
        /// (kg C year^-1)
        /// </summary>
        public double GetTotalCarbonFromAppliedManure()
        {
            return this.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Livestock) + this.GetTotalCarbonFromAppliedManure(ManureLocationSourceType.Imported);
        }

        /// <summary>
        /// (kg C year^-1)
        /// </summary>
        public double GetTotalCarbonFromAppliedManure(ManureLocationSourceType manureLocationSourceType)
        {
            var manureApplications = this.GetManureApplicationsInYear(manureLocationSourceType);
            var result = this.CalculateTotalCarbonFromManureApplications(manureApplications);

            return result;
        }

        public IEnumerable<ManureApplicationViewItem> GetManureApplicationsInYear(ManureLocationSourceType manureLocationSourceType)
        {
            return this.ManureApplicationViewItems.Where(manureApplicationViewItem => manureApplicationViewItem.ManureLocationSourceType == manureLocationSourceType && manureApplicationViewItem.DateOfApplication.Year == this.Year);
        }

        /// <summary>
        /// (kg C year^-1)
        /// </summary>
        public double CalculateTotalCarbonFromManureApplications(IEnumerable<ManureApplicationViewItem> manureApplicationViewItems)
        {
            var result = 0d;

            foreach (var manureApplication in manureApplicationViewItems)
            {
                var carbonFraction = manureApplication.DefaultManureCompositionData.CarbonContent;
                var volumeOfManure = manureApplication.AmountOfManureAppliedPerHectare;
                var area = this.Area;

                result += carbonFraction * volumeOfManure * area;
            }

            return result;
        }

        #endregion

        #region Private Methods



        #endregion

        #region Event Handlers

        private void GrazingViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasGrazingViewItems = this.GrazingViewItems.Count > 0;
        }

        private void ManureApplicationViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasManureApplicationViewItems = this.ManureApplicationViewItems.Count > 0;
        }

        private void HayImportViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasHayImportViewItems = this.HayImportViewItems.Count > 0;
        }

        private void HarvestViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasHarvestViewItems = this.HarvestViewItems.Count > 0;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        private void OnIsIrrigatedChanged()
        {
            this.IrrigationType = this.IsIrrigated == Response.Yes ? IrrigationType.Irrigated : IrrigationType.RainFed;
        }

        private void OnHarvestMethodChanged()
        {
            this.HarvestMethodString = this.HarvestMethod.GetDescription();
        }

        private void OnTillageTypeChanged()
        {
            this.TillageTypeString = this.TillageType.GetDescription();
        }

        private void OnCropTypeChanged()
        {
            this.CropTypeString = this.CropType.GetDescription();
            this.CropTypeStringWithYear = $"[{this.Year}] - {this.CropType.GetDescription()}";

            // If user changes from a perennial a non-perennial must reset this property to false
            if (this.CropType.IsPerennial() == false)
            {
                this.UnderSownCropsUsed = false;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {CropTypeString}, {nameof(FieldName)}: {FieldName}, {nameof(Year)}: {Year}";
        }

        #endregion
    }
}